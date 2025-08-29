using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.Inventory
{
    public class StockInResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> UpdatedProducts { get; set; } = new();
        public DateTime TransactionDate { get; set; }
    }
}
