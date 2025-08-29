using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.Inventory
{
    public class StockInDto
    {
        public Guid ProductId { get; set; }
        public Guid? BatchId { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? WholesaleCost { get; set; }
        public string? Location { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }
    }
}
