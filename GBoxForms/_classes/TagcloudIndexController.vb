Option Strict Off
Public Class TagcloudIndexController
    Inherits System.Web.UI.Page
    Private mUser As myUser
    Private mRequest As String = ""
    'Private mTAG As TagcloudIndexController

    ' ///////////////////////////////////////////////////
    ' Declaration of tag cloud
    Public Function TagcloudingBig(ByVal sender As Object, ByVal e As EventArgs, Optional ByVal lTagcloudeID As String = "")
        ' ///////////////////////////////////////////////////
        ' Ceck current user
        Tagcloudchack()

        Try
            Dim lPath As String = Context.Request.FilePath.ToString()
            lPath = lPath.ToUpper
            Dim lHost As String = GetHoststring(Context.Request.Url.Host.ToString())

            Dim lSQLTC As String = "SELECT * " & _
                " FROM [TAG_CLOUD_TAG] " & _
                " WHERE [TAG_CLOUD_TARGET] = '" & lHost & "'" & _
                " AND [TAG_CLOUD_ID] = '" & lTagcloudeID & "'" & _
                " AND [TAG_CLOUD_SITETYPE_ID] = 'WEBSITE'" & _
                " Order By [TAG_ID] "
            Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSQLTC)
            Dim lCount As String = dt.Rows.Count.ToString

            Dim lTCRoles As String = ""
            Dim lTagcloud As String = ""

            Dim lSQLCOLOR As String = "SELECT [TAG_CLOUD_STYLE_ID] " & _
                ", [TAG_CLOUD_ID], [DESCRIPTION] " & _
                " FROM [TAG_CLOUD_STYLE] " & _
                " WHERE [TAG_CLOUD_ID] = '" & lTagcloudeID & "'"
            Dim dtCOLOR As DataTable = mUser.Databasemanager.MakeDataTable(lSQLCOLOR)
            Dim z As Integer = 0
            For i As Integer = 0 To lCount - 1
                Dim lColor As DataRow = dtCOLOR.Rows(z)
                Dim lData As DataRow = dt.Rows(i)
                lTCRoles = lTCRoles & "<span style='font-size:" & lData("TAG_SIZE").ToString & "%' "
                lTCRoles = lTCRoles & "class='" & lColor("TAG_CLOUD_STYLE_ID").ToString
                lTCRoles = lTCRoles & "'>"
                lTCRoles = lTCRoles & "<a href='" & lData("TAG_URL").ToString & "' "
                lTCRoles = lTCRoles & "target='_" & lData("TAG_TARGET_WINDOW").ToString & "'>"
                lTCRoles = lTCRoles & lData("DISPLAYTEXT").ToString & "</a> "
                lTCRoles = lTCRoles & "</span>"
                lTagcloud = lTCRoles
                z = z + 1
                If z >= dtCOLOR.Rows.Count - 1 Then z = 0
            Next i
            Return lTagcloud
        Catch ex As Exception
            mRequest = "TagcloudingBig: An error has occured. "
            mRequest += ex.Message
            Return mRequest
        End Try
    End Function

    ' ///////////////////////////////////////////////////
    ' Ceck current user
    Private Sub Tagcloudchack()
        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        pObjCurrentUsers.CONTEXT = Context

        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If
    End Sub
    Private Function GetHoststring(ByVal lContext As String) As String
        Dim lHost As String = ""
        Select Case lContext
            Case "localhost"
                lHost = "localhost"
            Case "by-dbox.bayer-ag.com"
                lHost = "DBOX"
            Case "by-dbox"
                lHost = "DBOX"
            Case "by-qbox.bayer-ag.com"
                lHost = "QBOX"
            Case "by-qbox"
                lHost = "QBOX"
            Case "by-gbox.bayer-ag.com"
                lHost = "GBOX"
            Case "by-gbox"
                lHost = "GBOX"
            Case Else
                lHost = " pHostRequest was not Initialized. "
        End Select
        Return lHost
    End Function

    ' ///////////////////////////////////////////////////
    ' To check location of host and path string
    Public Function Urlgetter(ByVal sender As Object, ByVal e As EventArgs) As String
        Try
            Dim lPath As String = Context.Request.FilePath.ToString()
            lPath = lPath.ToUpper
            Dim lHost As String = GetHoststring(Context.Request.Url.Host.ToString())
            Dim lShortlink As String = ""
            Dim lShortlinkUrl As String = ""
            Dim lShortlinktext As String = ""
            ' ///////////////////////////////////////////////////
            ' Ceck current user
            Tagcloudchack()
            ' ///////////////////////////////////////////////////
            ' Selectrion with lPath and lHost
            Dim lSQL As String = "SELECT * " & _
                        " FROM [TAG_CLOUD_TAG] " & _
                        " WHERE [TAG_NAVIGATION_CONTEXT] = '" & lPath & "' " & _
                        " AND [TAG_CLOUD_TARGET] = '" & lHost & "' "
            Dim lDt As DataTable = mUser.Databasemanager.MakeDataTable(lSQL)
            Dim lRoles As String = lDt.Rows(0)("TAG_CLOUD_SITETYPE_ID").ToString
            Dim lTagcloudeID As String = lDt.Rows(0)("TAG_CLOUD_ID").ToString
            If lRoles = "LANDINGPAGE" Then
                Return TagcloudingBig(sender, e, lTagcloudeID)
            ElseIf lRoles = "WEBSITE" Then
                lShortlinkUrl = lDt.Rows(0)("TAG_LINK_URL").ToString
                lShortlinktext = "here &raquo;".ToString
                lShortlink = lShortlinktext & "|" & lShortlinkUrl
                Return lShortlink
            Else
                mRequest = "An error has occured in ASP.Net Case Statement. "
                mRequest &= "Host: " & lHost & lPath
            End If
            mRequest = lPath.ToString

        Catch ex As Exception
            mRequest = "Urlgetter: An error has occured"
            mRequest += ex.Message

        End Try

        Return mRequest

    End Function

    
End Class
