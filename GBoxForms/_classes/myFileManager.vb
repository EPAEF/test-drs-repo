Option Strict Off
Public Class myFileManager : Inherits Page
    Private mStreamWriter As System.IO.StreamWriter

    Public Sub New(ByVal lFilename As String)
        mStreamWriter = New System.IO.StreamWriter(Server.MapPath(lFilename))
        mFilename = lFilename
    End Sub
    Public Sub Writeline(ByVal lstrLine As String)
        mStreamWriter.WriteLine(lstrLine)
    End Sub
    Public Sub SendFile(ByRef lResponse As Object)
        mStreamWriter.Close()
        With lResponse
            .ClearContent()
            .Clear()
            .ContentType = mContentType
            .AppendHeader("Content-Disposition", "attachment; filename=" & mFilename)
            .WriteFile(Server.MapPath(mFilename))
            .Flush()
            .Close()
        End With
    End Sub

    Private mFilename As String

    Public Property Filename() As String
        Get
            Return mFilename
        End Get
        Set(ByVal value As String)
            mFilename = value
        End Set
    End Property

    Private mContentType As String

    Public Overloads Property ContentType() As String
        Get
            Return mContentType
        End Get
        Set(ByVal value As String)
            mContentType = value
        End Set
    End Property

    Private mAppendHeader As String

    Public Property AppendHeader() As String
        Get
            Return mAppendHeader
        End Get
        Set(ByVal value As String)
            mAppendHeader = value
        End Set
    End Property

End Class
