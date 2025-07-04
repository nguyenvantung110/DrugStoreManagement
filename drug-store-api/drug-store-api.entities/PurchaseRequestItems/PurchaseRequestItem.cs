using System.ComponentModel.DataAnnotations;

namespace drug_store_api.entities.PurchaseRequestItems
{
    public class PurchaseRequestItem
    {
        [Key]
        public Guid RequestItemId { get; set; }
        public Guid RequestId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
