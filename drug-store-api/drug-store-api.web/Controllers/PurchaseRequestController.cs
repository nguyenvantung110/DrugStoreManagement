using drug_store_api.services.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace drug_store_api.web.Controllers
{
    [Route("api/purchase-request")]
    [ApiController]
    public class PurchaseRequestController : ControllerBase
    {
        private readonly IPurchaseRequestService _service;
        public PurchaseRequestController(IPurchaseRequestService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(service)); ;
        }

        [HttpGet]
        [Route("get-by-created-date/{requestDate}")]
        public async Task<IActionResult> GetByCreatedDate(DateTime requestDate)
        {
            var res = await _service.GetByRequestDate(requestDate);
            return Ok(res);
        }
    }
}
