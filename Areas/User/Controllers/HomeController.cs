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
    public class HomeController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            IMovieRepository movieRepository,
            IBookingRepository bookingRepository,
            IBookingService bookingService,
            UserManager<ApplicationUser> userManager)
        {
            _movieRepository = movieRepository;
            _bookingRepository = bookingRepository;
            _bookingService = bookingService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            ViewBag.FullName = user.FullName;
            ViewBag.Email = user.Email;

            var bookings = await _bookingRepository.GetByUserIdAsync(user.Id);
            var availableMovies = await _movieRepository.GetActiveWithShowTimesAsync();

            ViewBag.Bookings = bookings;
            ViewBag.AvailableMovies = availableMovies;

            return View();
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

            return RedirectToAction(nameof(Index));
        }
    }
}
