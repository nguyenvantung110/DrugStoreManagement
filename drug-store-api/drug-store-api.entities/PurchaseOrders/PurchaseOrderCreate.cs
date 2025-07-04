using drug_store_api.entities.PurchaseOrderItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.entities.PurchaseOrders
{
    public class PurchaseOrderCreate
    {
        public Guid SupplierId { get; set; }
        public Guid UserId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    }
}
