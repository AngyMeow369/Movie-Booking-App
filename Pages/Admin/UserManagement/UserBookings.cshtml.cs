using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.Admin.UserManagement
{
    [Authorize(Roles = "Admin")]
    public class UserBookingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserBookingsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ApplicationUser UserInfo { get; set; } = default!;
        public List<Booking> Bookings { get; set; } = new();
        public UserStatsViewModel UserStats { get; set; } = new();

        public class UserStatsViewModel
        {
            public int TotalBookings { get; set; }
            public int TotalTickets { get; set; }
            public decimal TotalSpent { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("./Index");
            }

            UserInfo = await _userManager.FindByIdAsync(userId);
            if (UserInfo == null)
            {
                return NotFound();
            }

            // Load bookings with related data
            Bookings = await _context.Bookings
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Theater)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            // Calculate user stats
            UserStats = new UserStatsViewModel
            {
                TotalBookings = Bookings.Count,
                TotalTickets = Bookings.Sum(b => b.NumberOfTickets),
                TotalSpent = Bookings.Sum(b => b.TotalAmount)
            };

            return Page();
        }
    }
}