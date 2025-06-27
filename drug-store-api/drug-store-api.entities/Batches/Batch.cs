using System.ComponentModel.DataAnnotations;

namespace drug_store_api.entities.Batches
{
    public class Batch
    {
        [Key]
        public Guid BatchId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required, StringLength(100)]
        public string BatchNumber { get; set; }

        [Required]
        public DateTime ManufactureDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public int Quantity { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
