<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="MondayProject.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>sunday.com: Where Teams Get Work Done</title>
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
    <style>
        body {
            font-family: "Poppins", sans-serif;
            text-align: center;
        }

        label {
            text-align: right;
        }

        a {
            text-decoration: none;
        }

            a:hover {
                text-decoration: underline;
            }

        .navbar {
            padding: 10px;
            border-bottom: 1px solid #e0e0e0;
            background: #f7f7f7;
        }

        .form-control {
            border: 1px solid gray;
        }

        form#form1 {
            background: #ffe;
            padding: 2rem;
            border-radius: 20px;
            margin-top: -80px;
        }

        @media (min-width: 768px) {
            .login-container {
                max-width: 100%;
                height: 100vh;
                display: flex;
                align-items: center;
                justify-content: center;
            }
        }

        }
    </style>
</head>
<body>
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <div class="container-fluid">
            <a href="~/Login" class="navbar-brand">
                <img src="./assets/logo-full-big.png" alt="monday.com" width="250px" />
            </a>
        </div>
    </nav>
    <div class="container login-container">
        <form id="form1" runat="server">
            <div class="mb-5">
                <h1>Log in to your account</h1>
            </div>
            <div class="mb-3 row align-items-center">
                <label for="Login_Email" class="form-label text-muted col-sm-3 mb-0">Email</label>
                <div class="col-sm-9">
                    <input id="Login_Email" runat="server" class="form-control py-2 px-3 rounded-1" type="text" />
                </div>
            </div>
            <div class="mb-3 row align-items-center">
                <label for="Login_Password" class="form-label text-muted col-sm-3 mb-0">Password</label>
                <div class="col-sm-9">
                    <input id="Login_Password" runat="server" class="form-control py-2 px-3 rounded-1" type="password" />
                </div>
            </div>
            <asp:Button ID="Login_Button" runat="server" Text="Login" CssClass="btn btn-primary w-100" OnClick="OnLoginBtnClicked" />

            <asp:Label ID="ErrorMessage" runat="server" Text="" CssClass="text-danger"></asp:Label>

            <p class="mt-5 fw-light">
                Don't have an account yet? <a href="SignUp.aspx">Sign up</a>
                <br />
                Can't log in? <a href="#">Visit out help center</a>
            </p>
        </form>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>
    <script src="https://kit.fontawesome.com/9d2324ac46.js" crossorigin="anonymous"></script>
</body>
</html>
