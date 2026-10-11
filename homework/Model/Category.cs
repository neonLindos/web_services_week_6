
namespace homework.Models;

using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }


    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;


    
}