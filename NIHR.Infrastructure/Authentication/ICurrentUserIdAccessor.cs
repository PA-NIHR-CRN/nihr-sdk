namespace NIHR.Infrastructure
{
    public interface ICurrentUserIdAccessor<T> : ICurrentUserIdProvider<T> where T : struct
    {
        void SetCurrentUserId(T id);
    }
}