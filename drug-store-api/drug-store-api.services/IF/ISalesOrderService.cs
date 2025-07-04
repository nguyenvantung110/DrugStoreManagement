using drug_store_api.dtos.SaleOrders;

namespace drug_store_api.services.IF
{
    public interface ISalesOrderService
    {
        Task<IEnumerable<SalesOrderDto>> GetSaleOrderByCreatedDate(DateTime createdDate);
    }
}
