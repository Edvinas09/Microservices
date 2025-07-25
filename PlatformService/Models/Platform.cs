using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models
{
    public class Platform
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required] //Runtime validation
        public required string Name { get; set; } // Required for compile-time validation
        [Required] 
        public required string Publisher { get; set; }
        [Required]
        public required string Cost { get; set; }
    }
}