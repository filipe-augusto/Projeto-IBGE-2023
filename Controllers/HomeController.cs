using Microsoft.AspNetCore.Mvc;
using ProjetoIBGE.ViewModels;

namespace ProjetoIBGE.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            //RedirectToAction("swagger");
            return Ok();
        }
    }
}
