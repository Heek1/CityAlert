using CityAlert.Data;
using CityAlert.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CityAlert.Controllers
{
    [Authorize(Roles = "resident")]
    public class SubscriptionsController : Controller
    {
        private readonly CityAlertDbContext _context;

        public SubscriptionsController(CityAlertDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subscriptions = await _context.Subscriptions
                .Include(s => s.District)
                .Where(s => s.UserId == userId)
                .ToListAsync();

            return View(subscriptions);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int districtId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!_context.Subscriptions.Any(s => s.UserId == userId && s.DistrictId == districtId))
            {
                _context.Subscriptions.Add(new Subscription
                {
                    UserId = userId!,
                    DistrictId = districtId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe(int subscriptionId)
        {
            var sub = await _context.Subscriptions.FindAsync(subscriptionId);
            if (sub != null)
            {
                _context.Subscriptions.Remove(sub);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}