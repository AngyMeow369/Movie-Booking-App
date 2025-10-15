using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        //DI implementation
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalMovies { get; set; }
        public int TotalTheaters { get; set; }
        public int TotalUsers { get; set; }
        public int TotalBookings { get; set; }
        public int TotalShowTimes { get; set; }

        public async Task OnGetAsync()
        {
            TotalMovies = await _context.Movies.CountAsync();
            TotalTheaters = await _context.Theaters.CountAsync();
            TotalUsers = await _context.Users.CountAsync();
            TotalBookings = await _context.Bookings.CountAsync();
            TotalShowTimes = await _context.ShowTimes.CountAsync();
        }
    }
}
