Public Partial Class Security
    Inherits System.Web.UI.Page
    Protected Sub cmdTest_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdTest.Click
        If My.User.Name = "" Then
            lblUser.Text = "You are " & My.User.Name
        Else
            lblUser.Text = "You are unknown !"
        End If
        If Context.User.Identity.Name = "" Then
            lblContext.Text = "YOU are UNKNOWN"
        Else
            lblContext.Text = "YOU ARE " & Context.User.Identity.Name
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub
End Class