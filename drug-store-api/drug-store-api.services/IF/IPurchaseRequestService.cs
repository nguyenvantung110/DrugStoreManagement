using drug_store_api.dtos.PurchaseRequests;

namespace drug_store_api.services.IF
{
    public interface IPurchaseRequestService
    {
        Task<IEnumerable<PurchaseRequestDto>> GetByRequestDate(DateTime requestDate);
    }
}
