using Movie_Booking_App.Interfaces;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Services
{
    public class BookingService : IBookingService
    {
        private readonly IShowTimeRepository _showTimeRepository;
        private readonly IBookingRepository _bookingRepository;

        public BookingService(
            IShowTimeRepository showTimeRepository,
            IBookingRepository bookingRepository)
        {
            _showTimeRepository = showTimeRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<(bool Succeeded, string Message, Booking? Booking)> CreateBookingAsync(string userId, int showTimeId, int numberOfTickets)
        {
            var showTime = await _showTimeRepository.GetByIdAsync(showTimeId);
            if (showTime == null)
            {
                return (false, "Showtime not found.", null);
            }

            if (!showTime.HasAvailableSeats(numberOfTickets))
            {
                return (false, "Not enough seats available.", null);
            }

            var booking = new Booking
            {
                UserId = userId,
                ShowTimeId = showTime.Id,
                NumberOfTickets = numberOfTickets,
                TotalAmount = showTime.TicketPrice * numberOfTickets,
                Status = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Completed
            };

            showTime.AvailableSeats -= numberOfTickets;
            await _showTimeRepository.UpdateAsync(showTime);
            await _bookingRepository.AddAsync(booking);

            return (true, "Booking confirmed.", booking);
        }

        public async Task<(bool Succeeded, string Message)> CancelBookingAsync(int bookingId, string userId, bool isAdmin = false)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null)
            {
                return (false, "Booking not found.");
            }

            if (!isAdmin && booking.UserId != userId)
            {
                return (false, "Unauthorized cancellation attempt.");
            }

            if (booking.Status != BookingStatus.Confirmed)
            {
                return (false, "Booking cannot be cancelled as it is not confirmed.");
            }

            // Return seats to showtime
            var showTime = booking.ShowTime;
            if (showTime != null)
            {
                showTime.AvailableSeats += booking.NumberOfTickets;
                if (showTime.AvailableSeats > showTime.TotalSeats)
                {
                    showTime.AvailableSeats = showTime.TotalSeats;
                }
                await _showTimeRepository.UpdateAsync(showTime);
            }

            booking.Status = BookingStatus.Cancelled;
            booking.PaymentStatus = PaymentStatus.Refunded;
            await _bookingRepository.UpdateAsync(booking);

            return (true, "Booking cancelled successfully.");
        }
    }
}
