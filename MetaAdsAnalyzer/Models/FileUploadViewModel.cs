using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MetaAdsAnalyzer.Models
{
    /// <summary>
    /// Modelo para la carga de archivos CSV de Meta ADS
    /// </summary>
    public class FileUploadViewModel
    {
        /// <summary>
        /// Archivo CSV a cargar
        /// </summary>
        [Display(Name = "Archivo CSV")]
        public IFormFile CsvFile { get; set; }

        /// <summary>
        /// Mensaje de resultado después de la carga
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Indica si la carga fue exitosa
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Número de registros procesados del archivo
        /// </summary>
        public int RecordsProcessed { get; set; }
    }
}