using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUndocumentedAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIFrameworkServices;

namespace RevitUndocumentedAPI
{
    [Transaction(TransactionMode.Manual)]
    public class BackgroundProcessesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;

            var calculationsPanelWrapper = CalculationsPanelHelper.GetCalculationsPanelWrapper();

            if (calculationsPanelWrapper != null && calculationsPanelWrapper.HasValue)
            {
                var helloWorldCalculationsPanelItem = new HelloWorldCalculationsPanelItem();
                helloWorldCalculationsPanelItem.SetDetails("Some Hello World Details!");
                calculationsPanelWrapper.Value.Items.Add(helloWorldCalculationsPanelItem);
            }

            return Result.Succeeded;
        }
    }
}
