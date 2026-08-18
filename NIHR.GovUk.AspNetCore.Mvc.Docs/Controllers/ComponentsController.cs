using Microsoft.AspNetCore.Mvc;

namespace NIHR.GovUk.AspNetCore.Mvc.Docs.Controllers;

public class ComponentsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Tabs()
    {
        return View();
    }
}