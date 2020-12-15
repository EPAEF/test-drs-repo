<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page Language="VB" %>

<script runat="server">
    Private mUser As GBoxForms.myUser
    Private mTAG As GBoxForms.TagcloudIndexController
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim mTAG As GBoxForms.TagcloudIndexController
        mTAG = New GBoxForms.TagcloudIndexController

        Dim s As String
        Dim i As Integer = 0
        Dim lLinktext As String = ""
        Dim lLinkUrl As String = ""
        Dim lLinkTimer As String = ""
        s = mTAG.Urlgetter(sender, e).ToString
        Dim parts As String() = s.Split(New Char() {"|"c})
        Dim part As String
        For Each part In parts
            If i = 0 Then
                lLinktext = part
            ElseIf i = 1 Then
                lLinkUrl = part
            ElseIf i = 2 Then
                lLinkTimer = part
            Else
                Exit For
            End If
            i += 1
        Next

        lblLinkbuttonTest.Text = "<span><a href='" & lLinkUrl.ToString & "' target='_self'>" & lLinktext.ToString & "</a></span>"
        ' Render: 
        '<meta http-equiv="refresh" content="1; URL=http://www.bing.com" />
        Dim meta As HtmlMeta = New HtmlMeta
        meta.Attributes.Add("http-equiv", "refresh")
        meta.Attributes.Add("content", "1; URL=" & lLinkUrl)

        Page.Header.Controls.Add(meta)
    End Sub
</script>

<html dir="ltr" xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="author" content="BBS-IBS-BI&amp;DM-Master Data Application Service" />
<meta name="publisher" content="BBS-IBS-BI&amp;DM-Master Data Application Service" />
<meta name="copyright" content="Bayer Business Services GmbH - IBS-BI&amp;DM-Master Data Application Service" />
<meta name="description" content="Welcome to G|Box, the web portal for all issues regarding Master Data Reference Server. You get information, help, templates and services, forms to request user access or customizing settings, documentation of Master Data projects and more." />
<meta name="keywords" content="GBox, Golden, Box, MDS, MDRS, Master, Data, Server, Management, Services, Application, Service, Microsoft, Windows, Office, Sharepoint, Access, Information, WSS, MOSS, DRS, Request, Customer, Vendor, Material, Currencey, MDAS, Product, BARDO, BCC, LSMW" />
<meta name="audience" content="Alle" />
<meta http-equiv="content-language" content="en" />
<meta name="robots" content="index, follow" />
<meta name="DC.Creator" content="BBS-IBS-BI&amp;DM-Master Data Application Service" />
<meta name="DC.Publisher" content="BBS-IBS-BI&amp;DM-Master Data Application Service" />
<meta name="DC.Rights" content="Bayer Business Services GmbH - IBS-BI&amp;DM-Master Data Application Service" />
<meta name="DC.Description" content="Welcome to G|Box, the web portal for all issues regarding Master Data Reference Server. You get information, help, templates and services, forms to request user access or customizing settings, documentation of Master Data projects and more." />
<meta name="DC.Language" content="en" />
<%--<meta http-equiv="refresh" content="1; URL=http://by-bdc.bayer-ag.com/ibs/MDRS/projects/default.aspx" />
--%>
<title>G|Box - Projects</title>
<link href="/_styles/subbox.css" rel="stylesheet" type="text/css" />
</head>

<body>

<form id="form1" runat="server">
<div class="mc-subweb-tp"><img src="../Images/gbox-wl_logo.jpg" alt="G|Box - Bridging your Master Data" width="616" height="82" border="0" /></div>
<div class="mc-subweb-bdy">
<h1>Welcome to GBox.</h1>
<h1>You will be forwarded to Projects immediately!</h1>
<p>If you are not being redirected, please click <asp:Label ID="lblLinkbuttonTest" runat="server">here</asp:Label></p>
</div>
</form>

</body>

</html>
