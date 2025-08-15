using drug_store_api.entities.PrescriptionTemplates;
using drug_store_api.entities.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.entities.PrescriptionTemplateItems
{
    [Table("prescription_template_items")]
    public class PrescriptionTemplateItem
    {
        [Key]
        [Column("template_id")]
        public Guid TemplateId { get; set; }

        [Key]
        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("dosage")]
        public string? Dosage { get; set; }

        [Column("usage_instructions")]
        public string? UsageInstructions { get; set; }

        public PrescriptionTemplate? PrescriptionTemplate { get; set; }

        public Product? Product { get; set; } = new();
    }
}
