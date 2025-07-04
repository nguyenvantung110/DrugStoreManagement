using drug_store_api.dtos.PurchaseOrders;

namespace drug_store_api.services.IF
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrder();
        Task<PurchaseOrderDetailDto?> GetDetailsAsync(Guid purchaseId);
        Task CreateAsync(PurchaseOrderCreateDto dto);
        Task UpdateAsync(Guid id, PurchaseOrderUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
