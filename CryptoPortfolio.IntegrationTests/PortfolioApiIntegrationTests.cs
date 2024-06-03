using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CryptoPortfolio.Application.Common.Identity.Commands;
using CryptoPortfolio.Application.Common.Identity.Queries;
using CryptoPortfolio.Application.Portfolios.Commands;
using CryptoPortfolio.Application.Portfolios.Interfaces;
using CryptoPortfolio.Application.Portfolios.Services;
using CryptoPortfolio.Application.Users.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoPortfolio.IntegrationTests
{
    public class PortfolioApiIntegrationTests
    {
        private IntegrationTestWebApplicationFactory _factory;
        private HttpClient _httpClient;
        private string _token;
        private string _userId;
        private string _userName = "testIntegration";
        private string _password = "1234";
        private string _portfolioId = "";

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _factory = new IntegrationTestWebApplicationFactory();
            _userId = await CreateTestUser(_userName, _password);
            _token = await Login(_userName, _password);
        }

        private async Task<string> CreateTestUser(string userName, string password)
        {
            var httpClient = _factory.CreateClient();
            var command = new CreateUserCommand
            {
                UserName = userName,
                Password = password
            };
            var payload = JsonSerializer.Serialize(command);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/Security/Users", content);
            var userDto = JsonSerializer.Deserialize<UserDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            httpClient.Dispose();
            return userDto.Id;
        }

        private async Task<string> Login(string userName, string password)
        {
            var httpClient = _factory.CreateClient();
            var loginQuery = new LoginQuery
            {
                UserName = userName,
                Password = password
            };
            var payload = JsonSerializer.Serialize(loginQuery);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/Security/Login", content);
            var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            httpClient.Dispose();
            return loginResponse.Token;
        }

        [SetUp]
        public void Setup()
        {
            _httpClient = _factory.CreateClient();
        }

        [TearDown]
        public async Task TearDown()
        {
            _httpClient?.Dispose();
            if (!String.IsNullOrEmpty(_portfolioId))
            {
                using (var scope = _factory.Services.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetService<IPortfolioRepository>();
                    await repo.RemoveAsync(_portfolioId);
                }
            }
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await DeleteTestUser(_userId);
        }

        private async Task DeleteTestUser(string userId) {
            if (!String.IsNullOrEmpty(userId))
            {
                using (var scope = _factory.Services.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetService<IUserRepository>();
                    await repo.RemoveAsync(userId);
                }
            }
        }

        private async Task<HttpResponseMessage> CreateTestPorfolio()
        {
            var command = new CreatePortfolioCommand
            {
                Coin = "ETH",
                Quantity = 5,
                UserId = _userId
            };

            var payload = JsonSerializer.Serialize(command);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.PostAsync("/Portfolio", content);
            return response;
        }

        [Test]
        public async Task ShouldCreatePortfolioSuccessfully()
        {
            var response = await CreateTestPorfolio();

            var result = JsonSerializer.Deserialize<PortfolioOperationResponseDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _portfolioId = result.Id;

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Id, Is.Not.Empty);
            Assert.That(result.Coin, Is.EqualTo("ETH"));
            Assert.That(result.Quantity, Is.EqualTo(5));
        }

        [Test]
        public async Task ShouldIncrementQuantityOfExistingPortfolioSuccessfully()
        {
            var response = await CreateTestPorfolio();

            var result = JsonSerializer.Deserialize<PortfolioOperationResponseDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _portfolioId = result.Id;

            var command = new CreatePortfolioCommand
            {
                Coin = "ETH",
                Quantity = 15,
                UserId = _userId
            };
            var payload = JsonSerializer.Serialize(command);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            response = await _httpClient.PostAsync("/Portfolio", content);
            

            result = JsonSerializer.Deserialize<PortfolioOperationResponseDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Id, Is.EqualTo(_portfolioId));
            Assert.That(result.Coin, Is.EqualTo("ETH"));
            Assert.That(result.Quantity, Is.EqualTo(20));
        }

        [Test]
        public async Task ShouldNotAllowRequestWithoutAuthentication()
        {
            var command = new CreatePortfolioCommand
            {
                Coin = "ETH",
                Quantity = 5,
                UserId = _userId
            };

            var payload = JsonSerializer.Serialize(command);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = null;
            var response = await _httpClient.PostAsync("/Portfolio", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task ShouldUpdatePortfolioSuccessfully()
        {
            var response = await CreateTestPorfolio();

            var result = JsonSerializer.Deserialize<PortfolioOperationResponseDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _portfolioId = result.Id;

            var updCommand = new UpdatePortfolioCommand
            {
                Id = _portfolioId,
                Quantity = 10,
                UserId = _userId
            };
            var payload = JsonSerializer.Serialize(updCommand);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            response = await _httpClient.PutAsync($"/Portfolio/{_portfolioId}", content);

            result = JsonSerializer.Deserialize<PortfolioOperationResponseDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Quantity, Is.EqualTo(10));
        }

        [Test]
        public async Task ShouldUpdateNotExistingPortfolioReturnsNotFound()
        {
            var updCommand = new UpdatePortfolioCommand
            {
                Id = "765d3bee39589ced8cdfb856",
                Quantity = 10,
                UserId = _userId
            };
            var payload = JsonSerializer.Serialize(updCommand);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.PutAsync("/Portfolio/765d3bee39589ced8cdfb856", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task ShouldDeletePortfolioSuccessfully()
        {
            var response = await CreateTestPorfolio();

            var result = JsonSerializer.Deserialize<PortfolioOperationResponseDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _portfolioId = result.Id;

            var delCommand = new DeletePortfolioCommand
            {
                Id = _portfolioId,
                UserId = _userId
            };
            var payload = JsonSerializer.Serialize(delCommand);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"http://localhost:5000/Portfolio/{_portfolioId}"),
                Content = content
            };
            response = await _httpClient.SendAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task ShouldNotAllowUserModifyOtherUserData()
        {
            var response = await CreateTestPorfolio();
            var result = JsonSerializer.Deserialize<PortfolioOperationResponseDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _portfolioId = result.Id;

            // create second user
            var userId = await CreateTestUser("test1", "1234");
            var token = await Login("test1", "1234");

            var command = new UpdatePortfolioCommand
            {
                Id = _portfolioId,
                Quantity = 10,
                UserId = _userId // trying to use other userId
            };

            var payload = JsonSerializer.Serialize(command);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            response = await _httpClient.PutAsync($"/Portfolio/{_portfolioId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

            //trying to update other user portfolio
            command = new UpdatePortfolioCommand
            {
                Id = _portfolioId,
                Quantity = 10,
                UserId = userId
            };

            payload = JsonSerializer.Serialize(command);
            content = new StringContent(payload, Encoding.UTF8, "application/json");

            response = await _httpClient.PutAsync($"/Portfolio/{_portfolioId}", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            await DeleteTestUser(userId);
        }
    }
}