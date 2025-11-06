using System;
using System.ComponentModel.DataAnnotations;
namespace AdminTemplate.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(150)]
        public string FullName { get; set; }
        [Required, MaxLength(150)]
        public string Email { get; set; }
        [Required, MaxLength(15)]
        public string PhoneNumber { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [MaxLength(255)]
        public string AuthToken { get; set; }
        public bool IsEmailVerified { get; set; } = false;

        // Add these properties
        [MaxLength(500)]
        public string Address { get; set; }
        [MaxLength(50)]
        public string Coordinates { get; set; } // Format: "latitude,longitude"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}