using CentroDeSalud.Data;
using CentroDeSalud.Services;
using CentroDeSalud.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using CentroDeSalud.Models;

namespace CentroDeSalud.Controllers
{
    public class ChatsController : Controller
    {
        private readonly IServicioChats servicioChats;
        private readonly IServicioMensajes servicioMensajes;

        public ChatsController(IServicioChats servicioChats, IServicioMensajes servicioMensajes)
        {
            this.servicioChats = servicioChats;
            this.servicioMensajes = servicioMensajes;
        }

        [Authorize(Roles = Constantes.RolPaciente + "," + Constantes.RolMedico)]
        public async Task<IActionResult> Index(Guid id)
        {
            //Comprobamos que el usuario que va a acceder al listado de chats sea el propio usuario de la sesion
            var sesionId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (sesionId != id.ToString())
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Denegado", "Avisos");
            }

            var rol = User.FindFirstValue(ClaimTypes.Role); 
            
            var listadoChat = rol == Constantes.RolPaciente 
                ? await servicioChats.ListarChatsPorPaciente(id) 
                : await servicioChats.ListarChatsPorMedico(id);

            return View(listadoChat);
        }

        [Authorize(Roles = Constantes.RolPaciente + "," + Constantes.RolMedico)]
        public async Task<IActionResult> Conversacion(Guid id)
        {
            //Comprobamos que la conversación pertenezca al usuario que está logueado
            var sesionId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(sesionId, out Guid usuarioIdGuid))
                return null;

            var resultado = await servicioChats.ExisteChat(id, usuarioIdGuid);

            //Si la conversación NO pertenece a la sesión actual le redirigimos a la vista de acceso denegado
            if (!resultado)
            {
                TempData["Acceso"] = true;
                return RedirectToAction("Denegado", "Avisos");
            }

            var listadoMensajes = await servicioMensajes.ListarMensajesPorChatId(id);

            var modelo = new MensajeViewModel
            {
                ChatId = id,
                UsuarioId = usuarioIdGuid,
                Mensajes = listadoMensajes
            };

            return View(modelo);
        }
    }
}
