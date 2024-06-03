using CryptoPortfolio.Application.Portfolios.Commands;
using CryptoPortfolio.Application.Portfolios.Queries;
using CryptoPortfolio.Application.Portfolios.Services;

namespace CryptoPortfolio.Application.Portfolios.Interfaces
{
    public interface IPortfolioService
    {
        Task<IEnumerable<PortfolioDto>> GetPortfolios(GetPortfoliosQuery query);

        Task<PortfolioOperationResponseDto> CreatePortfolio(CreatePortfolioCommand command);

        Task<PortfolioOperationResponseDto> UpdatePortfolio(UpdatePortfolioCommand command);

        Task<PortfolioOperationResponseDto> DeletePortfolio(DeletePortfolioCommand command);
    }
}
