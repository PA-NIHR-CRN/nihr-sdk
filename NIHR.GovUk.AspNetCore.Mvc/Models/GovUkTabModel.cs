using Microsoft.AspNetCore.Html;

namespace NIHR.GovUk.AspNetCore.Mvc.Models;

public class GovUkTabModel
{
    public string Id { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public bool Selected { get; init; }

    public IHtmlContent Content { get; init; } = HtmlString.Empty;
}