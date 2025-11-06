using System.ComponentModel.DataAnnotations;
namespace AdminTemplate.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AuthToken { get; set; }

        // Add these properties
        public string Address { get; set; }
        public string Coordinates { get; set; } // Format: "latitude,longitude"
    }
}