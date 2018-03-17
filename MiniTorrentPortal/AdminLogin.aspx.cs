using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniTorrentPortal
{
    public partial class AdminLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UsernameLabel.Attributes.Add("for", "UsernameTextBox");
            UsernameTextBox.Attributes.Add("class", "form-control");
            UsernameTextBox.Attributes.Add("placeholder", "Username");

            PasswordLabel.Attributes.Add("for", "PasswordTextBox");
            PasswordTextBox.Attributes.Add("class", "form-control");
            PasswordTextBox.Attributes.Add("placeholder", "Password");

            SubmitButton.Attributes.Add("class", "btn btn-default");
        }

        protected void SubmitButton_OnClick(object sender, EventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;
            if(username != null && password != null && username.Equals("admin") && password.Equals("admin"))
                Response.Redirect("Admin.aspx");
            else
            {
                LoginFeedback.Text = "Login failed, check the credentials.";
            }
        }
    }
}