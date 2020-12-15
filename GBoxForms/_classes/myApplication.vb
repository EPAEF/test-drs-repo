Public Class myApplication
    Private mAuthorizationsets As List(Of AuthorizationSet)
    Private mApplicationname As String
    Private mSMEText As String = ""
    Private mImlementationtext As String = ""
    Private mImplementationpack As List(Of String)
    Private mRollbackpack As List(Of String)
    Private mImplementationblock As String
    Public Property RollbackPack() As List(Of String)
        Get
            If mRollbackpack Is Nothing Then mRollbackpack = New List(Of String)
            Return mRollbackpack
        End Get
        Set(ByVal value As List(Of String))
            mRollbackpack = value
        End Set
    End Property
    Public Property Implementationblock() As String
        Get
            Return mImplementationblock
        End Get
        Set(ByVal value As String)
            mImplementationblock = value
        End Set
    End Property
    Public Property ImplementationPack() As List(Of String)
        Get
            If mImplementationpack Is Nothing Then mImplementationpack = New List(Of String)
            Return mImplementationpack
        End Get
        Set(ByVal value As List(Of String))
            mImplementationpack = value
        End Set
    End Property
    Public Property Implementationtext() As String
        Get
            Return mImlementationtext
        End Get
        Set(ByVal value As String)
            mImlementationtext = value
        End Set
    End Property
    Public Property Authorizationsets() As List(Of AuthorizationSet)
        Get
            If mAuthorizationsets Is Nothing Then mAuthorizationsets = New List(Of AuthorizationSet)
            Return mAuthorizationsets
        End Get
        Set(ByVal value As List(Of AuthorizationSet))
            mAuthorizationsets = value
        End Set
    End Property
    Public Property Applicationname() As String
        Get
            Return mApplicationname
        End Get
        Set(ByVal value As String)
            mApplicationname = value
        End Set
    End Property
    Public Property SmeText() As String
        Get
            Return mSMEText
        End Get
        Set(ByVal value As String)
            mSMEText = value
        End Set
    End Property
End Class

