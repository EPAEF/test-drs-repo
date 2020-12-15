Public Class myRequestedUser
    Private mAuthorizationsets As List(Of AuthorizationSet)
    Private mCW_ID As String
    Private mClientText As String
    Public Property Authorizationsets() As List(Of AuthorizationSet)
        Get
            If mAuthorizationsets Is Nothing Then mAuthorizationsets = New List(Of AuthorizationSet)
            Return mAuthorizationsets
        End Get
        Set(ByVal value As List(Of AuthorizationSet))
            mAuthorizationsets = value
        End Set
    End Property
    Public Property CW_ID() As String
        Get
            Return mCW_ID
        End Get
        Set(ByVal value As String)
            mCW_ID = value
        End Set
    End Property
    Public Property Clienttext() As String
        Get
            Return mClientText
        End Get
        Set(ByVal value As String)
            mClientText = value
        End Set
    End Property
End Class
