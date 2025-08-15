using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using drug_store_api.entities.Categories;

namespace drug_store_api.entities.Products
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Required, StringLength(255)]
        [Column("product_name")]
        public string ProductName { get; set; }

        [StringLength(255)]
        [Column("generic_name")]
        public string GenericName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [StringLength(50)]
        [Column("unit_of_measure")]
        public string UnitOfMeasure { get; set; }

        [Column("price_per_unit", TypeName = "decimal(10,2)")]
        public decimal PricePerUnit { get; set; }

        [Column("wholesale_price", TypeName = "decimal(10,2)")]
        public decimal WholesalePrice { get; set; }

        [Column("category_id")]
        public Guid CategoryId { get; set; }

        [StringLength(100)]
        [Column("manufacturer")]
        public string Manufacturer { get; set; }

        [Column("requires_prescription")]
        public bool RequiresPrescription { get; set; } = false;

        [Column("stock_threshold")]
        public int StockThreshold { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("barcode")]
        public string? Barcode { get; set; }

        public Category Category { get; set; }
    }
}
