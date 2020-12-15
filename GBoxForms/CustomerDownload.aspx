<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CustomerDownload.aspx.vb"
    Inherits="GBoxForms.CustomerDownload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>GBOX Download Request</title>
    <link rel="Stylesheet" type="text/css" href="/_styles/uaCss.css" media="screen" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Table ID="Table1" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0" Width="600px">
            <asp:TableRow Height="15px">
                <asp:TableCell ColumnSpan="2">
                    <h1>
                        GBOX Download Request
                    </h1>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>&nbsp;<br /></asp:TableCell>
                <asp:TableCell>&nbsp;<br /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Height="15px">
                <asp:TableCell ColumnSpan="2">
                    <p>
                        Choose a download from the list and click the Download Button&nbsp;&nbsp;
                        <br />                        
                    </p>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Height="15px">
                <asp:TableCell ColumnSpan="2">
                    <asp:Label ID="lblErr" runat="server" Font-Bold="True" ForeColor="Red" CssClass=""></asp:Label>
                    <asp:Label ID="lblGreen" runat="server" Font-Bold="True" ForeColor="#009900"></asp:Label>
                    <br />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Height="15px">
                <asp:TableCell Width="100px">Download</asp:TableCell>
                <asp:TableCell Width="400px">
                    <asp:DropDownList ID="cboReqFor0" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>&nbsp;<br /></asp:TableCell>
                <asp:TableCell>&nbsp;<br /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Height="15px">
                <asp:TableCell >Description</asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="lblDesc" runat="server"></asp:Label></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>&nbsp;<br /></asp:TableCell>
                <asp:TableCell>&nbsp;<br /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Height="15px">
                <asp:TableCell>Long Description</asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="lblLongDesc" runat="server"></asp:Label></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>&nbsp;<br /></asp:TableCell>
                <asp:TableCell>&nbsp;<br /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Height="15px">
                <asp:TableCell ColumnSpan="2">
                    <asp:Button ID="btnDownload" runat="server" Text="Start Download" /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>&nbsp;<br /></asp:TableCell>
                <asp:TableCell>&nbsp;<br /></asp:TableCell>
            </asp:TableRow>
        </asp:Table>        
    </div>
    </form>
</body>
</html>
