using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drug_store_api.entities.SalesOrders
{
    public enum PaymentMethodEnum { Cash, BankTransfer, Card }
    public enum SalesOrderStatusEnum { Completed, Canceled, Pending }

    public class SalesOrder
    {
        [Key]
        public Guid OrderId { get; set; }

        public Guid? CustomerId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalAmount { get; set; }

        public PaymentMethodEnum? PaymentMethod { get; set; }

        public SalesOrderStatusEnum? Status { get; set; }

        [Required]
        public Guid UserId { get; set; } // Nhân viên thực hiện

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
