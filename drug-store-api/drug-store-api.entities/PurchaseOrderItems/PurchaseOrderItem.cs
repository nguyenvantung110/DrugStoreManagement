using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drug_store_api.entities.PurchaseOrderItems
{
    public class PurchaseOrderItem
    {
        [Key]
        public Guid PurchaseItemId { get; set; }

        [Required]
        public Guid PurchaseId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int QuantityOrdered { get; set; }

        public int? QuantityReceived { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal UnitCost { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
