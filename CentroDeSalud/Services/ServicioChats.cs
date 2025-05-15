using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioChats
    {
        Task<Guid> CrearChat(Chat chat);
        Task<bool> EliminarChat(Guid chatId);
        Task<bool> ExisteChat(Guid chatId, Guid usuarioId);
        Task<IEnumerable<ChatInfoViewModel>> ListarChatsPorMedico(Guid medicoId);
        Task<IEnumerable<ChatInfoViewModel>> ListarChatsPorPaciente(Guid pacienteId);
        Task<Guid> ObtenerReceptor(Guid chatId, Guid remitenteId);
    }

    public class ServicioChats : IServicioChats
    {
        private readonly IRepositorioChats repositorioChats;

        public ServicioChats(IRepositorioChats repositorioChats)
        {
            this.repositorioChats = repositorioChats;
        }

        public async Task<Guid> CrearChat(Chat chat)
        {
            return await repositorioChats.CrearChat(chat);
        }

        public async Task<bool> EliminarChat(Guid chatId)
        {
            return await repositorioChats.EliminarChat(chatId);
        }

        public async Task<IEnumerable<ChatInfoViewModel>> ListarChatsPorPaciente(Guid pacienteId)
        {
            return await repositorioChats.ListarChatsPorPaciente(pacienteId);
        }

        public async Task<IEnumerable<ChatInfoViewModel>> ListarChatsPorMedico(Guid medicoId)
        {
            return await repositorioChats.ListarChatsPorMedico(medicoId);
        }

        public async Task<bool> ExisteChat(Guid chatId, Guid usuarioId)
        {
            return await repositorioChats.ExisteChat(chatId, usuarioId);
        }

        public async Task<Guid> ObtenerReceptor(Guid chatId, Guid remitenteId)
        {
            return await repositorioChats.ObtenerReceptor(chatId, remitenteId);
        }
    }
}
