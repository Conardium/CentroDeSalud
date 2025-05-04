using CentroDeSalud.Models;
using CentroDeSalud.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CentroDeSalud.Data
{
    public class UsuarioStore : IUserStore<Usuario>, IUserEmailStore<Usuario>, IUserPasswordStore<Usuario>, 
        IUserRoleStore<Usuario>, IUserLoginStore<Usuario>
    {
        private readonly IRepositorioUsuarios _repositorioUsuarios;
        private readonly IRepositorioRoles _repositorioRoles;
        private readonly IRepositorioUsuariosLoginExterno _repositorioUsuariosLoginExterno;

        public UsuarioStore(IRepositorioUsuarios repositorioUsuarios, IRepositorioRoles repositorioRoles, IRepositorioUsuariosLoginExterno repositorioUsuariosLoginExterno)
        {
            _repositorioUsuarios = repositorioUsuarios;
            _repositorioRoles = repositorioRoles;
            _repositorioUsuariosLoginExterno = repositorioUsuariosLoginExterno;
        }

        Task IUserLoginStore<Usuario>.AddLoginAsync(Usuario user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            return AddLoginAsync(user, login, cancellationToken);
        }

        public async Task<IdentityResult> AddLoginAsync(Usuario user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //Comprobamos que los campos no sean nulos
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(login);

            //Mapeamos los datos a UsuarioLoginExterno
            var usuarioLoginExterno = new UsuarioLoginExterno()
            {
                UsuarioId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName
            };

            await _repositorioUsuariosLoginExterno.InsertarLoginExterno(usuarioLoginExterno);
            return IdentityResult.Success;
        }

        public async Task AddToRoleAsync(Usuario user, string roleName, CancellationToken cancellationToken)
        {
            var rol = await _repositorioRoles.BuscarRolPorNombre(roleName);

            if (rol == null)
                throw new InvalidOperationException($"El rol {roleName} no existe.");
            else
            {
                user.RolId = rol.Id;
                await _repositorioUsuarios.ActualizarRol(user.Id, rol.Id);
            }
        }

        public async Task<IdentityResult> CreateAsync(Usuario user, CancellationToken cancellationToken)
        {
            user.Id = await _repositorioUsuarios.CrearUsuario(user);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(Usuario user, CancellationToken cancellationToken)
        {
            //No se implementa
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //No requiero liberacion de recursos
        }

        public async Task<Usuario> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var emailNormalizado = normalizedEmail.ToUpper();
            return await _repositorioUsuarios.BuscarUsuarioPorEmail(emailNormalizado);
        }

        public async Task<Usuario> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out Guid id))
                return null;

            return await _repositorioUsuarios.BuscarUsuarioPorId(id);
        }

        public async Task<Usuario> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(loginProvider);
            ArgumentNullException.ThrowIfNull(providerKey);
            
            var usuarioLoginExterno = await _repositorioUsuariosLoginExterno.ObtenerLoginExterno(loginProvider, providerKey);
            
            if(usuarioLoginExterno == null) 
                return null;
            
            return await _repositorioUsuarios.BuscarUsuarioPorId(usuarioLoginExterno.UsuarioId);
        }

        public async Task<Usuario> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _repositorioUsuarios.BuscarUsuarioPorEmail(normalizedUserName);
        }

        public Task<string> GetEmailAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(Usuario user, CancellationToken cancellationToken)
        {
            //No se implementa
            throw new NotImplementedException();
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(Usuario user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            var loginsExternos = await _repositorioUsuariosLoginExterno.ListadoLoginsPorUsuarioId(user.Id);

            return loginsExternos.Select(login => new UserLoginInfo(
                login.LoginProvider, login.ProviderKey, login.ProviderDisplayName)).ToList();
        }

        public Task<string> GetNormalizedEmailAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailNormalizado);
        }

        public Task<string> GetNormalizedUserNameAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailNormalizado);
        }

        public Task<string> GetPasswordHashAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public async Task<IList<string>> GetRolesAsync(Usuario user, CancellationToken cancellationToken)
        {
            if (user.RolId.HasValue)
            {
                var rol = await _repositorioRoles.BuscarRolPorId(user.RolId.Value);
                return new List<string> { rol?.Nombre };
            }

            return new List<string>();
        }

        public Task<string> GetUserIdAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<IList<Usuario>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            //No se implementa
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(Usuario user, CancellationToken cancellationToken)
        {
            //No se implementa
            throw new NotImplementedException();
        }

        public async Task<bool> IsInRoleAsync(Usuario user, string roleName, CancellationToken cancellationToken)
        {
            var rol = await _repositorioRoles.BuscarRolPorNombre(roleName);
            return (rol != null && user.RolId == rol.Id);
        }

        public async Task RemoveFromRoleAsync(Usuario user, string roleName, CancellationToken cancellationToken)
        {
            var rol = await _repositorioRoles.BuscarRolPorNombre(roleName);

            if (rol != null && user.RolId == rol.Id)
            {
                user.RolId = null;
                await _repositorioUsuarios.ActualizarRol(user.Id, null);
            }
            else
            {
                throw new InvalidOperationException($"El rol {roleName} no existe.");
            }         
        }

        public async Task RemoveLoginAsync(Usuario user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(loginProvider);
            ArgumentNullException.ThrowIfNull(providerKey);

            await _repositorioUsuariosLoginExterno.EliminarLoginExterno(user.Id, loginProvider, providerKey);
        }

        public Task SetEmailAsync(Usuario user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(Usuario user, bool confirmed, CancellationToken cancellationToken)
        {
            //No se implementa
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(Usuario user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.EmailNormalizado = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(Usuario user, string normalizedName, CancellationToken cancellationToken)
        {
            //user.Nombre = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(Usuario user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(Usuario user, string userName, CancellationToken cancellationToken)
        {
            user.Email = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(Usuario user, CancellationToken cancellationToken)
        {
            await _repositorioUsuarios.ActualizarPassword(user);
            return IdentityResult.Success;
        }
    }
}
