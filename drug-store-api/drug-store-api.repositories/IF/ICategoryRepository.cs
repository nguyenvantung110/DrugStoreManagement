using drug_store_api.entities.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.repositories.IF
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategory();
        Task<IEnumerable<Category>> GetCategoryByType(string categoryType);
    }
}
