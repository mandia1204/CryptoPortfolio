using CryptoPortfolio.Application.Portfolios.Queries;
using CryptoPortfolio.Application.Portfolios.Interfaces;
using CryptoPortfolio.Application.Portfolios.Commands;
using CryptoPortfolio.Domain.Entities;
using ValidationException = CryptoPortfolio.Application.Common.Exceptions.ValidationException;
using CryptoPortfolio.Application.Common;
using CryptoPortfolio.Application.Common.Exceptions;
using CryptoPortfolio.Domain.Constants;

namespace CryptoPortfolio.Application.Portfolios.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _repository;
        private readonly IValidationService _validationService;

        public PortfolioService(IPortfolioRepository repository, IValidationService validationService)
        {
            _repository = repository;
            _validationService = validationService;
        }

        public async Task<IEnumerable<PortfolioDto>> GetPortfolios(GetPortfoliosQuery query)
        {
            var list = await _repository.GetAsync(p => p.UserId == query.UserId);

            return list.Select(l => new PortfolioDto { 
                Id = l.Id ,
                Quantity = l.Quantity,
                Coin = l.Coin,
            });
        }

        public async Task<PortfolioOperationResponseDto> CreatePortfolio(CreatePortfolioCommand command)
        {
            ValidateCommand(command);

            var portfolio = new Portfolio
            {
                Quantity = command.Quantity ,
                UserId  = command.UserId ,
                Coin = command.Coin
            };

            var existing = await _repository.FindOneAsync(p => p.UserId == portfolio.UserId && p.Coin == portfolio.Coin);

            if (existing != null)
            {
                existing.Quantity += portfolio.Quantity;
                await _repository.UpdateAsync(existing.Id, existing);
                return new PortfolioOperationResponseDto
                {
                    Id = existing.Id,
                    Message = Messages.UpdateSuccess,
                    Coin = portfolio.Coin,
                    Quantity = existing.Quantity
                };
            }

            await _repository.CreateAsync(portfolio);

            return new PortfolioOperationResponseDto
            {
                Id = portfolio.Id ,
                Message = Messages.CreateSuccess,
                Coin = portfolio.Coin,
                Quantity = command.Quantity
            };
        }

        public async Task<PortfolioOperationResponseDto> UpdatePortfolio(UpdatePortfolioCommand command)
        {
            ValidateCommand(command);
            
            var item = await _repository.FindOneAsync(p => p.Id == command.Id && p.UserId == command.UserId);

            if (item is null)
            {
                throw new NotFoundException();
            }

            var portfolio = new Portfolio
            {
                Quantity = command.Quantity,
                Id = command.Id ,
                UserId= command.UserId ,
            };

            await _repository.UpdateAsync(command.Id, portfolio);

            return new PortfolioOperationResponseDto
            {
                Id = portfolio.Id,
                Message = Messages.UpdateSuccess,
                Coin = portfolio.Coin,
                Quantity = command.Quantity
            };
        }

        public async Task<PortfolioOperationResponseDto> DeletePortfolio(DeletePortfolioCommand command)
        {
            ValidateCommand(command);

            var item = await _repository.FindOneAsync(p => p.Id == command.Id && p.UserId == command.UserId);

            if (item is null)
            {
                throw new NotFoundException();
            }

            await _repository.RemoveAsync(command.Id);

            return new PortfolioOperationResponseDto
            {
                Id = command.Id,
                Message = Messages.DeleteSuccess,
                Coin = item.Coin,
                Quantity = 0
            };
        }

        private void ValidateCommand<T>(T command)
        {
            var result = _validationService.Validate(command);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
