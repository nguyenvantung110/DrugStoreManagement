using drug_store_api.entities.PurchaseOrders;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using drug_store_api.entities.PrescriptionTemplateItems;

namespace drug_store_api.data.Configurations
{
    internal class PrescriptionTemplateItemConfiguration : IEntityTypeConfiguration<PrescriptionTemplateItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionTemplateItem> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.HasKey(e => new { e.TemplateId, e.ProductId });
        }
    }
}
