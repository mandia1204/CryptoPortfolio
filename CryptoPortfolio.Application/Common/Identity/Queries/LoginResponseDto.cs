namespace CryptoPortfolio.Application.Common.Identity.Queries
{
    public class LoginResponseDto
    {
        public bool Sucess { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
    }
}
