using CentroDeSalud.Infrastructure.Validations;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    public class PublicarPreguntaViewModel
    {
        [MaxLength(200)]
        [Required(ErrorMessage = "Indique un título para su duda")]
        [EvitarInyecciones]
        public string Titulo { get; set; }

        [MaxLength(2000)]
        [Required(ErrorMessage = "Añade detalles a su duda")]
        [EvitarInyecciones]
        public string Detalles { get; set; }

        public IEnumerable<PreguntaForo> Preguntas { get; set; }
    }
}
