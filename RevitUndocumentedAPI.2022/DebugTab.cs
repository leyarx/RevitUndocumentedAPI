using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitUndocumentedAPI
{
    [Transaction(TransactionMode.Manual)]
    public class DebugTab : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var ribbonControl = UIFramework.RevitRibbonControl.RibbonControl;

            if(!ribbonControl.Tabs.Contains(ribbonControl.DebugTab))
            {
                ribbonControl.Tabs.Add(ribbonControl.DebugTab);
            }

            return Result.Succeeded;
        }
    }
}
