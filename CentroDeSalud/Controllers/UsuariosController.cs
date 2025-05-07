using CentroDeSalud.Data;
using CentroDeSalud.Enumerations;
using CentroDeSalud.Infrastructure.Services;
using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Repositories;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CentroDeSalud.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly IServicioPacientes servicioPaciente;
        private readonly IServicioEmail servicioEmail;
        private readonly IServicioUsuariosLoginsExternos servicioUsuariosLoginsExternos;

        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager,
            IServicioPacientes servicioPaciente, IServicioEmail servicioEmail, IServicioUsuariosLoginsExternos servicioUsuariosLoginsExternos)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.servicioPaciente = servicioPaciente;
            this.servicioEmail = servicioEmail;
            this.servicioUsuariosLoginsExternos = servicioUsuariosLoginsExternos;
        }

        private string ObtenerClaim(ClaimsPrincipal principal, string tipoClaim)
        {
            return principal.HasClaim(c => c.Type == tipoClaim) ? principal.FindFirstValue(tipoClaim) : null;
        }

        public IActionResult RegisterPaciente()
        {
            return View();    
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPaciente(RegisterPacienteViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

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


            var resultado = await userManager.CreateAsync(usuario, password: modelo.PasswordHash);
            if (resultado.Succeeded)
            {
                paciente.Id = usuario.Id;
                await servicioPaciente.CrearPacienteAsync(paciente);

                await userManager.AddToRoleAsync(usuario, Constantes.RolPaciente); //Le asignamos el rol "Paciente"

                //Comprobamos si el register proviene de un proveedor externo
                if (TempData["LoginExterno"] != null)
                {
                    var loginExterno = new UsuarioLoginExterno();
                    if (TempData["LoginExterno"] is string json)
                        loginExterno = JsonSerializer.Deserialize<UsuarioLoginExterno>(json);

                    loginExterno.UsuarioId = usuario.Id;

                    //Borramos los TempData
                    TempData.Remove("LoginExterno");

                    var resultadoAgregarLogin = await servicioUsuariosLoginsExternos.AgregarLoginExterno(loginExterno);

                    if (resultadoAgregarLogin.Succeeded)
                    {
                        await signInManager.SignInAsync(usuario, isPersistent: true, loginExterno.LoginProvider);
                        return LocalRedirect("/");
                    }
                    else
                        return View(modelo);
                }

                await signInManager.SignInAsync(usuario, isPersistent: false);
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
                return View(modelo);

            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame,
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

        // =============================== LOGIN EXTERNO =================================
        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult LoginExterno(string proveedor, string urlRetorno = null)
        {
            var urlRedireccion = Url.Action("RecogerDatosUsuarioExterno", values: new { urlRetorno });
            var propiedades = signInManager
                .ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion);

            if (proveedor == "Google")
                propiedades.Items["prompt"] = "select_account"; //Obligo a seleccionar una cuenta siempre

            return new ChallengeResult(proveedor, propiedades);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RecogerDatosUsuarioExterno(string urlRetorno = null,
            string remoteError = null)
        {
            urlRetorno = urlRetorno ?? Url.Content("~/");

            var mensaje = "";

            if (remoteError is not null)
            {
                mensaje = $"Error del proveedor externo: {remoteError}";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                mensaje = "Error cargando la data de login externo";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var resultadoLoginExterno = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
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

            var registroVM = new RegisterPacienteViewModel { 
                Email = email, 
                Nombre = nombre,
                Apellidos = apellidos,
                FechaNacimiento = DateTime.Now,
                Sexo = Sexo.NoSeleccionado,
                GrupoSanguineo = GrupoSanguineo.NoSeleccionado};

            var accessToken = info.AuthenticationTokens?.FirstOrDefault(t => t.Name == "access_token")?.Value;
            var refreshToken = info.AuthenticationTokens?.FirstOrDefault(t => t.Name == "refresh_token")?.Value;
            var expiresAtString = info.AuthenticationTokens?.FirstOrDefault(t => t.Name == "expires_at")?.Value;

            DateTime? expiresAt = null;
            if(DateTime.TryParse(expiresAtString, out var parsedFecha))
                expiresAt = parsedFecha;

            var DatosProveedor = new UsuarioLoginExterno
            {
                LoginProvider = info.LoginProvider,
                ProviderKey = info.ProviderKey,
                ProviderDisplayName = info.ProviderDisplayName,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenExpiraEn = expiresAt
            };
            //Guardamos temporalmente estos datos para a futuro guardarlos en el usuarioLoginExterno
            TempData["LoginExterno"] = JsonSerializer.Serialize(DatosProveedor);

            return View("RegisterPaciente", registroVM);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult OlvidarPassword(string mensaje = "")
        {
            ViewBag.Mensaje = mensaje;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> OlvidarPassword(OlvidarPasswordViewModel modelo)
        {
            var mensaje = "¡Todo listo! Si el email dado se corresponde con uno de nuestros usuarios, en su bandeja de entrada podrá encontrar un correo con las instrucciones para recuperar su contraseña";
            ViewBag.Mensaje = mensaje;
            ModelState.Clear();

            var usuario = await userManager.FindByEmailAsync(modelo.Email);

            //Si el usuario no existe
            if (usuario is null)
                return View();

            var codigo = await userManager.GeneratePasswordResetTokenAsync(usuario);
            //Convertimos a base64
            var codigoBase64 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));
            var enlace = Url.Action("RestablecerPassword", "Usuarios", new {codigo = codigoBase64}, protocol: Request.Scheme);

            var nombreCompleto = usuario.Nombre + " " + usuario.Apellidos;
            await servicioEmail.EnviarRecuperarPassword(usuario.Email, usuario.Nombre, nombreCompleto, enlace);

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RestablecerPassword(string codigo = null)
        {
            if (codigo is null)
            {
                TempData["Denegado"] = true;
                return RedirectToAction("AccesoDenegado", "Home");
            }

            var modelo = new RestablecerPasswordViewModel();
            modelo.CodigoReseteo = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(codigo));
            return View(modelo);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RestablecerPassword(RestablecerPasswordViewModel modelo)
        {
            var usuario = await userManager.FindByEmailAsync(modelo.Email);
            TempData["PasswordCambiado"] = true;

            if (usuario is null)
            {
                return RedirectToAction("PasswordCambiado");
            }
                

            var resultado = await userManager.ResetPasswordAsync(usuario, modelo.CodigoReseteo, modelo.Password);
            return RedirectToAction("PasswordCambiado");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PasswordCambiado()
        {
            //Para evitar acceder manualmente a esta vista
            if (!TempData.ContainsKey("PasswordCambiado"))
                return NotFound();

            TempData.Remove("PasswordCambiado");
            return View();
        }

        /*
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(usuarioId, out Guid id))
                return null;
         */
    }
}
