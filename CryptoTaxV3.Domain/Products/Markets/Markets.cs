﻿using System.Collections.Generic;
using System.Linq;
using CryptoTaxV3.Domain.Exceptions;
using CryptoTaxV3.Domain.Infrastructure;
using CryptoTaxV3.Domain.Products.DAL;
using CsvHelper.Configuration;

namespace CryptoTaxV3.Domain.Products
{
    public class Markets : IMarkets
    {
        private readonly IMarketRepository _repo;

        public Markets(IMarketRepository marketRepo)
        {
            _repo = marketRepo;
        }

        public IEnumerable<Market> GetActive(TxSource source) =>
            _repo.GetActive(source.ToString());

        public int Add(IEnumerable<Market> markets)
        {
            foreach (var m in markets)
            {
                Preconditions.ThrowValidationIfNullOrWhiteSpace(m.Base, "Market Base Asset required");
                Preconditions.ThrowValidationIfNullOrWhiteSpace(m.Quote, "Market Quote Asset required");
            }
            return _repo.Insert(markets);
        }

        public int Activate(IEnumerable<Market> markets)
        {
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
