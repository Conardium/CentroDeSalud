using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Infrastructure.Validations
{
    public class FechaNoPasadaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime fecha)
            {
                if (fecha.Date < DateTime.Now.Date)
                {
                    return new ValidationResult("La fecha no puede ser anterior a hoy.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
