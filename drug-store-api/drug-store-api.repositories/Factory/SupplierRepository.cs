using drug_store_api.data;
using drug_store_api.entities.Suppliers;
using drug_store_api.entities.User;
using drug_store_api.repositories.IF;
using Microsoft.EntityFrameworkCore;

namespace drug_store_api.repositories.Factory
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly DrugStoreDbContext _context;
        public SupplierRepository(DrugStoreDbContext context)
        {
            this._context = context;
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task CreateSupplierAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            await _context.Suppliers
            .Where(x => x.Supplier_Id == supplier.Supplier_Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Supplier_Name, supplier.Supplier_Name)
                .SetProperty(x => x.Contact_Person, supplier.Contact_Person)
                .SetProperty(x => x.Phone_Number, supplier.Phone_Number)
                .SetProperty(x => x.Email, supplier.Email)
                .SetProperty(x => x.Address, supplier.Address)
                .SetProperty(x => x.Updated_At, DateTime.UtcNow)
            );
        }

        public async Task DeleteSupplierAsync(Guid supplierId)
        {
            await _context.Suppliers
            .Where(p => p.Supplier_Id == supplierId)
            .ExecuteDeleteAsync();
        }
    }
}
