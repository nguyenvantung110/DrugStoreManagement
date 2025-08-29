using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drug_store_api.entities.SalesOrders
{
    public enum PaymentMethodEnum
    {
        [PgName("Cash")] Cash,
        [PgName("BankTransfer")] BankTransfer,
        [PgName("Card")] Card
    }

    public enum SalesOrderStatusEnum
    {
        [PgName("Completed")] Completed,
        [PgName("Canceled")] Canceled,
        [PgName("Pending")] Pending
    }

    [Table("sales_orders")]
    public class SalesOrder
    {
        [Key]
        [Column("order_id")]
        public Guid OrderId { get; set; }

        [Column("customer_id")]
        public Guid? CustomerId { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column("total_amount", TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column("discount_amount", TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; }

        [Column("final_amount", TypeName = "decimal(10,2)")]
        public decimal FinalAmount { get; set; }

        [Column("payment_method")]
        public PaymentMethodEnum? PaymentMethod { get; set; }

        [Column("status")]
        public SalesOrderStatusEnum? Status { get; set; }

        [Required]
        [Column("user_id")]
        public Guid UserId { get; set; } // Nhân viên thực hiện

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
