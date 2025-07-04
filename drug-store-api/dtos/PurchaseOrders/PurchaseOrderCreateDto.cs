namespace drug_store_api.dtos.PurchaseOrders
{
    public class PurchaseOrderCreateDto
    {
        public Guid SupplierId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }
}
