using System.Collections.Generic;
using CryptoTaxV3.Domain.Sources.DAL;

namespace CryptoTaxV3.Domain.Sources
{
    public class Sources : ISources
    {
        private readonly ISourceRepository _repo;

        public Sources(ISourceRepository sourceRepository)
        {
            _repo = sourceRepository;
        }

        public IEnumerable<SourceDto> Get() => _repo.Get();

        public Source Get(TxSource source) => _repo.Get(source.ToString());

        public IEnumerable<Source> GetActive() => _repo.GetActive();
    }
}
