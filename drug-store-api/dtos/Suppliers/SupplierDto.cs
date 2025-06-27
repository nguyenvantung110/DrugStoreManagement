using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace drug_store_api.dtos.Suppliers
{
    public class SupplierDto
    {
        public Guid Supplier_Id { get; set; }
        public string Supplier_Name { get; set; }

        public string Contact_Person { get; set; }

        public string Phone_Number { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DateTime? Created_At { get; set; }

        public DateTime? Updated_At { get; set; }
    }
}
