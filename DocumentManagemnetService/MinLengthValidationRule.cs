using System.Globalization;
using System.Windows.Controls;

namespace DocumentManagementService
{
    public class MinLengthValidationRule : ValidationRule
    {
        public int MinLength { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = value as string;
            if(str != string.Empty)
            {
                if (str?.Length < MinLength)
                    return new ValidationResult(false, $"Минимальная длина: {MinLength}");
            }
            return ValidationResult.ValidResult;
        }
    }
}
