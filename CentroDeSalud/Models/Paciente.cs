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
        public string Dni { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "DATE")]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        public GrupoSanguineo GrupoSanguineo { get; set; }

        [StringLength(100)]
        public string Direccion { get; set; }

        [Required]
        public Sexo Sexo { get; set; }
    }
}
