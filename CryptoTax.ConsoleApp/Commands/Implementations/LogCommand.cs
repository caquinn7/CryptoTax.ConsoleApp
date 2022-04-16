using System;
using System.IO;
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
            var logsPath = $"{AppContext.BaseDirectory}logs";
            var destination = $"{GetFilePath(args, AppSettingKey.LOGS_OUTFOLDER)}/Logs_{DateTime.Now:yyyy-MM-ddTHH-mm-ss}";
            Directory.CreateDirectory(destination);

            var files = new DirectoryInfo(logsPath).GetFiles();
            foreach (var file in files)
            {
                File.Copy(file.FullName, $"{destination}/{file.Name}", overwrite: true);
            }
            Output.WriteLine("Logs exported to " + destination);
        }
    }
}
