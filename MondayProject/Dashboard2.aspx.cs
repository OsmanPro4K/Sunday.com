using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Data.Common;
using System.Collections.ObjectModel;

namespace MondayProject
{
    public partial class Dashboard2 : System.Web.UI.Page
    {
        private string id;
        private string board_id;
        private string newStatus;
        private string teammate;
        private string oldPost;
        private string edittedPost;
        private string color;

        private List<Board> boards = new List<Board>();
        private List<Post> posts = new List<Post>();
        private List<Reply> replies = new List<Reply>();
        private List<string> teamMembers = new List<string>();
        private List<string> teamMembersWhoGotTaskAssigned = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["UserId"]) < 1)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                BindDiscussionsRepeater();

                this.PostProfilePicture.Text = this.GetCurrentlyLoggedInUserFullName().Split(' ')[0].Substring(0, 1) + this.GetCurrentlyLoggedInUserFullName().Split(' ')[1].Substring(0, 1);
                this.NavbarProfilePicture.Text = this.GetCurrentlyLoggedInUserFullName().Split(' ')[0].Substring(0, 1) + this.GetCurrentlyLoggedInUserFullName().Split(' ')[1].Substring(0, 1);
                this.SecondNavbarProfilePicture.Text = this.GetCurrentlyLoggedInUserFullName().Split(' ')[0].Substring(0, 1) + this.GetCurrentlyLoggedInUserFullName().Split(' ')[1].Substring(0, 1);

            }


            GetBoard();
            GetAllBoards();
            // UpdateTable();
            UpdatedUpdateTable();
            //SuperUpdatedTable();

            try
            {
                MakeTable();
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
            }
        }

        string getId() { return ViewState["id"] as string; }
        void setId(string id) { ViewState["id"] = id; }
        string getBoardId() { return ViewState["board_id"] as string; }
        void setBoardId(string board_id) { ViewState["board_id"] = board_id; }
        string getNewStatus() { return ViewState["newStatus"] as string; }
        void setNewStatus(string newStatus) { ViewState["newStatus"] = newStatus; }
        string getTeammate() { return ViewState["teammate"] as string; }
        void setTeammate(string teammate) { ViewState["teammate"] = teammate; }
        string getOldPost() { return ViewState["oldPost"] as string; }
        void setOldPost(string oldPost) { ViewState["oldPost"] = oldPost; }
        string getEditedPost() { return ViewState["editedPost"] as string; }
        void setEditedPost(string editedPost) { ViewState["editedPost"] = editedPost; }
        string getColor() {  return ViewState["color"] as string; }
        void setColor(string color) { ViewState["color"] = color; }

        protected string GetClientID(Control control)
        {
            return control.ClientID;
        }

        private void BindDiscussionsRepeater()
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            List<Post> posts = new List<Post>();
            Post post = new Post();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string getUserPfpColorQuery = "SELECT user_pfp_color FROM userInfo WHERE id = @user_id";
                using (SqlCommand getUserPfpColorCommand = new SqlCommand(getUserPfpColorQuery, connection))
                {
                    connection.Open();

                    getUserPfpColorCommand.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader1 = getUserPfpColorCommand.ExecuteReader())
                    {
                        while (reader1.Read())
                        {
                            this.setColor(reader1["user_pfp_color"].ToString());
                        }
                    }

                    connection.Close();
                }

                string getPostsQuery = "SELECT * FROM discussions";
                using (SqlCommand getPostsCommand = new SqlCommand(getPostsQuery, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = getPostsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            post = new Post
                            {
                                postID = reader["id"].ToString(),
                                username = reader["username"].ToString(),
                                post = reader["post"].ToString(),
                                date = reader["date"].ToString(),
                                colorValue = this.getColor(),
                            };

                            this.setColor(post.colorValue);
                            posts.Add(post);
                        }
                    }
                    connection.Close();
                }
            }

            string color = this.getColor();
            string[] rgbValues = color.Split(',');

            int r = Convert.ToInt32(rgbValues[0]);
            int g = Convert.ToInt32(rgbValues[1]);
            int b = Convert.ToInt32(rgbValues[2]);

            //this.PostProfilePicture.Style["background-color"] = $"rgb({r}, {g}, {b})";

            DiscussionsRepeater.DataSource = posts;
            DiscussionsRepeater.DataBind();
        }

        protected void DiscussionsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Post post = (Post)e.Item.DataItem;
                Repeater RepliesRepeater = (Repeater)e.Item.FindControl("RepliesRepeater");
                Label replyProfilePicture = (Label)e.Item.FindControl("ReplyProfilePicture");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string getUserPfpColorQuery = "SELECT user_pfp_color FROM userInfo WHERE id = @user_id";
                    using (SqlCommand getUserPfpColorCommand = new SqlCommand(getUserPfpColorQuery, connection))
                    {
                        connection.Open();
                        getUserPfpColorCommand.Parameters.AddWithValue("@user_id", user_id);
                        using (SqlDataReader reader1 = getUserPfpColorCommand.ExecuteReader())
                        {
                            while (reader1.Read())
                            {
                                this.setColor(reader1["user_pfp_color"].ToString());
                            }
                        }
                        connection.Close();
                    }
                }

                if (replyProfilePicture != null)
                {
                    string color = this.getColor();
                    int r = Convert.ToInt32(color.Split(',')[0]);
                    int g = Convert.ToInt32(color.Split(',')[1]);
                    int b = Convert.ToInt32(color.Split(',')[2]);
                    replyProfilePicture.BackColor = Color.FromArgb(r, g, b);
                }

                List<Reply> replies = GetRepliesForPost(post.postID);
                RepliesRepeater.DataSource = replies;
                RepliesRepeater.DataBind();
            }
        }

        private List<Reply> GetRepliesForPost(string postId)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            List<Reply> replies = new List<Reply>();
            Reply reply = new Reply();
            string username = this.GetCurrentlyLoggedInUserFullName();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {               
                string colorValue = "";
                string getUserPfpColorQuery = "SELECT user_pfp_color FROM userInfo WHERE id = @user_id";
                using (SqlCommand getUserPfpColorCommand = new SqlCommand(getUserPfpColorQuery, connection))
                {
                    connection.Open();

                    getUserPfpColorCommand.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader1 = getUserPfpColorCommand.ExecuteReader())
                    {
                        while (reader1.Read())
                        {
                            this.setColor(reader1["user_pfp_color"].ToString());
                        }
                    }

                    connection.Close();
                }

                string getRepliesQuery = "SELECT * FROM replies WHERE post_id = @post_id";
                using (SqlCommand getRepliesCommand = new SqlCommand(getRepliesQuery, connection))
                {
                    getRepliesCommand.Parameters.AddWithValue("@post_id", postId);
                    connection.Open();
                    using (SqlDataReader reader = getRepliesCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {                                                     
                            // Get relative time for the reply date
                            reply = new Reply
                            {
                                replyID = reader["id"].ToString(),
                                username = this.GetCurrentlyLoggedInUserFullName(),
                                insideOfPfp = this.GetCurrentlyLoggedInUserFullName().Split(' ')[0].Substring(0, 1) + this.GetCurrentlyLoggedInUserFullName().Split(' ')[1].Substring(0, 1),
                                reply = reader["reply"].ToString(),
                                likes = reader["likes"].ToString(),
                                date = GetRelativeTime(reader["date"].ToString()),  // Use relative time string here
                                colorValue = colorValue
                            };

                            replies.Add(reply);
                        }
                    }
                    connection.Close();
                }
            }
            return replies;
        }

        public string GetRelativeTime(string originalTimestamp)
        {
            DateTime postTime = DateTime.ParseExact(originalTimestamp, "MMMM dd, yyyy, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            DateTime currentTime = DateTime.Now;

            TimeSpan timeDifference = currentTime - postTime;

            if (timeDifference.TotalDays >= 1)
            {
                return $"{(int)timeDifference.TotalDays}d";
            }
            else if (timeDifference.TotalHours >= 1)
            {
                return $"{(int)timeDifference.TotalHours}h";
            }
            else if (timeDifference.TotalMinutes >= 1)
            {
                return $"{(int)timeDifference.TotalMinutes}m";
            }
            else
            {
                return "Just now";
            }
        }

        // After logging in
        protected void GetBoard()
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM boards WHERE user_id = @user_id AND board_name = 'My first board'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("user_id", user_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            this.setBoardId(reader["board_id"].ToString());
                            this.BoardTitle.InnerText = reader["board_name"].ToString();
                        }
                    }
                }
            }
        }

        // Board Related
        protected void AddBoard(object sender, EventArgs e)
        {
            string boardName = this.BoardNameInput.Value;

            if (boardName != null)
            {
                const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
                int user_id = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO boards (board_name, user_id) VALUES (@board_name, @user_id)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@board_name", boardName);
                        command.Parameters.AddWithValue("@user_id", user_id);

                        command.ExecuteNonQuery();

                        connection.Close();
                    }

                    // New query
                    string board_id = "";

                    string getCreatedBoardIdQuery = "SELECT board_id FROM boards WHERE board_name = @board_name";
                    using (SqlCommand getCreatedBoardIdCommand = new SqlCommand(getCreatedBoardIdQuery, connection))
                    {
                        connection.Open();

                        getCreatedBoardIdCommand.Parameters.AddWithValue("@board_name", boardName);
                        using (SqlDataReader reader = getCreatedBoardIdCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                board_id = reader["board_id"].ToString();

                            }
                        }
                    }

                    this.setBoardId(board_id);
                }
            }

            GetAllBoards();
        }

        protected void SelectBoard(object sender, EventArgs e)
        {
            LinkButton button = sender as LinkButton;
            Label buttonTextLabel = (Label)button.FindControl("BoardName");
            this.BoardHiddenField.Value = buttonTextLabel.Text;
            string board_name = this.BoardHiddenField.Value;

            if (board_name != null)
            {
                const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
                int user_id = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    int count = 0;
                    string query = "SELECT COUNT(*) FROM boards WHERE board_name = @board_name AND user_id = @user_id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@board_name", board_name);
                        command.Parameters.AddWithValue("@user_id", user_id);

                        count = (int)command.ExecuteScalar();

                        connection.Close();
                    }

                    if (count > 0)
                    {
                        string getBoardIdQuery = "SELECT * FROM boards WHERE board_name = @board_name AND user_id = @user_id";
                        using (SqlCommand getBoardIdCommand = new SqlCommand(getBoardIdQuery, connection))
                        {
                            connection.Open();

                            getBoardIdCommand.Parameters.AddWithValue("@board_name", board_name);
                            getBoardIdCommand.Parameters.AddWithValue("@user_id", user_id);

                            using (SqlDataReader reader = getBoardIdCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string board_id = reader["board_id"].ToString();
                                    this.setBoardId(board_id);
                                }
                            }
                        }
                    }
                }

                this.BoardTitle.InnerText = board_name;
                UpdateTable();
            }
        }

        protected void SearchBoard(object sender, EventArgs e)
        {
            string searchText = this.BoardSearchInput.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
                int user_id = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM boards WHERE board_name LIKE @boardNameToSearch AND user_id = @user_id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        // Add wildcard characters to the parameter value
                        command.Parameters.AddWithValue("@boardNameToSearch", "%" + searchText + "%");
                        command.Parameters.AddWithValue("@user_id", user_id);

                        List<string> boardNames = new List<string>();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                boardNames.Add(reader["board_name"].ToString());
                            }
                        }

                        BoardsRepeater.DataSource = boardNames;
                        BoardsRepeater.DataBind();
                    }
                }
            }
        }

        // Task Related
        protected void AddTask(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            string task = this.taskTextBox.Text;
            string username = this.GetCurrentlyLoggedInUsername();
            string workspace_name = "Main Workspace";
            string board_name = this.BoardHiddenField.Value;
            int board_id = Convert.ToInt32(this.getBoardId());
            int team_id = Convert.ToInt32(this.GetCurrentlyLoggedInUsersTeamId());

            if (task != null)
            {               
                using (SqlConnection connection = new SqlConnection(connectionString))
                {                   
                    string query = 
                        $"INSERT INTO dataOfTables (user_id, workspace_name, data, data_type, column_name, board_id) " +
                        $"VALUES (@user_id, @workspace_name, @data, @data_type, @column_name, @board_id)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@workspace_name", workspace_name);
                        command.Parameters.AddWithValue("@data", task);
                        command.Parameters.AddWithValue("@data_type", "Task");
                        command.Parameters.AddWithValue("@column_name", "Task");
                        command.Parameters.AddWithValue("@board_id", this.getBoardId());

                        command.ExecuteNonQuery();

                        connection.Close();
                    }

                    string column_name = "";
                    string data_type = "";
                    
                    string getDataFromDatasTableQuery = "SELECT * FROM columnsOfTables WHERE user_id = @user_id AND board_id = @board_id";
                    using (SqlCommand command = new SqlCommand(getDataFromDatasTableQuery, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@board_id", this.getBoardId());
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                column_name = reader["column_name"].ToString();
                                data_type = reader["data_type"].ToString();

                                if (data_type.Equals("Status"))
                                {
                                    InsertDataToDatabase(user_id, "Main Workspace", "Not Started", data_type, column_name, Convert.ToInt32(this.getBoardId()));
                                }
                                else if (data_type.Equals("Date"))
                                {
                                    DateTime todaysDate = DateTime.Now;
                                    InsertDataToDatabase(user_id, "Main Workspace", todaysDate.ToString("MMMM dd, yyyy"), data_type, column_name, Convert.ToInt32(this.getBoardId()));
                                }
                                else if (data_type.Equals("People"))
                                {
                                    InsertDataToDatabase(user_id, "Main Workspace", "NULL", data_type, column_name, Convert.ToInt32(this.getBoardId()));
                                }
                            }
                        }
                    }
                }

                // For New Table
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = $"INSERT INTO {username}_{team_id}_{board_id} (Task) VALUES (@task)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@task", task);
                        command.ExecuteNonQuery();

                        connection.Close();
                    }

                    int taskId = 0;
                    string getTaskIdQuery = $"SELECT id FROM {username}_{team_id}_{board_id}";
                    using (SqlCommand command = new SqlCommand(getTaskIdQuery, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                taskId = Convert.ToInt32(reader["id"].ToString());
                            }
                        }

                        connection.Close();
                    }

                    string column_name = "";
                    string data_type = "";

                    string getDataFromDatasTableQuery = "SELECT * FROM columnsOfTables WHERE user_id = @user_id AND board_id = @board_id";
                    using (SqlCommand command = new SqlCommand(getDataFromDatasTableQuery, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@board_id", this.getBoardId());
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                column_name = reader["column_name"].ToString();
                                data_type = reader["data_type"].ToString();

                                if (data_type.Equals("Status"))
                                {
                                    InsertDataToTable(user_id, column_name, "Not Started", data_type, taskId);
                                }
                                else if (data_type.Equals("Date"))
                                {
                                    DateTime todaysDate = DateTime.Now;
                                    InsertDataToTable(user_id, column_name, todaysDate.ToString("MMMM dd, yyyy"), data_type, taskId);
                                }
                                else if (data_type.Equals("People"))
                                {
                                    InsertDataToTable(user_id, column_name, "NULL", data_type, taskId);
                                }
                            }
                        }
                    }                    
                }

                // UpdateTable();
                UpdatedUpdateTable();
            }
        }


        protected void EditTask(object sender, EventArgs e)
        {
            TextBox textBoxWhereTaskNeedsEditing = sender as TextBox;

            if (textBoxWhereTaskNeedsEditing.Text != null)
            {
                string id = textBoxWhereTaskNeedsEditing.ID.Split('-')[2];
                string editedTask = textBoxWhereTaskNeedsEditing.Text;

                const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
                int user_id = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE dataOfTables SET data = @task WHERE id = @id AND user_id = @user_id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@task", editedTask);
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@user_id", user_id);

                        command.ExecuteNonQuery();
                    }
                }
                UpdatedUpdateTable();
            }
        }



        protected void DeleteTask(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string id = this.getId().Split('-')[2];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM assignedTasks WHERE task_id = @task_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@task_id", id);
                    command.ExecuteNonQuery();

                    connection.Close();
                }

                bool taskWasDeleted = false;
                string getDataQuery = @"WITH RankedData AS (
    SELECT d.*, 
           ROW_NUMBER() OVER (PARTITION BY d.data_type ORDER BY c.id) AS rank
    FROM dataOfTables d
    INNER JOIN columnsOfTables c ON d.column_name = c.column_name
    WHERE d.user_id = @user_id
)
SELECT *
FROM RankedData
ORDER BY rank DESC, data_type DESC;"";
";
                using (SqlCommand command = new SqlCommand(getDataQuery, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);

                }
            }

            UpdatedUpdateTable();
        }

        protected void DuplicateTask(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string id = this.getId().Split('-')[2];

            if (id != null && Convert.ToInt32(id) > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM mainWorkSpace WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            string task = "";
                            string date = "";
                            string status = "";
                            string board_id = "";

                            while (reader.Read())
                            {
                                task = reader["task"].ToString();
                                date = reader["date"].ToString();
                                status = reader["status"].ToString();
                                board_id = reader["board_id"].ToString();
                            }

                            connection.Close();

                            string insertQuery = "INSERT INTO mainWorkSpace (task, date, status, board_id, user_id) VALUES (@task, @date, @status, @board_id, @user_id)";
                            using (SqlCommand newCommand = new SqlCommand(insertQuery, connection))
                            {
                                connection.Open();

                                newCommand.Parameters.AddWithValue("@task", task);
                                newCommand.Parameters.AddWithValue("@date", date);
                                newCommand.Parameters.AddWithValue("@status", status);
                                newCommand.Parameters.AddWithValue("@board_id", board_id);
                                newCommand.Parameters.AddWithValue("@user_id", user_id);

                                newCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                UpdateTable();
            }
        }

        protected void DuplicateTaskWithoutUpdates(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string id = this.getId().Split('-')[2];

            if (id != null && Convert.ToInt32(id) > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM mainWorkSpace WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            string task = "";
                            string date = "";
                            string status = "";
                            string board_id = "";

                            while (reader.Read())
                            {
                                task = reader["task"].ToString();
                                date = "NULL";
                                status = "Not Started";
                                board_id = reader["board_id"].ToString();
                            }

                            connection.Close();

                            string insertQuery = "INSERT INTO mainWorkSpace (task, date, status, board_id, user_id) VALUES (@task, @date, @status, @board_id, @user_id)";
                            using (SqlCommand newCommand = new SqlCommand(insertQuery, connection))
                            {
                                connection.Open();

                                newCommand.Parameters.AddWithValue("@task", task);
                                newCommand.Parameters.AddWithValue("@date", date);
                                newCommand.Parameters.AddWithValue("@status", status);
                                newCommand.Parameters.AddWithValue("@board_id", board_id);
                                newCommand.Parameters.AddWithValue("@user_id", user_id);

                                newCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
                UpdateTable();
            }
        }

        // Add Column Button Related
        protected void AddStatusToColumn(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            int board_id = Convert.ToInt32(this.getBoardId());

            Button button = sender as Button;
            string data_type = button.Text;
            string column_name = button.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int multipleDataTypesCounter = 0;

                string checkIfColumnExistsQuery = "SELECT COUNT(@column_name) FROM columnsOfTables WHERE user_id = @user_id";
                using (SqlCommand checkIfColumnExistsCommand = new SqlCommand(checkIfColumnExistsQuery, connection))
                {
                    connection.Open();

                    checkIfColumnExistsCommand.Parameters.AddWithValue("@column_name", column_name);
                    checkIfColumnExistsCommand.Parameters.AddWithValue("@user_id", user_id);

                    int count = (int)checkIfColumnExistsCommand.ExecuteScalar();
                    bool ifDataTypeDoesExist = count > 0;

                    connection.Close();

                    while (ifDataTypeDoesExist)
                    {
                        column_name += multipleDataTypesCounter++;

                        int countAgain = 0;

                        string checkIfColumnExistsAgainQuery = "SELECT COUNT(@column_name) FROM columnsOfTables WHERE user_id = @user_id";
                        using (SqlCommand checkIfColumnExistsAgainCommand = new SqlCommand(checkIfColumnExistsAgainQuery, connection))
                        {
                            connection.Open();

                            checkIfColumnExistsAgainCommand.Parameters.AddWithValue("@column_name", column_name);
                            checkIfColumnExistsAgainCommand.Parameters.AddWithValue("@user_id", user_id);

                            countAgain = (int)checkIfColumnExistsAgainCommand.ExecuteScalar();

                            connection.Close();
                        }

                        if (countAgain > 0) break;
                        else continue;
                    }
                }

                string insertColumnToDBQuery = "INSERT INTO columnsOfTables (column_name, user_id, data_type) VALUES (@column_name, @user_id, @data_type)";
                using (SqlCommand insertColumnToDBCommand = new SqlCommand(insertColumnToDBQuery, connection))
                {
                    connection.Open();

                    insertColumnToDBCommand.Parameters.AddWithValue("@column_name", column_name);
                    insertColumnToDBCommand.Parameters.AddWithValue("@user_id", user_id);
                    insertColumnToDBCommand.Parameters.AddWithValue("@data_type", data_type);
                    insertColumnToDBCommand.ExecuteNonQuery();

                    connection.Close();
                }

                int tasksCounter = 0;
                string checkHowManyTasksQuery = "SELECT Count(*) FROM dataOfTables WHERE data_type = 'Task' AND user_id = @user_id";
                using (SqlCommand checkHowManyTasksCommand = new SqlCommand(checkHowManyTasksQuery, connection))
                {
                    connection.Open();

                    checkHowManyTasksCommand.Parameters.AddWithValue("@user_id", user_id);
                    tasksCounter = (int)checkHowManyTasksCommand.ExecuteScalar();

                    connection.Close();
                }

                for (int i = 0; i < tasksCounter; i++)
                {
                    InsertDataToDatabase(user_id, "Main Workspace", "Not Started", data_type, column_name, board_id);
                }
            }

            // UpdateTable();
            UpdatedUpdateTable();
        }

        protected void AddDateToColumn(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            int board_id = Convert.ToInt32(this.getBoardId());
            DateTime todaysDate = DateTime.Now;

            Button button = sender as Button;
            string column_name = button.Text;
            string data_type = button.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int multipleDataTypesCounter = 0;

                string checkIfDateColumnExistsQuery = "SELECT COUNT(@column_name) FROM columnsOfTables WHERE user_id = @user_id";
                using (SqlCommand command = new SqlCommand(checkIfDateColumnExistsQuery, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@column_name", column_name);
                    command.Parameters.AddWithValue("@user_id", user_id);

                    int count = (int)command.ExecuteScalar();
                    bool ifDataTypeDoesExist = count > 0;

                    connection.Close();

                    while (ifDataTypeDoesExist)
                    {
                        column_name += multipleDataTypesCounter++;

                        int countAgain = 0;

                        string checkIfColumnExistsAgainQuery = "SELECT COUNT(@column_name) FROM columnsOfTables WHERE user_id = @user_id";
                        using (SqlCommand checkIfColumnExistsAgainCommand = new SqlCommand(checkIfColumnExistsAgainQuery, connection))
                        {
                            connection.Open();

                            checkIfColumnExistsAgainCommand.Parameters.AddWithValue("@column_name", column_name);
                            checkIfColumnExistsAgainCommand.Parameters.AddWithValue("@user_id", user_id);

                            countAgain = (int)checkIfColumnExistsAgainCommand.ExecuteScalar();

                            connection.Close();
                        }

                        if (countAgain > 0) break;
                        else continue;
                    }
                }

                string insertColumnToDBQuery = "INSERT INTO columnsOfTables (column_name, user_id, data_type, board_id) VALUES (@column_name, @user_id, @data_type, @board_id)";
                using (SqlCommand insertColumnToDBCommand = new SqlCommand(insertColumnToDBQuery, connection))
                {
                    connection.Open();

                    insertColumnToDBCommand.Parameters.AddWithValue("@column_name", column_name);
                    insertColumnToDBCommand.Parameters.AddWithValue("@user_id", user_id);
                    insertColumnToDBCommand.Parameters.AddWithValue("@data_type", data_type);
                    insertColumnToDBCommand.Parameters.AddWithValue("@board_id", board_id);
                    insertColumnToDBCommand.ExecuteNonQuery();

                    connection.Close();
                }

                int tasksCounter = 0;
                string checkHowManyTasksQuery = "SELECT Count(*) FROM dataOfTables WHERE data_type = 'Task' AND user_id = @user_id";
                using (SqlCommand checkHowManyTasksCommand = new SqlCommand(checkHowManyTasksQuery, connection))
                {
                    connection.Open();

                    checkHowManyTasksCommand.Parameters.AddWithValue("@user_id", user_id);
                    tasksCounter = (int)checkHowManyTasksCommand.ExecuteScalar();

                    connection.Close();
                }

                for (int i = 0; i < tasksCounter; i++)
                {
                    InsertDataToDatabase(user_id, "Main Workspace", todaysDate.ToString("MMMM dd, yyyy"), data_type, column_name, board_id);
                }
            }

            UpdatedUpdateTable();
        }

        protected void AddPeopleToColumn(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            Button button = sender as Button;
            string column_name = button.Text;
            string data_type = button.Text;

            int multipleDataTypesCounter = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string checkIfPeopleColumnExist = "SELECT COUNT(@column_name) FROM columnsOfTables WHERE user_id = @user_id";
                using (SqlCommand command = new SqlCommand(checkIfPeopleColumnExist, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@column_name", column_name);
                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.ExecuteNonQuery();

                    int count = (int)command.ExecuteScalar();

                    connection.Close();

                    bool ifDataTypeDoesExist = count > 0;

                    while(ifDataTypeDoesExist)
                    {
                        column_name += multipleDataTypesCounter++;

                        int countAgain = 0;

                        string checkIfColumnExistsAgainQuery = "SELECT COUNT(@column_name) FROM columnsOfTables WHERE user_id = @user_id";
                        using (SqlCommand checkIfColumnExistsAgainCommand = new SqlCommand(checkIfColumnExistsAgainQuery, connection))
                        {
                            connection.Open();

                            checkIfColumnExistsAgainCommand.Parameters.AddWithValue("@column_name", column_name);
                            checkIfColumnExistsAgainCommand.Parameters.AddWithValue("@user_id", user_id);

                            countAgain = (int)checkIfColumnExistsAgainCommand.ExecuteScalar();

                            connection.Close();
                        }

                        if (countAgain > 0) break;
                        else continue;
                    }

                    string insertColumnToDBQuery = "INSERT INTO columnsOfTables (column_name, user_id, data_type, board_id) VALUES (@column_name, @user_id, @data_type, @board_id)";
                    using (SqlCommand insertColumnToDBCommand = new SqlCommand(insertColumnToDBQuery, connection))
                    {
                        connection.Open();

                        insertColumnToDBCommand.Parameters.AddWithValue("@column_name", column_name);
                        insertColumnToDBCommand.Parameters.AddWithValue("@user_id", user_id);
                        insertColumnToDBCommand.Parameters.AddWithValue("@data_type", data_type);
                        insertColumnToDBCommand.Parameters.AddWithValue("@board_id", this.getBoardId());
                        insertColumnToDBCommand.ExecuteNonQuery();

                        connection.Close();
                    }

                    int tasksCounter = 0;
                    string checkHowManyTasksQuery = "SELECT Count(*) FROM dataOfTables WHERE data_type = 'Task' AND user_id = @user_id";
                    using (SqlCommand checkHowManyTasksCommand = new SqlCommand(checkHowManyTasksQuery, connection))
                    {
                        connection.Open();

                        checkHowManyTasksCommand.Parameters.AddWithValue("@user_id", user_id);
                        tasksCounter = (int)checkHowManyTasksCommand.ExecuteScalar();

                        connection.Close();
                    }

                    for (int i = 0; i < tasksCounter; i++)
                    {
                        
                        InsertDataToDatabase(user_id, "Main Workspace", "NULL", data_type, column_name, Convert.ToInt32(this.getBoardId()));
                    }
                }

                // New Query
                string email = "";
                string getEmailQuery = "SELECT email FROM userInfo WHERE id = @user_id";
                using (SqlCommand getEmailCommand = new SqlCommand(getEmailQuery, connection))
                {
                    connection.Open();

                    getEmailCommand.Parameters.AddWithValue("@user_id", user_id);

                    using (SqlDataReader reader = getEmailCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            email = reader["email"].ToString();
                        }
                    }

                    connection.Close();
                }

                // New Query
                string insertQuery = "UPDATE mainWorkSpace SET people = @email WHERE user_id = @user_id";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    connection.Open();

                    insertCommand.Parameters.AddWithValue("@email", email);
                    insertCommand.Parameters.AddWithValue("@user_id", user_id);

                    insertCommand.ExecuteNonQuery();

                    connection.Close();
                }
            }

            UpdatedUpdateTable();
        }

        // Status Related
        protected void ChangeStatus(object sender, EventArgs e)
        {
            Button button = sender as Button;
            this.setNewStatus(button.Text);

            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string id = this.getId().Split('-')[2];
            string status = this.getNewStatus();

            if (id != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE dataOfTables SET data = @status WHERE user_id = @user_id AND id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }
                }

                UpdatedUpdateTable();
            }
        }

        // Date Related
        protected void ChangeDate(object sender, EventArgs e)
        {
            DateTime selectedDate = this.Calendar.SelectedDate;

            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string id = this.getId().Split('-')[2];

            if (id != null && selectedDate.ToString() != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE dataOfTables SET data = @selectedDate WHERE user_id = @user_id AND id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@selectedDate", selectedDate.ToString("MMMM dd, yyyy"));
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();

                        connection.Close();
                    }
                }

                UpdatedUpdateTable();
            }
        }

        // People Related

        protected void AssignTask(object sender, EventArgs e)
        {
            Button button = sender as Button;

            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int member_id = GetSpecificTeamMemberFromDB(button.Text);

            string task_id = this.getId().Split('-')[2];
            string email = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string getEmailQuery = "SELECT email FROM userInfo WHERE id = @id";
                using (SqlCommand getTeamIdCommand = new SqlCommand(getEmailQuery, connection))
                {
                    connection.Open();

                    getTeamIdCommand.Parameters.AddWithValue("@id", member_id);
                    using (SqlDataReader reader = getTeamIdCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            email = reader["email"].ToString();
                        }
                    }

                    connection.Close();
                }

                string query = "INSERT INTO assignedTasks (task_id, member_id, member_email) VALUES (@task_id, @member_id, @member_email)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@task_id", task_id);
                    command.Parameters.AddWithValue("@member_id", member_id);
                    command.Parameters.AddWithValue("@member_email", email);

                    command.ExecuteNonQuery();
                }
            }

            UpdatedUpdateTable();
        }

        protected void InviteNewTeamMember(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string teamMemberEmail = this.NewTeamMemberInput.Value;

            if (teamMemberEmail != null && teamMemberEmail.Length > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    int teamMemberId = 0;
                    int teamId = 0;

                    string checkIfUserExistQuery = "SELECT COUNT(*) FROM userInfo WHERE email = @email";
                    using (SqlCommand checkIfUserExistCommand = new SqlCommand(checkIfUserExistQuery, connection))
                    {
                        connection.Open();

                        checkIfUserExistCommand.Parameters.AddWithValue("@email", teamMemberEmail);

                        int count = (int)checkIfUserExistCommand.ExecuteScalar();

                        connection.Close();

                        if (count > 0)
                        {
                            string getUserIdQuery = "SELECT id FROM userInfo WHERE email = @email";
                            using (SqlCommand getUserIdCommand = new SqlCommand(getUserIdQuery, connection))
                            {
                                connection.Open();

                                getUserIdCommand.Parameters.AddWithValue("@email", teamMemberEmail.ToLower());
                                using (SqlDataReader reader = getUserIdCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        teamMemberId = Convert.ToInt32(reader["id"].ToString());
                                    }
                                }

                                connection.Close();
                            }

                            string getTeamIdQuery = "SELECT team_id FROM teams WHERE user_id = @user_id";
                            using (SqlCommand getTeamIdCommand = new SqlCommand(getTeamIdQuery, connection))
                            {
                                connection.Open();

                                getTeamIdCommand.Parameters.AddWithValue("@user_id", user_id);
                                using (SqlDataReader reader = getTeamIdCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        teamId = Convert.ToInt32(reader["team_id"].ToString());
                                    }
                                }

                                connection.Close();
                            }

                            string addNewTeamMemberQuery = "SET IDENTITY_INSERT teams ON; INSERT INTO teams (team_id, user_id, user_email, role) VALUES (@team_id, @user_id, @user_email, @role)";
                            using (SqlCommand addNewTeamMemberCommmand = new SqlCommand(addNewTeamMemberQuery, connection))
                            {
                                connection.Open();

                                addNewTeamMemberCommmand.Parameters.AddWithValue("@team_id", teamId);
                                addNewTeamMemberCommmand.Parameters.AddWithValue("@user_id", teamMemberId);
                                addNewTeamMemberCommmand.Parameters.AddWithValue("@user_email", teamMemberEmail);
                                addNewTeamMemberCommmand.Parameters.AddWithValue("@role", "admin");
                                addNewTeamMemberCommmand.ExecuteNonQuery();

                                connection.Close();
                            }
                        }
                    }
                }
            }
        }

        // Discussions Related
        protected void PostOnDiscussion(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            string username = this.GetCurrentlyLoggedInUsername();
            string postText = this.PostTextbox.Text;
            string task_id = this.getId().Split('-')[2];
            string dateToday = GetCurrentDayDate().ToString("MMMM dd, yyyy, hh:mm tt");

            if (postText != null && username != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO discussions (task_id, post, username, user_id, date, likes) VALUES (@task_id, @post, @username, @user_id, @date, @likes)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@task_id", task_id);
                        command.Parameters.AddWithValue("@post", postText);
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@date", dateToday);
                        command.Parameters.AddWithValue("@likes", 0);
                        command.ExecuteNonQuery();

                        connection.Close();
                    }
                }
            }

            DisplayPostsOfTasks(task_id);
        }

        protected void ReplyToPost(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            string post_id = (string)btn.CommandArgument; // Assuming post_id is stored in CommandArgument
            string username = this.GetCurrentlyLoggedInUsername();
            string dateToday = GetCurrentDayDate().ToString("MMMM dd, yyyy, hh:mm tt");
            string replyText = "";

            // Accessing Reply Text
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;
            TextBox replyTextBox = (TextBox)item.FindControl("ReplyTextBox");

            if (replyTextBox != null)
            {
                replyText = replyTextBox.Text;

                // Store in DB
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO replies (post_id, username, reply, date) VALUES (@post_id, @username, @reply, @date)";
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@post_id", post_id);
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@reply", replyText);
                        command.Parameters.AddWithValue("@date", dateToday);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }

                // Refresh Replies Repeater
                RefreshReplies(post_id, item);
            }
        }

        private void RefreshReplies(string post_id, RepeaterItem item)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            List<Reply> replies = new List<Reply>();

            // Fetch Replies from DB
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string getRepliesQuery = "SELECT * FROM replies WHERE post_id = @post_id";
                connection.Open();
                using (SqlCommand getRepliesCommand = new SqlCommand(getRepliesQuery, connection))
                {
                    getRepliesCommand.Parameters.AddWithValue("@post_id", post_id);
                    using (SqlDataReader reader = getRepliesCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Reply reply = new Reply
                            {
                                replyID = reader["id"].ToString(),
                                username = reader["username"].ToString(),
                                reply = reader["reply"].ToString(),
                                likes = reader["likes"].ToString(),
                                date = reader["date"].ToString()
                            };
                            replies.Add(reply);
                        }
                    }
                }
            }

            // Bind Replies Repeater
            Repeater repliesRepeater = (Repeater)item.FindControl("RepliesRepeater");
            if (repliesRepeater != null)
            {
                repliesRepeater.DataSource = replies;
                repliesRepeater.DataBind();
            }
        }

        protected void LikePost(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;

            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string post_id = (string)button.CommandArgument;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if the user has already liked the post
                string checkIfUserHasLikedQuery = "SELECT liked FROM usersWhoLikedPosts WHERE post_id = @post_id AND user_id = @user_id";
                using (SqlCommand checkIfUserHasLikedCommand = new SqlCommand(checkIfUserHasLikedQuery, connection))
                {
                    checkIfUserHasLikedCommand.Parameters.AddWithValue("@post_id", post_id);
                    checkIfUserHasLikedCommand.Parameters.AddWithValue("@user_id", user_id);

                    object result = checkIfUserHasLikedCommand.ExecuteScalar();
                    if (result != null)
                    {
                        bool liked = Convert.ToBoolean(result);

                        // Toggle the like status
                        string updateLikeStatusQuery = "UPDATE usersWhoLikedPosts SET liked = @liked WHERE post_id = @post_id AND user_id = @user_id";
                        using (SqlCommand updateLikeStatusCommand = new SqlCommand(updateLikeStatusQuery, connection))
                        {
                            updateLikeStatusCommand.Parameters.AddWithValue("@liked", !liked);
                            updateLikeStatusCommand.Parameters.AddWithValue("@post_id", post_id);
                            updateLikeStatusCommand.Parameters.AddWithValue("@user_id", user_id);
                            updateLikeStatusCommand.ExecuteNonQuery();
                        }

                        // Update the like counter
                        string updateLikeCounterQuery = liked ?
                            "UPDATE discussions SET likes = likes + 1 WHERE id = @post_id" :
                            "UPDATE discussions SET likes = likes - 1 WHERE id = @post_id";
                        using (SqlCommand updateLikeCounterCommand = new SqlCommand(updateLikeCounterQuery, connection))
                        {
                            updateLikeCounterCommand.Parameters.AddWithValue("@post_id", post_id);
                            updateLikeCounterCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Insert a new like record
                        string insertLikeRecordQuery = "INSERT INTO usersWhoLikedPosts (post_id, user_id, liked) VALUES (@post_id, @user_id, 1)";
                        using (SqlCommand insertLikeRecordCommand = new SqlCommand(insertLikeRecordQuery, connection))
                        {
                            insertLikeRecordCommand.Parameters.AddWithValue("@post_id", post_id);
                            insertLikeRecordCommand.Parameters.AddWithValue("@user_id", user_id);
                            insertLikeRecordCommand.ExecuteNonQuery();
                        }

                        // Increment the like counter
                        string incrementLikeCounterQuery = "UPDATE discussions SET likes = likes + 1 WHERE id = @post_id";
                        using (SqlCommand incrementLikeCounterCommand = new SqlCommand(incrementLikeCounterQuery, connection))
                        {
                            incrementLikeCounterCommand.Parameters.AddWithValue("@post_id", post_id);
                            incrementLikeCounterCommand.ExecuteNonQuery();
                        }
                    }
                }

                connection.Close();
            }
        }

        protected void EditPost(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string task_id = this.getId().Split('-')[2];

            // Retrieve the repeater item and the controls inside it
            RepeaterItem item = (sender as TextBox).NamingContainer as RepeaterItem;
            Label lblOldPost = item.FindControl("Post") as Label;
            TextBox txtEditPost = item.FindControl("EditPostTextbox") as TextBox;

            // Retrieve values
            string oldPost = lblOldPost.Text;
            string editedPost = txtEditPost.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE discussions SET post = @editedPost WHERE task_id = @task_id AND post = @oldPost AND user_id = @user_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@editedPost", editedPost);
                    command.Parameters.AddWithValue("@task_id", task_id);
                    command.Parameters.AddWithValue("@oldPost", oldPost);
                    command.Parameters.AddWithValue("@user_id", user_id);

                    command.ExecuteNonQuery();
                }
            }

            // Refresh the posts to show updated data
            DisplayPostsOfTasks(task_id);
        }


        protected void DeletePost(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;

            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string post_id = (string)button.CommandArgument;
            string task_id = this.getId().Split('-')[2];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string deleteLikedRowQuery = "DELETE FROM usersWhoLikedPosts WHERE post_id = @post_id AND user_id = @user_id";
                using (SqlCommand deleteLikedRowCommand = new SqlCommand(deleteLikedRowQuery, connection))
                {
                    connection.Open();

                    deleteLikedRowCommand.Parameters.AddWithValue("@post_id", post_id);
                    deleteLikedRowCommand.Parameters.AddWithValue("@user_id", user_id);
                    deleteLikedRowCommand.ExecuteNonQuery();

                    connection.Close();
                }

                string deleteRepliesOfPostQuery = "DELETE FROM replies WHERE post_id = @post_id";
                using (SqlCommand deleteRepliesOfPostCommand = new SqlCommand(deleteRepliesOfPostQuery, connection))
                {
                    connection.Open();

                    deleteRepliesOfPostCommand.Parameters.AddWithValue("@post_id", post_id);
                    deleteRepliesOfPostCommand.ExecuteNonQuery();

                    connection.Close();
                }

                string deletePostQuery = "DELETE FROM discussions WHERE id = @post_id AND user_id = @user_id";
                using (SqlCommand deletePostCommand = new SqlCommand(deletePostQuery, connection))
                {
                    connection.Open();

                    deletePostCommand.Parameters.AddWithValue("@post_id", post_id);
                    deletePostCommand.Parameters.AddWithValue("@user_id", user_id);
                    deletePostCommand.ExecuteNonQuery();

                    connection.Close();
                }
            }
            DisplayPostsOfTasks(task_id);
        }

        protected void EditReply(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string task_id = this.getId().Split('-')[2];
            string username = GetCurrentlyLoggedInUsername();
            string post_id = "";

            // Retrieve the RepeaterItem and the controls inside it
            RepeaterItem replyItem = (sender as TextBox).NamingContainer as RepeaterItem;
            Repeater repliesRepeater = replyItem.NamingContainer as Repeater;
            RepeaterItem postItem = repliesRepeater.NamingContainer as RepeaterItem;

            Label postLabel = postItem.FindControl("Post") as Label;
            Label oldReplyLabel = replyItem.FindControl("ReplyText") as Label;
            TextBox editedReplyTextBox = replyItem.FindControl("EditReplyTextBox") as TextBox;

            string post = postLabel.Text;
            string oldReply = oldReplyLabel.Text;
            string editedReply = editedReplyTextBox.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string getPostIdQuery = "SELECT id FROM discussions WHERE post = @post AND user_id = @user_id";
                using (SqlCommand getPostIdCommand = new SqlCommand(getPostIdQuery, connection))
                {
                    connection.Open();

                    getPostIdCommand.Parameters.AddWithValue("@post", post);
                    getPostIdCommand.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader = getPostIdCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            post_id = reader["id"].ToString();
                        }
                    }

                    connection.Close();
                }

                string query = "UPDATE replies SET reply = @editedReply WHERE username = @username AND reply = @oldReply AND post_id = @post_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@editedReply", editedReply);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@oldReply", oldReply);
                    command.Parameters.AddWithValue("@post_id", post_id);

                    command.ExecuteNonQuery();
                }
            }

            DisplayPostsOfTasks(task_id);
            GetRepliesForPost(post_id);
        }

        protected void DeleteReply(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string task_id = this.getId().Split('-')[2];
            string username = GetCurrentlyLoggedInUsername();
            string post_id = "";

            // Retrieve the RepeaterItem and the controls inside it
            RepeaterItem replyItem = (sender as LinkButton).NamingContainer as RepeaterItem;
            Repeater repliesRepeater = replyItem.NamingContainer as Repeater;
            RepeaterItem postItem = repliesRepeater.NamingContainer as RepeaterItem;

            Label postLabel = postItem.FindControl("Post") as Label;
            Label replyLabel = replyItem.FindControl("ReplyText") as Label;

            string post = postLabel.Text;
            string reply = replyLabel.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string getPostIdQuery = "SELECT id FROM discussions WHERE post = @post AND user_id = @user_id";
                using (SqlCommand getPostIdCommand = new SqlCommand(getPostIdQuery, connection))
                {
                    connection.Open();

                    getPostIdCommand.Parameters.AddWithValue("@post", post);
                    getPostIdCommand.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader = getPostIdCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            post_id = reader["id"].ToString();
                        }
                    }

                    connection.Close();
                }

                string query = "DELETE FROM replies WHERE reply = @reply AND post_id = @post_id AND username = @username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@reply", reply);
                    command.Parameters.AddWithValue("@post_id", post_id);
                    command.Parameters.AddWithValue("@username", username);

                    command.ExecuteNonQuery();
                }
            }

            DisplayPostsOfTasks(task_id);
            GetRepliesForPost(post_id);
        }

        protected void LikeReply(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;

            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string reply_id = (string)button.CommandArgument;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if the user has already liked the post
                string checkIfUserHasLikedQuery = "SELECT liked FROM userWhoLikedReplies WHERE reply_id = @reply_id AND user_id = @user_id";
                using (SqlCommand checkIfUserHasLikedCommand = new SqlCommand(checkIfUserHasLikedQuery, connection))
                {
                    checkIfUserHasLikedCommand.Parameters.AddWithValue("@reply_id", reply_id);
                    checkIfUserHasLikedCommand.Parameters.AddWithValue("@user_id", user_id);

                    object result = checkIfUserHasLikedCommand.ExecuteScalar();
                    if (result != null)
                    {
                        bool liked = Convert.ToBoolean(result);

                        // Toggle the like status
                        string updateLikeStatusQuery = "UPDATE userWhoLikedReplies SET liked = @liked WHERE reply_id = @reply_id AND user_id = @user_id";
                        using (SqlCommand updateLikeStatusCommand = new SqlCommand(updateLikeStatusQuery, connection))
                        {
                            updateLikeStatusCommand.Parameters.AddWithValue("@liked", !liked);
                            updateLikeStatusCommand.Parameters.AddWithValue("@reply_id", reply_id);
                            updateLikeStatusCommand.Parameters.AddWithValue("@user_id", user_id);
                            updateLikeStatusCommand.ExecuteNonQuery();
                        }

                        // Update the like counter
                        string updateLikeCounterQuery = liked ?
                            "UPDATE replies SET likes = likes + 1 WHERE id = @reply_id" :
                            "UPDATE replies SET likes = likes - 1 WHERE id = @reply_id";
                        using (SqlCommand updateLikeCounterCommand = new SqlCommand(updateLikeCounterQuery, connection))
                        {
                            updateLikeCounterCommand.Parameters.AddWithValue("@reply_id", reply_id);
                            updateLikeCounterCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Insert a new like record
                        string insertLikeRecordQuery = "INSERT INTO userWhoLikedReplies (reply_id, user_id, liked) VALUES (@reply_id, @user_id, 1)";
                        using (SqlCommand insertLikeRecordCommand = new SqlCommand(insertLikeRecordQuery, connection))
                        {
                            insertLikeRecordCommand.Parameters.AddWithValue("@reply_id", reply_id);
                            insertLikeRecordCommand.Parameters.AddWithValue("@user_id", user_id);
                            insertLikeRecordCommand.ExecuteNonQuery();
                        }

                        // Increment the like counter
                        string incrementLikeCounterQuery = "UPDATE replies SET likes = likes + 1 WHERE id = @reply_id";
                        using (SqlCommand incrementLikeCounterCommand = new SqlCommand(incrementLikeCounterQuery, connection))
                        {
                            incrementLikeCounterCommand.Parameters.AddWithValue("@reply_id", reply_id);
                            incrementLikeCounterCommand.ExecuteNonQuery();
                        }
                    }
                }

                connection.Close();
            }
        }

        // Board Related
        protected void RenameBoard(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            TextBox renameBoardTextBox = sender as TextBox;
            string oldBoardName = "";
            string editedBoardName = renameBoardTextBox.Text;

            if (renameBoardTextBox.Text != null)
            {
                var container = renameBoardTextBox.NamingContainer;
                Label boardNameLabel = (Label)container.FindControl("BoardName");
                if (boardNameLabel.Text != null)
                {
                    oldBoardName = boardNameLabel.Text;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE boards SET board_name = @editedBoardName WHERE board_name = @oldBoardName AND user_id = @user_id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@editedBoardName", editedBoardName);
                        command.Parameters.AddWithValue("@oldBoardName", oldBoardName);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.ExecuteNonQuery();

                        connection.Close();
                    }
                }
            }

            UpdatedUpdateTable();
        }

        // Buttons Related
        protected void OnOptionButtonClicked(object sender, EventArgs e)
        {

            Button button = sender as Button;
            string script = "document.querySelector('.options-menu').style.display = 'flex';";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "showOptionsMenu", script, true);

            this.setId(button.ID);

        }

        protected void OnAddColumnButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            string script = "document.querySelector('.columns-menu').removeAttribute('hidden');";
            ClientScript.RegisterStartupScript(this.GetType(), "showColumnsMenu", script, true);

            this.setId(button.ID);
        }

        protected void OnCurrentStatusButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            string script = "document.querySelector('.status-menu').style.display = 'flex';";
            ClientScript.RegisterStartupScript(this.GetType(), "showStatusMenu", script, true);

            string buttonID = button.ID;

            this.setId(buttonID);
        }

        protected void OnDateButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            // Registering the JavaScript to remove the 'hidden' attribute
            string script = @"
        var calendarMenu = document.querySelector('.calendar-menu');
        if (calendarMenu) {
            calendarMenu.removeAttribute('hidden');
        }
    ";
            ClientScript.RegisterStartupScript(this.GetType(), "showCalendarMenu", script, true);

            // Optionally, set some ID or other logic
            this.setId(button.ID);
        }

        protected void OnPeopleButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            string script = "document.querySelector('.people-menu').style.display = 'block';";
            ClientScript.RegisterStartupScript(this.GetType(), "showPeopleMenu", script, true);

            this.setId(button.ID);

            GetTeamMembersFromDB();
        }


        protected void OnInviteNewMemberButtonClicked(object sender, EventArgs e)
        {
            LinkButton button = sender as LinkButton;

            string script = @"
        document.querySelector('.add-new-member').removeAttribute('hidden');
        document.querySelector('.people-menu').setAttribute('hidden', 'hidden');
    ";
            ClientScript.RegisterStartupScript(this.GetType(), "showInviteMemberMenu", script, true);

            this.setId(button.ID);
        }

        protected void OnCancelButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            string script = @"
                document.querySelector('.add-new-member').setAttribute('hidden', 'hidden');
                document.querySelector('.people-menu').removeAttribute('hidden');
    ";
            ClientScript.RegisterStartupScript(this.GetType(), "showListOfMemberMenu", script, true);

            this.setId(button.ID);
        }

        protected void OnCommentsButtonClicked(object sender, EventArgs e)
        {
            LinkButton button = sender as LinkButton;

            string script = "document.querySelector('.task-discussion-menu').style = 'block';";
            ClientScript.RegisterStartupScript(this.GetType(), "showTaskDiscussionsMenu", script, true);

            this.setId(button.ID);
            this.GetTaskForTitle(this.getId().Split('-')[2]);
            this.PostProfilePicture.Text = this.GetCurrentlyLoggedInUserFullName().Split(' ')[0].Substring(0, 1) + this.GetCurrentlyLoggedInUserFullName().Split(' ')[1].Substring(0, 1);
            this.DisplayPostsOfTasks(this.getId().Split('-')[2]);

        }

        protected void OnCloseDiscussionsMenuButtonClicked(object sender, EventArgs e)
        {
            string script = "document.querySelector('.task-discussion-menu').attr('hidden', 'hidden');";
            ClientScript.RegisterStartupScript(this.GetType(), "closeTaskDiscussionsMenu", script, true);
        }

        protected void OnReplyButtonClicked(object sender, EventArgs e)
        {
            LinkButton button = sender as LinkButton;

            string script = $"document.querySelector('.reply').removeAttribute('hidden');";
            ClientScript.RegisterStartupScript(this.GetType(), "showReplyMenu", script, true);

            string post_id = button.CommandArgument;
            this.setId(post_id);
        }

        // Database related
        protected void GetAllBoards()
        {
            boards.Clear();

            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM boards WHERE user_id = @user_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("user_id", user_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Board board = new Board
                            {
                                boardID = reader["board_id"].ToString(),
                                boardName = reader["board_name"].ToString(),
                            };
                            boards.Add(board);
                        }
                    }
                }
            }

            BoardsRepeater.DataSource = boards;
            BoardsRepeater.DataBind();
        }

        protected void GetTeamMembersFromDB()
        {
            DisplayMembersWhoGotTaskAssigned(this.getId().Split('-')[2]);

            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int team_id = 0;
                string getTeamIdQuery = "SELECT team_id FROM teams WHERE user_id = @user_id";
                using (SqlCommand getTeamIdCommand = new SqlCommand(getTeamIdQuery, connection))
                {
                    connection.Open();

                    getTeamIdCommand.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader = getTeamIdCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            team_id = Convert.ToInt32(reader["team_id"].ToString());
                        }
                    }

                    connection.Close();
                }

                // New Query
                string getTeamMemebersQuery = "SELECT * FROM teams WHERE team_id = @team_id";
                using (SqlCommand getTeamMemebersCommand = new SqlCommand(getTeamMemebersQuery, connection))
                {
                    connection.Open();

                    getTeamMemebersCommand.Parameters.AddWithValue("@team_id", team_id);
                    using (SqlDataReader reader = getTeamMemebersCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string teamMember = reader["user_email"].ToString();

                            if (teamMembersWhoGotTaskAssigned.Contains(teamMember)) { continue; }
                            else { this.teamMembers.Add(teamMember); }

                            // this.teamMembers.Add(teamMember);
                        }
                    }

                    connection.Close();
                }
            }

            TeamMembersRepeater.DataSource = this.teamMembers;
            TeamMembersRepeater.DataBind();
        }

        protected int GetSpecificTeamMemberFromDB(string user_email)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int returned_id = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int team_id = 0;
                string getTeamIdQuery = "SELECT team_id FROM teams WHERE user_email = @user_email";
                using (SqlCommand getTeamIdCommand = new SqlCommand(getTeamIdQuery, connection))
                {
                    connection.Open();

                    getTeamIdCommand.Parameters.AddWithValue("@user_email", user_email);
                    using (SqlDataReader reader = getTeamIdCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            team_id = Convert.ToInt32(reader["team_id"].ToString());
                        }
                    }

                    connection.Close();
                }

                // New Query
                string getTeamMemebersQuery = "SELECT * FROM teams WHERE team_id = @team_id AND user_email = @user_email";
                using (SqlCommand getTeamMemebersCommand = new SqlCommand(getTeamMemebersQuery, connection))
                {
                    connection.Open();

                    getTeamMemebersCommand.Parameters.AddWithValue("@team_id", team_id);
                    getTeamMemebersCommand.Parameters.AddWithValue("@user_email", user_email);
                    using (SqlDataReader reader = getTeamMemebersCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returned_id = Convert.ToInt32(reader["user_id"].ToString());
                        }
                    }

                    connection.Close();
                }
            }

            return returned_id;
        }

        protected void DisplayMembersWhoGotTaskAssigned(string task_id)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM assignedTasks WHERE task_id = @task_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@task_id", task_id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            teamMembersWhoGotTaskAssigned.Add(reader["member_email"].ToString());
                            setTeammate(reader["member_email"].ToString());
                        }
                    }
                }
            }

            this.AssignedTeamMembers.DataSource = teamMembersWhoGotTaskAssigned;
            this.AssignedTeamMembers.DataBind();
        }

        protected void GetTaskForTitle(string task_id)
        {
            string titleForDiscussionsMenu = "";
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM dataOfTables WHERE id = @task_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@task_id", task_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            titleForDiscussionsMenu = reader["data"].ToString();
                        }
                    }
                }
            }

            this.TaskDiscussionMenuTitle.Text = titleForDiscussionsMenu;
        }

        protected string GetCurrentlyLoggedInUsername()
        {
            string username = string.Empty;
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT username FROM userInfo WHERE id = @user_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            username = reader["username"].ToString();
                        }
                    }
                }
            }

            return username;
        }

        protected string GetCurrentlyLoggedInUserFullName()
        {
            string full_name = string.Empty;
            string username = this.GetCurrentlyLoggedInUsername();
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection connection1 = new SqlConnection(connectionString))
            {
                string getFullNameQuery = "SELECT full_name FROM userInfo WHERE username = @username";
                using (SqlCommand getFullNameCommand = new SqlCommand(getFullNameQuery, connection1))
                {
                    connection1.Open();
                    getFullNameCommand.Parameters.AddWithValue("@username", username);
                    using (SqlDataReader reader1 = getFullNameCommand.ExecuteReader())
                    {
                        while (reader1.Read())
                        {
                            full_name += reader1["full_name"].ToString();
                        }
                    }
                }
            }

            return full_name;
        }

        protected string GetSpecificUserFullName(int user_id)
        {
            string full_name = string.Empty;
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string getFullNameQuery = "SELECT full_name FROM userInfo WHERE id = @user_id";
                using (SqlCommand getFullNameCommand = new SqlCommand(getFullNameQuery, connection))
                {
                    connection.Open();
                    getFullNameCommand.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader = getFullNameCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            full_name += reader["full_name"].ToString();
                        }
                    }
                }
            }

            return full_name;
        }

        protected void DisplayPostsOfTasks(string task_id)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            List<Post> posts = new List<Post>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string getUserPfpColorQuery = "SELECT user_pfp_color FROM userInfo WHERE id = @user_id";
                using (SqlCommand getUserPfpColorCommand = new SqlCommand(getUserPfpColorQuery, connection))
                {
                    connection.Open();

                    getUserPfpColorCommand.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader1 = getUserPfpColorCommand.ExecuteReader())
                    {
                        while (reader1.Read())
                        {
                            this.setColor(reader1["user_pfp_color"].ToString());
                        }
                    }

                    connection.Close();
                }

                string query = "SELECT * FROM discussions WHERE task_id = @task_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.Add("@task_id", SqlDbType.VarChar).Value = task_id;
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Post post = new Post
                            {
                                postID = reader["id"].ToString(),
                                username = this.GetCurrentlyLoggedInUserFullName(),
                                insideOfPfp = this.GetCurrentlyLoggedInUserFullName().Split(' ')[0].Substring(0, 1) + this.GetCurrentlyLoggedInUserFullName().Split(' ')[1].Substring(0, 1),
                                post = reader["post"].ToString(),
                                date = GetRelativeTime(reader["date"].ToString()),
                                likes = reader["likes"].ToString(),
                                colorValue = this.getColor(),
                            };

                            int r = Convert.ToInt32(post.colorValue.Split(',')[0]);
                            int g = Convert.ToInt32(post.colorValue.Split(',')[1]);
                            int b = Convert.ToInt32(post.colorValue.Split(',')[2]);
                            this.PostProfilePicture.BackColor = Color.FromArgb(r, g, b);
                            posts.Add(post);
                        }
                    }
                }
            }

            DiscussionsRepeater.DataSource = posts;
            DiscussionsRepeater.DataBind();
        }

        protected DateTime GetCurrentDayDate()
        {
            return DateTime.Now;
        }

        protected void InsertDataToDatabase(int user_id, string workspace_name, string data, string data_type, string column_name, int board_id)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string insertDataToTableQuery = "INSERT INTO dataOfTables (user_id, workspace_name, data, data_type, column_name, board_id) VALUES (@user_id, @workspace_name, @data, @data_type, @column_name, @board_id)";
                using (SqlCommand command = new SqlCommand(insertDataToTableQuery, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@workspace_name", workspace_name);
                    command.Parameters.AddWithValue("@data", data);
                    command.Parameters.AddWithValue("@data_type", data_type);
                    command.Parameters.AddWithValue("@column_name", column_name);
                    command.Parameters.AddWithValue("@board_id", board_id);
                    command.ExecuteNonQuery();

                    connection.Close();
                }                
            }
        }

        protected void InsertDataToTable(int user_id, string column_name, string data, string data_type, int taskId)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";

            int board_id = Convert.ToInt32(this.getBoardId());
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Insert Into New Table
                string username = this.GetCurrentlyLoggedInUsername();
                int team_id = this.GetCurrentlyLoggedInUsersTeamId();
                string insertToTableQuery = $"UPDATE {username}_{team_id}_{board_id} SET {column_name} = '{data}' WHERE id = {taskId}";
                using (SqlCommand command = new SqlCommand(insertToTableQuery, connection))
                {
                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }            
        }

        protected int GetCurrentlyLoggedInUsersTeamId() 
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            int team_id = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT team_id FROM teams WHERE user_id = @user_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            team_id = Convert.ToInt32(reader["team_id"].ToString());
                        }
                    }
                }
            }

            return team_id;
        }
        // Table Related
        protected void UpdateTable()
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM mainWorkSpace WHERE user_id = @user_id AND board_id = @board_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@board_id", this.getBoardId());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Clear the space for the updated rows excluding the taskTextBox
                        for (int i = Group.Rows.Count - 1; i > 1; i--)
                        {
                            Group.Rows.RemoveAt(i);
                        }

                        // Get column names from the sql table
                        DataTable schemaTable = reader.GetSchemaTable();
                        List<string> columnNames = new List<string>();

                        foreach (DataRow row in schemaTable.Rows)
                        {
                            string columnName = row.Field<string>("ColumnName");
                            if (columnName != "row_id" && columnName != "user_id") // Exclude 'row_id' and 'user_id'
                            {
                                columnNames.Add(columnName);
                            }
                        }

                        // For getting random colors
                        Random random = new Random();

                        while (reader.Read())
                        {
                            TableRow tableRow = new TableRow();
                            /*
                            foreach (string columnName in columnNames)
                            {
                                TableCell tableCell = new TableCell();

                                if (columnName == "task")
                                {
                                    // Adding Tasks To Table
                                    tableCell.CssClass = "task-text-box-cell";

                                    // Add the options button
                                    Button optionsButton = new Button();
                                    optionsButton.Text = "...";
                                    optionsButton.ID = $"options-btn-{reader["id"].ToString()}";
                                    optionsButton.CssClass = "options-btn";
                                    optionsButton.Click += OnOptionButtonClicked;
                                    tableCell.Controls.Add(optionsButton);

                                    // Add the text box
                                    TextBox textBox = new TextBox();
                                    textBox.Text = reader["task"].ToString();
                                    textBox.ID = $"taskTextBox-{reader["id"].ToString()}";
                                    textBox.CssClass = "task-text-box";
                                    textBox.TextChanged += EditTask;
                                    textBox.AutoPostBack = true;

                                    tableCell.Controls.Add(textBox);


                                    // Add the comments button
                                    TableCell commentsBtnCell = new TableCell();
                                    using (SqlConnection connection1 = new SqlConnection(connectionString))
                                    {
                                        string checkLikesQuery = "SELECT Count(*) FROM discussions WHERE task_id = @id";
                                        using (SqlCommand checkLikesCommand = new SqlCommand(checkLikesQuery, connection1))
                                        {
                                            connection1.Open();

                                            checkLikesCommand.Parameters.AddWithValue("@id", reader["id"].ToString());

                                            int count = (int)checkLikesCommand.ExecuteScalar();

                                            if (count > 0)
                                            {
                                                LinkButton commentsButton = new LinkButton();
                                                commentsButton.Text = $"<div class='posts-indicator'><svg viewBox=\"0 0 20 20\" fill=\"currentColor\" width=\"22\" height=\"22\" aria-hidden=\"true\" class=\"icon_52d6883634 conversation-cta-module_withUpdates__lLlWU noFocusStyle_72a084ca1b\" data-testid=\"icon\"><path d=\"M10.4339 1.95001C11.5975 1.94802 12.7457 2.2162 13.7881 2.73345C14.8309 3.25087 15.7392 4.0034 16.4416 4.93172C17.1439 5.86004 17.6211 6.93879 17.8354 8.08295C18.0498 9.22712 17.9955 10.4054 17.6769 11.525C17.3582 12.6447 16.7839 13.675 15.9992 14.5348C15.2144 15.3946 14.2408 16.0604 13.1549 16.4798C12.0689 16.8991 10.9005 17.0606 9.74154 16.9514C8.72148 16.8553 7.73334 16.5518 6.83716 16.0612L4.29488 17.2723C3.23215 17.7786 2.12265 16.6693 2.6287 15.6064L3.83941 13.0637C3.26482 12.0144 2.94827 10.8411 2.91892 9.64118C2.88616 8.30174 3.21245 6.97794 3.86393 5.80714C4.51541 4.63635 5.46834 3.66124 6.62383 2.98299C7.77896 2.30495 9.09445 1.9483 10.4339 1.95001ZM10.4339 1.95001C10.4343 1.95001 10.4347 1.95001 10.4351 1.95001L10.434 2.70001L10.4326 1.95001C10.433 1.95001 10.4334 1.95001 10.4339 1.95001ZM13.1214 4.07712C12.2867 3.66294 11.3672 3.44826 10.4354 3.45001L10.4329 3.45001C9.3608 3.44846 8.30778 3.73387 7.38315 4.2766C6.45852 4.81934 5.69598 5.59963 5.17467 6.5365C4.65335 7.47337 4.39226 8.53268 4.41847 9.6045C4.44469 10.6763 4.75726 11.7216 5.32376 12.6319C5.45882 12.8489 5.47405 13.1198 5.36416 13.3506L4.28595 15.6151L6.54996 14.5366C6.78072 14.4266 7.05158 14.4418 7.26863 14.5768C8.05985 15.0689 8.95456 15.3706 9.88225 15.458C10.8099 15.5454 11.7452 15.4162 12.6145 15.0805C13.4837 14.7448 14.2631 14.2119 14.8912 13.5236C15.5194 12.8354 15.9791 12.0106 16.2341 11.1144C16.4892 10.2182 16.5327 9.27504 16.3611 8.35918C16.1895 7.44332 15.8075 6.57983 15.2453 5.83674C14.6831 5.09366 13.9561 4.49129 13.1214 4.07712Z\" fill=\"currentColor\" fill-rule=\"evenodd\" clip-rule=\"evenodd\"></path></svg> <span class='post-count'>{count}</span></div>";
                                                commentsButton.ID = $"comments-btn-{reader["id"].ToString()}";
                                                commentsButton.CssClass = "text-primary comments-btn";
                                                commentsButton.Click += OnCommentsButtonClicked;

                                                commentsBtnCell.Controls.Add(commentsButton);
                                            }
                                            else
                                            {
                                                LinkButton commentsButton = new LinkButton();
                                                commentsButton.Text = "<svg viewBox=\"0 0 20 20\" fill=\"currentColor\" width=\"22\" height=\"22\" aria-hidden=\"true\" class=\"icon_52d6883634 conversation-cta-module_withoutUpdates__LoZDn noFocusStyle_72a084ca1b\" data-testid=\"icon\"><path d=\"M10.4339 1.94996C11.5976 1.94797 12.7458 2.21616 13.7882 2.7334C14.8309 3.25083 15.7393 4.00335 16.4416 4.93167C17.144 5.85999 17.6211 6.93874 17.8355 8.08291C18.0498 9.22707 17.9956 10.4054 17.6769 11.525C17.3583 12.6446 16.7839 13.6749 15.9992 14.5347C15.2145 15.3945 14.2408 16.0604 13.1549 16.4797C12.069 16.8991 10.9005 17.0605 9.7416 16.9513C8.72154 16.8552 7.7334 16.5518 6.83723 16.0612L4.29494 17.2723C3.23222 17.7785 2.12271 16.6692 2.62876 15.6064L3.83948 13.0636C3.26488 12.0144 2.94833 10.8411 2.91898 9.64114C2.88622 8.30169 3.21251 6.97789 3.86399 5.8071C4.51547 4.63631 5.4684 3.66119 6.62389 2.98294C7.77902 2.30491 9.09451 1.94825 10.4339 1.94996ZM10.4339 1.94996C10.4343 1.94996 10.4348 1.94996 10.4352 1.94996L10.4341 2.69996L10.4327 1.94996C10.4331 1.94996 10.4335 1.94996 10.4339 1.94996ZM13.1214 4.07707C12.2868 3.66289 11.3673 3.44821 10.4355 3.44996L10.433 3.44996C9.36086 3.44842 8.30784 3.73382 7.38321 4.27655C6.45858 4.81929 5.69605 5.59958 5.17473 6.53645C4.65341 7.47332 4.39232 8.53263 4.41853 9.60446C4.44475 10.6763 4.75732 11.7216 5.32382 12.6318C5.45888 12.8489 5.47411 13.1197 5.36422 13.3505L4.28601 15.615L6.55002 14.5365C6.78078 14.4266 7.05164 14.4418 7.26869 14.5768C8.05992 15.0689 8.95463 15.3706 9.88231 15.458C10.81 15.5454 11.7453 15.4161 12.6145 15.0805C13.4838 14.7448 14.2631 14.2118 14.8913 13.5236C15.5194 12.8353 15.9791 12.0106 16.2342 11.1144C16.4893 10.2182 16.5327 9.27499 16.3611 8.35913C16.1895 7.44328 15.8076 6.57978 15.2454 5.8367C14.6832 5.09362 13.9561 4.49125 13.1214 4.07707Z\" fill=\"currentColor\" fill-rule=\"evenodd\" clip-rule=\"evenodd\"></path><path d=\"M11.25 6.5C11.25 6.08579 10.9142 5.75 10.5 5.75C10.0858 5.75 9.75 6.08579 9.75 6.5V8.75H7.5C7.08579 8.75 6.75 9.08579 6.75 9.5C6.75 9.91421 7.08579 10.25 7.5 10.25H9.75V12.5C9.75 12.9142 10.0858 13.25 10.5 13.25C10.9142 13.25 11.25 12.9142 11.25 12.5V10.25H13.5C13.9142 10.25 14.25 9.91421 14.25 9.5C14.25 9.08579 13.9142 8.75 13.5 8.75H11.25V6.5Z\" fill=\"currentColor\" fill-rule=\"evenodd\" clip-rule=\"evenodd\"></path></svg>";
                                                commentsButton.ID = $"comments-btn-{reader["id"].ToString()}";
                                                commentsButton.CssClass = "comments-btn";
                                                commentsButton.Click += OnCommentsButtonClicked;

                                                commentsBtnCell.Controls.Add(commentsButton);
                                            }
                                        }
                                    }

                                    // Add the table cell to the row
                                    tableRow.Cells.Add(tableCell);
                                    tableRow.Cells.Add(commentsBtnCell);
                                }

                                else if (columnName == "status")
                                {
                                    //UpdatePanel updatePanel = new UpdatePanel();
                                   // updatePanel.ID = $"CurrentStatusUpdatePanel{multipleCurrentStatusUpdatePanelCounter}";
                                    //updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
                                    //updatePanel.Attributes += "runat='server'";

                                    TableHeaderRow tableHeaderRow = (TableHeaderRow)Group.Rows[0];
                                    bool statusHeaderExists = false;

                                    foreach (TableCell cell in tableHeaderRow.Cells)
                                    {
                                        if (cell.Text.Equals("status"))
                                        {
                                            statusHeaderExists = true;
                                            break;
                                        }
                                    }

                                    if (statusHeaderExists == false)
                                    {
                                        TableHeaderCell statusHeaderCell = new TableHeaderCell();
                                        statusHeaderCell.Text = "status";
                                        this.GroupHeaderRow.Cells.Add(statusHeaderCell);
                                    }

                                    Button statusBtn = new Button();
                                    statusBtn.Text = reader["status"].ToString();
                                    statusBtn.ID = $"status-btn-{reader["id"].ToString()}";
                                    statusBtn.CssClass = "status-btn";
                                    statusBtn.Click += OnCurrentStatusButtonClicked;

                                    if (statusBtn.Text.Equals("Done"))
                                    {
                                        statusBtn.Style["background-color"] = "rgb(0, 200, 117)";
                                        statusBtn.ForeColor = Color.White;
                                        tableCell.Style["background-color"] = "rgb(0, 200, 117)";
                                    }
                                    else if (statusBtn.Text.Equals("Working On It"))
                                    {
                                        statusBtn.Style["background-color"] = "#f4af53";
                                        statusBtn.ForeColor = Color.White;
                                        tableCell.Style["background-color"] = "#f4af53";
                                    }
                                    else if (statusBtn.Text.Equals("Stuck"))
                                    {
                                        statusBtn.Style["background-color"] = "#df2f4a";
                                        statusBtn.ForeColor = Color.White;
                                        tableCell.Style["background-color"] = "#df2f4a";
                                    }
                                    else
                                    {
                                        statusBtn.BackColor = Color.Gray;
                                        statusBtn.ForeColor = Color.White;
                                        tableCell.BackColor = statusBtn.BackColor;
                                    }

                                    tableCell.Controls.Add(statusBtn);
                                    tableRow.Cells.Add(tableCell);

                                    // Adding the tableRow to the Group (assuming Group is a Table)
                                    Group.Rows.Add(tableRow);

                                    // Add the UpdatePanel to the page
                                   // updatePanel.ContentTemplateContainer.Controls.Add(Group);
                                   // this.Controls.Add(updatePanel);
                                    multipleCurrentStatusUpdatePanelCounter++;
                                }
                                else if (columnName == "date")
                                {

                                    TableHeaderRow tableHeaderRow = (TableHeaderRow)Group.Rows[0];
                                    bool dateHeaderExists = false;

                                    foreach (TableCell cell in tableHeaderRow.Cells)
                                    {
                                        if (cell.Text.Equals("date"))
                                        {
                                            dateHeaderExists = true;
                                            break;
                                        }
                                    }

                                    if (dateHeaderExists == false)
                                    {
                                        TableHeaderCell dateHeaderCell = new TableHeaderCell();
                                        dateHeaderCell.Text = "date";
                                        this.GroupHeaderRow.Cells.Add(dateHeaderCell);
                                    }

                                    Button dateBtn = new Button();
                                    if (reader["date"].ToString().Equals("NULL"))
                                    {
                                        dateBtn.Text = "+";
                                    }
                                    else
                                    {
                                        dateBtn.Text = Convert.ToDateTime(reader["date"] ?? DateTime.MinValue).ToString("MMMM dd, yyyy");
                                    }

                                    dateBtn.ID = $"date-btn-{reader["id"].ToString()}";
                                    dateBtn.CssClass = "date-btn";
                                    dateBtn.Click += OnDateButtonClicked;

                                    tableCell.Controls.Add(dateBtn);

                                    tableRow.Cells.Add(tableCell);
                                }
                                else if (columnName == "people")
                                {
                                    TableHeaderRow tableHeaderRow = (TableHeaderRow)Group.Rows[0];
                                    bool peopleHeaderExists = false;

                                    foreach (TableCell cell in tableHeaderRow.Cells)
                                    {
                                        if (cell.Text.Equals("people"))
                                        {
                                            peopleHeaderExists = true;
                                            break;
                                        }
                                    }

                                    if (peopleHeaderExists == false)
                                    {
                                        TableHeaderCell dateHeaderCell = new TableHeaderCell();
                                        dateHeaderCell.Text = "people";
                                        this.GroupHeaderRow.Cells.Add(dateHeaderCell);
                                    }

                                    // I read through the task assigned table, if it is empty give +, else assign button to text

                                    string countAssignmentIdQuery = "SELECT COUNT(assignment_id) FROM assignedTasks WHERE task_id = @task_id";
                                    int count = 0;

                                    // Use separate connection for the count query
                                    using (SqlConnection connection2 = new SqlConnection(connectionString))
                                    {
                                        connection2.Open();
                                        using (SqlCommand countAssignmentIdCommand = new SqlCommand(countAssignmentIdQuery, connection2))
                                        {
                                            countAssignmentIdCommand.Parameters.AddWithValue("@task_id", reader["id"].ToString());
                                            count = (int)countAssignmentIdCommand.ExecuteScalar();
                                        }
                                        connection2.Close();
                                    }

                                    if (count > 0)
                                    {
                                        string teamMemberEmail = "";
                                        string getTeamMemberEmailQuery = "SELECT * FROM assignedTasks WHERE task_id = @task_id";

                                        using (SqlConnection connection3 = new SqlConnection(connectionString))
                                        {
                                            connection3.Open();
                                            using (SqlCommand getTeamMemberEmailCommand = new SqlCommand(getTeamMemberEmailQuery, connection3))
                                            {
                                                getTeamMemberEmailCommand.Parameters.AddWithValue("@task_id", reader["id"].ToString());
                                                using (SqlDataReader reader2 = getTeamMemberEmailCommand.ExecuteReader())
                                                {
                                                    int multipleButtonsCounter = 1;
                                                    while (reader2.Read())
                                                    {
                                                        teamMemberEmail = reader2["member_email"].ToString();

                                                        Button peopleBtn = new Button();
                                                        peopleBtn.Text = $"{teamMemberEmail.Substring(0, 2)}";
                                                        peopleBtn.ID = $"people-btn-{reader["id"].ToString()}-{multipleButtonsCounter}";
                                                        peopleBtn.CssClass = "people-btn";
                                                        peopleBtn.Click += OnPeopleButtonClicked;
                                                        peopleBtn.ToolTip = teamMemberEmail;
                                                        // Set random background color
                                                        peopleBtn.BackColor = Color.Gray;
                                                        peopleBtn.ForeColor = Color.White;
                                                        tableCell.Controls.Add(peopleBtn);

                                                        tableCell.CssClass = "people-cell";
                                                        tableRow.Cells.Add(tableCell);
                                                        multipleButtonsCounter++;
                                                    }
                                                    multipleButtonsCounter = 0;
                                                }
                                            }
                                            connection3.Close();
                                        }
                                    }
                                    else
                                    {
                                        Button peopleBtn = new Button();
                                        peopleBtn.Text = "+";
                                        peopleBtn.ID = $"people-btn-{reader["id"].ToString()}";
                                        peopleBtn.CssClass = "people-btn";
                                        peopleBtn.Click += OnPeopleButtonClicked;

                                        tableCell.Controls.Add(peopleBtn);

                                        tableRow.Cells.Add(tableCell);
                                    }
                                }
                            }

                            this.Group.Rows.Add(tableRow);
                            */
                        }

                        // Add the "Add Column Button" if there is no new columns
                        /*
                        Button addColumnBtn = new Button();
                        addColumnBtn.CssClass = "add-column-btn";
                        addColumnBtn.Text = "+";
                        addColumnBtn.ID = "AddColumnButton";
                        addColumnBtn.Click += OnAddColumnButtonClicked;*/

                        HtmlGenericControl addColumnBtn = new HtmlGenericControl("button");
                        addColumnBtn.Attributes["id"] = "AddColumnButton";
                        addColumnBtn.Attributes["class"] = "add-column-btn";
                        addColumnBtn.Attributes["type"] = "button";
                        addColumnBtn.Attributes["onclick"] = "showColumnsMenu()";
                        addColumnBtn.InnerText = "+";

                        TableHeaderRow headerRow = (TableHeaderRow)Group.Rows[0];
                        bool addColumnBtnHeaderExists = false;

                        foreach (TableCell cell in headerRow.Cells)
                        {
                            foreach (Control control in cell.Controls)
                            {
                                if (control is HtmlGenericControl btn && btn.Attributes["id"] == "AddColumnButton")
                                {
                                    addColumnBtnHeaderExists = true;
                                    break;
                                }
                            }
                            if (addColumnBtnHeaderExists)
                                break;
                        }

                        if (!addColumnBtnHeaderExists)
                        {
                            TableHeaderCell addColumnBtnHeaderCell = new TableHeaderCell();
                            addColumnBtnHeaderCell.Controls.Add(addColumnBtn);
                            headerRow.Cells.Add(addColumnBtnHeaderCell);
                        }
                    }
                }
            }
        }

        protected void MakeTable()
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            int team_id = GetCurrentlyLoggedInUsersTeamId();
            int board_id = Convert.ToInt32(this.getBoardId());
            string username = this.GetCurrentlyLoggedInUsername();

            List<string> columns = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string getColumnsQuery = "SELECT * FROM columnsOfTables WHERE user_id = @user_id AND board_id = @board_id";
                using (SqlCommand command = new SqlCommand(getColumnsQuery, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@board_id", board_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(reader["column_name"].ToString());
                        }
                    }

                    connection.Close();
                }
                                
                if (columns.Count > 0)
                {
                    StringBuilder createTableQuerySB = new StringBuilder();                   
                    createTableQuerySB.Append($"CREATE TABLE {username}_{team_id}_{board_id} (");
                    createTableQuerySB.Append("id INT PRIMARY KEY IDENTITY(1, 1), ");

                    foreach (string column in columns)
                    {
                        createTableQuerySB.Append($"{column} VarChar(255),");
                    }
                    createTableQuerySB.Append($")");

                    string createTableQuery = createTableQuerySB.ToString();
                    using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                    {
                        connection.Open();

                        command.ExecuteNonQuery();

                        connection.Close();
                    }
                }                
            }
        }

        protected void UpdatedUpdateTable()
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            List<string> columns = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Random randomColor = new Random();

                string checkIfUserHasColorQuery = "SELECT COUNT(user_pfp_color) FROM userInfo WHERE id = @user_id AND user_pfp_color = 'NULL'";
                using (SqlCommand command = new SqlCommand(checkIfUserHasColorQuery, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    int count = (int) command.ExecuteScalar();

                    connection.Close();

                    if (count > 0)
                    {
                        string updateUserPfpColor = "UPDATE userInfo SET user_pfp_color = @color WHERE id = @user_id AND user_pfp_color = 'NULL'";
                        using (SqlCommand updateColorCommand = new SqlCommand(updateUserPfpColor, connection))
                        {
                            connection.Open();

                            int r = randomColor.Next(256);
                            int g = randomColor.Next(256);
                            int b = randomColor.Next(256);
                            updateColorCommand.Parameters.AddWithValue("@color", $"{r},{g},{b}");
                            updateColorCommand.Parameters.AddWithValue("@user_id", user_id);
                            updateColorCommand.ExecuteNonQuery();

                            connection.Close();
                        }                        
                    }
                }

                string query = "SELECT * FROM columnsOfTables WHERE user_id = @user_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            columns.Add(reader["column_name"].ToString());
                        }
                    }

                    connection.Close();
                }

                /*string getDataForTableQuery = @"
    SELECT d.*
    FROM dataOfTables d
    JOIN columnsOfTables c ON d.data_type = c.data_type AND d.user_id = c.user_id
    WHERE d.user_id = 2
    ORDER BY c.id";*/

                //string getDataForTableQuery = "SELECT * FROM dataOfTables WHERE user_id = @user_id";
                string getDataForTableQuery = @"WITH RankedData AS (
    SELECT d.*, 
           ROW_NUMBER() OVER (PARTITION BY d.data_type ORDER BY c.id) AS rank
    FROM dataOfTables d
    INNER JOIN columnsOfTables c ON d.column_name = c.column_name
    WHERE d.user_id = 2
)
SELECT *
FROM RankedData
ORDER BY rank DESC, data_type DESC;";

                using (SqlCommand getColumnsCommand = new SqlCommand(getDataForTableQuery, connection))
                {
                    connection.Open();

                    getColumnsCommand.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader = getColumnsCommand.ExecuteReader())
                    {
                        // Clear the space for the updated rows excluding the taskTextBox
                        for (int i = Group.Rows.Count - 1; i > 1; i--)
                        {
                            Group.Rows.RemoveAt(i);
                        }

                        TableRow tableRow = new TableRow();

                        int columnCount = 1;
                        int lastColumnPosition = columns.Count;
                        string lastColumnName = columns[lastColumnPosition - 1].ToString();

                        while (reader.Read())
                        {
                            string dataType = reader["data_type"].ToString();

                            // check if it's the last row
                            if (columnCount > lastColumnPosition)
                            {
                                tableRow = new TableRow();
                                columnCount = 1;
                            }

                            if (dataType.Equals("Task"))
                            {

                                TableCell taskCell = new TableCell();
                                taskCell.CssClass = "table-cell task-text-box-cell";

                                string task = reader["data"].ToString();

                                Button optionsBtn = new Button();
                                optionsBtn.Text = "...";
                                optionsBtn.ID = $"options-btn-{reader["id"].ToString()}";
                                optionsBtn.CssClass = "options-btn d-flex align-items-center";
                                optionsBtn.Click += OnOptionButtonClicked;

                                TextBox textBoxWithTask = new TextBox();
                                textBoxWithTask.ID = $"task-textbox-{reader["id"].ToString()}";
                                textBoxWithTask.Text = task;
                                textBoxWithTask.CssClass = "task-text-box";
                                textBoxWithTask.TextChanged += EditTask;

                                taskCell.Controls.Add(optionsBtn);
                                taskCell.Controls.Add(textBoxWithTask);

                                // Add the comments button
                                TableCell commentsBtnCell = new TableCell();
                                using (SqlConnection connection2 = new SqlConnection(connectionString))
                                {
                                    string checkLikesQuery = "SELECT Count(*) FROM discussions WHERE task_id = @id";
                                    using (SqlCommand checkLikesCommand = new SqlCommand(checkLikesQuery, connection2))
                                    {
                                        connection2.Open();

                                        checkLikesCommand.Parameters.AddWithValue("@id", reader["id"].ToString());

                                        int count = (int)checkLikesCommand.ExecuteScalar();

                                        if (count > 0)
                                        {
                                            LinkButton commentsButton = new LinkButton();
                                            commentsButton.Text = $"<div class='posts-indicator'><svg viewBox=\"0 0 20 20\" fill=\"currentColor\" width=\"22\" height=\"22\" aria-hidden=\"true\" class=\"icon_52d6883634 conversation-cta-module_withUpdates__lLlWU noFocusStyle_72a084ca1b\" data-testid=\"icon\"><path d=\"M10.4339 1.95001C11.5975 1.94802 12.7457 2.2162 13.7881 2.73345C14.8309 3.25087 15.7392 4.0034 16.4416 4.93172C17.1439 5.86004 17.6211 6.93879 17.8354 8.08295C18.0498 9.22712 17.9955 10.4054 17.6769 11.525C17.3582 12.6447 16.7839 13.675 15.9992 14.5348C15.2144 15.3946 14.2408 16.0604 13.1549 16.4798C12.0689 16.8991 10.9005 17.0606 9.74154 16.9514C8.72148 16.8553 7.73334 16.5518 6.83716 16.0612L4.29488 17.2723C3.23215 17.7786 2.12265 16.6693 2.6287 15.6064L3.83941 13.0637C3.26482 12.0144 2.94827 10.8411 2.91892 9.64118C2.88616 8.30174 3.21245 6.97794 3.86393 5.80714C4.51541 4.63635 5.46834 3.66124 6.62383 2.98299C7.77896 2.30495 9.09445 1.9483 10.4339 1.95001ZM10.4339 1.95001C10.4343 1.95001 10.4347 1.95001 10.4351 1.95001L10.434 2.70001L10.4326 1.95001C10.433 1.95001 10.4334 1.95001 10.4339 1.95001ZM13.1214 4.07712C12.2867 3.66294 11.3672 3.44826 10.4354 3.45001L10.4329 3.45001C9.3608 3.44846 8.30778 3.73387 7.38315 4.2766C6.45852 4.81934 5.69598 5.59963 5.17467 6.5365C4.65335 7.47337 4.39226 8.53268 4.41847 9.6045C4.44469 10.6763 4.75726 11.7216 5.32376 12.6319C5.45882 12.8489 5.47405 13.1198 5.36416 13.3506L4.28595 15.6151L6.54996 14.5366C6.78072 14.4266 7.05158 14.4418 7.26863 14.5768C8.05985 15.0689 8.95456 15.3706 9.88225 15.458C10.8099 15.5454 11.7452 15.4162 12.6145 15.0805C13.4837 14.7448 14.2631 14.2119 14.8912 13.5236C15.5194 12.8354 15.9791 12.0106 16.2341 11.1144C16.4892 10.2182 16.5327 9.27504 16.3611 8.35918C16.1895 7.44332 15.8075 6.57983 15.2453 5.83674C14.6831 5.09366 13.9561 4.49129 13.1214 4.07712Z\" fill=\"currentColor\" fill-rule=\"evenodd\" clip-rule=\"evenodd\"></path></svg> <span class='post-count'>{count}</span></div>";
                                            commentsButton.ID = $"comments-btn-{reader["id"].ToString()}";
                                            commentsButton.CssClass = "text-primary comments-btn";
                                            commentsButton.Click += OnCommentsButtonClicked;

                                            commentsBtnCell.Controls.Add(commentsButton);
                                        }
                                        else
                                        {
                                            LinkButton commentsButton = new LinkButton();
                                            commentsButton.Text = "<svg viewBox=\"0 0 20 20\" fill=\"currentColor\" width=\"22\" height=\"22\" aria-hidden=\"true\" class=\"icon_52d6883634 conversation-cta-module_withoutUpdates__LoZDn noFocusStyle_72a084ca1b\" data-testid=\"icon\"><path d=\"M10.4339 1.94996C11.5976 1.94797 12.7458 2.21616 13.7882 2.7334C14.8309 3.25083 15.7393 4.00335 16.4416 4.93167C17.144 5.85999 17.6211 6.93874 17.8355 8.08291C18.0498 9.22707 17.9956 10.4054 17.6769 11.525C17.3583 12.6446 16.7839 13.6749 15.9992 14.5347C15.2145 15.3945 14.2408 16.0604 13.1549 16.4797C12.069 16.8991 10.9005 17.0605 9.7416 16.9513C8.72154 16.8552 7.7334 16.5518 6.83723 16.0612L4.29494 17.2723C3.23222 17.7785 2.12271 16.6692 2.62876 15.6064L3.83948 13.0636C3.26488 12.0144 2.94833 10.8411 2.91898 9.64114C2.88622 8.30169 3.21251 6.97789 3.86399 5.8071C4.51547 4.63631 5.4684 3.66119 6.62389 2.98294C7.77902 2.30491 9.09451 1.94825 10.4339 1.94996ZM10.4339 1.94996C10.4343 1.94996 10.4348 1.94996 10.4352 1.94996L10.4341 2.69996L10.4327 1.94996C10.4331 1.94996 10.4335 1.94996 10.4339 1.94996ZM13.1214 4.07707C12.2868 3.66289 11.3673 3.44821 10.4355 3.44996L10.433 3.44996C9.36086 3.44842 8.30784 3.73382 7.38321 4.27655C6.45858 4.81929 5.69605 5.59958 5.17473 6.53645C4.65341 7.47332 4.39232 8.53263 4.41853 9.60446C4.44475 10.6763 4.75732 11.7216 5.32382 12.6318C5.45888 12.8489 5.47411 13.1197 5.36422 13.3505L4.28601 15.615L6.55002 14.5365C6.78078 14.4266 7.05164 14.4418 7.26869 14.5768C8.05992 15.0689 8.95463 15.3706 9.88231 15.458C10.81 15.5454 11.7453 15.4161 12.6145 15.0805C13.4838 14.7448 14.2631 14.2118 14.8913 13.5236C15.5194 12.8353 15.9791 12.0106 16.2342 11.1144C16.4893 10.2182 16.5327 9.27499 16.3611 8.35913C16.1895 7.44328 15.8076 6.57978 15.2454 5.8367C14.6832 5.09362 13.9561 4.49125 13.1214 4.07707Z\" fill=\"currentColor\" fill-rule=\"evenodd\" clip-rule=\"evenodd\"></path><path d=\"M11.25 6.5C11.25 6.08579 10.9142 5.75 10.5 5.75C10.0858 5.75 9.75 6.08579 9.75 6.5V8.75H7.5C7.08579 8.75 6.75 9.08579 6.75 9.5C6.75 9.91421 7.08579 10.25 7.5 10.25H9.75V12.5C9.75 12.9142 10.0858 13.25 10.5 13.25C10.9142 13.25 11.25 12.9142 11.25 12.5V10.25H13.5C13.9142 10.25 14.25 9.91421 14.25 9.5C14.25 9.08579 13.9142 8.75 13.5 8.75H11.25V6.5Z\" fill=\"currentColor\" fill-rule=\"evenodd\" clip-rule=\"evenodd\"></path></svg>";
                                            commentsButton.ID = $"comments-btn-{reader["id"].ToString()}";
                                            commentsButton.CssClass = "comments-btn";
                                            commentsButton.Click += OnCommentsButtonClicked;

                                            commentsBtnCell.Controls.Add(commentsButton);
                                        }
                                    }
                                }

                                // Add the table cell to the row
                                tableRow.Cells.Add(taskCell);
                                tableRow.Cells.Add(commentsBtnCell);

                                columnCount++;
                            }
                            else if (dataType.Equals("Status"))
                            {

                                TableCell tableCell = new TableCell();
                                HiddenField statusHeaderHiddenField = new HiddenField();
                                statusHeaderHiddenField.ID = $"status-header_hidden_field-{reader["id"]}";
                                tableCell.CssClass = "table-cell status-cell";

                                TableHeaderRow tableHeaderRow = (TableHeaderRow)Group.Rows[0];
                                bool statusHeaderExists = false;

                                // Check if the header already exists to prevent duplication
                                foreach (TableCell cell in tableHeaderRow.Cells)
                                {
                                    TextBox textBox = cell.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Text == $"{reader["column_name"]}");
                                    if (textBox != null)
                                    {
                                        statusHeaderExists = true;
                                        break;
                                    }
                                }

                                if (!statusHeaderExists)
                                {
                                    TableHeaderCell statusHeaderCell = new TableHeaderCell();

                                    TextBox statusHeaderTextBox = new TextBox();
                                    statusHeaderHiddenField.Value = reader["column_name"].ToString();

                                    if (!string.IsNullOrEmpty(statusHeaderHiddenField.Value))
                                    {
                                        statusHeaderTextBox.Text = statusHeaderHiddenField.Value;
                                    }

                                    statusHeaderTextBox.ID = $"status-header-{reader["id"]}";
                                    statusHeaderTextBox.CssClass = "header-textbox";
                                    statusHeaderTextBox.TextChanged += EditHeaderText;

                                    statusHeaderCell.Controls.Add(statusHeaderTextBox);
                                    tableHeaderRow.Cells.Add(statusHeaderCell);
                                }


                                string status = reader["data"].ToString();

                                Button statusBtn = new Button();
                                statusBtn.Text = reader["data"].ToString();
                                statusBtn.Click += OnCurrentStatusButtonClicked;
                                statusBtn.ID = $"status-btn-{reader["id"].ToString()}";
                                statusBtn.Attributes["class"] = "status-btn";
                                statusBtn.Attributes["type"] = "button";

                                if (reader["data"].ToString().Equals("Done"))
                                {
                                    statusBtn.Attributes["style"] = "background-color: rgb(0, 200, 117); color: white;";
                                    tableCell.Style["background-color"] = "rgb(0, 200, 117)";
                                }
                                else if (reader["data"].ToString().Equals("Working On It"))
                                {
                                    statusBtn.Attributes["style"] = "background-color: rgb(253, 188, 100); color: white;";
                                    tableCell.Style["background-color"] = "rgb(253, 188, 100)";
                                }
                                else if (reader["data"].ToString().Equals("Stuck"))
                                {
                                    statusBtn.Attributes["style"] = "background-color: rgb(223, 47, 74); color: white;";
                                    tableCell.Style["background-color"] = "rgb(223, 47, 74)";
                                }
                                else
                                {
                                    statusBtn.Attributes["style"] = "background-color: rgb(121, 126, 147); color: white;";
                                    tableCell.Style["background-color"] = "rgb(121, 126, 147)";
                                }

                                tableCell.Controls.Add(statusBtn);
                                tableRow.Controls.Add(tableCell);

                                columnCount++;
                            }
                            else if (dataType.Equals("Date"))
                            {

                                TableCell tableCell = new TableCell();
                                HiddenField dateHeaderHiddenField = new HiddenField();
                                dateHeaderHiddenField.ID = $"date-header_hidden_field-{reader["id"]}";
                                tableCell.CssClass = "table-cell date-cell";

                                TableHeaderRow tableHeaderRow = (TableHeaderRow)Group.Rows[0];
                                bool dateHeaderExists = false;

                                // Check if the header already exists to prevent duplication
                                foreach (TableCell cell in tableHeaderRow.Cells)
                                {
                                    TextBox textBox = cell.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Text == $"{reader["column_name"]}");
                                    if (textBox != null)
                                    {
                                        dateHeaderExists = true;
                                        break;
                                    }
                                }

                                if (!dateHeaderExists)
                                {
                                    TableHeaderCell dateHeaderCell = new TableHeaderCell();

                                    TextBox dateHeaderTextBox = new TextBox();
                                    dateHeaderHiddenField.Value = reader["column_name"].ToString();

                                    if (!string.IsNullOrEmpty(dateHeaderHiddenField.Value))
                                    {
                                        dateHeaderTextBox.Text = dateHeaderHiddenField.Value;
                                    }

                                    dateHeaderTextBox.ID = $"date-header-{reader["id"]}";
                                    dateHeaderTextBox.CssClass = "header-textbox";
                                    dateHeaderTextBox.TextChanged += EditHeaderText;

                                    dateHeaderCell.Controls.Add(dateHeaderTextBox);
                                    tableHeaderRow.Cells.Add(dateHeaderCell);
                                }

                                Button dateBtn = new Button();
                                dateBtn.Text = $"{reader["data"].ToString()}";
                                dateBtn.ID = $"date-btn-{reader["id"].ToString()}";
                                dateBtn.CssClass = "date-btn";
                                dateBtn.Click += OnDateButtonClicked;

                                tableCell.Controls.Add(dateBtn);
                                tableRow.Controls.Add(tableCell);

                                columnCount++;
                            }

                            else if (dataType.Equals("People"))
                            {
                                TableCell tableCell = new TableCell();
                                HiddenField peopleHeaderHiddenField = new HiddenField();
                                peopleHeaderHiddenField.ID = $"people-header_hidden_field-{reader["id"].ToString()}";
                                tableCell.CssClass = "table-cell people-cell";

                                TableHeaderRow tableHeaderRow = (TableHeaderRow)Group.Rows[0];
                                bool peopleHeaderExists = false;

                                // Check if the header already exists to prevent duplication
                                foreach (TableCell cell in tableHeaderRow.Cells)
                                {
                                    TextBox textBox = cell.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Text == $"{reader["column_name"].ToString()}");
                                    if (textBox != null)
                                    {
                                        peopleHeaderExists = true;
                                        break;
                                    }
                                }

                                if (!peopleHeaderExists)
                                {
                                    TableHeaderCell peopleHeaderCell = new TableHeaderCell();

                                    TextBox peopleHeaderTextBox = new TextBox();
                                    peopleHeaderHiddenField.Value = reader["column_name"].ToString();

                                    if (!string.IsNullOrEmpty(peopleHeaderHiddenField.Value))
                                    {
                                        peopleHeaderTextBox.Text = peopleHeaderHiddenField.Value;
                                    }

                                    peopleHeaderTextBox.ID = $"people-header-{reader["id"]}";
                                    peopleHeaderTextBox.CssClass = "header-textbox";
                                    peopleHeaderTextBox.TextChanged += EditHeaderText;

                                    peopleHeaderCell.Controls.Add(peopleHeaderTextBox);
                                    tableHeaderRow.Cells.Add(peopleHeaderCell);
                                }


                                Button peopleBtn = new Button();

                                using (SqlConnection connection1 = new SqlConnection(connectionString))
                                {
                                    string countTeamMembersWhoGotTaskAssignedQuery = "SELECT COUNT(*) FROM assignedTasks WHERE task_id = @task_id";
                                    using (SqlCommand command = new SqlCommand(countTeamMembersWhoGotTaskAssignedQuery, connection1))
                                    {
                                        connection1.Open();

                                        command.Parameters.AddWithValue("@task_id", reader["id"].ToString());
                                        int count = (int) command.ExecuteScalar();

                                        connection1.Close();

                                        if (count > 0)
                                        {
                                            string getTeamMembersWhoGotTaskAssignedQuery = "SELECT * FROM assignedTasks WHERE task_id = @task_id";
                                            using (SqlCommand sqlCommand = new SqlCommand(getTeamMembersWhoGotTaskAssignedQuery, connection1))
                                            {
                                                connection1.Open();

                                                sqlCommand.Parameters.AddWithValue("@task_id", reader["id"].ToString());
                                                using (SqlDataReader taskAssignedReader = sqlCommand.ExecuteReader())
                                                {
                                                    while (taskAssignedReader.Read())
                                                    {
                                                        string full_name = this.GetSpecificUserFullName(Convert.ToInt32(taskAssignedReader["member_id"].ToString()));
                                                        string insideOfButton = full_name.Split(' ')[0].Substring(0, 1) + full_name.Split(' ')[1].Substring(0, 1);
                                                        peopleBtn.Text = $"{insideOfButton}";
                                                        peopleBtn.ID = $"people-btn-{reader["id"]}";
                                                        peopleBtn.CssClass = "people-btn";
                                                        peopleBtn.Click += OnPeopleButtonClicked;
                                                        peopleBtn.Style["vertical-align"] = "middle";
                                                        peopleBtn.Style["text-align"] = "center";

                                                        int r = Convert.ToInt32(this.getColor().Split(',')[0]);
                                                        int g = Convert.ToInt32(this.getColor().Split(',')[1]);
                                                        int b = Convert.ToInt32(this.getColor().Split(',')[2]);
                                                        peopleBtn.BackColor = Color.FromArgb(r, g, b);
                                                        peopleBtn.ForeColor = Color.White;

                                                        PostProfilePictureWrapper.Style["background-color"] = $"rgb({r}, {g}, {b}) !important";
                                                        NavbarProfilePicture.Style["background-color"] = $"rgb({r}, {g}, {b}) !important";
                                                    }
                                                }

                                                connection1.Close();
                                            }
                                        } 
                                        else
                                        {
                                            peopleBtn.Text = "+";
                                            peopleBtn.ID = $"people-btn-{reader["id"]}";
                                            peopleBtn.CssClass = "people-btn";
                                            peopleBtn.Click += OnPeopleButtonClicked;
                                        }
                                    }
                                }                                

                                if (lastColumnPosition == columns.Count)
                                {
                                    tableCell.Controls.Add(peopleBtn);
                                    tableRow.Controls.Add(tableCell);
                                }                                

                                columnCount++;
                            }

                            this.Group.Controls.Add(tableRow);
                        }
                    }

                    HtmlGenericControl addColumnBtn = new HtmlGenericControl("button");
                    addColumnBtn.Attributes["id"] = "AddColumnButton";
                    addColumnBtn.Attributes["class"] = "add-column-btn";
                    addColumnBtn.Attributes["type"] = "button";
                    addColumnBtn.Attributes["onclick"] = "showColumnsMenu()";
                    addColumnBtn.InnerText = "+";

                    TableHeaderRow headerRow = (TableHeaderRow)Group.Rows[0];
                    bool addColumnBtnHeaderExists = false;

                    foreach (TableCell cell in headerRow.Cells)
                    {
                        foreach (Control control in cell.Controls)
                        {
                            if (control is HtmlGenericControl btn && btn.Attributes["id"] == "AddColumnButton")
                            {
                                addColumnBtnHeaderExists = true;
                                break;
                            }
                        }
                        if (addColumnBtnHeaderExists)
                            break;
                    }

                    if (!addColumnBtnHeaderExists)
                    {
                        TableHeaderCell addColumnBtnHeaderCell = new TableHeaderCell();
                        addColumnBtnHeaderCell.Controls.Add(addColumnBtn);
                        headerRow.Cells.Add(addColumnBtnHeaderCell);
                    }
                }
            }
        }

        protected void SuperUpdatedTable()
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            List<string> columns = new List<string>();
            List<string> dataTypes = new List<string>();

            // Clear the space for the updated rows excluding the taskTextBox
            for (int i = Group.Rows.Count - 1; i > 1; i--)
            {
                Group.Rows.RemoveAt(i);
            }
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string username = this.GetCurrentlyLoggedInUsername();
                int team_id = this.GetCurrentlyLoggedInUsersTeamId();
                int board_id = Convert.ToInt32(this.getBoardId());

                string getColumnsQuery = "SELECT * FROM columnsOfTables WHERE user_id = @user_id";
                using (SqlCommand command = new SqlCommand(getColumnsQuery, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(reader["column_name"].ToString());
                            dataTypes.Add(reader["data_type"].ToString());
                        }
                    }

                    connection.Close();
                }              

                string query = $"SELECT * FROM {username}_{team_id}_{board_id}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TableRow tableRow = new TableRow();
                            for (int i = 0; i < columns.Count && i < dataTypes.Count; i++)
                            {
                                if (dataTypes[i] == "Task")
                                {
                                    TableCell taskCell = new TableCell();
                                    taskCell.CssClass = "table-cell task-text-box-cell";

                                    string task = reader[$"{columns[i]}"].ToString();

                                    Button optionsBtn = new Button();
                                    optionsBtn.Text = "...";
                                    optionsBtn.ID = $"options-btn-{reader["id"].ToString()}";
                                    optionsBtn.CssClass = "options-btn d-flex align-items-center";
                                    optionsBtn.Click += OnOptionButtonClicked;

                                    TextBox textBoxWithTask = new TextBox();
                                    textBoxWithTask.ID = $"task-textbox-{reader["id"].ToString()}";
                                    textBoxWithTask.Text = task;
                                    textBoxWithTask.CssClass = "task-text-box";
                                    textBoxWithTask.TextChanged += EditTask;

                                    taskCell.Controls.Add(optionsBtn);
                                    taskCell.Controls.Add(textBoxWithTask);

                                    // Add the comments button
                                    TableCell commentsBtnCell = new TableCell();
                                    using (SqlConnection connection2 = new SqlConnection(connectionString))
                                    {
                                        string checkLikesQuery = "SELECT Count(*) FROM discussions WHERE task_id = @id";
                                        using (SqlCommand checkLikesCommand = new SqlCommand(checkLikesQuery, connection2))
                                        {
                                            connection2.Open();

                                            checkLikesCommand.Parameters.AddWithValue("@id", reader["id"].ToString());

                                            int count = (int)checkLikesCommand.ExecuteScalar();

                                            if (count > 0)
                                            {
                                                LinkButton commentsButton = new LinkButton();
                                                commentsButton.Text = $"<div class='posts-indicator'><svg viewBox=\"0 0 20 20\" fill=\"currentColor\" width=\"22\" height=\"22\" aria-hidden=\"true\" class=\"icon_52d6883634 conversation-cta-module_withUpdates__lLlWU noFocusStyle_72a084ca1b\" data-testid=\"icon\"><path d=\"M10.4339 1.95001C11.5975 1.94802 12.7457 2.2162 13.7881 2.73345C14.8309 3.25087 15.7392 4.0034 16.4416 4.93172C17.1439 5.86004 17.6211 6.93879 17.8354 8.08295C18.0498 9.22712 17.9955 10.4054 17.6769 11.525C17.3582 12.6447 16.7839 13.675 15.9992 14.5348C15.2144 15.3946 14.2408 16.0604 13.1549 16.4798C12.0689 16.8991 10.9005 17.0606 9.74154 16.9514C8.72148 16.8553 7.73334 16.5518 6.83716 16.0612L4.29488 17.2723C3.23215 17.7786 2.12265 16.6693 2.6287 15.6064L3.83941 13.0637C3.26482 12.0144 2.94827 10.8411 2.91892 9.64118C2.88616 8.30174 3.21245 6.97794 3.86393 5.80714C4.51541 4.63635 5.46834 3.66124 6.62383 2.98299C7.77896 2.30495 9.09445 1.9483 10.4339 1.95001ZM10.4339 1.95001C10.4343 1.95001 10.4347 1.95001 10.4351 1.95001L10.434 2.70001L10.4326 1.95001C10.433 1.95001 10.4334 1.95001 10.4339 1.95001ZM13.1214 4.07712C12.2867 3.66294 11.3672 3.44826 10.4354 3.45001L10.4329 3.45001C9.3608 3.44846 8.30778 3.73387 7.38315 4.2766C6.45852 4.81934 5.69598 5.59963 5.17467 6.5365C4.65335 7.47337 4.39226 8.53268 4.41847 9.6045C4.44469 10.6763 4.75726 11.7216 5.32376 12.6319C5.45882 12.8489 5.47405 13.1198 5.36416 13.3506L4.28595 15.6151L6.54996 14.5366C6.78072 14.4266 7.05158 14.4418 7.26863 14.5768C8.05985 15.0689 8.95456 15.3706 9.88225 15.458C10.8099 15.5454 11.7452 15.4162 12.6145 15.0805C13.4837 14.7448 14.2631 14.2119 14.8912 13.5236C15.5194 12.8354 15.9791 12.0106 16.2341 11.1144C16.4892 10.2182 16.5327 9.27504 16.3611 8.35918C16.1895 7.44332 15.8075 6.57983 15.2453 5.83674C14.6831 5.09366 13.9561 4.49129 13.1214 4.07712Z\" fill=\"currentColor\" fill-rule=\"evenodd\" clip-rule=\"evenodd\"></path></svg> <span class='post-count'>{count}</span></div>";
                                                commentsButton.ID = $"comments-btn-{reader["id"].ToString()}";
                                                commentsButton.CssClass = "text-primary comments-btn";
                                                commentsButton.Click += OnCommentsButtonClicked;

                                                commentsBtnCell.Controls.Add(commentsButton);
                                            }
                                            else
                                            {
                                                LinkButton commentsButton = new LinkButton();
                                                commentsButton.Text = "<svg viewBox=\"0 0 20 20\" fill=\"currentColor\" width=\"22\" height=\"22\" aria-hidden=\"true\" class=\"icon_52d6883634 conversation-cta-module_withoutUpdates__LoZDn noFocusStyle_72a084ca1b\" data-testid=\"icon\"><path d=\"M10.4339 1.94996C11.5976 1.94797 12.7458 2.21616 13.7882 2.7334C14.8309 3.25083 15.7393 4.00335 16.4416 4.93167C17.144 5.85999 17.6211 6.93874 17.8355 8.08291C18.0498 9.22707 17.9956 10.4054 17.6769 11.525C17.3583 12.6446 16.7839 13.6749 15.9992 14.5347C15.2145 15.3945 14.2408 16.0604 13.1549 16.4797C12.069 16.8991 10.9005 17.0605 9.7416 16.9513C8.72154 16.8552 7.7334 16.5518 6.83723 16.0612L4.29494 17.2723C3.23222 17.7785 2.12271 16.6692 2.62876 15.6064L3.83948 13.0636C3.26488 12.0144 2.94833 10.8411 2.91898 9.64114C2.88622 8.30169 3.21251 6.97789 3.86399 5.8071C4.51547 4.63631 5.4684 3.66119 6.62389 2.98294C7.77902 2.30491 9.09451 1.94825 10.4339 1.94996ZM10.4339 1.94996C10.4343 1.94996 10.4348 1.94996 10.4352 1.94996L10.4341 2.69996L10.4327 1.94996C10.4331 1.94996 10.4335 1.94996 10.4339 1.94996ZM13.1214 4.07707C12.2868 3.66289 11.3673 3.44821 10.4355 3.44996L10.433 3.44996C9.36086 3.44842 8.30784 3.73382 7.38321 4.27655C6.45858 4.81929 5.69605 5.59958 5.17473 6.53645C4.65341 7.47332 4.39232 8.53263 4.41853 9.60446C4.44475 10.6763 4.75732 11.7216 5.32382 12.6318C5.45888 12.8489 5.47411 13.1197 5.36422 13.3505L4.28601 15.615L6.55002 14.5365C6.78078 14.4266 7.05164 14.4418 7.26869 14.5768C8.05992 15.0689 8.95463 15.3706 9.88231 15.458C10.81 15.5454 11.7453 15.4161 12.6145 15.0805C13.4838 14.7448 14.2631 14.2118 14.8913 13.5236C15.5194 12.8353 15.9791 12.0106 16.2342 11.1144C16.4893 10.2182 16.5327 9.27499 16.3611 8.35913C16.1895 7.44328 15.8076 6.57978 15.2454 5.8367C14.6832 5.09362 13.9561 4.49125 13.1214 4.07707Z\" fill=\"currentColor\" fill-rule=\"evenodd\" clip-rule=\"evenodd\"></path><path d=\"M11.25 6.5C11.25 6.08579 10.9142 5.75 10.5 5.75C10.0858 5.75 9.75 6.08579 9.75 6.5V8.75H7.5C7.08579 8.75 6.75 9.08579 6.75 9.5C6.75 9.91421 7.08579 10.25 7.5 10.25H9.75V12.5C9.75 12.9142 10.0858 13.25 10.5 13.25C10.9142 13.25 11.25 12.9142 11.25 12.5V10.25H13.5C13.9142 10.25 14.25 9.91421 14.25 9.5C14.25 9.08579 13.9142 8.75 13.5 8.75H11.25V6.5Z\" fill=\"currentColor\" fill-rule=\"evenodd\" clip-rule=\"evenodd\"></path></svg>";
                                                commentsButton.ID = $"comments-btn-{reader["id"].ToString()}";
                                                commentsButton.CssClass = "comments-btn";
                                                commentsButton.Click += OnCommentsButtonClicked;

                                                commentsBtnCell.Controls.Add(commentsButton);
                                            }
                                        }
                                    }

                                    // Add the table cell to the row
                                    tableRow.Cells.Add(taskCell);
                                    tableRow.Cells.Add(commentsBtnCell);
                                }
                                else if (dataTypes[i] == "Status")
                                {

                                    TableCell tableCell = new TableCell();
                                    HiddenField statusHeaderHiddenField = new HiddenField();
                                    statusHeaderHiddenField.ID = $"status-header_hidden_field-{reader["id"]}";
                                    tableCell.CssClass = "table-cell status-cell";

                                    TableHeaderRow tableHeaderRow = (TableHeaderRow)Group.Rows[0];
                                    bool statusHeaderExists = false;

                                    // Check if the header already exists to prevent duplication
                                    foreach (TableCell cell in tableHeaderRow.Cells)
                                    {
                                        TextBox textBox = cell.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Text == $"{columns[i]}");
                                        if (textBox != null)
                                        {
                                            statusHeaderExists = true;
                                            break;
                                        }
                                    }

                                    if (!statusHeaderExists)
                                    {
                                        TableHeaderCell statusHeaderCell = new TableHeaderCell();

                                        TextBox statusHeaderTextBox = new TextBox();
                                        statusHeaderHiddenField.Value = columns[i].ToString();

                                        if (!string.IsNullOrEmpty(statusHeaderHiddenField.Value))
                                        {
                                            statusHeaderTextBox.Text = statusHeaderHiddenField.Value;
                                        }

                                        statusHeaderTextBox.ID = $"status-header-{reader["id"]}";
                                        statusHeaderTextBox.CssClass = "header-textbox";
                                        statusHeaderTextBox.TextChanged += EditHeaderText;

                                        statusHeaderCell.Controls.Add(statusHeaderTextBox);
                                        tableHeaderRow.Cells.Add(statusHeaderCell);
                                    }

                                    // for debugging purposes
                                    string status = reader[$"{columns[i]}"].ToString();

                                    Button statusBtn = new Button();
                                    statusBtn.Text = reader[$"{columns[i]}"].ToString();
                                    statusBtn.Click += OnCurrentStatusButtonClicked;
                                    statusBtn.ID = $"status-btn-{reader["id"].ToString()}";
                                    statusBtn.Attributes["class"] = "status-btn";
                                    statusBtn.Attributes["type"] = "button";

                                    if (reader[$"{columns[i]}"].ToString().Equals("Done"))
                                    {
                                        statusBtn.Attributes["style"] = "background-color: rgb(0, 200, 117); color: white;";
                                        tableCell.Style["background-color"] = "rgb(0, 200, 117)";
                                    }
                                    else if (reader[$"{columns[i]}"].ToString().Equals("Working On It"))
                                    {
                                        statusBtn.Attributes["style"] = "background-color: rgb(253, 188, 100); color: white;";
                                        tableCell.Style["background-color"] = "rgb(253, 188, 100)";
                                    }
                                    else if (reader[$"{columns[i]}"].ToString().Equals("Stuck"))
                                    {
                                        statusBtn.Attributes["style"] = "background-color: rgb(223, 47, 74); color: white;";
                                        tableCell.Style["background-color"] = "rgb(223, 47, 74)";
                                    }
                                    else
                                    {
                                        statusBtn.Attributes["style"] = "background-color: rgb(121, 126, 147); color: white;";
                                        tableCell.Style["background-color"] = "rgb(121, 126, 147)";
                                    }

                                    tableCell.Controls.Add(statusBtn);
                                    tableRow.Controls.Add(tableCell);
                                }
                                else if (dataTypes[i] == "Date")
                                {

                                    TableCell tableCell = new TableCell();
                                    HiddenField dateHeaderHiddenField = new HiddenField();
                                    dateHeaderHiddenField.ID = $"date-header_hidden_field-{reader["id"]}";
                                    tableCell.CssClass = "table-cell date-cell";

                                    TableHeaderRow tableHeaderRow = (TableHeaderRow)Group.Rows[0];
                                    bool dateHeaderExists = false;

                                    // Check if the header already exists to prevent duplication
                                    foreach (TableCell cell in tableHeaderRow.Cells)
                                    {
                                        TextBox textBox = cell.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Text == $"{columns[i]}");
                                        if (textBox != null)
                                        {
                                            dateHeaderExists = true;
                                            break;
                                        }
                                    }

                                    if (!dateHeaderExists)
                                    {
                                        TableHeaderCell dateHeaderCell = new TableHeaderCell();

                                        TextBox dateHeaderTextBox = new TextBox();
                                        dateHeaderHiddenField.Value = columns[i];

                                        if (!string.IsNullOrEmpty(dateHeaderHiddenField.Value))
                                        {
                                            dateHeaderTextBox.Text = dateHeaderHiddenField.Value;
                                        }

                                        dateHeaderTextBox.ID = $"date-header-{reader["id"]}";
                                        dateHeaderTextBox.CssClass = "header-textbox";
                                        dateHeaderTextBox.TextChanged += EditHeaderText;

                                        dateHeaderCell.Controls.Add(dateHeaderTextBox);
                                        tableHeaderRow.Cells.Add(dateHeaderCell);
                                    }

                                    Button dateBtn = new Button();
                                    dateBtn.Text = $"{reader[$"{columns[i]}"].ToString()}";
                                    dateBtn.ID = $"date-btn-{reader["id"].ToString()}";
                                    dateBtn.CssClass = "date-btn";
                                    dateBtn.Click += OnDateButtonClicked;

                                    tableCell.Controls.Add(dateBtn);
                                    tableRow.Controls.Add(tableCell);
                                }
                                else if (dataTypes[i] == "People")
                                {
                                    TableCell tableCell = new TableCell();
                                    HiddenField peopleHeaderHiddenField = new HiddenField();
                                    peopleHeaderHiddenField.ID = $"people-header_hidden_field-{reader["id"].ToString()}";
                                    tableCell.CssClass = "table-cell people-cell";

                                    TableHeaderRow tableHeaderRow = (TableHeaderRow)Group.Rows[0];
                                    bool peopleHeaderExists = false;

                                    // Check if the header already exists to prevent duplication
                                    foreach (TableCell cell in tableHeaderRow.Cells)
                                    {
                                        TextBox textBox = cell.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Text == $"{columns[i]}");
                                        if (textBox != null)
                                        {
                                            peopleHeaderExists = true;
                                            break;
                                        }
                                    }

                                    if (!peopleHeaderExists)
                                    {
                                        TableHeaderCell peopleHeaderCell = new TableHeaderCell();

                                        TextBox peopleHeaderTextBox = new TextBox();
                                        peopleHeaderHiddenField.Value = columns[i];

                                        if (!string.IsNullOrEmpty(peopleHeaderHiddenField.Value))
                                        {
                                            peopleHeaderTextBox.Text = peopleHeaderHiddenField.Value;
                                        }

                                        peopleHeaderTextBox.ID = $"people-header-{reader["id"]}";
                                        peopleHeaderTextBox.CssClass = "header-textbox";
                                        peopleHeaderTextBox.TextChanged += EditHeaderText;

                                        peopleHeaderCell.Controls.Add(peopleHeaderTextBox);
                                        tableHeaderRow.Cells.Add(peopleHeaderCell);
                                    }


                                    Button peopleBtn = new Button();

                                    using (SqlConnection connection1 = new SqlConnection(connectionString))
                                    {
                                        string countTeamMembersWhoGotTaskAssignedQuery = "SELECT COUNT(*) FROM assignedTasks WHERE task_id = @task_id";
                                        using (SqlCommand command1 = new SqlCommand(countTeamMembersWhoGotTaskAssignedQuery, connection1))
                                        {
                                            connection1.Open();

                                            command1.Parameters.AddWithValue("@task_id", reader["id"].ToString());
                                            int count = (int)command1.ExecuteScalar();

                                            connection1.Close();

                                            if (count > 0)
                                            {
                                                string getTeamMembersWhoGotTaskAssignedQuery = "SELECT * FROM assignedTasks WHERE task_id = @task_id";
                                                using (SqlCommand sqlCommand = new SqlCommand(getTeamMembersWhoGotTaskAssignedQuery, connection1))
                                                {
                                                    connection1.Open();

                                                    sqlCommand.Parameters.AddWithValue("@task_id", reader["id"].ToString());
                                                    using (SqlDataReader taskAssignedReader = sqlCommand.ExecuteReader())
                                                    {
                                                        while (taskAssignedReader.Read())
                                                        {
                                                            string full_name = this.GetSpecificUserFullName(Convert.ToInt32(taskAssignedReader["member_id"].ToString()));
                                                            string insideOfButton = full_name.Split(' ')[0].Substring(0, 1) + full_name.Split(' ')[1].Substring(0, 1);
                                                            peopleBtn.Text = $"{insideOfButton}";
                                                            peopleBtn.ID = $"people-btn-{reader["id"]}";
                                                            peopleBtn.CssClass = "people-btn";
                                                            peopleBtn.Click += OnPeopleButtonClicked;
                                                            peopleBtn.Style["vertical-align"] = "middle";
                                                            peopleBtn.Style["text-align"] = "center";

                                                            int r = Convert.ToInt32(this.getColor().Split(',')[0]);
                                                            int g = Convert.ToInt32(this.getColor().Split(',')[1]);
                                                            int b = Convert.ToInt32(this.getColor().Split(',')[2]);
                                                            peopleBtn.BackColor = Color.FromArgb(r, g, b);
                                                            peopleBtn.ForeColor = Color.White;

                                                            PostProfilePictureWrapper.Style["background-color"] = $"rgb({r}, {g}, {b}) !important";
                                                            NavbarProfilePicture.Style["background-color"] = $"rgb({r}, {g}, {b}) !important";
                                                        }
                                                    }

                                                    connection1.Close();
                                                }
                                            }
                                            else
                                            {
                                                peopleBtn.Text = "+";
                                                peopleBtn.ID = $"people-btn-{reader["id"]}";
                                                peopleBtn.CssClass = "people-btn";
                                                peopleBtn.Click += OnPeopleButtonClicked;
                                            }
                                        }
                                    }
                                }
                            }

                            Group.Rows.Add(tableRow);
                        }
                    }
                }
            }

            HtmlGenericControl addColumnBtn = new HtmlGenericControl("button");
            addColumnBtn.Attributes["id"] = "AddColumnButton";
            addColumnBtn.Attributes["class"] = "add-column-btn";
            addColumnBtn.Attributes["type"] = "button";
            addColumnBtn.Attributes["onclick"] = "showColumnsMenu()";
            addColumnBtn.InnerText = "+";

            TableHeaderRow headerRow = (TableHeaderRow)Group.Rows[0];
            bool addColumnBtnHeaderExists = false;

            foreach (TableCell cell in headerRow.Cells)
            {
                foreach (Control control in cell.Controls)
                {
                    if (control is HtmlGenericControl btn && btn.Attributes["id"] == "AddColumnButton")
                    {
                        addColumnBtnHeaderExists = true;
                        break;
                    }
                }
                if (addColumnBtnHeaderExists)
                    break;
            }

            if (!addColumnBtnHeaderExists)
            {
                TableHeaderCell addColumnBtnHeaderCell = new TableHeaderCell();
                addColumnBtnHeaderCell.Controls.Add(addColumnBtn);
                headerRow.Cells.Add(addColumnBtnHeaderCell);
            }
        }
        protected void EditHeaderText(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            TextBox editedHeaderText = sender as TextBox;
            int id = Convert.ToInt32(editedHeaderText.ID.Split('-')[2]);
            int board_id = Convert.ToInt32(this.getBoardId());

            if (editedHeaderText.Text != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string oldColumnHeaderText = "";
                    string getOldColumnNameQuery = "SELECT column_name FROM dataOfTables WHERE user_id = @user_id AND id = @id AND board_id = @board_id";
                    using (SqlCommand command = new SqlCommand(getOldColumnNameQuery, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@board_id", board_id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                oldColumnHeaderText += reader.GetString(0);
                            }
                        }

                        connection.Close();
                    }

                    string changeColumnNameInColumnsTableQuery = "UPDATE columnsOfTables SET column_name = @column_name WHERE column_name = @old_column_name AND id = @id AND user_id = @user_id AND board_id = @board_id";
                    using (SqlCommand command = new SqlCommand(changeColumnNameInColumnsTableQuery, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@old_column_name", oldColumnHeaderText);
                        command.Parameters.AddWithValue("@column_name", editedHeaderText.Text);
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@board_id", board_id);
                        command.ExecuteNonQuery();

                        connection.Close();
                    }

                    string changeColumnNameIDatasTableQuery = "UPDATE dataOfTables SET column_name = @column_name WHERE column_name = @old_column_name AND user_id = @user_id AND board_id = @board_id";
                    using (SqlCommand command = new SqlCommand(changeColumnNameIDatasTableQuery, connection))
                    {
                        connection.Open();

                        command.Parameters.AddWithValue("@old_column_name", oldColumnHeaderText);
                        command.Parameters.AddWithValue("@column_name", editedHeaderText.Text);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@board_id", board_id);
                        command.ExecuteNonQuery();

                        connection.Close();
                    }
                }
            }
        }

        public string UserInitials
        {
            get
            {
                var fullName = GetCurrentlyLoggedInUserFullName();
                if (!string.IsNullOrEmpty(fullName))
                {
                    var names = fullName.Split(' ');
                    if (names.Length >= 2)
                    {
                        return names[0].Substring(0, 1) + names[1].Substring(0, 1);
                    }
                }
                return string.Empty;
            }
        }
    }
}

public class Post
{
    public string postID { get; set; }
    public string username { get; set; }
    public string insideOfPfp { get; set; }
    public string post { get; set; }
    public string date { get; set; }
    public string likes { get; set; }
    public string colorValue { get; set; }
    public List<Reply> Replies { get; set; } = new List<Reply>();
}

public class Reply
{
    public string replyID { get; set; }
    public string username { get; set; }
    public string insideOfPfp { get; set; }
    public string reply { get; set; }
    public string likes { get; set; }
    public string date { get; set; }
    public string colorValue { get; set; }
}

public class Board
{
    public string boardID { get; set; }
    public string boardName { get; set; }
}
