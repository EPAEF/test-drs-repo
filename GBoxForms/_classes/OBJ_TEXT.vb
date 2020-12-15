Public Class OBJ_TEXT
    Public Sub New()

    End Sub
    Public Sub New(ByVal lKeyname As String, ByVal lKeyValue As String, ByVal lLanguageCode As String, ByVal lLanguageDescription As String, ByVal lLanguagevalue As String, ByVal lobjVersionsnumber As String)
        mKEY_NAME = lKeyname
        mKEY_VALUE = lKeyValue
        mLANGUAGE_CODE = lLanguageCode
        mLANGUAGE_DESCRIPTION = lLanguageDescription
        mLANGUAGE_VALUE = lLanguagevalue
        mOBJ_VERSIONNUMBER = lobjVersionsnumber
    End Sub
    Private mKEY_NAME As String
    Public Property KEY_NAME As String
        Get
            Return mKEY_NAME
        End Get
        Set(ByVal value As String)
            mKEY_NAME = value
        End Set
    End Property
    Private mKEY_VALUE As String
    Public Property KEY_VALUE As String
        Get
            Return mKEY_VALUE
        End Get
        Set(ByVal value As String)
            mKEY_VALUE = value
        End Set
    End Property

    Private mOBJ_VERSIONNUMBER As String
    Public Property OBJ_VERSIONNUMBER As String
        Get
            Return mOBJ_VERSIONNUMBER
        End Get
        Set(ByVal value As String)
            mOBJ_VERSIONNUMBER = value
        End Set
    End Property

    Private mLANGUAGE_CODE As String
    Public Property LANGUAGE_CODE As String
        Get
            Return mLANGUAGE_CODE
        End Get
        Set(ByVal value As String)
            mLANGUAGE_CODE = value
        End Set
    End Property

    Private mLANGUAGE_DESCRIPTION As String
    Public Property LANGUAGE_DESCRIPTION As String
        Get
            Return mLANGUAGE_DESCRIPTION
        End Get
        Set(ByVal value As String)
            mLANGUAGE_DESCRIPTION = value
        End Set
    End Property
    Private mLANGUAGE_VALUE As String
    Public Property LANGUAGE_VALUE As String
        Get
            Return mLANGUAGE_VALUE
        End Get
        Set(ByVal value As String)
            mLANGUAGE_VALUE = value
        End Set
    End Property
    Private mNEW_ENTRY As String
    Public Property NEW_ENTRY As String
        Get
            Return mNEW_ENTRY
        End Get
        Set(ByVal value As String)
            mNEW_ENTRY = value
        End Set
    End Property


End Class
