using System.ComponentModel.DataAnnotations;

namespace drug_store_api.entities.InventoryTransactions
{
    public enum InventoryTransactionTypeEnum { In, Out, Adjustment }

    public class InventoryTransaction
    {
        [Key]
        public Guid TransactionId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        public Guid? BatchId { get; set; }

        [Required]
        public InventoryTransactionTypeEnum TransactionType { get; set; }

        [Required]
        public int QuantityChange { get; set; }

        public int? CurrentStock { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public string Reason { get; set; }

        public Guid? RelatedOrderId { get; set; }

        public Guid? UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
