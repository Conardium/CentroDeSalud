using CentroDeSalud.Data;
using CentroDeSalud.Enumerations;
using CentroDeSalud.Infrastructure.Utilidades;
using CentroDeSalud.Models;
using CentroDeSalud.Models.Requests;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Security.Claims;

namespace CentroDeSalud.Controllers
{
    public class CitasController : Controller
    {
        private readonly IServicioMedicos servicioMedicos;
        private readonly IServicioCitas servicioCitas;
        private readonly IServicioChats servicioChats;
        private readonly UserManager<Usuario> userManager;

        public CitasController(IServicioMedicos servicioMedicos, IServicioCitas servicioCitas, 
            UserManager<Usuario> userManager, IServicioChats servicioChats)
        {
            this.servicioMedicos = servicioMedicos;
            this.servicioCitas = servicioCitas;
            this.userManager = userManager;
            this.servicioChats = servicioChats;
        }

        [Authorize(Roles = Constantes.RolPaciente)]
        private async Task<CrearCitaViewModel> ObtenerMedicos()
        {
            var modelo = new CrearCitaViewModel();
            modelo.Medicos = await servicioMedicos.ListarMedicos() ?? new List<Medico>();
            modelo.Fecha = DateTime.Now;
            return modelo;
        }

        #region Funcionalidad para Pedir Cita, Vista de confirmación y Sincronización con Google Calendar (4 métodos)

        [HttpGet]
        [Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> PedirCita()
        {
            var modelo = await ObtenerMedicos();

            return View(modelo);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolPaciente)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PedirCita(CrearCitaViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo = await ObtenerMedicos();
                return View(modelo);
            }

            //Comprobamos si el usuario está en la BD
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (usuarioId is null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Denegado", "Avisos");
            }

            var usuario = await userManager.FindByIdAsync(usuarioId);

            if (usuario is null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Denegado", "Avisos");
            }

            //SI el usuario exite en la BD
            if (!Guid.TryParse(usuarioId, out Guid usuarioIdGuid))
                return null;

            var cita = new Cita
            {
                PacienteId = usuarioIdGuid,
                MedicoId = modelo.MedicoId,
                Fecha = modelo.Fecha,
                Hora = modelo.Hora,
                Motivo = modelo.Motivo,
                Detalles = modelo.Detalles
            };

            //Antes de crear la cita comprobamos si justamente no se ha pedido cita ya para esa hora.
            var citaOcupada = await servicioCitas.BuscarCitaPorFechaHora(cita.Fecha, cita.Hora);
            if (citaOcupada is not null)
            {
                ViewBag.MensajeError = @"<div class='alert alert-danger alert-dismissible fade show' role='alert'>
                    <i class='fa-solid fa-circle-exclamation'></i> Lo sentimos, esta cita ya ha sido seleccionada.
                    <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>";
                var Nuevomodelo = await ObtenerMedicos();
                return View(Nuevomodelo);
            }

            //Comprobamos que el usuario no tenga ya una cita pendiente con ese médico
            var citasPendientesPaciente = await servicioCitas.ObtenerCitasPendientesPorIdUsuario(usuarioIdGuid, Constantes.RolPaciente);
            if(citasPendientesPaciente.Any(c => c.MedicoId == cita.MedicoId))
            {
                ViewBag.MensajeError = @"<div class='alert alert-danger alert-dismissible fade show' role='alert'>
                    <i class='fa-solid fa-circle-exclamation'></i> Ya tiene una cita pendiente con el médico seleccionado.
                    <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>";
                var Nuevomodelo = await ObtenerMedicos();
                return View(Nuevomodelo);
            }

            //Creamos la cita
            var idCita = await servicioCitas.CrearCita(cita);

            //Creamos el chat entre el Medico y el Paciente de la cita
            var chat = new Chat()
            {
                PacienteId = usuario.Id,
                MedicoId = modelo.MedicoId
            };
            await servicioChats.CrearChat(chat);

            TempData["CitaSolicitada"] = true;
            return RedirectToAction("CitaSolicitada", new { id = idCita });
        }

        [Authorize(Roles = Constantes.RolPaciente)]
        public IActionResult CitaSolicitada(int id)
        {
            if (!TempData.ContainsKey("CitaSolicitada"))
                return NotFound();

            TempData.Remove("PasswordCambiado");
            ViewBag.IdCita = id;
            return View();
        }

        [Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> SincronizarCitaCalendario(int id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(usuarioId, out Guid usuarioIdGuid))
                return null;

            var resultado = new ResultadoOperacion<bool>();

            try
            {
                resultado = await servicioCitas.SincronizarCita(usuarioIdGuid, id);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                return View("ConsentimientoCalendario");
            }
            if (resultado.TieneError)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = resultado.MensajeError });
            }

            TempData["Acceso"] = true;
            return RedirectToAction("Exito", "Avisos", new { mensaje = "Su cita se ha sincronizado correctamente con su calendario de Google." });
        }

        #endregion

        #region Funcionalidad para Cancelar una cita (1 método)

        [Authorize(Roles = Constantes.RolPaciente + "," + Constantes.RolMedico)]
        public async Task<IActionResult> CancelarCita(int id, string returnUrl)
        {
            var citaPorCancelar = await servicioCitas.BuscarCitaPorId(id);
            if (citaPorCancelar == null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Ha habido un problema al intentar cancelar su cita. Su cita no existe." });
            }

            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(usuarioId, out Guid usuarioIdGuid))
                return null;

            //Comprobamos que el usuario que pide cancelar la cita sea uno de los pertenecientes a la cita
            if (citaPorCancelar.PacienteId != usuarioIdGuid && citaPorCancelar.MedicoId != usuarioIdGuid)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Denegado", "Avisos");
            }

            var resultado = await servicioCitas.CancelarCita(id);

            if (!resultado)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "No hemos podido cancelar su cita." });
            }

            return Redirect(returnUrl);
        }

        #endregion

        #region Funcionalidad para Eliminar una cita (1 método)

        [Authorize(Roles = Constantes.RolPaciente + "," + Constantes.RolMedico)]
        public async Task<IActionResult> EliminarCita(int id, string returnUrl)
        {
            //Obtenemos los datos de la cita a eliminar
            var citaPorEliminar = await servicioCitas.BuscarCitaPorId(id);
            if (citaPorEliminar == null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Ha habido un problema al intentar eliminar la cita. Su cita no existe." });
            }

            //Comprobamos que la cita tenga el estado de cancelada
            if(citaPorEliminar.EstadoCita != EstadoCita.Cancelada)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new {mensaje = "Ha habido un problema al intentar eliminar la cita. La cita debe de tener el estado Cancelada antes de poder eliminarla."});
            }

            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(usuarioId, out Guid usuarioIdGuid))
                return null;

            //Comprobamos que el usuario que pide eliminar la cita sea uno de los pertenecientes a la cita
            if (citaPorEliminar.PacienteId != usuarioIdGuid && citaPorEliminar.MedicoId != usuarioIdGuid)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Denegado", "Avisos");
            }

            var resultado = await servicioCitas.EliminarCita(id);

            if (!resultado)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "No hemos podido eliminar su cita." });
            }

            return Redirect(returnUrl);
        }

        #endregion

        #region Funcionalidad de FETCH para obtener las citas disponibles para una fecha (1 método)

        [HttpPost]
        [Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> ObtenerCitasDisponibles([FromBody] ObtenerCitasRequest modelo)
        {
            var horasDisponibles = await servicioCitas.ListarHorasDisponibles(modelo.MedicoId, modelo.Fecha);
            if (horasDisponibles.TieneError)
            {
                return BadRequest(new { mensaje = horasDisponibles.MensajeError });
            }
            var horas = horasDisponibles.Datos.Select(hora => new SelectListItem(hora.ToString(@"hh\:mm"), hora.ToString()));
            return Ok(horas);
        }

        #endregion

        #region Funcionalidad del Historial de Citas de un Usuario (1 método)

        [Authorize(Roles = Constantes.RolPaciente + "," + Constantes.RolMedico)]
        public async Task<IActionResult> HistorialCitas(Guid id)
        {
            //Comprobamos si el usuario que el id que recibimos coincide con el de la sesión
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(usuarioId, out Guid sesionId))
                return null;

            if(id != sesionId)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Denegado", "Avisos");
            }

            var rol = User.Claims.FirstOrDefault(c => c.Type.Contains("role"))?.Value;

            if(rol == null)
                return RedirectToAction("Index", "Home");

            var listadoCitas = await servicioCitas.ListarCitasPorUsuario(sesionId, rol);

            return View(listadoCitas);
        }

        #endregion
    }
}
