using drug_store_api.data;
using drug_store_api.entities.PurchaseRequests;
using drug_store_api.repositories.IF;
using Microsoft.EntityFrameworkCore;

namespace drug_store_api.repositories.Factory
{
    public class PurchaseRequestRepository : IPurchaseRequestRepository
    {
        private readonly DrugStoreDbContext _context;
        public PurchaseRequestRepository(DrugStoreDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<PurchaseRequestByRequestDate>> GetByRequestDate(DateTime requestDate)
        {
            try
            {
                var start = DateTime.SpecifyKind(requestDate, DateTimeKind.Unspecified).Date;
                var end = start.AddDays(1);

                var query =
                    from pr in _context.PurchaseRequests
                    join u in _context.Users on pr.CreatedBy equals u.UserId
                    join s in _context.Suppliers on pr.SupplierId equals s.Supplier_Id into gj
                    from sup in gj.DefaultIfEmpty()
                    where pr.RequestDate >= start && pr.RequestDate < end
                    select new PurchaseRequestByRequestDate
                    {
                        RequestId = pr.RequestId,
                        CreatedByName = u.FullName,
                        RequestDate = pr.RequestDate,
                        Status = pr.Status
                    };

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
