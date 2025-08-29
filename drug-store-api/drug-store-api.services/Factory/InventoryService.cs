using AutoMapper;
using drug_store_api.dtos.Inventory;
using drug_store_api.entities.Inventory;
using drug_store_api.repositories.IF;
using drug_store_api.services.IF;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.services.Factory
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IMapper mapper,
            ILogger<InventoryService> logger)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> ValidateStockAvailabilityAsync(Guid productId, int requiredQuantity, Guid? batchId = null)
        {
            var inventory = await _inventoryRepository.GetInventoryByProductAndBatchAsync(productId, batchId);

            if (inventory == null)
                return false;

            return inventory.CurrentStock >= requiredQuantity;
        }

        public async Task ProcessStockOutAsync(Guid productId, int quantity, string reason,
            Guid? relatedOrderId = null, Guid? batchId = null, Guid? userId = null)
        {
            if (!await ValidateStockAvailabilityAsync(productId, quantity, batchId))
            {
                throw new InvalidOperationException($"Insufficient stock for product {productId}");
            }

            var inventory = await _inventoryRepository.GetInventoryByProductAndBatchAsync(productId, batchId);

            // Update inventory
            inventory!.CurrentStock -= quantity;
            inventory.LastUpdated = DateTime.UtcNow;
            await _inventoryRepository.UpdateInventoryAsync(inventory);

            // Create transaction record
            var transaction = new InventoryTransaction
            {
                TransactionId = Guid.NewGuid(),
                ProductId = productId,
                BatchId = batchId,
                TransactionType = InventoryTransactionTypeEnum.Out,
                QuantityChange = -quantity,
                CurrentStock = inventory.CurrentStock,
                TransactionDate = DateTime.UtcNow,
                Reason = reason,
                RelatedOrderId = relatedOrderId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _inventoryRepository.CreateTransactionAsync(transaction);

            _logger.LogInformation("Stock out processed for Product {ProductId}: -{Quantity} units",
                productId, quantity);
        }

        public async Task<StockInResponseDto> ProcessStockInAsync(StockInRequestDto request, Guid userId)
        {
            _logger.LogInformation("Processing stock in for {ItemCount} items", request.Items.Count);

            var response = new StockInResponseDto
            {
                TransactionDate = DateTime.UtcNow,
                UpdatedProducts = new List<string>()
            };

            try
            {
                foreach (var stockInDto in request.Items)
                {
                    // Validate product exists (implement this method)
                    if (!await _inventoryRepository.ProductExistsAsync(stockInDto.ProductId))
                    {
                        throw new ArgumentException($"Product with ID {stockInDto.ProductId} not found");
                    }

                    // Get or create inventory record
                    var inventory = await _inventoryRepository.GetInventoryByProductAndBatchAsync(
                        stockInDto.ProductId, stockInDto.BatchId);

                    if (inventory == null)
                    {
                        // Create new inventory record
                        var newInventory = _mapper.Map<ProductInventory>(stockInDto);
                        newInventory.CurrentStock = stockInDto.Quantity;
                        newInventory.LastUpdated = DateTime.UtcNow;

                        inventory = await _inventoryRepository.CreateInventoryAsync(newInventory);
                    }
                    else
                    {
                        // Update existing inventory
                        inventory.CurrentStock += stockInDto.Quantity;
                        inventory.LastUpdated = DateTime.UtcNow;

                        // Update cost information if provided
                        if (stockInDto.UnitCost.HasValue)
                            inventory.UnitCost = stockInDto.UnitCost;

                        if (stockInDto.WholesaleCost.HasValue)
                            inventory.WholesaleCost = stockInDto.WholesaleCost;

                        if (!string.IsNullOrWhiteSpace(stockInDto.Location))
                            inventory.Location = stockInDto.Location;

                        if (stockInDto.ExpiryDate.HasValue)
                            inventory.ExpiryDate = stockInDto.ExpiryDate;

                        await _inventoryRepository.UpdateInventoryAsync(inventory);
                    }

                    // Create inventory transaction record
                    var transaction = new InventoryTransaction
                    {
                        TransactionId = Guid.NewGuid(),
                        ProductId = stockInDto.ProductId,
                        BatchId = stockInDto.BatchId,
                        TransactionType = InventoryTransactionTypeEnum.In,
                        QuantityChange = stockInDto.Quantity,
                        CurrentStock = inventory.CurrentStock,
                        TransactionDate = DateTime.UtcNow,
                        Reason = request.Reason,
                        RelatedOrderId = !string.IsNullOrEmpty(request.PurchaseOrderId) ?
                            Guid.Parse(request.PurchaseOrderId) : null,
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _inventoryRepository.CreateTransactionAsync(transaction);
                    response.UpdatedProducts.Add(stockInDto.ProductId.ToString());
                }

                response.Success = true;
                response.Message = $"Successfully processed stock in for {request.Items.Count} items";

                _logger.LogInformation("Stock in processed successfully for {ItemCount} items", request.Items.Count);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to process stock in: {ex.Message}";
                _logger.LogError(ex, "Error processing stock in");
                throw;
            }

            return response;
        }

        public async Task<List<ProductInventoryDto>> GetLowStockReportAsync()
        {
            var lowStockItems = await _inventoryRepository.GetLowStockItemsAsync();
            return _mapper.Map<List<ProductInventoryDto>>(lowStockItems);
        }

        public async Task<List<ProductInventoryDto>> GetExpiredItemsReportAsync()
        {
            var expiredItems = await _inventoryRepository.GetExpiredItemsAsync();
            return _mapper.Map<List<ProductInventoryDto>>(expiredItems);
        }

        public async Task<List<ProductInventoryDto>> GetInventoryStatusAsync(Guid productId)
        {
            var inventories = await _inventoryRepository.GetInventoriesByProductIdAsync(productId);
            return _mapper.Map<List<ProductInventoryDto>>(inventories);
        }
    }
}
