using CentroDeSalud.Data;
using CentroDeSalud.Models;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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

                await _userManager.AddToRoleAsync(usuario, Constantes.RolPaciente);



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
    }
}
