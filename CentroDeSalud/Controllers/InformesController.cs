using CentroDeSalud.Data;
using CentroDeSalud.Enumerations;
using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CentroDeSalud.Controllers
{
    public class InformesController : Controller
    {
        private readonly IServicioInformes servicioInformes;
        private readonly IServicioPacientes servicioPacientes;
        private readonly IServicioMedicos servicioMedicos;
        private readonly IServicioCitas servicioCitas;
        private readonly UserManager<Usuario> userManager;

        public InformesController(IServicioInformes servicioInformes, IServicioPacientes servicioPacientes,
            IServicioMedicos servicioMedicos, IServicioCitas servicioCitas, UserManager<Usuario> userManager)
        {
            this.servicioInformes = servicioInformes;
            this.servicioPacientes = servicioPacientes;
            this.servicioMedicos = servicioMedicos;
            this.servicioCitas = servicioCitas;
            this.userManager = userManager;
        }

        [Authorize(Roles = Constantes.RolMedico)]
        public async Task<IActionResult> MisPacientes(Guid id)
        {
            //Mostramos los pacientes que el medico tiene con citas pendientes
            var pacientes = await servicioCitas.ListarPacientesDelMedico(id);

            if(pacientes == null || !pacientes.Any())
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Parece que no tiene ningún paciente asignado en estos momentos." });
            }

            return View(pacientes);
        }

        [Authorize(Roles = Constantes.RolMedico)]
        [HttpGet]
        public async Task<IActionResult> Crear(Guid pacienteId)
        {
            var idMedico = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Recogemos los datos del paciente
            var usuarioPaciente = await userManager.FindByIdAsync(pacienteId.ToString());
            var usuarioMedico = await userManager.FindByIdAsync(idMedico);

            if(usuarioMedico is null || usuarioPaciente is null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "No se ha podido crear el formulario correctamente" });
            }

            //Mapeamos el ViewModel
            var modelo = new CrearInformeViewModel
            {
                NombrePaciente = usuarioPaciente.Nombre + " " + usuarioPaciente.Apellidos,
                PacienteId = usuarioPaciente.Id,
                NombreMedico = usuarioMedico.Nombre + " " + usuarioMedico.Apellidos,
                MedicoId = usuarioMedico.Id
            };


            return View(modelo);
        }

        [Authorize(Roles = Constantes.RolMedico)]
        [HttpPost]
        public async Task<IActionResult> Crear(CrearInformeViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var informe = new Informe();

            //Si se ha adjuntado un archivo lo añadimos al proyecto y al modelo
            if (modelo.ArchivoAdjunto != null && modelo.ArchivoAdjunto.Length > 0)
            {
                // Validamos la extensión
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var extension = Path.GetExtension(modelo.ArchivoAdjunto.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("ArchivoAdjunto", "Tipo de archivo no permitido.");
                    return View(modelo);
                }

                // Ruta de guardado
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "uploads");

                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await modelo.ArchivoAdjunto.CopyToAsync(stream);
                }

                //Guardamos la ruta del archivo en el modelo
                var rutaRelativa = Path.Combine("/images/uploads", nombreArchivo).Replace("\\", "/");
                informe.ArchivosAdjuntos = rutaRelativa;
            }

            //Rellenamos el resto del modelo
            informe.PacienteId = modelo.PacienteId;
            informe.MedicoId = modelo.MedicoId;
            informe.FechaCreacion = DateTime.Now;
            informe.EstadoInforme = EstadoInforme.Borrador;
            informe.Diagnostico = modelo.Diagnostico;
            informe.Tratamiento = modelo.Tratamiento;
            informe.Notas = modelo.Notas;
            informe.Recomendaciones = modelo.Recomendaciones;

            await servicioInformes.CrearInforme(informe);

            TempData["Acceso"] = true;
            return RedirectToAction("Exito", "Avisos", new { mensaje = "El informe del paciente se ha creado correctamente." });
        }
    }
}
