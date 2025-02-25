using Lobby;
using Serilog;
using Lobby.Extensions;
using Lobby.Models;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

// Общий шаблон для вывода логов
string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {RequestId} {Message:lj} {Exception}{NewLine}";

// Настройка Serilog для логирования
LoggerConfiguration loggerConfiguration;

if (!builder.Environment.IsProduction()) {
    loggerConfiguration = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.FromLogContext();
    
    // Настройка HTTP-логирования для Development
    builder.Services.AddHttpLogging(logging => {
        logging.LoggingFields = HttpLoggingFields.All;
        logging.RequestBodyLogLimit = 4096;
        logging.ResponseBodyLogLimit = 4096;
    });
} else {
    loggerConfiguration = new LoggerConfiguration()
        .MinimumLevel.Information();
}

// Конфигурация Serilog
var logger = loggerConfiguration
    .WriteTo.File(
        "logs/LobbyService-.log",
        outputTemplate: outputTemplate,
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true
    )
    .WriteTo.Console(outputTemplate: outputTemplate)
    .CreateLogger();

builder.Host.UseSerilog(logger);

// Используем Startup для настройки приложения
var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);
app.MigrateDatabase<WarcosLobbyDbContext>();

// Запуск приложения
app.Run();