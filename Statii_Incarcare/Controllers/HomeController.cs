using Microsoft.AspNetCore.Mvc;
using Statii_Incarcare.Models;
using System.Diagnostics;

namespace Statii_Incarcare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            ViewData["layout"] = "1";
            if (HttpContext.Request.Cookies.ContainsKey("user_id"))
            {
                var userId = HttpContext.Request.Cookies["user_id"];

                if (userId != null)
                    ViewData["layout"]="2";
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}