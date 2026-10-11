using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SecureProductsApi.Filters;

// Задание 2: ключ из заголовка X-Api-Key сравнивается с Api Key из конфигурации
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAuthorizationFilter
{
    public const string HeaderName = "X-Api-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string? expected = context.HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["ApiKey"];

        bool valid = expected is not null
            && context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var provided)
            && CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(provided.ToString()),
                Encoding.UTF8.GetBytes(expected));

        if (!valid)
            context.Result = new UnauthorizedObjectResult(new { message = "Неверный или отсутствующий X-Api-Key" });
    }
}
