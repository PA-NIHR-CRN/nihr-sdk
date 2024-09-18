namespace NIHR.Infrastructure.EntityFrameworkCore;

public interface ITimestamped
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
