using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.entities.PurchaseRequests
{
    public class PurchaseRequestByRequestDate
    {
        public Guid RequestId { get; set; }
        public string CreatedByName { get; set; }
        public DateTime RequestDate { get; set; }
        public PurchaseRequestStatusEnum Status { get; set; }
    }
}
