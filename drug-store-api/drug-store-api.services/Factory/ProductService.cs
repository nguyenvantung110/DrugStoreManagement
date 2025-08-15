using AutoMapper;
using drug_store_api.dtos.Products;
using drug_store_api.repositories.IF;
using drug_store_api.services.IF;

namespace drug_store_api.services.Factory
{
    internal class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ProductBasicInfoDto>> GetBasicProducInfoAsync()
        {
            var res = await _repository.GetProductBasicInfoAsync();
            List<ProductBasicInfoDto> productList = _mapper.Map<List<ProductBasicInfoDto>>(res);
            return productList;
        }
    }
}
