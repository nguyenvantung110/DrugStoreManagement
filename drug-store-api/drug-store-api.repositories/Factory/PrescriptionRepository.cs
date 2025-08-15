using drug_store_api.data;
using drug_store_api.entities.Categories;
using drug_store_api.entities.PrescriptionTemplates;
using drug_store_api.repositories.IF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.repositories.Factory
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly DrugStoreDbContext _context;
        public PrescriptionRepository(DrugStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PrescriptionTemplate?>> GetPrescriptionTemplate(Guid? categoryId)
        {
            var query = _context.PrescriptionTemplates
                .Include(t => t.Category)
                .Include(t => t.User)
                .Include(t => t.PrescriptionItems)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            var templates = await query.ToListAsync();

            return templates;
        }
    }
}
