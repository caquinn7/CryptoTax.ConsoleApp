using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Sources;
using CryptoTaxV3.Domain.Sources.DAL;

namespace CryptoTaxV3.Domain.Products.Importers
{
    public class ProductImporter : IProductImporter
    {
        private readonly IMarketImporterFactory _mktImporterFactory;
        private readonly IAccountImporter _acctImporter;
        private readonly ISources _sources;

        public ProductImporter(
            IMarketImporterFactory marketImporterFactory,
            IAccountImporter coinbaseAccountImporter,
            ISources sources)
        {
            _mktImporterFactory = marketImporterFactory;
            _acctImporter = coinbaseAccountImporter;
            _sources = sources;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(TxSource txSource)
        {
            Source source = _sources.Get(txSource);
            if (source.ProductType == "Market")
            {
                var importer = _mktImporterFactory.GetImporter(txSource);
                return await importer.GetMarketsAsync();
            }
            else if (txSource == TxSource.Coinbase)
            {
                return await _acctImporter.GetAccountsAsync();
            }
            else return Enumerable.Empty<ProductDto>();
        }
    }
}
