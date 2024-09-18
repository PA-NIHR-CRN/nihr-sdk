namespace NIHR.Infrastructure.EntityFrameworkCore
{
    public interface IHardDeleteAuthorizationProvider
    {
        bool CanHardDelete();
    }
}