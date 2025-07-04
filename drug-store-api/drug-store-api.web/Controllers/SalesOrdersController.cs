using drug_store_api.dtos.PurchaseOrders;
using drug_store_api.services.IF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace drug_store_api.web.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/sales-orders")]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrderService _service;
        public SalesOrdersController(ISalesOrderService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(service)); ;
        }

        [HttpGet]
        [Route("get-by-created-date/{date}")]
        public async Task<IActionResult> GetSaleOrderByCreatedDate(DateTime createdDate)
        {
            var res = await _service.GetSaleOrderByCreatedDate(createdDate);
            return Ok(res);
        }
    }
}
