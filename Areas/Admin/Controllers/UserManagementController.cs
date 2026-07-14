using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserManagementController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

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

        public async Task<IActionResult> Index()
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

            var userList = users.Select(u => new UserViewModel
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

            return View(userList);
        }

        public async Task<IActionResult> UserBookings(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Theater)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            ViewBag.UserInfo = user;
            ViewBag.TotalBookings = bookings.Count;
            ViewBag.TotalTickets = bookings.Sum(b => b.NumberOfTickets);
            ViewBag.TotalSpent = bookings.Sum(b => b.TotalAmount);

            return View(bookings);
        }
    }
}
