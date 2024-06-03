using CryptoPortfolio.Application.Common.Identity.Commands;
using CryptoPortfolio.Application.Common.Identity.Queries;
using System.Security.Claims;

namespace CryptoPortfolio.Application.Common.Identity.Interfaces
{
    public interface ISecurityService
    {
        Task<UserDto> CreateUser(CreateUserCommand command);

        Task<LoginResponseDto> Login(LoginQuery request);

        Task<bool> AuthorizeResource(ClaimsPrincipal user, object resource, string policyName);
    }
}
