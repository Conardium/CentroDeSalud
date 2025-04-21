using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CentroDeSalud.Infrastructure.Validations
{
    public class EvitarInyeccionesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;

            var input = value.ToString();

            if (Regex.IsMatch(input, @"(--|==|\|\||\\{2,}|['""<>]|\*|\*|-->|(?<!\w)(OR|AND)(?!\w)|eval\s*\(|<script\b)", RegexOptions.IgnoreCase))
            {
                return new ValidationResult($"El campo {validationContext.DisplayName} contiene caracteres o patrones no permitidos");
            }

            return ValidationResult.Success;
        }
    }
}
