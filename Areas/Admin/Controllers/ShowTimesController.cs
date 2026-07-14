using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Movie_Booking_App.Interfaces;
using Movie_Booking_App.Models;
using Movie_Booking_App.Services;

namespace Movie_Booking_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShowTimesController : Controller
    {
        private readonly IShowTimeRepository _showTimeRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ITheaterRepository _theaterRepository;
        private readonly IShowTimeService _showTimeService;
        private readonly ILogger<ShowTimesController> _logger;

        public ShowTimesController(
            IShowTimeRepository showTimeRepository,
            IMovieRepository movieRepository,
            ITheaterRepository theaterRepository,
            IShowTimeService showTimeService,
            ILogger<ShowTimesController> logger)
        {
            _showTimeRepository = showTimeRepository;
            _movieRepository = movieRepository;
            _theaterRepository = theaterRepository;
            _showTimeService = showTimeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var showTimes = await _showTimeRepository.GetAllWithDetailsAsync();
            return View(showTimes);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? movieId, int? theaterId)
        {
            await LoadDropdowns();

            var showTime = new ShowTime
            {
                MovieId = movieId ?? 0,
                TheaterId = theaterId ?? 0,
                ShowDateTime = DateTime.Now.AddHours(2)
            };

            // Round to nearest 15 minutes
            var minutes = showTime.ShowDateTime.Minute;
            var roundedMinutes = (minutes / 15) * 15;
            showTime.ShowDateTime = showTime.ShowDateTime.AddMinutes(roundedMinutes - minutes);

            return View(showTime);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShowTime showTime)
        {
            var validationErrors = await _showTimeService.ValidateShowTimeAsync(showTime);
            foreach (var error in validationErrors)
            {
                var memberName = error.MemberNames.FirstOrDefault() ?? string.Empty;
                ModelState.AddModelError(string.IsNullOrEmpty(memberName) ? string.Empty : $"ShowTime.{memberName}", error.ErrorMessage ?? string.Empty);
            }

            if (ModelState.IsValid)
            {
                showTime.AvailableSeats = showTime.TotalSeats;
                showTime.CreatedAt = DateTime.UtcNow;

                await _showTimeRepository.AddAsync(showTime);
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns();
            return View(showTime);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showTime = await _showTimeRepository.GetByIdWithDetailsAsync(id.Value);
            if (showTime == null)
            {
                return NotFound();
            }
            return View(showTime);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ShowTime showTime)
        {
            if (ModelState.IsValid)
            {
                var existingShowTime = await _showTimeRepository.GetByIdAsync(showTime.Id);
                if (existingShowTime == null)
                {
                    return NotFound();
                }

                // Update allowed fields
                existingShowTime.ShowDateTime = showTime.ShowDateTime;
                existingShowTime.TicketPrice = showTime.TicketPrice;
                existingShowTime.TotalSeats = showTime.TotalSeats;
                existingShowTime.AvailableSeats = showTime.AvailableSeats;
                existingShowTime.Screen = showTime.Screen;
                existingShowTime.Format = showTime.Format;
                existingShowTime.Language = showTime.Language;
                existingShowTime.IsActive = showTime.IsActive;

                if (existingShowTime.TotalSeats < existingShowTime.AvailableSeats)
                {
                    existingShowTime.AvailableSeats = existingShowTime.TotalSeats;
                }

                await _showTimeRepository.UpdateAsync(existingShowTime);
                return RedirectToAction(nameof(Index));
            }
            return View(showTime);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showTime = await _showTimeRepository.GetByIdWithDetailsAsync(id.Value);
            if (showTime == null)
            {
                return NotFound();
            }
            return View(showTime);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var showTime = await _showTimeRepository.GetByIdWithDetailsAsync(id);
            if (showTime != null)
            {
                if (showTime.Bookings.Any())
                {
                    ModelState.AddModelError(string.Empty, "Cannot delete show time with existing bookings.");
                    return View(showTime);
                }

                await _showTimeRepository.DeleteAsync(showTime);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            // Use all active movies (not just those with existing showtimes)
            var movies = await _movieRepository.GetActiveAsync();
            var theaters = await _theaterRepository.GetActiveAsync();

            ViewData["MovieId"] = new SelectList(movies, "Id", "Title");
            ViewData["TheaterId"] = new SelectList(theaters, "Id", "Name");
        }
    }
}
