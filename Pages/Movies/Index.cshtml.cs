using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Movie> Movies { get; set; } = new List<Movie>();

        public async Task OnGetAsync()
        {
            Movies = await _context.Movies
                .OrderBy(m => m.Title)
                .ToListAsync();
        }
    }
}