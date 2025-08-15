using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drug_store_api.entities.Categories
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public Guid CategoryId { get; set; }

        [Column("category_name")]
        public string CategoryName { get; set; }

        [Column("category_type")]
        public string CategoryType { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
