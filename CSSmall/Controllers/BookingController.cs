using CSSmall.Data;
using CSSmall.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSSmall.Controllers
{
    public class BookingController : Controller
    {
        private readonly HttpClient _httpClient;

        public BookingController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int roomId, string customerName, DateTime checkIn, DateTime checkOut, int adults, int children)
        {
            Console.WriteLine($"roomId som skickades: {roomId}");
            System.Diagnostics.Debug.WriteLine($"Vuxna: {adults}");
            if (roomId <= 0)
            {
                return RedirectToAction("Error", "Booking", new { errorMessage = "Ogiltigt roomId." });
            }

            var roomApiUrl = $"https://informatik1.ei.hv.se/RoomAPI/api/room/{roomId}";
            decimal roomPrice = 0;

            try
            {
                var roomResponse = await _httpClient.GetAsync(roomApiUrl);
                var roomData = await roomResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"RoomAPI Response: {roomData}");
                System.Diagnostics.Debug.WriteLine($"RoomAPI Response: {roomData}");



                if (roomResponse.IsSuccessStatusCode)
                {
                    var room = JsonConvert.DeserializeObject<Room>(roomData);  // ✅ Korrekt för ett objekt

                    if (room != null && room.RoomID == roomId)
                    {
                        roomPrice = room.Price;
                    }
                    else
                    {
                        return RedirectToAction("Error", "Booking", new { errorMessage = "Kunde inte hitta rummet i RoomAPI." });
                    }

                }
            }
            catch (Exception ex)

            {
                System.Diagnostics.Debug.WriteLine($"Felmeddelande: {ex.Message}");
                return RedirectToAction("Error", "Booking", new { errorMessage = $"Fel vid hämtning av pris: {ex.Message}" });
            }

            decimal totalSum = (checkOut - checkIn).Days * roomPrice;

            var bookingRequest = new
            {
                customerName = customerName,
                bookingID = new Random().Next(1, 1000),
                guestID = new Random().Next(1, 1000),
                roomID = roomId,
                startDate = checkIn,
                endDate = checkOut,
                adults = adults,
                children = children,
                totalSum = totalSum
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(bookingRequest), Encoding.UTF8, "application/json");
            var bookingApiUrl = "https://informatik1.ei.hv.se/BookingAPI/api/Booking";

            try
            {
                var bookingResponse = await _httpClient.PostAsync(bookingApiUrl, jsonContent);
                var bookingResponseData = await bookingResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"BookingAPI Response: {bookingResponseData}");
                System.Diagnostics.Debug.WriteLine($"roomApiUrl: {roomApiUrl}");
                
                if (!bookingResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("Error", "Booking", new { errorMessage = "Kunde inte skapa bokningen." });
                }

                var roomUpdateRequest = new
                {
                    roomID = roomId,
                    isVacant = false
                };

                var roomUpdateContent = new StringContent(JsonConvert.SerializeObject(roomUpdateRequest), Encoding.UTF8, "application/json");
                var roomUpdateResponse = await _httpClient.PutAsync(roomApiUrl, roomUpdateContent);

                var roomUpdateResponseData = await roomUpdateResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"roomUpdateResponse Body: {roomUpdateResponseData}");

                System.Diagnostics.Debug.WriteLine($"roomUpdateRequest: {JsonConvert.SerializeObject(roomUpdateRequest)}");
                System.Diagnostics.Debug.WriteLine($"roomUpdateResponse Status Code: {roomUpdateResponse.StatusCode}");
                

                //if (!roomUpdateResponse.IsSuccessStatusCode)
                //{
                //    return RedirectToAction("Error", "Booking", new { errorMessage = "Kunde inte uppdatera rummets status." });
                //}

                // Skicka bokningsuppgifter till Confirmation-sidan
                return RedirectToAction("Confirmation", new
                {
                   
                    customerName = customerName,
                    checkIn = checkIn.ToString("yyyy-MM-dd"),
                    checkOut = checkOut.ToString("yyyy-MM-dd"),
                    adults = adults,
                    children = children,
                    totalSum = totalSum
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Booking", new { errorMessage = $"Fel vid bokning: {ex.Message}" });
            }
        }

        [HttpGet("Booking/Confirmation")]
        public IActionResult Confirmation(
            [FromQuery] int roomId,
            [FromQuery] int bookingID,
            [FromQuery] string customerName,
            [FromQuery] string checkIn,
            [FromQuery] string checkOut,
            [FromQuery] int adults,
            [FromQuery] int children,
            [FromQuery] decimal totalSum)
        {
            ViewData["RoomId"] = roomId;
            ViewData["BookingId"] = bookingID;
            ViewData["CustomerName"] = customerName;
            ViewData["CheckIn"] = checkIn;
            ViewData["CheckOut"] = checkOut;
            ViewData["Adults"] = adults;
            ViewData["Children"] = children;
            ViewData["TotalSum"] = totalSum;

            return View();
        }

        public IActionResult Error(string errorMessage)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = errorMessage
            };

            return View(errorViewModel);
        }
    }
}
