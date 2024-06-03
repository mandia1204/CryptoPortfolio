using CryptoPortfolio.Application.Common.Identity.Commands;
using CryptoPortfolio.Application.Common.Identity.Interfaces;
using CryptoPortfolio.Application.Common.Identity.Queries;
using CryptoPortfolio.Application.Users.Interfaces;
using CryptoPortfolio.Domain.Entities;
using CryptoPortfolio.Domain.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CryptoPortfolio.Infrastructure.Identity
{
    public class SecurityService : ISecurityService
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenSettings _tokenSettings;
        private IAuthorizationService _authService;

        public SecurityService(IUserRepository userRepository, IOptions<TokenSettings> tokenSettings, IAuthorizationService authService)
        {
            _userRepository = userRepository;
            _tokenSettings = tokenSettings.Value;
            _authService = authService;
        }

        public async Task<bool> AuthorizeResource(ClaimsPrincipal user, object resource, string policyName)
        {
            var res = await _authService.AuthorizeAsync(user, resource, policyName);
            return res.Succeeded;
        }

        public async Task<UserDto> CreateUser(CreateUserCommand command)
        {
            var user = new User
            {
                Password = command.Password,
                UserName = command.UserName,
            };

            await _userRepository.CreateAsync(user);

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName
            };
        }

        public async Task<LoginResponseDto> Login(LoginQuery request)
        {
            var user = await _userRepository.GetAsync(u => u.UserName == request.UserName && u.Password == request.Password);

            if (user == null) {
                return new LoginResponseDto
                {
                    Sucess = false,
                    Message = "User or password incorrect",
                };
            }

            var signingKey = Encoding.ASCII.GetBytes(_tokenSettings.SigningKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("user_id", user.Id),
                ]),
                Expires = DateTime.UtcNow.AddMinutes(_tokenSettings.ExpiresInMinutes),
                Issuer = _tokenSettings.Issuer,
                Audience = _tokenSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return new LoginResponseDto
            {
                Sucess = true,
                Token = token,
            };
        }
    }
}
