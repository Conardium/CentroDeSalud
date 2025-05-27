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
    public class PublicacionesController : Controller
    {
        private readonly IServicioPublicaciones servicioPublicaciones;
        private readonly UserManager<Usuario> userManager;

        public PublicacionesController(IServicioPublicaciones servicioPublicaciones, UserManager<Usuario> userManager)
        {
            this.servicioPublicaciones = servicioPublicaciones;
            this.userManager = userManager;
        }

        #region Funcionalidad para crear una publicación

        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Crear(CrearPublicacionViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            //Comprobamos que el usuario exista en la BD
            var usuario = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var publicacion = new Publicacion();

            //Si se ha añadido la imagen lo añadimos al proyecto y al modelo
            if (modelo.Imagen != null && modelo.Imagen.Length > 0)
            {
                // Validamos la extensión
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(modelo.Imagen.FileName).ToLowerInvariant();

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
                    await modelo.Imagen.CopyToAsync(stream);
                }

                //Guardamos la ruta del archivo en el modelo
                var rutaRelativa = Path.Combine("/images/uploads", nombreArchivo).Replace("\\", "/");
                publicacion.ImagenURL = rutaRelativa;
            }

            //Creamos el slug para la vista de detalles de la publicacion
            var slug = servicioPublicaciones.GenerarSlug(modelo.Titulo);

            //Rellenamos el resto del modelo
            publicacion.Titulo = modelo.Titulo;
            publicacion.Cuerpo = modelo.Cuerpo;
            publicacion.Resumen = modelo.Resumen;
            publicacion.FechaPublicacion = DateTime.Now;
            publicacion.UsuarioId = usuario.Id;
            publicacion.Slug = slug;
            publicacion.Destacada = modelo.Destacada;
            publicacion.EstadoPublicacion = modelo.EstadoPublicacion;

            var resultado = await servicioPublicaciones.CrearPublicacion(publicacion);

            if(resultado == 0)
            {
                TempData["Mensaje"] = "Ha habido un problema al intentar subir la publicación.";
                TempData["Tipo"] = Constantes.Error;
                return View(modelo);
            }

            TempData["Mensaje"] = "La publicación se ha creado correctamente.";
            TempData["Tipo"] = Constantes.OK;
            return RedirectToAction("Index", "Admins");
        }

        #endregion

        #region Funcionalidad para listar las publicaciones

        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Listado()
        {
            var publicaciones = await servicioPublicaciones.ListarPublicaciones();
            return View(publicaciones);
        }

        #endregion
    }
}
