using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.User.BookMovie
{
    [Authorize]
    public class BookMovieModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookMovieModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public int MovieId { get; set; }

        [BindProperty]
        public int SelectedShowTimeId { get; set; }

        [BindProperty]
        public int NumberOfTickets { get; set; } = 1;

        public Movie Movie { get; set; } = default!;
        public Dictionary<Theater, List<ShowTime>> GroupedShowTimes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            MovieId = id;

            Movie = await _context.Movies
                .Include(m => m.ShowTimes)
                .ThenInclude(s => s.Theater)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Movie == null)
                return NotFound();

            GroupedShowTimes = Movie.ShowTimes
                .Where(s => s.IsActive && s.AvailableSeats > 0)
                .GroupBy(s => s.Theater)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.ShowDateTime).ToList());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var showTime = await _context.ShowTimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(s => s.Id == SelectedShowTimeId);

            if (showTime == null || !showTime.HasAvailableSeats(NumberOfTickets))
            {
                ModelState.AddModelError(string.Empty, "Not enough seats available.");
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);

            var booking = new Booking
            {
                UserId = user.Id,
                ShowTimeId = showTime.Id,
                NumberOfTickets = NumberOfTickets,
                TotalAmount = showTime.TicketPrice * NumberOfTickets,
                Status = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Completed
            };

            showTime.AvailableSeats -= NumberOfTickets;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"🎉 Booking confirmed at {showTime.Theater.Name} for {showTime.Movie.Title}!";

            return RedirectToPage("/User/Index");
        }
    }
}
