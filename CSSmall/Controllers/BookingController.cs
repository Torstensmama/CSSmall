using CSSmall.Data;
using CSSmall.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CSSmall.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create(int roomId, DateTime checkIn, DateTime checkOut, int adults, int children)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomID == roomId);

            if (room == null || !room.IsVacant)
            {
                return RedirectToAction("Index", "Rooms");
            }

            // Beräkna pris baserat på antal nätter
            int totalNights = (checkOut - checkIn).Days;
            decimal totalSum = totalNights * room.Price;

            var booking = new Booking
            {
                RoomID = roomId,
                StartDate = checkIn,
                EndDate = checkOut,
                AmountOfAdults = adults,
                AmountOfChildren = children,
                TotalSum = totalSum
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // Uppdaterar rummets status som upptaget
            room.IsVacant = false;
            _context.SaveChanges();

            return RedirectToAction("Confirmation", "Booking", new { bookingId = booking.BookingID });
        }

        public IActionResult Confirmation(int bookingId)
        {
            var booking = _context.Bookings
                .FirstOrDefault(b => b.BookingID == bookingId);

            if (booking == null)
            {
                return RedirectToAction("Index", "Rooms");
            }

            return View(booking);
        }
    }
}
