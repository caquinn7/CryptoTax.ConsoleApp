using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Products.Importers
{
    public interface IProductImporter
    {
        Task<IEnumerable<Product>> GetProductsAsync(TxSource txSource);
    }
}
