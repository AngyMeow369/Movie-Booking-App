using Movie_Booking_App.Models;

namespace Movie_Booking_App.Services
{
    public interface IBookingService
    {
        Task<(bool Succeeded, string Message, Booking? Booking)> CreateBookingAsync(string userId, int showTimeId, int numberOfTickets);
        Task<(bool Succeeded, string Message)> CancelBookingAsync(int bookingId, string userId, bool isAdmin = false);
    }
}
