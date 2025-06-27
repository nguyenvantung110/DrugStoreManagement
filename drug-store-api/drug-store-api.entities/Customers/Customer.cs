using System.ComponentModel.DataAnnotations;

namespace drug_store_api.entities.Customers
{
    public enum GenderEnum { Male, Female, Other }

    public class Customer
    {
        [Key]
        public Guid CustomerId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public GenderEnum? Gender { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
