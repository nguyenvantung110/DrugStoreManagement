using drug_store_api.services.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace drug_store_api.web.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoriesController(ICategoryService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCategory()
        {
            var res = await _service.GetAllCategory();
            return Ok(res);
        }

        [HttpGet("get-by-type")]
        public async Task<IActionResult> GetCategoryByType([FromQuery] string categoryId)
        {
            var res = await _service.GetCategoryByType(categoryId);
            return Ok(res);
        }
    }
}
