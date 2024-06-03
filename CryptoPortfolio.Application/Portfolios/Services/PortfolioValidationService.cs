using CryptoPortfolio.Application.Common;
using CryptoPortfolio.Application.Portfolios.Commands;
using FluentValidation;
using FluentValidation.Results;

namespace CryptoPortfolio.Application.Portfolios.Services
{
    public class PortfolioValidationService : IValidationService
    {
        private readonly IValidator<UpdatePortfolioCommand> _updatePortfolioCommandValidator;
        private readonly IValidator<CreatePortfolioCommand> _createPortfolioCommandValidator;
        private readonly IValidator<DeletePortfolioCommand> _deletePortfolioCommandValidator;

        public PortfolioValidationService(
            IValidator<UpdatePortfolioCommand> updatePortfolioCommandValidator,
            IValidator<CreatePortfolioCommand> createPortfolioCommandValidator,
            IValidator<DeletePortfolioCommand> deletePortfolioCommandValidator
            )
        {
            _updatePortfolioCommandValidator = updatePortfolioCommandValidator;
            _createPortfolioCommandValidator = createPortfolioCommandValidator;
            _deletePortfolioCommandValidator = deletePortfolioCommandValidator;
        }

        public ValidationResult Validate<T>(T entity)
        {
            if (entity is UpdatePortfolioCommand)
            {
                return _updatePortfolioCommandValidator.Validate(entity as UpdatePortfolioCommand);
            }
            else if (entity is CreatePortfolioCommand) {
                return _createPortfolioCommandValidator.Validate(entity as CreatePortfolioCommand);
            } 
            else if (entity is DeletePortfolioCommand)
            {
                return _deletePortfolioCommandValidator.Validate(entity as DeletePortfolioCommand);
            }

            return null;
        }
    }
}
