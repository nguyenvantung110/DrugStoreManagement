using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.Users
{
    public class UserListFilter
    {
        public string? Search {get; set;}
        public string? Role { get; set; }
        public string? Status { get; set; }
        public int? Page { get; set; }
        public int? Limit { get; set; }
    }
}
