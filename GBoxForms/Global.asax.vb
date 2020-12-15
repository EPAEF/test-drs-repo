Imports System.Web.SessionState

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Private mTAG As GBoxForms.TagcloudIndexController

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
    End Sub
    ' Reference         : CR ZHHR 1036254 - GBOX COC: New Concept for Landing Page
    ' Comment           : Added Global.asax file for building landing page concept
    '                     and redirecting the pages from a single point.
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 2015-01-12
    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)

        Dim lPath As String = Context.Request.FilePath.ToString()
        lPath = lPath.Replace("/", "")
        lPath = lPath.ToUpper

        If lPath.ToUpper.Contains(".") Then
            Return
        Else
            HttpContext.Current.RewritePath("pagehandler.aspx?obj=" + lPath, False)
        End If

    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs
        Response.Write("test")
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

End Class