using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace CryptoTaxV3.Domain.Infrastructure.Csv
{
    public class CsvReaderWrapper : ICsvReaderWrapper
    {
        public List<T> GetRecords<T>(string filePath, ClassMap<T> classMap = null)
        {
            using StreamReader streamReader = new(filePath);
            using CsvReader csvReader = new(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                MissingFieldFound = null,
                ShouldSkipRecord = args => args.Record.All(string.IsNullOrWhiteSpace),
                TrimOptions = TrimOptions.Trim
            });
            if (classMap != null)
            {
                csvReader.Context.RegisterClassMap(classMap.GetType());
            }
            return csvReader.GetRecords<T>().ToList();
        }

        public void WriteRecords<T>(string filePath, IEnumerable<T> data, ClassMap<T> classMap = null)
        {
            using StreamWriter streamWriter = new(filePath);
            using CsvWriter csvWriter = new(streamWriter, CultureInfo.InvariantCulture);
            if (classMap != null)
            {
                csvWriter.Context.RegisterClassMap(classMap.GetType());
            }
            csvWriter.WriteRecords(data);
        }
    }
}
