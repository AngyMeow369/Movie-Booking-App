using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.Admin.ShowTimes
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ✅ FIX: Load existing showtime and update only allowed fields
            var existingShowTime = await _context.ShowTimes.FindAsync(ShowTime.Id);
            if (existingShowTime == null)
            {
                return NotFound();
            }

            // ✅ Update only the fields that are allowed to change
            existingShowTime.ShowDateTime = ShowTime.ShowDateTime;
            existingShowTime.TicketPrice = ShowTime.TicketPrice;
            existingShowTime.TotalSeats = ShowTime.TotalSeats;
            existingShowTime.AvailableSeats = ShowTime.AvailableSeats;
            existingShowTime.Screen = ShowTime.Screen;
            existingShowTime.Format = ShowTime.Format;
            existingShowTime.Language = ShowTime.Language;
            existingShowTime.IsActive = ShowTime.IsActive;

            // ✅ Recalculate available seats if total seats changed
            if (existingShowTime.TotalSeats < existingShowTime.AvailableSeats)
            {
                existingShowTime.AvailableSeats = existingShowTime.TotalSeats;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowTimeExists(ShowTime.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ShowTimeExists(int id)
        {
            return _context.ShowTimes.Any(e => e.Id == id);
        }
    }
}