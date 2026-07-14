using Movie_Booking_App.Models;
using System.ComponentModel.DataAnnotations;

namespace Movie_Booking_App.Services
{
    public interface IShowTimeService
    {
        Task<IEnumerable<ValidationResult>> ValidateShowTimeAsync(ShowTime showTime, int? excludeId = null);
    }
}
