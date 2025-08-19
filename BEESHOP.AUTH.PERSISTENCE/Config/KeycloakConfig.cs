namespace BEESHOP.AUTH.PERSISTENCE.Config;
public class KeycloakConfig
{    
    public string Url { get; set; } = null!;
    public string AdminUsername { get; set; } = null!;
    public string AdminPassword { get; set; } = null!;
    public string AdminRealm { get; set; } = "master";
    public string ClientId { get; set; } = "admin-cli";
    public string ClientSecret { get; set; } = null!;
}
