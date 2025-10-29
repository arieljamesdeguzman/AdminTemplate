using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminTemplate.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string SupplierName { get; set; }

        [MaxLength(50)]
        public string ContactNumber { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        // Navigation
        public ICollection<Inventory> Inventories { get; set; }
    }
}