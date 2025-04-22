using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentroDeSalud.Models
{
    public class UsuarioLoginExterno
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string LoginProvider { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string ProviderKey { get; set; }

        [MaxLength(200)]
        public string ProviderDisplayName { get; set; }
    }
}
