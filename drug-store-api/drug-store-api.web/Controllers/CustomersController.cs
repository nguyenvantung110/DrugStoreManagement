using drug_store_api.services.IF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace drug_store_api.web.Controllers
{
    [Authorize]
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IUserService _service;
        public CustomersController(IUserService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(service));
        }
    }
}
