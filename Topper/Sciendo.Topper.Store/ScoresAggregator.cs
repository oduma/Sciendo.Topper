using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Store
{
    public class ScoresAggregator
    {
        private readonly Repository<TopItemWithScore> _repository;

        public ScoresAggregator(Repository<TopItemWithScore> repository)
        {
            _repository = repository;
        }
        public IEnumerable<BaseTopItem> AggregateTopItems(int limit = 0)
        {
            var allResults= _repository.GetAllItemsAsync().Result.GroupBy(i => i.Name)
                .Select(b => new BaseTopItem { Name = b.Key, Hits = b.Sum(x => x.Score) });
            return limit > 0 ? allResults.Take(limit) : allResults;
        }
    }
}
