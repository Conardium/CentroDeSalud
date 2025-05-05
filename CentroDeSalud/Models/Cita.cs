using CentroDeSalud.Enumerations;
using CentroDeSalud.Infrastructure.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentroDeSalud.Models
{
    public class Cita
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        [Required]
        public Guid MedicoId { get; set; }
        public Medico Medico { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Seleccione una hora")]
        [Column(TypeName = "time(0)")]
        public TimeSpan Hora { get; set; }

        public EstadoCita EstadoCita { get; set; }

        [Required]
        [MaxLength(120)]
        [EvitarCaracteresEspeciales]
        [EvitarInyecciones]
        public string Motivo { get; set; }

        [EvitarCaracteresEspeciales]
        [EvitarInyecciones]
        [MaxLength(250)]
        public string Detalles { get; set; }
    }
}
