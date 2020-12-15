<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DownLoad.aspx.vb" Inherits="GBoxForms.DownLoadMarty" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Marty Download</title>
    <link rel="Stylesheet" type="text/css" href="/_styles/uaCss.css" media="screen" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p class="mc-bd-simpletitle">
        <asp:Label ID="lblTitle" runat="server" 
                Text="Download the Current Programm Version:"></asp:Label>
        </p>
        <p>      
        <asp:Button ID="cmdDownload" runat="server" 
            Text="Download Current MARTY Version" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;      
        <asp:Button ID="btnDownloadPreviousMarty" runat="server" 
            Text="Download Previous MARTY Version" />
        </p>
        <p>      
        <asp:Button ID="cmdDownload0" runat="server" 
            Text="Download Current GBOXMANAGER Version" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;      
        <asp:Button ID="btnDownloadPreviousGBOXMGR" runat="server" 
            Text="Download Previous GBOXMANAGER Version" />
        </p>
        <p>
            <asp:Label ID="lblCurrentUser" runat="server" Text="Label"></asp:Label>
        </p> 
    </div>
    <p>
    <asp:Label ID="lblError" runat="server"></asp:Label>
    </p>
    <p>
        &nbsp;</p>
    </form>
</body>
</html>
