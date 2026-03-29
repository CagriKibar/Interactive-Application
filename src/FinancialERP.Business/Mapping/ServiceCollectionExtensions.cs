using FinancialERP.Business.Services.Abstract;
using FinancialERP.Business.Services.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialERP.Business.Mapping
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IMacroEconomyIntegrationService, MacroEconomyIntegrationService>();
            services.AddScoped<IFinancialAnalysisService, FinancialAnalysisAndRecommendationService>();
            return services;
        }
    }
}
