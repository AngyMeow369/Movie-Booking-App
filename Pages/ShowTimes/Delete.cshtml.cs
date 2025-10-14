using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.ShowTimes
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ShowTime ShowTime { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showTime = await _context.ShowTimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showTime == null)
            {
                return NotFound();
            }
            ShowTime = showTime;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showTime = await _context.ShowTimes.FindAsync(id);
            if (showTime != null)
            {
                ShowTime = showTime;
                _context.ShowTimes.Remove(ShowTime);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}