using BEESHOP.AUTH.APPLICATION.Dtos;
using BEESHOP.AUTH.APPLICATION.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BEESHOP.AUTH.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IKeycloakAdminRepository _keycloakAdminRepository;

    public AuthController(IKeycloakAdminRepository keycloakAdminRepository)
    {
        _keycloakAdminRepository = keycloakAdminRepository;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
    {
        await _keycloakAdminRepository.CreateUserAsync(dto);
        return Ok("User created successfully");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _keycloakAdminRepository.LoginAsync(dto);
        return Ok(new { Token = token });
    }

    [Authorize]
    [HttpGet("userinfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return Unauthorized("Missing or invalid Authorization header");

        var token = authHeader["Bearer ".Length..];
        var userInfo = await _keycloakAdminRepository.GetUserInfoAsync(token);

        return Ok(userInfo);
    }

    [HttpDelete("delete/{userId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string userId)
    {
        await _keycloakAdminRepository.DeleteUserAsync(userId);
        return Ok("User deleted successfully");
    }
}