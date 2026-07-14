using Movie_Booking_App.Models;

namespace Movie_Booking_App.Interfaces
{
    public interface IShowTimeRepository
    {
        Task<IList<ShowTime>> GetAllWithDetailsAsync();
        Task<ShowTime?> GetByIdWithDetailsAsync(int id);
        Task<ShowTime?> GetByIdAsync(int id);
        Task<ShowTime?> FindOverlapAsync(int theaterId, DateTime dateTime, int? excludeId = null);
        Task AddAsync(ShowTime showTime);
        Task UpdateAsync(ShowTime showTime);
        Task DeleteAsync(ShowTime showTime);
        bool Exists(int id);
    }
}
