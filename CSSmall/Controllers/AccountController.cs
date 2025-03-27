﻿using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;

public class AccountController : Controller
{
    private readonly HttpClient _httpClient;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string Username, string Password)
    {
        var loginRequest = new
        {
            username = Username,
            passwordHash = Password
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://informatik2.ei.hv.se/LoginService/api/auth/login", jsonContent);
        var cookies = response.Headers.GetValues("Set-Cookie");

        System.Diagnostics.Debug.WriteLine("Cookies mottagna vid inloggning:");
        foreach (var cookie in cookies)
        {
            System.Diagnostics.Debug.WriteLine(cookie);
        }
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.ErrorMessage = "Fel användarnamn eller lösenord.";
            return View();
        }

        var responseBody = await response.Content.ReadAsStringAsync();

        var sessionData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);
        System.Diagnostics.Debug.WriteLine($"Session skapad: {HttpContext.Session.GetString("SessionID")}");
        if (sessionData == null || !sessionData.ContainsKey("sessionId") ||
            !sessionData.ContainsKey("userId") || !sessionData.ContainsKey("roleId"))
        {
            System.Diagnostics.Debug.WriteLine("Fel: sessionData saknar nödvändiga fält!");
            ViewBag.ErrorMessage = "Fel vid hantering av session.";
            return View();
        }

        string sessionId = sessionData["sessionId"].ToString();
        int userId = Convert.ToInt32(sessionData["userId"]);
        int roleId = Convert.ToInt32(sessionData["roleId"]);

        HttpContext.Session.SetString("SessionID", sessionId);
        HttpContext.Session.SetInt32("UserID", userId);
        HttpContext.Session.SetInt32("UserRole", roleId);
        HttpContext.Session.SetString("Username", Username);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // Skickar utloggningsbegäran till API:et
            var response = await _httpClient.PostAsync("https://informatik2.ei.hv.se/api/auth/logout", null);
        }
        catch
        {
            // Om API-anropet misslyckas, fortsätt ändå med att rensa sessionen
        }

        // Rensar sessionen och cookies oavsett om API-anropet lyckades eller inte
        HttpContext.Session.Clear();
        Response.Cookies.Delete("SessionID");

        // Omdirigera alltid till startsidan
        return RedirectToAction("Index", "Home");
    }

}
