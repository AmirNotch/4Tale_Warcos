using System.Reflection;
using AuthService.Extensions.ExternalAuth;
using IdentityServer.Database;
using IdentityServer.Models;
using IdentityServer.Services.Configurator;
using IdentityServer.Services.Steam;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

string? steamWebApiKey = Environment.GetEnvironmentVariable("STEAM_WEB_API_KEY");
if (steamWebApiKey == null) {
    throw new ApplicationException("Steam web API key not set");
}

var builder = WebApplication.CreateBuilder(args);

// Add Database module
var databaseConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(databaseConnectionString));

// Add Microsoft Identity module
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = false;
})
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddSteam("steam", options => {
            options.ApplicationKey = steamWebApiKey;
            options.SaveTokens = true;
        });

// Add IdentityServer4 module
builder.Services.AddIdentityServer()
        .AddConfigurationStore<ConfigurationDbContext>(options => options.ConfigureDbContext = b =>
                b.UseNpgsql(databaseConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
        .AddOperationalStore<PersistedGrantDbContext>(options => options.ConfigureDbContext = b =>
                b.UseNpgsql(databaseConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
        .AddAspNetIdentity<ApplicationUser>()
        .AddExtensionGrantValidator<ExternalAuthGrandTypeValidator>()
        .AddDeveloperSigningCredential(); // Development only

// Add modules
builder.Services.AddTransient<ISteamService, SteamService>();

// Add base controller mapper
builder.Services.AddControllers();

// Add api/swagger/healthCheck modules
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Configure
builder.Services.Configure<RouteOptions>(options => {
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// Configure the HTTP request pipeline.
var app = builder.Build();
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

// Static configuration init
app.UseDatabaseConfigurator(builder.Configuration);

app.UseIdentityServer();
app.UseAuthorization();
app.MapControllers();

app.Run();
