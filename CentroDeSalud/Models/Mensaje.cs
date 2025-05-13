using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentroDeSalud.Models
{
    public class Mensaje
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }

        [Required]
        public Guid RemitenteId { get; set; }
        public Usuario UsuarioRemitente { get; set; }

        [Required]
        public Guid ReceptorId { get; set; }
        public Usuario UsuarioReceptor { get; set; }

        [Required]
        [MaxLength(250)]
        public string Contenido { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime FechaEnvio { get; set; }
    }
}
