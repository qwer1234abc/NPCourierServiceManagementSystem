using System.ComponentModel.DataAnnotations;
using WEB_Assignment_IT02_Team7.DAL;
namespace WEB_Assignment_IT02_Team7.Models
{
    public class ValidateEmailExists : ValidationAttribute
    {
        private CommonDAL commonContext = new CommonDAL();
        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            string email = Convert.ToString(value);
            Member member = (Member)validationContext.ObjectInstance;
            int memberID = member.MemberID;
            if (commonContext.IsEmailExist(email, memberID))
                return new ValidationResult
                ("Email address already exists!");
            else
                return ValidationResult.Success;
        }
    }
}