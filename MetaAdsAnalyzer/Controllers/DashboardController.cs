using MetaAdsAnalyzer.Models;
using MetaAdsAnalyzer.Models.ViewModels;
using MetaAdsAnalyzer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MetaAdsAnalyzer.Controllers
{
    /// <summary>
    /// Controlador para el dashboard y la carga de datos
    /// </summary>
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly CsvImportService _csvImportService;
        private readonly ILogger<DashboardController> _logger;

        /// <summary>
        /// Constructor que recibe los servicios necesarios
        /// </summary>
        /// <param name="dashboardService">Servicio del dashboard</param>
        /// <param name="csvImportService">Servicio de importación de CSV</param>
        /// <param name="logger">Servicio de logging</param>
        public DashboardController(
            DashboardService dashboardService,
            CsvImportService csvImportService,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _csvImportService = csvImportService;
            _logger = logger;
        }

        /// <summary>
        /// Muestra el dashboard con los datos filtrados
        /// </summary>
        /// <param name="startDate">Fecha de inicio para filtrar</param>
        /// <param name="endDate">Fecha de fin para filtrar</param>
        /// <param name="adSetName">Nombre del conjunto de anuncios para filtrar</param>
        /// <returns>Vista del dashboard</returns>
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string adSetName)
        {
            try
            {
                _logger.LogInformation("Accediendo al dashboard");

                // Si no se especifica un rango de fechas, usar el mes actual
                if (!startDate.HasValue)
                {
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }

                if (!endDate.HasValue)
                {
                    endDate = DateTime.Now;
                }

                _logger.LogInformation($"Cargando dashboard con filtros: startDate={startDate}, endDate={endDate}, adSetName={adSetName ?? "todos"}");

                // Obtener los datos para el dashboard
                var viewModel = await _dashboardService.GetDashboardDataAsync(startDate, endDate, adSetName);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el dashboard");

                // Crear un modelo vacío para evitar errores en la vista
                var emptyModel = new DashboardViewModel
                {
                    StartDate = startDate ?? DateTime.Now.AddDays(-30),
                    EndDate = endDate ?? DateTime.Now
                };

                // Agregar mensaje de error
                TempData["ErrorMessage"] = $"Error al cargar los datos del dashboard: {ex.Message}";

                return View(emptyModel);
            }
        }

        /// <summary>
        /// Obtiene los datos para los gráficos del dashboard
        /// </summary>
        /// <param name="year">Año para filtrar</param>
        /// <returns>Datos para los gráficos en formato JSON</returns>
        [HttpGet]
        public async Task<IActionResult> GetChartData(int year = 0)
        {
            try
            {
                if (year == 0)
                {
                    year = DateTime.Now.Year;
                }

                _logger.LogInformation($"Obteniendo datos para gráficos del año {year}");

                var monthlyData = await _dashboardService.GetMonthlyChartDataAsync(year);
                var adSetData = await _dashboardService.GetAdSetChartDataAsync();

                return Json(new { monthly = monthlyData, adSet = adSetData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos para gráficos");
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Muestra la vista para cargar un archivo CSV
        /// </summary>
        /// <returns>Vista de carga</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Analista")]
        public IActionResult Upload()
        {
            return View(new FileUploadViewModel());
        }

        /// <summary>
        /// Procesa la carga de un archivo CSV
        /// </summary>
        /// <param name="model">Datos del formulario</param>
        /// <returns>Vista con el resultado de la carga</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Analista")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(FileUploadViewModel model)
        {
            // Verificar que el archivo fue proporcionado
            if (Request.Form.Files.Count == 0 || Request.Form.Files[0].Length == 0)
            {
                model.Success = false;
                model.Message = "Por favor, seleccione un archivo CSV para cargar.";
                return View(model);
            }

            try
            {
                // Obtener el archivo del formulario
                var file = Request.Form.Files[0];

                // Log para debugging
                _logger.LogInformation($"Archivo recibido: {file.FileName}, Tamaño: {file.Length} bytes");

                // Obtener el ID del usuario actual
                if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                {
                    model.Success = false;
                    model.Message = "No se pudo identificar al usuario actual.";
                    return View(model);
                }

                // Importar los datos del archivo
                var result = await _csvImportService.ImportCsvDataAsync(file, userId);

                // Actualizar el modelo con el resultado
                model.Success = result.Success;
                model.Message = result.Message;
                model.RecordsProcessed = result.RecordsProcessed;

                // Si la carga fue exitosa, mostrar el dashboard
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                // Log del error
                _logger.LogError(ex, "Error al procesar el archivo CSV");

                // Mostrar mensaje de error
                model.Success = false;
                model.Message = $"Error al procesar el archivo: {ex.Message}";
            }

            // Si llegamos aquí, algo falló o no hubo redirección, volver a mostrar el formulario
            return View(model);
        }
    }
}