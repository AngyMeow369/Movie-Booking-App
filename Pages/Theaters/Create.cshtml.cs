using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.Theaters
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Theater Theater { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Theaters.Add(Theater);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}