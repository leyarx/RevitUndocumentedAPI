using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using UIFramework;

namespace RevitUndocumentedAPI.Common
{
    public static class CalculationsPanelHelper
    {
        public unsafe static CalculationsPanelWrapper? GetCalculationsPanelWrapper()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.FullName != null && a.FullName.Contains("UIFrameworkInterop"));
            if (assembly == null)
            {
                return null;
            }

            var module = assembly.GetModule("UIFrameworkInterop.dll");
            if (module == null)
            {
                return null;
            }

            var field = module.GetField("?s_paccessedBy_getCalculationsPanelWrapper@accessedBy_getCalculationsPanelWrapper@@0PEAV1@EA",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (field == null)
            {
                return null;
            }

            var value = field.GetValue(null);

            if (value == null)
            {
                return null;
            }

            var ptr = (IntPtr)Pointer.Unbox(value) + 24;

            var wrapper = Marshal.PtrToStructure(ptr, typeof(CalculationsPanelWrapper)); 

            if (wrapper == null)
            {
                return null;
            }

            return (CalculationsPanelWrapper)wrapper;
        }

        // This method return items only if Calculation Panel (Background Processes) is opened in Revit
        public static ObservableCollection<CalculationsPanelItem> GetItemsAlternative()
        {
            var mainWindow = MainWindow.getMainWnd();

            var propertyInfo = typeof(MainWindow).BaseType?
                .GetProperty("App", BindingFlags.NonPublic | BindingFlags.Instance);

            if (propertyInfo == null)
            {
                return null;
            }

            var app1 = propertyInfo.GetValue(mainWindow);
            var app = propertyInfo.GetValue(mainWindow) as Application;

            if (app == null)
            {
                return null;
            }

            var calculationsPanel = app.Windows.Cast<Window>().OfType<CalculationsPanel>().FirstOrDefault();

            if (calculationsPanel == null)
            {
                return null;
            }

            var fieldInfo = calculationsPanel.GetType().GetField("m_panelItems", BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo == null)
            {
                return null;
            }

            var m_panelItems = fieldInfo.GetValue(calculationsPanel) as ObservableCollection<CalculationsPanelItem>;

            return m_panelItems;
        }
    }
}
