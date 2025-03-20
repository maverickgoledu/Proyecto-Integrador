using MetaAdsAnalyzer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaAdsAnalyzer.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de datos de Meta ADS
    /// </summary>
    public class MetaAdsRepository : IMetaAdsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MetaAdsRepository> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        /// <param name="logger">Servicio de logging</param>
        public MetaAdsRepository(ApplicationDbContext context, ILogger<MetaAdsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de datos de Meta ADS
        /// </summary>
        /// <returns>Lista de registros</returns>
        public async Task<IEnumerable<MetaAdsDataModel>> GetAllMetaAdsDataAsync()
        {
            return await _context.MetaAdsData.ToListAsync();
        }

        /// <summary>
        /// Obtiene los registros de Meta ADS filtrados por fecha
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Lista de registros filtrados</returns>
        public async Task<IEnumerable<MetaAdsDataModel>> GetMetaAdsDataByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.MetaAdsData
                .Where(d => d.InicioInforme >= startDate && d.FinInforme <= endDate)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene los registros de Meta ADS filtrados por conjunto de anuncios
        /// </summary>
        /// <param name="adSetName">Nombre del conjunto de anuncios</param>
        /// <returns>Lista de registros filtrados</returns>
        public async Task<IEnumerable<MetaAdsDataModel>> GetMetaAdsDataByAdSetAsync(string adSetName)
        {
            return await _context.MetaAdsData
                .Where(d => d.NombreConjuntoAnuncios == adSetName)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene los registros de Meta ADS filtrados por fecha y conjunto de anuncios
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <param name="adSetName">Nombre del conjunto de anuncios</param>
        /// <returns>Lista de registros filtrados</returns>
        public async Task<IEnumerable<MetaAdsDataModel>> GetMetaAdsDataByDateRangeAndAdSetAsync(DateTime? startDate, DateTime? endDate, string adSetName)
        {
            var query = _context.MetaAdsData.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(d => d.InicioInforme >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(d => d.FinInforme <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(adSetName))
            {
                query = query.Where(d => d.NombreConjuntoAnuncios == adSetName);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Obtiene la lista de todos los nombres de conjuntos de anuncios
        /// </summary>
        /// <returns>Lista de nombres de conjuntos de anuncios</returns>
        public async Task<List<string>> GetAllAdSetNamesAsync()
        {
            return await _context.MetaAdsData
                .Select(d => d.NombreConjuntoAnuncios)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Agrega un nuevo registro de datos de Meta ADS
        /// </summary>
        /// <param name="data">Registro a agregar</param>
        /// <returns>Registro agregado con su ID generado</returns>
        public async Task<MetaAdsDataModel> AddMetaAdsDataAsync(MetaAdsDataModel data)
        {
            data.FechaCarga = DateTime.Now;

            try
            {
                _context.MetaAdsData.Add(data);
                await _context.SaveChangesAsync();
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar datos de Meta ADS");
                throw;
            }
        }

        /// <summary>
        /// Agrega múltiples registros de datos de Meta ADS
        /// </summary>
        /// <param name="dataList">Lista de registros a agregar</param>
        /// <returns>Número de registros agregados</returns>
        public async Task<int> AddMetaAdsDataRangeAsync(List<MetaAdsDataModel> dataList)
        {
            if (dataList == null || !dataList.Any())
            {
                return 0;
            }

            foreach (var data in dataList)
            {
                data.FechaCarga = DateTime.Now;
            }

            try
            {
                _context.MetaAdsData.AddRange(dataList);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al agregar {dataList.Count} registros de Meta ADS");
                throw;
            }
        }

        /// <summary>
        /// Registra una carga de archivo
        /// </summary>
        /// <param name="fileUpload">Datos de la carga del archivo</param>
        /// <returns>Registro de carga con su ID generado</returns>
        public async Task<FileUploadModel> AddFileUploadAsync(FileUploadModel fileUpload)
        {
            fileUpload.UploadedAt = DateTime.Now;

            try
            {
                _context.FileUploads.Add(fileUpload);
                await _context.SaveChangesAsync();
                return fileUpload;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar la carga del archivo");
                throw;
            }
        }

        /// <summary>
        /// Obtiene la fecha de la última carga de datos
        /// </summary>
        /// <returns>Fecha de la última carga o null</returns>
        public async Task<DateTime?> GetLastUploadDateAsync()
        {
            return await _context.FileUploads
                .OrderByDescending(f => f.UploadedAt)
                .Select(f => f.UploadedAt)
                .FirstOrDefaultAsync();
        }
    }
}