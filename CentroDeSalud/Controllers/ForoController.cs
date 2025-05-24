using Microsoft.AspNetCore.Mvc;

namespace CentroDeSalud.Controllers
{
    public class ForoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
