using CentroDeSalud.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentroDeSalud.Models
{
    /*
    public class Modelos
    {
    }
    */

    public class Usuario
    {
        [Key] //PK
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //IDENTITY
        public int Id { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")] //NOT NULL
        [MaxLength(120, ErrorMessage = "El correo es demasiado largo")] //nvarchar(120)
        [EmailAddress(ErrorMessage = "El correo debe ser de un tipo válido")]
        public string Email { get; set; }

        [Required] //NOT NULL
        [MaxLength(120)] //nvarchar(120)
        public string EmailNormalizado { get; set; }

        [Required(ErrorMessage = "Indique una contraseña")] //NOT NULL
        [Column(TypeName = "nvarchar(MAX)")] //nvarchar(MAX)
        [Display(Name = "Contraseña")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Indique un nombre")] //NOT NULL
        [MaxLength(50, ErrorMessage = "El nombre es demasiado largo")] //nvarchar(50)
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos")] //NOT NULL
        [MaxLength(70, ErrorMessage = "Los apellidos son demasiado largos")] //nvarchar(70)
        public string Apellidos { get; set; }

        [Phone(ErrorMessage = "El teléfono no es válido")]
        [MaxLength(20, ErrorMessage = "El teléfono es demasiado largo")] //nvarchar(20)
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }
    }

    public class Paciente : Usuario
    {
        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(9, ErrorMessage = "El DNI debe tener un máximo de 9 caracteres")]
        public string Dni { get; set; }

        [Required]
        [DataType(DataType.Date)] //Solo aplicable para el UI
        [Column(TypeName = "DATE")] //Date en SQL
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [StringLength(3, ErrorMessage = "El grupo sanguíneo debe tener un máximo de 3 caracteres")]
        [Display(Name = "Grupo Sanguíneo")]
        public GrupoSanguineo GrupoSanguineo { get; set; }

        [StringLength(100, ErrorMessage = "La dirección es demasiado larga")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "El sexo debe tener un máximo de 10 caracteres")]
        public Sexo Sexo { get; set; }
    }

    public class Medico : Usuario
    {
        [Required]
        [StringLength(9, ErrorMessage = "El DNI debe tener un máximo de 9 caracteres")]
        public string Dni { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "El sexo debe tener un máximo de 10 caracteres")]
        public Sexo Sexo { get; set; }
    }
}
