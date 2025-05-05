using CentroDeSalud.Enumerations;
using CentroDeSalud.Infrastructure.Validations;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    public class RegisterPacienteViewModel
    {
        [Required(ErrorMessage = "Indique un nombre")]
        [MaxLength(50, ErrorMessage = "El nombre es demasiado largo")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ ]+$", ErrorMessage = "El nombre solo puede contener letras y tildes")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [MaxLength(70, ErrorMessage = "Los apellidos son demasiado largos")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ ]+$", ErrorMessage = "Los apellidos solo pueden contener letras, tildes y espacios")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(9, ErrorMessage = "El DNI debe tener un máximo de 9 caracteres")]
        [RegularExpression("^[0-9]{8}[A-Za-z]$", ErrorMessage = "El DNI no tiene un formato correcto")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "Indique su Fecha de Nacimiento")]
        [DataType(DataType.Date, ErrorMessage = "Indique una fecha correcta")]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El Sexo es requerido")]
        public Sexo Sexo { get; set; }

        [RegularExpression(@"^\d{9}$", ErrorMessage = "El teléfono debe contener exactamente 9 dígitos.")]
        [MaxLength(20, ErrorMessage = "El teléfono es demasiado largo")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [StringLength(100, ErrorMessage = "La dirección es demasiado larga")]
        [Display(Name = "Dirección")]
        [EvitarInyecciones]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Seleccione un Grupo Sanguíneo")]
        [Display(Name = "Grupo Sanguíneo")]
        public GrupoSanguineo GrupoSanguineo { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [MaxLength(120, ErrorMessage = "El correo es demasiado largo")]
        [EmailAddress(ErrorMessage = "El correo debe ser de un tipo válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Indique una contraseña")]
        [Display(Name = "Contraseña")]
        [MinLength(6, ErrorMessage = "La contraseña debe de tener al menos 6 carácteres")]
        [EvitarInyecciones]
        public string PasswordHash { get; set; }
    }
}
