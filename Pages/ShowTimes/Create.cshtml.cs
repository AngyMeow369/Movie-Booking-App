using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.ShowTimes
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ShowTime ShowTime { get; set; } = new();

        public IActionResult OnGet(int? movieId, int? theaterId)
        {
            // Pre-select movie or theater if provided from the links
            if (movieId.HasValue)
            {
                ShowTime.MovieId = movieId.Value;
            }
            if (theaterId.HasValue)
            {
                ShowTime.TheaterId = theaterId.Value;
            }

            ViewData["MovieId"] = new SelectList(_context.Movies.Where(m => m.IsActive), "Id", "Title");
            ViewData["TheaterId"] = new SelectList(_context.Theaters.Where(t => t.IsActive), "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["MovieId"] = new SelectList(_context.Movies.Where(m => m.IsActive), "Id", "Title");
                ViewData["TheaterId"] = new SelectList(_context.Theaters.Where(t => t.IsActive), "Id", "Name");
                return Page();
            }

            // Set available seats equal to total seats initially
            ShowTime.AvailableSeats = ShowTime.TotalSeats;

            _context.ShowTimes.Add(ShowTime);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}