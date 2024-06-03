using Microsoft.AspNetCore.Authorization;
using CryptoPortfolio.Domain.Constants;
using CryptoPortfolio.Application.Common.Identity.Interfaces;

namespace CryptoPortfolio.Infrastructure.Auth
{
    public class DataOwnerAuthorizationHandler: AuthorizationHandler<DataOwnerRequirement, IResource>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DataOwnerRequirement requirement, IResource resource)
        {
            var userIdClaim = context.User.FindFirst(c => c.Type == Claims.UserId);

            if (userIdClaim is null)
            {
                return Task.CompletedTask;
            }

            if (resource.UserId == userIdClaim.Value)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
