using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Integrations.Kraken
{
    public interface IKrakenClient
    {
        Task<IEnumerable<KrakenLedgerEntryDto>> GetLedgerEntriesAsync();
    }
}