using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.Products;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class ProductCommand : FileBasedCommand, ICommand
    {
        private readonly IProducts _products;

        public ProductCommand(
            IProducts products,
            IAppSettings appSettings)
            : base(appSettings)
        {
            _products = products;
        }

        public void Execute(CommandArgs args)
        {
            if (args.Length == 1)
            {
                ShowActiveProducts();
                return;
            }

            switch (args[1])
            {
                case "activate":
                    Activate(args);
                    break;
                case "import":
                    Import();
                    break;
            }
        }

        private void ShowActiveProducts()
        {
            var products = _products.GetActive();
            Output.WriteTable(products);
        }

        private void Activate(CommandArgs args)
        {
            string filePath = GetFilePath(args, AppSettingKey.PRODUCTS_FILE);
            int count = _products.ActivateFromCsv(filePath);
            Output.WriteLine(count + " products activated");
        }

        private void Import()
        {
            int count = _products.ImportFromSourcesAsync().GetAwaiter().GetResult();
            Output.WriteLine(count + " products imported");
        }
    }
}
