using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drug_store_api.entities.PurchaseOrderItems
{
    [Table("purchase_order_items")]
    public class PurchaseOrderItem
    {
        [Key]
        [Column("purchase_item_id")]
        public Guid PurchaseItemId { get; set; }

        [Required]
        [Column("purchase_id")]
        public Guid PurchaseId { get; set; }

        [Required]
        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Required]
        [Column("quantity_ordered")]
        public int QuantityOrdered { get; set; }

        [Column("quantity_received")]
        public int? QuantityReceived { get; set; }

        [Required, Column("unit_cost", TypeName = "decimal(10,2)")]
        public decimal UnitCost { get; set; }

        [Required, Column("sub_total", TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
