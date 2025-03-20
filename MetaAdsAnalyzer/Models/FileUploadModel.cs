using System;
using System.ComponentModel.DataAnnotations;

namespace MetaAdsAnalyzer.Models
{
    /// <summary>
    /// Modelo que representa un registro de carga de archivo
    /// </summary>
    public class FileUploadModel
    {
        /// <summary>
        /// Identificador único del registro de carga
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del archivo cargado
        /// </summary>
        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        /// <summary>
        /// ID del usuario que cargó el archivo
        /// </summary>
        [Required]
        public int UploadedBy { get; set; }

        /// <summary>
        /// Fecha y hora en que se cargó el archivo
        /// </summary>
        public DateTime UploadedAt { get; set; }

        /// <summary>
        /// Número de registros procesados del archivo
        /// </summary>
        public int? RecordsProcessed { get; set; }

        /// <summary>
        /// Estado del procesamiento del archivo
        /// </summary>
        [StringLength(50)]
        public string Status { get; set; }
    }
}