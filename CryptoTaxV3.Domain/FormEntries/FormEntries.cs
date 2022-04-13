using System;
using System.Globalization;
using System.Linq;
using CryptoTaxV3.Domain.Infrastructure.Csv;
using CryptoTaxV3.Domain.Transactions;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace CryptoTaxV3.Domain.FormEntries
{
    public class FormEntries : IFormEntries
    {
        private readonly ITransactions _transactions;
        private readonly ICsvReaderWrapper _csvReader;
        private readonly ILogger<FormEntries> _logger;

        public FormEntries(
            ITransactions transactions,
            ICsvReaderWrapper csvReader,
            ILogger<FormEntries> logger)
        {
            _transactions = transactions;
            _csvReader = csvReader;
            _logger = logger;
        }

        public void ExportToCsv(int? taxYear, string asset, string folderPath, bool splitByTerm)
        {
            _logger.LogInformation("Generating form entries for asset {asset} in tax year {year}", asset, taxYear);

            var txs = _transactions.Get()
                .Where(t => t.Asset != "USD")
                .Where(t => asset is null || t.Asset == asset);

            var txGroups = txs
                .GroupBy(t => t.Asset)
                .OrderBy(g => g.Key);

            var formEntries = txGroups
                .Select(g => FormEntriesGenerator.GetFormEntries(g, _logger))
                .SelectMany(fs => fs
                    .Where(f => taxYear is null || f.DateSold.Year == taxYear)
                    .Where(f => decimal.Round(f.Gain, 2, MidpointRounding.AwayFromZero) != 0m)
                    .OrderBy(f => f.DateAcquired).ThenBy(f => f.DateSold))
                .ToList();

            string filePathStart = $"{folderPath}/";
            if (taxYear != null) filePathStart += $"{taxYear}_";
            if (asset != null) filePathStart += $"{asset}_";
            filePathStart += "Form8949Entries";

            if (!splitByTerm)
            {
                _csvReader.WriteRecords($"{filePathStart}.csv", formEntries.ToList(), new FormEntryMap());
                return;
            }

            var longs = formEntries.Where(e => e.IsLongTerm);
            var shorts = formEntries.Where(e => !e.IsLongTerm);

            _csvReader.WriteRecords($"{filePathStart}_Long.csv", longs, new FormEntryMap());
            _csvReader.WriteRecords($"{filePathStart}_Short.csv", shorts, new FormEntryMap());
        }
    }

    internal class FormEntryMap : ClassMap<FormEntry>
    {
        public FormEntryMap()
        {
            string dateFormat = "yyyy-MM-dd hh:mm tt zz";

            Map(f => f.Quantity).Index(0);
            Map(f => f.Asset).Index(1);

            Map(f => f.DateAcquired).Index(2)
                .Convert(convertToStringFunction: args => args.Value.DateAcquired.ToString(dateFormat));

            Map(f => f.DateSold).Index(3)
                .Convert(convertToStringFunction: args => args.Value.DateSold.ToString(dateFormat));

            Map(f => f.Proceeds).Index(4)
                .Convert(convertToStringFunction: args => args.Value.Proceeds.ToString("F", CultureInfo.InvariantCulture));

            Map(f => f.CostBasis).Index(5)
                .Convert(convertToStringFunction: args => args.Value.CostBasis.ToString("F", CultureInfo.InvariantCulture));

            Map(f => f.Gain).Index(6)
                .Convert(convertToStringFunction: args => args.Value.Gain.ToString("F", CultureInfo.InvariantCulture));
        }
    }
}