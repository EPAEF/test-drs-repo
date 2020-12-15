<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Search.aspx.vb" Inherits="GBoxForms.Search" StyleSheetTheme="" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head >
 <link rel="Stylesheet" type="text/css" href="/_styles/stadard.css" media="screen" />

</head>

<body bgcolor="#ffffff"  >
    <form id="form1" runat="server"  >
    <div>
    <asp:Label ID="lblFiltertext" runat="server" Font-Size="Small"></asp:Label>
    <asp:TextBox ID="txtKeywords" runat="server" Width="177px" 
                         AutoCompleteType="Search"></asp:TextBox>
                     <asp:ImageButton ID="imgEngine" runat="server" 
                         ImageUrl="~/Images/computer_go.gif" ToolTip="Search" TabIndex="9" 
                    onclientclick="target='_blank'" Height="24px" Width="22px" />
                     <asp:CheckBox ID="chkDrsSettings" runat="server" Text="DRS Handbook only" 
                            Font-Size="Small" />
        <hr />
                    <asp:Table ID="myDynamicTable" runat="server" GridLines="Both" Font-Size="Small" Visible="False"></asp:Table> 
    </div>
    </form>
</body>
</html>
