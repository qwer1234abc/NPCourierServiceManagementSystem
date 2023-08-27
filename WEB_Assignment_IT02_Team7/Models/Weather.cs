using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace WEB_Assignment_IT02_Team7.Models
{
    public class Weather
    {
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "name")]
        public string? City { get; set; }

        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Display(Name = "Temperature (°C)")]
        public double? Temperature { get; set; }

        [Display(Name = "Condition")]
        public string? Condition { get; set; }

        [Display(Name = "Precipitation (mm)")]
        public double? Precipitation { get; set; }

        [Display(Name = "Humidity (%)")]
        public double? Humidity { get; set; }

        [Display(Name = "Cloud (%)")]
        public double? Cloud { get; set; }

        [Display(Name = "Wind Speed (km/h)")]
        public double? WindSpeed { get; set; }

        [Display(Name = "UV Index")]
        public double? UVIndex { get; set; }

        public string? WeatherImageURL { get; set; }
    }
}
