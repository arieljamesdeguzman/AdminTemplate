using System.ComponentModel.DataAnnotations;

namespace AdminTemplate.DTOs
{
    public class SupplierDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(150, ErrorMessage = "Supplier name cannot exceed 150 characters")]
        public string SupplierName { get; set; }

        [StringLength(50, ErrorMessage = "Contact number cannot exceed 50 characters")]
        public string ContactNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
        public string Address { get; set; }
    }
}