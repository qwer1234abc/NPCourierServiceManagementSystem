using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WEB_Assignment_IT02_Team7.Models
{
    public class Parcel
    {
        [Display(Name = "Parcel ID")]
        public int ParcelID { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        [Display(Name = "Description")]
        public string? ItemDescription { get; set; }

        [Required(ErrorMessage = "Sender's name is required")]
        [StringLength(50, ErrorMessage = "Sender's name cannot exceed 50 characters")]
        [Display(Name = "Sender Name")]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "Sender's tel no is required")]
        [RegularExpression(@"^\+[0-9]{1,20}$", ErrorMessage = "Eg. +6512345678")]
        [Display(Name = "Sender Tel No")]
        public string SenderTelNo { get; set; }

        [Required(ErrorMessage = "Receiver's name is required")]
        [StringLength(50, ErrorMessage = "Receiver's name cannot exceed 50 characters")]
        [Display(Name = "Receiver Name")]
        public string ReceiverName { get; set; }

        [Required(ErrorMessage = "Receiver's tel no is required")]
        [RegularExpression(@"^\+[0-9]{1,20}$", ErrorMessage = "Eg. +6512345678")]
        [Display(Name = "Receiver Tel No")]
        public string ReceiverTelNo { get; set; }

        [Required(ErrorMessage = "Delivery address is required")]
        [StringLength(255, ErrorMessage = "Delivery address cannot exceed 255 characters")]
        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; }

        [StringLength(50)]
        [Display(Name = "From City")]
        public string FromCity { get; set; }

        [StringLength(50)]
        [Display(Name = "From Country")]
        public string FromCountry { get; set; }

        [StringLength(50)]
        [Display(Name = "To City")]
        public string ToCity { get; set; }

        [StringLength(50)]
        [Display(Name = "To Country")]
        public string ToCountry { get; set; }

        [Required(ErrorMessage = "Parcel weight is required")]
        [Range(0.01, 9999.99, ErrorMessage = "Parcel Weight >= 0.01kg and <= 10000kg")]
        [Display(Name = "Parcel Weight (kg)")]
        public double ParcelWeight { get; set; }

        [Display(Name = "Delivery Charge")]
        [Range(5, double.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double? DeliveryCharge { get; set; }

        [DataType(DataType.Currency)]
        [StringLength(3, ErrorMessage = "Currency code must be exactly 3 characters")]
        [RegularExpression(@"^[A-Za-z]{3}$", ErrorMessage = "Currency code must consist of 3 alphabetic characters")]
        [Display(Name = "Currency")]
        public string Currency { get; set; } = "SGD";

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Target Delivery Date")]
        public DateTime? TargetDeliveryDate { get; set; }

        public char DeliveryStatus { get; set; } = '0';

        public int? DeliveryManID { get; set; }

        [Range(5, double.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Unpaid Delivery Charge")]
        public double? UnpaidDeliveryCharge { get; set; }
    }
}
