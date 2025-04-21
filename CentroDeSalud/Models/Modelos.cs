using CentroDeSalud.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentroDeSalud.Infrastructure.Validations;

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
        public Rol Rol { get; set; } //Variable de navegacion
    }

    public class Paciente : Usuario
    {
        [Required]
        [StringLength(9)]
        [RegularExpression("^[0-9]{8}[A-Za-z]$")]
        public string Dni { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "DATE")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "Seleccione un Grupo Sanguíneo")]
        [Display(Name = "Grupo Sanguíneo")]
        public GrupoSanguineo GrupoSanguineo { get; set; }

        [StringLength(100)]
        [EvitarCaracteresEspeciales]
        [EvitarInyecciones]
        public string Direccion { get; set; }

        [Required]
        [RegularExpression("^[0-2]+$")]
        public Sexo Sexo { get; set; }
    }

    public class Medico : Usuario
    {
        [Required]
        [StringLength(9, ErrorMessage = "El DNI debe tener un máximo de 9 caracteres")]
        [RegularExpression("^[0-9]{8}[A-Za-z]$", ErrorMessage = "El DNI no tiene un formato correcto")]
        public string Dni { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "El sexo debe tener un máximo de 10 caracteres")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ]+$", ErrorMessage = "El Sexo solo puede contener letras y tildes")]
        public Sexo Sexo { get; set; }
    }

    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(30)]
        public string Nombre { get; set; }

        [MaxLength(30)]
        public string NombreNormalizado { get; set; }

        //Relacion con Usuario (Variable de navegacion)
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
