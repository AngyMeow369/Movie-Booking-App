using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<Booking> Bookings { get; set; } = new();
        public List<Movie> AvailableMovies { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            FullName = user.FullName;
            Email = user.Email;

            Bookings = await _context.Bookings
                .Include(b => b.ShowTime)
                .ThenInclude(s => s.Movie)
                .Include(b => b.ShowTime)
                .ThenInclude(s => s.Theater)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            AvailableMovies = await _context.Movies
                .Include(m => m.ShowTimes)
                .ThenInclude(s => s.Theater)
                .Where(m => m.IsActive && m.ShowTimes.Any(st => st.IsActive && st.AvailableSeats > 0)).OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.ShowTime)         // include showtime
                .ThenInclude(s => s.Movie)        // include movie inside showtime
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.Status != BookingStatus.Confirmed)
            {
                TempData["ErrorMessage"] = "Booking cannot be cancelled.";
                return RedirectToPage();
            }

            // Release seats
            booking.ShowTime.AvailableSeats += booking.NumberOfTickets;
            booking.Status = BookingStatus.Cancelled;
            booking.PaymentStatus = PaymentStatus.Refunded; // Optional

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Booking for {booking.ShowTime.Movie.Title} has been cancelled.";
            return RedirectToPage();
        }

    }
}
