using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureProductsApi.Filters;
using SecureProductsApi.Models;

namespace SecureProductsApi.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase
{
    private static readonly List<Product> products = new()
    {
        new Product { Id = 1, Name = "Ноутбук", Price = 350000m },
        new Product { Id = 2, Name = "Мышь", Price = 8000m },
        new Product { Id = 3, Name = "Клавиатура", Price = 15000m }
    };

    // GET /api/product — задания 1–2: нужен заголовок X-Api-Key
    [ApiKey]
    [HttpGet]
    public IEnumerable<Product> GetAll() => products;

    // GET /api/product/secure — задания 5–6: нужен JWT в Authorization: Bearer
    [Authorize]
    [HttpGet("secure")]
    public IEnumerable<Product> GetSecure() => products;
}
