namespace NIHR.GovUk.AspNetCore.Mvc
{
    public class GovUkOptions
    {
        public string? ServiceName { get; set; }

        public bool HasServiceName() => !string.IsNullOrWhiteSpace(ServiceName);
    }
}
