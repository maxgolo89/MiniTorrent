<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="MiniTorrentPortal.Home" %>

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
                <li class="active"><a href="Home.aspx">Home</a></li>
                <li><a href="AdminLogin.aspx">Admin Page</a></li>
                <li><a href="Registration.aspx">Registraition</a></li>
            </ul>
        </div>
    </div>
</nav>
<div class="container">
    <div class="row">
        <form id="form1" runat="server">
            <div class="col-md-6 col-md-offset-3"></div>
            <div class="jumbotron">
                <h1>Welcome to My MiniTorrent!</h1>
                <p>The best MiniTorrent in Afeka!</p>
                <p><a class="btn btn-primary btn-lg" href="/AdminLogin.aspx" role="button">Admin Page</a> <a class="btn btn-primary btn-lg" href="/Registration.aspx" role="button">Registration</a></p>
            </div>
        </form>
    </div>
</div>
</body>
</html>
