using System;
using System.ComponentModel.DataAnnotations;

namespace MetaAdsAnalyzer.Models.ViewModels
{
    /// <summary>
    /// Modelo para la visualización y edición de usuarios
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; }

        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        /// <summary>
        /// Nueva contraseña (opcional para edición)
        /// </summary>
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmación de la nueva contraseña
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Rol del usuario (Admin, Analista, Usuario)
        /// </summary>
        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Rol")]
        public string Role { get; set; }

        /// <summary>
        /// Fecha de creación de la cuenta
        /// </summary>
        [Display(Name = "Fecha de creación")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha del último inicio de sesión
        /// </summary>
        [Display(Name = "Último acceso")]
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        [Display(Name = "Activo")]
        public bool IsActive { get; set; }
    }
}