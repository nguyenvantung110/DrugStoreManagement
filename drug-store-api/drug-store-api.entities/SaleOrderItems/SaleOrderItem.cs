using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drug_store_api.entities.SaleOrderItems
{
    [Table("sale_order_items")]
    public class SaleOrderItem
    {
        [Key]
        [Column("order_item_id")]
        public Guid OrderItemId { get; set; }

        [Required]
        [Column("order_id")]
        public Guid OrderId { get; set; }

        [Required]
        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Column("batch_id")]
        public Guid? BatchId { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required, Column("unit_price", TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Required, Column("sub_total", TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        [Column("dosage")]
        public string? Dosage { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
