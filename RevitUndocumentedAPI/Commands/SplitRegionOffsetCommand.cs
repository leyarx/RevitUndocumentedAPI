using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUndocumentedAPI.Common;
using RevitUndocumentedAPI.Common.Controls;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace RevitUndocumentedAPI.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class SplitRegionOffsetCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var doc = uiapp.ActiveUIDocument.Document;
            var uidoc = uiapp.ActiveUIDocument;

            try
            {
                var viewCropRegionRef = uidoc.Selection.PickObject(ObjectType.Element, new ViewportSelectionFilter(), "Select View Crop Region");
                var viewCropRegion = doc.GetElement(viewCropRegionRef.ElementId);
                var viewId = viewCropRegion.GetDependentElements(
                    new LogicalOrFilter(new ElementClassFilter(typeof(ViewPlan)), new ElementClassFilter(typeof(ViewSection))))
                    .FirstOrDefault();
                if (viewId != null)
                {
                    var view = doc.GetElement(viewId) as View;
                    if (view != null)
                    {
                        var shapeManager = view.GetCropRegionShapeManager();

                        if(shapeManager.NumberOfSplitRegions > 1)
                        {
                            var value = InputDialog.Show("Offset", "Set Crop Region Offset");
                            if (value != null && value.HasValue)
                            {
                                var ptr = GetCropRegionShapeManagerPtr(shapeManager);

                                var regionIndex = 0;
                                using (Transaction tx = new Transaction(doc, "Move Split Region"))
                                {
                                    tx.Start();
                                    MoveRegionAlongBasis(ptr, regionIndex, value.Value);
                                    tx.Commit();
                                }
                                uidoc.RefreshActiveView();
                            }
                        }
                        else
                        {
                            Autodesk.Revit.UI.TaskDialog.Show("Warning", "The crop region is not splitted. Please split it first.", TaskDialogCommonButtons.Ok);
                        }
                    }
                }
            }
            catch
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("Utility.dll",
            EntryPoint = "?ptr@ProxyLink@@QEBAPEAVReferenceableClass@@XZ",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ProxyLink_ptr(IntPtr thisPtr);

        [DllImport("Utility.dll",
            EntryPoint = "?validPtr@ProxyLink@@QEBAPEAVReferenceableClass@@XZ",
            CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr ProxyLink_validPtr(IntPtr thisPtr);

#if REVIT2022
        [DllImport("RevitDB.dll",
            EntryPoint = "?isNonRectangularCropAreaPresent@Viewer@@QEBA_NPEAPEBVISketch@@@Z",
            CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern bool Viewer_isNonRectangularCropAreaPresent(
            IntPtr viewerPtr,
            IntPtr* outSketch);
#elif REVIT2025 || REVIT2026
        [DllImport("RevitDB.dll",
            EntryPoint = "?isNonRectangularCropAreaPresent@Viewer@@QEBA_NXZ",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Viewer_isNonRectangularCropAreaPresent(IntPtr thisPtr);
#endif

        [DllImport("RevitDB.dll",
            EntryPoint = "?moveViewRegionOffset@Viewer@@QEAAXHQEBNAEAVViewLayout@@@Z",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void Viewer_moveViewRegionOffset(
            IntPtr viewerPtr,   // Viewer*
            int regionIndex,
            double* moveVector, // const double* (3 doubles)
            IntPtr viewLayout); // ViewLayout&

        [DllImport("RevitDB.dll",
            EntryPoint = "?getBoundedSpace@Viewer@@QEBAAEBVBoundedSpace@@XZ",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Viewer_getBoundedSpace(IntPtr viewerPtr);

        [DllImport("RevitDB.dll",
            EntryPoint = "?getViewLayout@Viewer@@QEBAAEBVViewLayout@@XZ",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Viewer_getViewLayout(IntPtr viewerPtr);

        [DllImport("RevitDB.dll",
            EntryPoint = "?getViewLayoutForWrite@Viewer@@QEAAAEAVViewLayout@@XZ",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Viewer_getViewLayoutForWrite(IntPtr viewerPtr);

        private static IntPtr GetViewer(IntPtr cropManagerPtr)
        {
            IntPtr proxyLink = cropManagerPtr + 8;

            // quick null check
            IntPtr raw = ProxyLink_ptr(proxyLink);
            if (raw == IntPtr.Zero) return IntPtr.Zero;
            IntPtr viewer = raw - 8;
            if (viewer == IntPtr.Zero) return IntPtr.Zero;

            // safe validated ptr
            IntPtr valid = ProxyLink_validPtr(proxyLink);
            if (valid == IntPtr.Zero) return IntPtr.Zero;

            return valid - 8;
        }

        public unsafe static void MoveRegionAlongBasis(
            IntPtr cropManagerPtr,
            int regionIndex,
            double scalar)           // positive = forward, negative = backward
        {
            IntPtr viewer = GetViewer(cropManagerPtr);
            if (viewer == IntPtr.Zero) return;

            IntPtr layout = Viewer_getViewLayoutForWrite(viewer);
            if (layout == IntPtr.Zero) return;

            // read basisIndex from layout+8
            int basisIndex = Marshal.ReadInt32(layout + 8);

            IntPtr boundedSpace = Viewer_getBoundedSpace(viewer);

            // each XYZ = 3 doubles = 24 bytes
            IntPtr basisPtr = boundedSpace + (basisIndex + 1) * 3 * 8;
            XYZUtils_XYZ* basis = (XYZUtils_XYZ*)basisPtr;

            // build move vector along that axis only
            double dx = basis->X * scalar;
            double dy = basis->Y * scalar;
            double dz = basis->Z * scalar;

            double* vec = stackalloc double[3];
            vec[0] = dx;
            vec[1] = dy;
            vec[2] = dz;

            Viewer_moveViewRegionOffset(viewer, regionIndex, vec, layout);
        }

        public unsafe IntPtr GetCropRegionShapeManagerPtr(ViewCropRegionShapeManager viewCropRegionShapeManager)
        {
            var shapeManagerMProxyField = viewCropRegionShapeManager.GetType()
                .GetField("m_proxy", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var shapeManagerMProxy = shapeManagerMProxyField.GetValue(viewCropRegionShapeManager);

            var shapeManagerProxyMpOwnedObjectField = shapeManagerMProxy.GetType()
                .GetField("m_pOwnedObject", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var shapeManagerProxyMpOwnedObject = shapeManagerProxyMpOwnedObjectField.GetValue(shapeManagerMProxy);

            return (IntPtr)Pointer.Unbox(shapeManagerProxyMpOwnedObject);
        }

        [StructLayout(LayoutKind.Sequential, Size = 24)]
        public struct XYZUtils_XYZ
        {
            public double X;
            public double Y;
            public double Z;
        }
    }
}
