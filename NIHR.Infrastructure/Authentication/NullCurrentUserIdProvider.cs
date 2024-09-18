namespace NIHR.Infrastructure.EntityFrameworkCore
{
    public class NullCurrentUserIdProvider<T> : ICurrentUserIdAccessor<T> where T : struct
    { 
        public T? UserId => default;

        public bool SuppressUnknownUserIdWarning => false;

        public void SetCurrentUserId(T id)
        {
        }
    }
}