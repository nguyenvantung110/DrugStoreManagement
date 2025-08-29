using drug_store_api.dtos.Inventory;
using drug_store_api.services.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace drug_store_api.web.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpPost("stock-in")]
        public async Task<ActionResult<StockInResponseDto>> ProcessStockIn([FromBody] StockInRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // TODO: Get current user ID from JWT token
                var userId = Guid.NewGuid(); // Placeholder

                var response = await _inventoryService.ProcessStockInAsync(request, userId);

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in stock in request");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing stock in");
                return StatusCode(500, new { message = "Internal server error occurred" });
            }
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<List<object>>> GetLowStockReport()
        {
            try
            {
                var report = await _inventoryService.GetLowStockReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock report");
                return StatusCode(500, new { message = "Internal server error occurred" });
            }
        }

        [HttpGet("expired")]
        public async Task<ActionResult<List<object>>> GetExpiredItemsReport()
        {
            try
            {
                var report = await _inventoryService.GetExpiredItemsReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expired items report");
                return StatusCode(500, new { message = "Internal server error occurred" });
            }
        }

        [HttpGet("status/{productId}")]
        public async Task<ActionResult<object>> GetInventoryStatus(Guid productId)
        {
            try
            {
                var status = await _inventoryService.GetInventoryStatusAsync(productId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory status for product {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error occurred" });
            }
        }
    }
}
