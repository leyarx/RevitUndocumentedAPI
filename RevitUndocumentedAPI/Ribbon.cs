using Autodesk.Revit.UI;
using System;
using System.Linq;


namespace RevitUndocumentedAPI
{
    public static class Ribbon
    {
        private static readonly string TabName = "Undocumented";

        public static RibbonPanel CreatePanel(UIApplication uiapp, string panelName = "Panel")
        {
            try
            {
                uiapp.CreateRibbonTab(TabName);
            }
            catch (Exception) { }

            RibbonPanel panel = uiapp.GetRibbonPanels(TabName).FirstOrDefault(p => p.Name == panelName);

            if (panel == null)
            {
                panel = uiapp.CreateRibbonPanel(TabName, panelName);
            }

            return panel;
        }
    }
}
