using System.ComponentModel.DataAnnotations;

namespace Notes.Data.Models
{
    public class Project
    {

        [Key]
        public string ProjectId { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;
    }
}
