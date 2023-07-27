using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace RevitUndocumentedAPI
{
    [Transaction(TransactionMode.Manual)]
    public class RibbonLabelCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            RibbonPanel panel = Ribbon.CreatePanel(uiapp);

            ComboBoxData comboBoxDataLevel = new ComboBoxData("LevelsSelector");
            var sItems = panel.AddLabeledStackedItems("Active Workset:", comboBoxDataLevel);

            return Result.Succeeded;
        }
    }
}
