using CityAlert.Data;
using CityAlert.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityAlert.Controllers
{
    [Authorize(Roles = "moderator")]
    public class DistrictsController : Controller
    {
        private readonly CityAlertDbContext _context;

        public DistrictsController(CityAlertDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var districts = await _context.Districts
                .Select(d => new DistrictListItemViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    EventsCount = d.Events.Count,
                    SubscriptionsCount = d.Subscriptions.Count
                })
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(districts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(District district)
        {
            ModelState.Remove(nameof(District.Events));
            ModelState.Remove(nameof(District.Subscriptions));

            if (ModelState.IsValid)
            {
                var exists = await _context.Districts.AnyAsync(d => d.Name == district.Name);
                if (exists)
                {
                    ModelState.AddModelError(nameof(District.Name), "Район з такою назвою вже існує.");
                    return View(district);
                }

                _context.Districts.Add(district);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Район «{district.Name}» успішно створено.";
                return RedirectToAction(nameof(Index));
            }

            return View(district);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var district = await _context.Districts
                .Include(d => d.Events)
                .Include(d => d.Subscriptions)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (district is null)
                return NotFound();

            var eventsCount = district.Events.Count;
            var subscriptionsCount = district.Subscriptions.Count;

            _context.Districts.Remove(district);
            await _context.SaveChangesAsync();

            var details = new List<string>();
            if (eventsCount > 0) details.Add($"{eventsCount} подій");
            if (subscriptionsCount > 0) details.Add($"{subscriptionsCount} підписок");

            var suffix = details.Count > 0
                ? $" (разом з {string.Join(" та ", details)})"
                : "";

            TempData["SuccessMessage"] = $"Район «{district.Name}» видалено{suffix}.";
            return RedirectToAction(nameof(Index));
        }
    }
}