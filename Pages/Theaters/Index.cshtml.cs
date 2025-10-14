using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.Theaters
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Theater> Theaters { get; set; } = new List<Theater>();

        public async Task OnGetAsync()
        {
            Theaters = await _context.Theaters
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
    }
}