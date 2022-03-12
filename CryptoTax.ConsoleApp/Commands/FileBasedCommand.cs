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

        /// <summary>
        /// Checks args, then AppSettings for the filepath. Returns null if not found.
        /// If the filepath is found in args, then the AppSetting is updated.
        /// </summary>
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
