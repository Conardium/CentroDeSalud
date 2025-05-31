using CentroDeSalud.Data;
using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CentroDeSalud.Controllers
{
    public class AdminsController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly IServicioMedicos servicioMedicos;
        private readonly IServicioDisponibilidadesMedicos servicioDisponibilidades;

        public AdminsController(UserManager<Usuario> userManager, IServicioMedicos servicioMedicos, 
            IServicioDisponibilidadesMedicos servicioDisponibilidades)
        {
            this.userManager = userManager;
            this.servicioMedicos = servicioMedicos;
            this.servicioDisponibilidades = servicioDisponibilidades;
        }

        [Authorize(Roles = Constantes.RolAdmin)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)]
        public IActionResult CrearMedico()
        {
            return View();
        }

        #region Funcionalidad para crear un perfil médico (1 método)

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearMedico(CrearMedicoViewModel modelo)
        {
            //Comprobamos el modelo
            if (!ModelState.IsValid)
                return View(modelo);

            //Comprobamos los horarios de consultas
            if (modelo.HorarioConsultas == null || !modelo.HorarioConsultas.Any())
            {
                ModelState.AddModelError("HorarioConsultas", "Indique un horario de consultas para el médico");
                return View(modelo);
            }
            else if (modelo.HorarioConsultas.Count <= 2)
            {
                ModelState.AddModelError("HorarioConsultas", "Indique al menos 3 franjas horarias");
                return View(modelo);
            }

            //Mapeamos los modelos del usuario y el medico
            var usuario = new Usuario()
            {
                Email = modelo.Email,
                Nombre = modelo.Nombre,
                Apellidos = modelo.Apellidos,
                Telefono = modelo.Telefono,
                RolId = null
            };
            var medico = new Medico()
            {
                Dni = modelo.Dni,
                Sexo = modelo.Sexo,
                Especialidad = modelo.Especialidad,
                DisponibilidadesMedico = modelo.HorarioConsultas,
            };

            //Creamos el usuario
            var resultado = await userManager.CreateAsync(usuario, password: modelo.PasswordHash);
            if (!resultado.Succeeded)
            {
                TempData["Tipo"] = Constantes.Error;
                TempData["Mensaje"] = "Ha habido un error al crear el usuario.";
            }

            //Creamos el medico
            medico.Id = usuario.Id;
            var guidCrearMedico = await servicioMedicos.CrearMedico(medico);

            //Si hubo un problema al crear el medico borramos el usuario
            if (guidCrearMedico == Guid.Empty)
            {
                await userManager.DeleteAsync(usuario);
                TempData["Tipo"] = Constantes.Error;
                TempData["Mensaje"] = "Ha habido un error al crear el médico.";
                return View(modelo);
            }

            //Asignamos las franjas horarias al médico
            foreach (var franja in modelo.HorarioConsultas)
            {
                franja.MedicoId = medico.Id;
                var retorno = await servicioDisponibilidades.CrearDisponibilidad(franja);
                if (!retorno)
                {
                    await userManager.DeleteAsync(usuario);
                    TempData["Tipo"] = Constantes.Error;
                    TempData["Mensaje"] = "Ha habido un error al guardar las franjas horarias del médico";
                    return View(modelo);
                }
            }

            //Le asignamos el rol "Medico"
            await userManager.AddToRoleAsync(usuario, Constantes.RolMedico);
            TempData["Tipo"] = Constantes.OK;
            TempData["Mensaje"] = "El perfil médico se ha creado correctamente.";

            return RedirectToAction("Index");
        }

        #endregion
    }
}
