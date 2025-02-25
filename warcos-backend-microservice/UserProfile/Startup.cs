using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using UserProfile.Models;
using UserProfile.Repository;
using UserProfile.Repository.Interface;
using UserProfile.Service;
using UserProfile.Util;
using UserProfile.Validation;

namespace UserProfile;

public class Startup
{
    private readonly static string DateTimeOffsetFormatString = "yyyy-MM-ddTHH:mm:ss.fffK";

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Настройка подключения к базе данных
        services.AddDbContext<WarcosUserProfileDbContext>();
        
        // Регистрация репозиториев
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<ILevelRepository, LevelRepository>();
        
        // Регистрация сервисов
        services.AddScoped<UserProfileService>();
        services.AddScoped<AchievementService>();
        services.AddScoped<ItemService>();
        services.AddScoped<LevelService>();
        services.AddScoped<IValidationStorage, ValidationStorage>();
        
        // Регистрация AutoMapper
        services.AddAutoMapper(typeof(Startup));
        
        services.AddControllers();
        
        //services.AddFluentValidationAutoValidation()
        //    .AddFluentValidationClientsideAdapters()
        //    .AddValidatorsFromAssemblyContaining<Program>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Warcos-UserProfile API ",
                Version = "v1",
                Description = "API for the Warcos-UserProfile game "
            });
        });

        // Настройка аутентификации для Production
        if (_environment.IsProduction())
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Authority = "https://account.4taleproduction.com";
                    options.Audience = "warcos";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidTypes = new[] { "at+jwt" },
                        ValidateIssuer = true,
                        ValidIssuer = "https://account.4taleproduction.com",
                        ValidateAudience = false,
                        ValidateLifetime = true,
                    };
                });
        }
        JsonSerializerSettings jsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new OrderedContractResolver(),
            DateFormatString = DateTimeOffsetFormatString,
        };
        services.AddScoped(_ => jsonSerializerSettings);
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warcos-UserProfile API V1");
                c.RoutePrefix = string.Empty; // Это позволит открывать Swagger UI по адресу root
            });
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseWebSockets();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/api/public/ping", () => "O Captain! My Captain!");
            if (env.IsProduction())
            {
                endpoints.MapControllers();
            }
            else
            {
                endpoints.MapControllers().AllowAnonymous();
            }
        });
    }
}