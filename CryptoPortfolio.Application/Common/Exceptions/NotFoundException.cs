namespace CryptoPortfolio.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base("Resource not found") { }
}
