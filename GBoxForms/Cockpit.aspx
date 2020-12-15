<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Cockpit.aspx.vb" Inherits="GBoxForms.DynamicForm" MaintainScrollPositionOnPostback="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
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
    <style type="text/css">
        .style6
        {
            width: 145px;
        }
        .style7
        {
            width: 100%;
        }
        </style> 
    <!-- Insert Link to favorites -->
    <script language="javascript" type="text/javascript" src="_js/favorites.js"></script>
   
    <script type="text/javascript">
        function changeMyUrl(url) {
            document.location.href = url;
        }
    </script>

    <script language="javascript" type="text/javascript">
        function postBackByObject(source) { __doPostBack(source, ""); }
    </script>

    <script type="text/javascript">
        ///Reference         : CRT 2049665, GBOX_COC:Wrong selected grid cell value
        ///Comment           : Bug fixed in case of multipal value is selected and unselected back
        var cbCount = 0;
        var selectedIndex = [];
        function CBchanged(sender) {
            //Count selected items
            cbCount += sender.checked ? 1 : -1;
            //Disable change button if more than one selected             
            document.getElementById("<%=btnChangeValue.ClientID %>").disabled = (cbCount == 1 ? false : true);

            if (sender.checked) {
                //add to array if selected
                selectedIndex.push(sender.parentElement.parentElement.rowIndex - 1);
            }
            else {
                //remove from array if unseleted
                selectedIndex.splice(selectedIndex.indexOf(sender.parentElement.parentElement.rowIndex - 1), 1);
            }

            //Assign array to value
            document.getElementById("<%=hfRowIndex.ClientID %>").value = selectedIndex.toString();

            //if copy button and selected more than one then disable copy button
            if (document.getElementById('btnCopyValue') != null)
            {
                document.getElementById("<%=btnCopyValue.ClientID %>").disabled = (cbCount == 1 ? false : true);
            }
        }


    </script>
    <script type = "text/javascript">
        function SetSource(SourceID) {
            var hidSourceID = document.getElementById("<%=hidSourceID.ClientID%>");
            hidSourceID.value = SourceID;
        }
    </script>
    <script type = "text/javascript">
        function searchKeyPress(e) {
            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('imgEngine').click();
                return false;
            }
            return true;
        }
    </script>
    <script type="text/javascript">
        function filterKeyPress(e) {
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('imgAppend').focus();
                return false;
            }
            return true;
        }
    </script>
    <script type="text/javascript">
        function reportKeyPress(e, buttonid) {
            var evt = e ? e : window.event;
            var btnImgQuery = document.getElementById(buttonid);
            if (btnImgQuery) {
                if (evt.keyCode == 13) {
                    btnImgQuery.click();
                    return false;
                }
            }
        }
    </script>
    <script type="text/javascript">
        function enterKeyPress(e, buttonid) {
            var evt = e ? e : window.event;
            var btnNextSubmit = document.getElementById(buttonid);
            if (btnNextSubmit) {
                if (evt.keyCode == 13) {
                    btnNextSubmit.click();
                    return false;
                }
            }
        }
    </script>
    <%--<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jquery.dataTables/1.9.0/jquery.dataTables.min.js" ></script>--%>
</head>
<body style="margin: 0px 0px 0px 0px; padding: 0px 0px 0px 0px;">
<form id="myForm" method="post" runat="server">
<asp:HiddenField runat="server" id="hfRowIndex" value="" />
<asp:HiddenField ID="hidSourceID" runat="server" />
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
                <asp:LinkButton ID="lnkLogOut" runat="server">Logout</asp:LinkButton>
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
            <div id="toppart_logoleiste_right">

                    <asp:Image ID="imgBBS_Logo" runat="server" 
                    AlternateText="Bayer" Height="44px" 
                    ImageUrl="~/Images/bayer_cross_rgb.png" Width="44px" />
               <!-- 
                ' Reference         : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
                ' Comment           : Hide header links
                ' Added by          : Pratyusa Lenka (CWID : EOJCG)
                ' Date              : 07-11-2018
               -->
             <%--<div class="toppart_support_navi">--%>
                        <%--Search|Contact|Sitemap|Cockpit--%>
                    <%--<asp:Menu ID="mnSupportNavi" runat="server" Orientation="Horizontal">
                    <Items>
                    <asp:MenuItem Text="Search" Value="Search" Target="_blank" 
                    NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/Lists/Contact/AllItems.aspx"></asp:MenuItem>
                    <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                    <asp:MenuItem Text="Contact" Value="Contact" Target="_blank" 
                    NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/Shared%20Documents/search.aspx#"></asp:MenuItem>
                    <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                    <asp:MenuItem Text="Authorization management" Value="Authorization" Target="_blank" 
                    NavigateUrl="Autorisation.aspx"></asp:MenuItem>
                    <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                    <asp:MenuItem Text="Refresh menu"  > 
                    </asp:MenuItem>
                </Items>
                </asp:Menu>
             </div>--%>
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
            <StaticHoverStyle BackColor="#FFCC00" ForeColor="White" />
            <StaticItemTemplate>
                <%# Eval("Text") %>
            </StaticItemTemplate>
            </asp:Menu>
            </div>
        </div>
    </div>
<div id="corpus">
        <!-- 
        ' Reference         : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
        ' Comment           : Hide the wizard area
        ' Added by          : Pratyusa Lenka (CWID : EOJCG)
        ' Date              : 07-11-2018
        -->
       <%--<div id="corpus_desc">          
           <asp:Table ID="tblWiz" runat="server">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>
                    <asp:Label ID="lblTablenameData" runat="server" Visible="False" CssClass="corpus_info_label"></asp:Label>
                    <asp:Label ID="lblIsTree" runat="server" CssClass="corpus_info_label"></asp:Label>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
           <asp:TableRow>
                    <asp:TableCell style="border-style: none; width: 50px;">
                     
                        <asp:Image ID="imgBubble" runat="server" 
                            ImageUrl="Images/user_INFO.gif" 
                            BorderStyle="None" />
                        <asp:Label ID="lblInfo" runat="server" Font-Size="XX-Small"></asp:Label>
                    
                    </asp:TableCell>
                    <asp:TableCell VerticalAlign="Top" style="border-style: none; " CssClass="style3">
                
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
                </asp:TableCell>
             
                </asp:TableRow>
           </asp:Table>
       </div>--%> 
       <div id="corpus_center">                   
       <table id="tblButtons">
                    <tr align="right">
                        <td style="text-align:left; padding: 0px 10px 0px 0px;" >
                             <asp:Button ID="btnAddValue" runat="server" Text="Add value" OnClientClick = "SetSource(this.id)" />
                         </td>
                         <td style="text-align:left; padding: 0px 10px 0px 0px;" >
                             <asp:Button ID="btnChangeValue" runat="server" Text="Change value" disabled="true" OnClientClick = "SetSource(this.id)"/>
                         </td>
                         <td style="text-align:left; padding: 0px 10px 0px 0px;" >
                             <asp:Button ID="btnCopyValue" runat="server" Text="Copy Value" 
                                 OnClientClick = "SetSource(this.id)" Visible="False"/>
                         </td>
                         <!-- 
                            ' Reference         : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
                            ' Comment           : Hide how to request
                            ' Added by          : Pratyusa Lenka (CWID : EOJCG)
                            ' Date              : 07-11-2018
                           -->
                         <%--<td style="text-align:left; padding: 0px 10px 0px 0px; width:8%;">
                            <asp:HyperLink ID="hlHowToRequest" runat="server" Enabled="true" Text="HowTo Request" NavigateUrl="javascript:window.open('http://sp-appl-bbs.bayer-ag.com/sites/010045/Cockpit_Wiki/HowTo%20create%20a%20DRS%20request.aspx',null, 'toolbar=yes,location=yes, status= yes,menubar=yes, scrollbars=yes,resizable=yes'); void('');"></asp:HyperLink>
                         </td>--%>
                         <td class="style7">
                          <asp:ImageButton ID="imgRefresh" runat="server" ImageUrl="~/Images/arrow_refresh.gif" 
                                 Enabled="True" CssClass="et_multiview_naviImgBut" Height="32" Width="32" 
                                 ToolTip="Refresh tree"/>
                          <asp:ImageButton ID="imgDownloadToExcel" runat="server" Height="32px" Width="32px" 
                                 Enabled="False" ImageUrl="~/Images/download_gray.png"
                                 ToolTip="Download to Excel" Visible="False"/>
                          <asp:ImageButton ID="imgQuery" runat="server" ImageUrl="~/Images/report_go_2.gif" 
                                 Enabled="False" CssClass="et_multiview_naviImgBut" Height="32" Width="32" 
                                 ToolTip="Run report" Visible="False"/>
                       
                             <asp:ImageButton ID="imgCancel" runat="server" ImageUrl="~/Images/cancel.gif" 
                                 CssClass="et_multiview_naviImgBut" Height="32" Width="32" Enabled="False" 
                                 ToolTip="Cancel action" TabIndex="10"/>
                             <asp:ImageButton ID="imgAlert" runat="server" ImageUrl="~/Images/bell_add.png" 
                                 CssClass="et_multiview_naviImgBut" Height="32px" Width="34px" 
                                 ToolTip="Alert me" TabIndex="10" Enabled="False"/>
                             <asp:ImageButton ID="imgmySubscriptions" runat="server" ImageUrl="~/Images/user_edit.gif" 
                                 CssClass="et_multiview_naviImgBut" Height="32px" Width="34px" 
                                 ToolTip="My Alerts" TabIndex="10"/>
                            <asp:ImageButton ID="imgSearchEngine" runat="server" ImageUrl="~/Images/page_find.gif" 
                                 CssClass="et_multiview_naviImgBut" Height="32px" Width="34px" 
                                 ToolTip="Search for Objects" TabIndex="10"/>
                             <asp:ImageButton ID="imgHelp" runat="server" CssClass="et_multiview_naviImgBut" 
                                 Enabled="true" Height="32" ImageUrl="~/Images/book_go.png" 
                                 ToolTip="DRS Knowledge Base (in new window)" Width="32" /> 
                             <!-- 
                                ' Reference         : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
                                ' Comment           : Change tooltip text
                                ' Added by          : Pratyusa Lenka (CWID : EOJCG)
                                ' Date              : 07-11-2018
                             -->
                             <asp:ImageButton ID="imgHelp_withHelpIcon" runat="server" CssClass="et_multiview_naviImgBut" 
                                 Enabled="true" Height="32" ImageUrl="~/Images/help.gif" 
                                 ToolTip="G|Box Help" Width="32" Visible="False" />
                                             
                             <asp:CheckBox ID="chkPaging" runat="server" AutoPostBack="True" Checked="True" 
                                 ForeColor="Black" Text="use paging" TextAlign="Right" 
                                 ToolTip="Allow Paging" Visible="False" CssClass="corpus_info_label" 
                                 Font-Bold="True" />
                         
                                 <asp:DropDownList ID="cmbFieldFilter" runat="server" Visible="False" 
                                        style="margin-left: 1px" ></asp:DropDownList>                    
                                <asp:TextBox ID="txtFiltertext" runat="server" Visible="False" style="width: 128px" onkeypress ="filterKeyPress(event)"></asp:TextBox>
                                <asp:ImageButton ID="imgAppend" runat="server" ImageUrl="~/Images/check.gif" 
                                        Visible="False" ToolTip="Apply Filter" />
                                <asp:ImageButton ID="imgRelease" runat="server" ImageUrl="~/Images/BOMB.png" 
                                    Visible="False" style="width: 16px" ToolTip="Remove Filter" />
                         </td>
                        </tr>
                        <tr>
                            <td colspan="6" align ="right" >
                               <table  width="30%">
                               <tr><td><asp:Label ID="lblFilter" ForeColor="Black" runat="server" Font-Names="Verdana" Font-Size="Small" 
                                Text="Show Values"></asp:Label></td>
                                <td>
                               <asp:RadioButtonList ID="rdFilter" runat="server" RepeatDirection="Horizontal" 
                                        CssClass="corpus_info_label" Visible ="false" AutoPostBack="True" >
                                      <asp:ListItem Text="Active"></asp:ListItem>
                                      <asp:ListItem Text="InActive"></asp:ListItem> 
                                      <asp:ListItem Text="All" Selected="True"></asp:ListItem> 
                                  </asp:RadioButtonList>        
                                </td></tr>  
                                  </table>
                            </td>
                        </tr>
                        
                    </table>
                    <table width="650px" style="border: 1px solid black;" id="tblStartMenu" runat="server">
                        <tr align="left">
                        <%--<td>
                            <asp:Label ID="lblStart" runat="server" Font-Names="Verdana" Font-Size="Small" Text="START"></asp:Label>
                         </td>--%>
                         <td>
                            <asp:Label ID="lblMainData" runat="server" Font-Names="Verdana" Font-Size="Medium" Text="MAIN DATA"></asp:Label>
                         </td>
                         <td>
                            <asp:Label ID="lblTextTranslation" runat="server" Font-Names="Verdana" Font-Size="Medium" Text="TEXT TRANSLATION"></asp:Label>
                         </td>
                         <td>
                            <asp:Label ID="lblSystemsInfo" runat="server" Font-Names="Verdana" Font-Size="Medium" Text="SYSTEMS & REQUEST INFO"></asp:Label>
                         </td>
                         <td>
                            <asp:Label ID="lblSubmit" runat="server" Font-Names="Verdana" Font-Size="Medium" Text="SUBMIT"></asp:Label>
                         </td>
                        </tr>
                    </table>
              <table rules="all" style="width:100%;" id="MyTable">  
              <tr valign="top" style="line-height: 1px; padding: 0px; margin: 0px; ">
                <td style="width:25%; border: 0px;">
                    <asp:Image ID="imgSpacer" runat="server" Height="1px" 
                        ImageUrl="~/Images/_spacer.gif" Width="200px" /></td>
                <td style="width:75%; border: 0px;"><asp:Image ID="Image1" runat="server" Height="5px" 
                        ImageUrl="~/Images/_spacer.gif" Width="1px" /></td>
              </tr>
              <tr valign="top">
                 
                <td valign="top" rowspan="2" id="myFirstColumn" style="border: 0px;">
                <asp:Label ID="lblStatus" runat="server" ForeColor="Black" Text="Statusline" 
                        CssClass="corpus_info_label" Font-Bold="True"></asp:Label>  
                    <p />
                    <asp:TreeView ID="trvOBJ"    
                         runat="server" 
                        AutoGenerateDataBindings="False" PopulateNodesFromClient="False" 
                        ShowLines="True" EnableClientScript="False" Width="71%" 
                        Font-Names="Verdana" Font-Size="Small" ExpandDepth="0" 
                        HoverNodeStyle-BackColor="#5498D4" BorderWidth="0px">
                        <HoverNodeStyle BackColor="#5498D4"></HoverNodeStyle>
            	        <SelectedNodeStyle BackColor="#FEAE17" ForeColor="White" />
            	        <ParentNodeStyle Font-Bold="False" ImageUrl="~/Images/application_double_s.gif" />
                        <RootNodeStyle ImageUrl="~/Images/application_cascade_s.gif" />
                        <NodeStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="Black" 
                            HorizontalPadding="2px" NodeSpacing="0px" VerticalPadding="2px" 
                            ImageUrl="~/Images/application_s.gif" />
                    </asp:TreeView>
                    <p />
                    <asp:Label ID="lblLegende" runat="server" 
                            Text="To request changes in the treestructure above,  use hotlinebutton from menubar." 
                            Visible="False" CssClass="corpus_info_label" Font-Bold="True"></asp:Label>
                    <p />
                    <asp:Image ID="imgLegende" runat="server" ImageUrl="~/Images/Legende.png" 
                            Visible="False" />
                </td>
                  
              </tr>
              <tr>
              <td valign="top" style="border-style: none; text-align:left; margin-bottom: 10px;">
              <!-- 
               '---------------------------------------------------------------------------------------------------
                ' Reference         : ZHHR 1039680 - GBOX COC: Move Database Information on screen
                ' Comment           : Move database information on screen
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 2015-03-05
               -->
              <div id="ontopofgrid">
                <asp:Label ID="lblDatabasename" CssClass="corpus_info_label" Font-Bold="True" Font-size="Small"  BackColor="ButtonFace"  runat="server" />
              </div>
              <!-- 
              Reference End        : ZHHR 1039680
               -->             
             <asp:MultiView ID="mvContents" runat="server" ActiveViewIndex="0">
                <asp:View ID="vwGridView" runat="server">
                    <table rules="all" style="width:100%;" id="tblmnuDocTab">  
                         <tr valign="top">
                             <td style="border: 0px;" class="style6" >
                                 <asp:Menu ID="mnuDocTab" runat="server" DynamicHorizontalOffset="3" 
                                                                         Font-Bold="False" 
                                                                         Font-Names="Verdana"  
                                                                         ForeColor="Black" 
                                                                         Orientation="Horizontal"  
                                                                         StaticSubMenuIndent="10px" 
                                                                         Font-Size="Small" 
                                                                         Visible="False" 
                                                                         BorderColor="#CCCCCC" >
                                    <StaticSelectedStyle BackColor="#FEAE17" />                  
                                    <StaticMenuItemStyle HorizontalPadding="5px" BackColor="ButtonFace"/>
                                    <DynamicHoverStyle ForeColor="Black" BorderStyle="Outset" />
                                    <DynamicMenuStyle BackColor="#FEAE17" />
                                    <DynamicSelectedStyle BackColor="#FEAE17" ForeColor="Black" />
                                    <DynamicMenuItemStyle HorizontalPadding="5px"  BackColor="ButtonFace"/>
                                    <Items>
                                        <asp:MenuItem Text="Data" ToolTip="Display Data" Value="0" Selected="True"></asp:MenuItem>
                                        <asp:MenuItem Text="Info" ToolTip="Display Information" Value="1"></asp:MenuItem>
                                        <asp:MenuItem Text="2" ToolTip="" Value="2"></asp:MenuItem>
                                        <asp:MenuItem Text="3" ToolTip="" Value="3"></asp:MenuItem>
                                        <asp:MenuItem Text="4" ToolTip="" Value="4"></asp:MenuItem>
                                        <asp:MenuItem Text="5" ToolTip="" Value="5"></asp:MenuItem>
                                        <asp:MenuItem Text="6" ToolTip="" Value="6"></asp:MenuItem>                            
                                    </Items>
                                    <StaticHoverStyle BackColor="#FFCC00" ForeColor="Black" />
                                    <StaticItemTemplate>
                                        <%# Eval("Text") %>
                                    </StaticItemTemplate>
                                </asp:Menu>
                             </td>
                         </tr>   
                    </table>
                    <asp:MultiView ID="mvDocuTab" runat="server" ActiveViewIndex="0">
                        <asp:View ID="vwGrid" runat="server">
                           <asp:GridView ID="grdDat" runat="server" CellPadding="4" 
                                EnableModelValidation="True" Font-Names="Verdana" Font-Size="8pt" 
                                ForeColor="#333333" GridLines="None" SelectedIndex="0">
                                <FooterStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                <PagerStyle BackColor="#666666" Font-Bold="True" ForeColor="White" 
                                    HorizontalAlign="Center" />
                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                <HeaderStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                                <Columns>
                                    <asp:HyperLinkField Visible="false" />
                                    <asp:TemplateField HeaderText="Select">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="SelectCheckBox" runat="server" onclick="CBchanged(this)" Visible='<%# If(IsItemLocked(Container.DataItem) = "True", "False", "True") %>' />
                                            <asp:Image ID="imgLock" ImageUrl="~/Images/lock.png" runat="server" Visible='<%# If(IsItemLocked(Container.DataItem) = "True", "True", "False") %>' ToolTip="The value is currently locked." />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EditRowStyle BackColor="#999999" />
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            </asp:GridView>
                           
                        </asp:View>
                        <asp:View ID="vwDocuGrid" runat="server">
                            <asp:GridView ID="grdDocu" runat="server" CellPadding="4" 
                                EnableModelValidation="True" Font-Names="Verdana" Font-Size="8pt" 
                                ForeColor="#333333" GridLines="None" SelectedIndex="0" 
                                EmptyDataText="No Docu found...">
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
                        <asp:View ID="vwGrid2" runat="server">                            
                            <asp:GridView ID="grd2" runat="server" CellPadding="4" 
                                EnableModelValidation="True" Font-Names="Verdana" Font-Size="8pt" 
                                ForeColor="#333333" GridLines="None" SelectedIndex="0" 
                                EmptyDataText="No Data found...">
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
                        <asp:View ID="vwGrid3" runat="server">                        
                            <asp:GridView ID="grd3" runat="server" CellPadding="4" 
                                EnableModelValidation="True" Font-Names="Verdana" Font-Size="8pt" 
                                ForeColor="#333333" GridLines="None" SelectedIndex="0" 
                                EmptyDataText="No Data found...">
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
                        <asp:View ID="vwGrid4" runat="server">                        
                            <asp:GridView ID="grd4" runat="server" CellPadding="4" 
                                EnableModelValidation="True" Font-Names="Verdana" Font-Size="8pt" 
                                ForeColor="#333333" GridLines="None" SelectedIndex="0" 
                                EmptyDataText="No Data found...">
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
                        <asp:View ID="vwGrid5" runat="server">                        
                            <asp:GridView ID="grd5" runat="server" CellPadding="4" 
                                EnableModelValidation="True" Font-Names="Verdana" Font-Size="8pt" 
                                ForeColor="#333333" GridLines="None" SelectedIndex="0" 
                                EmptyDataText="No Data found...">
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
                        <asp:View ID="vwGrid6" runat="server">                        
                            <asp:GridView ID="grd6" runat="server" CellPadding="4" 
                                EnableModelValidation="True" Font-Names="Verdana" Font-Size="8pt" 
                                ForeColor="#333333" GridLines="None" SelectedIndex="0" 
                                EmptyDataText="No Data found...">
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
                </asp:View>
                <asp:View ID="vwDetails" runat="server">
                <asp:Label ID="lblCaption" runat="server" Text="1. Maintain object" Font-Bold="True" Font-Size="Small"></asp:Label>
                        <asp:ScriptManager EnablePartialRendering="true" ID="smdvInfo" runat="server"></asp:ScriptManager> 
                        <asp:UpdateProgress ID="updProgress" AssociatedUpdatePanelID="updvInfo" runat="server" DynamicLayout="true" DisplayAfter="0">
                            <ProgressTemplate>
                                <div class="overlay">
                                    <div class="center">
                                        <img alt="progress" src="images/ajax-loader.gif" /><br />
                                        Please wait...
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:UpdatePanel ID="updvInfo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <asp:DetailsView ID="dvInfo" runat="server" Height="50px" Width="90%" 
                                AutoGenerateRows="False" CellPadding="4" Font-Size="8pt" 
                                ForeColor="#333333" GridLines="None" DefaultMode="Edit" OnDataBound="dvInfo_DataBound">
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" />
                                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                <Fields>
                                    <asp:CommandField ShowInsertButton="True" />
                                </Fields>
                                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <EditRowStyle BackColor="#999999" />
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            </asp:DetailsView>
                    </ContentTemplate>
                    </asp:UpdatePanel>
                    <div style="text-align: right; position: relative; right: 10%;">
                        <asp:Button ID="btnNextEditText" runat="server" Text="Next >>" OnClientClick = "SetSource(this.id)" />
                    </div>
                </asp:View>
                
                <asp:View ID="vwQuery" runat="server">
                    <asp:DetailsView ID="dvQuery" runat="server" Height="50px" Width="100%" 
                        AutoGenerateRows="False" CellPadding="4"  Font-Size="8pt"
                        ForeColor="#333333" GridLines="None" DefaultMode="Edit">
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <Fields>
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <EditRowStyle BackColor="#999999" />
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    </asp:DetailsView>
                    <asp:Label ID="lblToMany" runat="server" Enabled="False" ForeColor="Red" 
                        CssClass="corpus_info_label"></asp:Label>
                    <br />
                    <asp:GridView ID="grdQuery" runat="server" CellPadding="4" Font-Names="Verdana" 
                        Font-Size="8pt" ForeColor="#333333" GridLines="None">
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <HeaderStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                        <EditRowStyle BackColor="#999999" />
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    </asp:GridView>
                    <asp:Label ID="lblSQlReport" runat="server" CssClass="corpus_info_label"></asp:Label>
                    
                </asp:View>
                <asp:View ID="vwEditTexts" runat="server">
                    <h2 class="mc-dfrm-EditTextsView">
                    <asp:Label ID="Label14" runat="server" Text="2. Maintain texts"></asp:Label>
                    </h2>
                    <p class="mc-dfrm-EditTextsView">
                    <asp:Label ID="lblObj_ID" runat="server" Text="DE"></asp:Label>
                    <asp:Label ID="Label15" runat="server" Text="texts for:"></asp:Label>
                    <asp:Label ID="lblObj_VALUE" runat="server" Text="DE"></asp:Label>
                    <asp:Label ID="Label16" runat="server" Text="version" Visible="False"></asp:Label>
                    <asp:Label ID="lblVersionnumber" runat="server" Text="DE" Visible="False"></asp:Label>
                    <asp:Label ID="lblRequestType" runat="server" Text="" Visible="False"></asp:Label>
                    <asp:Label ID="Label18" runat="server" Text=". Max text length:"></asp:Label>
                    <asp:Label ID="lblTextlenght" runat="server" Text="DE"></asp:Label>
                       
                    </p>
                     <%--<asp:ImageButton ID="imgTextFill" runat="server" 
                            ImageUrl="~/Images/textfield.png" ToolTip="Fill Empty Texts With EN" />
                     <asp:ImageButton ID="imgOverwriteEnglish" runat="server" 
                         ImageUrl="~/Images/textfield_rename.gif" ToolTip="Fill all Texts With EN" />--%>
                     <asp:Button ID="btnTextFill" runat="server" Text="Fill Empty Texts With EN" OnClientClick = "SetSource(this.id)" />
                     <asp:Button ID="btnOverwriteEnglish" runat="server" Text="Fill all Texts With EN" OnClientClick = "SetSource(this.id)" />
                     <asp:Table ID="tblTexts" runat="server" 
                         CssClass="mc-dfrm-EditTextsView-tblcell">
                           
                     </asp:Table>
                     <div style="text-align: right; position: relative; right: 20%;">
                        <asp:Button ID="btnNextEditSystems" runat="server" Text="Next >>" OnClientClick = "SetSource(this.id)" />
                    </div>
                 </asp:View>
                <asp:View ID="vwEditSysthems" runat="server">
                    <h2 class="mc-dfrm-EditTextsView">
                        <asp:Label ID="Label19" runat="server" Text="3. Maintain systems and SME for pMDAS customizing:" 
                            CssClass="corpus_info_label"></asp:Label>
                    </h2>
                    <p class="mc-dfrm-EditTextsView">
                        <asp:Label ID="lblSysObj_ID" runat="server" Text="DE" CssClass="mc-dfrm-EditTextsView-Label"></asp:Label>
                        <asp:Label ID="Label21" runat="server" Text="distributing systems for:" 
                            CssClass="mc-dfrm-EditTextsView-Label"></asp:Label>
                        <asp:Label ID="lblSysObj_VALUE" runat="server" Text="" CssClass="mc-dfrm-EditTextsView-Label"></asp:Label>
                        <asp:Label ID="Label23" runat="server" Text="Versionnumber:" CssClass="mc-dfrm-EditTextsView-Label"></asp:Label>
                        <asp:Label ID="lblSysVersionnumber" runat="server" Text="" CssClass="mc-dfrm-EditTextsView-Label"></asp:Label>
                    </p>
                    <p />
                     <div style="margin-bottom: 10px">
                  <asp:Label ID="lblrequestfor" runat="server" Text="Requested for: " 
                      Font-Size="Small" CssClass="lblRequestFor"></asp:Label>
                  <asp:DropDownList ID="cmborglevel" runat="server" AutoPostBack="True" onkeyup="return enterKeyPress(event,'btnSubmit')">
                  </asp:DropDownList> &nbsp;&nbsp;  
                  <asp:DropDownList ID="cmborglevelvalue" runat="server" AutoPostBack="True" onkeyup="return enterKeyPress(event,'btnSubmit')">
                  </asp:DropDownList></div>
                    <asp:Label ID="lblchkSYSTEMSCaption" visible="false" runat="server" Text="Please customize changes in the following systems:" 
                    CssClass="mc-dfrm-EditTextsView-Label" Font-Bold="True" Font-Underline="True"></asp:Label>
                    <p class="mc-dfrm-EditTextsView-tblcell-CB ">
                    <asp:CheckBoxList ID="chkSYSTEMS" runat="server" CssClass="mc-dfrm-EditTextsView-Label" onkeydown="return enterKeyPress(event,'btnSubmit')">
                    </asp:CheckBoxList>
                    </p>
                  <div style="margin-bottom: 10px; margin-top:10px;">
                  <asp:Label ID="lblRequestComment" runat="server" Text="Request Comment: " Font-Size="Small" CssClass="lblRequestFor"></asp:Label>
                  <asp:TextBox ID="txtRequestComment" runat="server" onkeypress="return enterKeyPress(event,'btnSubmit')"
                          Width="269px"></asp:TextBox>
                  </div>
                  <div>
                  <label class="file-upload">
                     <asp:Label ID="lblAttachFile" runat="server" Text="Attach file: " Font-Size="Small" CssClass="lblAttachFile"> Attach file :</asp:Label>
                    <asp:FileUpload ID="fuAttachment" runat="server" allowmultiple="true" CssClass="fuAttachment" />
                  </label>
                                   
                    <br />
                    <div class="compHolder">
                    <asp:Button ID="btnUpload" runat="server" Text="Add" OnClick="Upload" />
                    <asp:Button ID="btnDelete" runat="server" Text="Remove" />
                    <asp:Button ID="btnDownloadAttachment" runat="server" Text="Download" />
                    </div>
                    <div class="compHolder">
                    <asp:Label ID="lblMessage" runat="server" CssClass="corpus_info_label" Font-Bold="true" />
                    </div>
                    <div class="compHolder">
                    <asp:ListBox ID="lbAttachment" Visible="false"  runat="server" Rows="5" SelectionMode="Single" Width="407px"></asp:ListBox>
                    </div>
                  </div>
                  <br />
                    <div style="text-align: right; position: relative; right: 60%;">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClientClick = "SetSource(this.id)"/>
                    </div>
                 </asp:View>
                <asp:View ID="vwSql" runat="server">
                    <asp:ListBox ID="lstSQL" runat="server"></asp:ListBox>
                    <asp:ListBox ID="lstKey_IDs" runat="server"></asp:ListBox>
                    <asp:ListBox ID="lstKeyValues" runat="server"></asp:ListBox>
                    <asp:ListBox ID="lstFields" runat="server"></asp:ListBox>
                    <asp:ListBox ID="lstValues" runat="server"></asp:ListBox>
                     <asp:ListBox ID="lstDisplay" runat="server"></asp:ListBox>
                      <asp:ListBox ID="lstOldTexts" runat="server"></asp:ListBox>
                     <asp:ListBox ID="lstNewTexts" runat="server"></asp:ListBox>
                      <asp:ListBox ID="lstCurrentDocControls" runat="server"></asp:ListBox>
                      <asp:ListBox ID="lstKeyCollection" runat="server"></asp:ListBox>
                </asp:View>
                <asp:View  ID = "vwSubscribe" runat="server"> 
                    <asp:Label ID="Label34" runat="server" 
                        Text="Select the Subscriptions to delete:"></asp:Label>
                    <asp:CheckBoxList ID="chkSubscriptions" runat="server">
                    </asp:CheckBoxList>
                    <asp:ImageButton ID="imgSubmit_Cancel_Subscribtion" runat="server" 
                        ImageUrl="~/Images/computer_go.gif" ToolTip="Submit" />
                    <asp:ImageButton ID="imgCancel_Subscription" runat="server" 
                        CssClass="et_multiview_naviImgBut" Height="32" ImageUrl="~/Images/cancel.gif" 
                        TabIndex="10" ToolTip="Cancel action" Width="32" />
                 </asp:View>
                <asp:View  ID = "vwSearchEngine" runat="server"> 
                    <asp:TextBox ID="txtKeywords" runat="server" Width="243px" Text="Search"
                         AutoCompleteType="Search" onFocus="if(this.value == 'Search') {this.value = '';}" onBlur="if (this.value == '') {this.value = 'Search';}"
                         onkeypress="return searchKeyPress(event);"></asp:TextBox>
                     <asp:ImageButton ID="imgEngine" runat="server" 
                         ImageUrl="~/Images/computer_go.gif" ToolTip="Search" TabIndex="9" OnClick="imgEngine_Click" />
                     <asp:CheckBox ID="chkDrsSettings" runat="server" Text="DRS Handbook" Checked="true" />
                     <asp:CheckBox ID="chkGPGSettings" runat="server" Text="GPG Database" />
                     <br />
                     <asp:Label ID="lblSearchHelpText" runat="server" 
                        Text="Please enter your search term (e.g. object name, description, field name etc.)" 
                        Font-Size="Small"></asp:Label>
                    <p />
                     &nbsp;
                    <asp:Table ID="myDynamicTable" runat="server" GridLines="Both"></asp:Table> 
                 </asp:View> 
                <asp:View  ID = "vwCPS" runat="server"> 
                    <h2 class="mc-dfrm-EditTextsView">
                    <asp:Label ID="Label13" runat="server" Text="Customizing Object:"></asp:Label>
                    </h2>
                                                 
                    <p class="mc-dfrm-EditTextsView" />
                    <p class="mc-dfrm-EditTextsView" />
                    <asp:ImageButton ID="imgNewCustomizingObj" runat="server" 
                         ImageUrl="~/Images/table_add.png" CssClass="et_multiview_naviImgBut" 
                         Height="32" Width="32" 
                                 ToolTip="Add value"/>
         
                    <asp:ImageButton ID="imgEditCustomizingObj" runat="server" 
                         ImageUrl="~/Images/table_edit.gif" Enabled="False" 
                         ToolTip="Edit Customizing Object" />
                    <asp:ImageButton ID="imgSubmitCustomizingObj" runat="server" ImageUrl="~/Images/table_go.png" 
                                 CssClass="et_multiview_naviImgBut" Enabled="False" Height="32" Width="32" 
                                 ToolTip="Submit Customizing changes"/>
                    <asp:ImageButton ID="imgWiki" runat="server" 
                         ImageUrl="~/Images/book_addresses.gif" ToolTip="Navigate to Wiki" Height="32" 
                         Width="32" style="margin-left: 0px; margin-right: 15px" />
                    <asp:ImageButton ID="imgHotline" runat="server" 
                         ImageUrl="~/Images/application_side_tree.png" ToolTip="Navigate to Hotline" 
                         Height="32" Width="32" />
                    <p class="mc-dfrm-EditTextsView" />
                    <asp:Table ID="tblCPS" runat="server" GridLines="Both" Width="701px" >
                        <asp:TableRow>
                            <asp:TableCell>
                                <asp:Label ID="lblName" runat="server" Text="Name:" CssClass="corpus_info_label"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell>
                             <!--   <asp:DropDownList ID="cboCustomizingObjName" runat="server" Visible="False" Enabled="False" AutoPostBack="True">
                                </asp:DropDownList> -->
                                <asp:TextBox ID="txtCustomizingObjName" runat="server" Width="350px" Enabled="False" AutoPostBack="False"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell>
                                <asp:Label ID="lblMandatoryType" runat="server" Text="Mandatory Type:" CssClass="corpus_info_label"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:DropDownList ID="cboMandatoryType" runat="server" Enabled="False">
                                </asp:DropDownList>
                                
                            </asp:TableCell>
                            
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell>
                                <asp:Label ID="lblVersionTXT" runat="server" Text="     Versionnumber:" CssClass="corpus_info_label"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:Label ID="lblVersionnumberCust" runat="server" Text="" CssClass="corpus_info_label"></asp:Label>
                            </asp:TableCell>
                        </asp:TableRow>     
                        <asp:TableRow>
                            <asp:TableCell>
                                <asp:Label ID="lblSolutionType" runat="server" Text="Solution Type:" CssClass="corpus_info_label"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="chkALL" runat="server" Enabled="False" AutoPostBack="True" /><asp:Label ID="lblAll" runat="server" Text="ALL" CssClass="corpus_info_label"></asp:Label>
                              </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell></asp:TableCell>
                            <asp:TableCell>  
                                <asp:CheckBox ID="chkFi" runat="server" Enabled="False" AutoPostBack="True" /><asp:Label ID="lblFI" runat="server" Text="FI" CssClass="corpus_info_label"></asp:Label>
                             </asp:TableCell>
                        </asp:TableRow>

                        <asp:TableRow>
                        <asp:TableCell></asp:TableCell>
                            <asp:TableCell>   
                                 <asp:CheckBox ID="chkSO" runat="server" Enabled="False" AutoPostBack="True" /><asp:Label ID="lblSO" runat="server" Text="SO" CssClass="corpus_info_label"></asp:Label>   
                           </asp:TableCell>
                        </asp:TableRow> 
                        <asp:TableRow>
                            <asp:TableCell>
                                <asp:Label ID="lblWikiname" runat="server" Text="Wikiname:" CssClass="corpus_info_label" Visible="False"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox ID="txtWikiName" runat="server" Width="350px" Enabled="true" Visible="False"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell>
                                
                            </asp:TableCell>
                        </asp:TableRow>
                         <asp:TableRow>
                            <asp:TableCell>
                                <asp:Label ID="lblWikiUrl" runat="server" Text="Wikiurl:" CssClass="corpus_info_label" Visible="True"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox ID="txtWikiUrl" runat="server" Width="350px" Enabled="false" Visible="True"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell>
                                
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                     
                     <p />
                      <p />
                       <p />
                     <h2 class="mc-dfrm-EditTextsView">
                    <asp:Label ID="Label20" runat="server" Text="Documentation Object:"></asp:Label>
                    </h2>
                    <p class="mc-dfrm-EditTextsView" />
                    <asp:ImageButton ID="imgNewDocumentationObject" runat="server" 
                         ImageUrl="~/Images/table_add.png" CssClass="et_multiview_naviImgBut" 
                         Height="32" Width="32" 
                                 ToolTip="Add value" Enabled="False" Visible="False"/>
                    <asp:ImageButton ID="imgEditDocumentation" runat="server" 
                         ImageUrl="~/Images/table_edit.gif" Enabled="False" 
                         ToolTip="Edit Documentation Object" />
                    <asp:ImageButton ID="imgSubmitDocumentation" runat="server" ImageUrl="~/Images/table_go.png" 
                                 CssClass="et_multiview_naviImgBut" Enabled="False" Height="32" Width="32" 
                                 ToolTip="Submit Documentation changes"/>    
                     <p class="mc-dfrm-EditTextsView" />  
                     <asp:Table ID="tblNewNode" runat="server" GridLines="Both" Width="700px" 
                         Visible="False">
                        <asp:TableRow>
                            <asp:TableCell CssClass = "mc-dfrm-EditTextsView-tblcellLeft" BorderStyle="Ridge">
                                <asp:Label ID="lblNewNode" runat="server" Text="New Node Name:" Width="300" CssClass="mc-dfrm-EditTextsView-Label"  ></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass = "mc-dfrm-EditTextsView-tblcellRight">
                                <asp:TextBox ID="txtNewNode" runat="server" Width="350px" Enabled="True" CssClass="mc-dfrm-EditTextsView-TB"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell CssClass = "mc-dfrm-EditTextsView-tblcellLeft" BorderStyle="Ridge">
                                <asp:Label ID="lblCust" runat="server" Text="Customizing Object:" Width="300" CssClass="mc-dfrm-EditTextsView-Label"  ></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass = "mc-dfrm-EditTextsView-tblcellRight">
                                <asp:DropDownList ID="cboCustomizing_ObjName" runat="server" Visible="False" Enabled="False" AutoPostBack="True">
                                </asp:DropDownList>
                            </asp:TableCell>
                        </asp:TableRow>
                       
                        <asp:TableRow>
                            <asp:TableCell CssClass = "mc-dfrm-EditTextsView-tblcellLeft" BorderStyle="Ridge">
                                <asp:Label ID="lblSearchEngineKeyWords" runat="server" Text="Search Engine Key Words:" Width="300" CssClass="mc-dfrm-EditTextsView-Label"  ></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass = "mc-dfrm-EditTextsView-tblcellRight">
                                <asp:TextBox ID="txtSearchEngineKeyWords" runat="server" Width="350px" Enabled="True" CssClass="mc-dfrm-EditTextsView-TB"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell CssClass = "mc-dfrm-EditTextsView-tblcellLeft" BorderStyle="Ridge">
                                <asp:Label ID="lblRank" runat="server" Text="Rank of Node in Tree:" Width="300" CssClass="mc-dfrm-EditTextsView-Label"  ></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell CssClass = "mc-dfrm-EditTextsView-tblcellRight">
                                <asp:TextBox ID="txtRank" runat="server" Width="350px" Enabled="True" CssClass="mc-dfrm-EditTextsView-TB"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                       </asp:Table>
                     <p class="mc-dfrm-EditTextsView" />     
                    <asp:Table ID="tblDocumentation" runat="server" GridLines="Both" Width="700px" > 
                    </asp:Table>
                 </asp:View> 
              
               </asp:MultiView>
                </td>
              </tr>
        
            </table>
        </div>
    
  </div>

<hr />
<asp:Label ID="lblImpersonate" runat="server" Text="test as:" Visible="False"></asp:Label>
<asp:TextBox ID="txtImpersonate" runat="server" Visible="False"></asp:TextBox>
<asp:Button ID="cmdImpersonate" runat="server" Text="Impersonate" Visible="False" />
<br />
<div id="bottompart">
<asp:Label ID="lblTechnicalInfo" runat="server"></asp:Label>
</div>
</form>
</body>
</html>
