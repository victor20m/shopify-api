using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Context;
using Application.Interfaces;
using Infrastructure.Repository;
using Application.Implementations;

namespace ConfigurationFactory
{
    public class ServiceFactory
    {
        private readonly IServiceCollection Services;
        private readonly IConfiguration Configuration;
        public ServiceFactory(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;
            Configuration = configuration;
        }

        public void ConfigureServices()
        {
            Services.AddScoped<IOrderLocalService, OrderLocalService>();
            Services.AddScoped<ICustomerLocalService, CustomerLocalService>();
            Services.AddScoped<ICustomerRepository, CustomerRepository>();
            Services.AddScoped<IShopifyRepository, ShopifyRepository>();
            Services.AddSingleton<CustomerRepository>();
        }

        public void ConfigureDbContext()
        {
            Services.Configure<ShopifyDbContext>(Configuration.GetSection("ShopifyDbContext"));
        }
    }
}