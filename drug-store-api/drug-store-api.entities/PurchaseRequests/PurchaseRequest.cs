using drug_store_api.entities.SalesOrders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;

namespace drug_store_api.entities.PurchaseRequests
{
    public enum PurchaseRequestStatusEnum {
        [PgName("Pending")]
        Pending,
        [PgName("Ordered")]
        Ordered,
        [PgName("Canceled")]
        Canceled,
    }

    [Table("purchase_requests")]
    public class PurchaseRequest
    {
        [Key]
        [Required]
        [Column("request_id")]
        public Guid RequestId { get; set; }

        [Column("created_by")]
        public Guid CreatedBy { get; set; }

        [Column("request_date", TypeName = "timestamp without time zone")]
        public DateTime RequestDate { get; set; }

        [Column("expected_delivery_date", TypeName = "timestamp without time zone")]
        public DateTime? ExpectedDeliveryDate { get; set; }

        [Column("supplier_id")]
        public Guid? SupplierId { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("status")]
        public PurchaseRequestStatusEnum Status { get; set; } = PurchaseRequestStatusEnum.Pending;

        [Column("created_at", TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at", TypeName = "timestamp without time zone")]
        public DateTime UpdatedAt { get; set; }
    }
}
