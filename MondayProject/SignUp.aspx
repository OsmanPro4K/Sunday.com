<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="MondayProject.SignUp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>sunday.com: Your team's growth made easy | Sign Up</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap" rel="stylesheet">
    <style>
        body {
            font-family: "Poppins", sans-serif;
            margin-left: auto;
            margin-right: auto;
        }

        label {
            font-size: 15px;
            font-weight: 300;
        }

        input {
            border: 1px solid gray;
        }

        a {
            text-decoration: none;
        }

            a:hover {
                text-decoration: underline;
            }

        .signup-form {
            float: left;
            padding: 20px;
            display: flex;
            align-content: center;
            justify-content: center;
            flex-direction: column;
        }

        .email-screen {
            display: grid;
            grid-template-columns: auto 40%;
            height: 100vh;
            width: 100vw;
        }

        .image {
            background-color: rgb(97, 96, 255);
            display: flex;
            align-items: center;
            justify-content: center;
        }

        @media (max-width: 768px) {
            .signup-form,
            .image {
                float: none;
                width: auto;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="email-screen">
            <div class="text-center mx-auto w-75 signup-form">
                <h1 class="title">Welcome to sunday.com</h1>
                <p>Get started - it's free. No credit card needed.</p>

                <input id="SignUp_Email" runat="server" type="text" class="form-group mx-auto w-75 py-2 my-3 mt-5 px-3 rounded-3" placeholder="name@company.com" />
                <button class="btn btn-primary mx-auto w-75 py-2 rounded-2 continue-btn">Continue</button>
                <asp:Label ID="ErrorMessage" runat="server" Text="" CssClass="text-danger"></asp:Label>

                <p class="mt-3 mb-5">
                    By proceeding, you agree to the
                <br />
                    <a href="#">Terms of Service</a> and <a href="#">Privacy Policy</a>
                </p>
                <p class="my-5">
                    Already have an account? <a href="Login.aspx">Login</a>
                </p>
            </div>
            <div class="image">
                <asp:Image ID="SignUp_Image" runat="server" CssClass="w-100" ImageUrl="https://dapulse-res.cloudinary.com/image/upload/monday_platform/signup/signup-right-side-assets-new-flow/welcome-to-monday.png" />
            </div>
        </div>
        <div class="extra-info-screen" hidden="hidden">
            <div class="w-50 mt-5 mx-5 signup-form">
                <asp:Image ID="Logo" runat="server" ImageUrl="~/assets/logo-full-big.png" Width="150px" Height="10%" CssClass="mb-5" />
                <br />

                <h3>Create Your Account</h3>
                <label for="SignUp_FullName">Full Name</label>
                <br />
                <input id="SignUp_FullName" runat="server" type="text" class="form-group w-75 py-2 mb-3 px-3 rounded-3" placeholder="Enter your full name" />
                <br />
                <label for="SignUp_Password">Password</label>
                <br />
                <input id="SignUp_Password" runat="server" type="password" class="form-group w-75 py-2 mb-3 px-3 rounded-3" placeholder="Password must be more than 8 characters" />
                <br />
                <label for="SignUp_Username">Username</label>
                <br />
                <input id="SignUp_Username" runat="server" type="text" class="form-group w-75 py-2 mb-3 px-3 rounded-3" placeholder="Enter your username" />

                <asp:Button ID="ContinueBtn" runat="server" Text="Continue ➜" CssClass="btn btn-primary w-75 py-2 rounded-2" OnClick="OnContinueBtnClicked" />
            </div>
            <div class="image">
                <asp:Image ID="Image1" runat="server" Width="400px" ImageUrl="https://dapulse-res.cloudinary.com/image/upload/monday_platform/signup/signup-right-side-assets-new-flow/set-up-your-account.png" />
            </div>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>
    <script src="https://code.jquery.com/jquery-3.7.1.js" integrity="sha256-eKhayi8LEQwp4NKxN+CfCh+3qOVUtJn3QNZ0TciWLP4=" crossorigin="anonymous"></script>
    <script type="text/javascript">
        let emailScreen = $('.email-screen');
        let extraInfoScreen = $('.extra-info-screen')
        let continueBtn = $('.continue-btn');

        continueBtn.on('click', (event) => {
            event.preventDefault();
            extraInfoScreen.removeAttr('hidden');
            emailScreen.attr('hidden', 'hidden');
        })
    </script>
</body>
</html>
