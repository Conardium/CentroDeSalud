using CentroDeSalud.Data;
using CentroDeSalud.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroDeSalud.Controllers
{
    public class AvisosController : Controller
    {
        [AllowAnonymous]
        public IActionResult Exito(string mensaje)
        {
            if (!TempData.ContainsKey("Acceso"))
                return NotFound();

            var aviso = new AvisoViewModel
            {
                Titulo = "Exito",
                Tipo = Constantes.OK,
                Mensaje = mensaje
            };

            TempData.Remove("Acceso");
            return View("AvisosGenerales", aviso);
        }

        [AllowAnonymous]
        public IActionResult Error(string mensaje)
        {
            if (!TempData.ContainsKey("Acceso"))
                return NotFound();

            var aviso = new AvisoViewModel
            {
                Titulo = "Error",
                Tipo = Constantes.Error,
                Mensaje = mensaje
            };

            TempData.Remove("Acceso");
            return View("AvisosGenerales", aviso);
        }

        [AllowAnonymous]
        public IActionResult Denegado(string mensaje)
        {
            if (!TempData.ContainsKey("Acceso"))
                return NotFound();

            var aviso = new AvisoViewModel
            {
                Titulo = "Acceso denegado",
                Tipo = Constantes.Denegado,
                Mensaje = "No tienes permisos para acceder a esta página de la web."
            };

            TempData.Remove("Acceso");
            return View("AvisosGenerales", aviso);
        }
    }
}
