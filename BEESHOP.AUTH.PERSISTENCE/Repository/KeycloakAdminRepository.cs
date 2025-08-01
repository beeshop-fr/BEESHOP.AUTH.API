using BEESHOP.AUTH.APPLICATION.Dtos;
using BEESHOP.AUTH.APPLICATION.Interfaces;
using BEESHOP.AUTH.PERSISTENCE.Config;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace BEESHOP.AUTH.PERSISTENCE.Repository;

public class KeycloakAdminRepository : IKeycloakAdminRepository
{

    private readonly HttpClient _httpClient;
    private readonly KeycloakConfig _settings;

    private readonly string _realm = "beeshop";

    public KeycloakAdminRepository(HttpClient httpClient, IOptions<KeycloakConfig> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.BaseAddress = new Uri(_settings.Url);
    }

    public async Task CreateUserAsync(CreateUserDto dto)
    {
        var token = await GetAdminTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, $"/admin/realms/{_realm}/users");        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


        var userPayload = new
        {
            username = dto.Username,
            email = dto.Email,
            enabled = true,
            credentials = new[]
            {
                new { type = "password", value = dto.Password, temporary = false }
            }
        };

        request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(userPayload),
                                            System.Text.Encoding.UTF8,
                                            "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // 2. Récupération de l'ID du user créé
        var location = response.Headers.Location?.ToString();

        if (string.IsNullOrEmpty(location) || !location.Contains("/users/"))
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Impossible de récupérer l'ID de l'utilisateur. Réponse brute : {location}, Body : {error}");
        }

        var userId = location.Split("/users/").Last();

        if (string.IsNullOrWhiteSpace(userId))
            throw new Exception("Échec de la récupération de l'ID utilisateur depuis l'en-tête Location.");

        // 3. Attribution du rôle "user"
        await AssignRealmRoleAsync(userId, "user");
    }

    public async Task DeleteUserAsync(string userId)
    {
        var token = await GetAdminTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/admin/realms/{_realm}/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var parameters = new Dictionary<string, string>
        {
            {"client_id", "beeshop-auth"},
            {"client_secret", _settings.ClientSecret},
            {"grant_type", "password"},
            {"username", dto.username},
            {"password", dto.password},
            {"scope", "openid profile email" }
        };

        var response = await _httpClient.PostAsync(
            $"/realms/{_realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(parameters));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed login : {response.StatusCode} - {error}");
        }


        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var json = System.Text.Json.JsonDocument.Parse(content);

        return json.RootElement.GetProperty("access_token").GetString()!;
    }

    public async Task<UserInfosDto> GetUserInfoAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/realms/{_realm}/protocol/openid-connect/userinfo");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get userinfo: {response.StatusCode} - {err}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var json = System.Text.Json.JsonDocument.Parse(content);

        var username = json.RootElement.GetProperty("preferred_username").GetString();
        var email = json.RootElement.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;

        var roles = new List<string>();
        if (json.RootElement.TryGetProperty("realm_access", out var realmAccess) &&
            realmAccess.TryGetProperty("roles", out var rolesElement))
        {
            roles = rolesElement.EnumerateArray().Select(x => x.GetString()!).ToList();
        }

        return new UserInfosDto
        {
            Username = username!,
            Email = email,
            Roles = roles
        };
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var parameters = new Dictionary<string, string>
        {
            {"client_id", _settings.ClientId},
            {"grant_type", "password"},
            {"username", _settings.AdminUsername},
            {"password", _settings.AdminPassword}
        };

        var response = await _httpClient.PostAsync(
            "/realms/master/protocol/openid-connect/token",
            new FormUrlEncodedContent(parameters));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to retrieve admin token: {response.StatusCode} - {error}");
        }

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var json = System.Text.Json.JsonDocument.Parse(content);

        var token = json.RootElement.GetProperty("access_token").GetString()!;


        Console.WriteLine($"[DEBUG] Admin Token: {token}");

        return json.RootElement.GetProperty("access_token").GetString()!;
    }

    private async Task AssignRealmRoleAsync(string userId, string roleName)
    {
        var token = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Récupérer les infos du rôle
        var roleResponse = await _httpClient.GetAsync($"/admin/realms/{_realm}/roles/{roleName}");
        roleResponse.EnsureSuccessStatusCode();

        var roleContent = await roleResponse.Content.ReadAsStringAsync();
        var roleJson = System.Text.Json.JsonDocument.Parse(roleContent);
        var role = new[]
        {
        new
        {
            id = roleJson.RootElement.GetProperty("id").GetString(),
            name = roleJson.RootElement.GetProperty("name").GetString()
        }
    };

        // Assigner le rôle à l'utilisateur
        var assignRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/admin/realms/{_realm}/users/{userId}/role-mappings/realm");

        assignRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        assignRequest.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(role),
            System.Text.Encoding.UTF8,
            "application/json");

        var assignResponse = await _httpClient.SendAsync(assignRequest);
        assignResponse.EnsureSuccessStatusCode();
    }


}
