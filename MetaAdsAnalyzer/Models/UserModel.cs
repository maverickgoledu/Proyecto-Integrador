using System;
using System.ComponentModel.DataAnnotations;

namespace MetaAdsAnalyzer.Models
{
    /// <summary>
    /// Modelo que representa un usuario en el sistema
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de usuario (único)
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
        public string Username { get; set; }

        /// <summary>
        /// Contraseña almacenada como hash
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Correo electrónico del usuario (único)
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres")]
        public string Email { get; set; }

        /// <summary>
        /// Rol del usuario (Admin, Analista, Usuario)
        /// </summary>
        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Role { get; set; }

        /// <summary>
        /// Fecha de creación de la cuenta
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha del último inicio de sesión
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool IsActive { get; set; }
    }
}