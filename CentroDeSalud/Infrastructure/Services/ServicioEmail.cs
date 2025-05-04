using SendGrid.Helpers.Mail;
using SendGrid;

namespace CentroDeSalud.Infrastructure.Services
{
    public interface IServicioEmail
    {
        Task EnviarRecuperarPassword(string destinoEmail, string nombreReal, string destinoNombre, string urlRecuperacion);
    }

    public class ServicioEmail : IServicioEmail
    {
        private readonly IConfiguration configuration;

        public ServicioEmail(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task EnviarRecuperarPassword(string destinoEmail, string nombreReal, 
            string destinoNombre, string urlRecuperacion)
        {
            var apiKey = configuration["SendGridAPIkey"];
            var emailCentro = configuration["SendGridEmail"];
            var templateId = configuration["SendGridTemplateRecuperarPasswordID"];

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(emailCentro, "Cura Vitae Soporte");
            var to = new EmailAddress(destinoEmail, destinoNombre);

            var msg = new SendGridMessage();
            msg.SetFrom(from);
            msg.AddTo(to);
            msg.SetTemplateId(templateId);

            msg.SetTemplateData(new
            {
                nombre = nombreReal,
                url = urlRecuperacion
            });

            var response = await client.SendEmailAsync(msg);
        }
    }
}
