using drug_store_api.dtos.Customers;
using drug_store_api.dtos.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.SaleOrders
{
    public class OrderCreateDto
    {
        public CustomerForOrderDto Customer { get; set; }
        public decimal GrandTotal { get; set; }
        public IEnumerable<ProductForOrderDto> Products { get; set; }
        public string Notes { get; set; }
        public string PaymentMethod { get; set; }
        public Guid UserId { get; set; }
    }
}
