<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="MiniTorrentPortal.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link  rel="stylesheet" type="text/css" href="~/content/css/bootstrap.min.css"/>
</head>
<body>
    <div class="container">
        <div class="row">
            <form id="form1" runat="server">
                <div class="col-md-6 col-md-offset-3">
                    <h1>Welcome to registration page</h1>
                    <div class="form-group">
                        <asp:Label runat="server" ID="UsernameLabel" Text="Username:"></asp:Label>
                        <asp:TextBox runat="server" ID="UsernameTextBox"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" ID="PasswordLabel" Text="Password:"></asp:Label>
                        <asp:TextBox runat="server" ID="PasswordTextBox" TextMode="Password"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" ID="PasswordConfirmLabel" Text="Confirm Password:"></asp:Label>
                        <asp:TextBox runat="server" ID="PasswordConfirmTextBox" TextMode="Password"></asp:TextBox>
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
