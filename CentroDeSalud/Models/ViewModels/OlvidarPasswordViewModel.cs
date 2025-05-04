using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    public class OlvidarPasswordViewModel
    {
        [Required(ErrorMessage = "Ingrese un correo electrónico")]
        [EmailAddress(ErrorMessage = "Escriba un correo válido")]
        public string Email{ get; set; }
    }
}
