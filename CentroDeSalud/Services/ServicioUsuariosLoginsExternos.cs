using CentroDeSalud.Models;
using CentroDeSalud.Models.Requests;
using CentroDeSalud.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace CentroDeSalud.Services
{
    public interface IServicioUsuariosLoginsExternos
    {
        Task<IdentityResult> ActualizarConsentimientos(UsuarioLoginExterno loginGoogle, string accessToken, string refreshToken, DateTime? expiresAt);
        Task<IdentityResult> AgregarLoginExterno(UsuarioLoginExterno usuarioLoginExterno);
        Task<bool> ComprobarCaducidadToken(Guid usuarioId);
        Task<IEnumerable<UsuarioLoginExterno>> ListadoLoginsExternos(Guid usuarioId);
    }

    public class ServicioUsuariosLoginsExternos : IServicioUsuariosLoginsExternos
    {
        private readonly IRepositorioUsuariosLoginExterno repositorioUsuariosLoginExterno;
        private readonly IConfiguration configuration;

        public ServicioUsuariosLoginsExternos(IRepositorioUsuariosLoginExterno repositorioUsuariosLoginExterno, IConfiguration configuration)
        {
            this.repositorioUsuariosLoginExterno = repositorioUsuariosLoginExterno;
            this.configuration = configuration;
        }

        //Metodo privado que recoge el nuevo access token del proveedor de Google
        private async Task<TokenResponse> ObtenerNuevoAccessToken(string refreshToken)
        {
            var clientId = configuration["GoogleClientId"];
            var clientSecret = configuration["GoogleSecretId"];

            var url = "https://oauth2.googleapis.com/token";

            var parameters = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            };

            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(parameters);
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(responseContent);
                    var root = doc.RootElement;

                    return new TokenResponse
                    {
                        AccessToken = root.GetProperty("access_token").GetString(),
                        ExpiraEn = root.GetProperty("expires_in").GetInt32()
                    };
                }
                else
                {
                    //var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al renovar el token. Contenido: {errorContent}");
                }
            }
        }

        public async Task<IEnumerable<UsuarioLoginExterno>> ListadoLoginsExternos(Guid usuarioId)
        {
            return await repositorioUsuariosLoginExterno.ListadoLoginsPorUsuarioId(usuarioId);
        }

        public async Task<IdentityResult> AgregarLoginExterno(UsuarioLoginExterno usuarioLoginExterno)
        {
            return await repositorioUsuariosLoginExterno.InsertarLoginExterno(usuarioLoginExterno);
        }

        public async Task<bool> ComprobarCaducidadToken(Guid usuarioId)
        {
            var listaLogins = await repositorioUsuariosLoginExterno.ListadoLoginsPorUsuarioId(usuarioId);
            var loginGoogle = listaLogins.FirstOrDefault(l => l.LoginProvider == "Google");

            if (loginGoogle == null)
                return false;

            //Comprobamos si el access token a caducado
            if(DateTime.Now > loginGoogle.TokenExpiraEn)
            {
                var tokenResponse = await ObtenerNuevoAccessToken(loginGoogle.RefreshToken);

                if(tokenResponse == null)
                    return false;

                var NuevafechaCad = DateTime.Now.AddSeconds(tokenResponse.ExpiraEn);

                var resultado = await repositorioUsuariosLoginExterno.ActualizarAccessToken(
                    loginGoogle.LoginProvider, loginGoogle.ProviderKey, tokenResponse.AccessToken, NuevafechaCad);

                if (resultado.Succeeded)
                    return true;
            }

            return false;
        }

        public async Task<IdentityResult> ActualizarConsentimientos(UsuarioLoginExterno loginGoogle, string accessToken, string refreshToken, DateTime? expiresAt)
        {
            loginGoogle.AccessToken = accessToken;
            
            if(refreshToken != null)
                loginGoogle.RefreshToken = refreshToken;

            loginGoogle.TokenExpiraEn = loginGoogle.TokenExpiraEn < expiresAt ? expiresAt : loginGoogle.TokenExpiraEn;

            return await repositorioUsuariosLoginExterno.ActualizarLoginExterno(loginGoogle.LoginProvider, loginGoogle.ProviderKey, 
                loginGoogle.AccessToken, loginGoogle.RefreshToken, loginGoogle.TokenExpiraEn);
        }
    }
}
