using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Products
{
    public interface IProducts
    {
        IEnumerable<ProductDto> Get(TxSource txSource);
        Task<int> ImportFromSourcesAsync();
    }
}