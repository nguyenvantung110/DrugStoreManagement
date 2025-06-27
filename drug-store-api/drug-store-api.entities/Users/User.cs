using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace drug_store_api.entities.Users
{
    public enum UserRole
    {
        admin,
        user,
        guest,
        cashier
    }

    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("username")]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Column("password_hash")]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [Column("full_name")]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Column("role")]
        public UserRole Role { get; set; }

        [Column("email")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Column("phone_number")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

}
