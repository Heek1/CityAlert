using CityAlert.Data;
using CityAlert.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
            string? cachedData = await _cache.GetStringAsync(CacheKey);
            List<Event> events;

            if (string.IsNullOrEmpty(cachedData))
            {
                events = await _context.Events.Include(e => e.District)
                    .Where(e => e.IsActive)
                    .OrderByDescending(e => e.CreatedAt).ToListAsync();

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
                };

                await _cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(events), options);
            }
            else
            {
                events = JsonSerializer.Deserialize<List<Event>>(cachedData)!;
            }

            return View(events);
        }

        [Authorize(Roles = "moderator")]
        [HttpGet]
        public IActionResult Create() => View();

        [Authorize(Roles = "moderator")]
        [HttpPost]
        public async Task<IActionResult> Create(Event newEvent)
        {
            if (ModelState.IsValid)
            {
                newEvent.CreatedAt = DateTime.Now;
                newEvent.CreatedBy = User.Identity?.Name ?? "Admin";

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();

                await _cache.RemoveAsync(CacheKey);

                return RedirectToAction(nameof(Index));
            }
            return View(newEvent);
        }

        [Authorize(Roles = "moderator")]
        [HttpPost]
        public async Task<IActionResult> ClearCache()
        {
            await _cache.RemoveAsync(CacheKey);
            return RedirectToAction("AdminPanel", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) { 
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete == null) return NotFound();
            return View(eventToDelete);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete == null) return NotFound();

            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();

            await _cache.RemoveAsync(CacheKey);

            return RedirectToAction(nameof(Index));
        }
    }
}