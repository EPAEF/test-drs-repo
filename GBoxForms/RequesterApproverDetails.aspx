<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RequesterApproverDetails.aspx.vb"
    Inherits="GBoxForms.RequesterApproverDetails" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="frmRequesterApproverDetails" runat="server">
    <asp:Label ID="lblText" runat="server" Font-Names="Verdana" Font-Size="10pt" >
    </asp:Label>
    <br /><br />
    <div>
        <asp:GridView ID="grdDetails" runat="server" CellPadding="4" EnableModelValidation="True"
            Font-Names="Verdana" Font-Size="8pt" ForeColor="#333333" GridLines="None" 
            SelectedIndex="0" AutoGenerateColumns="False">
            <FooterStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <PagerStyle BackColor="#666666" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="CW ID">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CW_ID")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="First Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FIRST_NAME")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Last Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "LAST_NAME")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "SMTP_EMAIL")%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
