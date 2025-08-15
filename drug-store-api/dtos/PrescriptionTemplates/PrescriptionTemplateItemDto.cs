using drug_store_api.dtos.Products;
using drug_store_api.dtos.PurchaseOrders;
using drug_store_api.entities.PrescriptionTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.PrescriptionTemplates
{
    public class PrescriptionTemplateItemDto
    {
        public Guid TemplateId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Dosage { get; set; }
        public string? UsageInstructions { get; set; }
        public ProductForPrescriptionDto? Product { get; set; }
    }
}
