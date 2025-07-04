using drug_store_api.entities.PurchaseRequests;

namespace drug_store_api.dtos.PurchaseRequests
{
    public class PurchaseRequestDto
    {
        public Guid RequestId { get; set; }
        public string CreatedByName { get; set; }
        public DateTime RequestDate { get; set; }
        public PurchaseRequestStatusEnum Status { get; set; }
    }
}
