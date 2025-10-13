// Models/Theater.cs
using System;
using System.Collections.Generic;

namespace Movie_Booking_App.Models
{
    public class Theater
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string City { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    }
}