using NIHR.GovUk.AspNetCore.Mvc.Docs.Models;

namespace NIHR.GovUk.AspNetCore.Mvc.Docs.TagHelpers;

public class TabsContext
{
    public IList<GovUkTabModel> Tabs { get; } = new List<GovUkTabModel>();
}