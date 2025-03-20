using MetaAdsAnalyzer.Data.Repositories;
using MetaAdsAnalyzer.Models;
using MetaAdsAnalyzer.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaAdsAnalyzer.Services
{
    /// <summary>
    /// Servicio para la generación de datos del dashboard
    /// </summary>
    public class DashboardService
    {
        private readonly IMetaAdsRepository _metaAdsRepository;

        /// <summary>
        /// Constructor que recibe el repositorio de datos de Meta ADS
        /// </summary>
        /// <param name="metaAdsRepository">Repositorio de datos de Meta ADS</param>
        public DashboardService(IMetaAdsRepository metaAdsRepository)
        {
            _metaAdsRepository = metaAdsRepository;
        }

        /// <summary>
        /// Obtiene los datos para el dashboard filtrados por fecha y conjunto de anuncios
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <param name="adSetName">Nombre del conjunto de anuncios</param>
        /// <returns>ViewModel con los datos para el dashboard</returns>
        public async Task<DashboardViewModel> GetDashboardDataAsync(DateTime? startDate, DateTime? endDate, string adSetName)
        {
            // Obtener los datos filtrados
            var data = await _metaAdsRepository.GetMetaAdsDataByDateRangeAndAdSetAsync(startDate, endDate, adSetName);
            var adSetNames = await _metaAdsRepository.GetAllAdSetNamesAsync();
            var lastUploadDate = await _metaAdsRepository.GetLastUploadDateAsync();

            // Calcular métricas
            var viewModel = new DashboardViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SelectedAdSet = adSetName,
                AvailableAdSets = adSetNames,
                UltimaCarga = lastUploadDate
            };

            if (data.Any())
            {
                // Calcular totales
                viewModel.TotalImporteGastado = data.Sum(d => d.ImporteGastadoUSD ?? 0);
                viewModel.TotalAlcance = data.Sum(d => d.Alcance ?? 0);
                viewModel.TotalImpresiones = data.Sum(d => d.Impresiones ?? 0);
                viewModel.TotalResultados = data.Sum(d => d.Resultados ?? 0);

                // Calcular métricas derivadas
                viewModel.CostoPromedioResultado = viewModel.TotalResultados > 0
                    ? viewModel.TotalImporteGastado / viewModel.TotalResultados
                    : 0;

                viewModel.AlcanceVsImpresiones = viewModel.TotalImpresiones > 0
                    ? (decimal)viewModel.TotalAlcance / viewModel.TotalImpresiones
                    : 0;

                viewModel.TasaConversion = viewModel.TotalImpresiones > 0
                    ? (decimal)viewModel.TotalResultados / viewModel.TotalImpresiones
                    : 0;

                viewModel.CostoPorMilImpresiones = viewModel.TotalImpresiones > 0
                    ? (viewModel.TotalImporteGastado / viewModel.TotalImpresiones) * 1000
                    : 0;

                // Calcular datos para los gráficos
                viewModel.PresupuestoDiarioPorConjunto = data
                    .GroupBy(d => d.NombreConjuntoAnuncios)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(d => d.PresupuestoConjuntoAnuncios ?? 0)
                    );

                viewModel.ImporteGastadoPorConjunto = data
                    .GroupBy(d => d.NombreConjuntoAnuncios)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(d => d.ImporteGastadoUSD ?? 0)
                    );
            }

            return viewModel;
        }

        /// <summary>
        /// Obtiene los datos para los gráficos del dashboard por mes
        /// </summary>
        /// <param name="year">Año para filtrar</param>
        /// <returns>Datos para los gráficos por mes</returns>
        public async Task<Dictionary<string, List<decimal>>> GetMonthlyChartDataAsync(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            var data = await _metaAdsRepository.GetMetaAdsDataByDateRangeAsync(startDate, endDate);

            // Agrupar por mes y conjunto de anuncios
            var monthlyData = data
                .GroupBy(d => new
                {
                    Month = d.InicioInforme.Month,
                    AdSet = d.NombreConjuntoAnuncios
                })
                .Select(g => new
                {
                    g.Key.Month,
                    g.Key.AdSet,
                    Importe = g.Sum(d => d.ImporteGastadoUSD ?? 0),
                    Alcance = g.Sum(d => d.Alcance ?? 0),
                    Impresiones = g.Sum(d => d.Impresiones ?? 0),
                    Resultados = g.Sum(d => d.Resultados ?? 0)
                })
                .ToList();

            // Crear diccionario con los datos por mes
            var result = new Dictionary<string, List<decimal>>();

            // Preparar listas para cada métrica
            var months = Enumerable.Range(1, 12).Select(m => m.ToString()).ToList();
            result["ImporteGastado"] = new List<decimal>(new decimal[12]);
            result["Alcance"] = new List<decimal>(new decimal[12]);
            result["Impresiones"] = new List<decimal>(new decimal[12]);
            result["Resultados"] = new List<decimal>(new decimal[12]);

            // Llenar los datos
            foreach (var item in monthlyData)
            {
                var monthIndex = item.Month - 1; // Ajustar al índice base 0
                result["ImporteGastado"][monthIndex] += item.Importe;
                result["Alcance"][monthIndex] += item.Alcance;
                result["Impresiones"][monthIndex] += item.Impresiones;
                result["Resultados"][monthIndex] += item.Resultados;
            }

            return result;
        }

        /// <summary>
        /// Obtiene los datos para los gráficos del dashboard por conjunto de anuncios
        /// </summary>
        /// <returns>Datos para los gráficos por conjunto de anuncios</returns>
        public async Task<Dictionary<string, Dictionary<string, decimal>>> GetAdSetChartDataAsync()
        {
            var data = await _metaAdsRepository.GetAllMetaAdsDataAsync();

            // Agrupar por conjunto de anuncios
            var adSetData = data
                .GroupBy(d => d.NombreConjuntoAnuncios)
                .Select(g => new
                {
                    AdSet = g.Key,
                    Importe = g.Sum(d => d.ImporteGastadoUSD ?? 0),
                    Alcance = g.Sum(d => d.Alcance ?? 0),
                    Impresiones = g.Sum(d => d.Impresiones ?? 0),
                    Resultados = g.Sum(d => d.Resultados ?? 0),
                    CostoPorResultado = g.Sum(d => d.Resultados ?? 0) > 0
                        ? g.Sum(d => d.ImporteGastadoUSD ?? 0) / g.Sum(d => d.Resultados ?? 0)
                        : 0
                })
                .ToList();

            // Crear diccionario con los datos por conjunto de anuncios
            var result = new Dictionary<string, Dictionary<string, decimal>>();

            result["ImporteGastado"] = adSetData.ToDictionary(d => d.AdSet, d => d.Importe);
            result["Alcance"] = adSetData.ToDictionary(d => d.AdSet, d => (decimal)d.Alcance);
            result["Impresiones"] = adSetData.ToDictionary(d => d.AdSet, d => (decimal)d.Impresiones);
            result["Resultados"] = adSetData.ToDictionary(d => d.AdSet, d => (decimal)d.Resultados);
            result["CostoPorResultado"] = adSetData.ToDictionary(d => d.AdSet, d => d.CostoPorResultado);

            return result;
        }
    }
}