using Microsoft.EntityFrameworkCore;
using Movie_Booking_App.Data;
using Movie_Booking_App.Interfaces;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Movie>> GetAllAsync()
        {
            return await _context.Movies.OrderBy(m => m.Title).ToListAsync();
        }

        public async Task<IList<Movie>> GetActiveAsync()
        {
            return await _context.Movies
                .Where(m => m.IsActive)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IList<Movie>> GetActiveWithShowTimesAsync()
        {
            return await _context.Movies
                .Include(m => m.ShowTimes)
                .ThenInclude(s => s.Theater)
                .Where(m => m.IsActive && m.ShowTimes.Any(st => st.IsActive && st.AvailableSeats > 0))
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<Movie?> GetByIdAsync(int id)
        {
            return await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Movie?> GetActiveByIdAsync(int id)
        {
            return await _context.Movies.FirstOrDefaultAsync(m => m.Id == id && m.IsActive);
        }

        public async Task AddAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Attach(movie).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Movie movie)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }

        public bool Exists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
