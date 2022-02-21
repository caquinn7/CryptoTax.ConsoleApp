using System.Collections.Generic;
using System.Linq;
using CryptoTaxV3.Domain.Exceptions;
using CryptoTaxV3.Domain.Infrastructure;
using CryptoTaxV3.Domain.Infrastructure.Csv;
using CryptoTaxV3.Domain.Products.DAL;
using CsvHelper.Configuration;

namespace CryptoTaxV3.Domain.Products
{
    public class Markets : IMarkets
    {
        private readonly IMarketRepository _repo;
        private readonly ICsvReaderWrapper _csvReader;

        public Markets(
            IMarketRepository marketRepo,
            ICsvReaderWrapper csvReader)
        {
            _repo = marketRepo;
            _csvReader = csvReader;
        }

        public IEnumerable<MarketDto> Get(TxSource source) => _repo.Get(source.ToString());

        public IEnumerable<Market> GetActive(TxSource source) => _repo.GetActive(source.ToString());

        public int Add(IEnumerable<MarketDto> marketDtos)
        {
            var markets = new List<Market>();
            foreach (var m in marketDtos)
            {
                Preconditions.ThrowValidationIfNullOrWhiteSpace(m.Base, "Market Base Asset required");
                Preconditions.ThrowValidationIfNullOrWhiteSpace(m.Quote, "Market Quote Asset required");

                markets.Add(new Market
                {
                    Source = m.Source.FastToString(),
                    Base = m.Base,
                    Quote = m.Quote,
                    IsActive = false
                });
            }
            return _repo.Insert(markets);
        }

        public int ActivateFromCsv(string filePath)
        {
            var markets = _csvReader.GetRecords(filePath, new MarketMap());
            var invalidMkt = markets.FirstOrDefault(m => !_repo.Exists(m));
            if (invalidMkt != null)
            {
                string msg = $"Market not found: Source: {invalidMkt.Source}, Base: {invalidMkt.Base}, Quote: {invalidMkt.Quote}";
                throw new ValidationException(msg);
            }

            return _repo.Activate(markets);
        }
    }

    internal class MarketMap : ClassMap<Market>
    {
        public MarketMap()
        {
            Map(m => m.Source);
            Map(m => m.Base);
            Map(m => m.Quote);
        }
    }
}
