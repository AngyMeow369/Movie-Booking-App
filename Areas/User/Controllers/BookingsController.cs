using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Booking_App.Interfaces;
using Movie_Booking_App.Models;
using Movie_Booking_App.Services;

namespace Movie_Booking_App.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IShowTimeRepository _showTimeRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsController(
            IMovieRepository movieRepository,
            IShowTimeRepository showTimeRepository,
            IBookingRepository bookingRepository,
            IBookingService bookingService,
            UserManager<ApplicationUser> userManager)
        {
            _movieRepository = movieRepository;
            _showTimeRepository = showTimeRepository;
            _bookingRepository = bookingRepository;
            _bookingService = bookingService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Book(int id)
        {
            var movie = await _movieRepository.GetActiveByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var showtimes = await _showTimeRepository.GetAllWithDetailsAsync();
            var movieShowTimes = showtimes
                .Where(s => s.MovieId == id && s.IsActive && s.AvailableSeats > 0)
                .GroupBy(s => s.Theater)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.ShowDateTime).ToList());

            ViewBag.Movie = movie;
            ViewBag.GroupedShowTimes = movieShowTimes;
            ViewBag.MovieId = id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int movieId, int selectedShowTimeId, int numberOfTickets)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var result = await _bookingService.CreateBookingAsync(user.Id, selectedShowTimeId, numberOfTickets);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"🎉 Booking confirmed! Booking number: {result.Booking?.BookingNumber}";
                return RedirectToAction("Index", "Home", new { area = "User" });
            }

            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Book), new { id = movieId });
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var bookings = await _bookingRepository.GetByUserIdAsync(user.Id);
            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var result = await _bookingService.CancelBookingAsync(bookingId, user.Id, isAdmin: false);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(MyBookings));
        }
    }
}
