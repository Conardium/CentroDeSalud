using CentroDeSalud.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroDeSalud.Controllers
{
    public class AdminsController : Controller
    {
        [Authorize(Roles = Constantes.RolAdmin)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
