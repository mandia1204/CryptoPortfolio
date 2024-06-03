using CryptoPortfolio.Domain.Entities;
using System.Linq.Expressions;

namespace CryptoPortfolio.Application.Portfolios.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<List<Portfolio>> GetAsync(Expression<Func<Portfolio, bool>> filterExpression);

        Task<Portfolio> FindOneAsync(Expression<Func<Portfolio, bool>> filterExpression);

        Task CreateAsync(Portfolio portfolio);

        Task UpdateAsync(string id, Portfolio portfolio);

        Task RemoveAsync(string id);
    }
}