using drug_store_api.entities.PrescriptionTemplateItems;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using drug_store_api.entities.Categories;
using drug_store_api.dtos.Categories;

namespace drug_store_api.dtos.PrescriptionTemplates
{
    public class PrescriptionTemplateDto
{
        public Guid TemplateId { get; set; }

        public string TemplateName { get; set; } = default!;

        public string? ShortDescription { get; set; }

        public string? FullDescription { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int UsageFrequency { get; set; }

        public CategoryByTypeDto Category { get; set; }
        public List<PrescriptionTemplateItemDto> PrescriptionItems { get; set; } = new();
    }
}
