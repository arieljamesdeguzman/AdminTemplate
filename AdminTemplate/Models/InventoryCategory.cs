using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminTemplate.Models
{
    public class InventoryCategory
    {
        public int Id { get; set; }


        public string Name { get; set; }
        public string ImageUrl { get; set; }

        // Navigation
        public ICollection<Inventory> Inventories { get; set; }
    }
}