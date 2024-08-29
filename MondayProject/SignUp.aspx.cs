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
    public partial class SignUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void OnContinueBtnClicked(object sender, EventArgs e) 
        {
            string fullname = this.SignUp_FullName.Value;
            string username = this.SignUp_Username.Value;
            string email = this.SignUp_Email.Value;
            string password = this.SignUp_Password.Value;

            string encryptedPassword = EncryptPassword(password);

            StoreInDB(fullname, username, email, encryptedPassword);
        }

        protected string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the bytes to string
                StringBuilder stringBuilder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        protected void StoreInDB(string fullname, string username, string email, string password)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO userInfo (full_name, username, email, password) VALUES (@full_name, @username, @email, @password)";
                using (SqlCommand command = new SqlCommand(query, connection))
                { 
                    connection.Open();

                    command.Parameters.AddWithValue("@full_name", fullname);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);

                    command.ExecuteNonQuery();

                    connection.Close();
                }

                // New Query
                int user_id = 0;
                string getIdQuery = "SELECT id FROM userInfo WHERE email = @email";
                using (SqlCommand getIdCommand = new SqlCommand(getIdQuery, connection))
                {
                    connection.Open();

                    getIdCommand.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = getIdCommand.ExecuteReader()) 
                    {
                        while (reader.Read()) 
                        {
                            user_id = Convert.ToInt32(reader["id"].ToString());
                        }
                    }

                    connection.Close();
                }

                // New Query
                string insertQuery = "INSERT INTO teams (user_id, user_email, role) VALUES (@id, @email, 'admin')";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    connection.Open();

                    insertCommand.Parameters.AddWithValue("@id", user_id);
                    insertCommand.Parameters.AddWithValue("@email", email);

                    insertCommand.ExecuteNonQuery();

                    connection.Close();
                }

                // New Query
                string createBoardQuery = "INSERT INTO boards (board_name, user_id) VALUES (@board_name, @user_id)";
                using (SqlCommand createBoardCommand = new SqlCommand(createBoardQuery, connection))
                {
                    connection.Open();

                    createBoardCommand.Parameters.AddWithValue("@board_name", "My first board");
                    createBoardCommand.Parameters.AddWithValue("@user_id", user_id);

                    createBoardCommand.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }
    }
}