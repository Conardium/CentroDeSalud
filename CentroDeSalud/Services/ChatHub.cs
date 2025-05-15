using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Services
{
    public class ChatHub : Hub
    {
        private readonly IServicioMensajes servicioMensajes;
        private readonly IServicioChats servicioChats;

        public ChatHub(IServicioMensajes servicioMensajes, IServicioChats servicioChats)
        {
            this.servicioMensajes = servicioMensajes;
            this.servicioChats = servicioChats;
        }

        public async Task EnviarMensaje(MensajeViewModel modelo)
        {
            //Comprobamos el modelo
            var context = new ValidationContext(modelo, null, null);
            var resultados = new List<ValidationResult>();

            if (!Validator.TryValidateObject(modelo, context, resultados, true))
            {
                throw new HubException("El texto contiene errores de validación");
            }

            //Guardamos el mensaje en la BD
            await GuardarMensajesEnBD(modelo);

            //Enviamos el mensaje a todos los clientes del grupo del chat
            await Clients.Group(modelo.ChatId.ToString()).SendAsync("RecibirMensaje", modelo.UsuarioId, modelo.Texto, DateTime.Now.ToString("HH:mm"));
        }

        public async Task UnirseAlChat(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        private async Task GuardarMensajesEnBD(MensajeViewModel modelo)
        {
            var mensajeModel = new Mensaje
            {
                ChatId = modelo.ChatId,
                RemitenteId = modelo.UsuarioId,
                Contenido = modelo.Texto,
                FechaEnvio = DateTime.Now
            };

            mensajeModel.ReceptorId = await servicioChats.ObtenerReceptor(mensajeModel.ChatId, mensajeModel.RemitenteId);

            await servicioMensajes.GuardarMensaje(mensajeModel);
        }
    }
}
