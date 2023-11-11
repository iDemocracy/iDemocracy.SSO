using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;

namespace iDemocracy.SSO;

public static class Config
{
    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>();

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new("api1", "MyAPI")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // machine-to-machine client (from quickstart 1)
            new()
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // scopes that client has access to
                AllowedScopes = { "api1" }
            },
            // interactive ASP.NET Core Web App
            new()
            {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect after login
                RedirectUris = { "https://localhost:5002/signin-oidc" },

                // where to redirect after logout
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                AllowOfflineAccess = true,

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "api1"
                }
            }
        };

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static List<TestUser> TestUsers =>
        new()
        {
            new()
            {
                SubjectId = "1",
                Username = "alice",
                Password = "password",

                Claims = new List<Claim>
                {
                    new(JwtClaimTypes.Name, "Alice Smith"),
                    new(JwtClaimTypes.GivenName, "Alice"),
                    new(JwtClaimTypes.FamilyName, "Smith"),
                    new(JwtClaimTypes.Email, "alice@example.com"),
                    new(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new(JwtClaimTypes.WebSite, "http://alice.com")
                }
            }
        };
}