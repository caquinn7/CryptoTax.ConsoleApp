using CryptoTaxV3.Domain.AppSettings.DAL;
using CryptoTaxV3.Domain.Infrastructure;

namespace CryptoTaxV3.Domain.AppSettings
{
    public class AppSettings : IAppSettings
    {
        private readonly IAppSettingRepository _repo;

        public AppSettings(IAppSettingRepository appSettingRepository)
        {
            _repo = appSettingRepository;
        }

        public T Get<T>(string key)
        {
            Preconditions.ThrowValidationIfNullOrWhiteSpace(key, "AppSetting Key required");
            return _repo.Get<T>(key.Trim());
        }

        public void AddOrUpdate<T>(string key, T value)
        {
            Preconditions.ThrowValidationIfNullOrWhiteSpace(key, "AppSetting Key required");
            _repo.AddOrUpdate(key.Trim(), value);
        }
    }
}
