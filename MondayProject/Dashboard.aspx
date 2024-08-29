<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="MondayProject.Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .task-cell {
            position: relative;
        }

        .options-button {
            display: none;
            position: absolute;
            right: 5px;
            top: 5px;
        }

        .task-cell:hover .options-button {
            display: inline;
        }

        .task-textbox {
            width: calc(100% - 30px); /* Adjust the width of the textbox */
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Table ID="Group" runat="server">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>Task</asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:TextBox ID="taskTextBox" runat="server" placeholder="Enter Task" OnTextChanged="OnEnteredPressInTaskTextBox"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>

            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>

            <div id="options-menu" hidden="hidden">
                <asp:Button ID="CopyButton" runat="server" Text="Duplicate" OnClick="OnDuplicateButtonClicked" />
                <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="OnDeleteButtonClicked" />
            </div>

            <div id="columns-menu" hidden="hidden">
                <asp:Button ID="DateButton" runat="server" Text="Date" OnClick="OnAddDateToColumnClicked" />
                <asp:Button ID="StatusButton" runat="server" Text="Status" OnClick="OnAddStatusToColumnClicked" />
                <asp:Button ID="PeopleButton" runat="server" Text="People"  />
            </div>

            <div id="status-menu" hidden="hidden">
                <asp:Button ID="Status_Done" runat="server" Text="Done" OnClick="OnDoneStatusClicked"  />
                <asp:Button ID="Status_WorkingOnIt" runat="server" Text="Working On It" OnClick="OnWorkingOnItStatusClicked"  />
                <asp:Button ID="Status_Stuck" runat="server" Text="Stuck" OnClick="OnStuckStatusClicked" />
                <asp:Button ID="Status_NotStarted" runat="server" Text="Not Started" OnClick="OnNotStartedStatusClicked"  />
            </div>

            <div id="calendar-menu" hidden="hidden">
                <asp:Calendar ID="Calendar" runat="server" OnSelectionChanged="OnCalendarSelectionChanged"></asp:Calendar>
            </div>
        </div>
    </form>

    <script src="https://code.jquery.com/jquery-3.7.1.js" integrity="sha256-eKhayi8LEQwp4NKxN+CfCh+3qOVUtJn3QNZ0TciWLP4=" crossorigin="anonymous"></script>
    <script src="https://kit.fontawesome.com/9d2324ac46.js" crossorigin="anonymous"></script>

    <script>
        function preventPostback(e) {
            e.preventDefault();
        }
</script>
</body>
</html>
