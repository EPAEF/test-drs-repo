<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TagCloud.aspx.vb" Inherits="GBoxForms.TagCloud" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Tag Cloud</title>
    <link rel="Stylesheet" type="text/css" href="/_styles/tcloud.css" media="screen" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="tagcloudform">
        <asp:Label ID="lblTagcloud" runat="server" CssClass="tc"></asp:Label> 
        <%-- 
        <br />
        <asp:Label ID="lblInfo" runat="server"></asp:Label> 
        --%>
    </div>
    </form>
</body>
</html>
