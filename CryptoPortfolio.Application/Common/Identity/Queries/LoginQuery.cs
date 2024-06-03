
namespace CryptoPortfolio.Application.Common.Identity.Queries
{
    public record LoginQuery
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
