using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioUsuariosLoginExterno
    {
        Task EliminarLoginExterno(Guid usuarioId, string loginProvider, string providerKey);
        public Task InsertarLoginExterno(UsuarioLoginExterno usuarioLoginExterno);
        Task<IEnumerable<UsuarioLoginExterno>> ListadoLoginsPorUsuarioId(Guid usuarioId);
        Task<UsuarioLoginExterno> ObtenerLoginExterno(string loginProvider, string providerKey);
    }

    public class RepositorioUsuariosLoginExterno : IRepositorioUsuariosLoginExterno
    {
        private readonly string _connectionString;

        public RepositorioUsuariosLoginExterno(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task InsertarLoginExterno(UsuarioLoginExterno usuarioLoginExterno)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.ExecuteAsync(@"INSERT INTO UsuariosLoginExterno (UsuarioId, LoginProvider, ProviderKey, ProviderDisplayName)
                            VALUES (@UsuarioId, @LoginProvider, @ProviderKey, @ProviderDisplayName);", usuarioLoginExterno);
        }

        public async Task<UsuarioLoginExterno> ObtenerLoginExterno(string loginProvider, string providerKey)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryFirstOrDefaultAsync<UsuarioLoginExterno>(@"Select * from UsuariosLoginExterno
                                    where LoginProvider = @LoginProvider and ProviderKey = @ProviderKey;",
                                    new { LoginProvider = loginProvider, ProviderKey = providerKey});
        }

        public async Task<IEnumerable<UsuarioLoginExterno>> ListadoLoginsPorUsuarioId(Guid usuarioId)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<UsuarioLoginExterno>(@"Select * from UsuariosLoginExterno
                                        where UsuarioId = @UsuarioId;", new { UsuarioId = usuarioId});
        }

        public async Task EliminarLoginExterno(Guid usuarioId,  string loginProvider, string providerKey)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.ExecuteAsync(@"Delete from UsuariosLoginExterno where UsuarioId = @UsuarioId AND
                            LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey",
                            new { UsuarioId = usuarioId, LoginProvider = loginProvider, ProviderKey = providerKey});
        }
    }
}
