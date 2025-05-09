using CentroDeSalud.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroDeSalud.Controllers
{
    public class AvisosController : Controller
    {
        [AllowAnonymous]
        public IActionResult AvisosGenerales(AvisoViewModel modelo)
        {
            if (!TempData.ContainsKey("Acceso"))
                return NotFound();

            TempData.Remove("Acceso");
            return View(modelo);
        }
    }
}
