Option Strict Off
Public Class Dynamic_View_Requester
    Inherits Dynamic_View_Controller
    Implements IDynamic_View_Controller
    Private mWherestring As String
    Public WithEvents mDatagrid As GridView
    Private mErrText As String
    Private mOBJ_Field_Type_Id As String
    Private mFieldInfoDataTable As DataTable
    Private m_KeyPath As String = ""

    Public Overrides Function BindView(ByRef lView As System.Web.UI.WebControls.View, ByVal lWithPaging As Boolean) As Boolean
        Dim lGrd = CType(lView.FindControl("grdDat"), Object)
        If Me.User.Current_OBJ.OBJ_CLASSIFICATION_ID <> "COMPOSITE_S_T_CHILD" Then
            '---------------------------------------------------------------------
            ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
            ' Comment   : Checkbox is placed instead of Select button
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-12-24
            lGrd.AutoGenerateSelectButton = False
            ' Reference END : CR ZHHR 1050708
            '---------------------------------------------------------------------
        Else
            lGrd.AutoGenerateSelectButton = False
        End If

        BindGrid(lGrd, lWithPaging)
    End Function
    Public Sub New(ByVal lUser As myUser)
        Me.User = lUser
    End Sub


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

    '---------------------------------------------------------------------------------------------------
    ' Reference         : CR 1026150 - GBOX COC - Problems with searching in DRS handbook 
    ' Comment           : Added function to get results for query with filter criteria
    ' Added by          : Surendra Purav (CWID : EQIZU)
    ' Date              : 2014-03-26
    '---------------------------------------------------------------------------------------------------
    Public Function GetResultsForGrid(ByRef lDA As DataTable, ByRef lsql As String, Optional ByVal lErrorFlag As Boolean = False) As Boolean
        lsql = Me.User.GBOXmanager.GetDisplayStatement(Me.User.Current_OBJ.OBJ_ID)
        If lsql = "" Then
            Return False
        End If
        'If error comes then send only column names
        If lErrorFlag Then
            'Do not make Columns names in upper case
            ''If lErrorFlag Then lsql = Replace(lsql.ToUpper, "SELECT", "SELECT TOP 0")

            If InStr(lsql, "SELECT") <> 0 Then lsql = Replace(lsql, "SELECT", "SELECT TOP 0")
            If InStr(lsql, "Select") <> 0 Then lsql = Replace(lsql, "Select", "SELECT TOP 0")
            If InStr(lsql, "select") <> 0 Then lsql = Replace(lsql, "select", "SELECT TOP 0")
        End If


        lDA = Me.User.Databasemanager.MakeDataTable(lsql)
        Return True
    End Function

    Public Function BindGrid(ByRef lDatagrid As GridView, ByVal lWithPaging As Boolean) As Boolean
        Try
            Dim lDA As DataTable = Nothing
            lDatagrid.EmptyDataText = ""
            Dim mdt As DataTable = Me.User.Databasemanager.MakeDataTable(Me.User.GBOXmanager.GetFieldSql(Me.User.Current_OBJ))
            If mdt.Rows.Count = 0 Then
                RaiseeventErrorMessage("CUSTOMIZE " & Me.User.Current_OBJ.OBJ_ID & "-FIELDS in OBJ_FIELDS")
                Return False
            End If
            If Not Me.Request.Params("VALUES") Is Nothing Then
                '---------------------------------------------------------------------------------------------------
                ' Reference         : CR - BY-RZ04-CMT-27979 - GBOX forms: clear GBOX Cockpit filter
                ' Comment           : Check if different tree node is selected disable the filter criteria (where clause)
                ' Added by          : Surendra Purav (CWID : EQIZU)
                ' Date              : 2013-10-23
                '---------------------------------------------------------------------------------------------------                
                If Not Request.Params("PATH") Is Nothing Then
                    Dim lpath As String
                    lpath = Request.Params("PATH")
                    'Check if object id from url is different than in currentobject
                    If lpath.Contains(Me.User.Current_OBJ.OBJ_ID) Then
                        Me.User.GBOXmanager.PathValues = Me.Request.Params("VALUES").ToString()
                    Else
                        'don't the append the filter criteria in where clause if there is node change
                        Me.User.GBOXmanager.PathValues = ""
                    End If
                End If
            Else
                Me.User.GBOXmanager.PathValues = ""
            End If

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1026150 - GBOX COC - Problems with searching in DRS handbook 
            ' Comment           : Display error essage when no records are returned after entering filter crieria
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-03-26
            '---------------------------------------------------------------------------------------------------
            'Clear Database errors
            Me.User.Databasemanager.ErrText = ""
            Me.User.GBOXmanager.ErrString = ""
            Dim lsql As String = ""
            If Not GetResultsForGrid(lDA, lsql) Then
                RaiseeventErrorMessage(Me.User.Databasemanager.ErrText & vbCrLf & Me.User.GBOXmanager.ErrString)
                Return False
            End If

            'Database error occured
            If lDA Is Nothing Then
                'Clear the filters and get results without search filter with error message to user
                Me.User.GBOXmanager.PathValues = ""
                If Not GetResultsForGrid(lDA, lsql, True) Then
                    RaiseeventErrorMessage(Me.User.Databasemanager.ErrText & vbCrLf & Me.User.GBOXmanager.ErrString)
                    Return False
                End If
                'Show error message to user
                Me.User.GBOXmanager.ErrString = "Your Query returns no results. Look if filter column and filter value matches "

            End If

            If lDA Is Nothing Then
                RaiseeventErrorMessage("datatable is nothing" & vbCrLf & Me.User.Databasemanager.ErrText & vbCrLf & Me.User.GBOXmanager.ErrString)
                Return False
            End If
            Dim ldt As DataTable = Me.User.Databasemanager.MakeDataTable(lsql)
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1026150 - GBOX COC - Problems with searching in DRS handbook 
            ' Comment           : Display error essage when no records are returned after entering filter crieria
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-03-26
            '---------------------------------------------------------------------------------------------------
            If ldt.Rows.Count = 0 Then
                Me.User.GBOXmanager.ErrString = "Your Query returns no results. Look if filter column and filter value matches "
            End If

            columns = New List(Of String)
            For Each c As DataColumn In lDA.Columns
                columns.Add(c.ColumnName)
            Next
            With lDatagrid
                .AllowPaging = lWithPaging
                .DataSource = lDA
                .PageSize = 20
                .DataBind()
            End With
            mDatagrid = lDatagrid
            Return True
        Catch ex As Exception
            If InStr(ex.Message, "Conversion failed when") = 0 Then
                mErrText &= "MyDynamicForm_Requester:BINGRID:" & ex.Message & vbCrLf & Me.User.Databasemanager.ErrText & vbCrLf & Me.User.GBOXmanager.ErrString
                RaiseeventErrorMessage(mErrText)
                Return False
            Else
                mErrText = "Problem with Form Filter: " & m_KeyPath
                RaiseeventErrorMessage(mErrText)
                Return True
            End If
        End Try
    End Function
    Public Overrides Function BindDetail(ByRef lDetailsview As DetailsView, ByRef lDataGrid As GridView, ByVal ispostback As Boolean, ByVal lNewRequest As Boolean, Optional ByVal lCopyRequest As Boolean = False) As Boolean
        Try
            m_KeyPath = ""
            Dim lWherekey As String = " Where "
            Dim lPathKey As String = "&VALUES="
            Dim mdt As DataTable = Me.User.Databasemanager.MakeDataTable(Me.User.GBOXmanager.GetFieldSql(Me.User.Current_OBJ))
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
                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                ' Comment   : Make the key fields non-editable in case of update or copy
                ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                ' Date      : 2019-04-01
                lDetailsview.Fields.Add(lTemplateFactory.DynamicField(r, lCopyRequest))
            Next
            If lTemplateFactory.ErrText <> "" Then
                RaiseeventErrorMessage(lTemplateFactory.ErrText)
                Return False
            End If
            Dim lsql As String = Me.User.GBOXmanager.GetDisplayStatement(Me.User.Current_OBJ.OBJ_ID, myGBoxManager.DisplayType.DetailsView)
            If Not lDataGrid Is Nothing Then
                If lDataGrid.Rows.Count = 0 Then
                    Return False
                End If
                If lDataGrid.SelectedRow Is Nothing Then
                    RaiseeventErrorMessage(" No Row Selected ")
                    Return False
                End If
                If InStr(lsql.ToUpper, " Where ".ToUpper) <> 0 Then
                    lsql = lsql & " AnD "
                    lWherekey = " AnD "
                End If
            End If
            If Me.User.GBOXmanager.KeyCollection.Count = 0 Then
                RaiseeventErrorMessage("No Keymember defined: Customize IsKeyMember for " & Me.User.Current_OBJ.OBJ_TABLENAME)
                Return False
            End If
            If lDetailsview.ToolTip = "" Then

                If lNewRequest Then

                Else
                    For Each lKey As myKeyObj In Me.User.GBOXmanager.KeyCollection
                        Dim lKeyValue As String = ""
                        If Not lDataGrid Is Nothing Then
                            If Not lDataGrid.SelectedRow Is Nothing Then
                                ' lDataGrid.Rows(lDataGrid.SelectedIndex).c()
                                If Not lDataGrid.SelectedRow.Cells(lKey.OrdinalPosition + 1) Is Nothing Then
                                    If lDataGrid.SelectedRow.Cells(lKey.OrdinalPosition + 1).Text <> "&nbsp;" Then

                                        lKeyValue = lDataGrid.SelectedRow.Cells(lKey.OrdinalPosition + 1).Text
                                        lKey.CurrentValue = lKeyValue
                                        'Notwehr transport irgendwie gehen die werte verloren
                                        lDetailsview.ToolTip &= lKey.Key_ID & "|" & lKeyValue.ToString & "~"

                                    End If
                                End If
                            End If
                        End If
                        '  Me.User.OBJ_Value = lKeyValue
                        lWherekey = lWherekey & lKey.Key_ID & "='" & lKeyValue & "' AND "
                        lPathKey = lPathKey & lKey.Key_ID & "|" & lKeyValue & "/"
                        lKey.CurrentValue = lKeyValue
                    Next
                    If lWherekey <> "" Then
                        lWherekey = lWherekey.Substring(0, lWherekey.Length - 5)
                    End If
                    mWherestring = lWherekey
                    lsql = lsql & lWherekey
                End If
            Else
                'Dann aus dem Tooltip lesen
                Dim lTip As Array = lDetailsview.ToolTip.Split("~")

                For Each lKey As myKeyObj In Me.User.GBOXmanager.KeyCollection
                    Dim lKeyValue As String = ""
                    For i = 0 To lTip.GetUpperBound(0)
                        Dim lPair As Array = lTip(i).ToString.Split("|")
                        If lPair(0).ToString = lKey.Key_ID Then
                            lKeyValue = lPair(1).ToString
                            lKey.CurrentValue = lKeyValue

                            lWherekey = lWherekey & lKey.Key_ID & "='" & lKeyValue & "' AND "
                            lPathKey = lPathKey & lKey.Key_ID & "|" & lKeyValue & "/"
                            lKey.CurrentValue = lKeyValue
                        End If
                    Next i
                Next
                If lWherekey <> "" Then
                    lWherekey = lWherekey.Substring(0, lWherekey.Length - 5)
                End If
                mWherestring = lWherekey
                lsql = lsql & lWherekey

            End If
            pVarSQL = lsql
            If lNewRequest Then lDetailsview.ChangeMode(DetailsViewMode.Insert)
            Dim ldt As DataTable = Me.User.Databasemanager.MakeDataTable(lsql)
            lDetailsview.DataSource() = ldt
            columns = New List(Of String)
            For Each c As DataColumn In ldt.Columns
                columns.Add(c.ColumnName)
            Next
            'CHECKLOCK
            If Not lNewRequest Then
                If Me.User.Current_OBJ.OBJ_CLASSIFICATION_ID <> "ADMINISTRATION" Then

                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELD FROM DB AND CODE
                    ' Comment           : Remove title from database and code
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 2014-11-26

                    Dim lCheckLockedSql As String = "select [Locked], " & Me.User.Current_OBJ.OBJ_TABLENAME & ".[CW_ID],IMPLEMENTATION_STATUS,[LOCKEDTIME],[SMTP_EMAIL],[last_name],[first_name] from " & Me.User.Current_OBJ.OBJ_TABLENAME & " left Join MDRS_USER on " & Me.User.Current_OBJ.OBJ_TABLENAME & ".CW_ID =MDRS_USER.CW_ID " & lWherekey & " And Locked IS not NULL"

                    ' Reference  END    : CR ZHHR 1035817

                    Dim lLockDt As DataTable = Me.User.Databasemanager.MakeDataTable(lCheckLockedSql)
                    If lLockDt Is Nothing Then
                        ' RaiseeventErrorMessage("Not able to check lockstate")
                        Return False
                    End If
                    If lLockDt.Rows.Count <> 0 Then

                        '---------------------------------------------------------------------------------------------------
                        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELD FROM DB AND CODE
                        ' Comment           : Remove title from database and code
                        ' Added by          : Milind Randive (CWID : EOJCH)
                        ' Date              : 2014-11-26

                        Dim lMessage As String = "This value is locked by USER: " & vbCrLf & vbCrLf & lLockDt.Rows(0)("first_name").ToString & " " & lLockDt.Rows(0)("last_name").ToString & " " & vbCrLf & vbCrLf & lLockDt.Rows(0)("SMTP_EMAIL").ToString & vbCrLf & vbCrLf & " with REQ_ID " & lLockDt.Rows(0)("LOCKED").ToString & "(" & lLockDt.Rows(0)("LOCKEDTIME").ToString & ")"

                        ' Reference  END    : CR ZHHR 1035817

                        RaiseeventLocked(lMessage)
                        lDetailsview.ToolTip = ""
                        Return False
                    End If


                End If
            End If

            lDetailsview.DataBind()
            m_KeyPath = lPathKey.Substring(0, lPathKey.Length - 1)
            Return True
        Catch ex As Exception
            If InStr(ex.Message, "Conversion failed when") = 0 Then
                mErrText &= "MyDynamicForm_Requester:BindDetail:" & ex.Message & vbCrLf & Me.User.Databasemanager.ErrText & vbCrLf & Me.User.GBOXmanager.ErrString
                RaiseeventErrorMessage(mErrText)
                Return False
            Else
                mErrText = "Problem with Form Filter: " & m_KeyPath
                RaiseeventErrorMessage(mErrText)
                Return True
            End If


        End Try
    End Function

    Public Sub RaiseErrorMessage(ByVal lText As String)
        RaiseeventErrorMessage(lText)
    End Sub
    Public Overrides Function GetKeyValue(ByVal lRequest As Object) As String

        Dim lFieldValue As String = ""
        Dim mdt As DataTable = GetFieldInfoDataTable()
        For Each r As DataRow In mdt.Rows
            If CBool(r("IsKeyMember").ToString) = True And r("OBJ_FIELD_TYPE_ID").ToString <> "VERSIONNUMBER" Then
                Dim lFieldname As String = r("OBJ_FIELD_ID").ToString

                Dim lRequestValue As String = Me.User.Databasemanager.GetRequestValueByFieldname(lRequest, r("DISPLAY_NAME").ToString)
                lFieldValue &= lFieldname & "~" & lRequestValue & "|"
            End If
        Next r

        If lFieldValue Is Nothing Then Return ""
        lFieldValue = Left(lFieldValue, lFieldValue.Length - 1)
        Return lFieldValue
    End Function


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




    Public Property OBJ_Field_Type_Id() As String
        Get
            Return mOBJ_Field_Type_Id
        End Get
        Set(ByVal value As String)
            mOBJ_Field_Type_Id = value
        End Set
    End Property


    Public Overrides Function GetFieldInfoDataTable() As DataTable
        Dim lSqldts As String = Me.User.Databasemanager.getFieldSQL()
        mFieldInfoDataTable = Me.User.Databasemanager.MakeDataTable(lSqldts)
        Return mFieldInfoDataTable
    End Function
    Private Sub mDatagrid_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mDatagrid.PageIndexChanged
        mDatagrid.DataBind()
    End Sub

    Public Overrides ReadOnly Property KeyPath As String
        Get
            Return m_KeyPath
        End Get

    End Property



End Class
