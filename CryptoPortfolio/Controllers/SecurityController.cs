using CryptoPortfolio.Application.Common.Identity.Commands;
using CryptoPortfolio.Application.Common.Identity.Interfaces;
using CryptoPortfolio.Application.Common.Identity.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CryptoPortfolio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly ILogger<SecurityController> _logger;
        private readonly ISecurityService _securityService;

        public SecurityController(ILogger<SecurityController> logger, ISecurityService securityService)
        {
            _logger = logger;
            _securityService = securityService;
        }

        [HttpPost("Users")]
        public async Task<IActionResult> CreateUser(CreateUserCommand request)
        {
            var user = await _securityService.CreateUser(request);
            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginQuery request)
        {
            var result = await _securityService.Login(request);

            if (!result.Sucess) {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
    }
}
