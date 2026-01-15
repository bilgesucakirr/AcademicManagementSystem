using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Models;

public class AreaOfInterest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}