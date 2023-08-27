using System.ComponentModel.DataAnnotations;
namespace WEB_Assignment_IT02_Team7.Models
{
    public class Member
    {
        [Display(Name = "Member ID")]
        [Key] public int MemberID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Member's name cannot exceed 50 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Salutation is required")]        
        [RegularExpression(@"^(Ms|Mdm|Mr|Dr)$", ErrorMessage = " e.g., Ms, Mdm, Mr, Dr")]
        [Display(Name = "Salutation")]
        public string Salutation { get; set; }

        [Required(ErrorMessage = "Tel no is required")]
        [RegularExpression(@"^\+[0-9]{1,20}$", ErrorMessage = "e.g., +6598765432")]
        [Display(Name = "Tel No")]
        public string TelNo { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        [ValidateEmailExists]
        public string EmailAddr { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(25, ErrorMessage = "Password cannot exceed 25 characters")]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
        public string Password { get; set; }

		[Required(ErrorMessage = "Confirm Password is required")]
		[StringLength(25, ErrorMessage = "Confirm Password cannot exceed 25 characters")]
		[DataType(DataType.Password)] // Specify that this is a password field
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")] // Ensure that the password and confirm password match
		[Display(Name = "Confirm Password")]
		public string ConfirmPassword { get; set; }

		[DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Date of Birth")]
        public DateTime? BirthDate { get; set; }
                
        [Display(Name = "City")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Country is required")]      
        [Display(Name = "Country")]
        public string Country { get; set; }
    }
}
