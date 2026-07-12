# RevitUndocumentedAPI

Some useful Revit API function which is not documented. Use at your own risk.

## RibbonLabelCommand
Using functions from AdWindows.dll to create a label on Ribbon panel in Revit.

![Revit Ribbon Label](./Images/RibbonLabelCommand.PNG?raw=true)

Inspired by [Simulating a Ribbon Textbox Label](https://thebuildingcoder.typepad.com/blog/2010/09/simulating-a-ribbon-textbox-label.html)

## RibbonSettingsCommand
Create a Dialog Box Launchers button on the Revit Ribbon Panel.

![Revit Ribbon Dialog Box Launchers](./Images/RibbonSettingsCommand.png?raw=true)

Inspired by [Ribbon Panel Caption Button](https://forums.autodesk.com/t5/revit-api-forum/ribbon-panel-caption-button/m-p/9354199#M45054)

## SimulateCommandOnBarCommand
Imitate method from UIFrameworkServices.DialogBarService.simulateCommandOnBar()

## OptionBarTitleCommand
Some experiments with DynamicLabelDialogBar to set a get title value

## RibbonDebugTab
Activate Revit In-House Debug Tab

![Revit Ribbon In-House Debug Tab](./Images/RibbonDebugTab.png?raw=true)

## BackgroundProcessesCommand
Add Item to Background Processes Panel

![Revit Background Processes Panel](./Images/BackgroundProcessesCommand.png?raw=true)

## SplitRegionOffsetCommand
Move Splitted Crop Region

![Revit Split Region Offset](./Images/SplitRegionOffsetCommand.gif?raw=true)

## PdfWatermarkCommand
Add Watermark to PDF

![Revit PDF Watermark](./Images/PdfWatermarkCommand.png?raw=true)
#### Adobe.PDF.Library.LM.NET
```
Compatibility notes:

- Versions **18.42.0 through 18.53.0** are **not compatible** due to DLL dependency issues.
- Revit’s root folder contains an older `dbghelp.dll` that shadows the correct version in `System32`.

Known working versions:

- **Revit 2025** → **18.23.0**
- **Revit 2026** → **18.41.0**

Note: **18.49.0** is a right version for Revit 2026, but it cannot be used because of the DLL dependency conflict.
```
