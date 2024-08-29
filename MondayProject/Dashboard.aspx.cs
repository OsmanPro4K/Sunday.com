using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;

namespace MondayProject
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private string row_id;
        private string task;
        private string statusButtonID;

        void setRowId(string row_id)
        {
            ViewState["row_id"] = row_id;
        }

        string getRowId()
        {
            return ViewState["row_id"] as string;
        }

        void setTask(string task)
        {
            ViewState["task"] = task;
        }
        string getTask()
        {
            return ViewState["task"] as string;
        }

        void setStatusButtonId(string statusButtonID) { ViewState["statusButtonID"] = this.statusButtonID; }
        string getStatusButtonId() { return ViewState["statusButtonID"] as string; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["UserId"]) < 1)
            {
                Response.Redirect("Login.aspx");
            }

            

            // Load the table data
            UpdateTable();
        }

        protected void OnEnteredPressInTaskTextBox(object sender, EventArgs e)
        {
            string task = this.taskTextBox.Text;

            if (!string.IsNullOrEmpty(task))
            {
                StoreInDataBase(task);
            }

            UpdateTable();
        }

        protected void OnTaskEditted(object sender, EventArgs e)
        {
            TextBox editedTask = (TextBox)sender;
            Label1.Text = editedTask.Text;

            // Retrieve the row_id from the textbox's ID by extracting the numeric part
            string row_id = editedTask.ID.Replace("taskTextBox", "");

            // Update the task in the database using the row_id
            UpdateTaskInDatabase(row_id, editedTask.Text);
        }

        protected void OnOptionsButtonPressed(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button != null)
            {
                Label1.Text = $"Options Button with ID {button.ID} was pressed.";

                // Script for removing attribute
                string script = "document.getElementById('options-menu').removeAttribute('hidden')";
                ClientScript.RegisterStartupScript(this.GetType(), "showOptionsMenu", script, true);

                // Set Row ID for Delete Button
                setRowId(button.ID);
            }
        }

        protected void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            Label1.Text = $"Delete button for row {this.getRowId().Split('-')[2]} was pressed";

            int rowThatGetsDeleted = Convert.ToInt32(this.getRowId().Split('-')[2]);

            DeleteRowinDatabase(rowThatGetsDeleted);
        }

        protected void OnDuplicateButtonClicked(object sender, EventArgs e)
        {
            Label1.Text = $"Copy button for row {this.getRowId().Split('-')[2]} was pressed";

            int rowThatGetsCopied = Convert.ToInt32(this.getRowId().Split('-')[2]);

            DuplicateRowInDatabase(rowThatGetsCopied);
        }

        protected void OnAddColumnButtonClicked(object sender, EventArgs e)
        {
            string script = "document.getElementById('columns-menu').removeAttribute('hidden')";
            ClientScript.RegisterStartupScript(this.GetType(), "showColumnsMenu", script, true);
        }

        // Status Column

        protected void OnAddStatusToColumnClicked(object sender, EventArgs e)
        {
            AddColumnToDatabase("Status", "status", "VarChar(25)", "'Not Started'");
            UpdateTable();
        }

        protected void OnCurrentStatusClicked(object sender, EventArgs e)
        {
            string script = "document.getElementById('status-menu').removeAttribute('hidden')";
            ClientScript.RegisterStartupScript(this.GetType(), "showStatusMenu", script, true);

            Button statusButton = sender as Button;

            Label1.Text = statusButton.ID.ToString();

            this.setRowId(statusButton.ID);
        }

        protected void OnDoneStatusClicked(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            int row_id = Convert.ToInt32(this.getRowId().Split('-')[2]);
            Label1.Text += row_id;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE mainWorkSpace SET status = 'Done' WHERE user_id = @user_id AND row_id = @row_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@row_id", row_id);

                    command.ExecuteNonQuery();
                }
            }

            UpdateTable();
        }

        protected void OnWorkingOnItStatusClicked(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            int row_id = Convert.ToInt32(this.getRowId().Split('-')[2]);
            Label1.Text += row_id;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE mainWorkSpace SET status = 'Working On It' WHERE user_id = @user_id AND row_id = @row_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@row_id", row_id);

                    command.ExecuteNonQuery();
                }
            }

            UpdateTable();
        }

        protected void OnStuckStatusClicked(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            int row_id = Convert.ToInt32(this.getRowId().Split('-')[2]);
            Label1.Text += row_id;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE mainWorkSpace SET status = 'Stuck' WHERE user_id = @user_id AND row_id = @row_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@row_id", row_id);

                    command.ExecuteNonQuery();
                }
            }

            UpdateTable();
        }

        protected void OnNotStartedStatusClicked(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            int row_id = Convert.ToInt32(this.getRowId().Split('-')[2]);
            Label1.Text += row_id;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE mainWorkSpace SET status = 'Not Started' WHERE user_id = @user_id AND row_id = @row_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@row_id", row_id);

                    command.ExecuteNonQuery();
                }
            }

            UpdateTable();
        }

        // Date Column
        protected void OnAddDateToColumnClicked(object sender, EventArgs e)
        {
            AddColumnToDatabase("Date", "date", "VarChar(25)", "'June 9, 2024'");
            UpdateTable();
        }

        protected void OnDateClicked(object sender, EventArgs e)
        {
            Button dateButton = sender as Button;
            if (dateButton != null)
            {
                string rowId = dateButton.ID.Split('-').Last();
                string columnName = "date"; // Adjust column name if necessary

                // Create and configure the calendar control
                Calendar calendar = new Calendar();
                calendar.ID = $"calendar-{rowId}-{columnName}";

                // Attach client-side event handler to prevent default postback behavior
                calendar.Attributes["onclick"] = "preventPostback(event);";

                // Find the parent cell and add the calendar control
                TableCell cell = dateButton.Parent as TableCell;
                if (cell != null)
                {
                    cell.Controls.Clear();
                    cell.Controls.Add(calendar);
                }

                this.setRowId(dateButton.ID);
            }
        }


        protected void OnCalendarSelectionChanged(object sender, EventArgs e)
        {
            Calendar selectedCalendar = sender as Calendar;
            if (selectedCalendar != null)
            {
                // Get the selected date
                DateTime selectedDate = selectedCalendar.SelectedDate;

                // Storing in DB 
                const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
                int user_id = Convert.ToInt32(Session["UserId"]);
                int row_id = Convert.ToInt32(this.getRowId().Split('-')[2]);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE mainWorkSpace SET date = @date WHERE user_id = @user_id AND row_id = @row_id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        
                        command.Parameters.AddWithValue("@date", selectedDate);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        command.Parameters.AddWithValue("@row_id", row_id);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        protected void StoreInDataBase(string task)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO mainWorkSpace (user_id, task) VALUES (@user_id, @task)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@task", task);

                    command.ExecuteNonQuery();
                }
            }

            UpdateTable();
        }

        protected void DeleteRowinDatabase(int rowThatGetsDeleted)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            // Deleting data from database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM mainWorkSpace WHERE user_id = @user_id AND row_id = @row_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@row_id", rowThatGetsDeleted);

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            UpdateTable();
        }

        protected void DuplicateRowInDatabase(int rowThatGetsCopied)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            string task = "";

            // Copying data from database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT task FROM mainWorkSpace WHERE user_id = @user_id AND row_id = @row_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@row_id", rowThatGetsCopied);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            task = reader["task"].ToString();
                            this.setTask(task);
                            // Debugging output
                            Label1.Text += $" - Task copied: {getTask()}";
                        }
                    }
                }

                query = "INSERT INTO mainWorkSpace (user_id, task) VALUES (@user_id, @task)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@task", task);

                    command.ExecuteNonQuery();
                }
            }

            UpdateTable();
        }

        protected void UpdateTable()
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM mainWorkSpace WHERE user_id = @user_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@user_id", user_id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Clear existing rows, excluding the header row
                        for (int i = Group.Rows.Count - 1; i > 0; i--)
                        {
                            Group.Rows.RemoveAt(i);
                        }

                        // Get column names from the schema table
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

                        // Find the header cell for the 'Status' column
                        /*int statusHeaderIndex = columnNames.IndexOf("status");
                        if (statusHeaderIndex != -1) // Ensure the 'Status' column exists
                        {
                            statusHeaderIndex++; // Increment by 1 to match ASP.NET table cell index
                            if (statusHeaderIndex < Group.Rows[0].Cells.Count) // Ensure the index is within bounds
                            {
                                Group.Rows[0].Cells[statusHeaderIndex].Controls.Add(addColumnCell);
                            }
                        }*/

                        while (reader.Read())
                        {
                            TableRow tableRow = new TableRow();

                            foreach (string columnName in columnNames)
                            {
                                TableCell cell = new TableCell();

                                if (columnName == "task")
                                {
                                    cell.CssClass = "task-cell";

                                    TextBox textBox = new TextBox
                                    {
                                        ID = $"taskTextBox{reader["row_id"]}",
                                        Text = reader[columnName].ToString(),
                                        AutoPostBack = true
                                    };
                                    textBox.TextChanged += OnTaskEditted;

                                    Button button = new Button
                                    {
                                        Text = "...",
                                        ID = $"options-button-{reader["row_id"]}"
                                    };
                                    button.Click += OnOptionsButtonPressed;

                                    cell.Controls.Add(button);
                                    cell.Controls.Add(textBox);
                                }
                                else if (columnName == "status")
                                {
                                    bool statusHeaderExists = false;

                                    // Check if a header cell with the name "status" already exists
                                    foreach (TableCell headerCell in Group.Rows[0].Cells)
                                    {
                                        if (headerCell.Text.ToLower() == "status")
                                        {
                                            statusHeaderExists = true;
                                            break;
                                        }
                                    }

                                    // If the "status" header cell doesn't exist, add it
                                    if (!statusHeaderExists && columnName == "status")
                                    {
                                        TableHeaderCell statusHeaderCell = new TableHeaderCell();
                                        statusHeaderCell.Text = "status";
                                        Group.Rows[0].Cells.Add(statusHeaderCell);
                                    }

                                    cell.CssClass = "status-cell";
                                    Button statusButton = new Button();
                                    statusButton.ID = $"status-id-{reader["row_id"]}";
                                    statusButton.Text = reader[columnName].ToString();
                                    if (statusButton.Text == "Done")
                                    {
                                        statusButton.BackColor = Color.Green;
                                    }
                                    else if (statusButton.Text == "Working On It")
                                    {
                                        statusButton.BackColor = Color.Yellow;
                                    }
                                    else if (statusButton.Text == "Stuck")
                                    {
                                        statusButton.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        statusButton.BackColor = Color.Gray;
                                    }

                                    statusButton.Click += OnCurrentStatusClicked;
                                    cell.Controls.Add(statusButton);
                                }
                                else if (columnName == "date")
                                {
                                    bool dateHeaderExists = false;

                                    // Check if a header cell with the name "date" already exists
                                    foreach (TableCell headerCell in Group.Rows[0].Cells)
                                    {
                                        if (headerCell.Text.ToLower() == "date")
                                        {
                                            dateHeaderExists = true;
                                            break;
                                        }
                                    }

                                    // If the "date" header cell doesn't exist, add it
                                    if (!dateHeaderExists && columnName == "date")
                                    {
                                        TableHeaderCell dateHeaderCell = new TableHeaderCell();
                                        dateHeaderCell.Text = "date";
                                        Group.Rows[0].Cells.Add(dateHeaderCell);
                                    }


                                    // Create and configure the date label
                                    Button dateButton = new Button();
                                    dateButton.ID = $"date-id-{reader["row_id"]}";
                                    dateButton.CssClass = "date-button";
                                    dateButton.Text = reader[columnName].ToString();
                                    dateButton.Click += OnDateClicked;


                                    // Attach the click event handler to open the calendar control
                                    dateButton.Attributes["onclick"] = $"showCalendar('{dateButton.ClientID}');";

                                    cell.Controls.Add(dateButton);
                                    tableRow.Cells.Add(cell);
                                }

                                tableRow.Cells.Add(cell);
                            }

                            Group.Rows.Add(tableRow);
                        }

                        // Add the "Add Column Button
                        TableHeaderCell addColumnButtonHeaderCell = new TableHeaderCell();
                        addColumnButtonHeaderCell.HorizontalAlign = HorizontalAlign.Center; // Optional: Center the button
                        addColumnButtonHeaderCell.ColumnSpan = 2; // Optional: Set column span for the button
                        Group.Rows[0].Cells.Add(addColumnButtonHeaderCell);

                        Button addColumnButton = new Button();
                        addColumnButton.Text = "+";
                        addColumnButton.ID = "AddColumnButton";
                        addColumnButton.Click += OnAddColumnButtonClicked;
                        addColumnButtonHeaderCell.Controls.Add(addColumnButton);
                    }
                }
            }
        }


        protected void UpdateTaskInDatabase(string row_id, string newTask)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);
            int rowIdInt;

            // Convert row_id to integer
            if (!int.TryParse(row_id, out rowIdInt))
            {
                // Handle the error when row_id is not a valid integer
                Label1.Text = "Invalid row_id: " + row_id;
                return;
            }

            Label1.Text = newTask + user_id + row_id;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE mainWorkSpace SET task = @newTask WHERE user_id = @user_id AND row_id = @row_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@newTask", newTask);
                    command.Parameters.AddWithValue("@row_id", rowIdInt);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Task updated successfully
                        UpdateTable();
                    }
                    else
                    {
                        // Task update failed
                    }
                }
            }
        }

        protected void AddColumnToDatabase(string columnDisplayName, string columnName, string columnType, string defaultValue)
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=sunday.com;Integrated Security=True;Encrypt=False";
            int user_id = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Add column to the table
                string query = $"ALTER TABLE mainWorkSpace ADD {columnName} {columnType}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Update the column with default values
                query = $"UPDATE mainWorkSpace SET {columnName} = {defaultValue} WHERE {columnName} IS NULL";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}