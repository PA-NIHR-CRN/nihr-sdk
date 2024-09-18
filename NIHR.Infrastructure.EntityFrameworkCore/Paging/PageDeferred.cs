using Z.EntityFramework.Plus;

namespace NIHR.Infrastructure.Paging
{
    public class PageDeferred<T>
    {
        private QueryFutureEnumerable<T> _items;
        private QueryFutureValue<int> _totalCount;
        private int _pageSize;
        private int _currentPage;

        public PageDeferred(QueryFutureEnumerable<T> source, int pageSize, int currentPage, QueryFutureValue<int> totalCount)
        {

            _items = source;
            _totalCount = totalCount;
            _pageSize = pageSize;
            _currentPage = currentPage;
        }

        public Page<T> Value => new Page<T>(_items.ToList(), _pageSize, _currentPage, _totalCount.Value);

        public async Task<Page<T>> ValueAsync(CancellationToken token = default) => new Page<T>(await _items.ToListAsync(token), _pageSize, _currentPage, await _totalCount.ValueAsync(token));

        public static implicit operator Page<T>(PageDeferred<T> source) => source.Value;

    }
}
