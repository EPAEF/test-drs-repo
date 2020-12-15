<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AutoClass.aspx.vb" Inherits="GBoxForms.AutoClass"   MaintainScrollPositionOnPostback="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Request autoclassification or filter settings </title>
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

    <!-- Insert link to favorites -->
    <script language="javascript" type="text/javascript" src="_js/favorites.js"></script>
    <style type="text/css">
        .style1
        {
            width: 26%;
        }
        .style2
        {
            width: 28%;
        }
        .style3
        {
            width: 28%;
            height: 75px;
        }
        .style4
        {
            height: 75px;
        }
        .style5
        {
            width: 925px;
        }
        .style6
        {
            width: 64px;
        }
    </style>
</head>
<body>
<form id="form1" runat="server">

<div id="mc_wiz_df">
    
    <div id="toppart">
        <div id="toppart_topleiste">
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="text-align:left; width:50%; padding: 0px 0px 0px 4px; ">
                    <asp:HyperLink ID="hlHometop" runat="server" NavigateUrl="~/index.aspx" Text="Home" 
                            ForeColor="White" Target="_self" />
                    <asp:Label ID="lblDatabase" runat="server" />
                    </td>
                    <td style="text-align:right; padding: 0px 10px 0px 0px;">
                    <a name="_Top"></a>
                    <asp:Label ID="lblInformations" runat="server" />
                    </td>
                </tr>
            </table>
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
                                NavigateUrl="http://by-bdc.bayer-ag.com/ibs/MDRS/Shared%20Documents/search.aspx"></asp:MenuItem>
                            <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                            <asp:MenuItem Text="Contact" Value="Contact" Target="_blank" 
                                NavigateUrl="http://by-bdc.bayer-ag.com/ibs/MDRS/Lists/Contact/AllItems.aspx"></asp:MenuItem>
                            <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                            <asp:MenuItem Text="Sitemap" Value="Sitemap" Target="_blank" 
                                NavigateUrl="http://by-bdc.bayer-ag.com/ibs/MDRS/Lists/Sitemap/AllItems.aspx"></asp:MenuItem>
                        </Items>
                    </asp:Menu>
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
                <StaticHoverStyle ForeColor="White" />
                <StaticItemTemplate>
                    <%# Eval("Text") %>
                </StaticItemTemplate>
            </asp:Menu>
        </div>
    </div>
    <div id="navpart_subnavi">
        <div>&nbsp;</div>
    </div>
    <div id="corpus">
        <div id="corpus_center">
        <div>
            <asp:MultiView ID="mvAutoclass" runat="server" 
            ActiveViewIndex="0">
            <asp:View ID="vwAutoclassOverview" runat="server">
                <table width="100%" >
                <tr    style="height: 32px">
                <td class="style5">
                <asp:Label ID="lblSubgroup" runat="server" 
                        Text="Select subgroup:"></asp:Label>
                <asp:DropDownList ID="cmbSubgroup" runat="server" AutoPostBack="True" Height="24px">
                </asp:DropDownList>
                    <asp:Label ID="lblNoStrands" runat="server" Visible="False"></asp:Label>
                </td>
                <td align="center" class="style6">
                </td>
                <td align="right">
                    <asp:ImageButton ID="imgSearchEngine" runat="server" 
                        CssClass="et_multiview_naviImgBut" Height="32px" 
                        ImageUrl="~/Images/page_find.gif" TabIndex="10" ToolTip="Search for Values/Filter" 
                        Width="34px" />
                    <asp:HyperLink ID="HyperLink1" runat="server" ImageUrl="~/Images/help.gif" 
                        NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/request/REQUEST_wiki/Filter%20Settings%20and%20Autoclassification.aspx" 
                        Target="_ " ToolTip="SHOW WIKI">HyperLink</asp:HyperLink> 
                </td>
                </tr>
                </table>
                             
                <table width="100%" align="left">
                    <tr bgcolor="Silver" style="font-family: Verdana; font-size: small;">
                    <td class="style3"  > 
                        <asp:RadioButtonList ID="optList" runat="server" AutoPostBack="True" 
                            ToolTip="Show selected Type" Height="69px">
                            <asp:ListItem Selected="True">Filtersetting</asp:ListItem>
                            <asp:ListItem>Autoclassification</asp:ListItem>
                            <asp:ListItem>Both</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td align="left" width = "25%" class="style4">
                        &nbsp;<asp:Label ID="lblVal" runat="server" Text="Filter current values:"></asp:Label><br />
                        <asp:TextBox ID="txtCurSearch" runat="server" AutoPostBack="True" Width="118px"></asp:TextBox>
                            <asp:ImageButton ID="imgcurAppendFilter" runat="server" BackColor="Silver" 
                                ImageUrl="~/Images/page_find.gif" ToolTip="Filter value pool" 
                            ImageAlign="Baseline" />
                            <asp:ImageButton ID="imgcurInvertSelection" runat="server" 
                            BackColor="Silver" BorderColor="Silver" EnableTheming="False" ForeColor="White" 
                            ImageAlign="Baseline" ImageUrl="~/Images/page_gear.gif" SkinID="0" 
                            ToolTip="Invert selection" />
                            <asp:ImageButton ID="imgSaveAsExcel" runat="server" Height="32px" 
                            Width="32px" Enabled="False" 
                                ImageUrl="~/Images/download_gray.png" 
                            ToolTip="Download Current Values to Excel"/>
                    </td>
                    <td  width = "25%" class="style4">
                          &nbsp;<asp:Label ID="Label1" runat="server" Text="Filter value pool:"></asp:Label><br />
                         <asp:TextBox ID="txtSearch" runat="server" AutoPostBack="True" Width="116px"></asp:TextBox>
                            <asp:ImageButton ID="imgAppendFilter" runat="server" BackColor="Silver" 
                                ImageUrl="~/Images/page_find.gif" ToolTip="Filter value pool" 
                              BorderColor="Silver" ImageAlign="Baseline" />
                            <asp:ImageButton ID="imgInvertSelection" runat="server" BackColor="Silver" 
                                BorderColor="Silver" EnableTheming="False" ForeColor="White" 
                                ImageUrl="~/Images/page_gear.gif" SkinID="0" 
                              ToolTip="Invert selection" ImageAlign="Baseline" />
                          <asp:Label ID="lblGboxId" runat="server" Visible="False"></asp:Label>
                    </td>
                    <td align="char" bgcolor="Silver" width = "25%" class="style4">
                        Actionbuttons:&nbsp;
                        <asp:ImageButton ID="imgAdd" runat="server" BackColor="Silver" 
                            ImageUrl="~/Images/page_add.gif" ToolTip="Add selected values" 
                            BorderColor="Silver" ImageAlign="Left" />
                        <asp:ImageButton ID="imgDelete" runat="server" BackColor="Silver" 
                            BorderColor="Silver" EnableTheming="False" ForeColor="White" 
                            ImageUrl="~/Images/page_delete.gif" SkinID="0" 
                            ToolTip="Delete selected values" ImageAlign="Left" />
                        <asp:ImageButton ID="imgUndo" runat="server" BackColor="Silver" 
                            BorderColor="Silver" EnableTheming="False" ForeColor="White" 
                            ImageUrl="~/Images/page_white_get.gif" SkinID="0" 
                            ToolTip="Remove selected values from request" ImageAlign="Left" />
                        <asp:ImageButton ID="imgReset" runat="server" BackColor="Silver" 
                            BorderColor="Silver" EnableTheming="False" ForeColor="White" 
                            ImageUrl="~/Images/cancel.gif" SkinID="0" ToolTip="Reset request" 
                            ImageAlign="Left" />
                        <asp:ImageButton ID="imgShowRequest" runat="server" BackColor="Silver" 
                            BorderColor="Silver" EnableTheming="False" ForeColor="White" 
                            ImageUrl="~/Images/page_go.gif" SkinID="0" 
                            ToolTip="Show and submit request" ImageAlign="Left" />
                        
                     </td>  
                     
                    </tr>
                    <tr bgcolor="Silver" style="font-family: Verdana; font-size: small;">  
                    <td align="left" class="style2" >  &nbsp;Strand &amp; systems
                      &nbsp;&nbsp;&nbsp; <asp:Button 
                            ID="cmdExpandAll" runat="server" Text="expand tree" 
                            ToolTip="expand tree and use F3 for searching" /> 
                    </td>

                     
                        <td class="style1"  > &nbsp;Current 
                        values: 
                     </td>
                    <td align="left" width = "25%"> 
                            &nbsp;Value pool: 
                    </td>
                    <td align="left" width = "25%"> &nbsp;Requested values:
                        <asp:Label ID="lblLimit" runat="server" Visible="False"></asp:Label> 
                    </td> 
                    </tr>
                    <tr valign="top" style="border-style: none; float: none; font-family: Tahoma; font-size: 10px; font-weight: normal;">                            
                    <td  class="style2">
                             <asp:TreeView ID="trvStrands" 
                     runat="server" 
                    AutoGenerateDataBindings="False" PopulateNodesFromClient="False" 
                            EnableClientScript="False" Width="209px" 
                     BorderColor="#5D7B9D" ImageSet="XPFileExplorer" NodeIndent="15" 
                            Font-Names="Verdana">
            	    <HoverNodeStyle Font-Underline="True" ForeColor="#6666AA" />
            	    <SelectedNodeStyle Font-Underline="False" HorizontalPadding="0px" 
                                VerticalPadding="0px" BackColor="#B5B5B5" />
            	    <ParentNodeStyle Font-Bold="False" ImageUrl="Images/server_s.gif"  />
                    <RootNodeStyle ImageUrl="Images/server_key_s.gif"  />
                    <NodeStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="Black" 
                                HorizontalPadding="2px" NodeSpacing="0px" VerticalPadding="2px" 
                                 ImageUrl="Images/transmit_s.gif" />
                    <LeafNodeStyle ImageUrl="Images/table_s.gif"  />

                </asp:TreeView>
                    </td>
                    <td     class="style1">
                             <asp:CheckBoxList ID="chkCurrentValues" runat="server"  
                            Font-Names="Verdana" Font-Size="X-Small" Width="100%">
                        </asp:CheckBoxList>
                    </td>
                    <td   class="style1">
                            <asp:CheckBoxList ID="chkPossibleValues" runat="server" Font-Names="Verdana" 
                            Font-Size="X-Small" BorderColor="Silver" Width="100%">
                        </asp:CheckBoxList>
                    </td>
                    <td  class="style1">
                        <asp:TreeView ID="trvRequest" runat="server" AutoGenerateDataBindings="False" 
                            BackColor="White" EnableClientScript="False" PopulateNodesFromClient="False" 
                            ShowLines="True" Width="223px">
                            <HoverNodeStyle Font-Underline="True" ForeColor="#6666AA" />
            	            <SelectedNodeStyle Font-Underline="False" HorizontalPadding="0px" 
                                VerticalPadding="0px" BackColor="#B5B5B5" />
            	            <ParentNodeStyle Font-Bold="False" ImageUrl="Images/server_s.gif" BackColor="White" />
                            <RootNodeStyle ImageUrl="Images/server_key_s.gif" BackColor="White" />
                            <NodeStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="Black" 
                                HorizontalPadding="2px" NodeSpacing="0px" VerticalPadding="2px" 
                                BackColor="White" ImageUrl="Images/transmit_s.gif" />
                             <LeafNodeStyle ImageUrl="Images/table_s.gif" BackColor="White" />
                        </asp:TreeView>
                    </td>
                    </tr>
                   </table>

            </asp:View>
            <asp:View ID="vwRequest" runat="server">
                  <asp:Button ID="cmdBack" runat="server" Text="Back" />
                  <asp:Button ID="cmdsubmit" runat="server" Text="submit" Width="56px" 
                        Enabled="False" />
                    <br />
                    <asp:TextBox ID="txtRequest" runat="server" Height="568px" TextMode="MultiLine" 
                        Width="787px" ToolTip="Remove selected"></asp:TextBox>
                </asp:View>
            <asp:View ID="AccessDenied" runat="server">
                <asp:HyperLink ID="hplHome" runat="server" 
            NavigateUrl="http://by-dbox.bayer-ag.com/my%2Dgbox/" Visible="False">My 
        G|Box</asp:HyperLink>
        <asp:TextBox ID="lblACC" runat="server" Height="172px" TextMode="MultiLine" 
            Width="765px">Access Denied:</asp:TextBox>
                </asp:View>
                   <asp:View ID="CONSTR" runat="server">
                     <h1 style="font-family: Arial, Helvetica, Sans-Serif; font-size: 24px; font-weight: normal">Under construction</h1> 
                     <p>This Site is under construction. Please send your Autoclassification and Filtersettings request via 
                     eMail to <a href="mailto:BBS-MDRS-Support@bayer.com">MDAS-hotline</a>.</p>
                     <p>Diese Seite wird zur Zeit überarbeitet. Bitte senden Sie Ihre 
                       Anforderungen bezüglich Autoclassification und Filtersettings per Email an die 
                     <a href="mailto:BBS-MDRS-Support@bayer.com">MDAS-Hotline</a>.</p>

             </asp:View>
                <asp:View ID="DATA" runat="server">
                    <asp:TextBox ID="txtUserMail" runat="server" Height="172px" TextMode="MultiLine" Width="765px"></asp:TextBox>
                    <asp:TextBox ID="txtImplementationMail" runat="server" Height="172px" TextMode="MultiLine" Width="765px"></asp:TextBox>
                </asp:View>
                <asp:View ID="Search" runat="server">
                    <p>Enter search text in the textbox below use * for placeholders *xx* is also 
                        possible. </p>
                    <p>
                        This search comprises all objects in filtersettings tables: plants, sales 
                        organizations, message types,…</p>
                    <p/>
                    <asp:TextBox ID="txtKeywords" runat="server" AutoCompleteType="Search" 
                        Width="243px"></asp:TextBox>
                    <asp:ImageButton ID="imgEngine" runat="server" 
                        ImageUrl="~/Images/computer_go.gif" TabIndex="9" ToolTip="Search" />
                    <asp:ImageButton ID="imgSearchCancel" runat="server" 
                        CssClass="et_multiview_naviImgBut" Height="32" ImageUrl="~/Images/cancel.gif" 
                        TabIndex="10" ToolTip="back to Filter form" Width="32" />
                    <p/>
                    <p/>
                    <asp:GridView ID="grdSearch" runat="server" CellPadding="4" 
                        EmptyDataText="No Data found..." EnableModelValidation="True" 
                        Font-Names="Verdana" Font-Size="8pt" ForeColor="#333333" GridLines="None" 
                        SelectedIndex="0">
                        <FooterStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <PagerStyle BackColor="#666666" Font-Bold="True" ForeColor="White" 
                            HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <HeaderStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                        <Columns>
                            <asp:HyperLinkField />
                        </Columns>
                        <EditRowStyle BackColor="#999999" />
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    </asp:GridView>
                </asp:View>
        </asp:MultiView>
        </div>
    <div style="color:#555555; width:70%; padding: 10px 10px 5px 10px;">
        <asp:Menu ID="Menu2" runat="server" 
            DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="10px" 
            ForeColor="#555555" Orientation="Horizontal" StaticSubMenuIndent="10px">
            <StaticSelectedStyle BackColor="#FFFFFF" />
            <StaticMenuItemStyle HorizontalPadding="2px" VerticalPadding="2px" />
            <DynamicHoverStyle BackColor="#FFFFFF" ForeColor="#111111" Font-Underline="True" />
            <DynamicMenuStyle BackColor="#FFFFFF" />
            <DynamicSelectedStyle BackColor="#FFFFFF" />
            <DynamicMenuItemStyle HorizontalPadding="2px" VerticalPadding="2px" />
            <StaticHoverStyle BackColor="#FFFFFF" ForeColor="#111111" Font-Underline="True" />
            <Items>
                <asp:MenuItem NavigateUrl="#_Top" 
                    Target="_self" Text="Back to top" Value="Back to top" 
                    ImageUrl="~/Images/bullet_arrow_top.gif"></asp:MenuItem>
                <asp:MenuItem Text="Bookmark" 
                    Value="Bookmark" NavigateUrl="javascript:AddToFavorites();" 
                    ImageUrl="~/Images/bullet_star.gif"></asp:MenuItem>
            </Items>
        </asp:Menu>
    </div>
    </div>
</div>
<div id="bottompart">
    <%# Eval("Text") %>
</div>
    
  
</form>
</body>
</html>
