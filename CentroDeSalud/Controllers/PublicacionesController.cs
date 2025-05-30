using CentroDeSalud.Data;
using CentroDeSalud.Enumerations;
using CentroDeSalud.Migrations;
using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Services;
using EllipticCurve.Utils;
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
        private readonly ILogger<PublicacionesController> _logger;
        private readonly IWebHostEnvironment webHostEnvironment;

        public PublicacionesController(IServicioPublicaciones servicioPublicaciones, UserManager<Usuario> userManager, 
            ILogger<PublicacionesController> logger, IWebHostEnvironment webHostEnvironment)
        {
            this.servicioPublicaciones = servicioPublicaciones;
            this.userManager = userManager;
            _logger = logger;
            this.webHostEnvironment = webHostEnvironment;
        }

        #region Funcionalidad para crear una publicación (2 métodos)

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

            //Si ha habido un error en la BD
            if(resultado == 0)
            {
                //Borramos el archivo que se haya insertado
                if (!string.IsNullOrEmpty(publicacion.ImagenURL))
                {
                    try
                    {
                        var rutaFisica = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", publicacion.ImagenURL.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (System.IO.File.Exists(rutaFisica))
                            System.IO.File.Delete(rutaFisica);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "No se pudo eliminar la imagen en {RutaImagen}", publicacion.ImagenURL);
                    }
                }

                TempData["Mensaje"] = "Ha habido un problema al intentar subir la publicación.";
                TempData["Tipo"] = Constantes.Error;
                return View(modelo);
            }

            TempData["Mensaje"] = "La publicación se ha creado correctamente.";
            TempData["Tipo"] = Constantes.OK;
            return RedirectToAction("Index", "Admins");
        }

        #endregion

        #region Funcionalidad para listar las publicaciones y ver los detalles de estas (2 métodos)

        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Listado()
        {
            var publicaciones = await servicioPublicaciones.ListarPublicaciones();
            return View(publicaciones);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detalles(string id)
        {
            //Comprobamos que la publicacion existe
            var publicacion = await servicioPublicaciones.BuscarPublicacionPorSlug(id);

            if (publicacion == null)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "La publicación no existe." });
            }

            return View(publicacion);
        }

        #endregion

        #region Funcionalidad para borrar una publicación (1 método)

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Borrar(int id)
        {
            //Comprobamos que el usuario existe
            var usuario = userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (usuario is null)
                return NotFound();

            //Comprobamos que la publicacion existe
            var publicacion = await servicioPublicaciones.BuscarPublicacionPorId(id);

            if(publicacion is null)
            {
                TempData["Tipo"] = Constantes.Error;
                TempData["Mensaje"] = "Ha habido un problema al intentar eliminar la publicación o esta no existe";
                return RedirectToAction("Listado");
            }

            var resultado = await servicioPublicaciones.EliminarPublicacion(id);

            if (!resultado)
            {
                TempData["Tipo"] = Constantes.Error;
                TempData["Mensaje"] = "No se ha podido eliminar la publicación";
                return RedirectToAction("Listado");
            }

            //Borrar de la carpeta imagenes la imagen de la publicacion (si tiene)
            if (!string.IsNullOrEmpty(publicacion.ImagenURL))
            {
                var rutaRelativa = publicacion.ImagenURL.TrimStart('/');
                var rutaImagen = Path.Combine(webHostEnvironment.WebRootPath, rutaRelativa);

                if (System.IO.File.Exists(rutaImagen))
                {
                    System.IO.File.Delete(rutaImagen);
                }
            }

            TempData["Tipo"] = Constantes.OK;
            TempData["Mensaje"] = "La publicación se ha borrado correctamente";

            return RedirectToAction("Listado");
        }

        #endregion

        #region Funcionalidad para Editar una publicación (2 métodos)

        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Editar(int id)
        {
            //Comprobamos que la publicación existe
            var publicacion = await servicioPublicaciones.BuscarPublicacionPorId(id);

            if(publicacion == null)
            {
                TempData["Tipo"] = Constantes.Error;
                TempData["Mensaje"] = "La publicación a editar no existe.";
                return RedirectToAction("Listado");
            }

            //Mapeamos el viewModel
            var modelo = new CrearPublicacionViewModel()
            {
                Id = publicacion.Id,
                Titulo = publicacion.Titulo,
                Cuerpo = publicacion.Cuerpo,
                Resumen = publicacion.Resumen,
                Destacada = publicacion.Destacada,
                EstadoPublicacion = publicacion.EstadoPublicacion,
                ImagenURL = publicacion.ImagenURL
            };

            return View(modelo);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Editar(CrearPublicacionViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                return View(modelo);
            }

            //Comprobamos que el usuario existe
            var usuario = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if(usuario == null)
            {
                return NotFound();
            }

            //Comprobamos que la publicacion exista
            var publicacionExiste = await servicioPublicaciones.BuscarPublicacionPorId(modelo.Id);

            if(publicacionExiste == null)
            {
                return View(modelo);
            }

            //Mapeamos el modelo
            var publicacion = new Publicacion()
            {
                Id = modelo.Id,
                Titulo = modelo.Titulo,
                Cuerpo = modelo.Cuerpo,
                Resumen = modelo.Resumen,
                FechaModificacion = DateTime.Now,
                Slug = servicioPublicaciones.GenerarSlug(modelo.Titulo),
                Destacada = modelo.Destacada,
                EstadoPublicacion = modelo.EstadoPublicacion
            };

            // Si el usuario subió una nueva imagen
            if (modelo.Imagen != null && modelo.Imagen.Length > 0)
            {
                // 1. Eliminar imagen anterior
                if (!string.IsNullOrEmpty(publicacionExiste.ImagenURL))
                {
                    var rutaRelativa = publicacionExiste.ImagenURL.TrimStart('/');
                    var rutaImagen = Path.Combine(webHostEnvironment.WebRootPath, rutaRelativa);

                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }
                }

                // 2. Guardar nueva imagen
                var nuevoNombre = Guid.NewGuid().ToString() + Path.GetExtension(modelo.Imagen.FileName);
                var rutaCarpeta = Path.Combine(webHostEnvironment.WebRootPath, "images", "uploads");

                if (!Directory.Exists(rutaCarpeta))
                    Directory.CreateDirectory(rutaCarpeta);

                var rutaCompleta = Path.Combine(rutaCarpeta, nuevoNombre);
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await modelo.Imagen.CopyToAsync(stream);
                }

                publicacion.ImagenURL = "/images/uploads/" + nuevoNombre;
            }

            //Guardamos los cambios en la BD
            var resultado = await servicioPublicaciones.ActualizarPublicacion(publicacion);

            if (!resultado)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Error", "Avisos", new { mensaje = "Ha habido un problema al intentar editar la publicación." });
            }

            TempData["Tipo"] = Constantes.OK;
            TempData["Mensaje"] = "La publicación se ha actualizado correctamente";
            return RedirectToAction("Listado");
        }

        #endregion
    }
}
