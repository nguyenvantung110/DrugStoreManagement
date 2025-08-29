using drug_store_api.data;
using drug_store_api.entities.SaleOrderItems;
using drug_store_api.repositories.IF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.repositories.Factory
{
    public class SalesOrderItemsRepository : ISalesOrderItemsRepository
    {
        private readonly DrugStoreDbContext _context;
        public SalesOrderItemsRepository(DrugStoreDbContext context)
        {
            this._context = context;
        }

        public async Task CreateSaleOrderItems(IEnumerable<SaleOrderItem> saleOrderItem)
        {
            await _context.SaleOrderItems.AddRangeAsync(saleOrderItem);
            await _context.SaveChangesAsync();
        }
    }
}
