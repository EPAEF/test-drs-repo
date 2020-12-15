Public Class RequesterApproverDetails
    Inherits System.Web.UI.Page
    Private mUser As myUser
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If String.IsNullOrEmpty(Request.QueryString("GroupId")) Or String.IsNullOrEmpty(Request.QueryString("Team")) Then
                Exit Sub
            End If
            LoadData()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub LoadData()
        Try
            Dim strValue As String = Request.QueryString("GroupId")
            Dim strTeam As String = Request.QueryString("Team")
            Dim strSQLUserSource As String = String.Empty
            Dim dtUserSource As DataTable = Nothing

            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
            End If
            'lblText.Text = "DRS " & strTeam & "for subgroup: " & strValue
            If strTeam.ToUpper = "REQUESTER TEAMS" Then
                lblText.Text = "DRS Requester(s) for subgroup: " & strValue
            Else
                lblText.Text = "DRS Approver(s) for subgroup: " & strValue
            End If
            dtUserSource = New DataTable
            dtUserSource = mUser.Databasemanager.MakeDataTable("SELECT USER_SOURCE FROM dp_Additional_Information WHERE TITLE = " & "'" & strTeam & "'")
            If Not dtUserSource Is Nothing Then
                strSQLUserSource = dtUserSource.Rows.Item(0).Item("USER_SOURCE").ToString
                strSQLUserSource = strSQLUserSource.Replace("|ORG_LEVEL_VALUE|", strValue)
            End If
            Dim dtResult As DataTable = mUser.Databasemanager.MakeDataTable(strSQLUserSource)
            If dtResult.Rows.Count <> 0 Then
                grdDetails.DataSource = dtResult
                grdDetails.DataBind()
            End If
        Catch ex As Exception
            grdDetails.DataSource = Nothing
            grdDetails.DataBind()
            Throw
        End Try
    End Sub
End Class