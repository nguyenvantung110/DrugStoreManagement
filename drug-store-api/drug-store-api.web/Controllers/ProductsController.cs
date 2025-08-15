using drug_store_api.dtos.Products;
using drug_store_api.dtos.Users;
using drug_store_api.services.IF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drug_store_api.web.Controllers
{
    [Authorize]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductsController(IProductService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("get-basic-info-list")]
        public async Task<IActionResult> GetBasicProducInfoList()
        {
            var res = await _service.GetBasicProducInfoAsync();
            return Ok(res);
        }
    }
}
