using MetaAdsAnalyzer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetaAdsAnalyzer.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de usuarios
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario encontrado o null</returns>
        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <returns>Usuario encontrado o null</returns>
        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Obtiene un usuario por su correo electrónico
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <returns>Usuario encontrado o null</returns>
        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Agrega un nuevo usuario
        /// </summary>
        /// <param name="user">Usuario a agregar</param>
        /// <returns>Usuario agregado con su ID generado</returns>
        public async Task<UserModel> AddUserAsync(UserModel user)
        {
            user.CreatedAt = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="user">Usuario con los datos actualizados</param>
        /// <returns>true si la actualización fue exitosa</returns>
        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            _context.Entry(user).State = EntityState.Modified;
            // No queremos modificar la fecha de creación
            _context.Entry(user).Property(x => x.CreatedAt).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return false;
                }
                throw;
            }
        }

        /// <summary>
        /// Elimina un usuario por su ID
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <returns>true si la eliminación fue exitosa</returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Actualiza la fecha del último inicio de sesión de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>true si la actualización fue exitosa</returns>
        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Verifica si un usuario existe por su ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>true si el usuario existe</returns>
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}