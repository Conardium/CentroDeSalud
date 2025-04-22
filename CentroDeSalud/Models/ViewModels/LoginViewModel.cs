using CentroDeSalud.Infrastructure.Validations;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Indique un correo")]
        [MaxLength(120, ErrorMessage = "El correo es demasiado largo")]
        [EmailAddress(ErrorMessage = "El correo debe ser de un tipo válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Indique una contraseña")]
        [Display(Name = "Contraseña")]
        [EvitarInyecciones]
        public string Password { get; set; }

        public bool Recuerdame { get; set; } //Para indicar si la sesion del usuario se mantiene o no
    }
}
