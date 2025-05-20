using CentroDeSalud.Enumerations;
using CentroDeSalud.Infrastructure.Validations;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    public class EditarPerfilViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Indique un nombre")]
        [MaxLength(50, ErrorMessage = "El nombre es demasiado largo")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ ]+$", ErrorMessage = "El nombre solo puede contener letras y tildes")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [MaxLength(70, ErrorMessage = "Los apellidos son demasiado largos")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ ]+$", ErrorMessage = "Los apellidos solo pueden contener letras, tildes y espacios")]
        public string Apellidos { get; set; }

        [RegularExpression(@"^\d{9}$", ErrorMessage = "El teléfono debe contener exactamente 9 dígitos.")]
        [MaxLength(20, ErrorMessage = "El teléfono es demasiado largo")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        public Sexo Sexo { get; set; }

        //Datos editables del paciente
        [StringLength(100, ErrorMessage = "La dirección es demasiado larga")]
        [Display(Name = "Dirección")]
        [EvitarInyecciones]
        public string Direccion { get; set; }

        //Datos eidtables del medico
        public Especialidad Especialidad { get; set; }
    }
}
