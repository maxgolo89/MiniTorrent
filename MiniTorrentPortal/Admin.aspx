﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="MiniTorrentPortal.Admin" %>

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
                <li class="active"><a href="AdminLogin.aspx">Admin Page</a></li>
                <li><a href="Registration.aspx">Registraition</a></li>
            </ul>
        </div>
    </div>
</nav>
<div class="container">
    <div class="row">
        <form id="form1" runat="server">
            <h1>Welcome to the admin page</h1>
            <!-- Currently logged in user grid -->
            <div class="col-md-6">
                <h3>Currently logged in users: <asp:Label runat="server" ID="NumberOfUsers" Text=""></asp:Label></h3>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSource1" DataKeyNames="Username" OnRowDeleted="GridView1_OnRowDeleted">
                    <Columns>
                        <asp:CommandField ShowDeleteButton="True" />
                        <asp:BoundField DataField="Username" HeaderText="Username" SortExpression="Username" />
                        <asp:BoundField DataField="Ip" HeaderText="Ip" SortExpression="Ip" />
                        <asp:BoundField DataField="Port" HeaderText="Port" SortExpression="Port" />
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" DeleteMethod="DeleteLoggedInUser" SelectMethod="ReadLoggedInUsers" TypeName="MiniTorrentDAL.MiniTorrentCrud">
                    <DeleteParameters>
                        <asp:Parameter Name="username" Type="String" />
                    </DeleteParameters>
                </asp:ObjectDataSource>
            </div>
            <!-- Currently available files grid -->
            <div class="col-md-6">
                <h3>Currently available files: <asp:Label runat="server" ID="NumberofFiles" Text=""></asp:Label></h3>
                <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSource2">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                        <asp:BoundField DataField="Size" HeaderText="Size" SortExpression="Size" />
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" SelectMethod="ReadFiles" TypeName="MiniTorrentDAL.MiniTorrentCrud"></asp:ObjectDataSource>
            </div>
        </form>
    </div>
</div>
</body>
</html>
