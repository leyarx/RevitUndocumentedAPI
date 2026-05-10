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
                                MoveRegionAlongBasis(ptr, regionIndex, value.Value);
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

        [DllImport("RevitDB.dll", //C:\\Program Files\\Autodesk\\Revit 2025\\
            EntryPoint = "?moveViewRegionOffset@Viewer@@QEAAXHQEBNAEAVViewLayout@@@Z",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void Viewer_moveViewRegionOffset(
            IntPtr viewerPtr,   // Viewer*
            int regionIndex,
            double* moveVector, // const double* (3 doubles)
            IntPtr viewLayout); // ViewLayout&

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

        private unsafe static IntPtr GetLayoutPtr(IntPtr cropManagerPtr, IntPtr viewer)
        {
            var sketch = IntPtr.Zero;
            bool isNonRect = Viewer_isNonRectangularCropAreaPresent(viewer
#if REVIT2022
                , &sketch
#endif     
                );

            if (isNonRect)
            {
                // static global Owner<ViewLayout> — read inner ptr
                IntPtr sym = GetProcAddress(
                    GetModuleHandle("RevitDB.dll"),
                    "?s_defaultViewLayout@Viewer@@0V?$Owner@$$CBVViewLayout@@@@A");
                return Marshal.ReadIntPtr(sym);
            }
            else
            {
                // instance layout at Viewer+0x1E8
                return Marshal.ReadIntPtr(viewer +
#if REVIT2022
                    0x1B0
#elif REVIT2025 || REVIT2026
                    0x1E8
#endif
                    );
            }
        }

        public unsafe static void MoveRegionAlongBasis(
            IntPtr cropManagerPtr,
            int regionIndex,
            double scalar)           // positive = forward, negative = backward
        {
            IntPtr viewer = GetViewer(cropManagerPtr);
            if (viewer == IntPtr.Zero) return;

            IntPtr layout = GetLayoutPtr(cropManagerPtr, viewer);
            if (layout == IntPtr.Zero) return;

            // read basisIndex from layout+8
            int basisIndex = Marshal.ReadInt32(layout + 8);

            // build move vector along that axis only
            double dx = basisIndex == 0 ? scalar : 0.0;
            double dy = basisIndex == 1 ? scalar : 0.0;
            double dz = basisIndex == 2 ? scalar : 0.0;

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
    }
}
