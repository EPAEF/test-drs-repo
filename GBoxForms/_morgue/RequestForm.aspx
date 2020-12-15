<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RequestForm.aspx.vb" Inherits="GBoxForms.RequestForm" MaintainScrollPositionOnPostback="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Request to MDRS hotline</title>
    <style type="text/css">
    body
    {
        font-family: Verdana, Arial, sans-serif;
        margin: 0;
        padding: 0;
        background-color: #eeeeee;
    }
    #container
    {
    	background-color:#ffffff;
    	width: 1010px;
    }
    
    #mc_wiz
    {
        width: 1010px; 
        background-color:#ffffff; 
        background-image:url(../Images/farbverlauf_bg.jpg); 
        background-position:top right; 
        background-repeat:repeat-x;
        padding: 0px 0px 0px 0px; 
        margin: 0px 0px 0px 0px; 
        border-bottom: solid 1px #dddddd;
    }
    #footer
    {
        background-color:#999;
        background-image: url(../Images/bottom98763.jpg);
        background-position:left top;
        background-repeat:repeat-x;
        clear:left; 
        width: 834px;
        padding: 25px 0px 25px 176px; 
        margin: 0px 0px 0px 0px; 
    }
    #wrapper
    {
    	float:left; background-color:#FFF; height : 100%;
    }
    
    #navigationforms
    {
    	float:left;
    	width: 166px;
    	padding: 30px 0px 0px 0px; 
    }
    #forms
    {
	    color:#666;
	    text-align:left;
	    font-family: Verdana, Arial, sans-serif;
        padding: 10px 10px 10px 20px;
        width: 814px; 
    } 
    
    h1
    {
	    color:#666;
        font: Verdana, Arial, sans-serif;
	    font-size:20px;
	    font-weight:normal;
	    padding: 5px 0px 5px 0px;
	    margin: 0px 0px 0px 0px;
    }

    p
    {
    	color:#666;
    	font-family: Verdana, Arial, sans-serif;
	    font-size:12px;
    	padding: 0px 0px 0px 0px;
    	margin: 0px 0px 10px 0px;
    	width: 700px;
    }    
    .formsrow
    {
        background-color: #FFF; width: 814px;
    }
    .formsrow_left 
    {
    	font-size:9pt;
    	width: 245px;
    	text-align:left;
    	vertical-align:top;
    	float:left;
    	padding: 5px 0px 5px 0px;
    	background-color:#FFF;
	}
	.formsrow_right
	{
		width: 569px;
		text-align:left;
		float: left;
		padding: 5px 0px 5px 0px;
		background-color: #FFF;
	}
	/* Layout properties for your question  */
    .question
    {
        background-color: #FFF;	cursor:pointer;	
    }
		
    .answer
    {
        width:500px; background-color: #FFF;
		/* This one should not be changed */
        display:none;	
    }
	.formsrow_rightinf
	{
		font-family: Verdana, Arial, sans-serif; font-size: 11px; font-style:italic;
	}
    #txtTo, #txtCc, #txtReqShorttext
    {
    	line-height:15px; width:350px;
    }
    #txtReqLongtext
    {
    	width:350px;
    }
    #cboReqFor, #cboPriority
    {
    	line-height:20px; width:150px;
    }
    #rblProblemType, #rblSubType
    {
    	font-size:9pt; font-family: Verdana, Arial, sans-serif; font-style:normal;
    	color:#000;
    }
    #fileAttachments, #fileAttachment2, #fileAttachment3, #fileAttachment4, #fileAttachment5
    {
    	line-height:20px;
    	width:430px;
    }
    
    #txtTo, #txtCc, #txtReqShorttext,#txtReqLongtext, #cboReqFor, #cboPriority
    {
    	border: 1px solid #999;
    	border-collapse:separate;
    }
    #fileAttachments, #fileAttachment2, #fileAttachment3, #fileAttachment4, #fileAttachment5
    {
    	border: 1px solid #999;
    	border-collapse:separate;
    }
    
    /*********************************/
    #toppart 
    {
        background-color: #ffffff; 
        margin: 0px 0px 0px 0px; 
        padding: 0px 0px 0px 0px; 
        width: 1010px; 
        clear:left;
    }
    #toppart_topleiste 
    {
        font-size:10px; 
        color: #FFFFFF; 
        background-color: #444444; 
        height: 10px; 
        border-bottom: 1px solid #333333; 
        padding: 5px 5px 5px 5px;
        width: 1000px; 
    }
    #toppart_topleiste_left
    {
        text-align:left;
        width: 50%; 
        float:left;
    }
    #toppart_topleiste_right
    {
        width: auto;
        text-align:right; 
        width: 50%; 
        float:left;
    } 
    #toppart_topleiste_left a, #toppart_topleiste_right a
    {
    	text-decoration: none;
    }
	#toppart_logoleiste
    {
    	background-color: #ffffff;
		position:relative; 
		width: auto;
		height: 69px;
            top: 0px;
            left: 0px;
        }
    #toppart_logoleiste_left
    {
		background-color: #5c5d61;
		background-image: url(../Images/left-top-top.jpg);
		background-position:left;
		background-repeat:repeat-y;
		padding: 18px 0px 0px 21px;
		margin: 0px;
		width: 145px;
		height: 51px; 
		position: absolute;
		top: 0;
		left: 0;
   }
   #toppart_logoleiste_center
   {
		background: #ffffff;
		background-image:url(../Images/gbox_headimg.jpg); 
		background-position:bottom; 
		background-repeat:no-repeat;
		padding: 0;
		margin: 0px;
		width: 530px;
		height: 69px; 
		position: absolute;
		top: 0;
		left: 166px;
   	}
   	#toppart_logoleiste_right
   	{
		background-color: #ffffff;
		padding: 9px 0px 0px 0px; 
		margin: 0px 0px 0px 698px;
		height: 49px;
		width: auto;
		text-align:right;
   	}
    
    .toppart_support_navi
    {
	    font-size:9px; color: #5a5a5a; padding: 0px 70px 0px 0px;
    }
    .toppart_support_navi a
    {
	    font-size:9px; color: #5a5a5a; text-decoration: none;
    }
    .toppart_support_navi a:hover
    {
	    font-size:9px; color: #5a5a5a; text-decoration: none;
    }
    #toppart_navileiste
    {
    	background-color: #999999; color: #FFFFFF; 
    	border-bottom:1px solid #FFFFFF; border-top:1px solid #FFFFFF;
    }
    #Menu2
    {
    	font-size: small;
    	color: #FFFFFF;
    	padding: 0px 0px 0px 0px;
    	position: relative;
		left: 165px;
		border-left:1px solid #FFFFFF;
            top: 0px;
        }
    #Menu2 a
    {
    	padding: 0px 0px 0px 0px;
    }
    #toppart_bottomleiste 
    {
    	background-color: #666666; background-image:url(../Images/center-top_hellcolor.jpg); 
        background-position:top right; background-repeat:repeat-x;
        padding: 0px 0px 0px 0px; margin: 0px 0px 0px 0px; height: 33px;
    }
    .sitepath
    {
    	font-family: Verdana, Arial, sans-serif;
    	font-size: 10px;
    	color: #666;
    	padding: 5px 0px 5px 186px; 
    	margin: 0px 0px 0px 0px; 
    }
    #Menu3
    {
    } 
    /******************************************/
    .footerlink
    {
    	font-family: Verdana, Arial, sans-serif;
    	font-size: 8pt;
    	color: #FFF;
    	padding: 0px  50px  0px  0px;
    }
</style>
	<script type="text/javascript">
	    /*
	    (C) www.dhtmlgoodies.com, September 2005
	    This is a script from www.dhtmlgoodies.com. You will find this and a lot of other scripts at our website.	
	    www.dhtmlgoodies.com	
	    */
	    function showHideAnswer() {
	        var numericID = this.id.replace(/[^\d]/g, '');
	        var obj = document.getElementById('a' + numericID);
	        if (obj.style.display == 'block') {
	            obj.style.display = 'none';
	        } else {
	            obj.style.display = 'block';
	        }
	    }


	    function initShowHideContent() {
	        var divs = document.getElementsByTagName('DIV');
	        for (var no = 0; no < divs.length; no++) {
	            if (divs[no].className == 'question') {
	                divs[no].onclick = showHideAnswer;
	            }

	        }
	    }

	    window.onload = initShowHideContent;
	</script>
    <script language="javascript" type="text/javascript" src="_js/favorites.js"></script>
</head>

<body>
<form id="form1" runat="server" enctype="multipart/form-data">

<div id="container">
    <%--header with navigation bar--%>
<div id="mc_wiz_df">
    <div id="toppart">
        <div id="toppart_topleiste">
            <div id="toppart_topleiste_left">
                <asp:HyperLink ID="lnkHome" runat="server" 
                    NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/default.aspx" 
                    Text="Home" Target="_self" ForeColor="White" />
            </div>
            <div id="toppart_topleiste_right">
                <span style=""><asp:Label ID="lblInformations" runat="server" ></asp:Label></span>
            </div>
        </div>
        <div id="toppart_logoleiste">
            <div id="toppart_logoleiste_left">
                <asp:Image ID="imgGBoxLogo" runat="server" AlternateText="G|Box" ImageUrl="~/Images/gb_logo.jpg" />
            </div>
            <div id="toppart_logoleiste_center">
            &nbsp;
            </div>
            <div id="toppart_logoleiste_right">
                <asp:Image ID="imgBBS_Logo" runat="server" AlternateText="Bayer Business Services" Height="44px" ImageUrl="~/Images/logo_right_BBS.jpg" Width="312px" />
                <div class="toppart_support_navi">
                    <asp:Menu ID="Menu1" runat="server" Orientation="Horizontal" 
                        StaticSubMenuIndent="">
                        <Items>
                            <asp:MenuItem Text="Bookmark" Value="Bookmark" 
                                NavigateUrl="javascript:AddToFavorites();"></asp:MenuItem>
                            <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                            <asp:MenuItem Text="Services" Value="Services" NavigateUrl="http://by-gbox/" 
                                Target="_self"></asp:MenuItem>
                            <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                            <asp:MenuItem Text="Cockpit" Value="Cockpit" 
                                NavigateUrl="http://by-gbox/cockpit.aspx" Target="_self"></asp:MenuItem>
                            <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                            <asp:MenuItem Text="Search" Value="Search" 
                                NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/Shared%20Documents/search.aspx" 
                                Target="_self"></asp:MenuItem>
                            <asp:MenuItem Text="|" Value="|"></asp:MenuItem>
                            <asp:MenuItem Text="Contact" Value="Contact" 
                                NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/Lists/Contact/AllItems.aspx" 
                                Target="_self"></asp:MenuItem>
                        </Items>
                    </asp:Menu>
                </div>
            </div>
        </div>
        <div id="toppart_navileiste">
            <asp:Menu ID="Menu2" runat="server" Orientation="Horizontal" ForeColor="White" 
                StaticSubMenuIndent="">
                <Items>
                    <asp:MenuItem Text="Home" Value="Home" 
                        SeparatorImageUrl="~/Images/line_FFF.jpg" 
                        NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/default.aspx" 
                        Target="_self"></asp:MenuItem>
                    <asp:MenuItem SeparatorImageUrl="~/Images/line_FFF.jpg" Text="BCS" Value="BCS" 
                        NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/BCS/pMDASBCS/Home.aspx" 
                        Target="_self">
                    </asp:MenuItem>
                    <asp:MenuItem SeparatorImageUrl="~/Images/line_FFF.jpg" Text="BHC" Value="BHC" 
                        NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/BHC/default.aspx" 
                        Target="_self">
                    </asp:MenuItem>
                    <asp:MenuItem SeparatorImageUrl="~/Images/line_FFF.jpg" Text="DRS" Value="DRS" 
                        NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/drs/DRS_wiki/Home.aspx" 
                        Target="_self">
                    </asp:MenuItem>
                    <asp:MenuItem SeparatorImageUrl="~/Images/line_FFF.jpg" Text="Information" 
                        Value="Information" 
                        NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/information/default.aspx" 
                        Target="_self"></asp:MenuItem>
                    <asp:MenuItem SeparatorImageUrl="~/Images/line_FFF.jpg" Text="Internal" 
                        Value="Internal" 
                        NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/internal/default.aspx" 
                        Target="_self"></asp:MenuItem>
                    <asp:MenuItem SeparatorImageUrl="~/Images/line_FFF.jpg" Text="Projects" 
                        Value="Projects" 
                        NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/projects/default.aspx" 
                        Target="_self"></asp:MenuItem>
                    <asp:MenuItem SeparatorImageUrl="~/Images/line_FFF.jpg" Text="Request" 
                        Value="Request" 
                        NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/request/default.aspx" 
                        Target="_self"></asp:MenuItem>
                    <asp:MenuItem SeparatorImageUrl="~/Images/line_FFF.jpg" Text="Training" 
                        Value="Training" 
                        NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/training/default.aspx" 
                        Target="_self"></asp:MenuItem>
                </Items>
                <StaticHoverStyle BackColor="#FF9900" />
                <StaticMenuItemStyle VerticalPadding="0px" Font-Size="9pt" Height="20px" 
                    HorizontalPadding="5px" />
            </asp:Menu>
        </div>
        <div id="toppart_bottomleiste">
            <asp:Image ID="Image2" runat="server" 
                ImageUrl="~/Images/left-top-top.jpg" />
            <asp:Image ID="Image1" runat="server" 
                ImageUrl="~/Images/center-top_hellcolor_2.jpg" />
        </div>
        
        <div class="sitepath">Home > Request to MDAS hotline</div>
    </div>
</div>
<div id="navigationforms">
    <asp:Menu ID="Menu3" runat="server" Font-Size="8pt" Font-Bold="True">
        <Items>
            <asp:MenuItem Text="MDAS HOTLINE" Value="MDAS HOTLINE" 
                SeparatorImageUrl="~/Images/linevertical.jpg"></asp:MenuItem>
            <asp:MenuItem Text="GBOX Cockpit Help" Value="GBOX Cockpit Help" 
                SeparatorImageUrl="~/Images/linevertical.jpg" 
                NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/Cockpit_Wiki/Home.aspx" 
                Target="_self"></asp:MenuItem>
            <asp:MenuItem Text="About Us" Value="About Us" 
                SeparatorImageUrl="~/Images/linevertical.jpg" 
                NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/Shared%20Documents/AboutSite.aspx" 
                Target="_self"></asp:MenuItem>
            <asp:MenuItem Text="About Site" Value="About Site" 
                SeparatorImageUrl="~/Images/linevertical.jpg" 
                NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/Shared%20Documents/AboutSite.aspx" 
                Target="_self"></asp:MenuItem>
            <asp:MenuItem Text="Contact" Value="Contact" 
                SeparatorImageUrl="~/Images/linevertical.jpg" 
                NavigateUrl="http://sp-appl-bbs.bayer-ag.com/sites/010045/Lists/Contact/AllItems.aspx" 
                Target="_self"></asp:MenuItem>
        </Items>
        <StaticHoverStyle Font-Bold="True" Font-Underline="True" ForeColor="Black" 
            Font-Size="8pt" />
        <StaticMenuItemStyle Font-Size="8pt" ForeColor="#666666" 
            HorizontalPadding="20px" VerticalPadding="6px" />
    </asp:Menu>
</div>
    <%--central form--%>
<div id="wrapper">
    <div id="forms">
        <h1>Request to MDAS hotline </h1>
        <p>Use this form to send a request or question regarding all topics related to Master Data Reference Server on PMDM140 (e.g. MDRS access, material MD, customer and vendor MD, BARDO, BCC-LSMW,...)
            <br />
            <strong>For procurement functionality on Contract Server contact <a href="mailto:lisacsc@bayer-ag.de" target="_blank" title="lisacsc@bayer-ag.de">BBS Procurement SAS</a> at lisacsc@bayer-ag.de.</strong>
        <br /><br />
        Please describe your request IN ENGLISH and AS DETAILED AS POSSIBLE.
        <br />
                <asp:Label ID="lblRed" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblGreen" runat="server" Font-Bold="True" ForeColor="#009900"></asp:Label>
        </p>
        <%--
        <div class="formsrow">
            <div class="formsrow_left">To: </div> 
            <div class="formsrow_right"><asp:TextBox ID="txtTo" runat="server"></asp:TextBox></div>
        </div>
--%>
        <div class="formsrow">
            <div class="formsrow_left">Send a copy of this request to<br />
                use &quot;;&quot; as delimiter:</div>
            <div class="formsrow_right"><asp:TextBox ID="txtCc" runat="server"></asp:TextBox>
                <asp:ImageButton ID="imgCheckMail" runat="server" 
                    ImageUrl="~/Images/book_addresses.gif" 
                    ToolTip="Lookup email address for CWID(s)" />
            </div>
        </div>
        <div class="formsrow">
            <div class="formsrow_left">Request is for </div>
            <div class="formsrow_right">
                <asp:DropDownList ID="cboReqFor" runat="server">
                <asp:ListItem Value="BHC">BHC</asp:ListItem>
                <asp:ListItem Value="BCS"></asp:ListItem>
                <asp:ListItem Value="BBS"></asp:ListItem>
                <asp:ListItem Value="BAG"></asp:ListItem>
                <asp:ListItem Value="BMS"></asp:ListItem>
                <asp:ListItem Value="BTS"></asp:ListItem>
                <asp:ListItem Value="Currenta"></asp:ListItem>
                
                </asp:DropDownList>
            </div>
        </div>
        <div class="formsrow">
            <div class="formsrow_left">Select priority:</div>
            <div class="formsrow_right">
                <asp:DropDownList ID="cboPriority" runat="server">
                    <asp:ListItem>Low</asp:ListItem>
                    <asp:ListItem Selected="True">Normal</asp:ListItem>
                    <asp:ListItem>High</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="formsrow">
            <div class="formsrow_left">Problem type </div>
            <div class="formsrow_right">
                <span style="float:left; padding: 0px 10px 0px 0px;">
                <asp:RadioButtonList ID="rblProblemType" runat="server" Font-Size="9pt" 
                    AutoPostBack="True" EnableTheming="True" BorderColor="Gray" 
                    BorderStyle="Solid">
                </asp:RadioButtonList></span>
                <span style="float:left; padding: 0px 0px 0px 0px;">
                <asp:RadioButtonList ID="rblSubType" runat="server" Font-Size="9pt" 
                    BorderColor="Gray" BorderStyle="Solid">
                </asp:RadioButtonList></span>
            </div>
        </div>
        <div class="formsrow">
            <div class="formsrow_left">Short description </div>
            <div class="formsrow_right"><asp:TextBox ID="txtReqShorttext" runat="server" 
                    MaxLength="200"></asp:TextBox>
                    <div class="formsrow_rightinf">Please describe your request IN ENGLISH.</div>
            </div>
        </div>
        <div class="formsrow">
            <div class="formsrow_left">Long description </div>
            <div class="formsrow_right">
                <asp:TextBox ID="txtReqLongtext" runat="server" 
                    TextMode="MultiLine" Height="101px"></asp:TextBox></div>
        </div>
        <div class="formsrow">
            <div class="formsrow_left">Attachments&nbsp; </div>
            <div class="formsrow_right">
                <asp:FileUpload ID="fileAttachments" runat="server" EnableTheming="True" />
            </div>
        </div>
        <div class="formsrow">
            <div id="q1" class="question">+ add attachment</div>
            <div id="a1" class="answer">
                <div class="formsrow">
                    <div class="formsrow_left">&nbsp;</div>
                    <div class="formsrow_right">
                        <asp:FileUpload ID="fileAttachment2" runat="server" EnableTheming="True" /></div>
                </div>
                <div id="q2" class="question">+ add attachment</div>
                <div id="a2" class="answer">
                    <div class="formsrow">
                        <div class="formsrow_left">&nbsp;</div>
                        <div class="formsrow_right">
                            <asp:FileUpload ID="fileAttachment3" runat="server" EnableTheming="True" /></div>
                    </div>
                    <div id="q3" class="question">+ add attachment</div>
                    <div id="a3" class="answer">
                        <div class="formsrow">
                            <div class="formsrow_left">&nbsp;</div>
                            <div class="formsrow_right">
                            <asp:FileUpload ID="fileAttachment4" runat="server" EnableTheming="True" /></div>
                        </div>
                        <div id="q4" class="question">+ add attachment</div>
                        <div id="a4" class="answer">
                            <div class="formsrow">
                                <div class="formsrow_left">&nbsp;</div>
                                <div class="formsrow_right">
                                <asp:FileUpload ID="fileAttachment5" runat="server" EnableTheming="True" /></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="formsrow">
            <div class="formsrow_left">&nbsp;</div>
            <div class="formsrow_right">
                <span class="formsrow_rightinf">The maximum size of all attachements must not be over 5 MB.</span>
            </div>
        </div>
        <div class="formsrow">
            <asp:Button ID="btnSend" runat="server" Text="Submit" />&nbsp;<asp:Button ID="btnReset" runat="server" Text="Reset" />
        &nbsp;<br />
        </div>
    </div>
</div>
    <%--footer in the bottom--%>
<div id="footer">
</div>
</div>
</form>
</body>
</html>
