using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoTaxV3.Domain.Integrations.Binance
{
    public class BinanceExchangeInfoDto
    {
        [JsonProperty("symbols")]
        public IEnumerable<BinanceMarketDto> Markets { get; set; }
    }

    public class BinanceMarketDto
    {
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
    }
}
