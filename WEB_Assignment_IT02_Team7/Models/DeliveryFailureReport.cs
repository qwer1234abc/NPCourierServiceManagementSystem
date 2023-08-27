using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WEB_Assignment_IT02_Team7.Models
{
	public class DeliveryFailureReport
	{
		[Display(Name = "Report ID")]
		public int reportid { get; set; }

		[Display(Name = "Parcel ID")]
		public int parcelid { get; set; }

		[Display(Name = "DeliveryManID")]
		public int deliverymanid { get; set; }

		[Display(Name = "Failure Type")]
		[CharLength(1, ErrorMessage = "Failure Type can only include a single character.")]
		public char failuretype { get; set; }

		[Display(Name = "Description")]
		[StringLength(255, ErrorMessage = "Maximum description length is 255")]
		public string description { get; set; }

		[Display(Name = "Station Manager ID")]
		public int? stationmgrid { get; set; }

		[Display(Name = "Follow Up Action")]
		[StringLength(255, ErrorMessage = "Maximum string length is 255")]
		public string? followupaction { get; set; }

		[Display(Name = "Date Created")]
		public DateTime datecreated { get; set; }
	}

	public class CharLengthAttribute : ValidationAttribute
	{
		private readonly int _length;

		public CharLengthAttribute(int length)
		{
			_length = length;
		}
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value is char charValue)
            {
                if (charValue.ToString().Length <= _length)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult($"The field {validationContext.DisplayName} must be a char with a maximum length of {_length}.");
        }
    }
}
