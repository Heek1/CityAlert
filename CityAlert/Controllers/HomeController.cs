using CityAlert.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CityAlert.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult EventMap() => View();

        [Authorize(Roles = "Resident")]
        public IActionResult MySubscriptions() => View();

        [Authorize(Roles = "Moderator")]
        public IActionResult CreateEvents() => View();

        [Authorize(Roles = "Moderator")]
        public IActionResult AdminPanel() => View();

        public IActionResult Logout() => SignOut("Cookies", "OpenIdConnect");

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
