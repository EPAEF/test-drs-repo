Imports System.Security
Imports System.DirectoryServices
Imports System.DirectoryServices.ActiveDirectory
Imports System.Security.Principal
Imports System.Security.Permissions
Imports System.Runtime.InteropServices
Imports System.Environment
Public Module modAuth
    Private Declare Auto Function LogonUser Lib "advapi32.dll" (ByVal lpszUsername As [String], _
      ByVal lpszDomain As [String], ByVal lpszPassword As [String], _
      ByVal dwLogonType As Integer, ByVal dwLogonProvider As Integer, _
      ByRef phToken As IntPtr) As Boolean

    Public Declare Auto Function CloseHandle Lib "kernel32.dll" (ByVal handle As IntPtr) As Boolean

    Public Function Authenticate(ByVal strDomain As String, ByVal strUser As String, ByVal strPass As String) As Boolean
        Dim tokenHandle As New IntPtr(0)
        Try
            Const LOGON32_PROVIDER_DEFAULT As Integer = 0
            Const LOGON32_LOGON_INTERACTIVE As Integer = 2
            tokenHandle = IntPtr.Zero
            'Call the LogonUser function to obtain a handle to an access token.
            Authenticate = LogonUser(strUser, strDomain, strPass, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, tokenHandle)
            If Not System.IntPtr.op_Equality(tokenHandle, IntPtr.Zero) Then
                CloseHandle(tokenHandle)
            End If
            Exit Function
        Catch ex As Exception
            Return False
        End Try
    End Function
End Module


