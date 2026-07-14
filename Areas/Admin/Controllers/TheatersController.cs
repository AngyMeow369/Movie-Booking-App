using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Booking_App.Interfaces;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TheatersController : Controller
    {
        private readonly ITheaterRepository _theaterRepository;

        public TheatersController(ITheaterRepository theaterRepository)
        {
            _theaterRepository = theaterRepository;
        }

        public async Task<IActionResult> Index()
        {
            var theaters = await _theaterRepository.GetAllAsync();
            return View(theaters);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Theater());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Theater theater)
        {
            if (ModelState.IsValid)
            {
                await _theaterRepository.AddAsync(theater);
                return RedirectToAction(nameof(Index));
            }
            return View(theater);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theater = await _theaterRepository.GetByIdAsync(id.Value);
            if (theater == null)
            {
                return NotFound();
            }
            return View(theater);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Theater theater)
        {
            if (ModelState.IsValid)
            {
                if (!_theaterRepository.Exists(theater.Id))
                {
                    return NotFound();
                }
                await _theaterRepository.UpdateAsync(theater);
                return RedirectToAction(nameof(Index));
            }
            return View(theater);
        }
    }
}
