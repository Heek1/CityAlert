using CityAlert.Data;
using CityAlert.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CityAlert.Controllers
{
    [Authorize]
    public class SubscriptionsController : Controller
    {
        private readonly CityAlertDbContext _context;

        public SubscriptionsController(CityAlertDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "resident")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var subscribedDistrictIds = await _context.Subscriptions
                .Where(s => s.UserId == userId)
                .Select(s => s.DistrictId)
                .ToListAsync();

            var districtCounts = await _context.Subscriptions
                .GroupBy(s => s.DistrictId)
                .Select(g => new { DistrictId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DistrictId, x => x.Count);

            var districts = (await _context.Districts
                .OrderBy(d => d.Name)
                .ToListAsync())
                .Select(d => new DistrictSubscriptionItemViewModel
                {
                    DistrictId = d.Id,
                    DistrictName = d.Name,
                    IsSubscribed = subscribedDistrictIds.Contains(d.Id),
                    SubscriberCount = districtCounts.TryGetValue(d.Id, out var count) ? count : 0
                })
                .ToList();

            return View(new ResidentSubscriptionsViewModel
            {
                Districts = districts
            });
        }

        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> Manage()
        {
            var subscriptions = await _context.Subscriptions
                .Include(s => s.District)
                .OrderBy(s => s.District.Name)
                .ThenBy(s => s.UserId)
                .ToListAsync();

            var model = new AdminSubscriptionsViewModel
            {
                TotalSubscriptions = subscriptions.Count,
                UniqueUsersCount = subscriptions.Select(s => s.UserId).Distinct().Count(),
                SubscriptionsByDistrict = await _context.Districts
                    .OrderBy(d => d.Name)
                    .Select(d => new DistrictSubscriptionCount
                    {
                        DistrictName = d.Name,
                        Count = d.Subscriptions.Count
                    })
                    .ToListAsync(),
                AllSubscriptions = subscriptions.Select(s => new AdminSubscriptionItemViewModel
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    UserDisplayName = s.UserId,
                    DistrictName = s.District.Name
                }).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "resident")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(int districtId, string? returnUrl = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var districtExists = await _context.Districts.AnyAsync(d => d.Id == districtId);
            if (!districtExists)
            {
                return NotFound();
            }

            var exists = await _context.Subscriptions.AnyAsync(s => s.UserId == userId && s.DistrictId == districtId);
            if (!exists)
            {
                _context.Subscriptions.Add(new Subscription
                {
                    UserId = userId,
                    DistrictId = districtId
                });

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Підписку успішно оформлено.";
            }

            return RedirectToLocal(returnUrl);
        }

        [Authorize(Roles = "resident")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnsubscribeByDistrict(int districtId, string? returnUrl = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.DistrictId == districtId);

            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Підписку скасовано.";
            }

            return RedirectToLocal(returnUrl);
        }

        [Authorize(Roles = "moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sub = await _context.Subscriptions
                .Include(s => s.District)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sub != null)
            {
                _context.Subscriptions.Remove(sub);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Підписку для району «{sub.District.Name}» видалено.";
            }

            return RedirectToAction(nameof(Manage));
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
