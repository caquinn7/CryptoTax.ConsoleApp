using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Exceptions;
using CryptoTaxV3.Domain.Infrastructure.Csv;
using CryptoTaxV3.Domain.Services.CoinData;
using CryptoTaxV3.Domain.Sources;
using CryptoTaxV3.Domain.Sources.DAL;
using CryptoTaxV3.Domain.Transactions.DAL;
using CryptoTaxV3.Domain.Transactions.Importers;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CryptoTaxV3.Domain.Transactions
{
    public class Transactions : ITransactions
    {
        private readonly ITransactionRepository _repo;
        private readonly ITxImporterFactory _importerFactory;
        private readonly ISources _sources;
        private readonly ICoinDataService _coinDataService;
        private readonly ICsvReaderWrapper _csvReader;

        public Transactions(
            ITransactionRepository transactionRepository,
            ITxImporterFactory importerFactory,
            ISources sources,
            ICoinDataService coinDataService,
            ICsvReaderWrapper csvReader)
        {
            _repo = transactionRepository;
            _importerFactory = importerFactory;
            _sources = sources;
            _coinDataService = coinDataService;
            _csvReader = csvReader;
        }

        public IEnumerable<TransactionDto> Get() => _repo.GetTxDtos();

        public async Task<IEnumerable<ImportResultDto>> ImportFromSourcesAsync()
        {
            var sources = _sources.GetActive();
            ImportResult[] importResults = await Task.WhenAll(sources.Select(ImportFromSourceAsync));

            var importedTxs = importResults
                .SelectMany(r => r.Transactions ?? Enumerable.Empty<Transaction>())
                .Select(StandardizeAssetSymbol);

            var newTxs = DetermineNewTxs(importedTxs, _repo.Get());
            foreach (var t in newTxs)
            {
                if (t.UnitValue == null) await SetUnitValueAsync(t);
            }
            if (newTxs.Any())
            {
                _repo.InsertBatch(ImportType.Integration, newTxs);
            }

            return importResults.Select(r => new ImportResultDto
            {
                Source = r.Source,
                ErrorId = r.ErrorId,
                Count = r.ErrorId != null ? null
                    : newTxs.Count(t => t.Source == r.Source.ToString())
            });

            Task<ImportResult> ImportFromSourceAsync(Source source)
            {
                var importer = _importerFactory.GetImporter(Enum.Parse<TxSource>(source.Name));
                return importer.GetTransactionsAsync();
            }
        }

        public async Task<int> ImportFromCsvAsync(string filePath)
        {
            var txDtos = _csvReader.GetRecords(filePath, new TransactionMap());
            var importedTxs = txDtos.Select(t => new Transaction
            {
                Asset = t.Asset,
                Quantity = t.Quantity,
                UnitValue = t.UnitValue,
                Type = t.Type.FastToString(),
                Timestamp = t.Timestamp.ToUnixSeconds(),
                Source = t.Source,
                ExternalId = t.ExternalId
            }).Select(StandardizeAssetSymbol);

            var newTxs = DetermineNewTxs(importedTxs, _repo.Get());
            foreach (var t in newTxs)
            {
                if (t.UnitValue == null)
                {
                    await SetUnitValueAsync(t);
                }
                else if (t.UnitValueTimestamp == null)
                {
                    t.UnitValueTimestamp = t.Timestamp;
                }
            }

            return _repo.InsertBatch(ImportType.Csv, newTxs).count;
        }

        public void WriteToCsv(string filePath)
        {
            var txs = _repo.GetTxDtos();
            _csvReader.WriteRecords(filePath, txs, new TransactionMap());
        }

        public IEnumerable<TransactionDto> GetTxsWithoutUnitValue() => _repo.GetTxsWithoutUnitValue();

        public int UpdateUnitValue(int id, decimal unitValue, long? timestamp)
        {
            if (timestamp == null)
            {
                Transaction tx = _repo.Get(id);
                if (tx == null)
                {
                    throw new ValidationException($"No transaction with Id {id}");
                }
                timestamp = tx.Timestamp;
            }

            int affected = _repo.UpdateUnitValue(id, unitValue, timestamp.Value);
            if (affected == 0)
            {
                throw new ValidationException($"No transaction with Id {id}");
            }
            return affected;
        }

        public int Delete() => _repo.Delete();

        private static List<Transaction> DetermineNewTxs(
            IEnumerable<Transaction> importedTxs,
            IEnumerable<Transaction> existingTxs) =>
            importedTxs.Where(t => !t.IsDuplicate(existingTxs)).ToList();

        private Transaction StandardizeAssetSymbol(Transaction transaction)
        {
            string symbol = _coinDataService.GetStandardSymbol(transaction.Asset);
            if (symbol == null) return transaction;

            transaction.Asset = symbol;
            return transaction;
        }

        private async Task<Transaction> SetUnitValueAsync(Transaction transaction)
        {
            if (transaction.Asset == "USD")
            {
                transaction.UnitValue = 1m;
                transaction.UnitValueTimestamp = transaction.Timestamp;
                return transaction;
            }

            (decimal?, long?) priceData = await _coinDataService.GetHistoricalPriceAsync(transaction.Asset, transaction.Timestamp);
            transaction.UnitValue = priceData.Item1;
            transaction.UnitValueTimestamp = priceData.Item2;
            return transaction;
        }
    }

    internal class TransactionMap : ClassMap<TransactionDto>
    {
        private static readonly EnumConverter _enumConverter = new(typeof(TxType));
        public TransactionMap()
        {
            Map(t => t.Asset).Index(0);
            Map(t => t.Quantity).Index(1);
            Map(t => t.UnitValue).Index(2);
            Map(t => t.Type).TypeConverter(_enumConverter).Index(3);
            Map(t => t.Timestamp).Index(4)
               .Convert(convertFromStringFunction: args => DateTime.Parse(args.Row.GetField("Timestamp")))
               .Convert(convertToStringFunction: args => args.Value.Timestamp.ToString("yyyy-MM-dd hh:mm:ss tt"));
            Map(t => t.Source).Index(5);
            Map(t => t.ExternalId).Index(6);
        }
    }
}
