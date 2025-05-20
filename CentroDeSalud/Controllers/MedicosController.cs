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
    public class MedicosController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly IServicioMedicos servicioMedicos;
        private readonly IServicioDisponibilidadesMedicos servicioDisponibilidades;
        private readonly IServicioCitas servicioCitas;

        public MedicosController(UserManager<Usuario> userManager, IServicioMedicos servicioMedicos, 
            IServicioDisponibilidadesMedicos servicioDisponibilidades, IServicioCitas servicioCitas)
        {
            this.userManager = userManager;
            this.servicioMedicos = servicioMedicos;
            this.servicioDisponibilidades = servicioDisponibilidades;
            this.servicioCitas = servicioCitas;
        }

        [AllowAnonymous]
        public async Task<IActionResult> ListadoMedicos()
        {
            var medicos = await servicioMedicos.ListarMedicos();

            return View(medicos);
        }

        [AllowAnonymous]
        public async Task<IActionResult> PerfilPublico(Guid id)
        {
            //El perfil de los medicos es publico y todos pueden verlo

            //Obtenemos los datos del medico
            var usuarioMedico = await userManager.FindByIdAsync(id.ToString());
            var medico = await servicioMedicos.ObtenerMedicoPorId(id);

            if (usuarioMedico == null || medico == null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new {mensaje = "Ha habido un problema al recoger los datos del médico"});
            }

            //Mapeamos el PerfilViewModel
            var perfil = new PerfilViewModel
            {
                Id = id,
                Email = usuarioMedico.Email,
                Nombre = usuarioMedico.Nombre,
                Apellidos = usuarioMedico.Apellidos,
                Telefono = usuarioMedico.Telefono,
                RolId = usuarioMedico.RolId,
                Dni = medico.Dni,
                Sexo = medico.Sexo,
                Especialidad = medico.Especialidad
            };
            //Recogemos las disponibilidades del médico (Horario de consultas)
            perfil.DisponibilidadesMedico = (ICollection<DisponibilidadMedico>)await servicioDisponibilidades.ObtenerHorarioMedico(id);

            return View("Perfil", perfil);
        }

        [Authorize(Roles = Constantes.RolMedico)]
        public async Task<IActionResult> Perfil(Guid id)
        {
            //Comprobamos si el Id de la sesion se corresponde con el id del perfil a acceder
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(usuarioId, out Guid usuarioIdGuid))
                return null;

            if (usuarioIdGuid == Guid.Empty || usuarioIdGuid != id)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Denegado", "Avisos");
            }

            //Obtenemos los datos del medico
            var usuarioMedico = await userManager.FindByIdAsync(id.ToString());
            var medico = await servicioMedicos.ObtenerMedicoPorId(id);

            if (usuarioMedico == null || medico == null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Ha habido un problema al recoger los datos del médico" });
            }

            //Mapeamos el PerfilViewModel
            var perfil = new PerfilViewModel
            {
                Id = id,
                Email = usuarioMedico.Email,
                Nombre = usuarioMedico.Nombre,
                Apellidos = usuarioMedico.Apellidos,
                Telefono = usuarioMedico.Telefono,
                RolId = usuarioMedico.RolId,
                Dni = medico.Dni,
                Sexo = medico.Sexo,
                Especialidad = medico.Especialidad
            };
            //Recogemos las disponibilidades del médico (Horario de consultas) y sus citas
            perfil.DisponibilidadesMedico = (ICollection<DisponibilidadMedico>)await servicioDisponibilidades.ObtenerHorarioMedico(id);
            perfil.Citas = (ICollection<Cita>)await servicioCitas.ObtenerCitasPendientesPorIdUsuario(id, Constantes.RolMedico);

            return View(perfil);
        }
    }
}
