using CentroDeSalud.Enumerations;
using CentroDeSalud.Models;
using CentroDeSalud.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CentroDeSalud.Data
{
    public static class SeedMedicos
    {
        public static async Task CrearMedicosAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
            var repositorioMedicos = serviceProvider.GetRequiredService<IRepositorioMedicos>();
            var repositorioUsuarios = serviceProvider.GetRequiredService<IRepositorioUsuarios>();

            var usuario = new Usuario
            {
                Email = "me5@me.me",
                EmailNormalizado = "ME5@ME.ME",
                PasswordHash = "111222",
                Nombre = "Flin Flon",
                Apellidos = "Vazquez Fante",
                Telefono = "123456789",
                RolId = null,
                SecurityStamp = "662214f2-389e-4224-9022-4576593061da"
            };
            
            await userManager.CreateAsync(usuario, password: usuario.PasswordHash);
            var usuarioFinal = await userManager.FindByEmailAsync(usuario.Email);

            var medico = new Medico
            {
                Id = usuarioFinal.Id,
                Dni = "12345678A",
                Sexo = Sexo.Hombre,
                Especialidad = Especialidad.Ginecología
            };

            await repositorioMedicos.CrearMedico(medico);

            await userManager.AddToRoleAsync(usuario, Constantes.RolMedico);
        }
    }
}
