using CentroDeSalud.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentroDeSalud.Models
{
    public class PreguntaForo
    {
        [Key]
        public int Id { get; set; }

        //Relación 1 a muchos con Paciente
        public Guid PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        [MaxLength(200)]
        [Required]
        public string Titulo { get; set; }

        [MaxLength(2000)]
        [Required]
        public string Texto { get; set; }

        [Column(TypeName = "DATE")]
        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public EstadoPregunta EstadoPregunta { get; set; } = EstadoPregunta.Abierta;

        //Relación 1 a muchos con RespuestaForo
        public ICollection<RespuestaForo> RespuestasForo { get; set; }
    }
}
