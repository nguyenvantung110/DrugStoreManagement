using drug_store_api.services.IF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace drug_store_api.web.Controllers
{
    //[Authorize]
    [Route("api/prescriptions")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _service;
        public PrescriptionsController(IPrescriptionService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("get-templates")]
        public async Task<IActionResult> GetPrescriptionTemplate([FromQuery] Guid? categoryId)
        {
            var res = await _service.GetPrescriptionTemplate(categoryId);
            return Ok(res);
        }
    }
}
