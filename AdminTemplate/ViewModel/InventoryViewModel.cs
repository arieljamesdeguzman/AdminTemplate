using AdminTemplate.DTOs;
using AdminTemplate.Models;
using System.Collections.Generic;

namespace AdminTemplate.ViewModels
{
    public class InventoryViewModel
    {
        public InventoryDto Inventory { get; set; } = new InventoryDto();
        public IEnumerable<InventoryCategory> Categories { get; set; } = new List<InventoryCategory>();
        public IEnumerable<Supplier> Suppliers { get; set; } = new List<Supplier>();
    }
}