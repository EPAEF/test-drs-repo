Option Strict Off
Public Class myUsers
    Private mContext As Object

    Public Property CONTEXT() As Object
        Get
            Return mContext
        End Get
        Set(ByVal value As Object)
            mContext = value
        End Set
    End Property
    Private mGBOX_Users As List(Of myUser)
    Public ReadOnly Property GBOX_Users() As List(Of myUser)
        Get
            Return mGBOX_Users
        End Get
    End Property
    Public Sub New()
        mGBOX_Users = New List(Of myUser)
    End Sub

    Public Function GetCurrentUserByCwId(ByVal lCW_ID As String) As myUser
        If InStr(lCW_ID, "\") <> 0 Then
            lCW_ID = lCW_ID.Split("\")(1)
        End If
        'lCW_ID = "GDROY"
        For Each lUser As myUser In mGBOX_Users
            If lUser.CW_ID.ToUpper = lCW_ID.ToUpper Then
                Return lUser
            End If
        Next
        Dim lGBoxmanager As New myGBoxManager
        Dim lNewUser As myUser = lGBoxmanager.IsQualifiedUser(lCW_ID)
        If Not lNewUser Is Nothing Then
            mGBOX_Users.Add(lNewUser)
        End If
        Return lNewUser
    End Function
    Public Sub RemoveUser(ByVal lCW_ID As String)
        Dim lUser As myUser = GetCurrentUserByCwId(lCW_ID)
        mGBOX_Users.Remove(lUser)
    End Sub

    Private mAllUsers As String

    Public Property AllUsers() As String
        Get
            Dim lStr As String = ""
            For Each lUser As myUser In mGBOX_Users
                lStr = lStr & lUser.CW_ID & vbCrLf
            Next
            Return lStr
        End Get
        Set(ByVal value As String)
            mAllUsers = value
        End Set
    End Property

End Class
