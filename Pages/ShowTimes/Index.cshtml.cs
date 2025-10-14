using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.ShowTimes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

        public async Task OnGetAsync()
        {
            ShowTimes = await _context.ShowTimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .OrderBy(s => s.ShowDateTime)
                .ToListAsync();
        }
    }
}