using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Products.Importers;
using CryptoTaxV3.Domain.Sources;
using CryptoTaxV3.Domain.Sources.DAL;

namespace CryptoTaxV3.Domain.Products
{
    public class Products : IProducts
    {
        private readonly IAccounts _accounts;
        private readonly IMarkets _markets;
        private readonly ISources _sources;
        private readonly IProductImporter _importer;

        public Products(
            IAccounts accounts,
            IMarkets markets,
            ISources sources,
            IProductImporter productImporter)
        {
            _accounts = accounts;
            _markets = markets;
            _sources = sources;
            _importer = productImporter;
        }

        public IEnumerable<ProductDto> Get(TxSource txSource)
        {
            Source source = _sources.Get(txSource);
            return Enum.Parse<ProductType>(source.ProductType) switch
            {
                ProductType.Account => _accounts.Get(txSource),
                ProductType.Market => _markets.Get(txSource),
                _ => throw new NotImplementedException($"Invalid product type: {source.ProductType}")
            };
        }

        public async Task<int> ImportFromSourcesAsync()
        {
            var sources = _sources.GetActive();
            var importTasks = sources.Select(ImportAsync);
            int count = (await Task.WhenAll(importTasks)).Sum();
            return count;

            async Task<int> ImportAsync(Source source)
            {
                var products = await _importer.GetProductsAsync(Enum.Parse<TxSource>(source.Name, ignoreCase: true));
                if (products is IEnumerable<MarketDto> mkts)
                {
                    return _markets.Add(mkts);
                }
                else if (products is IEnumerable<AccountDto> accts)
                {
                    return _accounts.Add(accts);
                }
                return 0;
            }
        }
    }
}
