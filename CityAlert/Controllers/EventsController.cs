using CityAlert.Data;
using CityAlert.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Text.Json;

namespace CityAlert.Controllers
{
    public class EventsController : Controller
    {
        private readonly CityAlertDbContext _context;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "Events_MainPage";

        public EventsController(CityAlertDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            string? cachedData = null;
            List<Event> events;

            try
            {
                cachedData = await _cache.GetStringAsync(CacheKey);
            }
            catch
            {
                cachedData = null;
            }

            if (string.IsNullOrEmpty(cachedData))
            {
                events = await _context.Events.Include(e => e.District)
                    .Where(e => e.IsActive)
                    .OrderByDescending(e => e.CreatedAt).ToListAsync();

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
                };

                try
                {
                    await _cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(events), options);
                }
                catch
                {
                }
            }
            else
            {
                events = JsonSerializer.Deserialize<List<Event>>(cachedData)!;
            }

            if (User.Identity?.IsAuthenticated == true && User.IsInRole("resident"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var subscribedDistrictIds = await _context.Subscriptions
                        .Where(s => s.UserId == userId)
                        .Select(s => s.DistrictId)
                        .ToListAsync();

                    ViewBag.SubscribedDistrictIds = subscribedDistrictIds;
                }
            }

            return View(events);
        }

        [Authorize(Roles = "moderator")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Districts = new SelectList(await _context.Districts.ToListAsync(), "Id", "Name");
            return View();
        }

        [Authorize(Roles = "moderator")]
        [HttpPost]
        public async Task<IActionResult> Create(Event newEvent)
        {
            ModelState.Remove(nameof(Event.District));

            if (ModelState.IsValid)
            {
                newEvent.CreatedAt = DateTime.Now;
                newEvent.CreatedBy = User.Identity?.Name ?? "Admin";

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();

                try { await _cache.RemoveAsync(CacheKey); } catch { }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Districts = new SelectList(await _context.Districts.ToListAsync(), "Id", "Name", newEvent.DistrictId);
            return View(newEvent);
        }

        [Authorize(Roles = "moderator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev is null)
                return NotFound();

            ViewBag.Districts = new SelectList(await _context.Districts.ToListAsync(), "Id", "Name", ev.DistrictId);
            return View(ev);
        }

        [Authorize(Roles = "moderator")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Event updated)
        {
            if (id != updated.Id)
                return NotFound();

            ModelState.Remove(nameof(Event.District));

            if (ModelState.IsValid)
            {
                var ev = await _context.Events.FindAsync(id);
                if (ev is null)
                    return NotFound();

                ev.Title = updated.Title;
                ev.Description = updated.Description;
                ev.Category = updated.Category;
                ev.Severity = updated.Severity;
                ev.DistrictId = updated.DistrictId;
                ev.StartDate = updated.StartDate;
                ev.EndDate = updated.EndDate;
                ev.IsActive = updated.IsActive;

                await _context.SaveChangesAsync();
                try { await _cache.RemoveAsync(CacheKey); } catch { }

                return RedirectToAction("AdminPanel", "Home");
            }

            ViewBag.Districts = new SelectList(await _context.Districts.ToListAsync(), "Id", "Name", updated.DistrictId);
            return View(updated);
        }

        [Authorize(Roles = "moderator")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev is not null)
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
                try { await _cache.RemoveAsync(CacheKey); } catch { }
            }

            return RedirectToAction("AdminPanel", "Home");
        }

        [Authorize(Roles = "moderator")]
        [HttpPost]
        public async Task<IActionResult> ClearCache()
        {
            try { await _cache.RemoveAsync(CacheKey); } catch { }
            return RedirectToAction("AdminPanel", "Home");
        }
    }
}
