using System.ComponentModel.DataAnnotations;

namespace drug_store_api.entities.PrescriptionItems
{
    public class PrescriptionItem
    {
        [Key]
        public Guid PrescriptionItemId { get; set; }

        [Required]
        public Guid PrescriptionId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [StringLength(100)]
        public string Dosage { get; set; }

        public string Instructions { get; set; }

        public int? FilledQuantity { get; set; }

        public DateTime? FilledDate { get; set; }
    }
}
