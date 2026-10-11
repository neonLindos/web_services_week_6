using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using homework.Models;

namespace homework.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{

    private static readonly List<Product> products = new();


    [HttpGet]
    public IEnumerable<Product> List()
    {
        return products;
    }

    [HttpGet("{id}")]
    public IActionResult Details(int id)
    {
        Product? product = products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
    [Authorize]
    [HttpPost]
    public IActionResult Create([FromBody] Product product)
    {
        string? userName = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        product.Id = products.Count == 0
            ? 1
            : products.Max(p => p.Id) + 1;

        products.Add(product);

        return Ok(new { product, createdBy = userName });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        Product? product = products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        products.Remove(product);

        return NoContent();
    }
}
