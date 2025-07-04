namespace drug_store_api.dtos.PurchaseOrders
{
    public class PurchaseOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int QuantityOrdered { get; set; }
        public decimal UnitCost { get; set; }
    }
}
