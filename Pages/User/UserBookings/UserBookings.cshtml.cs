
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;



namespace Movie_Booking_App.Pages.User.UserBookings
{
    [Authorize]
    public class UserBookingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public UserBookingsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Booking> Bookings { get; set; } = new List<Booking>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                Bookings = await _context.Bookings
                    .Include(b => b.ShowTime)
                        .ThenInclude(s => s.Movie)
                    .Include(b => b.ShowTime)
                        .ThenInclude(s => s.Theater)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostCancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.ShowTime)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking != null && booking.Status == BookingStatus.Confirmed)
            {
                // Cancel booking
                booking.Status = BookingStatus.Cancelled;

                // Return seats to showtime
                booking.ShowTime!.AvailableSeats += booking.NumberOfTickets;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
