using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CentroDeSalud.Enumerations;

namespace CentroDeSalud.Models
{
    public class DisponibilidadMedico
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid MedicoId { get; set; }
        public Medico Medico { get; set; }

        [Required(ErrorMessage = "Seleccione un día de la semana")]
        public DiaSemana DiaSemana { get; set; }

        [Required(ErrorMessage = "Indique una Hora de Inicio")]
        [Column(TypeName = "time(0)")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "Seleccione una Hora de Fin")]
        [Column(TypeName = "time(0)")]
        public TimeSpan HoraFin { get; set; }
    }
}
