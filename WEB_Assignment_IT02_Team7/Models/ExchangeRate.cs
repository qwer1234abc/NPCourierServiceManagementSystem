using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WEB_Assignment_IT02_Team7.Models
{
    public class ExchangeRate
    {
        [JsonProperty("base")]
        [Display(Name = "From")]
        public string? Base { get; set; }

        [JsonProperty("target")]
        [Display(Name = "To")]
        public string? Target { get; set; }

        [JsonProperty("exchange_rates")]
        [Display(Name = "Exchange Rates")]
        public Dictionary<string, double>? ExchangeRates { get; set; }

        [JsonProperty("last_updated")]
        [Display(Name = "Last Updated On")]
        public long LastUpdated { get; set; }
    }
}
