using CentroDeSalud.Data;
using CentroDeSalud.Enumerations;
using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CentroDeSalud.Controllers
{
    public class UsuariosController : Controller
    {
        public readonly UserManager<Usuario> _userManager;
        public readonly SignInManager<Usuario> _signInManager;
        public readonly IServicioPaciente _servicioPaciente;

        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IServicioPaciente servicioPaciente)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _servicioPaciente = servicioPaciente;
        }

        public IActionResult RegisterPaciente()
        {
            return View();    
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPaciente(RegisterPacienteViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new Usuario() { 
                Email = modelo.Email,
                Nombre = modelo.Nombre,
                Apellidos = modelo.Apellidos,
                Telefono = modelo.Telefono,
                RolId = null};

            var paciente = new Paciente() {
                Dni = modelo.Dni,
                FechaNacimiento = modelo.FechaNacimiento,
                GrupoSanguineo = modelo.GrupoSanguineo,
                Direccion = modelo.Direccion,
                Sexo = modelo.Sexo};


            var resultado = await _userManager.CreateAsync(usuario, password: modelo.PasswordHash);
            if (resultado.Succeeded)
            {
                paciente.Id = usuario.Id;
                await _servicioPaciente.CrearPacienteAsync(paciente);

                await _userManager.AddToRoleAsync(usuario, Constantes.RolPaciente); //Le asignamos el rol "Paciente"

                //Comprobamos si el register proviene de un proveedor externo
                if (TempData["LoginProvider"] != null && TempData["ProviderKey"] != null)
                {
                    var loginExterno = new UserLoginInfo(
                        TempData["LoginProvider"].ToString(),
                        TempData["ProviderKey"].ToString(),
                        TempData["ProviderDisplayName"].ToString());

                    //Borramos los TempData
                    TempData.Remove("LoginProvider");
                    TempData.Remove("ProviderKey");
                    TempData.Remove("ProviderDisplayName");

                    var resultadoAgregarLogin = await _userManager.AddLoginAsync(usuario, loginExterno);

                    if (resultadoAgregarLogin.Succeeded)
                    {
                        await _signInManager.SignInAsync(usuario, isPersistent: true, loginExterno.LoginProvider);
                        return LocalRedirect("/");
                    }
                    else
                        return View(modelo);
                }

                await _signInManager.SignInAsync(usuario, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame,
                lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o Contraseña incorrecto.");
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        // =============================== LOGIN EXTERNO =================================
        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult LoginExterno(string proveedor, string urlRetorno = null)
        {
            var urlRedireccion = Url.Action("RegistrarUsuarioExterno", values: new { urlRetorno });
            var propiedades = _signInManager
                .ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion);

            if (proveedor == "Google")
                propiedades.Items["prompt"] = "select_account"; //Obligo a seleccionar una cuenta siempre

            return new ChallengeResult(proveedor, propiedades);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuarioExterno(string urlRetorno = null,
            string remoteError = null)
        {
            urlRetorno = urlRetorno ?? Url.Content("~/");

            var mensaje = "";

            if (remoteError is not null)
            {
                mensaje = $"Error del proveedor externo: {remoteError}";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                mensaje = "Error cargando la data de login externo";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var resultadoLoginExterno = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            // Ya la cuenta existe
            if (resultadoLoginExterno.Succeeded)
                return LocalRedirect(urlRetorno);
            
            //Recogemos los datos que queramos del login externo y preparamos el ViewModel para el register
            string email = ObtenerClaim(info.Principal, ClaimTypes.Email);
            
            if(email == null)
            {
                mensaje = "Error leyendo el email del usuario del proveedor";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var nombre = ObtenerClaim(info.Principal, ClaimTypes.GivenName);
            if(nombre == null)
            {
                mensaje = "Error leyendo el nombre del usuario del proveedor";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var apellidos = ObtenerClaim(info.Principal, ClaimTypes.Surname);
            if(apellidos == null)
            {
                mensaje = "Error leyendo los apellidos del usuario del proveedor";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var registroVM = new RegisterPacienteViewModel { 
                Email = email, 
                Nombre = nombre,
                Apellidos = apellidos,
                FechaNacimiento = DateTime.Now,
                Sexo = Sexo.NoSeleccionado,
                GrupoSanguineo = GrupoSanguineo.NoSeleccionado};

            //Guardamos temporalmente estos datos para a futuro guardarlos en el usuarioLoginExterno
            TempData["LoginProvider"] = info.LoginProvider;
            TempData["ProviderKey"] = info.ProviderKey;
            TempData["ProviderDisplayName"] = info.ProviderDisplayName;

            return View("RegisterPaciente", registroVM);
            /*
            var resultadoCrearUsuario = await userManager.CreateAsync(usuario);

            if (!resultadoCrearUsuario.Succeeded)
            {
                mensaje = resultadoCrearUsuario.Errors.First().Description;
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var resultadoAgregarLogin = await userManager.AddLoginAsync(usuario, info);

            if (resultadoAgregarLogin.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true, info.LoginProvider);
                return LocalRedirect(urlRetorno);
            }
            */
        }

        private string ObtenerClaim(ClaimsPrincipal principal, string tipoClaim)
        {
            return principal.HasClaim(c => c.Type == tipoClaim) ? principal.FindFirstValue(tipoClaim) : null;
        }
    }
}
