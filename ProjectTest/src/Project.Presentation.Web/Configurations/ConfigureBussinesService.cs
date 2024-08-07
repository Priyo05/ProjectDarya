using Project.Business.Interfaces;
using Project.Business.Repositories;

namespace Project.Presentation.Web.Configurations
{
    public static class ConfigureBussinesService
    {
        public static IServiceCollection AddBussinesServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestBarangRepository, RequestBarangRepository>();

            return services;
        }
    }
}
