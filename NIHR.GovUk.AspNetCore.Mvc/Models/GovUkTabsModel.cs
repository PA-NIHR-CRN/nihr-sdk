namespace NIHR.GovUk.AspNetCore.Mvc.Models;

public record GovUkTabsModel(
    string Title,
    IEnumerable<GovUkTabModel> Tabs);