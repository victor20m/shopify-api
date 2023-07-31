using ConfigurationFactory;
using ShopifyApp.Filters;
using ShopifySharp.Enums;
using ShopifySharp;

namespace ShopifyApp.Startup
{
    public static class Setup
    {
        public static void SetupServices(IServiceCollection services, IConfiguration configuration)
        {
            ServiceFactory serviceFactory = new(services, configuration);
            serviceFactory.ConfigureServices();
            serviceFactory.ConfigureDbContext();
            services.AddScoped<WebhookAuthFilter>();
            services.AddRazorPages();
            services.AddSwaggerGen();
            services.AddControllers(options => options.Filters.Add<ExceptionFilter>())
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        }
    }
}
