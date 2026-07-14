using Movie_Booking_App.Models;

namespace Movie_Booking_App.Interfaces
{
    public interface IBookingRepository
    {
        Task<IList<Booking>> GetByUserIdAsync(string userId);
        Task<IList<Booking>> GetAllWithDetailsAsync();
        Task<Booking?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
    }
}
