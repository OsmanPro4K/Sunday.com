using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace MondayProject
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void OnLoginBtnClicked(object sender, EventArgs e)
        {
            string email = this.Login_Email.Value;
            string password = this.Login_Password.Value;

            string encryptedPasswordToCheck = EncryptPassword(password);

            if (CheckLoginInformation(email, encryptedPasswordToCheck) == true)
            {
                GetUserId(email, encryptedPasswordToCheck);
                Response.Redirect("Dashboard2.aspx");
                ErrorMessage.Text = "Login Successful";
            }
            else
            {
                ErrorMessage.Text = "Invalid Email or Password";
            }
        }

        protected string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder stringBuilder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }

        protected bool CheckLoginInformation(string email, string encryptedPasswordToCheck)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Count(*) FROM userInfo WHERE email = @email AND password = @password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", encryptedPasswordToCheck);

                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        protected bool GetUserId(string email, string encryptedPassword)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id FROM userInfo WHERE email = @Email AND password = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", encryptedPassword);

                    // Execute the query and retrieve the user ID
                    int userId = (int)command.ExecuteScalar();

                    // Check if user exists
                    if (userId > 0)
                    {
                        // Set user ID in session or any other mechanism for maintaining user state
                        Session["UserId"] = userId;

                        return true; // Login successful
                    }
                    else
                    {
                        return false; // Login failed
                    }
                }
            }
        }
    }
}