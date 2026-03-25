using CityAlert.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CityAlert.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult EventMap() => View();

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" });
        }

        [Authorize(Roles = "resident")]
        public IActionResult MySubscriptions() => View();

        [Authorize(Roles = "moderator")]
        public IActionResult CreateEvents() => View();

        [Authorize(Roles = "moderator")]
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