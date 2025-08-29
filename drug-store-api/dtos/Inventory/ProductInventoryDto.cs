using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.Inventory
{
    public class ProductInventoryDto
    {
        public Guid InventoryId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? BatchId { get; set; }
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public int MaxStockLevel { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? WholesaleCost { get; set; }
        public string? Location { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? LastStockTakeDate { get; set; }
        public int? LastStockTakeQuantity { get; set; }

        // Computed properties
        public bool IsLowStock { get; set; }
        public bool IsExpired { get; set; }
        public bool IsExpiringSoon { get; set; }
        public decimal? TotalValue { get; set; }
        public int? DaysUntilExpiry { get; set; }

        public string StockStatus => IsExpired ? "Expired" :
                                   IsExpiringSoon ? "Expiring Soon" :
                                   IsLowStock ? "Low Stock" :
                                   "Normal";
    }
}
