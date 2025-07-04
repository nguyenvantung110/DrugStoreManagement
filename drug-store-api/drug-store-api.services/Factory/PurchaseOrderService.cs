using AutoMapper;
using drug_store_api.dtos.PurchaseOrders;
using drug_store_api.entities.PurchaseOrderItems;
using drug_store_api.entities.PurchaseOrders;
using drug_store_api.entities.Users;
using drug_store_api.repositories.IF;
using drug_store_api.services.IF;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace drug_store_api.services.Factory
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPurchaseOrdersRepository _repository;
        private readonly IMapper _mapper;
        public PurchaseOrderService(IPurchaseOrdersRepository repository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PurchaseOrderDetailDto?> GetDetailsAsync(Guid purchaseId)
        {
            var data = await _repository.GetDetailsAsync(purchaseId);
            if (data == null) return null;
            var (entity, items) = data.Value;

            var result = _mapper.Map<PurchaseOrderDetailDto>(entity);
            result.Items = new List<PurchaseOrderItemDetailDto>();
            foreach (var item in items)
            {
                var itemDto = _mapper.Map<PurchaseOrderItemDetailDto>(item);
                var product = await _repository.GetProductAsync(item.ProductId);
                itemDto.Product = product != null ? _mapper.Map<ProductDto>(product) : null;
                result.Items.Add(itemDto);
            }
            return result;
        }

        public async Task CreateAsync(PurchaseOrderCreateDto dto)
        {
            try
            {
                var order = _mapper.Map<PurchaseOrder>(dto);
                order.PurchaseId = Guid.NewGuid();
                order.OrderDate = DateTime.UtcNow;
                order.CreatedAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;
                order.Status = PurchaseOrderStatusEnum.Pending;
                order.UserId = GetUserId();

                var items = dto.Items.Select(dtoItem =>
                {
                    var entity = _mapper.Map<PurchaseOrderItem>(dtoItem);
                    entity.PurchaseItemId = Guid.NewGuid();
                    entity.PurchaseId = order.PurchaseId;
                    entity.SubTotal = dtoItem.UnitCost * dtoItem.QuantityOrdered;
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.UpdatedAt = DateTime.UtcNow;
                    return entity;
                }).ToList();
                order.TotalAmount = items.Sum(i => i.SubTotal);

                await _repository.AddAsync(order, items);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task UpdateAsync(Guid id, PurchaseOrderUpdateDto dto)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) throw new Exception("Not found");

            order.ExpectedDeliveryDate = dto.ExpectedDeliveryDate ?? order.ExpectedDeliveryDate;
            order.ActualDeliveryDate = dto.ActualDeliveryDate ?? order.ActualDeliveryDate;
            order.Status = dto.Status ?? order.Status;
            order.UpdatedAt = DateTime.UtcNow;

            var items = dto.Items.Select(dtoItem =>
            {
                var entity = _mapper.Map<PurchaseOrderItem>(dtoItem);
                entity.PurchaseItemId = Guid.NewGuid();
                entity.PurchaseId = order.PurchaseId;
                entity.SubTotal = dtoItem.UnitCost * dtoItem.QuantityOrdered;
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                return entity;
            }).ToList();
            order.TotalAmount = items.Sum(i => i.SubTotal);

            await _repository.UpdateAsync(order, items);
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) throw new Exception("Not found");
            await _repository.DeleteAsync(order);
        }

        private Guid GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var parsedGuid) ? parsedGuid : Guid.Empty;
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrder()
        {
            var purchaseOrders = await _repository.GetAllPurchaseOrder();

            var res = _mapper.Map<List<PurchaseOrderDto>>(purchaseOrders);
            return res;
        }
    }
}
