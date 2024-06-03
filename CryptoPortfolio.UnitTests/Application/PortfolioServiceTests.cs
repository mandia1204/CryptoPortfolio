using CryptoPortfolio.Application.Common;
using CryptoPortfolio.Application.Common.Exceptions;
using CryptoPortfolio.Application.Portfolios.Commands;
using CryptoPortfolio.Application.Portfolios.Interfaces;
using CryptoPortfolio.Application.Portfolios.Services;
using CryptoPortfolio.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace CryptoPortfolio.UnitTests.Application
{
    public class PortfolioServiceTests
    {
        private IPortfolioService _service;
        private IValidationService _validationService;
        private Mock<IPortfolioRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IPortfolioRepository>();
            _validationService = new PortfolioValidationService(
                new UpdatePortfolioCommandValidator(), 
                new CreatePortfolioCommandValidator(), 
                new DeletePortfolioCommandValidator());

            _service = new PortfolioService(_repositoryMock.Object, _validationService);
        }

        [Test]
        public void CreatePortfolio_ShouldRequireMinimumQuantity()
        {
            var command = new CreatePortfolioCommand
            {
                Coin = "ETH",
                Quantity = -1,
                UserId = "665baf2db260a0f03985df8a"
            };

            var expectedErrors = new Dictionary<string, string[]>
            {
                { "Quantity", new string[]{ "'Quantity' must be greater than or equal to '0'." } }
            };

            Assert.That(async () => await _service.CreatePortfolio(command), 
                Throws.TypeOf<ValidationException>().With.Property("Errors").EqualTo(expectedErrors));
        }

        [Test]
        public void CreatePortfolio_ShouldValidateCoinMaxLength()
        {
            var command = new CreatePortfolioCommand
            {
                Coin = "ETHFGF",
                Quantity = 20,
                UserId = "665baf2db260a0f03985df8a"
            };

            var expectedErrors = new Dictionary<string, string[]>
            {
                { "Coin", new string[]{ "The length of 'Coin' must be 5 characters or fewer. You entered 6 characters." } }
            };

            Assert.That(async () => await _service.CreatePortfolio(command),
                Throws.TypeOf<ValidationException>().With.Property("Errors").EqualTo(expectedErrors));
        }

        [Test]
        public async Task CreatePortfolio_ShouldCreatePortfolio()
        {
            _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<Portfolio>()))
                .Callback<Portfolio>(c =>
                {
                    c.Id = "675baf2db260a0f03985df8b";
                });

            var command = new CreatePortfolioCommand
            {
                Coin = "ETH",
                Quantity = 10,
                UserId = "665baf2db260a0f03985df8a"
            };

            var result = await _service.CreatePortfolio(command);

            Assert.That(result.Id, Is.EqualTo("675baf2db260a0f03985df8b"));
            Assert.That(result.Coin, Is.EqualTo("ETH"));
            Assert.That(result.Quantity, Is.EqualTo(10));
          
            _repositoryMock.Verify(x => x.CreateAsync(It.Is<Portfolio>(p => p.Quantity == 10)), Times.Once());
            _repositoryMock.Verify(x => x.CreateAsync(It.Is<Portfolio>(p => p.Coin == "ETH")), Times.Once());
        }

        [Test]
        public async Task CreatePortfolio_ShouldIncrementQuantityIfExisting()
        {
            var existing = new Portfolio
            {
                Id = "675baf2db260a0f03985df8b",
                Coin = "ETH",
                Quantity = 10,
                UserId = "665baf2db260a0f03985df8a"
            };
            _repositoryMock.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Portfolio, bool>>>()))
                .ReturnsAsync(existing);

            var command = new CreatePortfolioCommand
            {
                Coin = "ETH",
                Quantity = 10,
                UserId = "665baf2db260a0f03985df8a"
            };

            var result = await _service.CreatePortfolio(command);
            
            Assert.That(result.Quantity, Is.EqualTo(20));

            _repositoryMock.Verify(x => x.UpdateAsync(existing.Id, existing), Times.Once());
        }

        [Test]
        public void UpdatePortfolio_ShouldValidateNotFound()
        {
            var command = new UpdatePortfolioCommand
            {
                Quantity = 10,
                UserId = "665baf2db260a0f03985df8a"
            };

            Assert.That(async () => await _service.UpdatePortfolio(command),
                 Throws.TypeOf<NotFoundException>().With.Property("Message").EqualTo("Resource not found"));
        }

        [Test]
        public async Task UpdatePortfolio_ShouldUpdatePortfolio()
        {
            var existing = new Portfolio
            {
                Id = "655baf2db260a0f03985df8a",
                Coin = "ETH",
                Quantity = 10,
                UserId = "665baf2db260a0f03985df8a"
            };
            _repositoryMock.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Portfolio, bool>>>()))
                .ReturnsAsync(existing);

            var command = new UpdatePortfolioCommand
            {
                Id = "655baf2db260a0f03985df8a",
                Quantity = 20,
                UserId = "665baf2db260a0f03985df8a"
            };

            var result = await _service.UpdatePortfolio(command);

            Assert.That(result.Id, Is.EqualTo("655baf2db260a0f03985df8a"));
            Assert.That(result.Quantity, Is.EqualTo(20));

            _repositoryMock.Verify(x => x.UpdateAsync(command.Id, It.Is<Portfolio>(c => c.Quantity == 20)), Times.Once());
        }
    }
}
