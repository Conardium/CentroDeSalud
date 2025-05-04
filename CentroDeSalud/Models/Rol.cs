using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models
{
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
