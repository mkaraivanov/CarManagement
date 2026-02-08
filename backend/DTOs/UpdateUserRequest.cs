using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class UpdateUserRequest
{
    [MaxLength(50)]
    public string? Username { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }
}
