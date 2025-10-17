using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

using Movie_Booking_App.Models;

namespace Movie_Booking_App.Data

{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)

            : base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Theater> Theaters { get; set; }

        public DbSet<ShowTime> ShowTimes { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)

        {

            base.OnModelCreating(builder);

            // Configure decimal precision for Booking.TotalAmount

            builder.Entity<Booking>()

                .Property(b => b.TotalAmount)

                .HasPrecision(18, 2); // 18 total digits, 2 decimal places

            // Configure decimal precision for ShowTime.TicketPrice

            builder.Entity<ShowTime>()

                .Property(s => s.TicketPrice)

                .HasPrecision(18, 2); // 18 total digits, 2 decimal places

            // Your existing configurations

            builder.Entity<ShowTime>()

                .HasOne(st => st.Movie)

                .WithMany(m => m.ShowTimes)

                .HasForeignKey(st => st.MovieId)

                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ShowTime>()

                .HasOne(st => st.Theater)

                .WithMany(t => t.ShowTimes)

                .HasForeignKey(st => st.TheaterId)

                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()

                .HasIndex(b => b.BookingNumber)

                .IsUnique();

        }

    }

}
