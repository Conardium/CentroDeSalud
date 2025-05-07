using Azure.Core;
using CentroDeSalud.Models;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioUsuariosLoginExterno
    {
        Task<IdentityResult> ActualizarAccessToken(string loginProvider, string providerKey, string accessToken, DateTime fechaExp);
        Task EliminarLoginExterno(Guid usuarioId, string loginProvider, string providerKey);
        public Task<IdentityResult> InsertarLoginExterno(UsuarioLoginExterno usuarioLoginExterno);
        Task<IEnumerable<UsuarioLoginExterno>> ListadoLoginsPorUsuarioId(Guid usuarioId);
        Task<UsuarioLoginExterno> ObtenerLoginExterno(string loginProvider, string providerKey);
        Task<UsuarioLoginExterno> ObtenerLoginExternoPorUsuarioProveedor(Guid usuarioId, string proveedor);
    }

    public class RepositorioUsuariosLoginExterno : IRepositorioUsuariosLoginExterno
    {
        private readonly string _connectionString;

        public RepositorioUsuariosLoginExterno(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<IdentityResult> InsertarLoginExterno(UsuarioLoginExterno usuarioLoginExterno)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.ExecuteAsync(@"INSERT INTO UsuariosLoginExterno (UsuarioId, LoginProvider, ProviderKey, 
                    ProviderDisplayName, AccessToken, RefreshToken, TokenExpiraEn) 
                    VALUES (@UsuarioId, @LoginProvider, @ProviderKey, @ProviderDisplayName, @AccessToken, 
                    @RefreshToken, @TokenExpiraEn);", usuarioLoginExterno);
            return IdentityResult.Success;
        }

        public async Task<UsuarioLoginExterno> ObtenerLoginExterno(string loginProvider, string providerKey)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryFirstOrDefaultAsync<UsuarioLoginExterno>(@"Select * from UsuariosLoginExterno
                                    where LoginProvider = @LoginProvider and ProviderKey = @ProviderKey;",
                                    new { LoginProvider = loginProvider, ProviderKey = providerKey});
        }

        public async Task<UsuarioLoginExterno> ObtenerLoginExternoPorUsuarioProveedor(Guid usuarioId, string proveedor)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryFirstOrDefaultAsync<UsuarioLoginExterno>(@"Select * from UsuariosLoginExterno
                                    Where UsuarioId = @UsuarioId AND LoginProvider = @LoginProvider",
                                    new { UsuarioId = usuarioId, LoginProvider = proveedor });
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

        public async Task<IdentityResult> ActualizarAccessToken(string loginProvider, string providerKey, string accessToken, DateTime fechaExp)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.ExecuteAsync(@"Update UsuariosLoginExterno 
                       SET AccessToken = @AccessToken, TokenExpiraEn = @TokenExpiraEn
                       WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey",
                       new { AccessToken = accessToken, TokenExpiraEn = fechaExp, LoginProvider = loginProvider, ProviderKey = providerKey });
            
            return IdentityResult.Success; 
        }
    }
}
