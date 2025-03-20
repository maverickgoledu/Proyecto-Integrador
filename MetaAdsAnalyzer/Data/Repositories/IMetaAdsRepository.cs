using MetaAdsAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetaAdsAnalyzer.Data.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de datos de Meta ADS
    /// </summary>
    public interface IMetaAdsRepository
    {
        /// <summary>
        /// Obtiene todos los registros de datos de Meta ADS
        /// </summary>
        /// <returns>Lista de registros</returns>
        Task<IEnumerable<MetaAdsDataModel>> GetAllMetaAdsDataAsync();

        /// <summary>
        /// Obtiene los registros de Meta ADS filtrados por fecha
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Lista de registros filtrados</returns>
        Task<IEnumerable<MetaAdsDataModel>> GetMetaAdsDataByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene los registros de Meta ADS filtrados por conjunto de anuncios
        /// </summary>
        /// <param name="adSetName">Nombre del conjunto de anuncios</param>
        /// <returns>Lista de registros filtrados</returns>
        Task<IEnumerable<MetaAdsDataModel>> GetMetaAdsDataByAdSetAsync(string adSetName);

        /// <summary>
        /// Obtiene los registros de Meta ADS filtrados por fecha y conjunto de anuncios
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <param name="adSetName">Nombre del conjunto de anuncios</param>
        /// <returns>Lista de registros filtrados</returns>
        Task<IEnumerable<MetaAdsDataModel>> GetMetaAdsDataByDateRangeAndAdSetAsync(DateTime? startDate, DateTime? endDate, string adSetName);

        /// <summary>
        /// Obtiene la lista de todos los nombres de conjuntos de anuncios
        /// </summary>
        /// <returns>Lista de nombres de conjuntos de anuncios</returns>
        Task<List<string>> GetAllAdSetNamesAsync();

        /// <summary>
        /// Agrega un nuevo registro de datos de Meta ADS
        /// </summary>
        /// <param name="data">Registro a agregar</param>
        /// <returns>Registro agregado con su ID generado</returns>
        Task<MetaAdsDataModel> AddMetaAdsDataAsync(MetaAdsDataModel data);

        /// <summary>
        /// Agrega múltiples registros de datos de Meta ADS
        /// </summary>
        /// <param name="dataList">Lista de registros a agregar</param>
        /// <returns>Número de registros agregados</returns>
        Task<int> AddMetaAdsDataRangeAsync(List<MetaAdsDataModel> dataList);

        /// <summary>
        /// Registra una carga de archivo
        /// </summary>
        /// <param name="fileUpload">Datos de la carga del archivo</param>
        /// <returns>Registro de carga con su ID generado</returns>
        Task<FileUploadModel> AddFileUploadAsync(FileUploadModel fileUpload);

        /// <summary>
        /// Obtiene la fecha de la última carga de datos
        /// </summary>
        /// <returns>Fecha de la última carga o null</returns>
        Task<DateTime?> GetLastUploadDateAsync();
    }
}