namespace CryptoPortfolio.Domain.Settings
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string PortfoliosCollectionName { get; set; } = null!;
        public string UsersCollectionName { get; set; } = null!;
    }
}
