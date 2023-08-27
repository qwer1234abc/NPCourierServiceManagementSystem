using Microsoft.AspNetCore.Mvc.Rendering;
using System.Configuration;
using System.Data.SqlClient;
using WEB_Assignment_IT02_Team7.Models;
namespace WEB_Assignment_IT02_Team7.DAL
{
    public class FrontOfficeDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        public FrontOfficeDAL()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "NPCSConnectionString");
            conn = new SqlConnection(strConn);
        }

        public List<SelectListItem> GetToCitiesByCountry(string toCountry)
        {
            List<SelectListItem> cities = new List<SelectListItem>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT DISTINCT ToCity FROM ShippingRate WHERE ToCountry = @ToCountry";
            cmd.Parameters.AddWithValue("@ToCountry", toCountry);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string city = reader.GetString(0);
                SelectListItem item = new SelectListItem
                {
                    Text = city,
                    Value = city
                };
                cities.Add(item);
            }

            reader.Close();
            conn.Close();
            return cities;
        }

        public List<SelectListItem> GetFromCitiesByCountry(string fromCountry)
        {
            List<SelectListItem> cities = new List<SelectListItem>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT DISTINCT FromCity FROM ShippingRate WHERE FromCountry = @FromCountry";
            cmd.Parameters.AddWithValue("@FromCountry", fromCountry);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string city = reader.GetString(0);
                SelectListItem item = new SelectListItem
                {
                    Text = city,
                    Value = city
                };
                cities.Add(item);
            }

            reader.Close();
            conn.Close();
            return cities;
        }

        public List<SelectListItem> GetFromCountries()
        {
            List<SelectListItem> fromCountries = new List<SelectListItem>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT DISTINCT FromCountry FROM ShippingRate";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string fromCountry = reader.GetString(0);
                SelectListItem item = new SelectListItem
                {
                    Text = fromCountry,
                    Value = fromCountry
                };
                fromCountries.Add(item);
            }

            reader.Close();
            conn.Close();
            return fromCountries;
        }

        public List<SelectListItem> GetToCountries()
        {
            List<SelectListItem> toCountries = new List<SelectListItem>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT DISTINCT ToCountry FROM ShippingRate";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string toCountry = reader.GetString(0);
                SelectListItem item = new SelectListItem
                {
                    Text = toCountry,
                    Value = toCountry
                };
                toCountries.Add(item);
            }

            reader.Close();
            conn.Close();
            return toCountries;
        }

        public int GetTransitTime(Parcel parcel)
        {
            int transitTime = -1;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT TransitTime FROM ShippingRate WHERE FromCity = @FromCity AND FromCountry = @FromCountry AND ToCity = @ToCity AND ToCountry = @ToCountry";
            cmd.Parameters.AddWithValue("@FromCity", parcel.FromCity);
            cmd.Parameters.AddWithValue("@FromCountry", parcel.FromCountry);
            cmd.Parameters.AddWithValue("@ToCity", parcel.ToCity);
            cmd.Parameters.AddWithValue("@ToCountry", parcel.ToCountry);
            conn.Open();
            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                transitTime = Convert.ToInt32(result);
            }
            conn.Close();
            return transitTime;

        }
        public double GetShippingRate(Parcel parcel)
        {
            double shippingRate = -1;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT ShippingRate FROM ShippingRate WHERE FromCity = @FromCity AND FromCountry = @FromCountry AND ToCity = @ToCity AND ToCountry = @ToCountry";
            cmd.Parameters.AddWithValue("@FromCity", parcel.FromCity);
            cmd.Parameters.AddWithValue("@FromCountry", parcel.FromCountry);
            cmd.Parameters.AddWithValue("@ToCity", parcel.ToCity);
            cmd.Parameters.AddWithValue("@ToCountry", parcel.ToCountry);
            conn.Open();
            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                shippingRate = Convert.ToDouble(result);
            }
            conn.Close();
            return shippingRate;
        }

        public int CreateParcel(Parcel parcel, DeliveryHistory deliveryHistory)
        {
            SqlCommand cmd1 = new SqlCommand();
            cmd1.Connection = conn;
            cmd1.CommandText = @"INSERT INTO Parcel (ItemDescription, SenderName, SenderTelNo, ReceiverName, ReceiverTelNo, DeliveryAddress, FromCity, FromCountry, ToCity, ToCountry, ParcelWeight, DeliveryCharge, Currency, TargetDeliveryDate, DeliveryStatus, DeliveryManID) OUTPUT INSERTED.ParcelID
                        VALUES (@ItemDescription, @SenderName, @SenderTelNo, @ReceiverName, @ReceiverTelNo, @DeliveryAddress, @FromCity, @FromCountry, @ToCity, @ToCountry, @ParcelWeight, @DeliveryCharge, @Currency, @TargetDeliveryDate, @DeliveryStatus, @DeliveryManID)";
            cmd1.Parameters.AddWithValue("@SenderName", parcel.SenderName);
            cmd1.Parameters.AddWithValue("@SenderTelNo", parcel.SenderTelNo);
            cmd1.Parameters.AddWithValue("@ReceiverName", parcel.ReceiverName);
            cmd1.Parameters.AddWithValue("@ReceiverTelNo", parcel.ReceiverTelNo);
            cmd1.Parameters.AddWithValue("@DeliveryAddress", parcel.DeliveryAddress);
            cmd1.Parameters.AddWithValue("@FromCity", parcel.FromCity);
            cmd1.Parameters.AddWithValue("@FromCountry", parcel.FromCountry);
            cmd1.Parameters.AddWithValue("@ToCity", parcel.ToCity);
            cmd1.Parameters.AddWithValue("@ToCountry", parcel.ToCountry);
            cmd1.Parameters.AddWithValue("@ParcelWeight", parcel.ParcelWeight);
            cmd1.Parameters.AddWithValue("@TargetDeliveryDate", parcel.TargetDeliveryDate);
            cmd1.Parameters.AddWithValue("@DeliveryCharge", parcel.DeliveryCharge);
            cmd1.Parameters.AddWithValue("@ItemDescription", parcel.ItemDescription != null ? parcel.ItemDescription : DBNull.Value);
            cmd1.Parameters.AddWithValue("@Currency", parcel.Currency);
            cmd1.Parameters.AddWithValue("@DeliveryStatus", parcel.DeliveryStatus);
            cmd1.Parameters.AddWithValue("@DeliveryManID", DBNull.Value);

            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = conn;
            cmd2.CommandText = @"INSERT INTO DeliveryHistory (ParcelID, Description) VALUES (@ParcelID, @Description)";

            conn.Open();
            parcel.ParcelID = Convert.ToInt32(cmd1.ExecuteScalar());
            cmd2.Parameters.AddWithValue("@ParcelID", parcel.ParcelID);
            cmd2.Parameters.AddWithValue("@Description", deliveryHistory.Description);
            cmd2.ExecuteNonQuery();
            conn.Close();

            return parcel.ParcelID;
        }

        public List<Parcel> GetParcelCharges()
        {

            List<Parcel> parcels = new List<Parcel>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT p.ParcelID, p.DeliveryCharge, p.DeliveryCharge - COALESCE(SUM(pt.AmtTran), 0) AS UnpaidDeliveryCharge, p.Currency
FROM Parcel p
LEFT JOIN PaymentTransaction pt ON p.ParcelID = pt.ParcelID
GROUP BY p.ParcelID, p.DeliveryCharge, p.Currency
HAVING p.DeliveryCharge - COALESCE(SUM(pt.AmtTran), 0) > 0";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Parcel parcel = new Parcel
                {
                    ParcelID = reader.GetInt32(0),
                    DeliveryCharge = Convert.ToDouble(reader.GetDecimal(1)),
                    UnpaidDeliveryCharge = Convert.ToDouble(reader.GetDecimal(2)),
                    Currency = reader.GetString(3)
                };
                parcels.Add(parcel);
            }

            reader.Close();
            conn.Close();
            return parcels;
        }

        public Parcel GetParcelDetails(int parcelID)
        {
            Parcel parcel = new Parcel();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT ParcelID, FromCity, FromCountry, ToCity, ToCountry, ParcelWeight, Currency 
FROM Parcel WHERE ParcelID = @ParcelID";
            cmd.Parameters.AddWithValue("@ParcelID", parcelID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                parcel.ParcelID = reader.GetInt32(0);
                parcel.FromCity = reader.GetString(1);
                parcel.FromCountry = reader.GetString(2);
                parcel.ToCity = reader.GetString(3);
                parcel.ToCountry = reader.GetString(4);
                parcel.ParcelWeight = Convert.ToDouble(reader.GetDouble(5));
                parcel.Currency = reader.GetString(6);
            }
            reader.Close();
            conn.Close();
            return parcel;
        }

        public List<CashVoucher> GetCashVouchers()
        {
            List<CashVoucher> cashVouchers = new List<CashVoucher>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT CashVoucherID, Amount, Currency, Status FROM CashVoucher";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                CashVoucher cashVoucher = new CashVoucher
                {
                    CashVoucherID = reader.GetInt32(0),
                    Amount = Convert.ToDouble(reader.GetDecimal(1)),
                    Currency = reader.GetString(2),
                    Status = reader.GetString(3),
                };
                cashVouchers.Add(cashVoucher);
            }
            reader.Close();
            conn.Close();
            return cashVouchers;
        }

        public void CreatePaymentTransaction(PaymentTransaction paymentTransaction)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"INSERT INTO PaymentTransaction (ParcelID, AmtTran, Currency, TranType)
                        VALUES (@ParcelID, @AmtTran, @Currency, @TranType)";
            cmd.Parameters.AddWithValue("@ParcelID", paymentTransaction.ParcelID);
            cmd.Parameters.AddWithValue("@AmtTran", paymentTransaction.AmtTran);
            cmd.Parameters.AddWithValue("@Currency", paymentTransaction.Currency);
            cmd.Parameters.AddWithValue("@TranType", paymentTransaction.TranType);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void UpdateCashVoucherStatus(CashVoucher cashVoucher)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE CashVoucher SET Status = @Status WHERE CashVoucherID = @CashVoucherID";
            cmd.Parameters.AddWithValue("@Status", cashVoucher.Status);
            cmd.Parameters.AddWithValue("@CashVoucherID", cashVoucher.CashVoucherID);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
