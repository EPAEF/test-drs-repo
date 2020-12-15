Public Class AuthorizationSet
    Private mImplementationtext As String = ""
    Public Property Implementationtext() As String
        Get
            Return mImplementationtext
        End Get
        Set(ByVal value As String)
            mImplementationtext = value
        End Set
    End Property
    Private mIsInAuthsetTable As Boolean
    Public Property IsInAuthsetTable() As Boolean
        Get
            Return mIsInAuthsetTable
        End Get
        Set(ByVal value As Boolean)
            mIsInAuthsetTable = value
        End Set
    End Property
    Private mApplication As String = ""
    Public Property Application() As String
        Get
            Return mApplication
        End Get
        Set(ByVal value As String)
            mApplication = value
        End Set
    End Property
    Private mApplicationtext As String = ""
    Public Property Applicationtext() As String
        Get
            Return mApplicationtext
        End Get
        Set(ByVal value As String)
            mApplicationtext = value
        End Set
    End Property
    Private mApplicationpart As String = ""
    Public Property Applicationpart() As String
        Get
            Return mApplicationpart
        End Get
        Set(ByVal value As String)
            mApplicationpart = value
        End Set
    End Property
    Private mApplicationPartText As String = ""
    Public Property ApplicationPartText() As String
        Get
            Return mApplicationPartText
        End Get
        Set(ByVal value As String)
            mApplicationPartText = value
        End Set
    End Property
    Private mCW_ID As String = ""
    Public Property CW_ID() As String
        Get
            Return mCW_ID
        End Get
        Set(ByVal value As String)
            mCW_ID = value
        End Set
    End Property
    Private mApplicationrole As String = ""
    Public Property Applicationrole() As String
        Get
            Return mApplicationrole
        End Get
        Set(ByVal value As String)
            mApplicationrole = value
        End Set
    End Property
    Private mApplicationroleText As String = ""
    Public Property ApplicationroleText() As String
        Get
            Return mApplicationroleText
        End Get
        Set(ByVal value As String)
            mApplicationroleText = value
        End Set
    End Property
    Private mState As String = ""
    Public Property State() As String
        Get
            Return mState
        End Get
        Set(ByVal value As String)
            mState = value
        End Set
    End Property
    Private mSubgroup As String = "ALL"
    Public Property Subgroup() As String
        Get
            Return mSubgroup
        End Get
        Set(ByVal value As String)
            mSubgroup = value
        End Set
    End Property
  
 
    Private mOrglevel_Value As String = ""

    Public Property Orglevel_Value() As String
        Get
            Return mOrglevel_Value
        End Get
        Set(ByVal value As String)
            mOrglevel_Value = value
        End Set
    End Property

    Private mOrglevel_ID As String = ""

    Public Property Orglevel_ID() As String
        Get
            Return mOrglevel_ID
        End Get
        Set(ByVal value As String)
            mOrglevel_ID = value
        End Set
    End Property

End Class
