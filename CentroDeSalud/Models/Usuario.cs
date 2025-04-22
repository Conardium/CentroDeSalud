using CentroDeSalud.Infrastructure.Validations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [MaxLength(120, ErrorMessage = "El correo es demasiado largo")]
        [EmailAddress(ErrorMessage = "El correo debe ser de un tipo válido")]
        [EvitarCaracteresEspeciales]
        [EvitarInyecciones]
        public string Email { get; set; }

        [Required]
        [MaxLength(120)]
        public string EmailNormalizado { get; set; }

        [Required(ErrorMessage = "Indique una contraseña")]
        [Column(TypeName = "nvarchar(MAX)")]
        [Display(Name = "Contraseña")]
        [EvitarInyecciones]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Indique un nombre")]
        [MaxLength(50, ErrorMessage = "El nombre es demasiado largo")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ]+$", ErrorMessage = "El nombre solo puede contener letras y tildes")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [MaxLength(70, ErrorMessage = "Los apellidos son demasiado largos")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ ]+$", ErrorMessage = "Los apellidos solo pueden contener letras, tildes y espacios")]
        public string Apellidos { get; set; }

        [MaxLength(20, ErrorMessage = "El teléfono es demasiado largo")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }


        //Relacion con Rol
        public int? RolId { get; set; }
        public Rol Rol { get; set; } //Variable de navegacion con Rol
    }
}
