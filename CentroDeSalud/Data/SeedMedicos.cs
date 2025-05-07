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
                Email = "me2@me.me",
                EmailNormalizado = "ME2@ME.ME",
                PasswordHash = "111222",
                Nombre = "Mari Carmen",
                Apellidos = "Fernandez Ortega",
                Telefono = "123456789",
                RolId = null,
                SecurityStamp = "b9a0c112-8b43-4b12-a83b-7ba36f680f6d"
            };
            
            await userManager.CreateAsync(usuario, password: usuario.PasswordHash);
            var usuarioFinal = await userManager.FindByEmailAsync(usuario.Email);

            var medico = new Medico
            {
                Id = usuarioFinal.Id,
                Dni = "12345678A",
                Sexo = Sexo.Mujer,
                Especialidad = Especialidad.Pediatría
            };

            await repositorioMedicos.CrearMedico(medico);

            await userManager.AddToRoleAsync(usuario, Constantes.RolMedico);
        }
    }
}
