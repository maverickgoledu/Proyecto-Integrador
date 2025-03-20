using MetaAdsAnalyzer.Models;
using MetaAdsAnalyzer.Models.ViewModels;
using MetaAdsAnalyzer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MetaAdsAnalyzer.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios (CRUD)
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserService _userService;

        /// <summary>
        /// Constructor que recibe el servicio de usuarios
        /// </summary>
        /// <param name="userService">Servicio de usuarios</param>
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Muestra la lista de usuarios
        /// </summary>
        /// <returns>Vista con la lista de usuarios</returns>
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        /// <summary>
        /// Muestra los detalles de un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Vista con los detalles del usuario</returns>
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        /// <summary>
        /// Muestra el formulario para crear un nuevo usuario
        /// </summary>
        /// <returns>Vista de creación</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Procesa el formulario de creación de usuario
        /// </summary>
        /// <param name="model">Datos del formulario</param>
        /// <returns>Redirección a la lista de usuarios o vista con errores</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Registrar el usuario
                    var user = await _userService.RegisterUserAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el usuario.");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Muestra el formulario para editar un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Vista de edición</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                IsActive = user.IsActive
            };

            return View(viewModel);
        }

        /// <summary>
        /// Procesa el formulario de edición de usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="model">Datos del formulario</param>
        /// <returns>Redirección a la lista de usuarios o vista con errores</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar el usuario
                    var result = await _userService.UpdateUserAsync(model);
                    if (result)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el usuario.");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Muestra la confirmación para eliminar un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Vista de confirmación</returns>
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        /// <summary>
        /// Procesa la eliminación de un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Redirección a la lista de usuarios</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Cambia el estado de activación de un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="isActive">Nuevo estado de activación</param>
        /// <returns>Redirección a la lista de usuarios</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            await _userService.ChangeUserStatusAsync(id, isActive);
            return RedirectToAction(nameof(Index));
        }
    }
}