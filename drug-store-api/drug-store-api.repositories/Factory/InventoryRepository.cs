using drug_store_api.data;
using drug_store_api.entities.Inventory;
using drug_store_api.repositories.IF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.repositories.Factory
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly DrugStoreDbContext _context;

        public InventoryRepository(DrugStoreDbContext context)
        {
            this._context = context;
        }

        public async Task<ProductInventory?> GetInventoryByProductAndBatchAsync(Guid productId, Guid? batchId = null)
        {
            return await _context.ProductInventories
                .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.BatchId == batchId);
        }

        public async Task<List<ProductInventory>> GetInventoriesByProductIdAsync(Guid productId)
        {
            return await _context.ProductInventories
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
        }

        public async Task<ProductInventory> CreateInventoryAsync(ProductInventory inventory)
        {
            await _context.ProductInventories.AddAsync(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<ProductInventory> UpdateInventoryAsync(ProductInventory inventory)
        {
            _context.ProductInventories.Update(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<bool> ProductExistsAsync(Guid productId)
        {
            return await _context.Products.AnyAsync(p => p.ProductId == productId);
        }

        public async Task<bool> BatchExistsAsync(Guid batchId)
        {
            // Assuming you have a Batches table - adjust accordingly
            return await _context.Batches.AnyAsync(b => b.BatchId == batchId);
        }

        public async Task<InventoryTransaction> CreateTransactionAsync(InventoryTransaction transaction)
        {
            await _context.InventoryTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<InventoryTransaction>> GetTransactionHistoryAsync(Guid productId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.InventoryTransactions
                .Where(it => it.ProductId == productId);

            if (fromDate.HasValue)
                query = query.Where(it => it.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(it => it.TransactionDate <= toDate.Value);

            return await query
                .OrderByDescending(it => it.TransactionDate)
                .ToListAsync();
        }

        public async Task<List<ProductInventory>> GetLowStockItemsAsync()
        {
            return await _context.ProductInventories
                .Where(pi => pi.CurrentStock <= pi.ReorderLevel)
                .OrderBy(pi => pi.CurrentStock)
                .ToListAsync();
        }

        public async Task<List<ProductInventory>> GetExpiredItemsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.ProductInventories
                .Where(pi => pi.ExpiryDate.HasValue && pi.ExpiryDate.Value <= now)
                .OrderBy(pi => pi.ExpiryDate)
                .ToListAsync();
        }

        public async Task<List<ProductInventory>> GetInventoriesByProductIds(List<Guid> productIds)
        {
            return await _context.ProductInventories
                .Where(pi => productIds.Contains(pi.ProductId))
                .ToListAsync();
        }

        public async Task UpdateInventoriesAsync(List<ProductInventory> inventories)
        {
            _context.ProductInventories.UpdateRange(inventories);
            await _context.SaveChangesAsync();
        }

        public async Task CreateTransactionsAsync(List<InventoryTransaction> transactions)
        {
            await _context.InventoryTransactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InventoryTransaction>> GetTransactionsByOrderId(Guid orderId)
        {
            return await _context.InventoryTransactions
                .Where(it => it.RelatedOrderId == orderId)
                .ToListAsync();
        }
    }
}
