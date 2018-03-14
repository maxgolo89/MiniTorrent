using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using MiniTorrentDAL;
using MiniTorrentDAL.Messeges;

namespace MiniTorrentPortal
{
    public partial class Admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NumberOfUsers.Text = GridView1.Rows.Count.ToString();
            NumberofFiles.Text = GridView2.Rows.Count.ToString();
        }

        protected void GridView1_OnRowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            GridView2.DataBind();
            GridView1.DataBind();
            NumberofFiles.Text = GridView2.Rows.Count.ToString();
            NumberOfUsers.Text = GridView1.Rows.Count.ToString();
        }
    }
}