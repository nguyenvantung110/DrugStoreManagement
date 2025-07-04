using drug_store_api.data;
using drug_store_api.entities.PurchaseOrders;
using drug_store_api.entities.PurchaseOrderItems;
using drug_store_api.repositories.IF;
using Microsoft.EntityFrameworkCore;
using drug_store_api.entities.Products;

namespace drug_store_api.repositories.Factory
{
    public class PurchaseOrdersRepository : IPurchaseOrdersRepository
    {
        private readonly DrugStoreDbContext _context;
        public PurchaseOrdersRepository(DrugStoreDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
        => await _context.PurchaseOrders.FirstOrDefaultAsync(x => x.PurchaseId == id);

        public async Task AddAsync(PurchaseOrder order, List<PurchaseOrderItem> items)
        {
            _context.PurchaseOrders.Add(order);
            _context.PurchaseOrderItems.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PurchaseOrder order, List<PurchaseOrderItem> items)
        {
            _context.PurchaseOrders.Update(order);

            var oldItems = _context.PurchaseOrderItems.Where(i => i.PurchaseId == order.PurchaseId);
            _context.PurchaseOrderItems.RemoveRange(oldItems);
            _context.PurchaseOrderItems.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PurchaseOrder order)
        {
            var items = _context.PurchaseOrderItems.Where(i => i.PurchaseId == order.PurchaseId);
            _context.PurchaseOrderItems.RemoveRange(items);
            _context.PurchaseOrders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task<(PurchaseOrder, List<PurchaseOrderItem>)?> GetDetailsAsync(Guid purchaseId)
        {
            var order = await _context.PurchaseOrders.FirstOrDefaultAsync(x => x.PurchaseId == purchaseId);
            if (order == null) return null;
            var items = await _context.PurchaseOrderItems.Where(i => i.PurchaseId == purchaseId).ToListAsync();
            return (order, items);
        }

        public async Task<Product?> GetProductAsync(Guid productId)
            => await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

        public async Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrder()
            => await _context.PurchaseOrders.ToListAsync();
    }
}
