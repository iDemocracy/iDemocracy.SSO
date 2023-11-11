using Duende.IdentityServer.EntityFramework.Mappers;
using iDemocracy.SSO.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace iDemocracy.SSO;

internal static class HostingExtensions
{
    public static void InitializeDatabase(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients) context.Clients.Add(client.ToEntity());
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources) context.IdentityResources.Add(resource.ToEntity());
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes) context.ApiScopes.Add(resource.ToEntity());
                context.SaveChanges();
            }
        }
    }
}