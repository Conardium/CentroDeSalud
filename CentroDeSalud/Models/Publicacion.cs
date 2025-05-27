using CentroDeSalud.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentroDeSalud.Models
{
    public class Publicacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; }

        [Required]
        public string Cuerpo { get; set; }

        [StringLength(300)]
        public string Resumen { get; set; }

        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime FechaPublicacion { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime2(0)")]
        public DateTime? FechaModificacion { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        [StringLength(200)]
        public string Slug { get; set; }

        public bool Destacada { get; set; } = false;

        [Required]
        public EstadoPublicacion EstadoPublicacion { get; set; }

        [StringLength(500)]
        public string ImagenURL { get; set; }
    }
}
