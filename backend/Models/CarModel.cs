using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class CarModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(Make))]
    public int MakeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    // Navigation property
    public virtual CarMake Make { get; set; } = null!;
}
