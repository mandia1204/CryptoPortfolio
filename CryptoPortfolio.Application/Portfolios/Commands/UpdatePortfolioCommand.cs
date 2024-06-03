using CryptoPortfolio.Application.Common.Identity.Interfaces;
using FluentValidation;

namespace CryptoPortfolio.Application.Portfolios.Commands
{
    public class UpdatePortfolioCommand: IResource
    {
        public string Id { get; init; } = null!;

        public string UserId { get; init; } = null!;

        public double Quantity { get; init; }
    }

    public class UpdatePortfolioCommandValidator : AbstractValidator<UpdatePortfolioCommand>
    {
        public UpdatePortfolioCommandValidator()
        {
            RuleFor(v => v.Id)
                .Length(24);

            RuleFor(v => v.UserId)
                .Length(24);

            RuleFor(v => v.Quantity)
                .GreaterThanOrEqualTo(0);
        }
    }
}