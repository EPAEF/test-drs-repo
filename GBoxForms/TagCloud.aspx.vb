Option Strict Off
Partial Public Class TagCloud
    Inherits System.Web.UI.Page
    Private mUser As myUser
    Private mTAG As TagcloudIndexController
    Private UserId As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            mTAG = New GBoxForms.TagcloudIndexController()
            Dim lPathUrl As String = ""
            Dim UserId As String = Context.User.Identity.Name
            ' Reference         : CR ZHHR 1036254 - GBOX COC: New Concept for Landing Page
            ' Comment           : Added code for changing the folder structure and allowing only single file for showing the loading message
            '                     and redirecting the pages from a single point of folder and added empty string to Urlgetter.
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-12-26
            lPathUrl = mTAG.Urlgetter(sender, e).ToString
            ' Reference  END    : CR ZHHR 1036254
            If lPathUrl <> "" Then
                lblTagcloud.Text = lPathUrl
            Else
                lblTagcloud.Text = ""
            End If
        Catch ex As Exception
            lblTagcloud.Text = "Error in Tag-Cloud please call 99911 to adress ticket." & vbCrLf & "Your User info is: |" & UserId & "| ex"
        Finally
            If lblTagcloud.Text = "" Then
                lblTagcloud.Text = "Error in Tag-Cloud please call 99911 to adress ticket." & vbCrLf & "Your User info is: |" & UserId & "| fi"
            End If
        End Try
    End Sub

End Class
