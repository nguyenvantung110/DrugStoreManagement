using drug_store_api.services.Factory;
using drug_store_api.services.IF;
using Microsoft.Extensions.DependencyInjection;

namespace drug_store_api.services
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<IPurchaseRequestService, PurchaseRequestService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISalesOrderService, SalesOrderService>();
            services.AddScoped<IInventoryService, InventoryService>();
            return services;
        }
    }
}
