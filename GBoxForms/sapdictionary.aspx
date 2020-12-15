<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="sapdictionary.aspx.vb" Inherits="GBoxForms.SapDictionary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Unbenannte Seite</title>
    <link rel="stylesheet" type="text/css" href="_styles/uaCss.css" />
</head>
<body>
    <form id="DataViewer" runat="server">
    <asp:Label ID="lblTitle" runat="server" Text="SAP Dictionary" 
        CssClass="mc-bd-toptitle"></asp:Label>
    <p />
        <asp:TextBox ID="txtSearchstring" runat="server"></asp:TextBox>
        <asp:Button ID="cmdStart" runat="server" Text="Search" />
   <div>
        <p />
       <asp:Label ID="lblCountFound" runat="server" Text=""></asp:Label>
       <p />
       <asp:GridView ID="grdData" runat="server" AllowSorting="True" CellPadding="4" 
           ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" 
                DataSourceID="mySqlDataSource">
           <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
           <RowStyle BackColor="#EFF3FB" />
           <Columns>
               <asp:BoundField DataField="English" HeaderText="English" 
                   SortExpression="English" />
               <asp:BoundField DataField="German" HeaderText="German" 
                   SortExpression="German" />
               <asp:BoundField DataField="Module" HeaderText="Module" 
                   SortExpression="Module" />
           </Columns>
           <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
           <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
           <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
           <EditRowStyle BackColor="#2461BF" />
           <AlternatingRowStyle BackColor="White" />
       </asp:GridView>
        
            <asp:SqlDataSource ID="mySqlDataSource" runat="server" 
                ConnectionString="" 
                ProviderName="System.Data.SqlClient" 
                SelectCommand="SELECT * FROM [Dictionary] WHERE (([English] LIKE '%' + @English + '%') OR ([German] LIKE '%' + @German + '%'))">
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtSearchstring" Name="English" 
                        PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="txtSearchstring" Name="German" 
                        PropertyName="Text" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
        
    </div>
    </form>
</body>
</html>
