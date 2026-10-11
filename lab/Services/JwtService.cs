using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TaskManagementApi.Services;

public class JwtService(IConfiguration config)
{
    // Токен: Issuer, Audience, срок действия, подпись HS256 и Claims Name и Role
    public string Generate(string username, string role)
    {
        string secret = config["Jwt:Key"]
            ?? throw new InvalidOperationException("Укажите Jwt:Key.");

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(ExpireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int ExpireMinutes => int.Parse(config["Jwt:ExpireMinutes"] ?? "30");
}
