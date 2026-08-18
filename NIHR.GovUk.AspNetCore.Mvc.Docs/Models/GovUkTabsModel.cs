namespace NIHR.GovUk.AspNetCore.Mvc.Docs.Models;

public record GovUkTabsModel(
    string Title,
    IEnumerable<GovUkTabModel> Tabs);