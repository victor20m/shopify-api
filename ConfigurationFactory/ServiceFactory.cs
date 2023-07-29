using Microsoft.Extensions.DependencyInjection;


namespace ConfigurationFactory
{
    public class ServiceFactory
    {
        private readonly IServiceCollection _services;

        public ServiceFactory(IServiceCollection services)
        {
            _services = services;
        }

        public void ConfigureServices()
        {
            
        }
    }
}