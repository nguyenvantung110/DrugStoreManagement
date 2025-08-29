using drug_store_api.dtos.SaleOrders;

namespace drug_store_api.services.IF
{
    public interface ISalesOrderService
    {
        Task<SaleOrderResponseDto> CreateOrder(OrderCreateDto orderCreateDto);
        Task<IEnumerable<SalesOrderDto>> GetSaleOrderByCreatedDate(DateTime createdDate);
        Task<SalesOrderDto?> GetSaleOrderById(Guid orderId);
        Task<IEnumerable<SalesOrderDto>> GetSaleOrdersByDateRange(DateTime fromDate, DateTime toDate);
        Task<bool> CancelOrder(Guid orderId, string reason);
    }
}
