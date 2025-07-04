using AutoMapper;
using drug_store_api.dtos.PurchaseRequests;
using drug_store_api.repositories.IF;
using drug_store_api.services.IF;
using Microsoft.AspNetCore.Http;

namespace drug_store_api.services.Factory
{
    public class PurchaseRequestService : IPurchaseRequestService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPurchaseRequestRepository _repository;
        private readonly IMapper _mapper;
        public PurchaseRequestService(IPurchaseRequestRepository repository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<PurchaseRequestDto>> GetByRequestDate(DateTime requestDate)
        {
            var purchaseRequests = await _repository.GetByRequestDate(requestDate);
            var res = _mapper.Map<List<PurchaseRequestDto>>(purchaseRequests);
            return res;
        }
    }
}
