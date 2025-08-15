using drug_store_api.dtos.Products;

namespace drug_store_api.dtos.PurchaseOrders
{
    public class PurchaseOrderItemDetailDto
{
    public Guid PurchaseItemId { get; set; }
    public Guid ProductId { get; set; }
    public int QuantityOrdered { get; set; }
    public int? QuantityReceived { get; set; }
    public decimal UnitCost { get; set; }
    public decimal SubTotal { get; set; }
    public ProductDto Product { get; set; }
}
}
