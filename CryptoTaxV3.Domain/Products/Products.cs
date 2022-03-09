using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Exceptions;
using CryptoTaxV3.Domain.Infrastructure.Csv;
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
        private readonly ICsvReaderWrapper _csvReader;

        public Products(
            IAccounts accounts,
            IMarkets markets,
            ISources sources,
            IProductImporter productImporter,
            ICsvReaderWrapper csvReader)
        {
            _accounts = accounts;
            _markets = markets;
            _sources = sources;
            _importer = productImporter;
            _csvReader = csvReader;
        }

        public IEnumerable<ProductDto> GetActive()
        {
            var sources = _sources.GetActive();
            return GetAccounts()
                .Concat(GetMarkets())
                .OrderBy(p => p.Source.ToString())
                .ThenBy(p => p.Name);

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
            var products = _csvReader.GetRecords(filePath, new ProductMap());
            var sources = _sources.Get();

            var accounts = new List<Account>();
            var markets = new List<Market>();

            foreach (ProductCsvDto product in products)
            {
                Source source = sources.SingleOrDefault(s => s.Name == product.Source.FastToString());
                if (source is null)
                {
                    throw new ValidationException($"Source not found: {product.Source}");
                }
                if (source.ProductType == ProductType.Account.FastToString())
                {
                    accounts.Add(new Account
                    {
                        Source = source.Name,
                        Asset = product.Base
                    });
                }
                else if (source.ProductType == ProductType.Market.FastToString())
                {
                    markets.Add(new Market
                    {
                        Source = source.Name,
                        Base = product.Base,
                        Quote = product.Quote
                    });
                }
            }

            int count = _accounts.Activate(accounts);
            count += _markets.Activate(markets);
            return count;
        }
    }

    internal class ProductMap : ClassMap<ProductCsvDto>
    {
        private static readonly EnumConverter _enumConverter = new(typeof(TxSource));
        public ProductMap()
        {
            Map(p => p.Source).TypeConverter(_enumConverter).Index(0);
            Map(p => p.Base).Index(1);
            Map(p => p.Quote).Index(2);
        }
    }

    internal class ProductCsvDto
    {
        public TxSource Source { get; init; }
        public string Base { get; init; }
        public string Quote { get; init; }
    }
}
