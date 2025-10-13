// Models/ShowTime.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Movie_Booking_App.Models
{
    public class ShowTime
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public int TheaterId { get; set; }

        [Required]
        public DateTime ShowDateTime { get; set; }

        [Range(0.01, 1000.00)]
        public decimal TicketPrice { get; set; }

        [Range(1, 500)]
        public int TotalSeats { get; set; } = 100;

        public int AvailableSeats { get; set; } = 100;

        [StringLength(50)]
        public string Screen { get; set; } = "Screen 1";

        [StringLength(50)]
        public string Language { get; set; } = "English";

        [StringLength(20)]
        public string Format { get; set; } = "2D";

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Movie Movie { get; set; } = default!;
        public Theater Theater { get; set; } = default!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public bool HasAvailableSeats(int requestedSeats)
            => AvailableSeats >= requestedSeats && IsActive;

        public bool ReserveSeats(int numberOfSeats)
        {
            if (!HasAvailableSeats(numberOfSeats)) return false;
            AvailableSeats -= numberOfSeats;
            return true;
        }

        public void ReleaseSeats(int numberOfSeats)
        {
            AvailableSeats += numberOfSeats;
            if (AvailableSeats > TotalSeats) AvailableSeats = TotalSeats;
        }
    }
}