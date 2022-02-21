using System.Collections.Generic;
using CryptoTaxV3.Domain.Infrastructure.DataAccess;

namespace CryptoTaxV3.Domain.Sources.DAL
{
    public class SourceRepository : Repository, ISourceRepository
    {
        public SourceRepository(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<SourceDto> Get() =>
            Select<SourceDto>(@"
                select
                    name,
                    product_type ProductType,
                    is_active IsActive
                from sources
                order by name");

        public Source Get(string name) =>
            SelectSingle<Source>(@"
                select
                    name,
                    product_type ProductType,
                    market_hyphenated MarketHyphenated,
                    is_active IsActive
                from sources
                where name = @name", new { name });

        public IEnumerable<Source> GetActive() =>
            Select<Source>(@"
                select
                    name,
                    product_type ProductType,
                    market_hyphenated MarketHyphenated,
                    is_active IsActive
                from sources
                where is_active = 1
                order by name");
    }
}
