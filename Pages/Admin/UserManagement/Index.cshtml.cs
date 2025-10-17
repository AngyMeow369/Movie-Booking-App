using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Pages.Admin.UserManagement
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<UserViewModel> Users { get; set; } = new();

        public class UserViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? PhoneNumber { get; set; }
            public DateTime CreatedAt { get; set; }
            public int TotalBookings { get; set; }
            public decimal TotalSpent { get; set; }
            public DateTime? LastBookingDate { get; set; }
        }

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var userBookings = await _context.Bookings
                .GroupBy(b => b.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalBookings = g.Count(),
                    TotalSpent = g.Sum(b => b.TotalAmount),
                    LastBookingDate = g.Max(b => b.BookingDate)
                })
                .ToDictionaryAsync(x => x.UserId, x => x);

            Users = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty,
                PhoneNumber = u.PhoneNumber,
                CreatedAt = u.CreatedAt,
                TotalBookings = userBookings.ContainsKey(u.Id) ? userBookings[u.Id].TotalBookings : 0,
                TotalSpent = userBookings.ContainsKey(u.Id) ? userBookings[u.Id].TotalSpent : 0,
                LastBookingDate = userBookings.ContainsKey(u.Id) ? userBookings[u.Id].LastBookingDate : null
            }).ToList();
        }
    }
}