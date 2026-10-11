using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SecureProductsApi.Services;

public class JwtService(IConfiguration config)
{
    // Задание 4: токен с Issuer, Audience, сроком действия и подписью HS256
    public string Generate(string username)
    {
        string secret = config["Jwt:Key"]
            ?? throw new InvalidOperationException("Укажите Jwt:Key.");

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: new[] { new Claim(ClaimTypes.Name, username) },
            expires: DateTime.UtcNow.AddMinutes(ExpireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int ExpireMinutes => int.Parse(config["Jwt:ExpireMinutes"] ?? "30");
}
