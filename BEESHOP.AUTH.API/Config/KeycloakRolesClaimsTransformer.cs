using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace BEESHOP.AUTH.API.Config;

public class KeycloakRolesClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity != null)
        {
            // Trouver le claim "realm_access" émis par Keycloak
            var realmAccessClaim = identity.FindFirst("realm_access");
            if (realmAccessClaim != null && realmAccessClaim.Value != null)
            {
                try
                {
                    // Extraire la liste des roles du JSON du claim
                    var realmAccess = JsonSerializer.Deserialize<RealmAccess>(realmAccessClaim.Value);
                    foreach (var roleName in realmAccess?.roles ?? Enumerable.Empty<string>())
                    {
                        // Ajouter un claim de type Role pour chaque rôle
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                    }
                }
                catch { /* gérer erreurs de parsing éventuelles */ }
            }
        }
        return Task.FromResult(principal);
    }
    // Classe auxiliaire pour désérialiser le JSON de realm_access
    private class RealmAccess { public string[] roles { get; set; } = Array.Empty<string>(); }

}