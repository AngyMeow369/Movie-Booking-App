using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.Admin.ShowTimes
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

            // ✅ FIX: Include Bookings to handle cascade delete or check for existing bookings
            var showTime = await _context.ShowTimes
                .Include(s => s.Bookings)  // Add this line
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showTime != null)
            {
                // ✅ Check if there are existing bookings
                if (showTime.Bookings.Any())
                {
                    ModelState.AddModelError("", "Cannot delete show time with existing bookings.");
                    return await OnGetAsync(id); // Reload the page with error
                }

                _context.ShowTimes.Remove(showTime);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }


    }
}