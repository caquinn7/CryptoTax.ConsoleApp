using System.Linq;
using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.Transactions;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class TransactionCommand : FileBasedCommand, ICommand
    {
        private readonly ITransactions _transactions;

        public TransactionCommand(ITransactions transactions, IAppSettings appSettings)
            : base(appSettings)
        {
            _transactions = transactions;
        }

        public void Execute(CommandArgs args)
        {
            switch (args[1])
            {
                // txs delete
                case "delete":
                    DeleteTxs();
                    break;
                // txs export -f <path>
                case "export":
                    ExportToCsv(args);
                    break;
                // txs import -s (import from api sources)
                // txs import -f <path to csv file with txs>
                case "import":
                    Import(args);
                    break;
                // txs invalid
                case "invalid":
                    DisplayInvalidTxs();
                    break;
                // txs unitvalue -i <tx id> -v <$ value of 1 unit at time of tx> -t <timestamp in unix seconds>
                case "unitvalue":
                    UpdateUnitValue(args);
                    break;
            }
        }

        private void DeleteTxs()
        {
            int count = _transactions.Delete();
            Output.WriteLine($"All {count} transactions deleted");
        }

        private void ExportToCsv(CommandArgs args)
        {
            string filePath = GetFilePath(args, AppSettingKey.TXS_OUTFILE);
            _transactions.WriteToCsv(filePath);
        }

        private void Import(CommandArgs args)
        {
            switch (args[2])
            {
                case "-s":
                    ImportFromIntegrations();
                    break;
                case "-f":
                    ImportFromCsv();
                    break;
            }

            void ImportFromIntegrations()
            {
                var results = _transactions.ImportFromSourcesAsync().GetAwaiter().GetResult();
                int importCount = results.Select(r => r.Count).Sum().Value;
                Output.WriteTable(results);
                Output.WriteLine("\n" + importCount + " new transactions imported");
            }

            void ImportFromCsv()
            {
                string filePath = GetFilePath(args, AppSettingKey.TXS_INFILE);
                int count = _transactions.ImportFromCsvAsync(filePath).GetAwaiter().GetResult();
                Output.WriteLine(count + " transactions imported");
            }
        }

        private void DisplayInvalidTxs()
        {
            var txs = _transactions.GetTxsWithoutUnitValue();
            Output.WriteTable(txs);
        }

        private void UpdateUnitValue(CommandArgs args)
        {
            int id = int.Parse(args.GetArg("-i"));
            decimal unitvalue = decimal.Parse(args.GetArg("-v"));
            bool parsed = long.TryParse(args.GetArg("-t"), out long timestamp);
            _transactions.UpdateUnitValue(id, unitvalue, parsed ? timestamp : null );
        }
    }
}
