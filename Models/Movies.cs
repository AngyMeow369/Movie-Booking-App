// Models/Movie.cs
using System;
using System.Collections.Generic;

namespace Movie_Booking_App.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Genre { get; set; } = default!;
        public int Duration { get; set; }
        public string Language { get; set; } = default!;
        public string PosterUrl { get; set; } = default!;
        public DateTime ReleaseDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    }
}