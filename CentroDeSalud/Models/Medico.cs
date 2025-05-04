using CentroDeSalud.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models
{
    public class Medico : Usuario
    {
        [Required]
        [StringLength(9, ErrorMessage = "El DNI debe tener un máximo de 9 caracteres")]
        [RegularExpression("^[0-9]{8}[A-Za-z]$", ErrorMessage = "El DNI no tiene un formato correcto")]
        public string Dni { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "El sexo debe tener un máximo de 10 caracteres")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ]+$", ErrorMessage = "El Sexo solo puede contener letras y tildes")]
        public Sexo Sexo { get; set; }
    }
}
