using Ecommerce.Helper;
using EcommerceCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ecommerce.Controllers
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
            try
            {
                var user = HttpContext.Session.GetCurrentAuthentication();
                return View();
                //return Redirect("/user/login");

            }
            catch (Exception ex)
            {
                Response.Cookies.Delete("token");
                return Redirect("/user/login");
            }
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