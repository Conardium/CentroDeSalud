using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CentroDeSalud.Infrastructure.Validations
{
    public class EvitarCaracteresEspecialesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;

            var input = value.ToString();

            if(Regex.IsMatch(input, @"[<>:&|/+#$()?¿!¡=;{}^`*\\]"))
                return new ValidationResult("No se permiten caracteres especiales");

            // Validación para evitar símbolos (- . _ >) repetidos
            if (Regex.IsMatch(input, @"[\.\-_>]{2,}"))
                return new ValidationResult("No se permiten caracteres especiales seguidos");

            // Validación para evitar caracteres(- . _) al inicio o final
            if (input.StartsWith(".") || input.StartsWith("-") || input.StartsWith("_") ||
                input.EndsWith(".") || input.EndsWith("-") || input.EndsWith("_"))
                return new ValidationResult("No se permite comenzar o finalizar con caracteres especiales");

            return ValidationResult.Success;
        }
    }
}
