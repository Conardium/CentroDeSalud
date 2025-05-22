using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    public class SelectPacienteViewModel
    {
        [Required]
        public Guid Id { get; set; }
        public string Nombre { get; set; }
    }
}
