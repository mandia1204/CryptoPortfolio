using CryptoPortfolio.Application.Common.Identity.Interfaces;
using FluentValidation;

namespace CryptoPortfolio.Application.Portfolios.Commands
{
    public record CreatePortfolioCommand: IResource
    {
        public string UserId { get; init; } = null!;
        
        public string Coin { get; init; } = null!;

        public double Quantity { get; init; }
    }

    public class CreatePortfolioCommandValidator : AbstractValidator<CreatePortfolioCommand>
    {
        public CreatePortfolioCommandValidator()
        {
            RuleFor(v => v.UserId)
                .Length(24);

            RuleFor(v => v.Coin)
                .MaximumLength(5)
                .NotEmpty();

            RuleFor(v => v.Quantity)
                .GreaterThanOrEqualTo(0);
        }
    }
}