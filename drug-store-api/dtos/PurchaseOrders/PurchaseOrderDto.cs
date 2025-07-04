using drug_store_api.entities.PurchaseOrders;

namespace drug_store_api.dtos.PurchaseOrders
{
    public class PurchaseOrderDto
    {
        public Guid PurchaseId { get; set; }

        public Guid SupplierId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpectedDeliveryDate { get; set; }

        public DateTime? ActualDeliveryDate { get; set; }

        public decimal TotalAmount { get; set; }

        public PurchaseOrderStatusEnum? Status { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
