using System;
using System.Linq;
using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.Products;
using CryptoTaxV3.Domain.Sources;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class AccountCommand : FileBasedCommand, ICommand
    {
        private readonly IAccounts _accounts;
        private readonly ISources _sources;

        public AccountCommand(
            IAccounts accounts,
            ISources sources,
            IAppSettings appSettings)
            : base(appSettings)
        {
            _accounts = accounts;
            _sources = sources;
        }

        public void Execute(CommandArgs args)
        {
            if (args.Length == 1)
            {
                var activeAccounts = _sources.GetActive()
                    .SelectMany(s => _accounts.Get(Enum.Parse<TxSource>(s.Name, ignoreCase: true)))
                    .Where(a => a.IsActive);

                Output.WriteTable(activeAccounts);
                return;
            }

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
