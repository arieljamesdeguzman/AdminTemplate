using AdminTemplate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminTemplate.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string ItemName { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public InventoryCategory Category { get; set; }

        [MaxLength(20)]
        public string Unit { get; set; }

        public decimal CurrentQuantity { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal? SellingPrice { get; set; }

        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string Status { get; set; } = "active";

        // Navigation
        public ICollection<InventoryTransaction> Transactions { get; set; }
    }
}
