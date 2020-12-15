Option Strict Off
Public Class Dynamic_View_Documentator
    Inherits Dynamic_View_Controller
    Implements IDynamic_View_Controller
    Private mUser As myUser
    Private mWherestring As String
    Private mErrText As String

    Public Property ErrText() As String
        Get
            Return mErrText
        End Get
        Set(ByVal value As String)
            mErrText &= value
        End Set
    End Property
    Public Overrides Property WhereString() As String
        Get
            Return mWherestring
        End Get
        Set(ByVal value As String)
            mWherestring = value
        End Set
    End Property
    Private mFieldInfoDataTable As DataTable
    Public Overrides Function GetFieldInfoDataTable() As DataTable
        Dim lSqldts As String = Me.User.Databasemanager.getFieldSQL()
        mFieldInfoDataTable = Me.User.Databasemanager.MakeDataTable(lSqldts)
        Return mFieldInfoDataTable
    End Function
    Public Overrides Function GetWhereString(ByVal lRequest As Object) As String
        Dim lWhereString As String = " where "
        Dim mdt As DataTable = GetFieldInfoDataTable()
        For Each r As DataRow In mdt.Rows
            If CBool(r("IsKeyMember").ToString) = True And r("OBJ_FIELD_TYPE_ID").ToString <> "VERSIONNUMBER" Then
                Dim lRequestValue As String = Me.User.Databasemanager.GetRequestValueByFieldname(lRequest, r("DISPLAY_NAME").ToString)
                lWhereString = lWhereString & r("OBJ_FIELD_ID").ToString & "='" & lRequestValue & "' and "
            End If
        Next r
        lWhereString = lWhereString.Substring(0, lWhereString.Length - 4)
        Return lWhereString
    End Function
     Private mOBJ_Field_Type_Id As String

    Public Property OBJ_Field_Type_Id() As String
        Get
            Return mOBJ_Field_Type_Id
        End Get
        Set(ByVal value As String)
            mOBJ_Field_Type_Id = value
        End Set
    End Property

    Public Overrides Function BindDetail(ByRef lDetailsview As System.Web.UI.WebControls.DetailsView, ByRef lDataGrid As System.Web.UI.WebControls.GridView, ByVal ispostback As Boolean, ByVal lNewRequest As Boolean, Optional ByVal lCopyRequest As Boolean = False) As Boolean
        Try
            If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
            pObjCurrentUsers.CONTEXT = CONTEXT
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
            End If
            Me.User = mUser
            lDataGrid.AutoGenerateSelectButton = True
            Dim lWherekey As String = " Where "
            mUser.Current_OBJ = mUser.GBOXmanager.SetCurrentObj("OBJ_DOCUMENTATION", mUser)
            Dim mdt As DataTable = Me.User.Databasemanager.MakeDataTable(Me.User.GBOXmanager.GetFieldSql(mUser.Current_OBJ))
            If mdt Is Nothing Then
                RaiseeventErrorMessage(Me.User.Databasemanager.ErrText)
                Return False
            End If
            If mdt.Rows.Count = 0 Then
                RaiseeventErrorMessage(Me.User.Databasemanager.ErrText)
                Return False
            End If
            lDetailsview.Fields.Clear()
            Dim lTemplateFactory As New myTemplateFactory(Me.User)
            lTemplateFactory.Requestform = Requestform
            For Each r As DataRow In mdt.Rows
                lDetailsview.Fields.Add(lTemplateFactory.DynamicField(r))

            Next
            If lTemplateFactory.ErrText <> "" Then
                RaiseeventErrorMessage(lTemplateFactory.ErrText)
                Return False
            End If
            Dim lsql As String = Me.User.GBOXmanager.GetDisplayStatement(Me.User.Current_OBJ.OBJ_ID, myGBoxManager.DisplayType.DetailsView)

            If lDataGrid.SelectedRow Is Nothing Then
                RaiseeventErrorMessage(" No Row Selected ")
                Return False
            End If
            If Not lDataGrid.SelectedRow Is Nothing Then
                If InStr(lsql.ToUpper, " Where ".ToUpper) = 0 Then
                    lsql = lsql & " where "
                Else
                    lsql = lsql & " and "
                End If
                If Me.User.GBOXmanager.KeyCollection.Count = 0 Then
                    RaiseeventErrorMessage("No Keymember defined: Customize IsKeyMember for " & Me.User.Current_OBJ.OBJ_TABLENAME)
                    Return False
                End If
                For Each lKey As myKeyObj In Me.User.GBOXmanager.KeyCollection
                    If lDataGrid.SelectedRow.Cells(lKey.OrdinalPosition).Text <> "&nbsp;" Then
                        lsql = lsql & lKey.Key_ID & "='" & lDataGrid.SelectedRow.Cells(lKey.OrdinalPosition + 1).Text & "' AND "
                        Me.User.OBJ_Value = lDataGrid.SelectedRow.Cells(lKey.OrdinalPosition).Text
                        lWherekey = lWherekey & lKey.Key_ID & "='" & lDataGrid.SelectedRow.Cells(lKey.OrdinalPosition).Text & "' AND "
                        lKey.CurrentValue = lDataGrid.SelectedRow.Cells(lKey.OrdinalPosition).Text
                    End If
                Next
                lsql = lsql.Substring(0, lsql.Length - 5)

                lWherekey = lWherekey.Substring(0, lWherekey.Length - 5)
                mWherestring = lWherekey
            End If
            lDetailsview.DataSource() = Me.User.Databasemanager.MakeDataTable(lsql)
            lDetailsview.DataBind()
            Return True
        Catch ex As Exception
            mErrText &= "MyDynamicForm_Requester:BindDetail:" & ex.Message & vbCrLf & Me.User.Databasemanager.ErrText & vbCrLf & Me.User.GBOXmanager.ErrString
            RaiseeventErrorMessage(mErrText)
            Return False
        End Try
        Return True
    End Function

    Public ReadOnly Property CONTEXT()
        Get
            Return pObjCurrentUsers.CONTEXT
        End Get

    End Property

    Public Sub New(ByVal lUser As myUser)
        mUser = lUser
        mButtonlist = New List(Of String)

    End Sub
    Public Overrides Function BindView(ByRef lView As View, ByVal lWithPages As Boolean) As Boolean
        Dim lSql As String = ""
        Dim lGrd = CType(lView.FindControl("grdDat"), Object)
        Select Case mUser.Current_OBJ.OBJ_CLASSIFICATION_ID
            Case "DOCUMENTATION"
                lSql = "SELECT  "
                lSql = lSql & "OBJ_ID,TOPIC_ID, OBJ_DOCUMENTATION_FIELD_NAME AS name"
                lSql = lSql & ",OBJ_DOCUMENTATION_FIELD_VALUE as value "
                lSql = lSql & "FROM OBJ_DOCUMENTATION   where Topic_ID ='" & TOPIC_ID & "' AND OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "' Order by RANK"

            Case "COMPOSITE_S_T_CHILD"
                lGrd.AutoGenerateSelectButton = True
                lSql = mUser.GBOXmanager.GetDisplayStatement(mUser.Current_OBJ.OBJ_ID)
            Case Else
                lGrd.AutoGenerateSelectButton = False
                lSql = mUser.GBOXmanager.GetDisplayStatement(mUser.Current_OBJ.OBJ_ID)
        End Select

        lGrd.Datasource = mUser.Databasemanager.MakeDataTable(lSql)
        lGrd.databind()
        If mUser.Databasemanager.ErrText <> "" Then
            RaiseeventErrorMessage(mUser.Databasemanager.ErrText)
        End If
    End Function

    Private mButtonlist As List(Of String)

    Public Property Buttonlist() As List(Of String)
        Get
            Return mButtonlist
        End Get
        Set(ByVal value As List(Of String))
            mButtonlist = value
        End Set
    End Property

End Class
