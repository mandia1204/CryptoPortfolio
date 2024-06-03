using CryptoPortfolio.Application.Common;

namespace CryptoPortfolio.Application.Portfolios.Services
{
    public class PortfolioOperationResponseDto: ApiResponse
    {
        public string Coin { get; set; } = null!;

        public double Quantity { get; set; }

    }
}
