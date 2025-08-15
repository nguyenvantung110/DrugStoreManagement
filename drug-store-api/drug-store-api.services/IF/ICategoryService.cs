using drug_store_api.dtos.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.services.IF
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategory();
        Task<IEnumerable<CategoryByTypeDto>> GetCategoryByType(string categoryType);
    }
}
