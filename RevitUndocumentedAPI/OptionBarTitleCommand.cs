using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using UIFrameworkServices;

namespace RevitUndocumentedAPI
{
    [Transaction(TransactionMode.Manual)]
    class OptionBarTitleCommand : IExternalCommand
    {
        unsafe public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var dynamicLabelDialogBar = getDynamicLabelDialogBar();

            if (dynamicLabelDialogBar == null)
            {
                createDynamicLabelDialogBar(null);
                dynamicLabelDialogBar = getDynamicLabelDialogBar();
            }

            var sPtr = Marshal.StringToCoTaskMemUni("Test");
            AString astring2 = new AString() { strPtr = sPtr };
            setOptionBarTitle(dynamicLabelDialogBar, &astring2);

            AString astring;
            getOptionBarTitle(dynamicLabelDialogBar, &astring);
            string s = Marshal.PtrToStringUni(astring.strPtr);

            return Result.Succeeded;
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\DesktopMFC.dll",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true,
            EntryPoint = "?getDynamicLabelDialogBar@DynamicLabelDlgBar@@SAPEAV1@XZ")]
        internal unsafe static extern DynamicLabelDlgBar* getDynamicLabelDialogBar();

        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\DesktopMFC.dll",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true,
            EntryPoint = "?createDynamicLabelDialogBar@DynamicLabelDlgBar@@SAXPEAVADialogBarHost@@@Z")]
        internal unsafe static extern void createDynamicLabelDialogBar(ADialogBarHost* aDialogBarHost);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\DesktopMFC.dll",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true,
            EntryPoint = "?getOptionBarTitle@DynamicLabelDlgBar@@QEAA?AVAString@@XZ")]
        internal unsafe static extern AString* getOptionBarTitle(DynamicLabelDlgBar* dynamicLabelDlgBar, AString* astring);


        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\DesktopMFC.dll",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true,
            EntryPoint = "?setOptionBarTitle@DynamicLabelDlgBar@@QEAAXAEBVAString@@@Z")]
        internal unsafe static extern void setOptionBarTitle(DynamicLabelDlgBar* dynamicLabelDlgBar, AString* astring);


        [UnsafeValueType]
        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential, Size = 744)]
        internal struct DynamicLabelDlgBar { }

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential, Size = 8)]
        internal struct ADialogBarHost { }

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential, Size = 8)]
        internal struct AString
        {
            public IntPtr strPtr;
        }
    }
}
