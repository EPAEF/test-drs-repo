<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page Language="VB" %>
<html dir="ltr" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="description" content="GBox is a service for Master data." />
<meta name="keywords" lang="en" content="GBox, Golden Box, box, Master Data Application Service, MDRS, Master Data, Cockpit, service, services, Bayer, Bayer Business Services, BBS, information, request, MARTY" />
<meta name="robots" content="index,follow" />
<meta name="DC.Title" content="GBox-Bridging your Master Data" />
<meta name="author" content="Bayer Business Services GmbH" />
<meta name="DC.Creator" content="Bayer Business Services GmbH" />
<meta name="robots" content="index,follow" />
<meta name="publisher" content="Bayer Business Services GmbH" />

<!-- 
    Weiterleitung durch refresh: <meta http-equiv="refresh" content="5; URL=http://www.example.org/" /> 
-->
<title>Welcome to G|Box</title>
<!-- Icon -->
<link rel="apple-touch-icon" href="Images/apple-touch-icon.png" />
<link rel="shortcut icon" href="Images/favicon.ico" />
<!-- CSS and Design -->
<link rel="Stylesheet" type="text/css" href="/_styles/index.css" media="screen" />
<!-- Check Server -->
<script language="javascript" type="text/javascript" src="_js/hostbox.js"></script>
<!-- Insert Link to favorites -->
<script language="javascript" type="text/javascript" src="_js/favorites.js"></script>
<!-- START JavaScript to open ext. Flashevent -->
</head>

<body>
    <form id="form1" runat="server">
   <div id="base"> 
    <div id="mc_top"><a href="http://sp-appl-bbs.bayer-ag.com/sites/010045/default.aspx" target="_self">
	    <img src="Images/gbox-index_logo.jpg" alt="G|Box - Bridging your Master Data" style="border:0;" /></a>
    </div>
    <!-- 18-08-2017 - Eojch - IM0005075548 - added link to Continue text-->
    <div id="mc_toplink">
        <a href="http://sp-appl-bbs.bayer-ag.com/sites/010045/default.aspx">CONTINUE &raquo;</a>
    </div>
	<div id="mc_image_section">
    <a href="http://sp-appl-bbs.bayer-ag.com/sites/010045/default.aspx" target="_self">
        <img  usemap="#map1" src="Images/gbox_bridge.jpg"   alt="G|Box - Bridging your Master Data"  width="984" height="400" style="border:0;" /></a>
    </div>
    <div>
	<div id="mc_bottom">
	    <div class="mc_bottom_catalog">
		<h1>New at GBOX?</h1>
		<ul>
		<li><a href="http://sp-appl-bbs.bayer-ag.com/sites/010045/Cockpit_Wiki/NEW.aspx" target="_self">Find your way around &raquo;</a></li>
		</ul>
	    </div>
		<div class="mc_bottom_catalog">
			<h1>DRS Handbook</h1>
			<ul>
                <li><a href="http://by-gbox.bayer-ag.com/DRS-Knowledge-Base/" target="_self">DRS Knowledge Base &raquo;</a></li>
                <li><a href="http://by-gbox.bayer-ag.com/DRS-Global-Settings" target="_self">DRS Global Settings &raquo;</a></li>
                <li><a href="http://pc.intranet.cnb/#/profitcenter" target="_self">Profit Center Settings &raquo;</a></li>
                <li><a href="http://gbox.intranet.cnb/" target="_self">Chart of Accounts &raquo;</a></li>
                <li><a href="http://by-gbox.bayer-ag.com/Cockpit.aspx?CONTEXT=general&TOPICGROUP=Global%20Process%20Governance&TOPIC=SAP%20Implementation%20Guide" target="_self">GPG Handbook &raquo;</a></li>
			</ul>
		</div>
	    <div class="mc_bottom_catalog">
			<h1>Master Data Services</h1>
			<ul>
			    <li><a href="http://by-gbox.bayer-ag.com/BCC-LSMW/" target="_self">Mass update with BCC-LSMW &raquo;</a></li>
                <li><a href="http://by-gbox.bayer-ag.com/BARDO/" target="_self">Reporting with BARDO &raquo;</a></li>
                <li><a href="http://sp-appl-bbs.bayer-ag.com/sites/010045/information/services/Services_wiki/" target="_self">Check Master Data with Trillium and PIES &raquo;</a></li>
                <li><a href="http://by-gbox.bayer-ag.com/pMDAS/" target="_self">pMDAS Services &raquo;</a></li>
                <li>&nbsp;</li>
           </ul>
		</div>
	</div>
    </div>
    <div id="mc_bottom2">
	    <div class="mc_bottom_catalog">
		    <h1>Request Management</h1>
		    <ul>
		        <li><a href="http://sp-appl-bbs.bayer-ag.com/sites/010045/Cockpit_Wiki/Requesting.aspx" target="_self">Learn how request Master Data related changes &raquo;</a>
                <li><a href="http://by-gbox.bayer-ag.com/DRS-Global-Settings" target="_self">- DRS Global Settings &raquo;</a></li></li>
                <li><a href="http://by-gbox.bayer-ag.com/AutoClass.aspx" target="_self">- Filter Settings &raquo;</a></li>
                <li><a href="http://by-gbox.bayer-ag.com/DRS-Global-Settings" target="_self">- Non-DRS Customizing Settings &nbsp;»</a></li>
		    </ul>
	    </div>
		<div class="mc_bottom_catalog">
			<h1>Master Data Information</h1>
			<ul>
			    <li><a href="http://by-gbox.bayer-ag.com/Material/" target="_self"> Material Master Data &raquo;</a></li>
                <li><a href="http://by-gbox.bayer-ag.com/Customer-and-Vendor/" target="_self"> Customer and Vendor Master Data &raquo;</a></li>
                <li></li>
			</ul>
		</div>
	    <div class="mc_bottom_catalog">
			<h1>Access and Password reset</h1>
			<ul>
			    <li><a href="http://by-gbox.bayer-ag.com/System-Access/" target="_self">Access to MD SAP&raquo;</a></li>
                <li><a href="http://sp-appl-bbs.bayer-ag.com/sites/010045/Cockpit_Wiki/Access.aspx" target="_self">Access to GBOX &raquo;</a></li>
                <li><a href="http://by-gbox.bayer-ag.com/BCC-LSMW/" target="_self">Access to BCC-LSMW &raquo;</a></li>
                <li><a href="http://by-gbox.bayer-ag.com/BARDO/" target="_self">Access to BARDO &raquo;</a></li>
			</ul>
		</div>
	</div>
	<div id="mc_task">
		<!--
        <div id="mc_task_line">
            <img src="Images/gbox-index_task_linie.jpg" alt="" width="984" height="11" border="0" />
        </div>
        -->
		<div id="mc_task_txt"><a href="http://sp-appl-bbs.bayer-ag.com/sites/010045/default.aspx" target="_self">Home</a> | <a href="http://sp-appl-bbs.bayer-ag.com/sites/010045/Lists/AboutUs/AllItems.aspx" target="_self">About</a> | <a href="javascript:AddToFavorites();">Bookmark</a><span style="color:#FF0000;"></span></div>
	</div>
</div>
</form>
</body>
</html>

