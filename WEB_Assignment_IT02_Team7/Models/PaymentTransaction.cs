using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WEB_Assignment_IT02_Team7.Models
{
    public class PaymentTransaction
    {
        public int TransactionID { get; set; }

        public int? ParcelID { get; set; }

        public double? AmtTran { get; set; }

        [DataType(DataType.Currency)]
        [StringLength(3, ErrorMessage = "Currency code must be exactly 3 characters")]
        [RegularExpression(@"^[A-Za-z]{3}$", ErrorMessage = "Currency code must consist of 3 alphabetic characters")]
        [Display(Name = "Currency")]
        public string Currency { get; set; } = "SGD";

        [StringLength(4)]
        public string TranType { get; set; } = "CASH";

        public DateTime TranDate { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Cash Amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cash Amount >= $0.01")]
        public double? CashAmt { get; set; }

        [Display(Name = "Voucher Amount")]
        public double? VoucherAmt { get; set; }

        [Display(Name = "Total Amount")]
        public double? TotalAmt { get; set; }
    }
}
