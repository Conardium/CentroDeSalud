using CentroDeSalud.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Data;
using System.Security.Claims;

namespace CentroDeSalud.Data
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<Usuario>
    {
        public CustomClaimsPrincipalFactory(
        UserManager<Usuario> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Usuario user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // Listado de claims personalizados
            //Nombre real del usuario
            if (!string.IsNullOrEmpty(user.Nombre))
            {
                identity.AddClaim(new Claim("NombreReal", user.Nombre));
            }

            //Rol del usuario
            var roles = await UserManager.GetRolesAsync(user);
            foreach (var rol in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, rol));
            }

            return identity;
        }
    }
}
