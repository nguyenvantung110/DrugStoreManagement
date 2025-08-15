using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.entities.Products
{
    public class ProductBasicInfo
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Manufacturer { get; set; }
        public decimal PricePerUnit { get; set; }
        public string UnitOfMeasure { get; set; }
        public string? Barcode { get; set; }
    }
}
