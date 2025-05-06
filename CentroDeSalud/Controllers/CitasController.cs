using CentroDeSalud.Data;
using CentroDeSalud.Models;
using CentroDeSalud.Models.Requests;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;

namespace CentroDeSalud.Controllers
{
    public class CitasController : Controller
    {
        private readonly IServicioMedicos servicioMedicos;
        private readonly IServicioCitas servicioCitas;

        public CitasController(IServicioMedicos servicioMedicos, IServicioCitas servicioCitas)
        {
            this.servicioMedicos = servicioMedicos;
            this.servicioCitas = servicioCitas;
        }

        private async Task<CrearCitaViewModel> ObtenerMedicos()
        {
            var modelo = new CrearCitaViewModel();
            modelo.Medicos = await servicioMedicos.ListarMedicos() ?? new List<Medico>();
            modelo.Fecha = DateTime.Now;
            return modelo;
        }

        [HttpGet]
        //[Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> PedirCita()
        {
            var modelo = await ObtenerMedicos();

            return View(modelo);
        }

        [HttpPost]
        //[Authorize(Roles = Constantes.RolPaciente)]
        public async Task<IActionResult> PedirCita(CrearCitaViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                modelo = await ObtenerMedicos();
                return View(modelo);
            }

            await servicioCitas.ListarHorasDisponibles(modelo.MedicoId, modelo.Fecha);

            return View(modelo);
        }

        [HttpPost]
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
