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
using UIFramework;
using UIFrameworkServices;

namespace RevitUndocumentedAPI
{
    [UnsafeValueType]
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential, Size = 744)]
    internal struct DynamicLabelDlgBar
    {

    }


    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential, Size = 232)]
    internal struct CWnd
    {
        /*
        private IntPtr a0;
        private IntPtr a8;
        private IntPtr a16;
        private IntPtr a24;
        private IntPtr a32;
        private IntPtr a40;
        private IntPtr a48;
        private IntPtr a56;
        */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private IntPtr[] align;
        public IntPtr m_hWnd;
        // Token: 0x040002FA RID: 762
        //private long <alignment\u0020member>;
    }

    [NativeCppClass]
    [UnsafeValueType]
    [StructLayout(LayoutKind.Sequential, Size = 1448)]
    internal unsafe struct FamilyDialogBarImpl
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 181)]
        public long[] intPtrs;
    }

    [UnsafeValueType]
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential, Size = 744)]
    internal struct ADialogBarWrapper
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 81)]
        private IntPtr[] align;
        public IntPtr dialogCore;
    }


    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    internal struct AString
    {
        public IntPtr strPtr;
    }

    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    internal struct ADialogBarHost { }


    [NativeCppClass]
    [UnsafeValueType]
    [StructLayout(LayoutKind.Sequential, Size = 744)]
    internal struct ADialogBar
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 93)]
        private IntPtr[] align;
        //648 ADialogCore
        //736 bool (TemporaryValueSetter)
    }



    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        [DllImport("user32.dll")]
        public static extern int SendMessageW(IntPtr hWnd, uint wMsg, ulong wParam, long lParam);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\DesktopMFC.dll", 
            CallingConvention = CallingConvention.Cdecl, 
            SetLastError = true, 
            EntryPoint = "?dialogCore@ADialogCore@@QEBAPEAVMFCDialogCore@@XZ")]
        //[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
        internal unsafe static extern IntPtr dialogCore(IntPtr ADialogCore);


        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\DesktopMFC.dll", 
            CallingConvention = CallingConvention.Cdecl, 
            SetLastError = true,
            EntryPoint = "?replayMessage@MFCDialogCore@@QEAA_JI_K_J@Z")]
        //[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
        internal unsafe static extern long replayMessage(IntPtr MFCDialogCore, uint wMsg, ulong wParam, long lParam);


        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files\Autodesk\Revit 2022\DesktopMFC.dll",
            CallingConvention = CallingConvention.Cdecl,
            SetLastError = true,
            EntryPoint = "?getFirstOptionBar@ADialogBar@@QEAAPEAV1@XZ")]
        //[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
        internal unsafe static extern IntPtr getFirstOptionBar();

        unsafe public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {


            /*
            var id = DialogBarService.getControlId("Dialog_HostObj_MeasurePropDbar");


            var findDialogBarMethod =
                typeof(DialogBarService).GetMethod("findDialogBar", BindingFlags.NonPublic | BindingFlags.Static);


            var dialog = findDialogBarMethod.Invoke(null, new object[] { "Dialog_Revit_DynamicLabelDBar" });


            //var command = DialogBarService.simulateCommandOnBar("Dialog_Revit_EditLoadedFamilyDbar", "Control_Revit_EditLoadedFam");

            //var labelDBar = getDynamicLabelDialogBar();

            DialogBarService.setOptionBarTitle("Test");
            */
            var wnd = UIFramework.MainWindow.getMainWnd();

            var bar = getFirstOptionBar();

            ADialogBar aDialogBar = (ADialogBar)Marshal.PtrToStructure(bar, typeof(ADialogBar));



            /*
            wnd.ShowOptionBar(true);

            var m_optionBarField = typeof(MainWindow).GetField("m_optionBar", BindingFlags.NonPublic | BindingFlags.Instance);

            var m_optionBar = m_optionBarField.GetValue(wnd) as DialogBarControl;

            var m_hostField = typeof(DialogBarControl).GetField("m_host", BindingFlags.NonPublic | BindingFlags.Instance);

            var m_host = m_hostField.GetValue(m_optionBar) as ControlHost;

            */



            /*
            wnd.SetOptionBar(m_host.m_win32Hwnd, true);

            var findFamilyDialogBarMethod = 
                typeof(TypeSelectorService).GetMethod("findFamilyDialogBar", BindingFlags.Public | BindingFlags.Static);

            var familyDialogBarImpl = findFamilyDialogBarMethod.Invoke(null, new object[] { });


            IntPtr fDBarPtr = (IntPtr)Pointer.Unbox(familyDialogBarImpl);


            var fDBar = (FamilyDialogBarImpl)Marshal.PtrToStructure(fDBarPtr, typeof(FamilyDialogBarImpl));


            var searchTexts = TypeSearchTexts.getSearchTexts();

            var dBarPanels = UIFramework.ContextualTabs.DBarPanels;

            Assembly asm = Assembly.LoadFrom(@"C:\Program Files\Autodesk\Revit 2022\UIFrameworkInterop.dll");

            var types = asm.GetTypes();
            var entry = asm.EntryPoint;

            //RbsSystemDialogBarService.RbsSystemDialogBarRibbonTabActive("MechanicalSystemPanel");



            var isCheked = UIFrameworkServices.DialogBarService.isChecked("Dialog_BuildingSystems_RbsLayoutOptionsBar", "Control_BuildingSystems_MaintainSlope");

            */
            /*
            var findControlMethod =
                typeof(DialogBarService).GetMethod("findControl", BindingFlags.NonPublic | BindingFlags.Static);

            var ptr = findControlMethod.Invoke(null, new object[] { "Dialog_Revit_FamilyBar", "Control_Revit_PropertyPanelTypePropBtn" });

            IntPtr cWndPtr = (IntPtr)Pointer.Unbox(ptr);

            CWnd cWnd = (CWnd)Marshal.PtrToStructure(cWndPtr, typeof(CWnd));
            */
            /*
            var ptr2B = findDialogBarMethod.Invoke(null, new object[] { "Dialog_Revit_FamilyBar" });
            var ptr2 = (IntPtr)Pointer.Unbox(ptr2B);

            ADialogBarWrapper aDialogBarWrapper = (ADialogBarWrapper)Marshal.PtrToStructure(ptr2, typeof(ADialogBarWrapper));

            IntPtr ptr4 = dialogCore(aDialogBarWrapper.dialogCore);

            uint controlId = DialogBarService.getControlId("Control_Revit_PropertyPanelTypePropBtn");
            */
            /*
            SendMessageW(cWnd.m_hWnd, 513U, 1UL, 65537L);   // WM_LBUTTONDOWN 
            SendMessageW(cWnd.m_hWnd, 514U, 1UL, 65537L);   // WM_LBUTTONUP 
            */
            //var r3 = SendMessageW(obj.m_hWnd, 242U, 0UL, 0L);   // BM_GETSTATE






            //ManageViewsService.onWndMsg(1);


            //DialogBarService.simulateCommandOnBar("Dialog_Revit_FamilyBar", "Control_Revit_PropertyPanelTypePropBtn");



            //var pointer = SendMessageW(obj.m_hWnd, 242, IntPtr.Zero, IntPtr.Zero);




            //HWND__* ptr3 = *(long*)(ptr + 64L / (long)sizeof(CWnd));






            //SendMessageW(, 242, 0 , 0)


            //(< Module >.SendMessageW(*(long*)(ptr + 64L / (long)sizeof(CWnd)), 242U, 0UL, 0L) & 3) == 1;



            //CWnd* ptr = DialogBarService.findControl("Dialog_Revit_FamilyBar", "Control_Revit_PropertyPanelTypePropBtn");

            //var bTypes = types.Where(t => t.BaseType == null).ToList();



            //Autodesk.Windows.RibbonPanel ribbonPanel = contextualPanels[i];
            //string dbar = ContextFilter.GetDBar(ribbonPanel.Source);

            /*
            DialogBarService.setOptionBarTitle("Test");
            var bars = DialogBarService.getBarIdList();

            DialogBarService.simulateCommandOnBar("Dialog_HostObj_MeasurePropDbar", "Control_Essentials_ActiveoptionOnly");





            var cd = new UIFramework.ContextData() { 
                bConsumeBreadcrumb = true,
                contextType = ContextType.ToolCommand,
                productId = ProdId.Revit,
                selectedCategoryCount = CountType.CountZero,
                selectedElementCount = CountType.CountZero,
                selectedFamilyCount = CountType.CountZero,
                selectedTypeCount = CountType.CountZero,
                selSetStruct = SelSetStruct.SelMixedElems,
                strBarIdList = "Dialog_Revit_DynamicLabelDBar,Dialog_HostObj_LineStyleComboDbar,IDD_PLANE_SELECTION,Dialog_Essentials_CurveCreate,Dialog_Essentials_WallProp",
                strCommand = "ID_OBJECTS_PROJECT_CURVE",
                strEditMode = "IDR_COMMON",
                strIdenticalFamilyName = "",
                strProviderIdList = "",
                strTabName = "Create a straight line or an arc\nLines"
            };

            var cd2 = new UIFramework.ContextData()
            {
                bConsumeBreadcrumb = true,
                contextType = ContextType.ToolCommand,
                productId = ProdId.Revit,
                selectedCategoryCount = CountType.CountZero,
                selectedElementCount = CountType.CountZero,
                selectedFamilyCount = CountType.CountZero,
                selectedTypeCount = CountType.CountZero,
                selSetStruct = SelSetStruct.SelMixedElems,
                strBarIdList = "Dialog_Revit_DynamicLabelDBar,Dialog_Essentials_CurveCreate,Dialog_HostObj_MeasurePropDbar,Dialog_Essentials_WallProp",
                strCommand = "ID_MEASURE_LINE",
                strEditMode = "IDR_COMMON",
                strIdenticalFamilyName = "",
                strProviderIdList = "",
                strTabName = "Create a line\nMeasure - Line"
            };

            */

            //var wnd = UIFramework.MainWindow.getMainWnd();
            //RevitRibbonControl ribbonControl = UIFramework.RevitRibbonControl.RibbonControl;



            /*
            var buildContextualTabMethod = 
                typeof(RevitRibbonControl).GetMethod("buildContextualTab", BindingFlags.Public | BindingFlags.Instance);

            //buildContextualTabMethod.Invoke(ribbonControl, new object[] { cd2 });

            //ribbonControl.buildContextualTab(cd);


            
            var initDBarPanelMethod =
                typeof(RevitRibbonControl).GetMethod("initDBarPanel", BindingFlags.Public | BindingFlags.Instance);

            initDBarPanelMethod.Invoke(ribbonControl, new object[] { "Dialog_HostObj_MeasurePropDbar" });

            var updateDbarPanelsMethod =
                typeof(RevitRibbonControl).GetMethod("updateDbarPanels", BindingFlags.Public | BindingFlags.Instance);


            updateDbarPanelsMethod.Invoke(ribbonControl, new object[] { "Dialog_Revit_DynamicLabelDBar,Dialog_Essentials_CurveCreate,Dialog_HostObj_MeasurePropDbar,Dialog_Essentials_WallProp" });
            */


            /*
            var addInHouseDebugUIMethod =
                typeof(RevitRibbonControl).GetMethod("addInHouseDebugUI", BindingFlags.NonPublic | BindingFlags.Instance);

            addInHouseDebugUIMethod.Invoke(ribbonControl, new object[] { });
            */

            return Result.Succeeded;
        }


    }
}
