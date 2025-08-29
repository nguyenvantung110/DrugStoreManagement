using AutoMapper;
using drug_store_api.dtos.Customers;
using drug_store_api.dtos.Products;
using drug_store_api.dtos.SaleOrders;
using drug_store_api.entities.Customers;
using drug_store_api.entities.Inventory;
using drug_store_api.entities.SaleOrderItems;
using drug_store_api.entities.SalesOrders;
using drug_store_api.repositories.IF;
using drug_store_api.services.IF;
using drug_store_api.systemcommon.Transaction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace drug_store_api.services.Factory
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISalesOrderRepository _saleOrderRepository;
        private readonly ISalesOrderItemsRepository _saleOrderItemRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SalesOrderService> _logger;

        public SalesOrderService(
            ISalesOrderRepository saleOrderRepository,
            ISalesOrderItemsRepository saleOrderItemRepository,
            ICustomerRepository customerRepository,
            IInventoryRepository inventoryRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SalesOrderService> logger)
        {
            _saleOrderRepository = saleOrderRepository;
            _saleOrderItemRepository = saleOrderItemRepository;
            _customerRepository = customerRepository;
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        //public async Task<IEnumerable<SalesOrderDto>> GetSaleOrderByCreatedDate(DateTime createdDate)
        //{
        //    var saleOrder = await _saleOrderRepository.GetSalesOrderByCreatedDate(createdDate);
        //    var res = _mapper.Map<List<SalesOrderDto>>(saleOrder);
        //    return res;
        //}

        public async Task<IEnumerable<SalesOrderDto>> GetSaleOrderByCreatedDate(DateTime createdDate)
        {
            try
            {
                _logger.LogInformation("Getting sale orders for date: {CreatedDate}", createdDate);

                var saleOrders = await _saleOrderRepository.GetSalesOrderByCreatedDate(createdDate);
                var result = _mapper.Map<List<SalesOrderDto>>(saleOrders);

                _logger.LogInformation("Retrieved {Count} sale orders for date: {CreatedDate}",
                    result.Count, createdDate);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sale orders for date: {CreatedDate}", createdDate);
                throw;
            }
        }

        public async Task<SaleOrderResponseDto> CreateOrder(OrderCreateDto orderCreateDto)
        {
            _logger.LogInformation("Creating order for customer {CustomerName} with {ItemCount} items",
                orderCreateDto.Customer?.CustomerName, orderCreateDto.Products?.Count() ?? 0);

            try
            {
                // 1. Comprehensive validation
                await ValidateOrderRequest(orderCreateDto);

                // 2. Pre-validate all inventory availability (batch check)
                await ValidateInventoryAvailability(orderCreateDto.Products);

                using (var scope = TransactionScopeManager.CreateTransactionScope())
                {
                    // 3. Process or create customer
                    var customer = await ProcessCustomer(orderCreateDto.Customer);

                    // 4. Create sales order
                    var salesOrder = await CreateSalesOrder(orderCreateDto, customer?.CustomerId);

                    // 5. Create order items (batch insert)
                    var saleOrderItems = await CreateOrderItems(orderCreateDto.Products, salesOrder.OrderId);

                    // 6. Process inventory updates (optimized batch operations)
                    await ProcessInventoryUpdates(orderCreateDto.Products, salesOrder.OrderId, orderCreateDto.UserId);

                    scope.Complete();

                    var response = new SaleOrderResponseDto
                    {
                        Success = true,
                        Message = "Order created successfully",
                        OrderId = salesOrder.OrderId,
                        InvoiceNumber = GenerateInvoiceNumber(salesOrder.OrderId),
                        TotalAmount = orderCreateDto.GrandTotal,
                        OrderDate = salesOrder.CreatedAt,
                        CustomerName = orderCreateDto.Customer?.CustomerName ?? "Walk-in Customer"
                    };

                    _logger.LogInformation("Order created successfully. OrderId: {OrderId}, Amount: {Amount}",
                        salesOrder.OrderId, orderCreateDto.GrandTotal);

                    return response;
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation error creating order: {Message}", ex.Message);
                return new SaleOrderResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Business logic error creating order: {Message}", ex.Message);
                return new SaleOrderResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating order for customer {CustomerName}",
                    orderCreateDto.Customer?.CustomerName);
                return new SaleOrderResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the order"
                };
            }
        }

        private async Task ValidateOrderRequest(OrderCreateDto orderCreateDto)
        {
            if (orderCreateDto == null)
                throw new ArgumentNullException(nameof(orderCreateDto), "Order data is required");

            if (orderCreateDto.Customer == null || string.IsNullOrWhiteSpace(orderCreateDto.Customer.CustomerName))
                throw new ArgumentException("Customer information is required");

            if (orderCreateDto.Products == null || !orderCreateDto.Products.Any())
                throw new ArgumentException("Order must contain at least one product");

            if (orderCreateDto.GrandTotal <= 0)
                throw new ArgumentException("Grand total must be greater than 0");

            if (orderCreateDto.UserId == Guid.Empty)
                throw new ArgumentException("User ID is required");

            // Validate individual products
            var productValidationErrors = new List<string>();

            foreach (var product in orderCreateDto.Products)
            {
                if (product.ProductId == Guid.Empty)
                    productValidationErrors.Add("Invalid product ID");

                if (product.Quantity <= 0)
                    productValidationErrors.Add($"Invalid quantity for product {product.ProductId}");

                if (product.UnitPrice < 0)
                    productValidationErrors.Add($"Invalid unit price for product {product.ProductId}");

                if (product.SubTotal != product.Quantity * product.UnitPrice)
                    productValidationErrors.Add($"Subtotal mismatch for product {product.ProductId}");
            }

            if (productValidationErrors.Any())
            {
                var errorMessage = string.Join("; ", productValidationErrors);
                throw new ArgumentException($"Product validation errors: {errorMessage}");
            }

            // Validate grand total calculation
            var calculatedTotal = orderCreateDto.Products.Sum(p => p.SubTotal);
            if (Math.Abs(calculatedTotal - orderCreateDto.GrandTotal) > 0.01m)
            {
                throw new ArgumentException($"Grand total mismatch. Expected: {calculatedTotal}, Received: {orderCreateDto.GrandTotal}");
            }
        }

        private async Task ValidateInventoryAvailability(IEnumerable<ProductForOrderDto> products)
        {
            _logger.LogDebug("Validating inventory availability for {ProductCount} products", products.Count());

            // Get all product IDs for batch query
            var productIds = products.Select(p => p.ProductId).Distinct().ToList();

            // Batch query all inventories
            var inventories = await _inventoryRepository.GetInventoriesByProductIds(productIds);
            var inventoryDict = inventories.ToDictionary(inv => inv.ProductId, inv => inv);

            var insufficientStockErrors = new List<string>();

            foreach (var product in products)
            {
                if (!inventoryDict.TryGetValue(product.ProductId, out var inventory))
                {
                    insufficientStockErrors.Add($"Product {product.ProductId} not found in inventory");
                    continue;
                }

                if (inventory.CurrentStock < product.Quantity)
                {
                    insufficientStockErrors.Add(
                        $"Insufficient stock for product {product.ProductId}. Available: {inventory.CurrentStock}, Required: {product.Quantity}");
                }
            }

            if (insufficientStockErrors.Any())
            {
                var errorMessage = string.Join("; ", insufficientStockErrors);
                throw new InvalidOperationException($"Inventory validation failed: {errorMessage}");
            }
        }

        private async Task<Customer?> ProcessCustomer(CustomerForOrderDto customerDto)
        {
            if (customerDto == null || string.IsNullOrWhiteSpace(customerDto.PhoneNumber))
            {
                _logger.LogDebug("No customer phone provided, treating as walk-in customer");
                return null;
            }

            var existingCustomer = await _customerRepository.GetUserByPhoneNumber(customerDto.PhoneNumber);

            if (existingCustomer != null)
            {
                _logger.LogDebug("Found existing customer: {CustomerId}", existingCustomer.CustomerId);

                // Update customer name if different
                if (!string.Equals(existingCustomer.FullName, customerDto.CustomerName, StringComparison.OrdinalIgnoreCase))
                {
                    existingCustomer.FullName = customerDto.CustomerName;
                    await _customerRepository.UpdateCustomer(existingCustomer);
                    _logger.LogDebug("Updated customer name for {CustomerId}", existingCustomer.CustomerId);
                }

                return existingCustomer;
            }

            // Create new customer
            var newCustomer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                FullName = customerDto.CustomerName.Trim(),
                PhoneNumber = customerDto.PhoneNumber.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _customerRepository.CreateCustomer(newCustomer);
            _logger.LogInformation("Created new customer: {CustomerId} - {CustomerName}",
                newCustomer.CustomerId, newCustomer.FullName);

            return newCustomer;
        }

        private async Task<SalesOrder> CreateSalesOrder(OrderCreateDto orderCreateDto, Guid? customerId)
        {
            var salesOrder = new SalesOrder
            {
                OrderId = Guid.NewGuid(),
                CustomerId = customerId,
                TotalAmount = orderCreateDto.GrandTotal,
                DiscountAmount = 0,
                FinalAmount = orderCreateDto.GrandTotal,
                PaymentMethod = ParsePaymentMethod(orderCreateDto.PaymentMethod),
                Status = SalesOrderStatusEnum.Completed,
                UserId = orderCreateDto.UserId,
                //Notes = orderCreateDto.Notes?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _saleOrderRepository.CreateSaleOrder(salesOrder);
            _logger.LogDebug("Created sales order: {OrderId}", salesOrder.OrderId);

            return salesOrder;
        }

        private async Task<List<SaleOrderItem>> CreateOrderItems(IEnumerable<ProductForOrderDto> products, Guid orderId)
        {
            var saleOrderItems = new List<SaleOrderItem>();

            foreach (var product in products)
            {
                var orderItem = new SaleOrderItem
                {
                    OrderId = orderId,
                    ProductId = product.ProductId,
                    BatchId = null,
                    Dosage = product.Dosage?.Trim(),
                    Quantity = product.Quantity,
                    UnitPrice = product.UnitPrice,
                    SubTotal = product.SubTotal,
                    CreatedAt = DateTime.UtcNow
                };

                saleOrderItems.Add(orderItem);
            }

            // Batch insert all items
            await _saleOrderItemRepository.CreateSaleOrderItems(saleOrderItems);
            _logger.LogDebug("Created {ItemCount} order items for order {OrderId}",
                saleOrderItems.Count, orderId);

            return saleOrderItems;
        }

        private async Task ProcessInventoryUpdates(IEnumerable<ProductForOrderDto> products, Guid orderId, Guid userId)
        {
            _logger.LogDebug("Processing inventory updates for {ProductCount} products", products.Count());

            // Group products by ProductId to handle multiple quantities for same product
            var productGroups = products
                .GroupBy(p => p.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(p => p.Quantity) })
                .ToList();

            var productIds = productGroups.Select(pg => pg.ProductId).ToList();

            // Batch query all inventories
            var inventories = await _inventoryRepository.GetInventoriesByProductIds(productIds);
            var inventoryDict = inventories.ToDictionary(inv => inv.ProductId, inv => inv);

            // Prepare batch updates
            var inventoriesToUpdate = new List<ProductInventory>();
            var transactionsToCreate = new List<InventoryTransaction>();

            foreach (var productGroup in productGroups)
            {
                if (!inventoryDict.TryGetValue(productGroup.ProductId, out var inventory))
                {
                    throw new InvalidOperationException($"Inventory not found for product {productGroup.ProductId}");
                }

                // Update inventory
                inventory.CurrentStock -= productGroup.TotalQuantity;
                inventory.LastUpdated = DateTime.UtcNow;
                inventory.UpdatedAt = DateTime.UtcNow;
                inventoriesToUpdate.Add(inventory);

                // Create transaction record
                var transaction = new InventoryTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    ProductId = productGroup.ProductId,
                    BatchId = null, // You might want to handle batch logic here
                    TransactionType = InventoryTransactionTypeEnum.Out,
                    QuantityChange = -productGroup.TotalQuantity,
                    CurrentStock = inventory.CurrentStock,
                    TransactionDate = DateTime.UtcNow,
                    Reason = "Sales transaction",
                    RelatedOrderId = orderId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                transactionsToCreate.Add(transaction);
            }

            // Batch update inventories
            await _inventoryRepository.UpdateInventoriesAsync(inventoriesToUpdate);

            // Batch create transactions
            await _inventoryRepository.CreateTransactionsAsync(transactionsToCreate);

            _logger.LogDebug("Updated {InventoryCount} inventories and created {TransactionCount} transactions",
                inventoriesToUpdate.Count, transactionsToCreate.Count);
        }

        private PaymentMethodEnum ParsePaymentMethod(string? paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
                return PaymentMethodEnum.Cash;

            return paymentMethod.ToUpperInvariant() switch
            {
                "CASH" => PaymentMethodEnum.Cash,
                //"CREDIT_CARD" => PaymentMethodEnum.CreditCard,
                "BANK_TRANSFER" => PaymentMethodEnum.BankTransfer,
                //"E_WALLET" => PaymentMethodEnum.EWallet,
                _ => PaymentMethodEnum.Cash
            };
        }

        private string GenerateInvoiceNumber(Guid orderId)
        {
            var today = DateTime.UtcNow;
            var orderIdShort = orderId.ToString("N")[..8].ToUpperInvariant();
            return $"INV{today:yyyyMMdd}-{orderIdShort}";
        }

        public async Task<SalesOrderDto?> GetSaleOrderById(Guid orderId)
        {
            try
            {
                _logger.LogDebug("Getting sale order by ID: {OrderId}", orderId);

                var saleOrder = await _saleOrderRepository.GetSaleOrderById(orderId);
                if (saleOrder == null)
                {
                    _logger.LogWarning("Sale order not found: {OrderId}", orderId);
                    return null;
                }

                var result = _mapper.Map<SalesOrderDto>(saleOrder);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sale order by ID: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<IEnumerable<SalesOrderDto>> GetSaleOrdersByDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Getting sale orders from {FromDate} to {ToDate}", fromDate, toDate);

                var saleOrders = await _saleOrderRepository.GetSaleOrdersByDateRange(fromDate, toDate);
                var result = _mapper.Map<List<SalesOrderDto>>(saleOrders);

                _logger.LogInformation("Retrieved {Count} sale orders for date range", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sale orders by date range");
                throw;
            }
        }

        public async Task<bool> CancelOrder(Guid orderId, string reason)
        {
            try
            {
                _logger.LogInformation("Cancelling order: {OrderId}, Reason: {Reason}", orderId, reason);

                using (var scope = TransactionScopeManager.CreateTransactionScope())
                {
                    var saleOrder = await _saleOrderRepository.GetSaleOrderById(orderId);
                    if (saleOrder == null)
                    {
                        _logger.LogWarning("Order not found for cancellation: {OrderId}", orderId);
                        return false;
                    }

                    if (saleOrder.Status == SalesOrderStatusEnum.Canceled)
                    {
                        _logger.LogWarning("Order already cancelled: {OrderId}", orderId);
                        return false;
                    }

                    // Update order status
                    saleOrder.Status = SalesOrderStatusEnum.Canceled;
                    saleOrder.UpdatedAt = DateTime.UtcNow;
                    await _saleOrderRepository.UpdateSaleOrder(saleOrder);

                    // Reverse inventory transactions
                    await ReverseInventoryTransactions(orderId, reason);

                    scope.Complete();

                    _logger.LogInformation("Order cancelled successfully: {OrderId}", orderId);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order: {OrderId}", orderId);
                throw;
            }
        }

        private async Task ReverseInventoryTransactions(Guid orderId, string reason)
        {
            // Get original transactions
            var originalTransactions = await _inventoryRepository.GetTransactionsByOrderId(orderId);

            var inventoriesToUpdate = new List<ProductInventory>();
            var reverseTransactions = new List<InventoryTransaction>();

            foreach (var originalTransaction in originalTransactions)
            {
                // Get current inventory
                var inventory = await _inventoryRepository.GetInventoryByProductAndBatchAsync(originalTransaction.ProductId);
                if (inventory != null)
                {
                    // Reverse the stock change
                    inventory.CurrentStock -= originalTransaction.QuantityChange; // Reverse the change
                    inventory.LastUpdated = DateTime.UtcNow;
                    inventory.UpdatedAt = DateTime.UtcNow;
                    inventoriesToUpdate.Add(inventory);

                    // Create reverse transaction
                    var reverseTransaction = new InventoryTransaction
                    {
                        TransactionId = Guid.NewGuid(),
                        ProductId = originalTransaction.ProductId,
                        BatchId = originalTransaction.BatchId,
                        TransactionType = InventoryTransactionTypeEnum.Adjustment,
                        QuantityChange = -originalTransaction.QuantityChange, // Opposite amount
                        CurrentStock = inventory.CurrentStock,
                        TransactionDate = DateTime.UtcNow,
                        Reason = $"Order cancellation: {reason}",
                        RelatedOrderId = orderId,
                        UserId = originalTransaction.UserId,
                        CreatedAt = DateTime.UtcNow
                    };

                    reverseTransactions.Add(reverseTransaction);
                }
            }

            // Batch update
            if (inventoriesToUpdate.Any())
            {
                await _inventoryRepository.UpdateInventoriesAsync(inventoriesToUpdate);
                await _inventoryRepository.CreateTransactionsAsync(reverseTransactions);
            }
        }
    }
}
