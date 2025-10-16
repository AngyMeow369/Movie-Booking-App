using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;
using Microsoft.EntityFrameworkCore;

namespace Movie_Booking_App.Pages.User.MovieDetails
{
    public class MovieDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MovieDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Movie Movie { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (Movie == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
