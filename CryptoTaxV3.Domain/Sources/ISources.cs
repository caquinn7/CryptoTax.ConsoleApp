using System.Collections.Generic;
using CryptoTaxV3.Domain.Sources.DAL;

namespace CryptoTaxV3.Domain.Sources
{
    public interface ISources
    {
        IEnumerable<SourceDto> GetDtos();
        Source Get(TxSource source);
        IEnumerable<Source> GetActive();
        IEnumerable<Source> Get();
    }
}
