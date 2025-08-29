using drug_store_api.entities.PurchaseOrders;
using drug_store_api.entities.SalesOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.repositories.IF
{
    public interface ISalesOrderRepository
    {
        Task<IEnumerable<SalesOrder>> GetSalesOrderByCreatedDate(DateTime createdDate);
        Task<SalesOrder> GetSalesOrderDetails();
        Task CreateSaleOrder(SalesOrder saleOrder);
        Task<SalesOrder?> GetSaleOrderById(Guid orderId);
        Task<List<SalesOrder>> GetSaleOrdersByDateRange(DateTime fromDate, DateTime toDate);
        Task<bool> UpdateSaleOrder(SalesOrder saleOrder);
    }
}
