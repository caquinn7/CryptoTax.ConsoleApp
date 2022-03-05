using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Products.DAL;

namespace CryptoTaxV3.Domain.Products.Importers
{
    public interface IMarketImporter
    {
        Task<IEnumerable<Market>> GetMarketsAsync();
    }
}
