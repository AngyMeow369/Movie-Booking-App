using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Interfaces;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Repositories
{
    public class ShowTimeRepository : IShowTimeRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowTimeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<ShowTime>> GetAllWithDetailsAsync()
        {
            return await _context.ShowTimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .OrderBy(s => s.ShowDateTime)
                .ToListAsync();
        }

        public async Task<ShowTime?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.ShowTimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<ShowTime?> GetByIdAsync(int id)
        {
            return await _context.ShowTimes.FindAsync(id);
        }

        public async Task<ShowTime?> FindOverlapAsync(int theaterId, DateTime dateTime, int? excludeId = null)
        {
            var query = _context.ShowTimes
                .Include(st => st.Movie)
                .Where(st => st.TheaterId == theaterId)
                .Where(st => st.ShowDateTime >= dateTime.AddHours(-3) && st.ShowDateTime <= dateTime.AddHours(3));

            if (excludeId.HasValue)
            {
                query = query.Where(st => st.Id != excludeId.Value);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(ShowTime showTime)
        {
            _context.ShowTimes.Add(showTime);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ShowTime showTime)
        {
            _context.Entry(showTime).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ShowTime showTime)
        {
            _context.ShowTimes.Remove(showTime);
            await _context.SaveChangesAsync();
        }

        public bool Exists(int id)
        {
            return _context.ShowTimes.Any(e => e.Id == id);
        }
    }
}
