using System;
namespace CryptoTaxV3.Domain.Infrastructure.Exceptions
{
    public class InsufficientBalanceException : Exception
    {
        public string Asset { get; init; }
        public decimal MissingQuantity { get; init; }
        public override string Message =>
            $"{Asset} outgoing transactions exceed incoming transactions by {MissingQuantity}";
    }
}
