using CentroDeSalud.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentroDeSalud.Models
{
    public class Informe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fecha de creación")]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "datetime2(0)")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Display(Name = "Última modificación")]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "datetime2(0)")]
        public DateTime? FechaModificacion { get; set; }

        [Required]
        [Display(Name = "Estado del informe")]
        public EstadoInforme EstadoInforme { get; set; } = EstadoInforme.Borrador;

        [Required]
        [StringLength(1000)]
        [Display(Name = "Diagnóstico")]
        public string Diagnostico { get; set; }

        [Required]
        [StringLength(1000)]
        public string Tratamiento { get; set; }

        [StringLength(2000)]
        public string Notas { get; set; }

        [StringLength(1000)]
        public string Recomendaciones { get; set; }

        [StringLength(500)]
        [Display(Name = "Archivos adjuntos")]
        public string ArchivosAdjuntos { get; set; }

        // Relaciones
        [Required]
        public Guid PacienteId { get; set; }

        [Required]
        public Guid MedicoId { get; set; }

        // Variables de navegación
        public virtual Paciente Paciente { get; set; }
        public virtual Medico Medico { get; set; }
    }
}
