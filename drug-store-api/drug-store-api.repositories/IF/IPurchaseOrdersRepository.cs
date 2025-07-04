using drug_store_api.entities.Products;
using drug_store_api.entities.PurchaseOrderItems;
using drug_store_api.entities.PurchaseOrders;

namespace drug_store_api.repositories.IF
{
    public interface IPurchaseOrdersRepository
    {
        Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrder();
        Task<PurchaseOrder?> GetByIdAsync(Guid id);
        Task AddAsync(PurchaseOrder order, List<PurchaseOrderItem> items);
        Task UpdateAsync(PurchaseOrder order, List<PurchaseOrderItem> items);
        Task DeleteAsync(PurchaseOrder order);
        Task<(PurchaseOrder, List<PurchaseOrderItem>)?> GetDetailsAsync(Guid purchaseId);
        Task<Product?> GetProductAsync(Guid productId);
    }
}
