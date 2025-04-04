using MetaAdsAnalyzer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MetaAdsAnalyzer.Controllers
{
    /// <summary>
    /// Controlador principal que maneja las vistas b�sicas de la aplicaci�n
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Constructor que recibe el servicio de logging
        /// </summary>
        /// <param name="logger">Servicio de logging</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Muestra la p�gina principal
        /// </summary>
        /// <returns>Vista de la p�gina principal</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Muestra la p�gina de privacidad
        /// </summary>
        /// <returns>Vista de la p�gina de privacidad</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Maneja los errores de la aplicaci�n
        /// </summary>
        /// <param name="id">ID de la solicitud con error</param>
        /// <returns>Vista de error</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string id = null)
        {
            return View(new ErrorViewModel { RequestId = id ?? Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}