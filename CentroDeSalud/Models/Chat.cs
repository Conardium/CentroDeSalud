using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models
{
    public class Chat
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        [Required]
        public Guid MedicoId { get; set; }
        public Medico Medico { get; set; }

        public ICollection<Mensaje> Mensajes { get; set; }
    }
}
