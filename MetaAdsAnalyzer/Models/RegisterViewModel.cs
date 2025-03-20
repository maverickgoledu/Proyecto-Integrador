using System.ComponentModel.DataAnnotations;

namespace MetaAdsAnalyzer.Models
{
    /// <summary>
    /// Modelo para el formulario de registro de usuarios
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Nombre de usuario único para el nuevo usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; }

        /// <summary>
        /// Correo electrónico único para el nuevo usuario
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        /// <summary>
        /// Contraseña para el nuevo usuario
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmación de la contraseña para validar
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Rol asignado al nuevo usuario (solo disponible para administradores)
        /// </summary>
        [Display(Name = "Rol")]
        public string Role { get; set; }
    }
}