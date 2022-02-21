using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.Credentials;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class CredentialCommand : FileBasedCommand, ICommand
    {
        private readonly ICredentials _creds;

        public CredentialCommand(
            ICredentials credentials,
            IAppSettings appSettings)
            : base(appSettings)
        {
            _creds = credentials;
        }

        public void Execute(CommandArgs args)
        {
            if (args.Length == 1)
            {
                var creds = _creds.Get();
                Output.WriteTable(creds);
                return;
            }

            switch (args[1])
            {
                // credentials import -f <path to csv with api credentials>
                case "import":
                    Import(args);
                    break;
            }
        }

        private void Import(CommandArgs args)
        {
            string filePath = GetFilePath(args, AppSettingKey.CREDS_FILE);
            int count = _creds.ImportFromCsv(filePath);
            Output.WriteLine(count + " credentials imported");
        }
    }
}
