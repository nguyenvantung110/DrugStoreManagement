using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.SaleOrders
{
    public class SaleOrderItemDto
    {
        public Guid ItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid? BatchId { get; set; }
        public string? Dosage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
