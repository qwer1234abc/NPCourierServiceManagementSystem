using Microsoft.AspNetCore.Mvc.Rendering;
using System.Configuration;
using System.Data.SqlClient;
using WEB_Assignment_IT02_Team7.Models;
namespace WEB_Assignment_IT02_Team7.DAL
{
    public class StationManagerDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        public StationManagerDAL()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "NPCSConnectionString");
            conn = new SqlConnection(strConn);
        }

        public List<FeedbackEnquiry> GetAllFeedbackEnquiries()
        {
            List<FeedbackEnquiry> feedbackEnquiries = new List<FeedbackEnquiry>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT * FROM FeedbackEnquiry WHERE Status = '0'";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                FeedbackEnquiry feedback = new FeedbackEnquiry
                {
                    FeedbackEnquiryID = reader.GetInt32(0),
                    MemberID = reader.GetInt32(1),
                    Content = reader.GetString(2),
                    DateTimePosted = reader.GetDateTime(3),
                    StaffID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    Response = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Status = Convert.ToChar(reader.GetString(6)),
                };
                feedbackEnquiries.Add(feedback);
            }
            reader.Close();
            conn.Close();
            return feedbackEnquiries;
        }

        public FeedbackEnquiry GetFeedbackEnquiryDetails(int feedbackEnquiryID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT * FROM FeedbackEnquiry WHERE FeedbackEnquiryID = @FeedbackEnquiryID";
            cmd.Parameters.AddWithValue("@FeedbackEnquiryID", feedbackEnquiryID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            FeedbackEnquiry feedbackEnquiry = new FeedbackEnquiry();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    feedbackEnquiry.FeedbackEnquiryID = reader.GetInt32(0);
                    feedbackEnquiry.MemberID = reader.GetInt32(1);
                    feedbackEnquiry.Content = reader.GetString(2);
                    feedbackEnquiry.DateTimePosted = reader.GetDateTime(3);
                    feedbackEnquiry.StaffID = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                    feedbackEnquiry.Response = reader.IsDBNull(5) ? null : reader.GetString(5);
                    feedbackEnquiry.Status = Convert.ToChar(reader.GetString(6));
                }
            }
            reader.Close();
            conn.Close();
            return feedbackEnquiry;
        }

        public void UpdateFeedbackEnquiryResponse(FeedbackEnquiry feedbackEnquiry)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE FeedbackEnquiry SET Response = @Response, StaffID = @StaffID, Status = @Status WHERE FeedbackEnquiryID = @FeedbackEnquiryID";
            cmd.Parameters.AddWithValue("@FeedbackEnquiryID", feedbackEnquiry.FeedbackEnquiryID);
            cmd.Parameters.AddWithValue("@Response", feedbackEnquiry.Response);
            cmd.Parameters.AddWithValue("@StaffID", feedbackEnquiry.StaffID);
            cmd.Parameters.AddWithValue("@Status", feedbackEnquiry.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public List<Parcel> GetParcelDetailsByID(int? parcelID)
        {
            List<Parcel> parcels = new List<Parcel>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT ParcelID, SenderName, SenderTelNo, ReceiverName, ReceiverTelNo, DeliveryStatus FROM Parcel WHERE ParcelID = @ParcelID";
            cmd.Parameters.AddWithValue("@ParcelID", parcelID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Parcel parcel = new Parcel
                {
                    ParcelID = reader.GetInt32(0),
                    SenderName = reader.GetString(1),
                    SenderTelNo = reader.GetString(2),
                    ReceiverName = reader.GetString(3),
                    ReceiverTelNo = reader.GetString(4),
                    DeliveryStatus = Convert.ToChar(reader.GetString(5)),
                };
                parcels.Add(parcel);
            }
            reader.Close();
            conn.Close();
            return parcels;
        }

        public List<Parcel> GetParcelDetailsByCustomerName(string? customerName)
        {
            List<Parcel> parcels = new List<Parcel>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT ParcelID, SenderName, SenderTelNo, ReceiverName, ReceiverTelNo, DeliveryStatus
                        FROM Parcel
                        WHERE SenderName = @CustomerName OR ReceiverName = @CustomerName";
            cmd.Parameters.AddWithValue("@CustomerName", customerName);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Parcel parcel = new Parcel
                {
                    ParcelID = reader.GetInt32(0),
                    SenderName = reader.GetString(1),
                    SenderTelNo = reader.GetString(2),
                    ReceiverName = reader.GetString(3),
                    ReceiverTelNo = reader.GetString(4),
                    DeliveryStatus = Convert.ToChar(reader.GetString(5)),
                };
                parcels.Add(parcel);
            }
            reader.Close();
            conn.Close();
            return parcels;
        }

        public List<DeliveryHistory> GetDeliveryHistories(int parcelID)
        {
            List<DeliveryHistory> deliveryHistories = new List<DeliveryHistory>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT * FROM DeliveryHistory WHERE ParcelID = @ParcelID";
            cmd.Parameters.AddWithValue("@ParcelID", parcelID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DeliveryHistory deliveryHistory = new DeliveryHistory
                {
                    RecordID = reader.GetInt32(0),
                    ParcelID = reader.GetInt32(1),
                    Description = reader.GetString(2),
                };
                deliveryHistories.Add(deliveryHistory);
            }
            reader.Close();
            conn.Close();
            return deliveryHistories;
        }

        public Parcel InProgressParcel (int? parcelID)
        {
            Parcel parcel = new Parcel();
            string query = "SELECT * FROM Parcel WHERE DeliveryStatus = 1 AND ParcelID = @SearchedParcelID";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SearchedParcelID", parcelID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                parcel.ParcelID = reader.GetInt32(0);
                parcel.SenderName = reader.GetString(2);
                parcel.ReceiverName = reader.GetString(4);
                parcel.DeliveryStatus = Convert.ToChar(reader.GetString(15));
            }
            reader.Close();
            conn.Close();
            return parcel;
        }

		public List<Staff> GetDeliveryMan()
		{
			List<Staff> staffs = new List<Staff>();
			conn.Open();
			string query = "SELECT * FROM STAFF WHERE Appointment = 'Delivery Man'";
			SqlCommand cmd = new SqlCommand(query, conn);
			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Staff deliveryman = new Staff
				{
					StaffID = reader.GetInt32(0),
					StaffName = reader.GetString(1),
					AppointMent = reader.GetString(4),
				};

				staffs.Add(deliveryman);
			}

			reader.Close();
			conn.Close();
			return staffs;
		}

		public int AssignParcel(AssignDeliveryViewModel parcel)
		{
			SqlCommand cmd = conn.CreateCommand();

			cmd.CommandText = @"UPDATE Parcel SET DeliveryStatus=@deliveryStatus, DeliveryManID=@deliveryManID WHERE ParcelID = @selectedParcelID";
			cmd.Parameters.AddWithValue("@deliveryStatus", '1');
			cmd.Parameters.AddWithValue("@deliveryManID", parcel.DeliveryManID);
			cmd.Parameters.AddWithValue("@selectedParcelID", parcel.ParcelID);
			conn.Open();
			int status = cmd.ExecuteNonQuery();
			conn.Close();
			return status;
		}

		public int AddDeliveryHistory(DeliveryHistory deliveryHistory)
		{
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = @"INSERT INTO DeliveryHistory (ParcelID, Description) 
                                OUTPUT INSERTED.RecordID
                                VALUES(@parcelid, @description)";
			cmd.Parameters.AddWithValue("@parcelid", deliveryHistory.ParcelID);
			cmd.Parameters.AddWithValue("@description", deliveryHistory.Description);

			conn.Open();
			deliveryHistory.RecordID = (int)cmd.ExecuteScalar();
			conn.Close();
			return deliveryHistory.RecordID;
		}

		public int DeliveryManParcel(int deliverymanid)
		{
			int count = 0;
			conn.Open();

			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = @"SELECT COUNT(*) FROM Parcel WHERE DeliveryManID = @deliveryManID AND DeliveryStatus = @deliveryStatus";
			cmd.Parameters.AddWithValue("@deliveryStatus", '1');
			cmd.Parameters.AddWithValue("@deliveryManID", deliverymanid);

			count = (int)cmd.ExecuteScalar();
			conn.Close();
			return count;

		}

		public bool CheckAssign(int parcelid)
		{
			bool isNull = false;
			conn.Open();
			string query = "Select * FROM Parcel Where ParcelID = @ParcelID";
			SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@ParcelID", parcelid);
			SqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				if (reader.IsDBNull(16))
				{
					isNull = true;
					break;
				}
			}
			conn.Close();
			return isNull;
		}

		public List<Parcel> ParcelDeliveryStatus0()
		{
			List<Parcel> plist = new List<Parcel>();
			conn.Open();
			string query = "SELECT * FROM Parcel WHERE DeliveryStatus = 0";
			SqlCommand cmd = new SqlCommand(query, conn);
			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Parcel parcel = new Parcel
				{
					ParcelID = reader.GetInt32(0),
					//DeliveryStatus = Convert.ToChar(reader.GetInt32 (16)),
					//DeliveryManID = reader.GetInt32 (17),
				};
				plist.Add(parcel);
			}
			reader.Close();
			conn.Close();
			return plist;

		}
	}
}