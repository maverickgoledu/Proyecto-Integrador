using System.ComponentModel.DataAnnotations;

namespace MetaAdsAnalyzer.Models
{
    /// <summary>
    /// Modelo para el formulario de inicio de sesión
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Nombre de usuario o correo electrónico para iniciar sesión
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string Username { get; set; }

        /// <summary>
        /// Contraseña para iniciar sesión
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Indica si se debe recordar al usuario en este dispositivo
        /// </summary>
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// URL a la que redirigir después del inicio de sesión
        /// </summary>
        public string ReturnUrl { get; set; } = "/";
    }
}