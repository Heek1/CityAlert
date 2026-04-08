using CityAlert.Data;
using CityAlert.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace CityAlert.Controllers
{
    public class HomeController : Controller
    {
        private readonly CityAlertDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(CityAlertDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IActionResult Index()
        {
            var eventsList = _context.Events.ToList();
            return View(eventsList);
        }
        
        public async Task<IActionResult> EventMap()
        {
            var events = await _context.Events
                .Include(e => e.District)
                .Where(e => e.IsActive)
                .ToListAsync();
            return View(events);
        }

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" });
        }

        [Authorize(Roles = "resident")]
        public async Task<IActionResult> MySubscriptions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subscriptions = await _context.Subscriptions
                .Include(s => s.District)
                .Where(s => s.UserId == userId)
                .ToListAsync();

            return View(subscriptions);
        }

        [Authorize(Roles = "moderator")]
        public IActionResult CreateEvents()
        {
            return RedirectToAction("Create", "Events");
        }

        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> AdminPanel()
        {
            var viewModel = new AdminPanelViewModel
            {
                TotalActiveEvents = await _context.Events.CountAsync(e => e.IsActive),

                AllEvents = await _context.Events
                    .Include(e => e.District)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync(),

                SubscriptionsByDistrict = await _context.Districts
                    .Select(d => new DistrictSubscriptionCount
                    {
                        DistrictName = d.Name,
                        Count = d.Subscriptions.Count
                    }).ToListAsync()
            };

            return View(viewModel);
        }
        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" }, "Cookies", "OpenIdConnect");
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