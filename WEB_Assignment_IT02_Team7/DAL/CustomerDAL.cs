using Microsoft.AspNetCore.Mvc.Rendering;
using System.Configuration;
using System.Data.SqlClient;
using WEB_Assignment_IT02_Team7.Models;
namespace WEB_Assignment_IT02_Team7.DAL
{
    public class CustomerDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        public CustomerDAL()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "NPCSConnectionString");
            conn = new SqlConnection(strConn);
        }

        public List<Parcel> GetPersonalParcelRecords(string customerName)
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

        public void CreateFeedbackEnquiry(FeedbackEnquiry feedbackEnquiry)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"INSERT INTO FeedbackEnquiry (MemberID, Content, DateTimePosted, StaffID, Response, Status)
                                VALUES (@MemberID, @Content, @DateTimePosted, @StaffID, @Response, @Status)";
            cmd.Parameters.AddWithValue("@MemberID", feedbackEnquiry.MemberID);
            cmd.Parameters.AddWithValue("@Content", feedbackEnquiry.Content);
            cmd.Parameters.AddWithValue("@DateTimePosted", feedbackEnquiry.DateTimePosted);
            cmd.Parameters.AddWithValue("@StaffID", feedbackEnquiry.StaffID != null ? feedbackEnquiry.StaffID : DBNull.Value);
            cmd.Parameters.AddWithValue("@Response", feedbackEnquiry.Response != null ? feedbackEnquiry.Response : DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", feedbackEnquiry.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public List<FeedbackEnquiry> GetPersonalFeedbackEnquiries(int memberID)
        {
            List<FeedbackEnquiry> feedbackEnquiries = new List<FeedbackEnquiry>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT * FROM FeedbackEnquiry WHERE MemberID = @MemberID";
            cmd.Parameters.AddWithValue("@MemberID", memberID);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                FeedbackEnquiry feedbackEnquiry = new FeedbackEnquiry
                {
                    FeedbackEnquiryID = reader.GetInt32(0),
                    MemberID = reader.GetInt32(1),
                    Content = reader.GetString(2),
                    DateTimePosted = reader.GetDateTime(3),
                    StaffID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    Response = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Status = Convert.ToChar(reader.GetString(6)),
                };
                feedbackEnquiries.Add(feedbackEnquiry);
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
    }
}