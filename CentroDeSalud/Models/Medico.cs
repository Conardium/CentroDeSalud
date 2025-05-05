using CentroDeSalud.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models
{
    public class Medico : Usuario
    {
        [Required]
        [StringLength(9, ErrorMessage = "El DNI debe tener un máximo de 9 caracteres")]
        [RegularExpression("^[0-9]{8}[A-Za-z]$", ErrorMessage = "El DNI no tiene el formato correcto")]
        public string Dni { get; set; }

        [Required]
        [RegularExpression("^[1-6]$", ErrorMessage = "Seleccione una especialidad válida")]
        public Especialidad Especialidad { get; set; }

        [Required]
        [RegularExpression("^[1-3]$", ErrorMessage = "Seleccione un sexo válido")]
        public Sexo Sexo { get; set; }

        public ICollection<Cita> Citas { get; set; }

        public ICollection<DisponibilidadMedico> DisponibilidadesMedico { get; set; }
    }
}
