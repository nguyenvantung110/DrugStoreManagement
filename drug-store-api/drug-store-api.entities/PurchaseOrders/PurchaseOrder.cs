using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drug_store_api.entities.PurchaseOrders
{
    public enum PurchaseOrderStatusEnum { Pending, Received, Canceled }

    public class PurchaseOrder
    {
        [Key]
        public Guid PurchaseId { get; set; }

        [Required]
        public Guid SupplierId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public PurchaseOrderStatusEnum? Status { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
