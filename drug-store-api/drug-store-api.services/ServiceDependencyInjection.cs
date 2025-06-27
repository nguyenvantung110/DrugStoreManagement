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

            return services;
        }
    }
}
