using drug_store_api.entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.dtos.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }
}
