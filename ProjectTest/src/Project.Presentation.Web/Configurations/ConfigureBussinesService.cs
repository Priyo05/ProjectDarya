using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Project.Business.Interfaces;
using Project.Business.Repositories;

namespace Project.Presentation.Web.Configurations
{
    public static class ConfigureBussinesService
    {
        public static IServiceCollection AddBussinesServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestBarangRepository, RequestBarangRepository>();
            services.AddSingleton<ICompositeViewEngine, CompositeViewEngine>();

            return services;
        }
    }
}
