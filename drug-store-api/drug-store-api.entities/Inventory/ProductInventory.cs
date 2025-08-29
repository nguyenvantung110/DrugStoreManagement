using drug_store_api.entities.Batches;
using drug_store_api.entities.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.entities.Inventory
{
    [Table("product_inventory")]
    public class ProductInventory
    {
        [Key]
        [Column("inventory_id")]
        public Guid InventoryId { get; set; } = Guid.NewGuid();

        [Required]
        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Column("batch_id")]
        public Guid? BatchId { get; set; }

        [Required]
        [Column("current_stock")]
        public int CurrentStock { get; set; } = 0;

        [Required]
        [Column("reorder_level")]
        public int ReorderLevel { get; set; } = 0;

        [Required]
        [Column("max_stock_level")]
        public int MaxStockLevel { get; set; } = 1000;

        [Column("unit_cost", TypeName = "numeric(10,2)")]
        public decimal? UnitCost { get; set; }

        [Column("wholesale_cost", TypeName = "numeric(10,2)")]
        public decimal? WholesaleCost { get; set; }

        [Column("location")]
        [StringLength(100)]
        public string? Location { get; set; }

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [Required]
        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [Column("last_stock_take_date")]
        public DateTime? LastStockTakeDate { get; set; }

        [Column("last_stock_take_quantity")]
        public int? LastStockTakeQuantity { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Computed properties for business logic
        [NotMapped]
        public bool IsLowStock => CurrentStock <= ReorderLevel;

        [NotMapped]
        public bool IsOverstocked => CurrentStock > MaxStockLevel;

        [NotMapped]
        public bool IsExpiringSoon => ExpiryDate.HasValue &&
            ExpiryDate.Value <= DateTime.UtcNow.AddDays(30);

        [NotMapped]
        public bool IsExpired => ExpiryDate.HasValue &&
            ExpiryDate.Value <= DateTime.UtcNow;

        [NotMapped]
        public decimal? TotalInventoryValue => UnitCost.HasValue ?
            UnitCost.Value * CurrentStock : null;

        [NotMapped]
        public decimal? TotalWholesaleValue => WholesaleCost.HasValue ?
            WholesaleCost.Value * CurrentStock : null;

        [NotMapped]
        public int DaysUntilExpiry => ExpiryDate.HasValue ?
            (int)(ExpiryDate.Value - DateTime.UtcNow).TotalDays : int.MaxValue;

        [NotMapped]
        public double StockTurnoverRate => LastStockTakeQuantity.HasValue && LastStockTakeQuantity > 0 ?
            (double)CurrentStock / LastStockTakeQuantity.Value : 0;

        // Helper method for stock status
        [NotMapped]
        public string StockStatus
        {
            get
            {
                if (IsExpired) return "Expired";
                if (IsExpiringSoon) return "Expiring Soon";
                if (IsLowStock) return "Low Stock";
                if (IsOverstocked) return "Overstocked";
                return "Normal";
            }
        }
    }
}
