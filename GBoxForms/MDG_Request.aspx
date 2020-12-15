<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="MDG_Request.aspx.vb" Inherits="GBoxForms.MDG_Request" MaintainScrollPositionOnPostback="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server" >
    <title>G|Box MDG Request</title>
    <%-- Projection --%>
    <link rel="Stylesheet" type="text/css" href="/_styles/beam.css" media="projection" />
    <%-- Print --%>
    <link rel="Stylesheet" type="text/css" href="/_styles/print.css" media="print" />
    <%-- All media solution --%>
    <link rel="Stylesheet" type="text/css" href="/_styles/stadard.css" media="screen" />
     <link rel="Stylesheet" type="text/css" href="/_styles/tcloud.css" media="screen" />
    <style type="text/css">
        .style8
        {
            width: 6%;
        }
        .style9
        {
            width: 542px;
        }
        </style> 
    
</head>
<body style="margin: 0px 0px 0px 0px; padding: 0px 0px 0px 0px;">
<form id="myForm" method="post" runat="server">
 
<div id="mc_wiz_df">
    <div id="toppart">
        <div id="toppart_topleiste">
            <div id="toppart_topleiste_left">
                <asp:HyperLink ID="hlHometop" runat="server" NavigateUrl="~/index.aspx" Text="Home" 
                ForeColor="White" Target="_self" />
                <asp:Label ID="lblDatabase" runat="server" />
            </div>
            <div id="toppart_topleiste_right">
                <a id="_Top"></a>
                <asp:Label ID="lblInformations" runat="server" />
                
            </div>
        </div>
        <div id="toppart_logoleiste">
            <div id="toppart_logoleiste_left">
                <asp:Image ID="imgGBoxLogo" runat="server" AlternateText="G|Box" 
                ImageUrl="~/Images/gb_logo.jpg" />
            </div>
            <div id="toppart_logoleiste_center">
            &nbsp;
            </div>
             <div id="toppart_topleiste_right" >
                    <asp:ImageButton ID="imghelp_Logo" runat="server" 
                        CssClass="et_multiview_naviImgBut" 
                        Enabled="true" Height="50px" ImageUrl="~/Images/MDAS.gif" 
                        ToolTip="Help(new window)" Width="150px" />
                   
            </div>
        </div>
    </div>
</div>

<div id="corpus" >
       <ul style="font-family: Verdana; font-size: small; width:100%;">
           <li>1. Step : Choose which Master Data is to be changed: supplier (CR, vendor), customer (DE) or material (MM).</li>
           <li>2. Step : Choose your role in MDG: request changes or approve changes.</li>
           <li>3. Step : Choose organisation level(s) for which you need the role(s).</li>
           <li>4. Step : Show the roles and pick the role(s) you need. If any role is missing, create a ticket for application GBOX for MDAS hotline (<a href="http://by-gbox.bayer-ag.com/hotline" target ="_blank">LINK</a>).</li>
           <li>5. Step : Add the relevant role(s) to your Shopping Cart.</li>
           <li>6. Step : Add users and submit your request.</li>
        </ul> 
       <div id="corpus_center">                   
           <table rules="all" id="MyTable" bgcolor="Silver" style="font-family: Verdana; font-size: small; width:100%;">  
              <tr valign="top">
                <td valign="top" id="Td7" style="border: 0px;" class="style8" colspan ="4">
                    <table rules="all" style="width:10%;" id="MySTable" >
                    <tr valign="top">
                        <td valign="top" id="Tds1" style="border: 0px;" class="style8">
                        <asp:ImageButton ID="ImgShowRoles" runat="server" 
                        ImageUrl="~/Images/report_go_2.gif" CssClass="et_multiview_naviImgBut"  
                        ToolTip="Show Roles" />
                        </td>
                        <td valign="top" id="Tds2" style="border: 0px;" class="style8">
                        <asp:ImageButton ID="imgSubmitRequest" runat="server" 
                        ImageUrl="~/Images/page_go.gif" CssClass="et_multiview_naviImgBut"  
                        ToolTip="Submit" />
                        </td>
                    </tr>
                    <tr valign="top">
                        <td valign="top" id="Td1" style="border: 0px;" class="style8">
                        <asp:Label ID="Label5" runat="server" Text="Show Roles" Width="100px"  Font-Size="Smaller"></asp:Label>
                        </td>
                        <td valign="top" id="Td2" style="border: 0px;" class="style8">
                        <asp:Label ID="Label6" runat="server" Text="Submit"  Width="100px" Font-Size="Smaller"></asp:Label>
                        </td>
                    </tr>
                    </table>
                </td>
              </tr>
              <tr valign="top">
                <td valign="top" id="myFirstColumn" style="border: 0px;" class="style8" colspan="4" >
                    <table style="font-family: Verdana; font-size: small; width:70%;">
                        <tr>
                            <td><asp:Label ID="Label1" runat="server" Text="Step 1:" Font-Size="Smaller"></asp:Label></td>
                            <td><asp:Label ID="Label2" runat="server" Text="Step 2:" Font-Size="Smaller"></asp:Label></td>
                            <td><asp:Label ID="Label3" runat="server" Text="Step 3:" Font-Size="Smaller"></asp:Label></td>
                        </tr>
                        <tr>
                            <td style="border: 5px;width:30%" valign="top">
                                <asp:CheckBoxList ID="chkArea" runat="server" Font-Size="Smaller">
                                    <asp:ListItem Value="CR">Vendor Master Data</asp:ListItem>
                                    <asp:ListItem Value="DE">Customer Master Data</asp:ListItem>
                                    <asp:ListItem Value="MM">Material Master Data</asp:ListItem>
                                </asp:CheckBoxList>
                            </td>
                            <td style="border: 5px;width:30%" valign="top">
                                <asp:CheckBoxList ID="ChkApprover" runat="server" Font-Size="Smaller">
                                    <asp:ListItem>Approver</asp:ListItem>
                                    <asp:ListItem>Requester</asp:ListItem>
                                </asp:CheckBoxList>    
                            </td>
                            <td style="border: 5px;width:100%" valign="top">
                            <table id="tblTextbox" style="font-family: Verdana; font-size: small; width:100%;">
                                <tr>
                                    <td valign="top">
                                        <asp:RadioButtonList ID="RdbRequester" runat="server" Font-Size="Smaller" >
                                            <asp:ListItem Value="LE">Company Code</asp:ListItem>
                                            <asp:ListItem Value="BEGRU">Authorization Group</asp:ListItem>
                                             <asp:ListItem Value="HUB">Hub Authorization Group</asp:ListItem>  
                                            <asp:ListItem Value="EKORG">Purchase Organization</asp:ListItem>
                                            <asp:ListItem Value="VKORG">Sales Organization</asp:ListItem>
                                            <asp:ListItem Value="WERKS">Plant</asp:ListItem> 
                                        </asp:RadioButtonList>
                                        
                                    </td>
                                <td>
                                    <asp:textbox ID="txtboxLE" runat="server" ></asp:textbox> <br />
                                      <asp:textbox ID="txtBegru" runat="server"></asp:textbox> <br />
                                      <asp:textbox ID="txtHub" runat="server"></asp:textbox><br />
                                    <asp:textbox ID="txtEKORG" runat="server"></asp:textbox><br />
                                    <asp:textbox ID="txtVKORG" runat="server"></asp:textbox><br />
                                    <asp:textbox ID="txtWERKS" runat="server"></asp:textbox>
                                </td>
                             </tr>
                            
                             </table>
                            </td>
                        </tr>
                    </table> 
                    
                </td>
                              
              </tr>
              <tr>
              <td class="style8" colspan ="4">
                  <asp:Label ID="lblError" runat="server" ForeColor="Red" 
                      CssClass="corpus_info_label" Font-Bold="True" Font-Size="Large"></asp:Label>
              </td>
              </tr>

            </table>
            <asp:Panel ID="pnlView" Visible ="false" runat="server">
            <table rules="all" id="Table1" style="font-family: Verdana; font-size: small; width:100%;"> 
              <tr valign="top">
                <td valign="top" id="Td5" style="border: 0px;width:50%" class="style8">
                    <asp:Label ID="lblSearchResult" runat="server" Text="Search result:" style="font-size:10pt; font-weight:bold; color:#000000;"></asp:Label> 
                    <br />
                    <asp:ImageButton ID="imgAddRole" runat="server" 
                                        CssClass="et_multiview_naviImgBut" Height="32" 
                                        ImageUrl="~/Images/user_add.gif" Width="32" 
                                        ToolTip="Add role to shopping cart"/>
                            
                </td>
                <td valign="top" id="Td6" style="border-style: none; text-align:left; margin-bottom: 10px;">
                     <asp:Label ID="Label4" runat="server" Text="Shopping Cart:" 
                         style="font-size:10pt; font-weight:bold; color:#000000;"></asp:Label>
                     <br />
                     <asp:ImageButton ID="imgRemoveRole" runat="server" 
                                    CssClass="et_multiview_naviImgBut" Height="32" 
                                    ImageUrl="~/Images/user_delete.gif" ToolTip="Remove role from shopping cart" 
                                    Width="32" />
                </td>
               </tr> 
              <tr valign="top">
                <td valign="top" id="Td4" style="border: 5px;width:50%" class="style8">
                    <table rules="all" id="Table2" style="font-family: Verdana; font-size: small; width:100%;">
                        <tr>
                            <td >
                            <asp:CheckBoxList ID="chkBoxAgr" runat="server" 
                                 DataTextField="AGR_DEFINE_VIEW" Width="100%" Font-Size="Smaller">
                            </asp:CheckBoxList>
                            </td>
                           
                            <td align ="center" >
                            <br />
                            <br />
                            </td>
                        </tr>
                    </table>
                    
                </td>
                <td>
                <asp:CheckBoxList ID="chkShoppingCart" runat="server" Font-Size="Smaller">
                            </asp:CheckBoxList>
                </td>
                </tr>
               <tr>
                    <td colspan="2" >
                    <table rules="all" id="Table3" bgcolor="Silver" style="font-family: Verdana; font-size: small; width:100%;"  
                            cellpadding="5" class="tc_blue" frame="box">
                                    <tr>
                                        <td class="style9">
                                        <asp:Label ID="lblUser" runat="server"                                    
                                    Text="Choose the users (use CWID) to request access for. The users can be listed with &quot;,&quot; or &quot;;&quot;. Please note that no spaces are used." 
                                    CssClass="mc-dfrm-Label" Width="500px"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                       <td class="style9">
                                       <asp:TextBox ID="txtUserToAdd" runat="server" Width="300px" 
                                    CssClass="mc-dfrm-TB"></asp:TextBox>
						        <asp:ImageButton ID="imgUserAdd_Role" runat="server" 
                                        CssClass="et_multiview_naviImgBut" Height="32" 
                                        ImageUrl="~/Images/user_add.gif" Width="32" 
                                        ToolTip="Add user to request list"/>
                                       </td>
                                       <td>
                                            <asp:Label ID="lblRequestComment" runat="server" Text="Explain why these roles are needed:" Font-Size="Small" CssClass="mc-dfrm-Label"></asp:Label><br />
                                            <asp:TextBox ID="txtRequestComment" runat="server" Width="300px" CssClass="mc-dfrm-TB"></asp:TextBox>
                                       </td>
                                    </tr>
                                    <tr>
                                    <td class="style9">
                                    <asp:Label ID="lblRequestList" runat="server" 
                                        Text="CWID's to request:"
                                        CssClass="mc-dfrm-Label"></asp:Label> <br />
                                <asp:ListBox ID="lstUsersToCheck" runat="server" 
                        CssClass="mc-dfrm-LB" Rows="5" 
                                    Width="300px"></asp:ListBox>
                                <asp:ImageButton ID="imgUserDelete_Role" runat="server" 
                                    CssClass="et_multiview_naviImgBut" Height="32" 
                                    ImageUrl="~/Images/user_delete.gif" ToolTip="remove user from Request list" 
                                    Width="32" />
                                    </td>
                                    </tr>
                                </table>
                    </td>
               </tr>
            </table>
            </asp:Panel>
        </div>
</div>
</form>
</body>

</html>
