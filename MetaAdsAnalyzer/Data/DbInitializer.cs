using MetaAdsAnalyzer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using BC = BCrypt.Net.BCrypt;

namespace MetaAdsAnalyzer.Data
{
    /// <summary>
    /// Clase para inicializar la base de datos con datos semilla
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Inicializa la base de datos con datos semilla si es necesario
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public static void Initialize(ApplicationDbContext context)
        {
            // Asegurar que la base de datos existe
            context.Database.EnsureCreated();

            // Verificar si ya existen usuarios
            if (context.Users.Any())
            {
                return; // La base de datos ya ha sido inicializada
            }

            try
            {
                // Crear usuario administrador por defecto
                var admin = new UserModel
                {
                    Username = "admin",
                    Email = "admin@metaadsanalyzer.com",
                    PasswordHash = BC.HashPassword("Admin123*"),
                    Role = "Admin",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                // Crear usuario analista por defecto
                var analista = new UserModel
                {
                    Username = "analista",
                    Email = "analista@metaadsanalyzer.com",
                    PasswordHash = BC.HashPassword("Analista123*"),
                    Role = "Analista",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                // Crear usuario normal por defecto
                var usuario = new UserModel
                {
                    Username = "usuario",
                    Email = "usuario@metaadsanalyzer.com",
                    PasswordHash = BC.HashPassword("Usuario123*"),
                    Role = "Usuario",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                // Agregar usuarios a la base de datos
                context.Users.AddRange(admin, analista, usuario);
                context.SaveChanges();

                Console.WriteLine("Usuarios predeterminados creados correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea la base de datos mediante SQL directo si es necesario
        /// </summary>
        /// <param name="connectionString">Cadena de conexión SQL Server</param>
        public static void EnsureDatabaseCreatedSQL(string connectionString)
        {
            try
            {
                // Obtener el nombre de la base de datos de la cadena de conexión
                string databaseName = connectionString.Split(';')
                    .FirstOrDefault(part => part.Trim().StartsWith("Database=", StringComparison.OrdinalIgnoreCase))?
                    .Substring("Database=".Length).Trim();

                if (string.IsNullOrEmpty(databaseName))
                {
                    throw new ArgumentException("No se pudo extraer el nombre de la base de datos de la cadena de conexión.");
                }

                // Modificar la cadena de conexión para conectar a master
                string masterConnectionString = connectionString.Replace($"Database={databaseName}", "Database=master");

                // Crear la base de datos si no existe
                using (var connection = new Microsoft.Data.SqlClient.SqlConnection(masterConnectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $@"
                            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
                            BEGIN
                                CREATE DATABASE [{databaseName}];
                            END";

                        command.ExecuteNonQuery();
                    }
                }

                // Ahora conectar a la base de datos creada y crear las tablas
                using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
                            BEGIN
                                CREATE TABLE [dbo].[Users] (
                                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                                    [Username] NVARCHAR(50) NOT NULL,
                                    [PasswordHash] NVARCHAR(255) NOT NULL,
                                    [Email] NVARCHAR(100) NOT NULL,
                                    [Role] NVARCHAR(20) NOT NULL DEFAULT 'Usuario',
                                    [CreatedAt] DATETIME DEFAULT GETDATE(),
                                    [LastLogin] DATETIME NULL,
                                    [IsActive] BIT DEFAULT 1,
                                    CONSTRAINT UC_Username UNIQUE (Username),
                                    CONSTRAINT UC_Email UNIQUE (Email)
                                );
                            END;

                            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MetaAdsData]') AND type in (N'U'))
                            BEGIN
                                CREATE TABLE [dbo].[MetaAdsData] (
                                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                                    [InicioInforme] DATE NOT NULL,
                                    [FinInforme] DATE NOT NULL,
                                    [NombreConjuntoAnuncios] NVARCHAR(255) NOT NULL,
                                    [EntregaConjuntoAnuncios] NVARCHAR(50) NULL,
                                    [Puja] INT NULL,
                                    [TipoPuja] NVARCHAR(50) NULL,
                                    [PresupuestoConjuntoAnuncios] DECIMAL(18, 2) NULL,
                                    [TipoPresupuestoConjuntoAnuncios] NVARCHAR(50) NULL,
                                    [UltimoCambioSignificativo] DATETIME NULL,
                                    [ConfiguracionAtribucion] NVARCHAR(255) NULL,
                                    [Resultados] INT NULL,
                                    [IndicadorResultado] NVARCHAR(255) NULL,
                                    [Alcance] INT NULL,
                                    [Impresiones] INT NULL,
                                    [CostoPorResultados] DECIMAL(18, 6) NULL,
                                    [ImporteGastadoUSD] DECIMAL(18, 2) NULL,
                                    [Finalizacion] NVARCHAR(50) NULL,
                                    [Inicio] DATE NULL,
                                    [FechaCarga] DATETIME DEFAULT GETDATE(),
                                    [CargadoPor] INT NULL,
                                    FOREIGN KEY (CargadoPor) REFERENCES Users(Id)
                                );
                            END;

                            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FileUploads]') AND type in (N'U'))
                            BEGIN
                                CREATE TABLE [dbo].[FileUploads] (
                                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                                    [FileName] NVARCHAR(255) NOT NULL,
                                    [UploadedBy] INT NOT NULL,
                                    [UploadedAt] DATETIME DEFAULT GETDATE(),
                                    [RecordsProcessed] INT NULL,
                                    [Status] NVARCHAR(50) NULL,
                                    FOREIGN KEY (UploadedBy) REFERENCES Users(Id)
                                );
                            END;";

                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine($"Base de datos {databaseName} y tablas creadas correctamente mediante SQL directo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la base de datos mediante SQL directo: {ex.Message}");
            }
        }
    }
}