using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.Requests
{
    public class ObtenerCitasRequest
    {
        public Guid MedicoId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
    }
}
