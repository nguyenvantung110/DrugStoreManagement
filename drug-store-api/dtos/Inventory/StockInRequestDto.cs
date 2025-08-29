using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.Inventory
{
    public class StockInRequestDto
    {
        public List<StockInDto> Items { get; set; } = new();
        public string? PurchaseOrderId { get; set; }
        public string Reason { get; set; } = "Stock purchase";
    }
}
