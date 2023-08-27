using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WEB_Assignment_IT02_Team7.Models
{
    public class CashVoucher
    {
        [Display(Name = "Voucher ID")]
        [Range(1, int.MaxValue, ErrorMessage = "The Voucher ID >=1")]
        public int? CashVoucherID { get; set; }

        public int StaffID { get; set; }

        [Display(Name = "Voucher Amount")]
        public double Amount { get; set; }

        [DataType(DataType.Currency)]
        [StringLength(3, ErrorMessage = "Currency code must be exactly 3 characters")]
        [RegularExpression(@"^[A-Za-z]{3}$", ErrorMessage = "Currency code must consist of 3 alphabetic characters")]
        [Display(Name = "Currency")]
        public string Currency { get; set; } = "SGD";

        public char IssuingCode { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverTelNo { get; set; }

        public DateTime DateTimeIssued { get; set; }

        public string Status { get; set; } = "0";
    }
}
