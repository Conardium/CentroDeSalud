using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CentroDeSalud.Infrastructure.Validations;

namespace CentroDeSalud.Models.ViewModels
{
    public class MensajeViewModel
    {
        public Guid UsuarioId { get; set; }

        [Required]
        [MaxLength(250)]
        [EvitarInyecciones]
        [Display(Name = "Mensaje")]
        public string Texto { get; set; }

        public IEnumerable<Mensaje> Mensajes { get; set; }
    }
}
