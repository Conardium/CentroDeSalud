using CentroDeSalud.Enumerations;
using CentroDeSalud.Infrastructure.Validations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    public class CrearCitaViewModel : Cita
    {
        [Required(ErrorMessage = "Seleccione un médico válido")]
        public IEnumerable<Medico> Medicos { get; set; } = new List<Medico>();
    }
}
