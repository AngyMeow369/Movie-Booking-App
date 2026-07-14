using System.ComponentModel.DataAnnotations;
using Movie_Booking_App.Interfaces;
using Movie_Booking_App.Models;

namespace Movie_Booking_App.Services
{
    public class ShowTimeService : IShowTimeService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ITheaterRepository _theaterRepository;
        private readonly IShowTimeRepository _showTimeRepository;

        public ShowTimeService(
            IMovieRepository movieRepository,
            ITheaterRepository theaterRepository,
            IShowTimeRepository showTimeRepository)
        {
            _movieRepository = movieRepository;
            _theaterRepository = theaterRepository;
            _showTimeRepository = showTimeRepository;
        }

        public async Task<IEnumerable<ValidationResult>> ValidateShowTimeAsync(ShowTime showTime, int? excludeId = null)
        {
            var results = new List<ValidationResult>();

            if (showTime.MovieId <= 0)
            {
                results.Add(new ValidationResult("Please select a valid movie.", new[] { nameof(showTime.MovieId) }));
            }
            if (showTime.TheaterId <= 0)
            {
                results.Add(new ValidationResult("Please select a valid theater.", new[] { nameof(showTime.TheaterId) }));
            }

            var currentTime = DateTime.Now;
            var minimumAllowedTime = currentTime.AddHours(1);

            if (showTime.ShowDateTime.Date < DateTime.Today)
            {
                results.Add(new ValidationResult("Show date cannot be in the past. Please select today or a future date.", new[] { nameof(showTime.ShowDateTime) }));
            }
            else if (showTime.ShowDateTime < minimumAllowedTime)
            {
                results.Add(new ValidationResult($"Show time must be at least 1 hour from now. Earliest allowed: {minimumAllowedTime:MMM dd, yyyy hh:mm tt}", new[] { nameof(showTime.ShowDateTime) }));
            }

            if (results.Any()) return results;

            var movie = await _movieRepository.GetActiveByIdAsync(showTime.MovieId);
            if (movie == null)
            {
                results.Add(new ValidationResult("Selected movie is not available.", new[] { nameof(showTime.MovieId) }));
            }

            var theater = await _theaterRepository.GetActiveByIdAsync(showTime.TheaterId);
            if (theater == null)
            {
                results.Add(new ValidationResult("Selected theater is not available.", new[] { nameof(showTime.TheaterId) }));
            }

            if (results.Any()) return results;

            if (showTime.ShowDateTime > DateTime.Now.AddYears(1))
            {
                results.Add(new ValidationResult("Show time cannot be more than 1 year in the future.", new[] { nameof(showTime.ShowDateTime) }));
            }

            if (showTime.TicketPrice <= 0)
            {
                results.Add(new ValidationResult("Ticket price must be greater than 0.", new[] { nameof(showTime.TicketPrice) }));
            }

            if (showTime.TotalSeats <= 0)
            {
                results.Add(new ValidationResult("Total seats must be greater than 0.", new[] { nameof(showTime.TotalSeats) }));
            }

            if (showTime.ShowDateTime > DateTime.Now)
            {
                var existingShowTime = await _showTimeRepository.FindOverlapAsync(showTime.TheaterId, showTime.ShowDateTime, excludeId);
                if (existingShowTime != null)
                {
                    results.Add(new ValidationResult(
                        $"Theater is already booked for another show at {existingShowTime.ShowDateTime:g} (Movie: {existingShowTime.Movie?.Title}). Please choose a different time.",
                        new[] { nameof(showTime.ShowDateTime) }));
                }
            }

            return results;
        }
    }
}
