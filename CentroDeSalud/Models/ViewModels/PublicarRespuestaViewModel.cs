using CentroDeSalud.Infrastructure.Validations;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    public class PublicarRespuestaViewModel
    {
        [MaxLength(2000)]
        [Required(ErrorMessage = "Añade la respuesta a la pregunta")]
        [EvitarInyecciones]
        public string Respuesta { get; set; }

        public int PreguntaForoId { get; set; }

        public PreguntaForo PreguntaForo { get; set; }
    }
}
