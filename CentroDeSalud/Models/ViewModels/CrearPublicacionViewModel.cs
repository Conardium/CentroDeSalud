using CentroDeSalud.Enumerations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CentroDeSalud.Infrastructure.Validations;

namespace CentroDeSalud.Models.ViewModels
{
    public class CrearPublicacionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Indique el título de la publicación")]
        [StringLength(150, ErrorMessage = "El título no puede tener más de 150 caracteres")]
        [EvitarInyecciones]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Indique el cuerpo de la publiación")]
        [EvitarInyecciones]
        public string Cuerpo { get; set; }

        [StringLength(300)]
        [EvitarInyecciones]
        public string Resumen { get; set; }

        [Display(Name = "Destacar")]
        public bool Destacada { get; set; } = false;

        [Display(Name = "Estado")]
        public EstadoPublicacion EstadoPublicacion { get; set; } = EstadoPublicacion.Borrador;

        public IFormFile Imagen { get; set; }

        public string ImagenURL { get; set; }
    }
}
