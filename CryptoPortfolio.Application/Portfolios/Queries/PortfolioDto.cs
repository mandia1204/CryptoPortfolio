namespace CryptoPortfolio.Application.Portfolios.Queries
{
    public class PortfolioDto
    {
        public string Id { get; set; } = null!;

        public string Coin { get; set; } = null!;

        public double Quantity { get; set; }
    }
}
