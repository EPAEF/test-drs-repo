Imports Microsoft.VisualBasic
Imports System.Xml.Serialization
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Partial Public Class DownLoadMarty
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        pObjCurrentUsers.Context = Context
        lblCurrentUser.Text = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name).CW_ID
        If Context.User.Identity.Name = "" Then
            lblCurrentUser.Text = "persist security info is off: My.User.Text is empty "
        End If
    End Sub
    Public Function SaveBlobAsFile(ByVal m_conn As SqlConnection, ByVal lFilename As String) As Boolean
        Dim commGetBlob As New SqlCommand("Select FILE_BINARY from MDRS_History where  [filename] ='" & lFilename & "' And IsCurrentVersion=1", m_conn)
        Dim fs As FileStream
        Try
            Dim arrGetBlob() As Byte = CType(commGetBlob.ExecuteScalar(), Byte())
            If arrGetBlob Is Nothing Then
                lblCurrentUser.Text = "no file available..."
                Return False
            End If
            Response.ContentType = "EXE"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & lFilename)
            Dim bw As New BinaryWriter(Response.OutputStream)
            bw.Write(arrGetBlob)
            bw.Flush()
            bw.Close()
            arrGetBlob = Nothing
            bw = Nothing

            Response.Flush()
            'Response.End()
            lblError.Text = "The MARTY Download was succesful."

            Return True
        Catch ex As Exception
            lblError.Text = ex.Message
            Return False
        Finally
            fs = Nothing
            GC.Collect()
        End Try
    End Function



    Private Function GetLastMarty_Version() As Boolean
        Try
            Dim ldatabasmanager As New mySQLDBMANAGER()
            'Dim lPath As String = Application.StartupPath & "\"
            Dim lsqlCn As SqlConnection = ldatabasmanager.cnSQL
            'lsqlCn.Open()
            If SaveBlobAsFile(lsqlCn, "Marty.EXE") Then
                lblError.Text = "The MARTY Download was succesful."
            End If
            'lsqlCn.Close()


            Return True
        Catch ex As Exception
            lblError.Text = ex.Message
            Return False
        End Try
    End Function



    Protected Sub cmdDownload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDownload.Click
        GetLastMarty_Version()
    End Sub

    
    Protected Sub cmdDownload0_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDownload0.Click
        Try
            Dim ldatabasmanager As New mySQLDBMANAGER()
            'Dim lPath As String = Application.StartupPath & "\"
            Dim lsqlCn As SqlConnection = ldatabasmanager.cnSQL
            'lsqlCn.Open()
            If SaveBlobAsFile(lsqlCn, "GBOX_MANAGER.exe") Then
                lblError.Text = "The GBOXMANAGER Download was succesful."
            End If
            'lsqlCn.Close()



        Catch ex As Exception
            lblError.Text = ex.Message

        End Try
    End Sub
    '---------------------------------------------------------------------------------------
    ' Reference         : CR ZHHR 1038319 - GBOX Enhance Download Page for Previous Versions
    ' Comment           : User can download previous version of Marty and GBOX Manager
    ' Added by          : Pratyusa Lenka (CWID : EOJCG)
    ' Date              : 2015-02-05
    Protected Sub btnDownloadPreviousMarty_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDownloadPreviousMarty.Click
        Try
            Dim ldatabasmanager As New mySQLDBMANAGER()
            Dim lsqlCn As SqlConnection = ldatabasmanager.cnSQL
            If SaveBlobAsFileForPreviousVersion(lsqlCn, "Marty.EXE") Then
                lblError.Text = "The MARTY Download was succesful."
            End If
        Catch ex As Exception
            lblError.Text = ex.Message
        End Try
    End Sub

    Protected Sub btnDownloadPreviousGBOXMGR_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDownloadPreviousGBOXMGR.Click
        Try
            Dim ldatabasmanager As New mySQLDBMANAGER()
            Dim lsqlCn As SqlConnection = ldatabasmanager.cnSQL
            If SaveBlobAsFileForPreviousVersion(lsqlCn, "GBOX_MANAGER.exe") Then
                lblError.Text = "The GBOXMANAGER Download was succesful."
            End If
        Catch ex As Exception
            lblError.Text = ex.Message

        End Try
    End Sub

    Private Function SaveBlobAsFileForPreviousVersion(ByVal m_conn As SqlConnection, ByVal lFilename As String) As Boolean
        Dim commGetBlob As New SqlCommand("SELECT FILE_BINARY FROM MDRS_History WHERE FILEVERSION = (SELECT MAX(FILEVERSION) FROM MDRS_History WHERE [FILENAME] ='" & lFilename & "' AND ISCURRENTVERSION = 0)", m_conn)
        Dim fs As FileStream
        Try
            Dim arrGetBlob() As Byte = CType(commGetBlob.ExecuteScalar(), Byte())
            If arrGetBlob Is Nothing Then
                lblCurrentUser.Text = "no file available..."
                Return False
            End If
            Response.ContentType = "EXE"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & lFilename)
            Dim bw As New BinaryWriter(Response.OutputStream)
            bw.Write(arrGetBlob)
            bw.Flush()
            bw.Close()
            arrGetBlob = Nothing
            bw = Nothing

            Response.Flush()

            Return True
        Catch ex As Exception
            lblError.Text = ex.Message
            Return False
        Finally
            fs = Nothing
            GC.Collect()
        End Try
    End Function
    ' Reference  END    : CR ZHHR 1038319
    '---------------------------------------------------------------------------------------
End Class