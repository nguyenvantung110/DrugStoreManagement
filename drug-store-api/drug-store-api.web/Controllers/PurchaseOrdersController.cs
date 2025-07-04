using drug_store_api.dtos.PurchaseOrders;
using drug_store_api.entities.PurchaseOrders;
using drug_store_api.services.IF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drug_store_api.web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/purchase-orders")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _service;
        public PurchaseOrdersController(IPurchaseOrderService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(service)); ;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAllPurchaseOrder()
        {
            var res = await _service.GetAllPurchaseOrder();
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseOrderCreateDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok();
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PurchaseOrderUpdateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<PurchaseOrderDetailDto>> GetDetails(Guid id)
        {
            var order = await _service.GetDetailsAsync(id);
            if (order == null) return NotFound();
            return order;
        }
    }
}
