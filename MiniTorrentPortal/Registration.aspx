<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="MiniTorrentPortal.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link  rel="stylesheet" type="text/css" href="~/content/css/bootstrap.min.css"/>
</head>
<body>
<nav class="navbar navbar-default">
  <div class="container-fluid">
    <div class="navbar-header">
      <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
        <span class="sr-only">Toggle navigation</span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
      </button>
      <a class="navbar-brand" href="#">MiniTorrent</a>
    </div>

    <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
      <ul class="nav navbar-nav">
        <li><a href="Home.aspx">Home</a></li>
        <li><a href="AdminLogin.aspx">Admin Page</a></li>
        <li class="active"><a href="Registration.aspx">Registraition</a></li>
      </ul>
    </div>
  </div>
</nav>
    <div class="container">
        <div class="row">
            <form id="form1" runat="server">
                <div class="col-md-6 col-md-offset-3">
                    <h1>Welcome to registration page</h1>
                    <div class="form-group">
                        <asp:Label runat="server" ID="UsernameLabel" Text="Username:"></asp:Label>
                        <asp:TextBox runat="server" ID="UsernameTextBox"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="RequiredFieldValidator" ControlToValidate="UsernameTextBox"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" ID="PasswordLabel" Text="Password:"></asp:Label>
                        <asp:TextBox runat="server" ID="PasswordTextBox" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="RequiredFieldValidator" ControlToValidate="PasswordTextBox"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" ID="PasswordConfirmLabel" Text="Confirm Password:"></asp:Label>
                        <asp:TextBox runat="server" ID="PasswordConfirmTextBox" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="RequiredFieldValidator" ControlToValidate="PasswordConfirmTextBox"></asp:RequiredFieldValidator>
                        <asp:CompareValidator runat="server" ErrorMessage="Password fields do not match!" ID="ComparePassword" ControlToCompare="PasswordTextBox" ControlToValidate="PasswordConfirmTextBox"></asp:CompareValidator>
                    </div>
                    <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_OnClick" />
                    <asp:Label runat="server" ID="RegistrationFeedback" Text=""></asp:Label>
                </div>
            </form>
        </div>
    </div>
</body>
</html>
