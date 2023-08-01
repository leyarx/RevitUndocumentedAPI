using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UIFrameworkServices;

namespace RevitUndocumentedAPI
{
    [Transaction(TransactionMode.Manual)]
    class SimulateCommandOnBarCommand : IExternalCommand
    {
        unsafe public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var findControlMethod =
                typeof(DialogBarService).GetMethod("findControl", BindingFlags.NonPublic | BindingFlags.Static);

            var ptr = findControlMethod.Invoke(null, new object[] { "Dialog_Revit_FamilyBar", "Control_Revit_PropertyPanelTypePropBtn" });

            IntPtr cWndPtr = (IntPtr)Pointer.Unbox(ptr);

            CWnd cWnd = (CWnd)Marshal.PtrToStructure(cWndPtr, typeof(CWnd));

            SendMessageW(cWnd.m_hWnd, 513U, 1UL, 65537L);   // WM_LBUTTONDOWN 
            SendMessageW(cWnd.m_hWnd, 514U, 1UL, 65537L);   // WM_LBUTTONUP 

            return Result.Succeeded;
        }

        [DllImport("user32.dll")]
        public static extern int SendMessageW(IntPtr hWnd, uint wMsg, ulong wParam, long lParam);

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential, Size = 232)]
        internal struct CWnd
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            private IntPtr[] align;
            public IntPtr m_hWnd;
        }
    }
}
