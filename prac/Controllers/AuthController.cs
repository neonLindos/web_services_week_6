using Microsoft.AspNetCore.Mvc;
using SecureProductsApi.DTO;
using SecureProductsApi.Services;

namespace SecureProductsApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(JwtService jwt) : ControllerBase
{
    // Учебный пользователь из методички
    private const string DemoUsername = "student";
    private const string DemoPassword = "12345";

    // POST /api/auth/login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Username != DemoUsername || request.Password != DemoPassword)
            return Unauthorized(new { message = "Неверный логин или пароль" });

        return Ok(new
        {
            accessToken = jwt.Generate(request.Username),
            tokenType = "Bearer",
            expiresInSeconds = jwt.ExpireMinutes * 60
        });
    }
}
