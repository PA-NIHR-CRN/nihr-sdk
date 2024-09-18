namespace NIHR.Infrastructure
{
    public class SimpleCurrentUserIdProvider<T> : ICurrentUserIdProvider<T> where T : struct
    {
        private readonly ICurrentUserIdAccessor<T> _currentUserIdAccessor;

        public SimpleCurrentUserIdProvider(ICurrentUserIdAccessor<T> currentUserIdAccessor)
        {
            _currentUserIdAccessor = currentUserIdAccessor;
        }

        public T? UserId => _currentUserIdAccessor.UserId;

        public virtual bool SuppressUnknownUserIdWarning => _currentUserIdAccessor.SuppressUnknownUserIdWarning;
    };
}