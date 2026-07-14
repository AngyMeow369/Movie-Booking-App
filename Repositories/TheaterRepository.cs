using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Interfaces;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Repositories
{
    public class TheaterRepository : ITheaterRepository
    {
        private readonly ApplicationDbContext _context;

        public TheaterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Theater>> GetAllAsync()
        {
            return await _context.Theaters.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<IList<Theater>> GetActiveAsync()
        {
            return await _context.Theaters.Where(t => t.IsActive).OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<Theater?> GetByIdAsync(int id)
        {
            return await _context.Theaters.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Theater?> GetActiveByIdAsync(int id)
        {
            return await _context.Theaters.FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
        }

        public async Task AddAsync(Theater theater)
        {
            _context.Theaters.Add(theater);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Theater theater)
        {
            _context.Attach(theater).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Theater theater)
        {
            _context.Theaters.Remove(theater);
            await _context.SaveChangesAsync();
        }

        public bool Exists(int id)
        {
            return _context.Theaters.Any(e => e.Id == id);
        }
    }
}
