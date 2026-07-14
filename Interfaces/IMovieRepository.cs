using Movie_Booking_App.Models;

namespace Movie_Booking_App.Interfaces
{
    public interface IMovieRepository
    {
        Task<IList<Movie>> GetAllAsync();
        Task<IList<Movie>> GetActiveAsync();
    Task<IList<Movie>> GetActiveWithShowTimesAsync();
        Task<Movie?> GetByIdAsync(int id);
        Task<Movie?> GetActiveByIdAsync(int id);
        Task AddAsync(Movie movie);
        Task UpdateAsync(Movie movie);
        Task DeleteAsync(Movie movie);
        bool Exists(int id);
    }
}
