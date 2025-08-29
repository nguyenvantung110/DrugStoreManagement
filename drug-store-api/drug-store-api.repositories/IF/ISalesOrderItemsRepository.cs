using drug_store_api.entities.SaleOrderItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.repositories.IF
{
    public interface ISalesOrderItemsRepository
    {
        Task CreateSaleOrderItems(IEnumerable<SaleOrderItem> saleOrderItem);
    }
}
