using CentroDeSalud.Infrastructure.Services;
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
        private readonly IChatAI servicioChatAI;

        public ChatHub(IServicioMensajes servicioMensajes, IServicioChats servicioChats, IChatAI servicioChatAI)
        {
            this.servicioMensajes = servicioMensajes;
            this.servicioChats = servicioChats;
            this.servicioChatAI = servicioChatAI;
        }

        #region Chat entre paciente y médico

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

        //Crea el grupo para poder hablar de forma privada con el medico cuyo nombre del grupo es el Id del chat entre estos
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

        #endregion

        #region Chat con la IA

        public async Task EnviarMensajeIA(MensajeViewModel modelo)
        {
            string respuestaIA;
            try
            {
                respuestaIA = await servicioChatAI.EnviarMensajeAI(modelo.Texto);
            }
            catch (Exception ex)
            {
                respuestaIA = "He tenido el siguiente problema para procesar su mensaje: " + ex.Message;
            }

            //Enviar mensaje del paciente:
            await Clients.Group(modelo.ChatId.ToString()).SendAsync("RecibirMensajeIA", modelo.UsuarioId, modelo.Texto);

            //Enviar mensaje de la IA:
            await Clients.Group(modelo.ChatId.ToString()).SendAsync("RecibirMensajeIA", "IA", respuestaIA);
        }

        //Crea el grupo para poder hablar de forma privada con la IA cuyo nombre del grupo es el Id del paciente
        public async Task UnirseAlChatConIA(Guid pacienteId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, pacienteId.ToString());
        }

        #endregion
    }
}
