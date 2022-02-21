using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.AppSettings;

namespace CryptoTax.ConsoleApp.Commands
{
    public abstract class FileBasedCommand
    {
        private readonly IAppSettings _appSettings;

        public FileBasedCommand(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        protected string GetFilePath(CommandArgs args, string appSettingKey)
        {
            string filePath = args.GetArg("-f");
            if (filePath is not null)
            {
                _appSettings.AddOrUpdate(appSettingKey, filePath);
                return filePath;
            }
            return _appSettings.Get<string>(appSettingKey);
        }
    }
}
