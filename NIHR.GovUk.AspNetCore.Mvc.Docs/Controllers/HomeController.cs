using Microsoft.AspNetCore.Mvc;

namespace NIHR.GovUk.AspNetCore.Mvc.Docs.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}