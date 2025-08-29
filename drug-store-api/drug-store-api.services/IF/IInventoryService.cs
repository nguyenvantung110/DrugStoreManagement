using drug_store_api.dtos.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.services.IF
{
    public interface IInventoryService
    {
        Task<StockInResponseDto> ProcessStockInAsync(StockInRequestDto request, Guid userId);
        Task<bool> ValidateStockAvailabilityAsync(Guid productId, int requiredQuantity, Guid? batchId = null);
        Task ProcessStockOutAsync(Guid productId, int quantity, string reason, Guid? relatedOrderId = null, Guid? batchId = null, Guid? userId = null);
        Task<List<ProductInventoryDto>> GetLowStockReportAsync();
        Task<List<ProductInventoryDto>> GetExpiredItemsReportAsync();
        Task<List<ProductInventoryDto>> GetInventoryStatusAsync(Guid productId);
    }
}
