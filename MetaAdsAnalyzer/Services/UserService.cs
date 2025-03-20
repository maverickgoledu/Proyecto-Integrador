using MetaAdsAnalyzer.Data.Repositories;
using MetaAdsAnalyzer.Models;
using MetaAdsAnalyzer.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace MetaAdsAnalyzer.Services
{
    /// <summary>
    /// Servicio para la gestión de usuarios
    /// </summary>
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// Constructor que recibe el repositorio de usuarios
        /// </summary>
        /// <param name="userRepository">Repositorio de usuarios</param>
        /// <param name="logger">Servicio de logging</param>
        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario encontrado o null</returns>
        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        /// <summary>
        /// Autentifica a un usuario por su nombre de usuario y contraseña
        /// </summary>
        /// <param name="username">Nombre de usuario o correo electrónico</param>
        /// <param name="password">Contraseña</param>
        /// <returns>Usuario autentificado o null</returns>
        public async Task<UserModel> AuthenticateAsync(string username, string password)
        {
            try
            {
                _logger.LogInformation($"Intentando autenticar usuario: {username}");

                // Verificar si el usuario existe
                var user = await _userRepository.GetUserByUsernameAsync(username);

                // Si no se encuentra por nombre de usuario, buscar por correo electrónico
                if (user == null)
                {
                    _logger.LogInformation($"Usuario no encontrado por nombre, buscando por email: {username}");
                    user = await _userRepository.GetUserByEmailAsync(username);
                }

                // Si no se encontró usuario o está inactivo, autentificación fallida
                if (user == null)
                {
                    _logger.LogWarning($"Usuario no encontrado: {username}");
                    return null;
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning($"Usuario inactivo: {username}");
                    return null;
                }

                // Preparar contraseña para verificar
                string hashedPassword = user.PasswordHash;

                _logger.LogDebug($"Verificando contraseña para usuario: {username}");
                _logger.LogDebug($"Hash almacenado: {hashedPassword.Substring(0, 10)}...");

                // Verificar la contraseña
                bool passwordMatches = false;
                try
                {
                    // Intentar verificar con BCrypt
                    passwordMatches = BC.Verify(password, hashedPassword);
                    _logger.LogDebug($"Resultado de BCrypt.Verify: {passwordMatches}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al verificar contraseña: {ex.Message}");
                    return null;
                }

                if (!passwordMatches)
                {
                    _logger.LogWarning($"Contraseña incorrecta para usuario: {username}");
                    return null;
                }

                // Actualizar la fecha del último inicio de sesión
                await _userRepository.UpdateLastLoginAsync(user.Id);
                _logger.LogInformation($"Usuario autenticado correctamente: {username}, ID: {user.Id}");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en el proceso de autenticación: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        /// <param name="model">Datos del nuevo usuario</param>
        /// <returns>Usuario creado o null si ocurrió un error</returns>
        public async Task<UserModel> RegisterUserAsync(RegisterViewModel model)
        {
            // Verificar si el nombre de usuario ya existe
            if (await _userRepository.GetUserByUsernameAsync(model.Username) != null)
            {
                throw new InvalidOperationException("El nombre de usuario ya está en uso.");
            }

            // Verificar si el correo electrónico ya existe
            if (await _userRepository.GetUserByEmailAsync(model.Email) != null)
            {
                throw new InvalidOperationException("El correo electrónico ya está en uso.");
            }

            // Crear el usuario
            var user = new UserModel
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = BC.HashPassword(model.Password),
                Role = model.Role,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            return await _userRepository.AddUserAsync(user);
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="model">Datos actualizados del usuario</param>
        /// <returns>true si la actualización fue exitosa</returns>
        public async Task<bool> UpdateUserAsync(UserViewModel model)
        {
            var user = await _userRepository.GetUserByIdAsync(model.Id);
            if (user == null)
            {
                return false;
            }

            // Actualizar propiedades
            user.Username = model.Username;
            user.Email = model.Email;
            user.Role = model.Role;
            user.IsActive = model.IsActive;

            // Actualizar la contraseña si se proporciona una nueva
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.PasswordHash = BC.HashPassword(model.Password);
            }

            return await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Elimina un usuario por su ID
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <returns>true si la eliminación fue exitosa</returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        /// <summary>
        /// Cambia el estado de activación de un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="isActive">Nuevo estado de activación</param>
        /// <returns>true si la actualización fue exitosa</returns>
        public async Task<bool> ChangeUserStatusAsync(int id, bool isActive)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            user.IsActive = isActive;
            return await _userRepository.UpdateUserAsync(user);
        }
    }
}