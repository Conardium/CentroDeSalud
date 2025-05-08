using CentroDeSalud.Data;
using CentroDeSalud.Models;
using CentroDeSalud.Models.Requests;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;
using System.Security.Claims;

namespace CentroDeSalud.Controllers
{
    public class CitasController : Controller
    {
        private readonly IServicioMedicos servicioMedicos;
        private readonly IServicioCitas servicioCitas;
        private readonly UserManager<Usuario> userManager;

        public CitasController(IServicioMedicos servicioMedicos, IServicioCitas servicioCitas, UserManager<Usuario> userManager)
        {
            this.servicioMedicos = servicioMedicos;
            this.servicioCitas = servicioCitas;
            this.userManager = userManager;
        }

        private async Task<CrearCitaViewModel> ObtenerMedicos()
        {
            var modelo = new CrearCitaViewModel();
            modelo.Medicos = await servicioMedicos.ListarMedicos() ?? new List<Medico>();
            modelo.Fecha = DateTime.Now;
            return modelo;
        }

        [HttpGet]
        [Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> PedirCita()
        {
            var modelo = await ObtenerMedicos();

            return View(modelo);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> PedirCita(CrearCitaViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                modelo = await ObtenerMedicos();
                return View(modelo);
            }

            //Comprobamos si el usuario está en la BD
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (usuarioId is null)
            {
                TempData["Denegado"] = true;
                return RedirectToAction("AccesoDenegado", "Home");
            }

            var usuario = await userManager.FindByIdAsync(usuarioId);        

            if (usuario is null)
            {
                TempData["Denegado"] = true;
                return RedirectToAction("AccesoDenegado", "Home");
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

            var idCita = await servicioCitas.CrearCita(cita);

            TempData["CitaSolicitada"] = true;
            return RedirectToAction("CitaSolicitada", new {id = idCita});
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

            var resultado = await servicioCitas.SincronizarCita(usuarioIdGuid, id);

            if (resultado.TieneError)
            {
                ViewBag.IdCita = id;
                ViewBag.MensajeError = resultado.MensajeError;
                return RedirectToAction("CitaSolicitada", new { id });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> ObtenerCitasDisponibles([FromBody] ObtenerCitasRequest modelo)
        {
            var horasDisponibles = await servicioCitas.ListarHorasDisponibles(modelo.MedicoId, modelo.Fecha);
            if(horasDisponibles.TieneError)
            {
                return BadRequest(new {mensaje = horasDisponibles.MensajeError});
            }
            var horas = horasDisponibles.Datos.Select(hora => new SelectListItem(hora.ToString(@"hh\:mm"), hora.ToString()));
            return Ok(horas);
        }
    }
}
