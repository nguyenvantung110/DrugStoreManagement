using System.ComponentModel.DataAnnotations;

namespace drug_store_api.entities.Prescriptions
{
    public enum PrescriptionStatusEnum { Filled, Unfilled }

    public class Prescription
    {
        [Key]
        public Guid PrescriptionId { get; set; }

        public Guid? CustomerId { get; set; }

        [StringLength(100)]
        public string DoctorName { get; set; }

        public DateTime? PrescriptionDate { get; set; }

        public string Notes { get; set; }

        public PrescriptionStatusEnum? Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
