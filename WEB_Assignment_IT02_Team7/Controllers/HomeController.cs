using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using WEB_Assignment_IT02_Team7.Models;
using WEB_Assignment_IT02_Team7.DAL;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WEB_Assignment_IT02_Team7.Controllers
{
    public class HomeController : Controller
    {
        private CommonDAL commonContext = new CommonDAL();

        private readonly ILogger<HomeController> _logger;

        private List<SelectListItem> salutationList = new List<SelectListItem>()
        {
            new SelectListItem { Text = "Mr", Value = "Mr" },
            new SelectListItem { Text = "Mrs", Value = "Mrs" },
            new SelectListItem { Text = "Ms", Value = "Ms" },
            new SelectListItem { Text = "Mdm", Value = "Mdm" },
            new SelectListItem { Text = "Dr", Value = "Dr" },
        };

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public ActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult AboutUs()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult ContactUs()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult CommonTrackDelivery()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult LoginRegister()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult MemberLogin()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult StaffLogin()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult MemberRegister()
        {
            ViewData["SalutationList"] = salutationList;
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberRegister(Member member)
        {
            ViewData["SalutationList"] = salutationList;
            if (ModelState.IsValid)
            {
                member.MemberID = commonContext.MemberRegister(member);                
                return RedirectToAction("LoginRegister");
            }
            else
            {
                return View(member);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginStaff(IFormCollection formData)
        {
            string staffLoginId = formData["staffLoginId"].ToString().ToLower();
            string staffLoginPassword = formData["staffLoginPassword"].ToString();

            if (commonContext.StaffLogin(staffLoginId, staffLoginPassword, HttpContext))
            {
                if (HttpContext.Session.GetString("Role") == "Front Office Staff")
                {
                    return RedirectToAction("Index", "FrontOffice");
                }
                else if (HttpContext.Session.GetString("Role") == "Delivery Man")
                {
                    return RedirectToAction("Index", "DeliveryMan");
                }
                else if (HttpContext.Session.GetString("Role") == "Station Manager")
                {
                    return RedirectToAction("Index", "StationManager");
                }
            }
            TempData["Message"] = "Invalid Login Credentials!";
            return RedirectToAction("StaffLogin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginMember(IFormCollection formData)
        {
            string memberLoginEmail = formData["memberLoginEmail"].ToString().ToLower();
            string memberLoginPassword = formData["memberLoginPassword"].ToString();

            if (commonContext.MemberLogin(memberLoginEmail, memberLoginPassword, HttpContext))
            {
                return RedirectToAction("Index", "Customer");
            }

            TempData["Message"] = "Invalid Login Credentials!";
            return RedirectToAction("MemberLogin");
        }

        public async Task<ActionResult> ExchangeRates()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://exchange-rates.abstractapi.com");

            List<string> currencies = new List<string> { "USD", "EUR", "JPY", "MYR", "CNY", "GBP", "AUD" };

            List<ExchangeRate> conversionResults = new List<ExchangeRate>();

            string baseCurrency = "SGD";
            string apiKey = "74a6be1294ad4965a7e60e1c5643d470";
            string url = $"/v1/live?api_key={apiKey}&base={baseCurrency}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                ExchangeRate currencyRates = JsonConvert.DeserializeObject<ExchangeRate>(data);
                var exchangeRates = currencyRates.ExchangeRates;

                foreach (var currency in currencies)
                {
                    conversionResults.Add(new ExchangeRate
                    {
                        Base = baseCurrency,
                        Target = currency,
                        ExchangeRates = exchangeRates,
                        LastUpdated = currencyRates.LastUpdated
                    });
                }
            }
            else
            {
                return View(conversionResults);
            }

            return View(conversionResults);
        }

        public async Task<string>? GetIPAddress()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://ipgeolocation.abstractapi.com");

            string apiKey = "7d9f5a4ece0745fcb309540cd0872018";
            string url = $"/v1?api_key={apiKey}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(data);
                string ipAddress = (string)json["ip_address"];
                return ipAddress;
            }
            else
            {
                return null;
            }
        }

        public async Task<ActionResult> Weather()
        {
            string ipAddress = await GetIPAddress();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                Console.WriteLine(ipAddress);
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://api.weatherapi.com");

                string apiKey = "1711426e8f20455b8bb32725231707";
                int days = 8;
                string url = $"/v1/forecast.json?key={apiKey}&q={ipAddress}&days={days}&aqi=no&alerts=no";
                HttpResponseMessage weatherResponse = await client.GetAsync(url);

                if (weatherResponse.IsSuccessStatusCode)
                {
                    string data = await weatherResponse.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<dynamic>(data);
                    List<Weather> weatherList = new List<Weather>();

                    for (int i = 0; i < days; i++)
                    {
                        var forecast = results.forecast.forecastday[i];
                        Weather weather = new Weather()
                        {
                            Date = forecast.date,
                            City = results.location.name,
                            Country = results.location.country,
                            Temperature = forecast.day.avgtemp_c,
                            Condition = forecast.day.condition.text,
                            Precipitation = forecast.day.totalprecip_mm,
                            Humidity = forecast.day.avghumidity,
                            Cloud = forecast.day.avgvis_km,
                            WindSpeed = forecast.day.maxwind_kph,
                            UVIndex = forecast.day.uv,
                            WeatherImageURL = "http:" + forecast.day.condition.icon
                        };

                        weatherList.Add(weather);
                    }

                    return View(weatherList);
                }
                else
                {
                    return View(new List<Weather>());
                }
            }
            return View(new List<Weather>());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}