using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentroDeSalud.Models
{
    public class RespuestaForo
    {
        [Key]
        public int Id { get; set; }

        //Relación 1 a muchos con Medico
        public Guid MedicoId { get; set; }
        public Medico Medico { get; set; }

        //Relación 1 a muchos con PreguntaForo
        public int PreguntaForoId { get; set; }
        public PreguntaForo PreguntaForo { get; set; }

        [MaxLength(2000)]
        [Required]
        public string Texto { get; set; }

        [Column(TypeName = "DATE")]
        [Required]
        public DateTime FechaRespuesta { get; set; }
    }
}
