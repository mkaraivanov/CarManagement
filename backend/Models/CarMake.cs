using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class CarMake
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    // Navigation property
    public virtual ICollection<CarModel> Models { get; set; } = new List<CarModel>();
}
