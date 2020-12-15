Public Partial Class SapDictionary
    Inherits System.Web.UI.Page
    Private mUser As myUser

    Sub SEARCH()
        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        txtSearchstring.Text.Replace("*", "%")
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If

        mySqlDataSource.ConnectionString = mUser.Databasemanager.cnnString
        mySqlDataSource.SelectCommand = "Select * from dictionary where English LIKE'%" & txtSearchstring.Text & "%' or German LIKE '%" & txtSearchstring.Text & "%'"
        grdData.DataBind()
        lblCountFound.Text = grdData.Rows.Count & " expressions found."
    End Sub
    Protected Sub cmdStart_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdStart.Click
        SEARCH()
    End Sub

    Protected Sub txtSearchstring_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtSearchstring.TextChanged
        SEARCH()
    End Sub

    
End Class