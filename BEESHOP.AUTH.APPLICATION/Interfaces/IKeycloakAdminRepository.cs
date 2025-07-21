using BEESHOP.AUTH.APPLICATION.Dtos;

namespace BEESHOP.AUTH.APPLICATION.Interfaces;

public interface IKeycloakAdminRepository
{
    Task CreateUserAsync(CreateUserDto dto);
    Task DeleteUserAsync(string userId);
    Task<string> LoginAsync(LoginDto dto);
}
