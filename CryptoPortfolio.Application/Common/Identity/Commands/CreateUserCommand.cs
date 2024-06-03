namespace CryptoPortfolio.Application.Common.Identity.Commands
{
    public record CreateUserCommand
    {
        public string UserName { get; init; } = null!;

        public string Password { get; init; } = null!;
    }
}