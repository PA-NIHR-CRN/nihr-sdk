namespace NIHR.Infrastructure.EntityFrameworkCore
{
    public class NihrConventionOptions
    {
        public bool DisableAutodetectChangesInInterceptors { get; set; } = false;
        public bool UseTableNameFromDbSet { get; set; } = false;
    }
}