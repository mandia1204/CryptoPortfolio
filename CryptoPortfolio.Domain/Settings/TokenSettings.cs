﻿namespace CryptoPortfolio.Domain.Settings
{
    public class TokenSettings
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string SigningKey { get; set; } = null!;
        public short ExpiresInMinutes { get; set; }
    }
}
