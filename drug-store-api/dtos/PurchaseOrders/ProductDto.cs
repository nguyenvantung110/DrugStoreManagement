using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.PurchaseOrders
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string GenericName { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal WholesalePrice { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public bool RequiresPrescription { get; set; }
        public int StockThreshold { get; set; }
    }
}
