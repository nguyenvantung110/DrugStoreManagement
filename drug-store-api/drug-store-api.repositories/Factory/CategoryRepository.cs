using drug_store_api.data;
using drug_store_api.entities.Categories;
using drug_store_api.repositories.IF;
using Microsoft.EntityFrameworkCore;

namespace drug_store_api.repositories.Factory
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DrugStoreDbContext _context;
        public CategoryRepository(DrugStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategory()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoryByType(string categoryType)
        {
            return await _context.Categories.Where(x => x.CategoryType == categoryType).ToListAsync();
        }
    }
}
