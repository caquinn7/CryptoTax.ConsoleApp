using System.Collections.Generic;

namespace CryptoTaxV3.Domain.Sources.DAL
{
    public interface ISourceRepository
    {
        IEnumerable<Source> GetActive();
        IEnumerable<SourceDto> Get();
        Source Get(string name);
    }
}