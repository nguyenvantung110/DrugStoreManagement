using drug_store_api.entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.repositories.IF
{
    public interface IInventoryRepository
    {
        Task<ProductInventory?> GetInventoryByProductAndBatchAsync(Guid productId, Guid? batchId = null);
        Task<List<ProductInventory>> GetInventoriesByProductIdAsync(Guid productId);
        Task<ProductInventory> CreateInventoryAsync(ProductInventory inventory);
        Task<ProductInventory> UpdateInventoryAsync(ProductInventory inventory);
        Task<bool> ProductExistsAsync(Guid productId);
        Task<bool> BatchExistsAsync(Guid batchId);
        Task<InventoryTransaction> CreateTransactionAsync(InventoryTransaction transaction);
        Task<List<InventoryTransaction>> GetTransactionHistoryAsync(Guid productId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<ProductInventory>> GetLowStockItemsAsync();
        Task<List<ProductInventory>> GetExpiredItemsAsync();
        Task<List<ProductInventory>> GetInventoriesByProductIds(List<Guid> productIds);
        Task UpdateInventoriesAsync(List<ProductInventory> inventories);

        // Transaction methods
        Task CreateTransactionsAsync(List<InventoryTransaction> transactions);
        Task<List<InventoryTransaction>> GetTransactionsByOrderId(Guid orderId);
    }
}
