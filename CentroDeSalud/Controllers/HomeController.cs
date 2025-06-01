using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Services;

namespace CentroDeSalud.Controllers;

public class HomeController : Controller
{
    private readonly IServicioPublicaciones servicioPublicaciones;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IServicioPublicaciones servicioPublicaciones)
    {
        _logger = logger;
        this.servicioPublicaciones = servicioPublicaciones;
    }

    public async Task<IActionResult> Index()
    {
        var modelo = new PaginaInicioViewModel();

        modelo.Destacadas = (ICollection<Models.Publicacion>)
            await servicioPublicaciones.ListarDestacadas();
        modelo.Publicaciones = (ICollection<Models.Publicacion>)
            await servicioPublicaciones.ListarUltimasNoDestacadas(4);

        return View(modelo);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
