using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;

namespace iDemocracy.SSO;

public static class Config
{
    private static readonly object address = new
    {
        street_address = "One Hacker Way",
        locality = "Heidelberg",
        postal_code = "69118",
        country = "Germany"
    };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new("api1", "API")
            {
                Scopes = { "api1" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new("api1", "Full access to API")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // machine-to-machine client (from quickstart 1)
            new()
            {
                ClientId = "api",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = new List<string>
                {
                    "https://localhost:7166/swagger/oauth2-redirect.html"
                },
                RequirePkce = true,
                RequireClientSecret = true,
                // scopes that client has access to
                AllowedScopes = new List<string>
                {
                    "api1"
                },
                AllowedCorsOrigins = { "https://localhost:7166" }
            },
            // interactive ASP.NET Core Web App
            new()
            {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },
                RequirePkce = true,
                RequireClientSecret = true,
                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect after login
                RedirectUris = new List<string>
                {
                    "http://localhost:4200",
                    "https://localhost:4200",
                    "https://localhost:4200/signin-oidc",
                    "http://localhost:4200/signin-oidc"
                },

                // where to redirect after logout
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                AllowOfflineAccess = true,

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "api1"
                },
                AllowedCorsOrigins = new List<string>
                {
                    "https://localhost:4200",
                    "http://localhost:4200"
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
            new TestUser
            {
                SubjectId = "1",
                Username = "alice",
                Password = "alice",

                Claims = new List<Claim>
                {
                    new(JwtClaimTypes.Name, "Alice Smith"),
                    new(JwtClaimTypes.GivenName, "Alice"),
                    new(JwtClaimTypes.FamilyName, "Smith"),
                    new(JwtClaimTypes.Email, "alice@example.com"),
                    new(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new(JwtClaimTypes.WebSite, "http://alice.com"),
                    new(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                        IdentityServerConstants.ClaimValueTypes.Json)
                }
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "bob",
                Password = "bob",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                        IdentityServerConstants.ClaimValueTypes.Json)
                }
            }
        };
}