using System;

namespace AdminTemplate.DTOs
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public string Unit { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal? SellingPrice { get; set; }
        public string Location { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; }
    }
}