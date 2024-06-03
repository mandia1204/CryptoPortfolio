using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CryptoPortfolio.Application.Common.Identity.Commands;
using CryptoPortfolio.Application.Common.Identity.Queries;
using CryptoPortfolio.Application.Portfolios.Commands;
using CryptoPortfolio.Application.Portfolios.Services;

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

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _factory = new IntegrationTestWebApplicationFactory();
            var httpClient = _factory.CreateClient();

            var command = new CreateUserCommand
            {
                UserName = _userName,
                Password = _password
            };
            var payload = JsonSerializer.Serialize(command);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/Security/Users", content);
            var userDto = JsonSerializer.Deserialize<UserDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _userId = userDto.Id;

            var loginQuery = new LoginQuery
            {
                UserName = _userName,
                Password = _password
            };

            response = await httpClient.PostAsync("/Security/Login", content);
            var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>((await response.Content.ReadAsStringAsync()), 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

            _token = loginResponse.Token;
            httpClient.Dispose();
        }

        [SetUp]
        public async Task Setup()
        {
            _httpClient = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }

        [Test]
        public async Task ShouldCreatePortfolioSuccessfully()
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

            var result = JsonSerializer.Deserialize<PortfolioOperationResponseDto>((await response.Content.ReadAsStringAsync()),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Id, Is.Not.Empty);
            Assert.That(result.Coin, Is.EqualTo("ETH"));
            Assert.That(result.Quantity, Is.EqualTo(5));
        }
    }
}