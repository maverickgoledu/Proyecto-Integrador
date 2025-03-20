namespace MetaAdsAnalyzer.Models
{
    /// <summary>
    /// Modelo para la vista de errores
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// ID de la solicitud que generó el error
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Indica si se debe mostrar el ID de la solicitud
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}