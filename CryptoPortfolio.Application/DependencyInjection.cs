using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using CryptoPortfolio.Application.Portfolios.Services;
using CryptoPortfolio.Application.Common;

namespace CryptoPortfolio.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient<IValidationService, PortfolioValidationService>();

            return services;
        }
    }
}
