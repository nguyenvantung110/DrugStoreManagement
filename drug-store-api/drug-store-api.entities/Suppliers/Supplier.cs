using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.entities.Suppliers
{
    [Table("suppliers")]
    public class Supplier
    {
        [Column("supplier_id")]
        [Key]
        [Required]
        public Guid Supplier_Id { get; set; }

        [Column("supplier_name")]
        public string Supplier_Name { get; set; }

        [Column("contact_person")]
        public string Contact_Person { get; set; }

        [Column("phone_number")]
        public string Phone_Number { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("created_at")]
        public DateTime Created_At { get; set; }

        [Column("updated_at")]
        public DateTime Updated_At { get; set; }
    }
}
