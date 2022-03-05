using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Products.DAL;
using CryptoTaxV3.Domain.Products.Importers;
using CryptoTaxV3.Domain.Sources;
using CryptoTaxV3.Domain.Sources.DAL;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

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

        public IEnumerable<ProductDto> GetActive()
        {
            var sources = _sources.GetActive();
            return GetAccounts().Concat(GetMarkets())
                .OrderBy(p => p.Source.ToString());

            List<ProductDto> GetAccounts()
            {
                var products = new List<ProductDto>();

                var accountSources = sources
                    .Where(s => s.ProductType == ProductType.Account.ToString())
                    .Select(s => s.Name)
                    .Select(Enum.Parse<TxSource>);

                foreach (TxSource source in accountSources)
                {
                    var accounts = _accounts.GetActive(source);
                    products.AddRange(accounts.Select(a => new ProductDto
                    {
                        Id = a.Id,
                        Source = source,
                        Name = a.Asset
                    }));
                }

                return products;
            }

            List<ProductDto> GetMarkets()
            {
                var products = new List<ProductDto>();

                var marketSources = sources
                    .Where(s => s.ProductType == ProductType.Market.ToString());

                foreach (Source source in marketSources)
                {
                    var sourceEnum = Enum.Parse<TxSource>(source.Name);
                    var markets = _markets.GetActive(sourceEnum);
                    products.AddRange(markets.Select(m => new ProductDto
                    {
                        Id = m.Id,
                        Source = sourceEnum,
                        Name = source.MarketHyphenated.Value
                            ? $"{m.Base}-{m.Quote}"
                            : $"{m.Base}{m.Quote}"
                    }));
                }

                return products;
            }
        }

        public async Task<int> ImportFromSourcesAsync()
        {
            var sources = _sources.Get();
            var importTasks = sources.Select(ImportAsync);
            int count = (await Task.WhenAll(importTasks)).Sum();
            return count;

            async Task<int> ImportAsync(Source source)
            {
                var products = await _importer.GetProductsAsync(Enum.Parse<TxSource>(source.Name, ignoreCase: true));
                if (products is IEnumerable<Market> mkts)
                {
                    return _markets.Add(mkts);
                }
                if (products is IEnumerable<Account> accts)
                {
                    return _accounts.Add(accts);
                }
                return 0;
            }
        }

        public int ActivateFromCsv(string filePath)
        {
            throw new NotImplementedException();
        }
    }

    internal class ProductMap : ClassMap<ProductDto>
    {
        private static readonly EnumConverter _enumConverter = new(typeof(TxSource));
        public ProductMap()
        {
            Map(p => p.Source).TypeConverter(_enumConverter).Index(0);
            Map(p => p.Name).Index(1);
        }
    }
}
