using drug_store_api.data.Configurations;
using drug_store_api.entities.Batches;
using drug_store_api.entities.Customers;
using drug_store_api.entities.InventoryTransactions;
using drug_store_api.entities.PrescriptionItems;
using drug_store_api.entities.Prescriptions;
using drug_store_api.entities.Products;
using drug_store_api.entities.PurchaseOrderItems;
using drug_store_api.entities.PurchaseOrders;
using drug_store_api.entities.PurchaseRequestItems;
using drug_store_api.entities.PurchaseRequests;
using drug_store_api.entities.SaleOrderItems;
using drug_store_api.entities.SalesOrders;
using drug_store_api.entities.Suppliers;
using drug_store_api.entities.Users;
using Microsoft.EntityFrameworkCore;

namespace drug_store_api.data
{
    public class DrugStoreDbContext : DbContext
    {
        public DrugStoreDbContext(DbContextOptions<DrugStoreDbContext> options) : base(options) { }
        #region List DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SaleOrderItem> SaleOrderItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderConfiguration());
        }
    }
}
