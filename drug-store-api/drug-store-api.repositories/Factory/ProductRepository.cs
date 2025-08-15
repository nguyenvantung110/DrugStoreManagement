using drug_store_api.data;
using drug_store_api.entities.Products;
using drug_store_api.repositories.IF;
using Microsoft.EntityFrameworkCore;

namespace drug_store_api.repositories.Factory
{
    public class ProductRepository : IProductRepository
    {
        private readonly DrugStoreDbContext _context;
        public ProductRepository(DrugStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductBasicInfo?>> GetProductBasicInfoAsync()
        {
            var res = await _context.Products.Select(u => new ProductBasicInfo {
                ProductId = u.ProductId,
                ProductName = u.ProductName,
                Manufacturer = u.Manufacturer,
                UnitOfMeasure = u.UnitOfMeasure,
                PricePerUnit = u.PricePerUnit,
                Barcode = u.Barcode
            }).ToListAsync();

            return res;
        }
    }
}
