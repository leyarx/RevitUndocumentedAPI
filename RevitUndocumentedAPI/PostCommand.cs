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

    class PostCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;

            var cmdId = RevitCommandId.LookupCommandId("ID_DEBUG_DUMP_RIBBON_STATE");

            if(uiapp.CanPostCommand(cmdId))
            {
                uiapp.PostCommand(cmdId);
            }

            return Result.Succeeded;
        }
    }
}
