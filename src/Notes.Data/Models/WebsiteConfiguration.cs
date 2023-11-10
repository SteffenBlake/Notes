using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Notes.Data.Models;

[Keyless]
public class WebsiteConfiguration
{
    [Required]
    public string WebsiteName { get; set; } = null!;

    [Required]
    public string WebsiteDescription { get; set; } = null!;
}
