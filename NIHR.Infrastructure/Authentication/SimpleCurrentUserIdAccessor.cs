using NIHR.Infrastructure;

namespace NIHR.Infrastructure
{
    public class SimpleCurrentUserIdAccessor<T> : ICurrentUserIdAccessor<T> where T : struct
    {
        protected T? _userId;

        public T? UserId => _userId;

        public bool SuppressUnknownUserIdWarning => false;

        public void SetCurrentUserId(T id)
        {
            _userId = id;
        }
    }
}