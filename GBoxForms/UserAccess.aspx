<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserAccess.aspx.vb" Inherits="GBoxForms.UserAccess" MaintainScrollPositionOnPostback="True" %>



<html>

<head id = "Autorisationset" runat="server">
<title>MDRS Autorisationset Manager</title>
<link rel="stylesheet" type="text/css" href="_styles/uaCss.css" />
        <style type="text/css">
            .style6
            {
                background-image: url('Images/linie-senkrecht.gif');
                background-position: right;
                background-repeat: repeat-y;
                vertical-align: top;
                height: 500px;
                width: 219px;
            }
        </style>
</head>
<body>
<form id="frmUserAccess" runat="server">
    <script language="javascript" type="text/javascript">
        function postBackByObject()
            {
                var o = window.event.srcElement;
                if (o.tagName == "INPUT" && o.type == "checkbox")
                {
                   __doPostBack("","");
                } 
            }
    </script><asp:MultiView id="mvUserAccess" runat="server" ActiveViewIndex="0">
        <asp:View id="Welcome" runat="server">
        <table style="width: 568px">
			<tr>
                 <td class="style5">
			    <asp:Label ID="lblUsername" runat="server" CssClass="mc-frm-username">
			    </asp:Label>
			    </td>
			    <td>
			    <asp:HyperLink ID="lnkMySite" runat="server" 
                        NavigateUrl="http://by-gbox.bayer-ag.com/my%2Dgbox/" CssClass="mc-frm-username" 
                        Target="_ ">My G|Box</asp:HyperLink>
			    </td>
			    <td>
			    <asp:HyperLink ID="lnkHelp" runat="server" 
                        NavigateUrl="http://by-bdc.bayer-ag.com/ibs/MDRSDev/Cockpit_Wiki/Home.aspx" 
                        CssClass="mc-frm-username" Target=" ">Help</asp:HyperLink>
		        </td>
		    </tr>
		</table>
			<asp:Table ID="tblUserInfo" runat="server" GridLines="none" CssClass="mc-frm-tblUserInfo">
			</asp:Table>
            <asp:Button id="cmdChooseApplication" runat="server" Text="Request authorization" 
                    Enabled="False" CssClass="mc-frm-cmdChooseApplication" />
			<asp:Button ID="cmdRequestResponisible" runat="server" 
                Text="Who can request access  for me ? " Visible="False" />
			<asp:Button ID="cmdComplete" runat="server" Text="Maintain G|Box user data" />&nbsp;
            <asp:Button ID="cmdShowMySmes" runat="server" Text="Show my tasks" />
            <p />    
            <asp:Label ID="lblImpersonate" runat="server" Text="test as:" Visible="False"></asp:Label>
            <asp:TextBox ID="txtImpersonate" runat="server" Visible="False"></asp:TextBox>
            <asp:Button ID="cmdImpersonate" runat="server" Text="Impersonate" 
                Visible="False" /> 
			    <p />
                    <asp:HyperLink ID="lnkMyMDS" runat="server" 
                        NavigateUrl="http://by-dbox.bayer-ag.com/MDR-Contact-Persons/" Target="_ " 
                        Visible="False">Who can help getting access ?</asp:HyperLink>
                    <p />
			<asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
			<p />
			    <asp:Label ID="lblRequestText" runat="server" 
                    Text="Your roles in G|Box : "></asp:Label>
			<p />
			<asp:GridView ID="grdRoles" runat="server" EnableSortingAndPagingCallbacks="True" 
                    CssClass="mc-frm-tblUserInfo" EmptyDataText="No G|Box role defined">
			</asp:GridView>
			 
			</asp:View>
			<asp:View id="ChooseTheApplication" runat="server">
			<table style="width: 568px">
			<tr>
                 <td class="style5">
			    <asp:Label ID="Label4" runat="server" CssClass="mc-frm-username" Text = "G|Box Authorization request">
			    </asp:Label>
			    </td>
			    <td>
			    <asp:HyperLink ID="HyperLink7" runat="server" 
                        NavigateUrl="http://by-dbox.bayer-ag.com/my%2Dgbox/" CssClass="mc-frm-username">My G|Box</asp:HyperLink>
			    </td>
			    <td>
			    <asp:HyperLink ID="HyperLink8" runat="server" 
                        NavigateUrl="http://by-bdc.bayer-ag.com/ibs/MDRSDev/request/req_Systemaccess/RequestAccessWiki/Home.aspx" 
                        CssClass="mc-frm-username" Target=" ">Help</asp:HyperLink>
		        </td>
		    </tr>
		</table>

			<asp:Button ID="cmdShowRequest" runat="server" Text="Display request" 
                        CssClass="mc-frm-cmdShowRequest" />
                 <p />       
				<table rules="all" style="width:100%;">
					<tr>
						<td class="style6" >
                        <asp:Label ID="lblChooseSubgroup" runat="server" Text="Subgroup" Visible="False" 
                                Width="100px"></asp:Label>
                        <asp:DropDownList ID="cmbAuthSetSubgroup" runat="server" AutoPostBack="True" 
                                Visible="False">
                        </asp:DropDownList>
                        <br />
                        <asp:Label ID="lblChooseDivision" runat="server" Text="Division" Visible="False" 
                                Width="100px"></asp:Label>
                        <asp:DropDownList ID="cmbAuthSetDivision" runat="server" AutoPostBack="True" 
                                Enabled="False" Visible="False">
                        </asp:DropDownList>
                        <br />
						
						<asp:TreeView ID="trvApplicationSet" onclick="javascript:postBackByObject()" 
                                runat="server" ImageSet="XPFileExplorer" NodeIndent="15" ShowCheckBoxes="All" 
                                ShowLines="True" PopulateNodesFromClient="False" Height="445px">
							<ParentNodeStyle Font-Bold="False" ImageUrl="~/Images/part-16x16.gif" />
							<SelectedNodeStyle BackColor="#B5B5B5" Font-Underline="False" HorizontalPadding="0px" VerticalPadding="0px" />
							<RootNodeStyle ImageUrl="~/Images/database-16x16.gif" />
							<NodeStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="Black" HorizontalPadding="2px" NodeSpacing="0px" VerticalPadding="2px" />
						</asp:TreeView>
						</td>
						<td style="height: 500px" class="mc-frm-tablePartBlue">
						<img src="Images/user.gif" width="16" height="16" border="0" style="border-style: none; border-color: inherit; border-width: medium; padding: 50px 0px 0px 0px; margin-left: 15px; margin-right: 0px; margin-bottom: 0px;" class="imgstyle1" />
						<asp:Label ID="lblUser" runat="server" 
                                Text="Choose the users to request access for" CssClass="mc-frm-lblUser"></asp:Label>
						<br />
						<asp:TextBox ID="txtUserToAdd" runat="server" Width="199px" CssClass="mc-frm-txtUserToAdd">
						</asp:TextBox>
						<img src="Images/add.gif" width="16" height="16" border="0" style="border:none; margin: 0px 0px 4px 0px; padding: 0px 10px 0px 10px; " />
						<asp:Button ID="cmdAddUser" runat="server" Text="Add user" 
                                CssClass="mc-frm-cmdAddUser" Width="150px" />
						<br />
						<asp:ListBox ID="lstUsersToCheck" runat="server" Width="200px" CssClass="mc-frm-lstUsersToCheck">
						</asp:ListBox>
						<img src="Images/remove.gif" width="16" height="16" border="0" style="border:none; margin: 0px 0px 4px 0px; padding: 0px 10px 0px 10px; " />
						<asp:Button ID="cmdRomoveSelected" runat="server" Text="Remove selected user" 
                                CssClass="mc-frm-cmdRomoveSelected" Width="150px" />
						<br />
                            <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                          
						</td>
						
					</tr>
				</table>
				<p />
				
				</asp:View>
        <asp:View id="smeView" runat="server">
        <p />
        <p />
        <asp:Button ID="cmdBack" runat="server" Text="Back" Width="100px" 
                CssClass="mc-frm-cmdBack" />

        <asp:Button ID="cmdSubmit" runat="server" Text="Submit" Width="100px" 
                CssClass="mc-frm-cmdSubmit" />
        <p />
            &nbsp;<asp:TextBox ID="txtSME" runat="server" TextMode="MultiLine" Height="356px" 
                Width="684px" ReadOnly="True" ></asp:TextBox>
                <asp:TextBox ID="txtSQL" runat="server" TextMode="MultiLine" Height="356px" 
                Width="684px" Visible="False" ></asp:TextBox>
        </asp:View>

        <asp:View id="UserData" runat="server">
        <table style="width: 568px">
			<tr>
                 <td class="style5">
			    <asp:Label ID="Label1" runat="server" CssClass="mc-frm-username" Text="Your Userdata">
			    </asp:Label>
			    </td>
			    <td>
			    <asp:HyperLink ID="HyperLink1" runat="server" 
                        NavigateUrl="http://by-dbox.bayer-ag.com/my%2Dgbox/" CssClass="mc-frm-username">My G|Box</asp:HyperLink>
			    </td>
			    <td>
			    <asp:HyperLink ID="HyperLink2" runat="server" 
                        NavigateUrl="http://by-bdc.bayer-ag.com/ibs/MDRSDev/request/req_Systemaccess/RequestAccessWiki/Home.aspx" 
                        CssClass="mc-frm-username" Target=" ">Help</asp:HyperLink>
		        </td>
		    </tr>
		</table>
              <table rules="all" style="width:100%;">
                       <tr>
						<td class="style1">
                            <asp:Label ID="lblSubgroup" runat="server" Text="Subgroup:"></asp:Label>
                        </td>
					    <td>
                        <asp:DropDownList ID="cmbSubgroup" runat="server" Width="250px"/>
                        </td>
                     </tr>  
                    <tr>
						<td class="style1">
                            <asp:Label ID="lblWindowsDomain" runat="server" Text="Windows Domain:"></asp:Label>
					    </td>
					    <td>
					    <asp:TextBox ID="txtWindowsDomain" runat="server" Width="250px"></asp:TextBox>
					    <asp:Label ID="lblWindowsDomainValidate" runat="server" Font-Size="12px">e.g. 
                            byaccount</asp:Label>
					    </td>
                     </tr>  
                     <tr>
						<td class="style1">
                            <asp:Label ID="lblCw_ID" runat="server" Text="Corporate wide ID (cwid):"></asp:Label>
					    </td>
					    <td>
					    <asp:TextBox ID="txtCwId" runat="server" Width="250px"></asp:TextBox>
					    <asp:Label ID="lblCwIdValidate" runat="server" Font-Size="12px"></asp:Label>

					    </td>
                     </tr>

					 <%--
                     '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELD FROM DB AND CODE
                    ' Comment           : Removed title from database and code
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 2014-11-26
                     ' Reference  END    : CR ZHHR 1035817
                     --%>
                        
                      <tr>
						<td class="style4">
                          <asp:Label ID="lblFirstname" runat="server" Text="First name:"></asp:Label>
					    </td>
					    <td class="style3">
                            <asp:TextBox ID="txtFirstname" runat="server" Width="250px"></asp:TextBox>
                            <asp:Label ID="lblFirstnameValidate" runat="server" Font-Size="12px"></asp:Label>
					    </td>
                     </tr>
                     <tr>
						<td class="style4">
                            <asp:Label ID="lblName" runat="server" Text="Last name:"></asp:Label>
					    </td>
					    <td class="style3">
                            <asp:TextBox ID="txtName" runat="server" Width="250px"></asp:TextBox>
                            <asp:Label ID="lblNameValidate" runat="server" Font-Size="12px"></asp:Label>
					    </td>
                     </tr>
                     <tr>
						<td class="style4">
                            <asp:Label ID="lblEmail" runat="server" Text="email address     "></asp:Label>
					    </td>
					    <td class="style3">
                            <asp:TextBox ID="txtEmail" runat="server" Width="250px"></asp:TextBox>
                            <asp:Label ID="lblEmailValidate" runat="server" Font-Size="12px">use internet 
                            mail address: mic.mail@BayerBBS.com</asp:Label>
    					</td>
                     </tr>
            </table>
            <p />
            <p />
            <p />
            <asp:Button ID="cmdSubmitUserData" runat="server" Text="submit" 
                    CssClass="mc-frm-cmdSubmit" />
        </asp:View>

        <asp:View id="mwImplementation" runat="server">
                <table style="width: 568px">
			<tr>
                 <td class="style5">
			    <asp:Label ID="Label2" runat="server" CssClass="mc-frm-username" Text="Request Management">
			    </asp:Label>
			    </td>
			    <td>
			    <asp:HyperLink ID="HyperLink3" runat="server" 
                        NavigateUrl="http://by-dbox.bayer-ag.com/my%2Dgbox/" CssClass="mc-frm-username">My G|Box</asp:HyperLink>
			    </td>
			    <td>
			    <asp:HyperLink ID="HyperLink4" runat="server" 
                        NavigateUrl="http://by-bdc.bayer-ag.com/ibs/MDRSDev/request/req_Systemaccess/RequestAccessWiki/Home.aspx" 
                        CssClass="mc-frm-username" Target=" ">Help</asp:HyperLink>
		        </td>
		    </tr>
		</table>
		    <br />
            <asp:Button ID="cmdMaintainCount" runat="server" Text="" />
            <asp:Button ID="cmdClosedCount" runat="server" Text="" />
            <asp:Button ID="cmdRejectedCount" runat="server" Text="" />
            <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
            <asp:Button ID="cmdSearch"
                runat="server" Text="Search" />
            <asp:Label ID="lblSearch" runat="server" Text=""></asp:Label>
            <asp:GridView ID="grdSme" runat="server" AutoGenerateEditButton="True" 
                CssClass="mc-frm-tblUserInfo" AllowSorting="True">
            </asp:GridView>
		    <asp:Label ID="lblClosed" runat="server" Text="False" Visible="False"></asp:Label>
            <asp:Label ID="lblSort" runat="server" Text="ASC" Visible="False"></asp:Label>
		</asp:View>
		<asp:View id="mwSMEdetail" runat="server">
		<table style="width: 568px">
			<tr>
                 <td class="style5">
			    <asp:Label ID="Label3" runat="server" CssClass="mc-frm-username" Text="Request Viewer">
			    </asp:Label>
			    </td>
			    <td>
			    <asp:HyperLink ID="HyperLink5" runat="server" 
                        NavigateUrl="http://by-dbox.bayer-ag.com/my%2Dgbox/" CssClass="mc-frm-username">My G|Box</asp:HyperLink>
			    </td>
			    <td>
			    <asp:HyperLink ID="HyperLink6" runat="server" 
                        NavigateUrl="http://by-bdc.bayer-ag.com/ibs/MDRSDev/request/req_Systemaccess/RequestAccessWiki/Home.aspx" 
                        CssClass="mc-frm-username" Target=" ">Help</asp:HyperLink>
		        </td>
		    </tr>
		</table>
		    <br />
            <asp:Label ID="lblID" runat="server" Text="Label"></asp:Label>
            <p /> 
                <asp:Label ID="lblRexesterLabel" runat="server" Text="Requester:"></asp:Label>
            <asp:Label ID="lblRequester" runat="server" Text=""></asp:Label>
            <p /> 
            <asp:Button ID="cmdInprogress" runat="server" Text="set inprogress" />
            <asp:Button ID="cmdClose" runat="server" Text="close ticket" />
            <asp:Button ID="cmdRejekt" runat="server" Text="reject ticket" />
                <asp:Button ID="cmdNewBack" runat="server" Text="Back" />
            <p />
            <asp:TextBox ID="txtSmeText" runat="server" Text="Label" Height="440px" 
                    TextMode="MultiLine" Width="698px"></asp:TextBox>
            <p />
            <p />
		</asp:View>
		<asp:View id="vwLocked" runat="server">
		<asp:Label ID="lblLocked" runat="server" Text=""></asp:Label>
		</asp:View>
		<asp:View id="vwReject" runat="server">

		    <asp:Label ID="lblRejektReason" runat="server" Text="Reject Reason:"></asp:Label>
            <p />
            <asp:TextBox ID="txtReason" runat="server" AutoPostBack="True" Height="322px" 
                    TextMode="MultiLine" Width="791px"></asp:TextBox>
             <p />
            <asp:Button ID="cmdSubmitRejekt" runat="server" Text="Reject" />

		</asp:View>
    </asp:MultiView>
</form>

</body>

</html>