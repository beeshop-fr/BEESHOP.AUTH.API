namespace BEESHOP.AUTH.APPLICATION.Dtos;

public class CreateUserDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}