using System;
using System.Collections.Generic;

namespace MetaAdsAnalyzer.Models.ViewModels
{
    /// <summary>
    /// Modelo para la visualización del dashboard
    /// </summary>
    public class DashboardViewModel
    {
        /// <summary>
        /// Fecha de inicio seleccionada para el filtro
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Fecha de fin seleccionada para el filtro
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Nombre del conjunto de anuncios seleccionado para el filtro
        /// </summary>
        public string SelectedAdSet { get; set; }

        /// <summary>
        /// Lista de todos los conjuntos de anuncios disponibles
        /// </summary>
        public List<string> AvailableAdSets { get; set; }

        /// <summary>
        /// Suma total del importe gastado
        /// </summary>
        public decimal TotalImporteGastado { get; set; }

        /// <summary>
        /// Suma total del alcance
        /// </summary>
        public int TotalAlcance { get; set; }

        /// <summary>
        /// Suma total de impresiones
        /// </summary>
        public int TotalImpresiones { get; set; }

        /// <summary>
        /// Suma total de resultados
        /// </summary>
        public int TotalResultados { get; set; }

        /// <summary>
        /// Costo promedio por resultado
        /// </summary>
        public decimal CostoPromedioResultado { get; set; }

        /// <summary>
        /// Relación de alcance vs impresiones
        /// </summary>
        public decimal AlcanceVsImpresiones { get; set; }

        /// <summary>
        /// Datos para el gráfico de presupuesto diario por conjunto de anuncios
        /// </summary>
        public Dictionary<string, decimal> PresupuestoDiarioPorConjunto { get; set; }

        /// <summary>
        /// Datos para el gráfico de importe gastado por conjunto de anuncios
        /// </summary>
        public Dictionary<string, decimal> ImporteGastadoPorConjunto { get; set; }

        /// <summary>
        /// Tasa de conversión (resultados / impresiones)
        /// </summary>
        public decimal TasaConversion { get; set; }

        /// <summary>
        /// Costo por mil impresiones
        /// </summary>
        public decimal CostoPorMilImpresiones { get; set; }

        /// <summary>
        /// Fecha de la última carga de datos
        /// </summary>
        public DateTime? UltimaCarga { get; set; }

        /// <summary>
        /// Constructor para inicializar las propiedades de listas y diccionarios
        /// </summary>
        public DashboardViewModel()
        {
            AvailableAdSets = new List<string>();
            PresupuestoDiarioPorConjunto = new Dictionary<string, decimal>();
            ImporteGastadoPorConjunto = new Dictionary<string, decimal>();
        }
    }
}