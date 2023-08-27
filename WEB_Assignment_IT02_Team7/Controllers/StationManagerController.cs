using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_Assignment_IT02_Team7.Models;
using WEB_Assignment_IT02_Team7.DAL;
using System.Globalization;
using System.Xml.Linq;

namespace WEB_Assignment_IT02_Team7.Controllers
{
    public class StationManagerController : Controller
    {
        private StationManagerDAL StationManagerContext = new StationManagerDAL();

        public ActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Station Manager"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

		public ActionResult AssignParcel()
		{
			if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Station Manager"))
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AssignParcel(AssignDeliveryViewModel parcel)
		{
            int count = StationManagerContext.DeliveryManParcel(parcel.DeliveryManID);
			int id = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));

            List<Staff> dmlist = new List<Staff>();
            dmlist = StationManagerContext.GetDeliveryMan();

			if (ModelState.IsValid)
			{
				if (count <=5 )
				{
                    if (count == 5)
                    {
                        return View();
                    }

                    foreach(var item in dmlist)
                    {
                        if(item.StaffID == parcel.DeliveryManID)
                        {
                            if (StationManagerContext.CheckAssign(parcel.ParcelID))
                            {
                                DeliveryHistory deliveyHistory = new DeliveryHistory();
                                deliveyHistory.ParcelID = parcel.ParcelID;
                                deliveyHistory.Description = "Received by StationMgrSG on " + DateTime.Now.ToString("d MMMM yyyy h:mmtt");
                                StationManagerContext.AssignParcel(parcel);
                                deliveyHistory.RecordID = StationManagerContext.AddDeliveryHistory(deliveyHistory);
                                return RedirectToAction("Index");
                            }
                            return View(parcel);
                        }
                    }
				}

				if (StationManagerContext.CheckAssign(parcel.ParcelID))
				{
					TempData["ErrorMessage"] = "Error!";
					return View(parcel);
				}

				return View();
			}

			else
			{
				return View(parcel);
			}
		}



		public ActionResult DeliveryInformation()
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Station Manager"))
            {
                return RedirectToAction("Index", "Home");
            }

            ParcelViewModel parcelVM = new ParcelViewModel();
            parcelVM.ParcelList = null;
            return View(parcelVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeliveryInformation(ParcelViewModel parcelVM, string submitButton)
        {
            parcelVM.ParcelList = null;

            if (submitButton == "SearchParcel")
            {
                if (parcelVM.SearchedParcelID != null)
                {
                    parcelVM.ParcelList = StationManagerContext.GetParcelDetailsByID(parcelVM.SearchedParcelID);
                }
                else
                {
                    ViewData["Error"] = true;
                }
            }
            else if (submitButton == "SearchCustomer")
            {
                if (parcelVM.SearchedCustomerName != null)
                {
                    parcelVM.ParcelList = StationManagerContext.GetParcelDetailsByCustomerName(parcelVM.SearchedCustomerName);
                }
                else
                {
                    ViewData["Error"] = true;
                }
            }

            return View(parcelVM);
        }

        public ActionResult ViewDeliveryHistory(int? parcelID)
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Station Manager"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (parcelID == null)
            {
                return RedirectToAction("DeliveryInformation");
            }

            List<DeliveryHistory> deliveryHistoryList = StationManagerContext.GetDeliveryHistories(Convert.ToInt32(parcelID));
            return View(deliveryHistoryList);
        }

        public ActionResult RespondFeedback()
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Station Manager"))
            {
                return RedirectToAction("Index", "Home");
            }

            List<FeedbackEnquiry> feedbackEnquiryList = StationManagerContext.GetAllFeedbackEnquiries();
            return View(feedbackEnquiryList);
        }

        public ActionResult CreateResponse(int? feedbackEnquiryID)
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Station Manager"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (feedbackEnquiryID == null)
            {
                return RedirectToAction("RespondFeedback");
            }

            FeedbackEnquiry feedbackEnquiry = StationManagerContext.GetFeedbackEnquiryDetails(Convert.ToInt32(feedbackEnquiryID));
            return View(feedbackEnquiry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateResponse(FeedbackEnquiry feedbackEnquiry, int? feedbackEnquiryID)
        {
            feedbackEnquiry.FeedbackEnquiryID = Convert.ToInt32(feedbackEnquiryID);

            if (ModelState.IsValid)
            {
                feedbackEnquiry.Status = '1';
                feedbackEnquiry.StaffID = Convert.ToInt32(HttpContext.Session.GetInt32("StaffID"));
                StationManagerContext.UpdateFeedbackEnquiryResponse(feedbackEnquiry);
                return RedirectToAction("Index");
            }
            return View(feedbackEnquiry);
        }

        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}