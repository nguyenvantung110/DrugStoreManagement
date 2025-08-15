using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.Categories
{
    public class CategoryDto
    {
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryType { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
