using Movie_Booking_App.Models;

namespace Movie_Booking_App.Interfaces
{
    public interface ITheaterRepository
    {
        Task<IList<Theater>> GetAllAsync();
        Task<IList<Theater>> GetActiveAsync();
        Task<Theater?> GetByIdAsync(int id);
        Task<Theater?> GetActiveByIdAsync(int id);
        Task AddAsync(Theater theater);
        Task UpdateAsync(Theater theater);
        Task DeleteAsync(Theater theater);
        bool Exists(int id);
    }
}
