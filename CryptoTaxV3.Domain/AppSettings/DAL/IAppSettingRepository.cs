using System.Collections.Generic;

namespace CryptoTaxV3.Domain.AppSettings.DAL
{
    public interface IAppSettingRepository
    {
        T Get<T>(string key);
        void AddOrUpdate<T>(string key, T value);
        IEnumerable<KeyValuePair<string, string>> Get();
    }
}
