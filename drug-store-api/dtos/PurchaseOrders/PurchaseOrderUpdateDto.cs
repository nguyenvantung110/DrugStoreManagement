using drug_store_api.entities.PurchaseOrders;

namespace drug_store_api.dtos.PurchaseOrders
{
    public class PurchaseOrderUpdateDto
    {
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public PurchaseOrderStatusEnum? Status { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }
}
