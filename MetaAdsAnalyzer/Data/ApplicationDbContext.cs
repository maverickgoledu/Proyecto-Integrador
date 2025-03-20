using MetaAdsAnalyzer.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MetaAdsAnalyzer.Data
{
    /// <summary>
    /// Contexto de base de datos para Entity Framework Core
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor que recibe las opciones de configuración
        /// </summary>
        /// <param name="options">Opciones de configuración del contexto</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // Asegurar que la base de datos existe
            Database.EnsureCreated();
        }

        /// <summary>
        /// DbSet para la tabla de Usuarios
        /// </summary>
        public DbSet<UserModel> Users { get; set; }

        /// <summary>
        /// DbSet para la tabla de Datos de Meta ADS
        /// </summary>
        public DbSet<MetaAdsDataModel> MetaAdsData { get; set; }

        /// <summary>
        /// DbSet para la tabla de registros de carga de archivos
        /// </summary>
        public DbSet<FileUploadModel> FileUploads { get; set; }

        /// <summary>
        /// Configurar la conexión a la base de datos
        /// </summary>
        /// <param name="optionsBuilder">Constructor de opciones</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Usar solo como fallback si no se configuró en Program.cs
                optionsBuilder.UseSqlServer("Server=localhost;Database=analisis;User Id=sa;Password=Alfa1232*;TrustServerCertificate=True;");
            }
        }

        /// <summary>
        /// Configuración de las entidades y sus relaciones
        /// </summary>
        /// <param name="modelBuilder">Objeto para construir el modelo de datos</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad User
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Role).HasDefaultValue("Usuario");
            });

            // Configuración de la entidad MetaAdsData
            modelBuilder.Entity<MetaAdsDataModel>(entity =>
            {
                entity.ToTable("MetaAdsData");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaCarga).HasDefaultValueSql("GETDATE()");
                entity.HasOne<UserModel>().WithMany().HasForeignKey("CargadoPor");
            });

            // Configuración de la entidad FileUpload
            modelBuilder.Entity<FileUploadModel>(entity =>
            {
                entity.ToTable("FileUploads");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("GETDATE()");
                entity.HasOne<UserModel>().WithMany().HasForeignKey(e => e.UploadedBy);
            });

            // Datos semilla para el usuario administrador
            modelBuilder.Entity<UserModel>().HasData(new UserModel
            {
                Id = 1,
                Username = "admin",
                Email = "admin@metaadsanalyzer.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
                Role = "Admin",
                CreatedAt = DateTime.Now,
                IsActive = true
            });

            // Datos semilla para el usuario analista
            modelBuilder.Entity<UserModel>().HasData(new UserModel
            {
                Id = 2,
                Username = "analista",
                Email = "analista@metaadsanalyzer.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Analista123*"),
                Role = "Analista",
                CreatedAt = DateTime.Now,
                IsActive = true
            });

            // Datos semilla para el usuario regular
            modelBuilder.Entity<UserModel>().HasData(new UserModel
            {
                Id = 3,
                Username = "usuario",
                Email = "usuario@metaadsanalyzer.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Usuario123*"),
                Role = "Usuario",
                CreatedAt = DateTime.Now,
                IsActive = true
            });
        }
    }
}