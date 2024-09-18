namespace NIHR.Infrastructure.EntityFrameworkCore;

public interface IAudit : ITimestamped
{
    int CreatedById { get; set; }
    int UpdatedById { get; set; }
}
