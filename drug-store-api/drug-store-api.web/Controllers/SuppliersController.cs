using drug_store_api.dtos.Suppliers;
using drug_store_api.entities.Suppliers;
using drug_store_api.services.IF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drug_store_api.web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/suppliers")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        public SuppliersController(ISupplierService supplierService)
        { 
            this._supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService)); ;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetSuppliers()
        {
            var suppliers = await _supplierService.GetSuppliersAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetSupplier(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound();
            return Ok(supplier);
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateSupplier(SupplierDto supplierDto)
        {
            await _supplierService.CreateSupplier(supplierDto);
            return NoContent();
        }

        [HttpPost("update")]
        public async Task<ActionResult> UpdateSupplier(SupplierDto supplierDto)
        {
            await _supplierService.UpdateSupplierInfo(supplierDto);
            return NoContent();
        }

        [HttpPost("delete")]
        public async Task<ActionResult> DeleteSupplier([FromBody] Guid supplierId)
        {
            await _supplierService.DeleteSupplierAsync(supplierId);
            return NoContent();
        }
    }
}
