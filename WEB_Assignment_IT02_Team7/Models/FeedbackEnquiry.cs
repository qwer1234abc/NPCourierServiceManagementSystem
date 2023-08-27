using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WEB_Assignment_IT02_Team7.Models
{
    public class FeedbackEnquiry
    {
        public int FeedbackEnquiryID { get; set; }

        public int MemberID { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(255, ErrorMessage = "Content cannot exceed 255 characters")]
        public string Content { get; set; }

        public DateTime DateTimePosted { get; set; }

        public int? StaffID { get; set; }

        [Required(ErrorMessage = "Response is required.")]
        [StringLength(255, ErrorMessage = "Response cannot exceed 255 characters")]
        public string? Response { get; set; }

        public char Status { get; set; } = '0';
    }
}
