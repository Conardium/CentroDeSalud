using CentroDeSalud.Enumerations;
using CentroDeSalud.Infrastructure.Validations;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    public class CrearInformeViewModel
    {
        [Display(Name = "Paciente")]
        public string NombrePaciente { get; set; }

        public Guid PacienteId { get; set; }

        [Display(Name = "Médico")]
        public string NombreMedico { get; set; }

        public Guid MedicoId { get; set; }

        [Display(Name = "Fecha creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Estado")]
        public EstadoInforme EstadoInforme { get; set; }

        [Required(ErrorMessage = "Indique el diagnóstico del paciente")]
        [EvitarInyecciones]
        [MaxLength(500)]
        [Display(Name = "Diagnóstico")]
        public string Diagnostico { get; set; }

        [Required(ErrorMessage = "Indique el tratamiento del paciente")]
        [EvitarInyecciones]
        [MaxLength(1000)]
        public string Tratamiento { get; set; }

        [EvitarInyecciones]
        [MaxLength(1000)]
        public string Notas { get; set; }

        [EvitarInyecciones]
        [MaxLength(1000)]
        public string Recomendaciones { get; set; }

        [Display(Name = "Archivo adjunto")]
        public IFormFile ArchivoAdjunto { get; set; }
    }
}
