using CentroDeSalud.Models;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioMensajes
    {
        Task<int> GuardarMensaje(Mensaje mensaje);
        Task<IEnumerable<Mensaje>> ListarMensajesPorChatId(Guid chatId);
    }

    public class ServicioMensajes : IServicioMensajes
    {
        private readonly IRepositorioMensajes repositorioMensajes;

        public ServicioMensajes(IRepositorioMensajes repositorioMensajes)
        {
            this.repositorioMensajes = repositorioMensajes;
        }

        public async Task<int> GuardarMensaje(Mensaje mensaje)
        {
            return await repositorioMensajes.GuardarMensaje(mensaje);
        }

        public async Task<IEnumerable<Mensaje>> ListarMensajesPorChatId(Guid chatId)
        {
            return await repositorioMensajes.ListarMensajesPorChatId(chatId);
        }
    }
}
