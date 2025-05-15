using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioMensajes
    {
        Task<int> GuardarMensaje(Mensaje mensaje);
        Task<IEnumerable<Mensaje>> ListarMensajesPorChatId(Guid chatId);
    }

    public class RepositorioMensajes : IRepositorioMensajes
    {
        private readonly string _connectionString;

        public RepositorioMensajes(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<int> GuardarMensaje(Mensaje mensaje)
        {
            using var conexion = new SqlConnection(_connectionString);

            return await conexion.QuerySingleAsync<int>(@"Insert into Mensajes (ChatId, RemitenteId, ReceptorId, Contenido, FechaEnvio)
                                Values (@ChatId, @RemitenteId, @ReceptorId, @Contenido, @FechaEnvio);
                                SELECT SCOPE_IDENTITY();", mensaje);
        }

        public async Task<IEnumerable<Mensaje>> ListarMensajesPorChatId(Guid chatId)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<Mensaje>(@"Select * from Mensajes where ChatId = @ChatId",
                                        new {ChatId = chatId});
        }
    }
}
