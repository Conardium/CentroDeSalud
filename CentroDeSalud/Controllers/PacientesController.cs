using CentroDeSalud.Data;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Models;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CentroDeSalud.Controllers
{
    public class PacientesController : Controller
    {
        private readonly IServicioPacientes servicioPacientes;
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly IServicioUsuariosLoginsExternos servicioLoginsExternos;
        private readonly IServicioCitas servicioCitas;

        public PacientesController(IServicioPacientes servicioPacientes, UserManager<Usuario> userManager,
            IServicioUsuariosLoginsExternos servicioLoginsExternos, IServicioCitas servicioCitas, SignInManager<Usuario> signInManager)
        {
            this.servicioPacientes = servicioPacientes;
            this.userManager = userManager;
            this.servicioLoginsExternos = servicioLoginsExternos;
            this.servicioCitas = servicioCitas;
            this.signInManager = signInManager;
        }

        #region Funcionalidad para acceder al perfil del paciente y editar sus datos

        [Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> Perfil(Guid id)
        {
            //El perfil de los pacientes es privado, por lo que solamente tiene acceso el propio paciente

            //Comprobamos si el Id de la sesion se corresponde con el id del perfil a acceder
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(usuarioId, out Guid usuarioIdGuid))
                return null;

            if (usuarioIdGuid == Guid.Empty || usuarioIdGuid != id)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Denegado", "Avisos");
            }

            //Obtenemos los datos del usuario y del paciente
            var usuarioPaciente = await userManager.FindByIdAsync(usuarioId);
            var paciente = await servicioPacientes.ObtenerPacientePorId(id);

            if (usuarioPaciente == null || paciente == null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Ha habido un problema al recoger los datos del usuario" });
            }

            //Mapeamos el PerfilViewModel
            var perfil = new PerfilViewModel
            {
                Id = id,
                Email = usuarioPaciente.Email,
                Nombre = usuarioPaciente.Nombre,
                Apellidos = usuarioPaciente.Apellidos,
                Telefono = usuarioPaciente.Telefono,
                RolId = usuarioPaciente.RolId,
                Dni = paciente.Dni,
                Sexo = paciente.Sexo,
                GrupoSanguineo = paciente.GrupoSanguineo,
                Direccion = paciente.Direccion,
                FechaNacimiento = paciente.FechaNacimiento
            };
            //Recogemos los logins externos y las citas pendientes del paciente
            perfil.LoginsExternos = (ICollection<UsuarioLoginExterno>)await servicioLoginsExternos.ListadoLoginsExternos(perfil.Id);
            perfil.Citas = (ICollection<Cita>)await servicioCitas.ObtenerCitasPendientesPorIdUsuario(perfil.Id, Constantes.RolPaciente);

            return View(perfil);
        }

        [HttpGet]
        [Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> EditarPerfil(Guid id)
        {
            //Comprobamos que el usuario que vaya a editar los datos sea el mismo que la sesión
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(usuarioId, out Guid usuarioIdGuid))
                return null;

            if (usuarioIdGuid == Guid.Empty || usuarioIdGuid != id)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Denegado", "Avisos");
            }

            //Obtenemos los datos del medico
            var usuarioPaciente = await userManager.FindByIdAsync(id.ToString());
            var paciente = await servicioPacientes.ObtenerPacientePorId(id);

            if (usuarioPaciente == null || paciente == null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Ha habido un problema al recoger los datos del paciente" });
            }

            //Mapeamos el EditarPerfilViewModel
            var editarPerfil = new EditarPerfilViewModel
            {
                Id = id,
                Nombre = usuarioPaciente.Nombre,
                Apellidos = usuarioPaciente.Apellidos,
                Telefono = usuarioPaciente.Telefono,
                Sexo = paciente.Sexo,
                Direccion = paciente.Direccion
            };

            return View(editarPerfil);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> EditarPerfil(EditarPerfilViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var usuario = await userManager.FindByIdAsync(modelo.Id.ToString());

            if (usuario is null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Ha habido un problema al acutalizar el usuario. El usuario no existe." });
            }

            var resultado = await servicioPacientes.ActualizarDatosPerfil(modelo);

            if (!resultado)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Ha habido un problema al acutalizar el usuario." });
            }

            var usuarioActualizado = await userManager.FindByIdAsync(modelo.Id.ToString());

            await signInManager.SignInAsync(usuarioActualizado, isPersistent: false);

            return RedirectToAction("Perfil", new { id = modelo.Id });
        }

        #endregion
    }
}
