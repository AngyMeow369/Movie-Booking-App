using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.Admin.ShowTimes
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

        public async Task<IActionResult> OnGetAsync(int? movieId, int? theaterId)
        {
            await LoadDropdowns();

            // Pre-select if IDs provided
            if (movieId.HasValue)
                ShowTime.MovieId = movieId.Value;
            if (theaterId.HasValue)
                ShowTime.TheaterId = theaterId.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Create debug file to see what's happening
            var debugInfo = new List<string>
    {
        $"=== DEBUG {DateTime.Now} ===",
        $"MovieId: {ShowTime.MovieId}",
        $"TheaterId: {ShowTime.TheaterId}",
        $"ShowDateTime: {ShowTime.ShowDateTime}",
        $"ModelState.IsValid: {ModelState.IsValid}",
        $"ModelState Error Count: {ModelState.ErrorCount}"
    };

            // Log all ModelState errors
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state.Errors.Count > 0)
                {
                    debugInfo.Add($"ERROR - {key}: {string.Join(", ", state.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            // Write to file immediately
            System.IO.File.WriteAllLines("debug_showtime.txt", debugInfo);

            Console.WriteLine("=== CREATE SHOWTIME ATTEMPT ===");
            Console.WriteLine($"STEP 1 - IMMEDIATELY AFTER BINDING:");
            Console.WriteLine($"  ShowTime.MovieId = {ShowTime.MovieId}");
            Console.WriteLine($"  ShowTime.TheaterId = {ShowTime.TheaterId}");
            Console.WriteLine($"  ModelState.IsValid = {ModelState.IsValid}");

            // Check ModelState errors BEFORE any validation
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state.Errors.Count > 0)
                {
                    Console.WriteLine($"  ModelState Error - {key}: {string.Join(", ", state.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            // Rest of your code...
            await ValidateShowTime();

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ MODEL VALIDATION FAILED!");

                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Count > 0)
                    {
                        Console.WriteLine($"Field: {key}");
                        foreach (var error in state.Errors)
                        {
                            Console.WriteLine($"  Error: {error.ErrorMessage}");
                        }
                    }
                }

                await LoadDropdowns();
                return Page();
            }

            try
            {
                // Set additional properties
                ShowTime.AvailableSeats = ShowTime.TotalSeats;
                ShowTime.CreatedAt = DateTime.UtcNow;

                Console.WriteLine("✅ VALIDATION PASSED - Creating ShowTime:");
                Console.WriteLine($"   MovieId: {ShowTime.MovieId}");
                Console.WriteLine($"   TheaterId: {ShowTime.TheaterId}");
                Console.WriteLine($"   ShowDateTime: {ShowTime.ShowDateTime}");
                Console.WriteLine($"   TicketPrice: {ShowTime.TicketPrice}");
                Console.WriteLine($"   TotalSeats: {ShowTime.TotalSeats}");

                _context.ShowTimes.Add(ShowTime);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ SUCCESS: ShowTime created with ID: {ShowTime.Id}");

                // Log success to file
                System.IO.File.AppendAllLines("debug_showtime.txt", new[] { $"SUCCESS: ShowTime created with ID: {ShowTime.Id}" });

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"❌ DATABASE ERROR: {dbEx.Message}");

                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"❌ INNER EXCEPTION: {dbEx.InnerException.Message}");

                    // Handle foreign key constraint violations
                    if (dbEx.InnerException.Message.Contains("FOREIGN KEY constraint"))
                    {
                        ModelState.AddModelError("", "Invalid Movie or Theater selected. Please verify your selection.");
                    }
                    else if (dbEx.InnerException.Message.Contains("UNIQUE") ||
                             dbEx.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError("", "A show time already exists for this movie and theater at the specified time.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Error creating show time. Please try again.");
                }

                await LoadDropdowns();
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ UNEXPECTED ERROR: {ex.Message}");
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                await LoadDropdowns();
                return Page();
            }
        }
        private async Task ValidateShowTime()
        {
            Console.WriteLine($"VALIDATION CHECK - MovieId: {ShowTime.MovieId}, TheaterId: {ShowTime.TheaterId}");

            bool hasErrors = false;

            // Check for valid IDs but DON'T return early
            if (ShowTime.MovieId <= 0)
            {
                ModelState.AddModelError("ShowTime.MovieId", "Please select a valid movie.");
                hasErrors = true;
            }

            if (ShowTime.TheaterId <= 0)
            {
                ModelState.AddModelError("ShowTime.TheaterId", "Please select a valid theater.");
                hasErrors = true;
            }

            // If basic validation failed, skip database checks
            if (hasErrors) return;

            // Validate Movie exists and is active
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == ShowTime.MovieId && m.IsActive);
            if (movie == null)
            {
                ModelState.AddModelError("ShowTime.MovieId", "Selected movie is not available.");
                hasErrors = true;
            }

            // Validate Theater exists and is active
            var theater = await _context.Theaters
                .FirstOrDefaultAsync(t => t.Id == ShowTime.TheaterId && t.IsActive);
            if (theater == null)
            {
                ModelState.AddModelError("ShowTime.TheaterId", "Selected theater is not available.");
                hasErrors = true;
            }

            // Only check other validations if movie and theater are valid
            if (!hasErrors)
            {
                // Validate against very distant future (optional)
                if (ShowTime.ShowDateTime > DateTime.Now.AddYears(1))
                {
                    ModelState.AddModelError("ShowTime.ShowDateTime", "Show time cannot be more than 1 year in the future.");
                }

                // Validate positive values
                if (ShowTime.TicketPrice <= 0)
                {
                    ModelState.AddModelError("ShowTime.TicketPrice", "Ticket price must be greater than 0.");
                }

                if (ShowTime.TotalSeats <= 0)
                {
                    ModelState.AddModelError("ShowTime.TotalSeats", "Total seats must be greater than 0.");
                }

                // Check for overlapping show times
                if (ShowTime.ShowDateTime > DateTime.Now)
                {
                    var existingShowTime = await _context.ShowTimes
                        .Include(st => st.Movie)
                        .Where(st => st.TheaterId == ShowTime.TheaterId)
                        .Where(st => st.ShowDateTime >= ShowTime.ShowDateTime.AddHours(-3) &&
                                     st.ShowDateTime <= ShowTime.ShowDateTime.AddHours(3))
                        .FirstOrDefaultAsync();

                    if (existingShowTime != null)
                    {
                        ModelState.AddModelError("ShowTime.ShowDateTime",
                            $"Theater is already booked for another show at {existingShowTime.ShowDateTime:g} " +
                            $"(Movie: {existingShowTime.Movie?.Title}). Please choose a different time.");
                    }
                }
            }
        }

        private async Task LoadDropdowns()
        {
            try
            {
                var movies = await _context.Movies
                    .Where(m => m.IsActive)
                    .OrderBy(m => m.Title)
                    .ToListAsync();

                var theaters = await _context.Theaters
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Name)
                    .ToListAsync();

                Console.WriteLine($"Dropdowns - Movies: {movies.Count}, Theaters: {theaters.Count}");

                ViewData["MovieId"] = new SelectList(movies, "Id", "Title");
                ViewData["TheaterId"] = new SelectList(theaters, "Id", "Name");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR LOADING DROPDOWNS: {ex.Message}");
                // Set empty lists to avoid null reference exceptions
                ViewData["MovieId"] = new SelectList(Enumerable.Empty<object>(), "Id", "Title");
                ViewData["TheaterId"] = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
            }
        }
    }
}