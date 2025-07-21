using BEESHOP.AUTH.APPLICATION.Dtos;
using BEESHOP.AUTH.APPLICATION.Interfaces;

namespace BEESHOP.AUTH.PERSISTENCE.Repository;

public class KeycloakAdminRepository : IKeycloakAdminRepository
{

    private readonly HttpClient _httpClient;
    private readonly string _realm = "beeshop";
    private readonly string _adminToken; // À récupérer dynamiquement en vrai

    public KeycloakAdminRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _adminToken = "TON_TOKEN_ADMIN"; // TODO: récupérer dynamiquement avec le compte admin
    }

    public async Task CreateUserAsync(CreateUserDto dto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/admin/realms/{_realm}/users");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

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
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/admin/realms/{_realm}/users/{userId}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var parameters = new Dictionary<string, string>
        {
            {"client_id", "beeshop-auth-api"},
            {"grant_type", "password"},
            {"username", dto.Username},
            {"password", dto.Password}
        };

        var response = await _httpClient.PostAsync(
            $"/realms/{_realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(parameters));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var json = System.Text.Json.JsonDocument.Parse(content);

        return json.RootElement.GetProperty("access_token").GetString()!;
    }
}
