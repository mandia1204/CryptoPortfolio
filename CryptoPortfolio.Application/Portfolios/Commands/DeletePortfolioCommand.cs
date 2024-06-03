using CryptoPortfolio.Application.Common.Identity.Interfaces;
using FluentValidation;

namespace CryptoPortfolio.Application.Portfolios.Commands
{
    public record DeletePortfolioCommand : IResource
    {
        public string Id { get; init; } = null!;

        public string UserId { get; init; } = null!;
    }

    public class DeletePortfolioCommandValidator : AbstractValidator<DeletePortfolioCommand>
    {
        public DeletePortfolioCommandValidator()
        {
            RuleFor(v => v.UserId)
                .Length(24);
        }
    }
}