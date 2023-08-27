using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Configuration;
using System.Data.SqlClient;
using WEB_Assignment_IT02_Team7.Models;

namespace WEB_Assignment_IT02_Team7.DAL
{
	public class DeliveryManDAL
	{
		private IConfiguration Configuration { get; }

		private SqlConnection conn;
		public DeliveryManDAL()
		{
			var builder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json");
			Configuration = builder.Build();
			string strConn = Configuration.GetConnectionString(
			"NPCSConnectionString");
			conn = new SqlConnection(strConn);
		}

		public List<Parcel> GetParcelsForDeliveryMan(int? deliveryManID)
		{
			List<Parcel> parcels = new List<Parcel>();
			conn.Open();
			string query = "SELECT p.* FROM Parcel p INNER JOIN Staff s ON p.DeliveryManID = s.StaffID WHERE s.StaffID = @DeliveryManID AND p.DeliveryStatus = 1";
			SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@DeliveryManID", deliveryManID);
			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Parcel parcel = new Parcel
				{
					ParcelID = reader.GetInt32(0),
					FromCountry = reader.GetString(8),
					ToCountry = reader.GetString(10),
					TargetDeliveryDate = reader.GetDateTime(14),
					DeliveryStatus = Convert.ToChar(reader.GetString(15)),
				};

				parcels.Add(parcel);
			}

			reader.Close();
			conn.Close();
			return parcels;
		}

		public void UpdateDeliveryStatusLocal(Parcel parcel, DeliveryHistory deliveryHistory)
		{
			DateTime date = DateTime.Now;

			string query = "Update Parcel SET DeliveryStatus = @DeliveryStatus WHERE ParcelID = @ParcelID";

			conn.Open();
			SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@ParcelID", parcel.ParcelID);
			//cmd.Parameters.AddWithValue("@ParcelDescription", parceldescription);

			if (parcel.FromCountry == parcel.ToCountry)
			{
				cmd.Parameters.AddWithValue("DeliveryStatus", 3);
				cmd.ExecuteNonQuery();

				SqlCommand cmd2 = new SqlCommand();
				cmd2.Connection = conn;
				cmd2.CommandText = @"INSERT INTO DeliveryHistory (ParcelID, Description) VALUES (@ParcelID, @Description)";

				cmd2.Parameters.AddWithValue("@ParcelID", parcel.ParcelID);
				cmd2.Parameters.AddWithValue("@Description", deliveryHistory.Description);
				cmd2.ExecuteNonQuery();
				conn.Close();

			}
			conn.Close();
		}

		public void UpdateDeliveryStatusOverSeas(Parcel parcel, DeliveryHistory deliveryHistory)
		{
			DateTime date = DateTime.Now;

			string query = "Update Parcel SET DeliveryStatus = @DeliveryStatus WHERE ParcelID = @ParcelID";

			conn.Open();
			SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@ParcelID", parcel.ParcelID);
			//cmd.Parameters.AddWithValue("@ParcelDescription", parceldescription);

			if (parcel.FromCountry != parcel.ToCountry)
			{
				cmd.Parameters.AddWithValue("DeliveryStatus", 2);
				cmd.ExecuteNonQuery();

				SqlCommand cmd2 = new SqlCommand();
				cmd2.Connection = conn;
				cmd2.CommandText = @"INSERT INTO DeliveryHistory (ParcelID, Description) VALUES (@ParcelID, @Description)";

				cmd2.Parameters.AddWithValue("@ParcelID", parcel.ParcelID);
				cmd2.Parameters.AddWithValue("@Description", deliveryHistory.Description);
				cmd2.ExecuteNonQuery();
				conn.Close();

			}
			conn.Close();
		}


		public void FailDeliveryStatus(int? parcelID)
		{
			string query = "Update Parcel SET DeliveryStatus = @DeliveryStatus WHERE ParcelID = @ParcelID";

			SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@ParcelID", parcelID);
			//cmd.Parameters.AddWithValue("@ParcelDescription", parceldescription);

			cmd.Parameters.AddWithValue("DeliveryStatus", 4);

			conn.Open();
			cmd.ExecuteNonQuery();
			conn.Close();
		}

		public List<DeliveryFailureReport> GetDeliveryFailureReport()
		{
			List<DeliveryFailureReport> deliveryFailureReports = new List<DeliveryFailureReport>();
			conn.Open();
			string query = "SELECT * FROM DeliveryFailure";
			SqlCommand cmd = new SqlCommand(query, conn);
			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				DeliveryFailureReport deliveryFailureReport = new DeliveryFailureReport
				{
					reportid = reader.GetInt32(0),
					parcelid = reader.GetInt32(1),
					deliverymanid = reader.GetInt32(2),
					failuretype = Convert.ToChar(reader.GetString(3)),
					description = reader.GetString(4),
					datecreated = reader.GetDateTime(7),
				};
				deliveryFailureReports.Add(deliveryFailureReport);

			}

			reader.Close();
			conn.Close();
			return deliveryFailureReports;

		}

		public List<Parcel> GetFailedDelivery()
		{
			List<Parcel> failedDelivery = new List<Parcel>();
			conn.Open();
			string query = "SELECT * FROM parcel LEFT JOIN DeliveryFailure ON parcel.ParcelID = DeliveryFailure.ParcelID WHERE parcel.DeliveryStatus = '4' AND DeliveryFailure.ReportID IS NULL";
			SqlCommand cmd = new SqlCommand(query, conn);
			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Parcel parcellist = new Parcel
				{
					ParcelID = reader.GetInt32(0),

					DeliveryStatus = Convert.ToChar(reader.GetString(15)),

				};
				failedDelivery.Add(parcellist);
			}
			reader.Close();
			conn.Close();
			return failedDelivery;
		}

		public int AddDeliveryFailureReport(DeliveryFailureReport deliveryFailureReport)
		{
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = @"INSERT INTO DeliveryFailure ( ParcelID, DeliveryManID, FailureType,
                Description, DateCreated)
                OUTPUT INSERTED.ReportID
                VALUES(@parcelid, @deliverymanid, @failuretype, @description, @datecreated)";

			cmd.Parameters.AddWithValue("@parcelid", deliveryFailureReport.parcelid);
			cmd.Parameters.AddWithValue("@deliverymanid", deliveryFailureReport.deliverymanid);
			cmd.Parameters.AddWithValue("@failuretype", deliveryFailureReport.failuretype);
			cmd.Parameters.AddWithValue("@description", deliveryFailureReport.description);
			cmd.Parameters.AddWithValue("@datecreated", deliveryFailureReport.datecreated);

			conn.Open();
			deliveryFailureReport.reportid = Convert.ToInt32(cmd.ExecuteScalar());
			conn.Close();
			return deliveryFailureReport.reportid;
		}

		public bool CheckDeliveryFailure(int parcelid)
		{
			bool exist = false;
			conn.Open();
			string query = "SELECT TOP 1 ParcelID FROM DeliveryFailure WHERE ParcelID = @ParcelID";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ParcelID", parcelid);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                exist = true; // If the reader contains a row, it means the parcelid exists in the database.
            }

            reader.Close();
            conn.Close();

            return exist;

        }


	}
}
