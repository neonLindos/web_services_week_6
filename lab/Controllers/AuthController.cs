using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.DTO;
using TaskManagementApi.Services;

namespace TaskManagementApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(JwtService jwt) : ControllerBase
{
    // Учебный пользователь из методички; в реальном проекте — база и хеш пароля
    private const string DemoUsername = "student";
    private const string DemoPassword = "12345";
    private const string DemoRole = "User";

    // POST /api/auth/login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Username != DemoUsername || request.Password != DemoPassword)
            return Unauthorized(new { message = "Неверный логин или пароль" });

        return Ok(new
        {
            accessToken = jwt.Generate(DemoUsername, DemoRole),
            tokenType = "Bearer",
            expiresInSeconds = jwt.ExpireMinutes * 60
        });
    }

    // GET /api/auth/me — показать Claims из токена (для отчёта)
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var claims = HttpContext.User.Claims
            .Select(c => new { c.Type, c.Value });
        return Ok(new { name = User.FindFirstValue(ClaimTypes.Name), claims });
    }
}
