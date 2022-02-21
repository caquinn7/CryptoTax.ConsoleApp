using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.CoinLookups;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class CoinLookupCommand : FileBasedCommand, ICommand
    {
        private readonly ICoinLookups _coinLookups;

        public CoinLookupCommand(
            ICoinLookups coinLookups,
            IAppSettings appSettings)
            : base (appSettings)
        {
            _coinLookups = coinLookups;
        }

        public void Execute(CommandArgs args)
        {
            if (args.Length == 1)
            {
                var lookups = _coinLookups.Get();
                Output.WriteTable(lookups);
                return;
            }

            switch (args[1])
            {
                // coinlookups import -f <path to csv with symbol mappings>
                case "import":
                    Import(args);
                    break;
            }
        }

        private void Import(CommandArgs args)
        {
            string filePath = GetFilePath(args, AppSettingKey.COIN_LOOKUPS_FILE);
            int count = _coinLookups.ImportFromCsv(filePath);
            Output.WriteLine(count + " lookups imported");
        }
    }
}
