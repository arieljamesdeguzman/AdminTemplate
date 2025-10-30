using System;
using System.ComponentModel.DataAnnotations;

namespace AdminTemplate.DTOs
{
    public class InventoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(150, ErrorMessage = "Item name cannot exceed 150 characters")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        public int CategoryId { get; set; }

        public int? SupplierId { get; set; }

        [StringLength(20, ErrorMessage = "Unit cannot exceed 20 characters")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Current quantity is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public decimal CurrentQuantity { get; set; }

        [Required(ErrorMessage = "Reorder level is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Reorder level must be a positive number")]
        public decimal ReorderLevel { get; set; }

        [Required(ErrorMessage = "Cost per unit is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost per unit must be greater than 0")]
        public decimal CostPerUnit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Selling price must be a positive number")]
        public decimal? SellingPrice { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string Location { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20)]
        public string Status { get; set; } = "active";
    }
}