using drug_store_api.entities.PurchaseOrderItems;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drug_store_api.entities.PurchaseOrders
{
    public enum PurchaseOrderStatusEnum {
        [PgName("Pending")]
        Pending,
        [PgName("Received")]
        Received,
        [PgName("Canceled")]
        Canceled 
    }

    [Table("purchase_orders")]
    public class PurchaseOrder
    {
        [Key]
        [Column("purchase_id")]
        public Guid PurchaseId { get; set; }

        [Required]
        [Column("supplier_id")]
        public Guid SupplierId { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column("expected_delivery_date")]
        public DateTime? ExpectedDeliveryDate { get; set; }

        [Column("actual_delivery_date")]
        public DateTime? ActualDeliveryDate { get; set; }

        [Column("total_amount", TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column("status")]
        public PurchaseOrderStatusEnum? Status { get; set; }

        [Required]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
