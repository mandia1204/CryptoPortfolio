using CryptoPortfolio.Domain.Entities;
using System.Linq.Expressions;

namespace CryptoPortfolio.Application.Users.Interfaces
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User> GetAsync(Expression<Func<User, bool>> filterExpression);
        Task RemoveAsync(string id);
        Task UpdateAsync(string id, User user);
    }
}
