using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.FormEntries;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class FormEntriesCommand : FileBasedCommand, ICommand
    {
        private readonly IFormEntries _formEntries;

        public FormEntriesCommand(IFormEntries formEntries, IAppSettings appSettings)
            : base(appSettings)
        {
            _formEntries = formEntries;
        }

        public void Execute(CommandArgs args)
        {
            GenerateFormEntries(args);
        }

        // formentries -y <tax year> -a <asset> -f <output path> -s
        private void GenerateFormEntries(CommandArgs args)
        {
            int? taxYear = int.Parse(args.GetArg("-y"));
            string asset = args.GetArg("-a");
            string outputPath = GetFilePath(args, AppSettingKey.FORM_ENTRIES_FOLDER);
            bool splitByTerm = args.GetArg("-s", isPair: false) is not null;

            _formEntries.ExportToCsv(taxYear, asset, outputPath, splitByTerm);
            Output.WriteLine("Form Entries exported");
        }
    }
}
