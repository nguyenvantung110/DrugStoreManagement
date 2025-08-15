using drug_store_api.repositories.Factory;
using drug_store_api.repositories.IF;
using Microsoft.Extensions.DependencyInjection;

namespace drug_store_api.repositories
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPurchaseOrdersRepository, PurchaseOrdersRepository>();
            services.AddScoped<IPurchaseRequestRepository, PurchaseRequestRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            return services;
        }
    }
}
