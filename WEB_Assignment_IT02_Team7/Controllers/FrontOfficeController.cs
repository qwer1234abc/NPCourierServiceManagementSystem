using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_Assignment_IT02_Team7.Models;
using WEB_Assignment_IT02_Team7.DAL;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;

namespace WEB_Assignment_IT02_Team7.Controllers
{
    public class FrontOfficeController : Controller
    {
        private FrontOfficeDAL frontOfficeContext = new FrontOfficeDAL();
        private Dictionary<string, List<SelectListItem>> toCitiesByCountry = new Dictionary<string, List<SelectListItem>>();
        private Dictionary<string, List<SelectListItem>> fromCitiesByCountry = new Dictionary<string, List<SelectListItem>>();
        private List<SelectListItem> paymentMethods = new List<SelectListItem>();


        public FrontOfficeController()
        {
            List<SelectListItem> toCountriesSelectListItems = frontOfficeContext.GetToCountries();
            List<string> toCountryList = toCountriesSelectListItems.Select(item => item.Text).ToList();

            List<SelectListItem> fromCountriesSelectListItems = frontOfficeContext.GetFromCountries();
            List<string> fromCountryList = fromCountriesSelectListItems.Select(item => item.Text).ToList();

            foreach (string toCountry in toCountryList)
            {
                List<SelectListItem> toCityList = frontOfficeContext.GetToCitiesByCountry(toCountry);
                toCitiesByCountry.Add(toCountry, toCityList);
            }
            foreach (string fromCountry in fromCountryList)
            {
                List<SelectListItem> fromCityList = frontOfficeContext.GetFromCitiesByCountry(fromCountry);
                fromCitiesByCountry.Add(fromCountry, fromCityList);
            }
            paymentMethods.Add(new SelectListItem { Text = "Cash", Value = "CASH" });
            paymentMethods.Add(new SelectListItem { Text = "Cash Voucher", Value = "VOUC" });
            paymentMethods.Add(new SelectListItem { Text = "Cash & Cash Voucher", Value = "CASHVOUC" });
        }

        public ActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Front Office Staff"))
            {
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.Remove("Page");
            return View();
        }

        public ActionResult CreateParcelRecord()
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Front Office Staff"))
            {
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.Remove("Page");
            ViewData["ShowCompute"] = false;
            ViewData["FromCountriesList"] = frontOfficeContext.GetFromCountries();
            ViewData["ToCountriesList"] = frontOfficeContext.GetToCountries();
            ViewData["ToCountryCityDict"] = toCitiesByCountry;
            ViewData["FromCountryCityDict"] = fromCitiesByCountry;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateParcelRecord(Parcel parcel, string submitButton)
        {
            ViewData["FromCountriesList"] = frontOfficeContext.GetFromCountries();
            ViewData["ToCountriesList"] = frontOfficeContext.GetToCountries();
            ViewData["ToCountryCityDict"] = toCitiesByCountry;
            ViewData["FromCountryCityDict"] = fromCitiesByCountry;
            if (submitButton == "Compute")
            {
                if (ModelState.IsValid)
                {
                    ViewData["ShippingRate"] = $"S${frontOfficeContext.GetShippingRate(parcel):F2}/kg";
                    ViewData["FromCityCountry"] = $"{parcel.FromCity}, {parcel.FromCountry}";
                    ViewData["ToCityCountry"] = $"{parcel.ToCity}, {parcel.ToCountry}";
                    int transitDays = frontOfficeContext.GetTransitTime(parcel);
                    ViewData["TransitDays"] = transitDays;
                    DateTime transiteDateTime = DateTime.Now.AddDays(transitDays);
                    string formattedDate = transiteDateTime.ToString("d-MMM-yyyy");
                    parcel.TargetDeliveryDate = DateTime.ParseExact(formattedDate, "d-MMM-yyyy", CultureInfo.InvariantCulture);
                    double shippingRate = frontOfficeContext.GetShippingRate(parcel);
                    double rawDeliveryCharge = Math.Round(shippingRate * parcel.ParcelWeight, 2);
                    ViewData["RawDeliveryCharge"] = $"({shippingRate} x {parcel.ParcelWeight}) = S${rawDeliveryCharge}";
                    double roundedDeliveryCharge = Math.Round(rawDeliveryCharge, 0);
                    ViewData["RoundedDeliveryCharge"] = $"S${roundedDeliveryCharge:F2}";
                    parcel.DeliveryCharge = Math.Max(roundedDeliveryCharge, 5);
                    ViewData["FinalDeliveryCharge"] = $"S${parcel.DeliveryCharge:F2}";
                    ViewData["ShowCompute"] = true;
                }
                else
                {
                    ViewData["ShowCompute"] = false;
                }
            }
            else if (submitButton == "Create")
            {
                if (ModelState.IsValid)
                {
                    int transitDays = frontOfficeContext.GetTransitTime(parcel);
                    DateTime transiteDateTime = DateTime.Now.AddDays(transitDays);
                    string formattedDate = transiteDateTime.ToString("d-MMM-yyyy");
                    parcel.TargetDeliveryDate = DateTime.ParseExact(formattedDate, "d-MMM-yyyy", CultureInfo.InvariantCulture);
                    double shippingRate = frontOfficeContext.GetShippingRate(parcel);
                    double rawDeliveryCharge = Math.Round(shippingRate * parcel.ParcelWeight, 2);
                    double roundedDeliveryCharge = Math.Round(rawDeliveryCharge, 0);
                    parcel.DeliveryCharge = Math.Max(roundedDeliveryCharge, 5);

                    DeliveryHistory deliveryHistory = new DeliveryHistory();
                    deliveryHistory.Description = $"Received parcel by {HttpContext.Session.GetString("LoginID")} on {DateTime.Now.ToString("d MMM yyyy h:mmtt", CultureInfo.CreateSpecificCulture("en-GB"))}.";
                    parcel.ParcelID = frontOfficeContext.CreateParcel(parcel, deliveryHistory);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["ShowCompute"] = false;
                }
            }
            else if (submitButton == "Edit")
            {
                ModelState.Clear();
                ViewData["ShowCompute"] = false;
            }
            ViewData["SelectedToCity"] = parcel.ToCity;
            ViewData["SelectedFromCity"] = parcel.FromCity;
            return View(parcel);
        }

        public ActionResult ViewUnpaidParcels()
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Front Office Staff"))
            {
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.Remove("Page");
            List<Parcel> parcelList = frontOfficeContext.GetParcelCharges();
            return View(parcelList);
        }

        public ActionResult PaymentTransaction(int? parcelID, double? originalCharge, double? unpaidCharge)
        {
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Front Office Staff"))
            {
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetString("Page", "Transact");

            if (parcelID == null || unpaidCharge <= 0)
            {
                RedirectToAction("ViewUnpaidParcels");
            }

            ParcelPayment parcelPayment = new ParcelPayment();
            parcelPayment.Parcels = frontOfficeContext.GetParcelDetails(parcelID.Value);
            ViewData["ParcelID"] = parcelID.Value;
            ViewData["FromCityCountry"] = $"{parcelPayment.Parcels.FromCity}, {parcelPayment.Parcels.FromCountry}";
            ViewData["ToCityCountry"] = $"{parcelPayment.Parcels.ToCity}, {parcelPayment.Parcels.ToCountry}";
            ViewData["ParcelWeight"] = $"{parcelPayment.Parcels.ParcelWeight}kg";
            ViewData["OriginalCharge"] = $"S${originalCharge:F2}";
            ViewData["UnpaidCharge"] = $"S${unpaidCharge:F2}";
            ViewData["Currency"] = parcelPayment.Parcels.Currency;
            ViewData["OriginalAmount"] = originalCharge;
            ViewData["UnpaidAmount"] = unpaidCharge;
            ViewData["PaymentMethods"] = paymentMethods;
            ViewData["CashVoucherValid"] = false;
            return View(parcelPayment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentTransaction(ParcelPayment parcelPayment, int? parcelID, double? originalCharge, double? unpaidCharge, string submitButton)
        {
            parcelPayment.Parcels = frontOfficeContext.GetParcelDetails(parcelID.Value);
            ViewData["ParcelID"] = parcelID.Value;
            ViewData["FromCityCountry"] = $"{parcelPayment.Parcels.FromCity}, {parcelPayment.Parcels.FromCountry}";
            ViewData["ToCityCountry"] = $"{parcelPayment.Parcels.ToCity}, {parcelPayment.Parcels.ToCountry}";
            ViewData["ParcelWeight"] = $"{parcelPayment.Parcels.ParcelWeight}kg";
            ViewData["OriginalCharge"] = $"S${originalCharge:F2}";
            ViewData["UnpaidCharge"] = $"S${unpaidCharge:F2}";
            ViewData["Currency"] = parcelPayment.Parcels.Currency;
            ViewData["OriginalAmount"] = originalCharge;
            ViewData["UnpaidAmount"] = unpaidCharge;
            ViewData["PaymentMethods"] = paymentMethods;

            List<CashVoucher> cashVoucherList = frontOfficeContext.GetCashVouchers();

            if (submitButton == "Verify")
            {
                CashVoucher cashVoucher = new CashVoucher();
                int? cashVoucherID = parcelPayment.CashVouchers.CashVoucherID;
                cashVoucher = cashVoucherList.FirstOrDefault(cv => cv.CashVoucherID == cashVoucherID);
                if (cashVoucher != null)
                {
                    parcelPayment.CashVouchers.CashVoucherID = cashVoucher.CashVoucherID;
                    parcelPayment.CashVouchers.Amount = cashVoucher.Amount;
                    parcelPayment.CashVouchers.Currency = cashVoucher.Currency;
                    switch (cashVoucher.Status)
                    {
                        case "0":
                            parcelPayment.CashVouchers.Status = "Pending";
                            break;
                        case "1":
                            parcelPayment.CashVouchers.Status = "Collected";
                            ViewData["CashVoucherValid"] = true;
                            break;
                        case "2":
                            parcelPayment.CashVouchers.Status = "Redeemed";
                            break;
                        default:
                            parcelPayment.CashVouchers.Status = "Unknown";
                            break;
                    }
                }
                else
                {
                    parcelPayment.CashVouchers.CashVoucherID = null;
                    ViewData["InvalidErrorMessage"] = true;
                }
            }
            else if (submitButton == "Create")
            {
                bool cashAmtTranRequired = parcelPayment.PaymentTransactions.CashAmt == null;
                bool voucherAmtTranRequired = parcelPayment.PaymentTransactions.VoucherAmt == null;
                bool cashVoucherIDRequired = parcelPayment.CashVouchers.CashVoucherID == null;

                if (parcelPayment.PaymentTransactions.PaymentMethod == "CASH")
                {
                    if (cashAmtTranRequired)
                    {
                        ModelState.AddModelError("PaymentTransactions.CashAmt", "Cash Amount required.");
                    }
                    else if (parcelPayment.PaymentTransactions.TotalAmt > unpaidCharge)
                    {
                        ModelState.AddModelError("PaymentTransactions.TotalAmt", "Total Amount more than Unpaid Charge.");
                    }
                    else
                    {
                        PaymentTransaction cashPaymentTransaction = CreatePaymentTransaction(parcelID, parcelPayment.PaymentTransactions.TotalAmt, "CASH");
                        frontOfficeContext.CreatePaymentTransaction(cashPaymentTransaction);

                        return RedirectToAction("ViewUnpaidParcels");
                    }
                }
                else if (parcelPayment.PaymentTransactions.PaymentMethod == "VOUC")
                {
                    if (voucherAmtTranRequired || cashVoucherIDRequired)
                    {
                        if (voucherAmtTranRequired)
                            ModelState.AddModelError("PaymentTransactions.VoucherAmt", "Voucher Amount required.");

                        if (cashVoucherIDRequired)
                            ModelState.AddModelError("CashVouchers.CashVoucherID", "Voucher ID required.");
                    }
                    else
                    {
                        PaymentTransaction voucherPaymentTransaction = CreatePaymentTransaction(parcelID, parcelPayment.PaymentTransactions.TotalAmt, "VOUC");
                        frontOfficeContext.CreatePaymentTransaction(voucherPaymentTransaction);

                        CashVoucher cashVoucher = new CashVoucher
                        {
                            CashVoucherID = parcelPayment.CashVouchers.CashVoucherID,
                            Status = "2"
                        };
                        frontOfficeContext.UpdateCashVoucherStatus(cashVoucher);

                        return RedirectToAction("ViewUnpaidParcels");
                    }
                }
                else
                {
                    if (cashAmtTranRequired || voucherAmtTranRequired || cashVoucherIDRequired)
                    {
                        if (cashAmtTranRequired)
                            ModelState.AddModelError("PaymentTransactions.CashAmt", "Cash Amount required.");
                        if (voucherAmtTranRequired)
                            ModelState.AddModelError("PaymentTransactions.VoucherAmt", "Voucher Amount required.");
                        if (cashVoucherIDRequired)
                            ModelState.AddModelError("CashVouchers.CashVoucherID", "Voucher ID required.");
                    }
                    else if (parcelPayment.PaymentTransactions.TotalAmt > unpaidCharge)
                    {
                        if (parcelPayment.PaymentTransactions.VoucherAmt >= unpaidCharge)
                        {
                            ModelState.AddModelError("PaymentTransactions.TotalAmt", "Voucher Amount is sufficient.");
                        }
                        else
                        {
                            ModelState.AddModelError("PaymentTransactions.TotalAmt", "Total Amount more than Charge.");
                        }
                    }
                    else
                    {
                        PaymentTransaction cashPaymentTransaction = CreatePaymentTransaction(parcelID, parcelPayment.PaymentTransactions.CashAmt, "CASH");
                        PaymentTransaction voucherPaymentTransaction = CreatePaymentTransaction(parcelID, parcelPayment.PaymentTransactions.VoucherAmt, "VOUC");
                        CashVoucher cashVoucher = new CashVoucher
                        {
                            CashVoucherID = parcelPayment.CashVouchers.CashVoucherID,
                            Status = "2"
                        };

                        frontOfficeContext.CreatePaymentTransaction(cashPaymentTransaction);
                        frontOfficeContext.CreatePaymentTransaction(voucherPaymentTransaction);
                        frontOfficeContext.UpdateCashVoucherStatus(cashVoucher);

                        return RedirectToAction("ViewUnpaidParcels");
                    }
                }
            }
            return View(parcelPayment);
        }

        private PaymentTransaction CreatePaymentTransaction(int? parcelID, double? amount, string tranType)
        {
            return new PaymentTransaction
            {
                ParcelID = parcelID,
                AmtTran = amount,
                TranType = tranType
            };
        }

        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
