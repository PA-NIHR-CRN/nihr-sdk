using NIHR.GovUk.AspNetCore.Mvc.Models;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers;

public class TabsContext
{
    public IList<GovUkTabModel> Tabs { get; } = new List<GovUkTabModel>();
}