using System.Collections.Generic;

namespace CryptoTaxV3.Domain.AppSettings
{
    public interface IAppSettings
    {
        void AddOrUpdate<T>(string key, T value);
        T Get<T>(string key);
        IEnumerable<KeyValuePair<string, string>> Get();
    }
}
