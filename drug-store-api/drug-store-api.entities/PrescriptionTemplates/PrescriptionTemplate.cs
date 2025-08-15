using drug_store_api.entities.Categories;
using drug_store_api.entities.PrescriptionTemplateItems;
using drug_store_api.entities.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drug_store_api.entities.PrescriptionTemplates
{
    [Table("prescription_templates")]
    public class PrescriptionTemplate
    {
        [Key]
        [Column("template_id")]
        public Guid TemplateId { get; set; }

        [Column("template_name")]
        public string TemplateName { get; set; } = default!;

        [Column("short_description")]
        public string? ShortDescription { get; set; }

        [Column("full_description")]
        public string? FullDescription { get; set; }

        [Column("user_id")]
        public Guid? UserId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("usage_frequency")]
        public int UsageFrequency { get; set; }

        [Column("category_id")]
        public Guid CategoryId { get; set; }

        public Category Category { get; set; }
        public List<PrescriptionTemplateItem> PrescriptionItems { get; set; } = new();
        public User User { get; set; }
    }
}
