using drug_store_api.entities.PurchaseOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.PurchaseOrders
{
    public class PurchaseOrderDetailDto
{
        public Guid PurchaseId { get; set; }
        public Guid SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public PurchaseOrderStatusEnum? Status { get; set; }
        public Guid UserId { get; set; }
        public List<PurchaseOrderItemDetailDto> Items { get; set; } = new();
    }
}
