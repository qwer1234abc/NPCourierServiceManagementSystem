using System.ComponentModel.DataAnnotations;

namespace WEB_Assignment_IT02_Team7.Models
{
    public class ParcelViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Parcel ID >= 1")]

        public int? SearchedParcelID { get; set; }

        [StringLength(50, ErrorMessage = "Customer's name cannot exceed 50 characters")]
        public string? SearchedCustomerName { get; set; }
        public List<Parcel>? ParcelList { get; set; }

        public ParcelViewModel()
        {
            ParcelList = new List<Parcel>();
        }
    }
}
