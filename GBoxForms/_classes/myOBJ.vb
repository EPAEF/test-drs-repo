Public Class myOBJ

    Private mFULL_PATH_ID As String = ""
    Private mFULL_PATH_GUID As String = ""
    Private mTOPIC_ID As String = ""
    Public Property TOPIC_ID() As String
        Get
            Return mTOPIC_ID
        End Get
        Set(ByVal value As String)
            mTOPIC_ID = value
        End Set
    End Property
    Private mOBJ_ID As String = ""
    Public Property OBJ_ID() As String
        Get
            Return mOBJ_ID
        End Get
        Set(ByVal value As String)
            mOBJ_ID = value
        End Set
    End Property
    Private mOBJ_TABLENAME As String = ""
    Public Property OBJ_TABLENAME() As String
        Get
            Return mOBJ_TABLENAME
        End Get
        Set(ByVal value As String)
            mOBJ_TABLENAME = value
        End Set
    End Property
    Private mOBJ_VersionNumber As String = ""
    Public Property OBJ_VersionNumber() As String
        Get
            Return mOBJ_VersionNumber
        End Get
        Set(ByVal value As String)
            mOBJ_VersionNumber = value
        End Set
    End Property

   

    Private mOBJ_DESCRIPTION As String = ""
    Public Property OBJ_DESCRIPTION() As String
        Get
            Return mOBJ_DESCRIPTION
        End Get
        Set(ByVal value As String)
            mOBJ_DESCRIPTION = value
        End Set
    End Property


    Private mREQUEST_TYPE_ID As String = ""
    Public Property REQUEST_TYPE_ID() As String
        Get
            Return mREQUEST_TYPE_ID
        End Get
        Set(ByVal value As String)
            mREQUEST_TYPE_ID = value
        End Set
    End Property

    Private mOBJ_CLASSIFICATION_ID As String = ""

    Public Property OBJ_CLASSIFICATION_ID() As String
        Get
            Return mOBJ_CLASSIFICATION_ID
        End Get
        Set(ByVal value As String)
            mOBJ_CLASSIFICATION_ID = value
        End Set
    End Property

    Private mOBJ_TABLENAME_KEY As String = ""

    Public Property OBJ_TABLENAME_KEY() As String
        Get
            Return mOBJ_TABLENAME_KEY
        End Get
        Set(ByVal value As String)
            mOBJ_TABLENAME_KEY = value
        End Set
    End Property

    Private mDATABASE_OBJ_Classification_ID As String = ""

    Public Property DATABASE_OBJ_Classification_ID() As String
        Get
            Return mDATABASE_OBJ_Classification_ID
        End Get
        Set(ByVal value As String)
            mDATABASE_OBJ_Classification_ID = value
        End Set
    End Property

    Private mHelpUrL As String

    Public Property HelpUrL() As String
        Get
            Return mHelpUrL
        End Get
        Set(ByVal value As String)
            mHelpUrL = value
        End Set
    End Property

    Private mOBJ_DISPLAY_NAME As String

    Public Property OBJ_DISPLAY_NAME() As String
        Get
            Return mOBJ_DISPLAY_NAME
        End Get
        Set(ByVal value As String)
            mOBJ_DISPLAY_NAME = value
        End Set
    End Property


    Private mLoad_Data_At_Startup As Boolean = False
    Public Property Load_Data_At_Startup() As Boolean
        Get
            Return mLoad_Data_At_Startup
        End Get
        Set(ByVal value As Boolean)
            mLoad_Data_At_Startup = value
        End Set
    End Property

    Private mLoaded_Firsttime As Boolean = False

    Public Property Loaded_Firsttime() As Boolean
        Get
            Return mLoaded_Firsttime
        End Get
        Set(ByVal value As Boolean)
            mLoaded_Firsttime = value
        End Set
    End Property
    Private mTexts As List(Of OBJ_TEXT)
    Public Property Texts As List(Of OBJ_TEXT)
        Get
            If mTexts Is Nothing Then mTexts = New List(Of OBJ_TEXT)
            Return mTexts
        End Get
        Set(ByVal value As List(Of OBJ_TEXT))
            mTexts = value
        End Set
    End Property

    

    Public Property FULL_PATH_ID As String
        Get
            Return mFULL_PATH_ID
        End Get
        Set(ByVal value As String)
            mFULL_PATH_ID = value
        End Set
    End Property

    Public Property FULL_PATH_GUID As String
        Get
            Return mFULL_PATH_GUID
        End Get
        Set(ByVal value As String)
            mFULL_PATH_GUID = value
        End Set
    End Property

End Class
