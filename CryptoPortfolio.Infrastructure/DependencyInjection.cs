using CryptoPortfolio.Application.Common.Identity.Interfaces;
using CryptoPortfolio.Application.Portfolios.Interfaces;
using CryptoPortfolio.Application.Users.Interfaces;
using CryptoPortfolio.Domain.Constants;
using CryptoPortfolio.Domain.Settings;
using CryptoPortfolio.Infrastructure.Auth;
using CryptoPortfolio.Infrastructure.Identity;
using CryptoPortfolio.Infrastructure.Repositories;
using Cryptouser.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CryptoPortfolio.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPortfolioRepository, PortfolioRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            
            MongoClassMap.Map();

            var tokenSettings = configuration.GetSection("TokenSettings").Get<TokenSettings>();
            

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = tokenSettings.Issuer,
                    ValidAudience = tokenSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SigningKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddSingleton<IAuthorizationHandler, DataOwnerAuthorizationHandler>();
            services.AddAuthorization(o =>
            {
                o.AddPolicy(Policies.OwnerOnly, policy =>
                    policy.Requirements.Add(new DataOwnerRequirement()));
            });

            services.AddTransient<ISecurityService, SecurityService>();

            return services;
        }
    }
}
