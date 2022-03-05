using System.Collections.Generic;

namespace CryptoTaxV3.Domain.Sources.DAL
{
    public interface ISourceRepository
    {
        IEnumerable<Source> GetActive();
        IEnumerable<SourceDto> GetDtos();
        Source Get(string name);
        IEnumerable<Source> Get();
    }
}
