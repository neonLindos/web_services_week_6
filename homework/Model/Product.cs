
using System.ComponentModel.DataAnnotations;

namespace homework.Models;

public class Product
{
    public int Id { get; set; }

    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "цена")]
    public decimal Price { get; set; }

    [Display(Name = "Категория")]
    public Category? Category { get; set; }
}


