using IdentityServer.Database;
using IdentityServer.Services.Configurator.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Services.Configurator
{
    public static class ConfiguratorService
    {
        public static IEnumerable<IdentityResource> GetIdentityResources(IConfiguration Configuration)
        {
            return Configuration.GetSection("IdentityServerConfiguration").Get<IdentityServerConfiguration>().IdentityResources;
        }

        public static IEnumerable<ApiResource> GetApis(IConfiguration Configuration)
        {
            return Configuration.GetSection("IdentityServerConfiguration").Get<IdentityServerConfiguration>().ApiResources;
        }

        public static IEnumerable<ApiScope> GetScopes(IConfiguration Configuration)
        {
            return Configuration.GetSection("IdentityServerConfiguration").Get<IdentityServerConfiguration>().ApiScopes;
        }

        public static IEnumerable<Client> GetClients(IConfiguration Configuration)
        {
            return Configuration.GetSection("IdentityServerConfiguration").Get<IdentityServerConfiguration>().Clients;
        }

        public static void UseDatabaseConfigurator(this IApplicationBuilder app, IConfiguration Configuration)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            applicationDbContext.Database.Migrate();

            var persistedGrantDbContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
            persistedGrantDbContext.Database.Migrate();

            var configDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            configDbContext.Database.Migrate();

            {
                var toRemoveEntries = GetClients(Configuration).Select(x => x.ClientId);
                var toRemoveQuery = configDbContext.Clients.Where(x => !toRemoveEntries.Contains(x.ClientId));
                configDbContext.Clients.RemoveRange(toRemoveQuery);

                foreach (var entry in GetClients(Configuration))
                {
                    var targetEntry = entry.ToEntity();

                    if (configDbContext.Clients.SingleOrDefault(x => x.ClientId == targetEntry.ClientId) is var finedEntity && finedEntity != null)
                    {
                        targetEntry.Id = finedEntity.Id;
                        configDbContext.Entry(finedEntity).CurrentValues.SetValues(targetEntry);
                    }
                    else
                    {
                        configDbContext.Clients.Add(targetEntry);
                    }
                }
            }

            {
                var toRemoveEntries = GetIdentityResources(Configuration).Select(x => x.Name);
                var toRemoveQuery = configDbContext.IdentityResources.Where(x => !toRemoveEntries.Contains(x.Name));
                configDbContext.IdentityResources.RemoveRange(toRemoveQuery);

                foreach (var entry in GetIdentityResources(Configuration))
                {
                    var targetEntry = entry.ToEntity();

                    if (configDbContext.IdentityResources.SingleOrDefault(x => x.Name == targetEntry.Name) is var finedEntity && finedEntity != null)
                    {
                        targetEntry.Id = finedEntity.Id;
                        configDbContext.Entry(finedEntity).CurrentValues.SetValues(targetEntry);
                    }
                    else
                        configDbContext.IdentityResources.Add(targetEntry);
                }
            }

            {
                var toRemoveEntries = GetApis(Configuration).Select(x => x.Name);
                var toRemoveQuery = configDbContext.ApiResources.Where(x => !toRemoveEntries.Contains(x.Name));
                configDbContext.ApiResources.RemoveRange(toRemoveQuery);

                foreach (var entry in GetApis(Configuration))
                {
                    var targetEntry = entry.ToEntity();

                    if (configDbContext.ApiResources.SingleOrDefault(x => x.Name == targetEntry.Name) is var finedEntity && finedEntity != null)
                    {
                        targetEntry.Id = finedEntity.Id;
                        configDbContext.Entry(finedEntity).CurrentValues.SetValues(targetEntry);
                    }
                    else
                        configDbContext.ApiResources.Add(targetEntry);
                }
            }

            {
                var toRemoveEntries = GetScopes(Configuration).Select(x => x.Name);
                var toRemoveQuery = configDbContext.ApiScopes.Where(x => !toRemoveEntries.Contains(x.Name));
                configDbContext.ApiScopes.RemoveRange(toRemoveQuery);

                foreach (var entry in GetScopes(Configuration))
                {
                    var targetEntry = entry.ToEntity();

                    if (configDbContext.ApiScopes.SingleOrDefault(x => x.Name == targetEntry.Name) is var finedEntity && finedEntity != null)
                    {
                        targetEntry.Id = finedEntity.Id;
                        configDbContext.Entry(finedEntity).CurrentValues.SetValues(targetEntry);
                    }
                    else
                        configDbContext.ApiScopes.Add(targetEntry);
                }
            }

            configDbContext.SaveChanges();
        }
    }
}
