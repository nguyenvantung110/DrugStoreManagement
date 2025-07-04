using drug_store_api.entities.PurchaseOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace drug_store_api.data.Configurations
{
    internal class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.HasKey(e => new { e.UserId });
        }
    }
}
