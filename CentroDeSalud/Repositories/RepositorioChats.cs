using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioChats
    {
        Task<Guid> CrearChat(Chat chat);
        Task<bool> EliminarChat(Guid chatId);
        Task<bool> ExisteChat(Guid chatId, Guid idUsuario);
        Task<IEnumerable<ChatInfoViewModel>> ListarChatsPorMedico(Guid medicoId);
        Task<IEnumerable<ChatInfoViewModel>> ListarChatsPorPaciente(Guid pacienteId);
    }

    public class RepositorioChats : IRepositorioChats
    {
        private readonly string _connectionString;

        public RepositorioChats(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<Guid> CrearChat(Chat chat)
        {
            using var conexion = new SqlConnection(_connectionString);

            chat.Id = Guid.NewGuid();
            await conexion.ExecuteAsync(@"Insert into Chats (Id, PacienteId, MedicoId)
                                Values (@Id, @PacienteId, @MedicoId);", chat);

            return chat.Id;
        }

        public async Task<bool> EliminarChat(Guid chatId)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.ExecuteAsync(@"Delete from Chats where Id = @Id);",
                                    new { Id = chatId });
            return true;
        }
        
        public async Task<IEnumerable<ChatInfoViewModel>> ListarChatsPorPaciente(Guid pacienteId)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<ChatInfoViewModel>("ObtenerChatsPorPaciente",
                        new { PacienteId = pacienteId }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ChatInfoViewModel>> ListarChatsPorMedico(Guid medicoId)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<ChatInfoViewModel>("ObtenerChatsPorMedico",
                        new { MedicoId = medicoId }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<bool> ExisteChat(Guid chatId, Guid idUsuario)
        {
            using var conexion = new SqlConnection(_connectionString);
            var resultado = await conexion.QueryFirstOrDefaultAsync<int>(@"Select 1 from Chats 
                            Where Id = @Id AND (PacienteId = @PacienteId OR MedicoId = @MedicoId)",
                            new { Id = chatId, PacienteId = idUsuario, MedicoId = idUsuario });

            return resultado == 1;
        }
    }
}
