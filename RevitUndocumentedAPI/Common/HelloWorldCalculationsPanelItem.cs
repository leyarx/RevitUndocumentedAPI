using UIFramework;

namespace RevitUndocumentedAPI.Common
{
    public class HelloWorldCalculationsPanelItem : CalculationsPanelItem
    {
        public override Status ItemStatus => Status.Calculating;

        private string _itemSummary = "Hello World is Running";
        public override string ItemSummary => _itemSummary;

        private string _itemDetails = string.Empty;
        public override string ItemDetails => _itemDetails;

        public override string ItemName => "Hello World";


        private bool _canAbort = true;
        public override bool CanAbort { get => _canAbort; set => _canAbort = value; }

        public override bool abortTask()
        {
            if (_itemSummary.Contains("Stop"))
            {
                _itemSummary = $"{ItemName} is Running";
            }
            else
            {
                _itemSummary = $"{ItemName} is Stopped";
            }

            NotifyPropertyChanged(nameof(ItemSummary));

            return true;
        }

#if REVIT2026
        public override bool Equals(CalculationsPanelItem other)
        {
            return false;
        }
#endif

        public override bool isMiniDumpReportAvailable()
        {
            return true;
        }

        public override bool reportCalculationCrash()
        {
            return true;
        }

        public override bool restartCalculation()
        {
            return true;
        }

        public void SetDetails(string details)
        {
            _itemDetails = details;
        }
    }
}
