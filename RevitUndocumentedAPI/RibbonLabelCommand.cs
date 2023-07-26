using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using Autodesk.Windows;

namespace RevitUndocumentedAPI
{
    [Transaction(TransactionMode.Manual)]
    public class RibbonLabelCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string tabName = "Undocumented";
            string panelName = "Panel";

            var uiapp = commandData.Application;

            try
            {
                uiapp.CreateRibbonTab(tabName);
            }
            catch (Exception) { }

            RibbonPanel panel = uiapp.GetRibbonPanels(tabName).FirstOrDefault(p => p.Name == panelName);

            if (panel == null)
            {
                panel = uiapp.CreateRibbonPanel(tabName, panelName);
            }

            ComboBoxData comboBoxDataLevel = new ComboBoxData("LevelsSelector");
            var sItems = panel.AddLabeledStackedItems("Active Workset:", comboBoxDataLevel);

            return Result.Succeeded;
        }
    }
}
