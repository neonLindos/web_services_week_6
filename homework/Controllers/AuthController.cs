
using Microsoft.AspNetCore.Mvc;
using homework.Models;
using homework.DTO;
using homework.Services;

namespace homework.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    public static readonly List<User> users = new();

    private readonly JwtService jwt;

    public AuthController(JwtService jwt)
    {
        this.jwt = jwt;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        User? user = users.FirstOrDefault(
            u => u.Email.Equals(
                request.Email,
                StringComparison.OrdinalIgnoreCase
            )
        );

        if (user == null)
            return Unauthorized("Invalid email or password");

        bool validPassword = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash
        );

        if (!validPassword)
            return Unauthorized("Invalid email or password");

        string accessToken = jwt.Generate(user);

        return Ok(new
        {
            message = "Login successful",
            accessToken = accessToken,
            tokenType = "Bearer",
            userId = user.Id,
            email = user.Email
        });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (users.Any(u => u.Email.Equals(
            request.Email,
            StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict(new
            {
                error = "USER_ALREADY_EXISTS",
                message = "User with this email already exists"
            });
        }

        if (request.Password != request.PasswordConfirm)
        {
            return BadRequest(new
            {
                error = "PASSWORD_MISMATCH",
                message = "Passwords do not match"
            });
        }

        User user = new User
        {
            Id = users.Count == 0
                ? 1
                : users.Max(u => u.Id) + 1,

            Email = request.Email,

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                request.Password
            )
        };

        users.Add(user);

        return Ok(new
        {
            message = "Registration successful",
            userId = user.Id,
            email = user.Email
        });
    }
}
