using CentroDeSalud.Models;
using CentroDeSalud.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CentroDeSalud.Data
{
    public class UsuarioStore : IUserStore<Usuario>, IUserEmailStore<Usuario>, IUserPasswordStore<Usuario>, 
        IUserRoleStore<Usuario>
    {
        private readonly IRepositorioUsuarios _repositorioUsuarios;
        private readonly IRepositorioRoles _repositorioRoles;

        public UsuarioStore(IRepositorioUsuarios repositorioUsuarios, IRepositorioRoles repositorioRoles)
        {
            _repositorioUsuarios = repositorioUsuarios;
            _repositorioRoles = repositorioRoles;
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
            return await _repositorioUsuarios.BuscarUsuarioPorEmail(normalizedEmail);
        }

        public async Task<Usuario> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out Guid id))
                return null;

            return await _repositorioUsuarios.BuscarUsuarioPorId(id);
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
            throw new NotImplementedException();
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

        public Task SetEmailAsync(Usuario user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(Usuario user, bool confirmed, CancellationToken cancellationToken)
        {
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

        public Task<IdentityResult> UpdateAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
