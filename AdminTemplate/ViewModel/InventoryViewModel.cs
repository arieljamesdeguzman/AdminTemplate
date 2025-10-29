using AdminTemplate.Models;
using System.Collections.Generic;

namespace AdminTemplate.ViewModels
{
    public class InventoryViewModel
    {
        public Inventory Inventory { get; set; }
        public IEnumerable<InventoryCategory> Categories { get; set; }
        public IEnumerable<Supplier> Suppliers { get; set; }
    }
}