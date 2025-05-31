using CentroDeSalud.Data;
using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Enumerations;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CentroDeSalud.Controllers
{
    public class ForoController : Controller
    {
        private readonly IServicioPreguntasForos servicioPreguntas;
        private readonly IServicioRespuestasForos servicioRespuestas;
        private readonly UserManager<Usuario> userManager;

        public ForoController(IServicioPreguntasForos servicioPreguntas, IServicioRespuestasForos servicioRespuestas, 
            UserManager<Usuario> userManager)
        {
            this.servicioPreguntas = servicioPreguntas;
            this.servicioRespuestas = servicioRespuestas;
            this.userManager = userManager;
        }

        #region Funcionalidad para listas las preguntas del foro (2 métodos)

        [AllowAnonymous]
        //El indice mostrará una vista inicial del foro con las ultimas 5 preguntas y el formulario para hacer una
        public async Task<IActionResult> Index()
        {
            var listadoPreguntas = await servicioPreguntas.ListarCincoUltimasPreguntas();
            if (listadoPreguntas == null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Ha habido un error al cargar las últimas preguntas del foro." });
            }

            //Obtenemos la/s respuestas de cada pregunta
            foreach (var pregunta in listadoPreguntas)
            {
                var respuestas = await servicioRespuestas.ListarRespuestasPorPreguntaId(pregunta.Id);
                pregunta.RespuestasForo = (ICollection<Models.RespuestaForo>)respuestas;
            }

            var modelo = new PublicarPreguntaViewModel()
            {
                Preguntas = listadoPreguntas
            };

            return View(modelo);
        }

        [AllowAnonymous]
        //En esta vista se encuentra el listado de todas las preguntas junto con un filtro
        public async Task<IActionResult> HistorialPreguntas()
        {
            var listadoPreguntas = await servicioPreguntas.ListarPreguntasForo();
            if (listadoPreguntas == null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Ha habido un error al cargar las preguntas del foro." });
            }

            //Obtenemos la/s respuestas de cada pregunta
            foreach (var pregunta in listadoPreguntas)
            {
                var respuestas = await servicioRespuestas.ListarRespuestasPorPreguntaId(pregunta.Id);
                pregunta.RespuestasForo = (ICollection<Models.RespuestaForo>)respuestas;
            }

            var modelo = new PublicarPreguntaViewModel()
            {
                Preguntas = listadoPreguntas
            };

            return View(modelo);
        }

        #endregion

        #region Funcionalidad Publicar una pregunta (1 método)

        [HttpPost]
        [Authorize(Roles = Constantes.RolPaciente)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublicarPregunta(PublicarPreguntaViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                TempData["Tipo"] = Constantes.Error;
                TempData["Mensaje"] = "Error: La pregunta contiene carácteres inválidos";
                return RedirectToAction("Index");
            }

            //Comprobamos que el usuario existe en la BD
            var usuario = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if(usuario is null)
            {
                return NotFound();
            }

            //Mapeamos el modelo
            var pregunta = new PreguntaForo()
            {
                PacienteId = usuario.Id,
                Titulo = modelo.Titulo,
                Texto = modelo.Detalles,
                FechaCreacion = DateTime.Now,
                EstadoPregunta = EstadoPregunta.Abierta
            };

            var resultado = await servicioPreguntas.CrearPreguntaForo(pregunta);

            if(resultado == 0)
            {
                TempData["Tipo"] = Constantes.Error;
                TempData["Mensaje"] = "Error: Ha habido un problema al publicar su pregunta";
                return RedirectToAction("Index");
            }

            TempData["Tipo"] = Constantes.OK;
            TempData["Mensaje"] = "La pregunta se ha publicado correctamente.";
            return RedirectToAction("Index");
        }

        #endregion

        #region Funcionalidad Responder una pregunta (2 métodos)

        [Authorize(Roles = Constantes.RolMedico)]
        public async Task<IActionResult> ResponderPregunta(int id)
        {
            //Recogemos la pregunta y la enviamos a la vista
            var pregunta = await servicioPreguntas.ObtenerPreguntaPorId(id);

            if(pregunta is null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "No se ha podido encontrar la pregunta." });
            }

            var modelo = new PublicarRespuestaViewModel();

            modelo.PreguntaForo = pregunta;
            modelo.PreguntaForoId = pregunta.Id;

            return View(modelo);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolMedico)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublicarRespuesta(PublicarRespuestaViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                TempData["Tipo"] = Constantes.Error;
                TempData["Mensaje"] = "Error: La respuesta contiene carácteres inválidos";
                return RedirectToAction("Index");
            }

            //Comprobamos que el usuario existe en la BD
            var usuario = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (usuario is null)
            {
                return NotFound();
            }

            //Mapeamos el modelo
            var respuesta = new RespuestaForo()
            {
                MedicoId = usuario.Id,
                PreguntaForoId = modelo.PreguntaForoId,
                Texto = modelo.Respuesta,
                FechaRespuesta = DateTime.Now
            };

            var resultado = await servicioRespuestas.CrearRespuestaForo(respuesta);

            if (resultado == 0)
            {
                TempData["Tipo"] = Constantes.Error;
                TempData["Mensaje"] = "Error: Ha habido un problema al publicar su respuesta";
                return RedirectToAction("Index");
            }

            TempData["Tipo"] = Constantes.OK;
            TempData["Mensaje"] = "La respuesta se ha publicado correctamente.";
            return RedirectToAction("Index");
        }

        #endregion
    }
}
