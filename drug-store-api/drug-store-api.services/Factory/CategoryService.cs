using AutoMapper;
using drug_store_api.dtos.Categories;
using drug_store_api.dtos.PrescriptionTemplates;
using drug_store_api.entities.Categories;
using drug_store_api.repositories.IF;
using drug_store_api.services.IF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.services.Factory
{
    public class CategoryService : ICategoryService
    {

        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategory()
        {
            var res = await _repository.GetAllCategory();
            List<CategoryDto> prescriptionList = _mapper.Map<List<CategoryDto>>(res);
            return prescriptionList;
        }

        public async Task<IEnumerable<CategoryByTypeDto>> GetCategoryByType(string categoryType)
        {
            var res = await _repository.GetCategoryByType(categoryType);
            List<CategoryByTypeDto> prescriptionList = _mapper.Map<List<CategoryByTypeDto>>(res);
            return prescriptionList;
        }
    }
}
