<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Security.aspx.vb" Inherits="GBoxForms.Security" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Unbenannte Seite</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Button ID="cmdTest" runat="server" Text="Test User CWID" />
        <asp:Label ID="lblUser" runat="server"></asp:Label>
    
    </div>
    <asp:Label ID="lblContext" runat="server"></asp:Label>
    </form>
</body>
</html>
