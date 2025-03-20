using MetaAdsAnalyzer.Models;
using MetaAdsAnalyzer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MetaAdsAnalyzer.Controllers
{
    /// <summary>
    /// Controlador para la gestión de cuentas de usuario y autenticación
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        /// <summary>
        /// Constructor que recibe el servicio de usuarios
        /// </summary>
        /// <param name="userService">Servicio de usuarios</param>
        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Muestra la vista de inicio de sesión
        /// </summary>
        /// <param name="returnUrl">URL a la que redirigir después del inicio de sesión</param>
        /// <returns>Vista de inicio de sesión</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            // Siempre asignar un valor por defecto a returnUrl si es null
            ViewData["ReturnUrl"] = returnUrl ?? "/";
            return View(new LoginViewModel { ReturnUrl = returnUrl ?? "/" });
        }

        /// <summary>
        /// Procesa el formulario de inicio de sesión
        /// </summary>
        /// <param name="model">Datos del formulario</param>
        /// <param name="returnUrl">URL a la que redirigir después del inicio de sesión</param>
        /// <returns>Redirección o vista con errores</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Asegurarse de que returnUrl siempre tenga un valor
            model.ReturnUrl = model.ReturnUrl ?? "/";
            ViewData["ReturnUrl"] = model.ReturnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    // Intentar autenticar al usuario
                    var user = await _userService.AuthenticateAsync(model.Username, model.Password);

                    if (user != null)
                    {
                        // Crear las claims para el usuario
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, user.Role)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        // Crear las propiedades de autenticación
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                        };

                        // Iniciar sesión
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        // Redirigir a la URL de retorno o a la página principal
                        return RedirectToLocal(model.ReturnUrl);
                    }

                    ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error al iniciar sesión: {ex.Message}");
                }
            }

            // Si llegamos aquí, algo falló, volver a mostrar el formulario
            return View(model);
        }

        /// <summary>
        /// Cierra la sesión del usuario
        /// </summary>
        /// <returns>Redirección a la página principal</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        /// Muestra la vista de registro (solo para administradores)
        /// </summary>
        /// <returns>Vista de registro</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Procesa el formulario de registro
        /// </summary>
        /// <param name="model">Datos del formulario</param>
        /// <returns>Redirección o vista con errores</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Si no se especifica un rol, asignar "Usuario" por defecto
                    if (string.IsNullOrEmpty(model.Role))
                    {
                        model.Role = "Usuario";
                    }

                    // Registrar el usuario
                    var user = await _userService.RegisterUserAsync(model);

                    // Redirigir a la lista de usuarios
                    return RedirectToAction(nameof(UsersController.Index), "Users");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Ha ocurrido un error al registrar el usuario: {ex.Message}");
                }
            }

            // Si llegamos aquí, algo falló, volver a mostrar el formulario
            return View(model);
        }

        /// <summary>
        /// Muestra la vista de acceso denegado
        /// </summary>
        /// <returns>Vista de acceso denegado</returns>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Redirecciona a la URL local o a la página principal
        /// </summary>
        /// <param name="returnUrl">URL a la que redirigir</param>
        /// <returns>Redirección</returns>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}