using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    public class RestablecerPasswordViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ingrese una contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string CodigoReseteo { get; set; }
    }
}
