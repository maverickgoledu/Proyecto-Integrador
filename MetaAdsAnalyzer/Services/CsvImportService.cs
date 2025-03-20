using MetaAdsAnalyzer.Data.Repositories;
using MetaAdsAnalyzer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace MetaAdsAnalyzer.Services
{
    /// <summary>
    /// Servicio para la importación de datos CSV de Meta ADS
    /// </summary>
    public class CsvImportService
    {
        private readonly IMetaAdsRepository _metaAdsRepository;
        private readonly ILogger<CsvImportService> _logger;

        /// <summary>
        /// Constructor que recibe el repositorio de datos de Meta ADS y el logger
        /// </summary>
        /// <param name="metaAdsRepository">Repositorio de datos de Meta ADS</param>
        /// <param name="logger">Servicio de logging</param>
        public CsvImportService(IMetaAdsRepository metaAdsRepository, ILogger<CsvImportService> logger)
        {
            _metaAdsRepository = metaAdsRepository;
            _logger = logger;
        }

        /// <summary>
        /// Importa datos desde un archivo CSV
        /// </summary>
        /// <param name="file">Archivo CSV</param>
        /// <param name="userId">ID del usuario que realiza la importación</param>
        /// <returns>Resultado de la importación</returns>
        public async Task<(bool Success, string Message, int RecordsProcessed)> ImportCsvDataAsync(IFormFile file, int userId)
        {
            if (file == null)
            {
                _logger.LogWarning("Se intentó importar un archivo nulo");
                return (false, "No se ha seleccionado ningún archivo.", 0);
            }

            if (file.Length == 0)
            {
                _logger.LogWarning("Se intentó importar un archivo de tamaño cero");
                return (false, "El archivo seleccionado está vacío.", 0);
            }

            _logger.LogInformation($"Iniciando importación de archivo: {file.FileName}, tamaño: {file.Length} bytes");

            try
            {
                // Verificar que el archivo sea CSV
                if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning($"Extensión de archivo no válida: {Path.GetExtension(file.FileName)}");
                    return (false, "El archivo debe ser de tipo CSV.", 0);
                }

                // Leer los datos del archivo
                List<MetaAdsDataModel> dataList = new List<MetaAdsDataModel>();

                using (var memoryStream = new MemoryStream())
                {
                    // Copiar el archivo a memoria para procesarlo
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    // Leer una muestra del archivo para depuración
                    byte[] buffer = new byte[Math.Min(4096, (int)memoryStream.Length)];
                    await memoryStream.ReadAsync(buffer, 0, buffer.Length);
                    memoryStream.Position = 0;

                    string sampleContent = Encoding.UTF8.GetString(buffer);
                    _logger.LogInformation($"Muestra del contenido del archivo:\n{sampleContent}");

                    using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
                    {
                        // Configurar CsvHelper
                        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            Delimiter = ",",
                            HasHeaderRecord = true,
                            MissingFieldFound = null,
                            BadDataFound = null,
                            IgnoreBlankLines = true,
                            TrimOptions = TrimOptions.Trim
                        };

                        using (var csv = new CsvReader(reader, config))
                        {
                            try
                            {
                                // Registrar mapeo de columnas
                                _logger.LogInformation("Configurando mapeo de columnas CSV");
                                csv.Context.RegisterClassMap<MetaAdsDataMapClass>();

                                // Leer registros
                                _logger.LogInformation("Iniciando lectura de registros");
                                var records = csv.GetRecords<MetaAdsCsvRecord>().ToList();
                                _logger.LogInformation($"Registros leídos: {records.Count}");

                                // Convertir registros al modelo de datos
                                foreach (var record in records)
                                {
                                    try
                                    {
                                        var data = new MetaAdsDataModel
                                        {
                                            InicioInforme = DateTime.Parse(record.InicioInforme),
                                            FinInforme = DateTime.Parse(record.FinInforme),
                                            NombreConjuntoAnuncios = record.NombreConjuntoAnuncios,
                                            EntregaConjuntoAnuncios = record.EntregaConjuntoAnuncios,
                                            Puja = record.Puja,
                                            TipoPuja = record.TipoPuja,
                                            PresupuestoConjuntoAnuncios = record.PresupuestoConjuntoAnuncios,
                                            TipoPresupuestoConjuntoAnuncios = record.TipoPresupuestoConjuntoAnuncios,
                                            UltimoCambioSignificativo = record.UltimoCambioSignificativo,
                                            ConfiguracionAtribucion = record.ConfiguracionAtribucion,
                                            Resultados = record.Resultados,
                                            IndicadorResultado = record.IndicadorResultado,
                                            Alcance = record.Alcance,
                                            Impresiones = record.Impresiones,
                                            CostoPorResultados = record.CostoPorResultados,
                                            ImporteGastadoUSD = record.ImporteGastadoUSD,
                                            Finalizacion = record.Finalizacion,
                                            Inicio = record.Inicio,
                                            CargadoPor = userId
                                        };

                                        dataList.Add(data);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogWarning($"Error al convertir registro: {ex.Message}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error al leer registros del CSV");
                                throw new Exception($"Error al leer registros del CSV: {ex.Message}", ex);
                            }
                        }
                    }
                }

                if (dataList.Count == 0)
                {
                    _logger.LogWarning("No se pudieron extraer datos del archivo CSV");
                    return (false, "No se pudieron extraer datos del archivo CSV. Verifica el formato.", 0);
                }

                // Guardar los datos en la base de datos
                _logger.LogInformation($"Intentando guardar {dataList.Count} registros en la base de datos");
                int recordsProcessed = await _metaAdsRepository.AddMetaAdsDataRangeAsync(dataList);
                _logger.LogInformation($"Registros guardados exitosamente: {recordsProcessed}");

                // Registrar la carga del archivo
                var fileUpload = new FileUploadModel
                {
                    FileName = file.FileName,
                    UploadedBy = userId,
                    RecordsProcessed = recordsProcessed,
                    Status = "Completado"
                };

                await _metaAdsRepository.AddFileUploadAsync(fileUpload);
                _logger.LogInformation("Registro de carga del archivo completado");

                return (true, $"Se han importado {recordsProcessed} registros correctamente.", recordsProcessed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar el archivo CSV");

                // Registrar la carga fallida
                try
                {
                    var fileUpload = new FileUploadModel
                    {
                        FileName = file.FileName,
                        UploadedBy = userId,
                        RecordsProcessed = 0,
                        Status = $"Error: {ex.Message}"
                    };

                    await _metaAdsRepository.AddFileUploadAsync(fileUpload);
                    _logger.LogInformation("Registro de carga fallida completado");
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error al registrar la carga fallida");
                }

                return (false, $"Error al importar el archivo: {ex.Message}", 0);
            }
        }
    }

    /// <summary>
    /// Clase para mapear los campos del CSV a nuestro modelo
    /// </summary>
    public class MetaAdsDataMapClass : ClassMap<MetaAdsCsvRecord>
    {
        public MetaAdsDataMapClass()
        {
            Map(m => m.InicioInforme).Name("Inicio del informe");
            Map(m => m.FinInforme).Name("Fin del informe");
            Map(m => m.NombreConjuntoAnuncios).Name("Nombre del conjunto de anuncios");
            Map(m => m.EntregaConjuntoAnuncios).Name("Entrega del conjunto de anuncios");
            Map(m => m.Puja).Name("Puja");
            Map(m => m.TipoPuja).Name("Tipo de puja");
            Map(m => m.PresupuestoConjuntoAnuncios).Name("Presupuesto del conjunto de anuncios");
            Map(m => m.TipoPresupuestoConjuntoAnuncios).Name("Tipo de presupuesto del conjunto de anuncios");
            Map(m => m.UltimoCambioSignificativo).Name("Último cambio significativo");
            Map(m => m.ConfiguracionAtribucion).Name("Configuración de atribución");
            Map(m => m.Resultados).Name("Resultados");
            Map(m => m.IndicadorResultado).Name("Indicador de resultado");
            Map(m => m.Alcance).Name("Alcance");
            Map(m => m.Impresiones).Name("Impresiones");
            Map(m => m.CostoPorResultados).Name("Costo por resultados");
            Map(m => m.ImporteGastadoUSD).Name("Importe gastado (USD)");
            Map(m => m.Finalizacion).Name("Finalización");
            Map(m => m.Inicio).Name("Inicio");
        }
    }

    /// <summary>
    /// Clase para representar los registros del CSV
    /// </summary>
    public class MetaAdsCsvRecord
    {
        public string InicioInforme { get; set; }
        public string FinInforme { get; set; }
        public string NombreConjuntoAnuncios { get; set; }
        public string EntregaConjuntoAnuncios { get; set; }
        public int? Puja { get; set; }
        public string TipoPuja { get; set; }
        public decimal? PresupuestoConjuntoAnuncios { get; set; }
        public string TipoPresupuestoConjuntoAnuncios { get; set; }
        public DateTime? UltimoCambioSignificativo { get; set; }
        public string ConfiguracionAtribucion { get; set; }
        public int? Resultados { get; set; }
        public string IndicadorResultado { get; set; }
        public int? Alcance { get; set; }
        public int? Impresiones { get; set; }
        public decimal? CostoPorResultados { get; set; }
        public decimal? ImporteGastadoUSD { get; set; }
        public string Finalizacion { get; set; }
        public DateTime? Inicio { get; set; }
    }
}