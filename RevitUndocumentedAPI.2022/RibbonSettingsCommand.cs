using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Reflection;


namespace RevitUndocumentedAPI
{
    [Transaction(TransactionMode.Manual)]
    public class RibbonSettingsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;

            // Option 1
            // Create a regular Revit PushButton and bind it to IExternalCommand

            RibbonPanel panel = Ribbon.CreatePanel(uiapp);

            FieldInfo aRibbonPanelField = typeof(RibbonPanel).GetField("m_RibbonPanel", BindingFlags.NonPublic | BindingFlags.Instance);
            Autodesk.Windows.RibbonPanel aRibbonPanel = aRibbonPanelField.GetValue(panel) as Autodesk.Windows.RibbonPanel;

            Assembly assembly = Assembly.GetExecutingAssembly();
            PushButtonData pushButtonData = new PushButtonData("DummyOptionsButton", "Dummy Button",
                assembly.Location, typeof(HelloRevitCommand).FullName);

            MethodInfo createPushButtonMethod = 
                typeof(PushButtonData).GetMethod("createPushButton", BindingFlags.NonPublic | BindingFlags.Static);
            var rPushButton = createPushButtonMethod.Invoke(null, new object[] { pushButtonData, false, aRibbonPanel.Source.Id });

            MethodInfo getRibbonButtonMethod = typeof(PushButton).GetMethod("getRibbonButton", BindingFlags.NonPublic | BindingFlags.Instance);
            var aRibbonButton = getRibbonButtonMethod.Invoke(rPushButton, new object[] { }) as Autodesk.Windows.RibbonButton;

            aRibbonPanel.Source.DialogLauncher = aRibbonButton;


            // Option 2
            // Create Autodesk RibbonButton and bind it to ICommand

            RibbonPanel panel2 = Ribbon.CreatePanel(uiapp, "Panel 2");

            // Another option is to parse RevitRibbonControl.RibbonControl.Tabs to find a panel
            Autodesk.Windows.RibbonPanel aRibbonPanel2 = aRibbonPanelField.GetValue(panel2) as Autodesk.Windows.RibbonPanel;

            Autodesk.Windows.RibbonButton dialogLauncher = new Autodesk.Windows.RibbonButton();
            dialogLauncher.Text = "Dummy Button 2";
            dialogLauncher.CommandHandler = new HelloRevitCommand2();

            aRibbonPanel2.Source.DialogLauncher = dialogLauncher;

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.ReadOnly)]
    public class HelloRevitCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            new TaskDialog("Hello, Revit! -> IExternalCommand").Show();

            return Result.Succeeded;
        }
    }

    public class HelloRevitCommand2 : System.Windows.Input.ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            new TaskDialog("Hello, Revit! -> ICommand").Show();
        }
    }
}
