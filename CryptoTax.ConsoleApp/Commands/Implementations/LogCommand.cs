using System;
using System.IO;
using System.Reflection;
using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.AppSettings;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class LogCommand : FileBasedCommand, ICommand
    {
        public LogCommand(IAppSettings appSettings) : base(appSettings)
        {
        }

        public void Execute(CommandArgs args)
        {
            var logsPath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}/log.txt";

            var fileName = $"log_{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.txt";
            var outputPath = $"{GetFilePath(args, AppSettingKey.LOGS_OUTFILE)}/{fileName}";

            File.Copy(logsPath, outputPath, true);
            Output.WriteLine("Logs exported to " + outputPath);
        }
    }
}
