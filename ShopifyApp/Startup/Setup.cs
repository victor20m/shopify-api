using ConfigurationFactory;
using ShopifyApp.Filters;

namespace ShopifyApp.Startup
{
    public static class Setup
    {
        public static IServiceCollection SetupServices(IServiceCollection services)
        {
            ServiceFactory serviceFactory = new ServiceFactory(services);
            serviceFactory.ConfigureServices();
            services.AddRazorPages();
            services.AddControllers();
            services.AddScoped<WebhookAuthFilter>();
            services.AddSwaggerGen();
            return services;
        }
    }
}
