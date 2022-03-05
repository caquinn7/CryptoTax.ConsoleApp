using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Products
{
    public interface IProducts
    {
        IEnumerable<ProductDto> GetActive();
        Task<int> ImportFromSourcesAsync();
    }
}
