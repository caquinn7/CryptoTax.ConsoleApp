using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Products
{
    public interface IProducts
    {
        int ActivateFromCsv(string filePath);
        IEnumerable<ProductDto> GetActive();
        Task<int> ImportFromSourcesAsync();
    }
}
