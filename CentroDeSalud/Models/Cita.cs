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

        [Required(ErrorMessage = "Seleccione un médico valido")]
        [Display(Name = "Médico")]
        [ComprobarGuid]
        public Guid MedicoId { get; set; }
        public Medico Medico { get; set; }

        [Required(ErrorMessage = "Seleccione una fecha")]
        [Column(TypeName = "DATE")]
        [FechaNoPasada]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Seleccione una hora")]
        [Column(TypeName = "time(0)")]
        public TimeSpan Hora { get; set; }

        public EstadoCita EstadoCita { get; set; }

        [Required(ErrorMessage = "Indique el motivo de su cita")]
        [MaxLength(120)]
        public string Motivo { get; set; }

        [MaxLength(250)]
        public string Detalles { get; set; }

        public bool Sincronizada { get; set; } = false;
    }
}
