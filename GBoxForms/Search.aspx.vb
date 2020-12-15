Option Strict Off
Public Class Search
    Inherits System.Web.UI.Page
    Private mSearchEngine As New mySearchEngine
    Private mUser As myUser
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim lCriteria As String = ""
        Dim lViewname As String = "vw_Search_Engine"
        
        If Not Me.Request.Params("Search") Is Nothing Then
            lCriteria = Me.Request.Params("Search").Split("=")(0)
            txtKeywords.Text = lCriteria
            txtKeywords.Visible = False
            chkDrsSettings.Visible = False
            myDynamicTable.Visible = True
            imgEngine.Visible = False
        Else
            lblFiltertext.Text = ""
        End If
        If Not Me.Request.Params("Viewname") Is Nothing Then
            lViewname = Me.Request.Params("Viewname").Split("=")(0)
        End If
        With mSearchEngine
            If lCriteria <> "" Then
                Select Case lViewname
                    Case "vw_Search_Engine"
                        lblFiltertext.Text = "This is the search result for: " & lCriteria
                    Case "vw_Search_Engine_SETTINGS_ONLY"
                        lblFiltertext.Text = "This is the DRS Handbook search results for: " & lCriteria
                End Select
            End If
            .Context = Me.Context
            .Request = Me.Request
            mUser = .Authenticate_User()
            If mUser Is Nothing Then
                Exit Sub
            End If
            mUser.GBOXmanager.User = mUser
            .Linkserver = mUser.GBOXmanager.LinkServer
            If lCriteria <> "" Then
                Search(lCriteria, lViewname)
            End If
        End With
    End Sub
    Private Sub Search(ByVal lCriteria As String, ByVal lViewName As String)
        Dim I As Long
        With mSearchEngine
            .Viewname = lViewName
            .Search(lCriteria)
        End With
        For Each r As SearchResult In mSearchEngine.SearchResult
            Dim ctl As New HyperLink
            Dim lTablerow As New TableRow
            I = I + 1
            If I Mod 2 = 0 Then
                lTablerow.BackColor = Drawing.Color.LightBlue
            Else
                lTablerow.BackColor = Drawing.Color.LightGray
            End If
            Dim lTableCell As New TableCell
            ctl.NavigateUrl = r.NavigateUrl
            ctl.ID = "Search" & I
            ctl.Target = "_search"
            ctl.Text = r.ResultText
            ctl.Visible = True
            lTableCell = New TableCell
            lTableCell.Controls.Add(ctl)
            lTablerow.Cells.Add(lTableCell)
            myDynamicTable.Rows.Add(lTablerow)
        Next r
        
        If lViewName = "vw_Search_Engine_SETTINGS_ONLY" Then
            chkDrsSettings.ForeColor = Drawing.Color.Blue
        Else
            chkDrsSettings.ForeColor = Drawing.Color.Black
        End If
    End Sub
    Protected Sub imgEngine_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgEngine.Click
        Dim lParamlist As String = "?Search=" & txtKeywords.Text
        Dim lViewName As String = "&Viewname=vw_Search_Engine"
        If chkDrsSettings.Checked Then lViewName = "&Viewname=vw_Search_Engine_SETTINGS_ONLY"
        lParamlist = lParamlist & lViewName
        Me.Response.Redirect("~" & Me.Request.FilePath & lParamlist)
    End Sub

  
End Class