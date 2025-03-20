using MetaAdsAnalyzer.Data;
using MetaAdsAnalyzer.Data.Repositories;
using MetaAdsAnalyzer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cultura para usar dólares en lugar de euros
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Configurar nivel de log para mostrar más información
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
builder.Logging.AddFilter("MetaAdsAnalyzer", LogLevel.Debug);

// Configurar la conexión a la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging() // Permite ver los valores de los parámetros en las consultas SQL
           .LogTo(Console.WriteLine, LogLevel.Information));

// Registrar repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMetaAdsRepository, MetaAdsRepository>();

// Registrar servicios
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CsvImportService>();
builder.Services.AddScoped<DashboardService>();

// Configurar la autenticación basada en cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// Agregar MVC y configurar las vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Configurar las rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Asegurar que la base de datos existe y está inicializada
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Inicializando la base de datos...");

        var context = services.GetRequiredService<ApplicationDbContext>();

        // EnsureCreated garantiza que la base de datos existe con sus tablas
        if (context.Database.EnsureCreated())
        {
            logger.LogInformation("Base de datos creada correctamente.");
        }
        else
        {
            logger.LogInformation("La base de datos ya existía.");
        }

        // Inicializar datos si es necesario
        if (!context.Users.Any())
        {
            logger.LogInformation("Inicializando datos de usuarios...");
            DbInitializer.Initialize(context);
            logger.LogInformation("Datos de usuarios inicializados correctamente.");
        }
        else
        {
            logger.LogInformation("Ya existen usuarios en la base de datos.");
            logger.LogInformation($"Número de usuarios: {context.Users.Count()}");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

app.Run();