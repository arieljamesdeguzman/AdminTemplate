using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminTemplate.Models
{
    public class InventoryCategory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation
        public ICollection<Inventory> Inventories { get; set; }
    }
}