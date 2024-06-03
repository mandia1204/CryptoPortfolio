using CryptoPortfolio.Application.Portfolios.Interfaces;
using CryptoPortfolio.Domain.Entities;
using CryptoPortfolio.Domain.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CryptoPortfolio.Infrastructure.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly IMongoCollection<Portfolio> _portfoliosCollection;

        public PortfolioRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            _portfoliosCollection = mongoDatabase.GetCollection<Portfolio>(databaseSettings.Value.PortfoliosCollectionName);
        }

        public async Task<List<Portfolio>> GetAsync(Expression<Func<Portfolio, bool>> filterExpression) => 
            await _portfoliosCollection.Find(filterExpression).ToListAsync();

        public async Task<Portfolio> FindOneAsync(Expression<Func<Portfolio, bool>> filterExpression)
        {
            return (await this.GetAsync(filterExpression)).FirstOrDefault();
        }

        public async Task CreateAsync(Portfolio portfolio) =>
            await _portfoliosCollection.InsertOneAsync(portfolio);

        public async Task UpdateAsync(string id, Portfolio portfolio)
        {
            var filter = Builders<Portfolio>.Filter.Eq("_id", ObjectId.Parse(portfolio.Id));
            var update = Builders<Portfolio>.Update.Set("Quantity", portfolio.Quantity);

            await _portfoliosCollection.FindOneAndUpdateAsync(filter, update);
        }

        public async Task RemoveAsync(string id) =>
            await _portfoliosCollection.DeleteOneAsync(x => x.Id == id);
    }
}
