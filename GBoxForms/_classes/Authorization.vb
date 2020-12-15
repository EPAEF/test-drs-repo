'------------------------------------------------------------------------------------------------------------
' Reference         : CR - BY-RZ04-CMT-27932 - Enhance GBOX with a database parametered download website
' Comment           : Class for Authorization functionality
' Added by          : Surendra Purav (CWID : EQIZU)
' Date              : 2013-11-26
'------------------------------------------------------------------------------------------------------------

Public Class Authorization

    Private mErrtext As String = ""
    Public Property ErrText() As String
        Get
            Return mErrtext
        End Get
        Set(ByVal value As String)
            mErrtext = value
        End Set
    End Property
    Private mUser As myUser
    Public Property User() As myUser
        Get
            Return mUser
        End Get
        Set(ByVal value As myUser)
            mUser = value
        End Set
    End Property
    Private mContext As HttpContext = Nothing
    Public Property Context() As HttpContext
        Get
            Return mContext
        End Get
        Set(ByVal value As HttpContext)
            mContext = value
        End Set
    End Property
    Private mImpersonate As String = ""
    Public Property Impersonate() As String
        Get
            Return mImpersonate
        End Get
        Set(ByVal value As String)
            mImpersonate = value
        End Set
    End Property

    Public Function Authenticate_User() As myUser
        If Context.User.Identity.Name = "" Then
            ErrText = "Can not identify user. Problems with persisting windows authentification. Activate windows persist security on the server."
        End If
        Dim lContextUser As String = ""
        If String.IsNullOrEmpty(Impersonate) Then
            lContextUser = Context.User.Identity.Name
        Else
            lContextUser = Impersonate
        End If
        If InStr(lContextUser, "\") <> 0 Then
            lContextUser = lContextUser.Split("\".ToCharArray)(1)
        End If
        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        pObjCurrentUsers.CONTEXT = Context
        If InStr(lContextUser, My.Computer.Name) <> 0 Then
            ErrText = "This user:" & lContextUser & " is not valid please log in at your network domain e.g. BYACCOUNT\USERNAME "
            Return Nothing
        End If
        User = pObjCurrentUsers.GetCurrentUserByCwId(lContextUser)
        If User Is Nothing Then
            ' ----------------------------------------------------------------------------------------
            ' Reference  : BY-RZ04 - CMT - 28633 - 110IM08222136,110IM08175309 - "Unknown User error in accessing GBOX"
            ' Comment    : Add Error information to user if he is not able able to login to GBox site
            ' Created by : EQIZU
            ' Date       : 06-NOV-2013
            ' ---------------------------------------------------------------------------------------
            ErrText = "Customer download: User Authentication failed " & vbCrLf
            Return Nothing
        End If
        Return User
    End Function

End Class
