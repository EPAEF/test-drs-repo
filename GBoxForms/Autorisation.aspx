<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Autorisation.aspx.vb" Inherits="GBoxForms.Autorisation" MaintainScrollPositionOnPostback="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" >
    <title>G|Box COCKPIT</title>
    <%-- Projection --%>
    <link rel="Stylesheet" type="text/css" href="/_styles/beam.css" media="projection" />
    <%-- Print --%>
    <link rel="Stylesheet" type="text/css" href="/_styles/print.css" media="print" />
    <%-- All media solution --%>
    <link rel="Stylesheet" type="text/css" href="/_styles/stadard.css" media="screen" />
    <!--[if lte IE 6]>
    <link rel="Stylesheet" type="text/css" href="/_styles/ie6.css" media="screen" />
    <![endif]-->
    <!--[if IE 6]>
    <link rel="Stylesheet" type="text/css" href="/_styles/ie6.css" media="screen" />
    <![endif]-->
    <!--[if gte IE 7]>
    <link rel="Stylesheet" type="text/css" href="/_styles/ie8.css" media="screen" />
    <![endif]-->
    <!--[if gt IE 7]>
    <link rel="Stylesheet" type="text/css" href="/_styles/ie8.css" media="screen" />
    <![endif]-->
    <link rel="Stylesheet" type="text/css" href="/_styles/tcloud.css" media="screen" />
    <!-- Insert Link to favorites -->
    <script language="javascript" type="text/javascript" src="_js/favorites.js"></script>
   
    <script type="text/javascript">
        function changeMyUrl(url) {
            document.location.href = url;
        }
    </script>



    <script language="javascript" type="text/javascript">
        function postBackByObject(source) {
            //                var o = window.event.srcElement;
            //                if (o.tagName == "INPUT" && o.type == "checkbox")
            //                {
            __doPostBack(source, "");

            //                } 
        }
    </script>
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
                    <a  name="_Top"></a>
                    <asp:Label ID="lblInformations" runat="server" />
                </div>
            </div>
            <div id="toppart_logoleiste">
                <div id="toppart_logoleiste_left">
                    <asp:Image ID="imgGBoxLogo" runat="server" AlternateText="G|Box" 
                    ImageUrl="~/Images/gb_logo.jpg" />
                </div>
                <div id="toppart_logoleiste_center"></div>
                                       <div id="toppart_logoleiste_right">
                    <asp:Image ID="imgBBS_Logo" runat="server" 
                    AlternateText="Bayer" Height="44px" 
                    ImageUrl="~/Images/bayer_cross_rgb.png" Width="44px" />
                    <div class="toppart_support_navi">
                        <%--Search|Contact|Sitemap|Cockpit--%>
                    <asp:Menu ID="mnSupportNavi" runat="server" Orientation="Horizontal">
                    <Items>
                    <asp:MenuItem Text="Search" Value="Search" Target="_blank" 
                    NavigateUrl="http://coll-appl-bbs.bayer-ag.com/ibs/MDRS/Shared%20Documents/search.aspx"></asp:MenuItem>
                    <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                    <asp:MenuItem Text="Contact" Value="Contact" Target="_blank" 
                    NavigateUrl="http://coll-appl-bbs.bayer-ag.com/ibs/MDRS/Lists/Contact/AllItems.aspx"></asp:MenuItem>
                    <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                    <asp:MenuItem Text="Sitemap" Value="Sitemap" Target="_blank" 
                    NavigateUrl="http://coll-appl-bbs.bayer-ag.com/ibs/MDRS/Lists/Sitemap/AllItems.aspx"></asp:MenuItem>
                    <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                    <asp:MenuItem Text="Authorization management" Value="Authorization" Target="_blank" 
                    NavigateUrl="Autorisation.aspx"></asp:MenuItem>
                    <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                    <asp:MenuItem Text="Refresh menu"  > 
                    </asp:MenuItem>
                </Items>
                </asp:Menu>
                    </div>
                    </div>

            </div>
        </div>
        </div>
    <div id="navpart">
        <div id="navpart_navi">
                <asp:Menu ID="mnuNavigate" runat="server" 
                    DynamicHorizontalOffset="3" Font-Bold="False" Font-Names="Verdana" 
                    Font-Size="Small" ForeColor="White" Orientation="Horizontal" 
                    StaticSubMenuIndent="10px" style="margin-left: 0px" >
                    <StaticSelectedStyle BackColor="#FEAE17" />
                    <StaticMenuItemStyle HorizontalPadding="5px" />
                    <DynamicHoverStyle BackColor="#5498D4" ForeColor="White"  />
                    <DynamicMenuStyle BackColor="#FEAE17" />
                    <DynamicSelectedStyle BackColor="#FEAE17" />
                    <DynamicMenuItemStyle HorizontalPadding="5px" />
                    <Items>
                        <asp:MenuItem Text="Authorisation management" Value="Authorisation management">
                        </asp:MenuItem>
                    </Items>
                    <StaticHoverStyle ForeColor="White" />
                    <StaticItemTemplate>
                        <%# Eval("Text") %>
                    </StaticItemTemplate>
                </asp:Menu>
            </div>
        <div id="navpart_subnavi">
            <div> 
            <asp:Menu ID="mnuDetailsMenu" runat="server" 
            DynamicHorizontalOffset="3" Font-Bold="False" Font-Names="Verdana" 
            ForeColor="White" Orientation="Horizontal" 
            StaticSubMenuIndent="10px" style="margin-left: 0px">
            <StaticSelectedStyle BackColor="#FEAE17" />
            <StaticMenuItemStyle HorizontalPadding="5px" />
            <DynamicHoverStyle BackColor="#5498D4" ForeColor="White" />
            <DynamicMenuStyle BackColor="#FEAE17" />
            <DynamicSelectedStyle BackColor="#FEAE17" />
            <DynamicMenuItemStyle HorizontalPadding="5px" />
                <Items>
                    <asp:MenuItem Text="Authorisation" Value="Authorisation"></asp:MenuItem>
                </Items>
            <StaticHoverStyle BackColor="#FFCC00" ForeColor="White" />
            <StaticItemTemplate>
                <%# Eval("Text") %>
            </StaticItemTemplate>
            </asp:Menu>
            </div>
        </div>
    </div>
    <div id="corpus">
       <div id="corpus_desc">
            <asp:Label ID="lblTablenameData" runat="server" Visible="False" CssClass="corpus_info_label"></asp:Label>
            <asp:Label ID="lblIsTree" runat="server" CssClass="corpus_info_label"></asp:Label>
        <table rules="all" style="border-style: none; width:100%;"> 
                     <tr valign="top">
                    <td style="border-style: none; width: 50px;">
                        <asp:Image ID="imgBubble" runat="server" 
                            ImageUrl="Images/user_INFO.gif" 
                            BorderStyle="None" />
                        <asp:Label ID="lblInfo" runat="server" Font-Size="XX-Small"></asp:Label>
                    </td>
                <td valign="top" style="border-style: none; " class="style3">
                    <asp:Menu ID="mnuWizzard" runat="server"  
                            DynamicHorizontalOffset="3" Font-Bold="False" Font-Names="Verdana" 
                            ForeColor="Black" Orientation="Horizontal" 
                            StaticSubMenuIndent="10px" Font-Size="Small" >
                        <StaticSelectedStyle BackColor="#FEAE17" />
                       
                        <StaticMenuItemStyle HorizontalPadding="5px" BackColor="ButtonFace"/>
                        <DynamicHoverStyle BackColor="#FEAE17" ForeColor="Black" />
                        <DynamicMenuStyle BackColor="#FEAE17" />
                        <DynamicSelectedStyle BackColor="#FEAE17" ForeColor="Black" />
                        <DynamicMenuItemStyle HorizontalPadding="5px"  BackColor="ButtonFace"/>
                        <StaticHoverStyle BackColor="#FFCC00" ForeColor="Black" />
                        <StaticItemTemplate>
                            <%# Eval("Text") %>
                        </StaticItemTemplate>
                    </asp:Menu>
                    <!-- Horizontal line inside Wizard 
                    <hr style=" width:100%; height:1px; color:#999999; margin: 0px 0px 0px 0px; padding: 0px 0px 0px 0px;" />
                    -->
                    <asp:MultiView ID="mvWizard" runat="server">
                        <asp:View ID="View1" runat="server">
                            <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Font-Size="Small" Text="Label"></asp:Label>
                        </asp:View>
                        <asp:View ID="View2" runat="server">
                            <asp:Label ID="Label2" runat="server" Font-Names="Verdana" Font-Size="Small" Text="Label"></asp:Label>
                       </asp:View>
                        <asp:View ID="View3" runat="server">
                            <asp:Label ID="Label3" runat="server" Font-Names="Verdana" Font-Size="Small" Text="Label"></asp:Label>
                        </asp:View>
                        <asp:View ID="View4" runat="server">
                            <asp:Label ID="Label4" runat="server" Font-Names="Verdana" Font-Size="Small" Text="Label"></asp:Label>
                        </asp:View>
                        <asp:View ID="View5" runat="server">
                            <asp:Label ID="Label5" runat="server" Font-Names="Verdana" Font-Size="Small" Text="Label"></asp:Label>                 
                      </asp:View>
                      <asp:View ID="View6" runat="server">
                            <asp:Label ID="Label6" runat="server" Font-Names="Verdana" Font-Size="Small" Text="Label"></asp:Label>
                        </asp:View>
                        <asp:View ID="View7" runat="server">
                            <asp:Label ID="Label7" runat="server" Font-Names="Verdana" Font-Size="Small" Text="Label"></asp:Label>
                        </asp:View>
                        <asp:View ID="View8" runat="server">
                            <asp:Label ID="Label8" runat="server" Font-Names="Verdana" Font-Size="Small" Text="Label"></asp:Label>
                        </asp:View>
                        <asp:View ID="View9" runat="server">
                            <asp:Label ID="Label9" runat="server" Font-Names="Verdana" Font-Size="Small" Text="Label"></asp:Label>
                        </asp:View>
                        <asp:View ID="View10" runat="server">
                            <asp:Label ID="Label10" runat="server" Font-Names="Verdana" Font-Size="Small" 
                                Text="Label"></asp:Label>
                        </asp:View>
                        <asp:View ID="View11" runat="server">
                            <asp:Label ID="Label11" runat="server" Font-Names="Verdana" Font-Size="Small" 
                                Text="Label"></asp:Label>
                        </asp:View>
                        <asp:View ID="View12" runat="server">
                            <asp:Label ID="Label12" runat="server" Font-Names="Verdana" Font-Size="Small" 
                                Text="Label"></asp:Label>
                        </asp:View>
                    </asp:MultiView>
                </td>
              </tr>
        </table>
       </div> 
       
    <asp:MultiView ID="mvGBoxAuthorizationOverview" runat="server" ActiveViewIndex="0">
         <asp:View ID="vwGBoxAuthorizationDetailsOverview" runat="server">
 <table rules="all" width="100%"  border="0" cellspacing="0" cellpadding="0" summary="Das ist ein Tabellenkommentar">
    <tr>
    <td colspan="2">
        <table width="800">
            <tr>
                <td>
                <asp:CheckBox ID="chkDisplayOnly" runat="server" 
                        Text="Display only" AutoPostBack="true" Checked="false" OnCheckedChanged="chkDisplayOnly_CheckedChanged" />
                <asp:ImageButton ID="imgRefresh" runat="server" ImageUrl="~/Images/arrow_refresh.gif" 
                        CssClass="et_multiview_naviImgBut" Height="32" Width="32"
                        ToolTip="Refresh"/>
                <asp:ImageButton ID="imgUserAdd" runat="server" 
                        ImageUrl="~/Images/user_add_grey.gif" 
                        CssClass="et_multiview_naviImgBut" Height="32" Width="32" Enabled="False"
                        ToolTip="Add user"/>
                <asp:ImageButton ID="imgUserDelete" runat="server" 
                        ImageUrl="~/Images/user_delete_grey.gif" 
                        CssClass="et_multiview_naviImgBut" Height="32" Width="32" Enabled="False"
                        ToolTip="Delete user"/>
                <asp:ImageButton ID="imgMailingList" runat="server" ImageUrl="~/Images/book_addresses.gif" 
                        CssClass="et_multiview_naviImgBut" Height="32" Width="32"
                        ToolTip="Mailing list" Enabled="False"/>  
                
                    <asp:ImageButton ID="imgHelp0" runat="server" 
                        CssClass="et_multiview_naviImgBut" 
                        Enabled="true" Height="32" ImageUrl="~/Images/help.gif" 
                        ToolTip="Help(new window)" Width="32" />
                
                    <asp:CheckBox ID="chkDisplayAdminRoles" runat="server" 
                        Text="Display Administrative Roles" AutoPostBack="true" Checked="false" OnCheckedChanged="chkDisplayAdminRoles_CheckedChanged" />
                
                </td>
            </tr>
            <tr><td>
            <p>
                        <asp:TextBox ID="txtErrorMsg" runat="server" Height="347px" ReadOnly="True" 
                            TextMode="MultiLine" Width="100%" BorderStyle="None"></asp:TextBox>
               </p>     
            
            </td></tr>
        </table>
    </td>
    </tr>
    <tr>
        <td style="width:25%;"><asp:Image ID="Image4" runat="server" Height="5px" 
            ImageUrl="~/Images/_spacer.gif" Width="200px" />
        </td>
        <td style="width:75%;"><asp:Image ID="Image5" runat="server" Height="5px" 
            ImageUrl="~/Images/_spacer.gif" Width="600px" />
        </td>
    </tr>
    <tr>
    <td valign="top">
    <asp:Label ID="lblStatus" runat="server" ForeColor="Black" Text="" 
                        CssClass="corpus_info_label" Font-Bold="True"></asp:Label>  
        <asp:TreeView ID="trvvwGBoxAuthorizationDetailsOverview" runat="server" 
            AutoGenerateDataBindings="False" EnableClientScript="False" 
            PopulateNodesFromClient="False" ShowLines="True" Width="100%" 
            BorderWidth="0px">
        <SelectedNodeStyle BackColor="#FEAE17" ForeColor="White" />
        <ParentNodeStyle Font-Bold="False" ImageUrl="~/Images/part-16x16.gif" />
        <RootNodeStyle ImageUrl="~/Images/database-16x16.gif" />
        <NodeStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="Black" 
            HorizontalPadding="2px" NodeSpacing="0px" VerticalPadding="2px" />
        </asp:TreeView>
    </td>
    <td valign="top">
        <table style="width:100%;">
            <tr>
                <td>
                <asp:Label ID="lblCountData" runat="server" CssClass="corpus_info_label"></asp:Label>
                    <asp:GridView ID="grdvwGBoxAuthorizationDetailsOverview" runat="server" 
                    CellPadding="4" Font-Names="Verdana"  Font-Size="8pt" ForeColor="#333333" 
                    GridLines="None">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <Columns>
                        <asp:TemplateField HeaderImageUrl="~/Images/user.gif">
                        <EditItemTemplate>
                        <asp:CheckBox ID="CheckBox1" runat="server"/>
                        </EditItemTemplate>
                        <ItemTemplate>
                        <asp:CheckBox ID="CheckBox1" runat="server"/>
                        </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    <HeaderStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#999999" />
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                </asp:GridView>
                </td>
            </tr>
        </table>
    </td>
    </tr>
</table>
        </asp:View> 
                <asp:View ID="vwGBoxUserAccess" runat="server">
                        <table rules="all" style="width:100%;">
                        <tr>
						    <td>
						        <asp:ImageButton ID="imgCancel_Role" runat="server" 
                                    CssClass="et_multiview_naviImgBut" Enabled="False" Height="32" 
                                    ImageUrl="~/Images/cancel_grey.gif" Width="32"
                                    ToolTip="Cancel action" />
                                <asp:ImageButton ID="imgShowRequest" runat="server" 
                                    ImageUrl="~/Images/page_go_grey.gif" Enabled="False"
                                    ToolTip="Submitt" />
						    </td>
					    </tr>
					    <tr>
					        <td>
				                <p style="font-size:12pt; font-weight:bold; color:#000000; padding: 5px 15px 15px 15px;">
                                    <asp:Label ID="lblRole" runat="server" Text="Request the" Font-Size="Small"></asp:Label>
                       
                                </p>
                                 <p style="font-size:12pt; font-weight:bold; color:#FF0000; padding: 5px 15px 15px 15px;">
                                 <asp:Label ID="lblErr" runat="server"></asp:Label>
                                 </p>
                    <p />
					        </td>
					    </tr>
					    <tr>
						    <td  bgcolor="#E9ECF1" style="border:solid 2px #e9ecf1; font-size: x-small;">
                                <div class="mc-dfrm-CL">
                                    <asp:Label ID="lblChooseSubgroup" runat="server" Text="Subgroup" 
                                            Width="133px" CssClass="mc-dfrm-Label"></asp:Label>
                                    <asp:DropDownList ID="cmbAuthSetSubgroup" runat="server" 
                                            AutoPostBack="True"></asp:DropDownList>
                                    <br />
                                   
                                     <asp:Label ID="lblOrglevelID" runat="server" 
                                            Text="Section type" Width="133px" CssClass="mc-dfrm-Label"></asp:Label>
                                    <asp:DropDownList ID="cmbOrglevelID" runat="server" 
                                            AutoPostBack="True"></asp:DropDownList>
                                          <br />
                                    <asp:Label ID="lblOrglevelValue" runat="server" Text="Section value" 
                                            Width="133px" CssClass="mc-dfrm-Label"></asp:Label>
                                    <asp:DropDownList ID="cmbOrglevelValue" runat="server" AutoPostBack="True"></asp:DropDownList>
                                </div>
                                1.	Pick subgroup the authorization is needed for.<br />
                                2.	Define the level on which the authorization is needed for.<br />
                                Possible entries: <br />
                                SUBGROUP, when an overall role is needed.<br />
                                DIVISION, when a role is requested for a division only.<br />
                                Examples:<br />
                                -	MARTY access for BHC: enter Subgroup=BHC, Section type=SUBGROUP, Section value=BHC<br />
                                -	DRS approver role for BBS F&A is needed: enter Subgroup=BBS, Section type=DIVISION, Section value=F&A<br />


						    </td>
                            
					    </tr>
					    <tr>
						    <td style="border:solid 2px #e9ecf1;">
						    
						        <asp:Label ID="lblUser" runat="server" 
                                    
                                    Text="Choose the users (use CWID) to request access for. The users can be listed with &quot;,&quot; or &quot;;&quot;. Please note that no spaces are used." 
                                    CssClass="mc-dfrm-Label" Width="500px"></asp:Label>
						        <br />
						        <asp:TextBox ID="txtUserToAdd" runat="server" Width="300px" 
                                    CssClass="mc-dfrm-TB"></asp:TextBox>
						        &nbsp;<asp:ImageButton ID="imgUserAdd_Role" runat="server" 
                                        CssClass="et_multiview_naviImgBut" Height="32" 
                                        ImageUrl="~/Images/user_add.gif" Width="32" 
                                        ToolTip="Add user to request list"/>
                                <br />
                                <asp:Label ID="lblRequestList" runat="server" 
                                        Text="CWID's to request:"
                                        CssClass="mc-dfrm-Label"></asp:Label>
                                <br />
                                <asp:ListBox ID="lstUsersToCheck" runat="server" CssClass="mc-dfrm-LB" Rows="5" 
                                    Width="300px"></asp:ListBox>
                                &nbsp;<asp:ImageButton ID="imgUserDelete_Role" runat="server" 
                                    CssClass="et_multiview_naviImgBut" Height="32" 
                                    ImageUrl="~/Images/user_delete.gif" ToolTip="remove user from Request list" 
                                    Width="32" />
                                <br />
                                <br />
                                <asp:Label ID="lblComment" runat="server" 
                                    Text="Provide well-grounded reason why role is needed (not just: &quot;needed for daily work&quot;)." 
                                    CssClass="mc-dfrm-Label" Width="500px"></asp:Label>
					            <br />
					            <asp:TextBox ID="txtComment" runat="server" CssClass="mc-dfrm-TB" Rows="10" 
                                    TextMode="MultiLine" Width="300px"></asp:TextBox>
					            <br />
                                <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                                <br />
                                <br />
                            </td>
					    </tr>
				       
                        </table>
                </asp:View>
                <asp:View ID="vwRequest" runat="server">
                    <asp:ImageButton ID="imgCancelRequest" runat="server" 
                        ImageUrl="~/Images/cancel.gif" CssClass="et_multiview_naviImgBut"  
                        ToolTip="Cancel" />
                    <asp:ImageButton ID="imgSubmitRequest" runat="server" 
                        ImageUrl="~/Images/page_go.gif" CssClass="et_multiview_naviImgBut"  
                        ToolTip="Submit" />
                    <asp:Label ID="lblInsertMode" runat="server" Visible="False"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtRequest" runat="server" Height="568px" TextMode="MultiLine" 
                        Width="787px" ToolTip="Remove selected"></asp:TextBox>
                </asp:View>
               
             </asp:MultiView>
        </div>
</form>
</body>
</html>
