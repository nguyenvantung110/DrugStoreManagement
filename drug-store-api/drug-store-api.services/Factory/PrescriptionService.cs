using AutoMapper;
using drug_store_api.dtos.PrescriptionTemplates;
using drug_store_api.dtos.Products;
using drug_store_api.entities.Categories;
using drug_store_api.entities.PrescriptionTemplates;
using drug_store_api.repositories.IF;
using drug_store_api.services.IF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.services.Factory
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _repository;
        private readonly IMapper _mapper;

        public PrescriptionService(IPrescriptionRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<PrescriptionTemplateDto?>> GetPrescriptionTemplate(Guid? categoryId)
        {
            var res = await _repository.GetPrescriptionTemplate(categoryId);
            List<PrescriptionTemplateDto> prescriptionList = _mapper.Map<List<PrescriptionTemplateDto>>(res);
            return prescriptionList;
        }
    }
}
