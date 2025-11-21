using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace DocumentManagementService
{

    public static class InputRestriction
    {

        public static readonly DependencyProperty AllowedPatternProperty =
            DependencyProperty.RegisterAttached(
                "AllowedPattern",
                typeof(string),
                typeof(InputRestriction),
                new PropertyMetadata(null, OnAllowedPatternChanged));

        public static void SetAllowedPattern(UIElement element, string value) =>
            element.SetValue(AllowedPatternProperty, value);

        public static string GetAllowedPattern(UIElement element) =>
            (string)element.GetValue(AllowedPatternProperty);

        private static void OnAllowedPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.PreviewTextInput -= HandlePreviewTextInput;
                DataObject.RemovePastingHandler(textBox, HandlePaste);

                if (e.NewValue != null)
                {
                    textBox.PreviewTextInput += HandlePreviewTextInput;
                    DataObject.AddPastingHandler(textBox, HandlePaste);
                }
            }
        }

        private static void HandlePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
            if (!IsValidInput(newText, GetAllowedPattern(textBox)))
            {
                e.Handled = true;
            }
        }

        private static void HandlePaste(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = (TextBox)sender;
            bool isText = e.SourceDataObject.GetDataPresent(DataFormats.Text, true);
            if (!isText) return;

            string pasteText = (string)e.SourceDataObject.GetData(DataFormats.Text);
            string newText = textBox.Text.Insert(textBox.CaretIndex, pasteText);

            if (!IsValidInput(newText, GetAllowedPattern(textBox)))
            {
                e.CancelCommand();
            }
        }

        private static bool IsValidInput(string input, string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return true;
            return Regex.IsMatch(input, pattern);
        }
    }
}
