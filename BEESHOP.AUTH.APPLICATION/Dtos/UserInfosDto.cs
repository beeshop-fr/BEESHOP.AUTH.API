namespace BEESHOP.AUTH.APPLICATION.Dtos;

public class UserInfosDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } = null!;
    public List<string> Roles { get; set; }
}
