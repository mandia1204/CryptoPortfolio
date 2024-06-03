namespace CryptoPortfolio.Domain.Entities
{
    public class Portfolio
    {
        public string Id { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string Coin { get; set; } = null!;

        public double Quantity { get; set; }
    }
}
