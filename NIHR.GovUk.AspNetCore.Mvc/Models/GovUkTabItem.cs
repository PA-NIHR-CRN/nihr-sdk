namespace NIHR.GovUk.AspNetCore.Mvc.Models;

public class GovUkTabItem
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public bool Selected { get; set; }

    public string Content { get; set; } = string.Empty;
}