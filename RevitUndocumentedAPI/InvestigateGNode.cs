using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Proxy.DB;

namespace RevitUndocumentedAPI
{
    [Transaction(TransactionMode.Manual)]
    unsafe public class InvestigateGNode : IExternalCommand
    {

        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\Graphics.dll",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true,
            EntryPoint = "?getGInfo@GNode@@IEAAAEAVGInfo@@XZ")]
        //[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
        internal unsafe static extern IntPtr getGInfo(IntPtr gNode);

        //GNode::getInfoString(bool)

        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\GeomUtil.dll",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true,
            EntryPoint = "?getVecs@Plane@@QEBAXQEAN000@Z")]
        //[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
        internal unsafe static extern void getVecs(IntPtr plane, XYZ* x, XYZ* y, XYZ* z, XYZ* o);


        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\GeomUtil.dll",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true,
            EntryPoint = "?getVecs@Plane@@QEBAXQEAN000@Z")]
        //[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
        internal unsafe static extern IntPtr getNormalProperty(IntPtr plane, long* xyz); //Plane*

        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\GeomUtil.dll",
            CallingConvention = CallingConvention.Cdecl,
            //SetLastError = true,
            EntryPoint = "?getOriginProperty@Plane@@QEBA?AVXYZ@XYZUtils@@XZ")]
        //[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
        internal unsafe static extern IntPtr getOriginProperty();

        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\GeomUtil.dll",
            CallingConvention = CallingConvention.Cdecl,
            //SetLastError = true,
            EntryPoint = "?getOrigin@Plane@@QEBAPEANQEAN@Z")]
        //[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
        internal unsafe static extern IntPtr getOrigin(IntPtr plane, XYZ* xyz);
        /*
        [SuppressUnmanagedCodeSecurity]
        [DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        //[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
        internal unsafe static extern double x(XYZUtils.XYZ*);
        */

        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\Graphics.dll",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true,
            EntryPoint = "?getBoundingBox@GNode@@QEBA_NPEAVOutline@@@Z")]
        //[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
        internal unsafe static extern bool getBoundingBox(IntPtr gNode, Outline* outline);

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            MethodInfo getGNodeMethod =
                typeof(GeometryObject).GetMethod("getGNode", BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo getGFaceMethod =
                typeof(Face).GetMethod("getGFace", BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo getPlaneMethod =
                typeof(PlanarFace).GetMethod("getPlane", BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo getSurfaceMethod =
                typeof(Face).GetMethod("getSurface", BindingFlags.NonPublic | BindingFlags.Instance);



            //var sr5 = Reference.ParseFromStableRepresentation(doc, "f36d0f7e-61f4-4b4c-8f2f-902cc3db031e-00000a38:2:SURFACE");
            //var sr5 = Reference.ParseFromStableRepresentation(doc, "b149e810-f78d-44d4-905f-99c8930db4b5-000010aa:0:INSTANCE:b149e810-f78d-44d4-905f-99c8930db4b5-0000109a:25:SURFACE");
            var sr5 = Reference.ParseFromStableRepresentation(doc, "b149e810-f78d-44d4-905f-99c8930db4b5-000010aa:0:INSTANCE:b149e810-f78d-44d4-905f-99c8930db4b5-0000109a:1:SURFACE");
            var gosr5 = doc.GetElement(sr5.ElementId).GetGeometryObjectFromReference(sr5);


            var point = uiapp.Application.Create.NewPointOnPlane(sr5, UV.Zero, UV.BasisU, 0);
            var point1 = uiapp.Application.Create.NewPointOnFace(sr5, UV.Zero);
            var point2 = uiapp.Application.Create.NewPointRelativeToPoint(sr5);

            /*
            var isValidPop = PointOnPlane.IsValidPlaneReference(doc, sr5);
            if(isValidPop)
            {
                var pop = PointOnPlane.NewPointOnPlane(doc, sr5, Autodesk.Revit.DB.XYZ.Zero, Autodesk.Revit.DB.XYZ.BasisX);
            }
            */



            var pointPR = point.GetPlaneReference();


            //var ds = DividedSurface.Create(doc, sr5);
            //var isValid = FaceWall.IsValidFaceReferenceForFaceWall(doc, sr5);

            /*
            SketchPlane sketchPlane;

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Testing");
                sketchPlane = SketchPlane.Create(doc, sr5);
                var pl = sketchPlane.GetPlane();
                trans.Commit();
            }

            */


            var gNodeP = getGNodeMethod.Invoke(gosr5, new object[] { }) as System.Reflection.Pointer;
            IntPtr gNodeIP = (IntPtr)Pointer.Unbox(gNodeP);
            GNode gNode = (GNode)Marshal.PtrToStructure(gNodeIP, typeof(GNode));

            //IntPtr planeIP = new IntPtr(gNode.intPtrs[10]);
            //Plane plane = (Plane)Marshal.PtrToStructure(planeIP, typeof(Plane));



            //var PlanarFaceCtor = typeof(PlanarFace).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[2];

            //var planarFace = (PlanarFace)PlanarFaceCtor.Invoke(new object[] { gNodeIP });

            //XYZ xyz;
            //long x;
            //getNormalProperty(planeIP, null);


            //getVecs(planeIP, 0, 0, 0, 0);

            /*
            var gFaceP = getGFaceMethod.Invoke(gosr5, new object[] { }) as System.Reflection.Pointer;
            IntPtr gFaceIP = (IntPtr)Pointer.Unbox(gFaceP);
            GFace gFace = (GFace)Marshal.PtrToStructure(gFaceIP, typeof(GFace));
            */


            //var planeP = getPlaneMethod.Invoke(gosr5, new object[] { }) as System.Reflection.Pointer;
            //IntPtr planeIP = (IntPtr)Pointer.Unbox(planeP);

            /*
            IntPtr planeIP = new IntPtr(gNode.intPtrs[10]);
            Plane plane = (Plane)Marshal.PtrToStructure(planeIP, typeof(Plane));

            IntPtr threeIP = new IntPtr(gNode.intPtrs[3]);
            GNode three = (GNode)Marshal.PtrToStructure(threeIP, typeof(GNode));

            XYZ origin;
            var p = getOrigin(planeIP, &origin);

            XYZ pOrigin = (XYZ)Marshal.PtrToStructure(p, typeof(XYZ));

            XYZ x, y, z, o;

            getVecs(planeIP, &x, &y, &z, &o);
            */

            Outline bb;
            var bbr = getBoundingBox(gNodeIP, &bb);

            /*
            var surfaceP = getSurfaceMethod.Invoke(gosr5, new object[] { }) as System.Reflection.Pointer;
            IntPtr surfaceIP = (IntPtr)Pointer.Unbox(surfaceP);
            Surface surface = (Surface)Marshal.PtrToStructure(surfaceIP, typeof(Surface));
            
            //(((uint)(*(global::< Module >.GNode.getGInfo(GNode) + 12L)) >> 5) & 15U)

            var gInfoIP = getGInfo(gNodeIP);
            GInfo gInfo = (GInfo)Marshal.PtrToStructure(gInfoIP, typeof(GInfo));
            */
            return Result.Succeeded;
        }

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential, Size = 240)] //24 + 240
        internal struct GNode
        {
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            //public long[] intPtrs;
            //public long intPtr;
            //public GInfo gInfo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)] //10
            public long[] intPtrs;
        }

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential, Size = 240)] //80
        internal struct GFace
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)] //10
            public long[] intPtrs;
        }

        [NativeCppClass]
        [UnsafeValueType]
        [StructLayout(LayoutKind.Sequential, Size = 56)]
        internal struct Surface
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public long[] intPtrs;
        }

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential, Size = 16)]
        internal struct GInfo
        {
            //public long Tag; //Id (8)
            //public int GStyleId; (4)
            // 17 Visible
            // 21 IsReference
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] intPtrs;
        }

        [NativeCppClass]
        [UnsafeValueType]
        [StructLayout(LayoutKind.Sequential, Size = 128)]
        internal struct Plane
        {
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            //public long[] intPtrs;
            Surface Surface;
            XYZ Origin; // [7-8-9] 
            XYZ XVector; // [10-11-12] 
            XYZ YVector; // [13-14-15] 
        }

        [UnsafeValueType]
        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential, Size = 24)]
        internal struct XYZ
        {
            double x;
            double y;
            double z;
            /*
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public long[] intPtrs;
            */
        }

        [NativeCppClass]
        [UnsafeValueType]
        [StructLayout(LayoutKind.Sequential, Size = 48)]
        internal struct Outline
        {
            XYZ min;
            XYZ max;
            /*
            long a1;
            long a2;
            long a3;
            long a4;
            long a5;
            long a6;*/
        }
    }
}
