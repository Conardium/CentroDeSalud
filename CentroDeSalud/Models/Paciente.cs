using CentroDeSalud.Enumerations;
using CentroDeSalud.Infrastructure.Validations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models
{
    public class Paciente : Usuario
    {
        [Required]
        [StringLength(9)]
        [RegularExpression("^[0-9]{8}[A-Za-z]$")]
        public string Dni { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "DATE")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "Seleccione un Grupo Sanguíneo")]
        [Display(Name = "Grupo Sanguíneo")]
        public GrupoSanguineo GrupoSanguineo { get; set; }

        [StringLength(100)]
        [EvitarCaracteresEspeciales]
        [EvitarInyecciones]
        public string Direccion { get; set; }

        [Required]
        [RegularExpression("^[0-2]+$")]
        public Sexo Sexo { get; set; }
    }
}
