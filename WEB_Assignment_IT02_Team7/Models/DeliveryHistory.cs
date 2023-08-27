using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WEB_Assignment_IT02_Team7.Models
{
    public class DeliveryHistory
    {
        public int RecordID { get; set; }

        public int ParcelID { get; set; }

        public string? Description { get; set; }
    }
}
