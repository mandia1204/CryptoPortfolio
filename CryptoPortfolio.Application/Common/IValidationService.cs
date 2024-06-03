using FluentValidation.Results;

namespace CryptoPortfolio.Application.Common
{
    public interface IValidationService
    {
        ValidationResult Validate<T>(T entity);
    }
}