using CSSmall.Data;
using CSSmall.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSSmall.Controllers
{
    public class RoomsController : Controller
        {
            private readonly HttpClient _httpClient;

            public RoomsController(IHttpClientFactory httpClientFactory)
            {
                _httpClient = httpClientFactory.CreateClient();
            }

        public async Task<IActionResult> Index(DateTime? checkIn, DateTime? checkOut, int adults = 1, int children = 0)
        {
            ViewData["CheckIn"] = checkIn;
            ViewData["CheckOut"] = checkOut;
            ViewData["Adults"] = adults;
            ViewData["Children"] = children;

            var apiUrl = "https://informatik1.ei.hv.se/RoomAPI/api/room";
            List<Room> rooms = new();

            try
                {
                    var response = await _httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        rooms = JsonConvert.DeserializeObject<List<Room>>(jsonData)
                            .Where(r => r.IsVacant) // Endast lediga rum
                            .ToList();
                    }
                    else
                    {
                        ViewBag.Error = "Kunde inte hämta rumsdata.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Fel vid API-anrop: {ex.Message}";
                }

            ViewData["CheckIn"] = checkIn;
            ViewData["CheckOut"] = checkOut;
            ViewData["Adults"] = adults;
            ViewData["Children"] = children;

            return View(rooms);
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
