namespace CentroDeSalud.Models.ViewModels
{
    public class ChatInfoViewModel
    {
        public Guid ChatId { get; set; }
        public string NombrePaciente { get; set; }
        public string NombreMedico { get; set; }
        public DateTime? FechaUltimoMensaje { get; set; }
    }
}
