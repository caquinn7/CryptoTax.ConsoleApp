using System.Collections.Generic;

namespace CryptoTaxV3.Domain.Products.DAL
{
    public interface IMarketRepository
    {
        int Activate(IEnumerable<Market> markets);
        IEnumerable<Market> GetActive(string source);
        int Insert(IEnumerable<Market> markets);
        bool Exists(Market market);
    }
}