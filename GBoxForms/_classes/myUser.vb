Public Class myUser
    Public Enum RequestTypeOption
        unknown = 0
        insert = 1
        delete = 2
        update = 3
    End Enum
    Private mRequestType As RequestTypeOption = RequestTypeOption.update
    Public Function IsAuthorizedFor(ByVal lApplication As String, ByVal lApplicationPart As String, ByVal lRoleToTestFor As String) As Boolean
        Dim lAuth As String = mGboxmanager.Authorisation(lApplication, lApplicationPart, mCW_ID)
        If InStr(lAuth.ToUpper, lRoleToTestFor.ToUpper) <> 0 Then
            Return True
        End If
        Return mGboxmanager.IsGBoxAdmin(mCW_ID)
    End Function

    '---------------------------------------------------------------------------------------------------
    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: Add code for updating LAST_LOGON, LAST_LOGON_APPLICATION FIELDS
    ' Comment           : Add code for LAST_LOGON, LAST_LOGON_APPLICATION fields in table MDRS_USER
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 2014-12-12

    ''' <summary>
    ''' Reference         : ZHHR 1058919 - GBOX MGR OTT 2857: Rename table USER
    ''' Comment           : Rename [USER] table to MDRS_USER
    ''' There are too much changes for this table name change, so i will comment only at this location.
    ''' Added by          : Milind Randive (CWID : EOJCH)
    ''' Date              : 01-Jul-2016
    ''' </summary>
    ''' <param name="mCW_ID"></param>
    ''' <param name="m_App"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UserAccessStatus(ByVal mCW_ID As String, ByVal m_App As String) As Boolean
        Dim lSql As String = ""
        Dim lPackage As New List(Of String)

        lSql = "Update MDRS_USER set LAST_LOGON='" & DateTime.Now.ToString & "', LAST_LOGON_APPLICATION='" & m_App & "'  where [CW_ID]='" & mCW_ID & "'"
        lPackage.Add(lSql)
        Return Me.Databasemanager.ExecutePackage(lPackage)
    End Function
    ' Reference  END    : CR ZHHR 1035817

    Private mfirst_name As String
    Public Property first_name() As String
        Get
            Return mfirst_name
        End Get
        Set(ByVal value As String)
            mfirst_name = value
        End Set
    End Property
    Private mlast_name As String
    Public Property last_name() As String
        Get
            Return mlast_name
        End Get
        Set(ByVal value As String)
            mlast_name = value
        End Set
    End Property
    Private mTITLE As String
    Public Property TITLE() As String
        Get
            Return mTITLE
        End Get
        Set(ByVal value As String)
            mTITLE = value
        End Set
    End Property
    Private mSUBGROUP_ID As String
    Public Property SUBGROUP_ID() As String
        Get
            Return mSUBGROUP_ID
        End Get
        Set(ByVal value As String)
            mSUBGROUP_ID = value
        End Set
    End Property
    Private mAREA_ID As String
    Public Property AREA_ID() As String
        Get
            Return mAREA_ID
        End Get
        Set(ByVal value As String)
            mAREA_ID = value
        End Set
    End Property
    Private mUserRole As String
    Public Property UserRole() As String
        Get
            Return mUserRole
        End Get
        Set(ByVal value As String)
            mUserRole = value
        End Set
    End Property
    Private mCW_ID As String
    Public Property CW_ID() As String
        Get

            Return mCW_ID
        End Get
        Set(ByVal value As String)
            mCW_ID = value
            If InStr(mCW_ID, "\") <> 0 Then
                mCW_ID = mCW_ID.Split(CChar("\"))(1).ToString
            End If

        End Set
    End Property

    Private mSMTP_EMAIL As String
    Public Property SMTP_EMAIL() As String
        Get
            Return mSMTP_EMAIL
        End Get
        Set(ByVal value As String)
            mSMTP_EMAIL = value
        End Set
    End Property

    Private mCurrent_Request_ID As String

    Public Property Current_Request_ID() As String
        Get
            If mCurrent_Request_ID = "" Then
                mCurrent_Request_ID = Me.GBOXmanager.GetGBOXId()
            End If
            Return mCurrent_Request_ID

        End Get
        Set(ByVal value As String)
            If value = "" Then
                value = Me.GBOXmanager.GetGBOXId()
            End If
            mCurrent_Request_ID = value
        End Set
    End Property

    Private mCurrent_OBJ As myOBJ

    Public Property Current_OBJ() As myOBJ
        Get
            Return mCurrent_OBJ
        End Get
        Set(ByVal value As myOBJ)
            mCurrent_OBJ = value
        End Set
    End Property




    Private mCheckBoxList As List(Of ListItem)

    Public Property CheckBoxList() As List(Of ListItem)
        Get
            Return mCheckBoxList
        End Get
        Set(ByVal value As List(Of ListItem))
            mCheckBoxList = value
        End Set
    End Property

    Private mOBJ_Value As String

    Public Property OBJ_Value() As String
        Get
            Return mOBJ_Value
        End Get
        Set(ByVal value As String)
            mOBJ_Value = value
        End Set
    End Property

    Private mOBJ_FIELD_ID As String

    Public Property OBJ_FIELD_ID() As String
        Get
            Return mOBJ_FIELD_ID
        End Get
        Set(ByVal value As String)
            mOBJ_FIELD_ID = value
        End Set
    End Property

    Private mCURRENT_OBJ__TYPE As String

    Public Property CURRENT_OBJ__TYPE() As String
        Get
            Return mCURRENT_OBJ__TYPE
        End Get
        Set(ByVal value As String)
            mCURRENT_OBJ__TYPE = value
        End Set
    End Property

    Private mQuery As Boolean = False

    Public Property Query() As Boolean
        Get
            Return mQuery
        End Get
        Set(ByVal value As Boolean)
            mQuery = value
        End Set
    End Property

    Private mEditMode As Boolean = False

    Public Property EditMode() As Boolean
        Get
            Return mEditMode
        End Get
        Set(ByVal value As Boolean)
            mEditMode = value
        End Set
    End Property

    Private mGboxmanager As myGBoxManager

    Public ReadOnly Property GBOXmanager() As myGBoxManager
        Get
            Return mGboxmanager
        End Get

    End Property

    Private mDatabasemanager As mySQLDBMANAGER

    Public ReadOnly Property Databasemanager() As mySQLDBMANAGER
        Get
            Return mDatabasemanager
        End Get

    End Property


    Public Sub New()
        mDatabasemanager = New mySQLDBMANAGER
        mDatabasemanager.User = Me
        mGboxmanager = New myGBoxManager()

    End Sub

    Private mRequestText As String

    Public Property RequestText() As String
        Get
            Return mRequestText
        End Get
        Set(ByVal value As String)
            mRequestText = value
        End Set
    End Property

    Private mRollbackPack As New List(Of String)

    Public Property RollbackPack() As List(Of String)
        Get
            Return mRollbackPack
        End Get
        Set(ByVal value As List(Of String))
            mRollbackPack = value
        End Set
    End Property

    Private mPackage As New List(Of String)

    Public Property Package() As List(Of String)
        Get
            Return mPackage
        End Get
        Set(ByVal value As List(Of String))
            mPackage = value
        End Set
    End Property



    Private mCurrentSql As String

    Public Property Current_SQL() As String
        Get
            Return mCurrentSql
        End Get
        Set(ByVal value As String)
            mCurrentSql = value
        End Set
    End Property



    Public Property RequestType As RequestTypeOption
        Get
            Return mRequestType
        End Get
        Set(ByVal value As RequestTypeOption)
            mRequestType = value
        End Set
    End Property

    '---------------------------------------------------------------------------------------------------
    ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
    ' Comment           : Added filter property
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 17-Feb-2016
    Private mCocFilter As String

    Public Property CocFilter() As String
        Get
            Return mCocFilter
        End Get
        Set(ByVal value As String)
            mCocFilter = value
        End Set
    End Property

    Private mSystemIds As String
    Public Property SystemIds As String
        Get
            Return mSystemIds
        End Get
        Set(ByVal value As String)
            mSystemIds = value
        End Set
    End Property

    ' Reference End     : ZHHR 1053017

End Class
