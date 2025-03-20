using System;
using System.ComponentModel.DataAnnotations;

namespace MetaAdsAnalyzer.Models
{
    /// <summary>
    /// Modelo que representa los datos importados de Meta ADS
    /// </summary>
    public class MetaAdsDataModel
    {
        /// <summary>
        /// Identificador único del registro
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fecha de inicio del informe
        /// </summary>
        [Required]
        [Display(Name = "Inicio del informe")]
        public DateTime InicioInforme { get; set; }

        /// <summary>
        /// Fecha de fin del informe
        /// </summary>
        [Required]
        [Display(Name = "Fin del informe")]
        public DateTime FinInforme { get; set; }

        /// <summary>
        /// Nombre del conjunto de anuncios
        /// </summary>
        [Required]
        [Display(Name = "Nombre del conjunto de anuncios")]
        public string NombreConjuntoAnuncios { get; set; }

        /// <summary>
        /// Estado de entrega del conjunto de anuncios
        /// </summary>
        [Display(Name = "Entrega del conjunto de anuncios")]
        public string EntregaConjuntoAnuncios { get; set; }

        /// <summary>
        /// Valor de la puja
        /// </summary>
        [Display(Name = "Puja")]
        public int? Puja { get; set; }

        /// <summary>
        /// Tipo de puja utilizado
        /// </summary>
        [Display(Name = "Tipo de puja")]
        public string TipoPuja { get; set; }

        /// <summary>
        /// Valor del presupuesto del conjunto de anuncios
        /// </summary>
        [Display(Name = "Presupuesto del conjunto de anuncios")]
        public decimal? PresupuestoConjuntoAnuncios { get; set; }

        /// <summary>
        /// Tipo de presupuesto (Diario, Vitalicio, etc.)
        /// </summary>
        [Display(Name = "Tipo de presupuesto del conjunto de anuncios")]
        public string TipoPresupuestoConjuntoAnuncios { get; set; }

        /// <summary>
        /// Fecha del último cambio significativo
        /// </summary>
        [Display(Name = "Último cambio significativo")]
        public DateTime? UltimoCambioSignificativo { get; set; }

        /// <summary>
        /// Configuración de atribución utilizada
        /// </summary>
        [Display(Name = "Configuración de atribución")]
        public string ConfiguracionAtribucion { get; set; }

        /// <summary>
        /// Número de resultados obtenidos
        /// </summary>
        [Display(Name = "Resultados")]
        public int? Resultados { get; set; }

        /// <summary>
        /// Descripción del indicador de resultado
        /// </summary>
        [Display(Name = "Indicador de resultado")]
        public string IndicadorResultado { get; set; }

        /// <summary>
        /// Alcance total del anuncio
        /// </summary>
        [Display(Name = "Alcance")]
        public int? Alcance { get; set; }

        /// <summary>
        /// Número de impresiones del anuncio
        /// </summary>
        [Display(Name = "Impresiones")]
        public int? Impresiones { get; set; }

        /// <summary>
        /// Costo por resultado obtenido
        /// </summary>
        [Display(Name = "Costo por resultados")]
        public decimal? CostoPorResultados { get; set; }

        /// <summary>
        /// Importe total gastado en USD
        /// </summary>
        [Display(Name = "Importe gastado (USD)")]
        public decimal? ImporteGastadoUSD { get; set; }

        /// <summary>
        /// Estado de finalización de la campaña
        /// </summary>
        [Display(Name = "Finalización")]
        public string Finalizacion { get; set; }

        /// <summary>
        /// Fecha de inicio de la campaña
        /// </summary>
        [Display(Name = "Inicio")]
        public DateTime? Inicio { get; set; }

        /// <summary>
        /// Fecha en que se cargaron los datos
        /// </summary>
        public DateTime FechaCarga { get; set; }

        /// <summary>
        /// ID del usuario que cargó los datos
        /// </summary>
        public int? CargadoPor { get; set; }
    }
}