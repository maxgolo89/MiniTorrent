using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MiniTorrentDAL;
using Console = System.Console;

namespace MiniTorrentPortal
{
    public partial class Registration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UsernameLabel.Attributes.Add("for", "UsernameTextBox");
            UsernameTextBox.Attributes.Add("class", "form-control");
            UsernameTextBox.Attributes.Add("placeholder", "Username");

            PasswordLabel.Attributes.Add("for", "PasswordTextBox");
            PasswordTextBox.Attributes.Add("class", "form-control");
            PasswordTextBox.Attributes.Add("placeholder", "Password");

            PasswordConfirmLabel.Attributes.Add("for", "UsernameTextBox");
            PasswordConfirmTextBox.Attributes.Add("class", "form-control");
            PasswordConfirmTextBox.Attributes.Add("placeholder", "Confirm Password");

            SubmitButton.Attributes.Add("class", "btn btn-default");
        }

        protected void SubmitButton_OnClick(object sender, EventArgs e)
        {
            
            string username = UsernameTextBox.Text;
            string password1 = PasswordTextBox.Text;
            string password2 = PasswordConfirmTextBox.Text;

            if (username != string.Empty && password1 != string.Empty && password1.Equals(password2))
            {
                MiniTorrentCrud db = new MiniTorrentCrud();
                string confirmation = db.CreateUser(username, password1);
                if (confirmation != null)
                {
                    UsernameTextBox.Attributes.Add("disabled", "");
                    PasswordTextBox.Attributes.Add("disabled", ""); ;
                    PasswordConfirmTextBox.Attributes.Add("disabled", ""); ;
                    SubmitButton.Attributes.Add("disabled", ""); ;
                    RegistrationFeedback.Text = "Registration completed, feel free to login using the client.";
                }
                else
                {
                    RegistrationFeedback.Text = "Registration failed, username probably taken.";
                }
            }
            else
            {
                RegistrationFeedback.Text = "Please make sure all fields are filled, and password fields match.";
            }
        }
    }
}