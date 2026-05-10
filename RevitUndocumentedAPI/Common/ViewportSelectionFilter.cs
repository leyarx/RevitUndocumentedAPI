using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Text;

namespace RevitUndocumentedAPI.Common
{
    public class ViewportSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element != null && element.GetType() == typeof(Element))
            {
                var doc = element.Document;
                var typeId = element.GetTypeId();
                if (typeId != null && typeId != ElementId.InvalidElementId)
                {
                    var type = doc.GetElement(typeId);
                    if (type != null && type is ViewFamilyType)
                    {
                        return true;
                    }
                }

                //var doc = viewport.Document;
                //var view = doc.GetElement(viewport.ViewId);
                //if (view is ViewPlan || view is ViewSection)
                //{
                //    return true;
                //}
            }

            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
}
