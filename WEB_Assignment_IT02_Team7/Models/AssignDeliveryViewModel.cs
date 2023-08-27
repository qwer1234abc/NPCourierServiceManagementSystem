using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WEB_Assignment_IT02_Team7.Models
{
	public class AssignDeliveryViewModel
	{
		[Display(Name = "Parcel ID")]
		public int ParcelID { get; set; }

		[Display(Name = "Delivery Man ID")]
		public int DeliveryManID { get; set; }
	}
}
