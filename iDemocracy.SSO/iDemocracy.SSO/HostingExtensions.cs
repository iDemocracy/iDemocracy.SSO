using Duende.IdentityServer.EntityFramework.Mappers;
using iDemocracy.Models;
using iDemocracy.SSO.DataAccessLayer;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
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

            if (!context.Users.Any())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                userManager.PasswordValidators.Clear(); // Esto desactivará las validaciones de contraseña


                foreach (var testUser in Config.TestUsers)
                {
                    var existingUser = userManager.FindByNameAsync(testUser.Username).Result;

                    if (existingUser == null)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = testUser.Username,
                            Email = testUser.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email)?.Value,
                            PasswordHash = userManager.PasswordHasher.HashPassword(null, testUser.Password),
                            Name = testUser.Username,
                            Id = testUser.SubjectId
                        };

                        var result = userManager.CreateAsync(user, testUser.Password).Result;

                        if (result.Succeeded)
                            // Añade los claims al usuario si es necesario
                            foreach (var claim in testUser.Claims)
                                userManager.AddClaimAsync(user, claim).Wait();
                        else
                            // Maneja el caso de error, imprime o registra el resultado.Errors
                            Console.WriteLine($"Error creando usuario: {string.Join(", ", result.Errors)}");
                    }
                }

                context.SaveChanges();
            }
        }
    }
}