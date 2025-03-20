using MetaAdsAnalyzer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetaAdsAnalyzer.Data.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de usuarios
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        Task<IEnumerable<UserModel>> GetAllUsersAsync();

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<UserModel> GetUserByIdAsync(int id);

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<UserModel> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Obtiene un usuario por su correo electrónico
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<UserModel> GetUserByEmailAsync(string email);

        /// <summary>
        /// Agrega un nuevo usuario
        /// </summary>
        /// <param name="user">Usuario a agregar</param>
        /// <returns>Usuario agregado con su ID generado</returns>
        Task<UserModel> AddUserAsync(UserModel user);

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="user">Usuario con los datos actualizados</param>
        /// <returns>true si la actualización fue exitosa</returns>
        Task<bool> UpdateUserAsync(UserModel user);

        /// <summary>
        /// Elimina un usuario por su ID
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <returns>true si la eliminación fue exitosa</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Actualiza la fecha del último inicio de sesión de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>true si la actualización fue exitosa</returns>
        Task<bool> UpdateLastLoginAsync(int userId);
    }
}