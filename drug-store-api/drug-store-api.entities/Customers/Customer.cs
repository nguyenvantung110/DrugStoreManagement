using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drug_store_api.entities.Customers
{
    public enum GenderEnum { Male, Female, Other }

    [Table("customers")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public Guid CustomerId { get; set; }

        [Column("full_name")]
        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Column("phone_number")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Column("address")]
        [StringLength(255)]
        public string Address { get; set; }

        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("gender")]
        public GenderEnum? Gender { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
