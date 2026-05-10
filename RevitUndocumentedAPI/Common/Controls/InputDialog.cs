using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RevitUndocumentedAPI.Common.Controls
{
    public class InputDialog : Window
    {
        private readonly TextBox _input = new TextBox() { Width = 200, VerticalContentAlignment = VerticalAlignment.Center };
        private readonly Button _ok = new Button() { Content = "OK", Width = 60, IsDefault = true };

        public double Value { get; private set; }

        public InputDialog(string prompt = "Enter value:", string title = "Input")
        {
            Title = title;
            SizeToContent = SizeToContent.WidthAndHeight;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Padding = new Thickness(12);

            var label = new Label { Content = prompt, VerticalContentAlignment = VerticalAlignment.Center };

            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(4) };
            row.Children.Add(label);
            row.Children.Add(_input);
            row.Children.Add(new UIElement()); // spacer via margin below
            _ok.Margin = new Thickness(6, 0, 0, 0);
            row.Children.Add(_ok);

            Content = row;

            // Only allow digits, minus, dot, and control keys
            _input.PreviewTextInput += (_, e) =>
            {
                string candidate = _input.Text.Insert(_input.CaretIndex, e.Text);
                bool isMinusStart = candidate == "-";
                e.Handled = !isMinusStart && !double.TryParse(candidate,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double _);
            };

            // Block paste of non-double text
            DataObject.AddPastingHandler(_input, (_, e) =>
            {
                if (e.DataObject.GetDataPresent(DataFormats.Text))
                {
                    string text = (string)e.DataObject.GetData(DataFormats.Text);
                    if (!double.TryParse(text,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out double _))
                        e.CancelCommand();
                }
                else e.CancelCommand();
            });

            _ok.Click += (_, e) =>
            {
                if (double.TryParse(_input.Text,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double v))
                {
                    Value = v;
                    DialogResult = true;
                }
                else MessageBox.Show("Please enter a valid number.", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            };
        }

        // Convenience factory — returns null if cancelled
        public static double? Show(string prompt = "Enter value:", string title = "Input")
        {
            var dlg = new InputDialog(prompt, title);
            if (dlg.ShowDialog() == true)
            {
                return dlg.Value;
            }

            return null;
        }
    }
}
