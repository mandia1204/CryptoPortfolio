using CryptoPortfolio.Application.Common.Identity.Interfaces;

namespace CryptoPortfolio.Application.Portfolios.Queries
{
    public record GetPortfoliosQuery: IResource
    {
        public string UserId { get; init; } = null!;
    }
}
