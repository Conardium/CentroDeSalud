using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Infrastructure.Validations
{
    public class ComprobarGuidAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is Guid guid)
            {
                if (guid == Guid.Empty)
                {
                    return new ValidationResult("Indique un médico válido");
                }
            }

            return ValidationResult.Success;
        }
    }
}
