using drug_store_api.entities.Products;

namespace drug_store_api.repositories.IF
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductBasicInfo?>> GetProductBasicInfoAsync();
    }
}
