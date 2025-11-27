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
            else if (d is PasswordBox passwordBox)
            {
                passwordBox.PreviewTextInput -= HandlePreviewTextInput;
                DataObject.RemovePastingHandler(passwordBox, HandlePaste);

                if (e.NewValue != null)
                {
                    passwordBox.PreviewTextInput += HandlePreviewTextInput;
                    DataObject.AddPastingHandler(passwordBox, HandlePaste);
                }
            }
        }

        private static void HandlePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
                if (!IsValidInput(newText, GetAllowedPattern(textBox)))
                {
                    e.Handled = true;
                }
            }
            else if (sender is PasswordBox passwordBox)
            {
                // Для PasswordBox мы не знаем позицию курсора, поэтому проверяем просто добавление текста
                string newPassword = passwordBox.Password + e.Text;
                if (!IsValidInput(newPassword, GetAllowedPattern(passwordBox)))
                {
                    e.Handled = true;
                }
            }
        }

        private static void HandlePaste(object sender, DataObjectPastingEventArgs e)
        {
            bool isText = e.SourceDataObject.GetDataPresent(DataFormats.Text, true);
            if (!isText) return;

            string pasteText = (string)e.SourceDataObject.GetData(DataFormats.Text);

            if (sender is TextBox textBox)
            {
                string newText = textBox.Text.Insert(textBox.CaretIndex, pasteText);
                if (!IsValidInput(newText, GetAllowedPattern(textBox)))
                {
                    e.CancelCommand();
                }
            }
            else if (sender is PasswordBox passwordBox)
            {
                // Для PasswordBox добавляем в конец
                string newPassword = passwordBox.Password + pasteText;
                if (!IsValidInput(newPassword, GetAllowedPattern(passwordBox)))
                {
                    e.CancelCommand();
                }
            }
        }

        private static bool IsValidInput(string input, string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return true;
            return Regex.IsMatch(input, pattern);
        }
    }
}
