using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;
using System.Linq;

namespace Movie_Booking_App.Pages.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<Booking> Bookings { get; set; } = new();

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return;

            UserName = user.UserName ?? string.Empty;
            Email = user.Email ?? string.Empty;

            // ✅ Fetch user's bookings + showtime + movie
            Bookings = _context.Bookings
                .Where(b => b.UserId == user.Id)
                .Include(b => b.ShowTime)          // include showtime data
                .ThenInclude(s => s.Movie)         // include movie data
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }
    }
}
