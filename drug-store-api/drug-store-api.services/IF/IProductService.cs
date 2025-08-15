using drug_store_api.dtos.Products;

namespace drug_store_api.services.IF
{
    public interface IProductService
    {
        Task<IEnumerable<ProductBasicInfoDto>> GetBasicProducInfoAsync();
    }
}
