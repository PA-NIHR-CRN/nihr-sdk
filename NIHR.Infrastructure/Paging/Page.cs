using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NIHR.Infrastructure.Paging
{
    public class Page<T> : Page, IEnumerable<T>
    {
        public static Page<T> Empty() => new Page<T>(Enumerable.Empty<T>(), 0, 0, 0);

        private IEnumerable<T> _items;

        public Page(IEnumerable<T> source, int pageSize, int currentPage, int totalCount) : base(source, pageSize, currentPage, totalCount)
        {
            _items = source;
        }

        public IEnumerable<T> Items { get => _items; }

        public new IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    }

    public abstract class Page : IPage
    {
        protected int _totalCount;
        protected int _pageSize;
        protected int _currentPage;

        private IEnumerable _items;

        public Page(IEnumerable source, int pageSize, int currentPage, int totalCount)
        {
            _totalCount = totalCount;
            _items = source;
            _pageSize = pageSize;
            _currentPage = currentPage;
        }

        public int CurrentPage { get => _currentPage; }
        public int PageSize { get => _pageSize; }
        public int TotalCount { get => _totalCount; }

        public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling((decimal)TotalCount / PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public int CurrentCount() => _items.Cast<object>().Count();

        public IEnumerator GetEnumerator() => _items.GetEnumerator();
    }

    public interface IPage : IEnumerable
    {
        public int CurrentPage { get; }
        public int CurrentCount();
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage { get; }
        public bool HasNextPage { get; }
    }
}