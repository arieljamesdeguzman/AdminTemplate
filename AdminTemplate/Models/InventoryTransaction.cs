using AdminTemplate.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminTemplate.Models
{
    public class InventoryTransaction
    {
        public int Id { get; set; }

        public int InventoryId { get; set; }
        [ForeignKey("InventoryId")]
        public Inventory Inventory { get; set; }

        [Required]
        [MaxLength(20)]
        public string TransactionType { get; set; } // purchase, usage, wastage, etc.

        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string Remarks { get; set; }

        public int? UserId { get; set; } // optional, depends on your auth setup
    }
}