using System.Configuration;
using System.Data.SqlClient;
using WEB_Assignment_IT02_Team7;
using WEB_Assignment_IT02_Team7.Models;
namespace WEB_Assignment_IT02_Team7.DAL
{
    public class CommonDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        public CommonDAL()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "NPCSConnectionString");
            conn = new SqlConnection(strConn);
        }
        public bool StaffLogin(string loginId, string password, HttpContext httpContext)
        {
            bool authenticated = false;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT * FROM Staff";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if ((reader.GetString(2).ToLower() == loginId) &&
                    (reader.GetString(3) == password))
                {
                    if (reader.GetString(4) == "Front Office Staff")
                    {
                        httpContext.Session.SetString("Name", reader.GetString(1));
                        httpContext.Session.SetString("LoginID", reader.GetString(2));
                        httpContext.Session.SetString("Role", reader.GetString(4));
                    }
                    else if (reader.GetString(4) == "Delivery Man")
                    {                        
                        httpContext.Session.SetInt32("StaffID", reader.GetInt32(0));
                        httpContext.Session.SetString("Name", reader.GetString(1));
                        httpContext.Session.SetString("LoginID", reader.GetString(2));
						httpContext.Session.SetString("Role", reader.GetString(4));
                    }
                    else if (reader.GetString(4) == "Station Manager")
                    {
                        httpContext.Session.SetString("Name", reader.GetString(1));
                        httpContext.Session.SetInt32("StaffID", reader.GetInt32(0));
						httpContext.Session.SetString("LoginID", reader.GetString(2));
						httpContext.Session.SetString("Role", reader.GetString(4));
                    }
                    authenticated = true;
                    break;
                }
            }
            reader.Close();
            conn.Close();
            return authenticated;
        }
        
        public bool MemberLogin(string email, string password, HttpContext httpContext)
        {
            bool authenticated = false;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT * FROM Member";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if ((reader.GetString(4).ToLower() == email) &&
                    (reader.GetString(5) == password))
                {
                    httpContext.Session.SetInt32("MemberID", reader.GetInt32(0));
                    httpContext.Session.SetString("Name", reader.GetString(1));
                    httpContext.Session.SetString("Role", "Customer");
                    authenticated = true;
                    break;
                }
            }
            reader.Close();
            conn.Close();
            return authenticated;
        }

        public bool IsEmailExist(string email, int memberID)
        {
            bool emailFound = false;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT MemberID FROM Member
 WHERE EmailAddr=@SelectedEmail";
            cmd.Parameters.AddWithValue("@SelectedEmail", email);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (reader.GetInt32(0) != memberID)
                        emailFound = true;
                    else
                        emailFound = false;
                }
            }
            else
            {
                emailFound = false;
            }
            reader.Close();
            conn.Close();
            return emailFound;
        }

        public int MemberRegister(Member member)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"INSERT INTO Member (Name, Salutation, TelNo, EmailAddr, Password, BirthDate, City, Country)
OUTPUT INSERTED.MemberID
VALUES (@Name, @Salutation, @TelNo, @EmailAddr, @Password, @BirthDate, @City, @Country)";

            cmd.Parameters.AddWithValue("@Name", member.Name);
            cmd.Parameters.AddWithValue("@Salutation", member.Salutation);
            cmd.Parameters.AddWithValue("@TelNo", member.TelNo);
            cmd.Parameters.AddWithValue("@EmailAddr", member.EmailAddr);
            cmd.Parameters.AddWithValue("@Password", member.Password);
            cmd.Parameters.AddWithValue("@BirthDate", member.BirthDate != null ? member.BirthDate : DBNull.Value);
            cmd.Parameters.AddWithValue("@City", member.City != null ? member.City : DBNull.Value);
            cmd.Parameters.AddWithValue("@Country", member.Country);

            conn.Open();
            member.MemberID = (int)cmd.ExecuteScalar();
            conn.Close();
            return member.MemberID;
        }   
    }
}
