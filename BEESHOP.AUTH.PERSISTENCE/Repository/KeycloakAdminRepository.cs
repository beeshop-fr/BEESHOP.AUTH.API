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
            {"client_id", "beeshop-auth-api"},
            {"client_secret", _settings.ClientSecret},
            {"grant_type", "password"},
            {"username", dto.Username},
            {"password", dto.Password}
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

}
