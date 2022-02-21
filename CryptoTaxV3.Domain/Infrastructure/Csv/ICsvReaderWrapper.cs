using System.Collections.Generic;
using CsvHelper.Configuration;

namespace CryptoTaxV3.Domain.Infrastructure.Csv
{
    public interface ICsvReaderWrapper
    {
        List<T> GetRecords<T>(string filePath, ClassMap<T> classMap = null);
        void WriteRecords<T>(string filePath, IEnumerable<T> data, ClassMap<T> classMap = null);
    }
}