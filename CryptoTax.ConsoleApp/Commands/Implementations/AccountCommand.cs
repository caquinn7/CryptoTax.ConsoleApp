using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.Products;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class AccountCommand : FileBasedCommand, ICommand
    {
        private readonly IAccounts _accounts;

        public AccountCommand(
            IAccounts accounts,
            IAppSettings appSettings)
            : base(appSettings)
        {
            _accounts = accounts;
        }

        public void Execute(CommandArgs args)
        {
            switch(args[1])
            {
                // accounts activate -f <path to csv with accounts to activate>
                case "activate":
                    Activate(args);
                    break;
            }
        }

        private void Activate(CommandArgs args)
        {
            string filePath = GetFilePath(args, AppSettingKey.ACCOUNTS_FILE);
            int count = _accounts.ActivateFromCsv(filePath);
            Output.WriteLine(count + " accounts activated");
        }
    }
}
