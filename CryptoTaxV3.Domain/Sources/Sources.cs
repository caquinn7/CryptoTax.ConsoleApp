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

        public IEnumerable<SourceDto> GetDtos() => _repo.GetDtos();

        public Source Get(TxSource source) => _repo.Get(source.ToString());

        public IEnumerable<Source> Get() => _repo.Get();

        public IEnumerable<Source> GetActive() => _repo.GetActive();
    }
}
