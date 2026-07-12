#if REVIT2025 || REVIT2026
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Datalogics.PDFL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace RevitUndocumentedAPI.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PdfWatermarkCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            var document = uiDoc.Document;

            // INFO: Exporting a PDF activates a Revit APDFL license in the current thread.
            // This way, we don't need our own license or setting the Library.LicenseKey property.
            try
            {
                _ = document.Export(
                    System.IO.Path.GetTempPath(),
                    new List<ElementId>() { document.ActiveView.Id },
                    new PDFExportOptions() { FileName = System.IO.Path.GetRandomFileName() }
                    );
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            var dialog = new FileOpenDialog("PDF Files (*.pdf)|*.pdf")
            {
                Title = "Add Watermark to PDF"
            };

            if (dialog.Show() != ItemSelectionDialogResult.Confirmed)
            {
                return Result.Failed;
            }

            var filePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(dialog.GetSelectedModelPath());

            using (Library lib = new Library())
            {
                using (var doc = new Datalogics.PDFL.Document(filePath))
                {
                    var watermarkParams = new WatermarkParams()
                    {
                        Opacity = 0.5f,
                        Rotation = 45.0f,
                        Scale = 1.0f,
                        HorizAlign = HorizontalAlignment.Left,
                        VertAlign = VerticalAlignment.Center,
                    };
                    watermarkParams.TargetRange.PageSpec = PageSpec.AllPages;
          
                    var color = new Datalogics.PDFL.Color(109.0f / 255.0f, 15.0f / 255.0f, 161.0f / 255.0f);
                    var font = new Datalogics.PDFL.Font("Arial", FontCreateFlags.Embedded | FontCreateFlags.Subset);
                    var watermarkTextParams = new WatermarkTextParams()
                    {
                        Color = color,
                        Text = "Hello from\nRevit!",
                        TextAlign = HorizontalAlignment.Center,
                        Font = font,
                        FontSize = 100.0f,
                    };

                    doc.Watermark(watermarkTextParams, watermarkParams);

                    doc.Save(SaveFlags.Full | SaveFlags.Linearized);
                }
            }
            
            _ = new TaskDialog("Add Watermark to PDF")
            {
                TitleAutoPrefix = false,
                MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                MainInstruction = "Watermark added successfully!",
                CommonButtons = TaskDialogCommonButtons.Close,
            }
            .Show();

            return Result.Succeeded;
        }
    }
}
#endif