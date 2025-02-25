using Lobby.Models;
using Lobby.Repository;
using Lobby.Repository.Interface;
using Lobby.Service;
using Lobby.Service.OpenMatch;
using Lobby.Util;
using Lobby.Background;
using Lobby.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Lobby;

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
        services.AddDbContext<WarcosLobbyDbContext>();

        // Регистрация репозиториев
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPartyRepository, PartyRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameRegimeRepository, GameRegimeRepository>();

        // Register the GameRepository itself for cases where the concrete class is used directly
        services.AddScoped<GameRepository>();

        // Регистрация сервисов
        services.AddScoped<UserService>();
        services.AddScoped<GameRegimeService>();
        services.AddScoped<PartyService>();
        services.AddScoped<GameService>();
        services.AddScoped<WebSocketHandler>();
        services.AddScoped<StartGameService>();
        services.AddScoped<MatchmakingService>();
        services.AddScoped<IValidationStorage, ValidationStorage>();

        services.AddControllers();

        services.AddHostedService<GameModeFillerJob>();

        //services.AddFluentValidationAutoValidation()
        //    .AddFluentValidationClientsideAdapters()
        //    .AddValidatorsFromAssemblyContaining<Program>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Warcos API",
                Version = "v1",
                Description = "API for the Warcos game"
            });
        });

        if (_environment.IsDevelopment())
        {
            services.AddScoped<IOpenMatchTicketService, FakeOpenMatchTicketService>();
        }
        else
        {
            services.AddScoped<IOpenMatchTicketService, RealOpenMatchTicketService>();
        }

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warcos API V1");
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
