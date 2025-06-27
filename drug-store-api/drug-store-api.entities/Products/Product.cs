using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.entities.Products
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required, StringLength(255)]
        public string ProductName { get; set; }

        [StringLength(255)]
        public string GenericName { get; set; }

        public string Description { get; set; }

        [StringLength(50)]
        public string UnitOfMeasure { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerUnit { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal WholesalePrice { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        [StringLength(100)]
        public string Manufacturer { get; set; }

        public bool RequiresPrescription { get; set; } = false;

        public int StockThreshold { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
