using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_Assignment_IT02_Team7.DAL;
using WEB_Assignment_IT02_Team7.Models;

namespace WEB_Assignment_IT02_Team7.Controllers
{
    public class CustomerController : Controller
    {
        private CustomerDAL CustomerContext = new CustomerDAL();

        public ActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Customer"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult DeliveryRecords()
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Customer"))
            {
                return RedirectToAction("Index", "Home");
            }

            List<Parcel> parcelList = CustomerContext.GetPersonalParcelRecords(HttpContext.Session.GetString("Name"));
            return View(parcelList);
        }

        public ActionResult CreateFeedbackEnquiries()
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Customer"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFeedbackEnquiries(FeedbackEnquiry feedbackEnquiry)
        {
            if (feedbackEnquiry.Content != null)
            {
                feedbackEnquiry.MemberID = Convert.ToInt32(HttpContext.Session.GetInt32("MemberID"));
                feedbackEnquiry.DateTimePosted = DateTime.Now;
                CustomerContext.CreateFeedbackEnquiry(feedbackEnquiry);
                return RedirectToAction("Index");
            }

            return View(feedbackEnquiry);
        }

        public ActionResult ViewResponses()
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Customer"))
            {
                return RedirectToAction("Index", "Home");
            }

            List<FeedbackEnquiry> feedbackEnquiryList = CustomerContext.GetPersonalFeedbackEnquiries(Convert.ToInt32(HttpContext.Session.GetInt32("MemberID")));
            return View(feedbackEnquiryList);
        }

        public ActionResult DisplayDeliveryHistory(int? parcelID)
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Customer"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (parcelID == null)
            {
                return RedirectToAction("DeliveryInformation");
            }

            List<DeliveryHistory> deliveryHistoryList = CustomerContext.GetDeliveryHistories(Convert.ToInt32(parcelID));
            return View(deliveryHistoryList);
        }

        public ActionResult ResponseDetails(int? feedbackEnquiryID)
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Customer"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (feedbackEnquiryID == null)
            {
                RedirectToAction("ViewResponses");
            }

            FeedbackEnquiry feedbackEnquiry = CustomerContext.GetFeedbackEnquiryDetails(Convert.ToInt32(feedbackEnquiryID));
            return View(feedbackEnquiry);
        }

        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}