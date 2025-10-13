// Models/Booking.cs
using System;
using System.Collections.Generic;
using Movie_Booking_App.Models;   // for ApplicationUser, ShowTime enums

namespace Movie_Booking_App.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public int ShowTimeId { get; set; }
        public string BookingNumber { get; set; } = GenerateBookingNumber();
        public int NumberOfTickets { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Razorpay fields
        public string? RazorpayOrderId { get; set; }
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // Navigation
        public ApplicationUser User { get; set; } = default!;
        public ShowTime ShowTime { get; set; } = default!;

        private static string GenerateBookingNumber()
            => "BK" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
}