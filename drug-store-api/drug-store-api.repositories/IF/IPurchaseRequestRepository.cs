using drug_store_api.entities.PurchaseRequests;

namespace drug_store_api.repositories.IF
{
    public interface IPurchaseRequestRepository
    {
        Task<IEnumerable<PurchaseRequestByRequestDate>> GetByRequestDate(DateTime requestDate);
    }
}