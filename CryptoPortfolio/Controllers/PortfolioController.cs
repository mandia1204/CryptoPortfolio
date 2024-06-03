using CryptoPortfolio.Application.Common.Identity.Interfaces;
using CryptoPortfolio.Application.Portfolios.Commands;
using CryptoPortfolio.Application.Portfolios.Interfaces;
using CryptoPortfolio.Application.Portfolios.Queries;
using CryptoPortfolio.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoPortfolio.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly ILogger<PortfolioController> _logger;
        private readonly IPortfolioService _service;
        private readonly ISecurityService _securityService;
        
        public PortfolioController(ILogger<PortfolioController> logger, IPortfolioService service, ISecurityService securityService)
        {
            _logger = logger;
            _service = service;
            _securityService = securityService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioDto>>> GetPortfolios([FromQuery] GetPortfoliosQuery query)
        {
            var success = await _securityService.AuthorizeResource(User, query, Policies.OwnerOnly);

            if (!success)
            {
                return StatusCode(403, Messages.ForbiddenError);
            }

            var portfolios = await _service.GetPortfolios(query);
            return Ok(portfolios);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePortfolio(CreatePortfolioCommand request)
        {
            var success = await _securityService.AuthorizeResource(User, request, Policies.OwnerOnly);

            if (!success)
            {
                return StatusCode(403, Messages.ForbiddenError);
            }

            var result = await _service.CreatePortfolio(request);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePortfolio([FromRoute] string id, [FromBody] UpdatePortfolioCommand request)
        {
            var success = await _securityService.AuthorizeResource(User, request, Policies.OwnerOnly);

            if (!success) {
                return StatusCode(403, Messages.ForbiddenError);
            }

            var result = await _service.UpdatePortfolio(request);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio([FromRoute] string id, [FromBody] DeletePortfolioCommand request)
        {
            var success = await _securityService.AuthorizeResource(User, request, Policies.OwnerOnly);

            if (!success)
            {
                return StatusCode(403, Messages.ForbiddenError);
            }

            await _service.DeletePortfolio(request);

            return Ok();
        }
    }
}
