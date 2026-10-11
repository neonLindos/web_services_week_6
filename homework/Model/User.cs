
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace homework.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    public string AccessToken {get;set; } = string.Empty;
}
