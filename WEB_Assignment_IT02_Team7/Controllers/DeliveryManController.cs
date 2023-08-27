using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;
using WEB_Assignment_IT02_Team7.DAL;
using WEB_Assignment_IT02_Team7.Models;
using System.Globalization;

namespace WEB_Assignment_IT02_Team7.Controllers
{
	public class DeliveryManController : Controller
	{
		//Create instance
		private DeliveryManDAL DeliveryManContext = new DeliveryManDAL();
		private List<string> FailureTypes = new List<string>()
		{
			"Receiver not found", "Wrong delivery address", "Parcel damaged", "Other"
		};

		//Redirect user back to Index - Home if user role != Delivery Man
		//Direct user to Index view if role is Delivery Man
		public ActionResult Index()
		{
			if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Delivery Man"))
			{
				return RedirectToAction("Index", "Home");
			}

			return View();
		}

		//Retrieve list of parcels assigned to delivery man and pass it to the AssignedParcelList
		public ActionResult AssignedParcels()
		{
			if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Delivery Man"))
			{
				return RedirectToAction("Index", "Home");
			}
			List<Parcel> AssignedParcelList = DeliveryManContext.GetParcelsForDeliveryMan(HttpContext.Session.GetInt32("StaffID"));
			return View(AssignedParcelList);
		}

		//If parcelID is null, redirect user to AssignedParcels
		//If not update the UpdateDeliveryStatus method of DeliveryManContext
		public ActionResult UpdateDeliveryStatus(Parcel parcel)
		{
			DeliveryHistory deliveryHistory = new DeliveryHistory();

			if (parcel.ParcelID == null)
			{
				RedirectToAction("AssignedParcels");
			}

			else if (parcel.ToCountry == parcel.FromCountry)
			{
				deliveryHistory.Description = $"Parcel delivered successfully by {HttpContext.Session.GetString("LoginID")} on {DateTime.Now.ToString("d MMM yyyy h:mmtt", CultureInfo.CreateSpecificCulture("en-GB"))}.";
				DeliveryManContext.UpdateDeliveryStatusLocal(parcel, deliveryHistory);
				return RedirectToAction("Index");
			}

			else if (parcel.ToCountry != parcel.FromCountry)
			{
				deliveryHistory.Description = $"Parcel delivered to airport by {HttpContext.Session.GetString("LoginID")} on {DateTime.Now.ToString("d MMM yyyy h:mmtt", CultureInfo.CreateSpecificCulture("en-GB"))}.";
				DeliveryManContext.UpdateDeliveryStatusLocal(parcel, deliveryHistory);
				DeliveryManContext.UpdateDeliveryStatusOverSeas(parcel, deliveryHistory);
				return RedirectToAction("Index");
			}

			return View(AssignedParcels);

		}

		public ActionResult FailDeliveryStatus(int? parcelID)
		{
			if (parcelID == null)
			{
				RedirectToAction("AssignedParcels");
			}

			DateTime date = DateTime.Now;
			DeliveryManContext.FailDeliveryStatus(parcelID);
			return RedirectToAction("Index");
		}

		//public ActionResult DeliverFailureReport()
		//{
		//    ViewData["FailureTypes"] = DFTypes();
		//    List<DeliveryFailureReport> deliveryFailureReports = DeliveryManContext.GetDeliveryFailureReport();
		//    return View(deliveryFailureReports);
		//}



		public ActionResult LogOut()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index", "Home");
		}

		private List<SelectListItem> DFTypes()
		{
			List<SelectListItem> FailureTypes = new List<SelectListItem>
			{
				new SelectListItem
				{
					Value = "0",
					Text = "Receiver not found"
				},
				new SelectListItem
				{
					Value = "1",
					Text = "Wrong delivery address"
				},
				new SelectListItem
				{
					Value = "2",
					Text = "Parcel damaged"
				},
				new SelectListItem
				{
					Value = "3",
					Text = "Other"
				},

			};
			return FailureTypes;
		}


		public ActionResult DeliverFailureReport()
		{
			if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Delivery Man"))
			{
				return RedirectToAction("Index", "Home");
			}

			List<DeliveryFailureReport> deliveryFailureReports = DeliveryManContext.GetDeliveryFailureReport();

			ViewData["NewFailureTypes"] = FailureTypes;
            ViewBag.FailureTypes = DFTypes();

            var model = new DeliveryFailureReport();
            model.failuretype = '0'; // Set a default failure type (optional)
            ViewBag.FailureTypes = DFTypes();
            return View(model);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeliverFailureReport(DeliveryFailureReport deliveryFailureReport)
		{
            ViewBag.FailureTypes = DFTypes();
			List<Parcel> failparcel = new List<Parcel>();
			failparcel = DeliveryManContext.GetFailedDelivery();
            List<char> failstatuslist = new List<char> { '1', '2', '3', '4' };
            char failstatus = '4';

            if (ModelState.IsValid)
			{
				if (DeliveryManContext.CheckDeliveryFailure(deliveryFailureReport.parcelid)==true)
				{
                    return RedirectToAction("Index");
                }


				foreach (var item in failparcel)
				{
					if (item.DeliveryStatus == failstatus)
					{
						if(item.ParcelID == deliveryFailureReport.parcelid)
						{
                            foreach (var failchar in failstatuslist)
                            {
                                if (failchar == deliveryFailureReport.failuretype)
                                {

                                    if (DeliveryManContext.CheckDeliveryFailure(deliveryFailureReport.parcelid) == false)
                                    {


                                        deliveryFailureReport.deliverymanid = Convert.ToInt32(HttpContext.Session.GetInt32("StaffID"));
                                        deliveryFailureReport.failuretype = deliveryFailureReport.failuretype.ToString()[0];
                                        deliveryFailureReport.datecreated = DateTime.Now;

                                        deliveryFailureReport.reportid = DeliveryManContext.AddDeliveryFailureReport(deliveryFailureReport);
                                        return RedirectToAction("Index");

                                    }
                                }
                            }
                        }
                    }

				}
            }
			return View("Index");

		}

	}
}
