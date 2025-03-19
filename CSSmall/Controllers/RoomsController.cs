using CSSmall.Data;
using CSSmall.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CSSmall.Controllers
{
    public class RoomsController : Controller
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? checkIn, DateTime? checkOut, int guests = 1)
        {
            // Hämta lediga rum
            var availableRooms = _context.Rooms
                .Where(r => r.IsVacant)
                .ToList();

            if (checkIn != null && checkOut != null)
            {
                // Hämta alla bokningar som överlappar med önskade datum
                var bookedRooms = _context.Bookings
                    .Where(b =>
                        (b.StartDate < checkOut && b.EndDate > checkIn))
                    .Select(b => b.RoomID)
                    .ToList();

                // Filtrera bort de rum som är upptagna
                availableRooms = availableRooms
                    .Where(r => !bookedRooms.Contains(r.RoomID))
                    .ToList();
            }

            return View(availableRooms);
        }

        public IActionResult DetailsStandard()
        {
            return View();
        }

        public IActionResult DetailsDeluxe()
        {
            return View();
        }

        public IActionResult DetailsSvit()
        {
            return View();
        }
    }
}
