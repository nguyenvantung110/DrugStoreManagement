using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.Customers
{
    public class CustomerForOrderDto
    {
        public string CustomerName { get; set; }

        public string PhoneNumber { get; set; }
    }
}
