Option Strict Off

Public Class Dynamic_View_Reporter
    Inherits Dynamic_View_Controller
    Implements IDynamic_View_Controller
    Private mUser As myUser
    Private mWherestring As String
    Public Overrides Property WhereString() As String
        Get
            Return mWherestring
        End Get
        Set(ByVal value As String)
            mWherestring = value
        End Set
    End Property
    Public Overrides Function BindDetail(ByRef lDetailsview As DetailsView, ByRef lDataGrid As GridView, ByVal ispostback As Boolean, ByVal lNewRequest As Boolean, Optional ByVal lCopyRequest As Boolean = False) As Boolean
        Try
            Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable(mUser.GBOXmanager.GetFieldSql(mUser.Current_OBJ))
            If mdt Is Nothing Then
                RaiseeventErrorMessage(mUser.Databasemanager.ErrText)
                Return False
            End If
            If mdt.Rows.Count = 0 Then
                RaiseeventErrorMessage(mUser.Databasemanager.ErrText)
                Return False
            End If
            lDetailsview.Fields.Clear()
            Dim lTemplateFactory As New myTemplateFactory(mUser)
            lTemplateFactory.Requestform = Requestform
            For Each r As DataRow In mdt.Rows
                lDetailsview.Fields.Add(lTemplateFactory.DynamicField(r))
            Next
            If lTemplateFactory.ErrText <> "" Then
                RaiseeventErrorMessage(lTemplateFactory.ErrText)
                Return False
            End If
            Dim lsql As String = mUser.GBOXmanager.GetDisplayStatement(mUser.Current_OBJ.OBJ_ID, myGBoxManager.DisplayType.DetailsView)


            lDetailsview.DataSource() = mUser.Databasemanager.MakeDataTable(lsql)
            lDetailsview.DataBind()


            Return True
        Catch ex As Exception
            mErrText &= "MyDynamicForm_Requester:BindDetail:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            RaiseeventErrorMessage(mErrText)
            Return False
        End Try
    End Function



    Public Sub New(ByVal lUser As myUser)
        mUser = lUser

    End Sub
    Public Overrides Function BindView(ByRef lView As View, ByVal lWithPages As Boolean) As Boolean
        Dim ldvQuery = lView.FindControl("dvQuery")
        Dim lGrd As GridView = lView.FindControl("grdQuery")
        lGrd.DataSource = Nothing
        lGrd.DataBind()
        If EnableQuery(ldvQuery) Then
            If Not Me.Request Is Nothing Then
                Requestform = Me.Request.Form
            End If
            'Return Showquery(lGrd)
            Return True

        End If
    End Function

    Public Function EnableQuery(ByRef ldvQuery As DetailsView) As Boolean
        Try
            ldvQuery.Fields.Clear()

            Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(mUser.GBOXmanager.GetFieldSql(mUser.Current_OBJ))
            If ldt.Rows.Count = 0 Then
                mUser.GBOXmanager.SetCurrentObj(mUser.Current_OBJ.OBJ_ID, mUser)

                ' gk 22.09.2011
                If mUser.Current_OBJ.DATABASE_OBJ_Classification_ID.ToUpper <> "ParameterFreeView".ToUpper Then
                    ErrorInfo("CUSTOMIZE " & mUser.Current_OBJ.OBJ_ID & "-FIELDS in OBJ_FIELDS")
                    Return False
                End If
            End If
            Dim lTemplateFactory As New myTemplateFactory(mUser)
            lTemplateFactory.Requestform = Requestform

            For Each r As DataRow In ldt.Rows
                ldvQuery.Fields.Add(lTemplateFactory.DynamicField(r))
            Next
            If lTemplateFactory.ErrText <> "" Then
                ErrorInfo(lTemplateFactory.ErrText)
                Return False
            Else
                ldvQuery.DataBind()
                ldvQuery.ChangeMode(DetailsViewMode.Insert)
                RaiseEventimgQuery(True)
                Return True
            End If
        Catch ex As Exception
            mErrText &= "MyDynamicForm_Reporter:EnableQuery:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            ErrorInfo(mErrText)
            Return False
        End Try
    End Function
    Public Overrides Function Showquery(ByRef lgrdQuery As GridView, Optional ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs = Nothing) As Boolean
        Try
            ' gk 22.09.2011
            lgrdQuery.EmptyDataText = "No Data found"
            Dim lDefSQL As String = "select 'Select ' + FieldList +' from ' + Query_String_SQL+' ' + Where_Condition+' Order by ' + Order_By_Condition as Statement from DATABASE_OBJ where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "'"
            Dim mdt As Object = mUser.Databasemanager.MakeDataTable(lDefSQL)
            Dim lsql As String = mdt.Rows(0)("Statement").ToString.ToUpper
            Dim lSqldts As String = mUser.Databasemanager.getFieldSQL()
            mdt = mUser.Databasemanager.MakeDataTable(lSqldts)

            lsql = ReplaceFixedParams(lsql)
            For Each r In mdt.Rows
                Dim lValue As String = ""
                If Not Requestform Is Nothing Then
                    lValue = mUser.Databasemanager.GetRequestFormValuebyId(Requestform, r("DISPLAY_NAME").ToString, "dvQuery").Trim
                End If
                'Replace Dynamic Vars
                'If parameter has no values then replace Parameters 
                lsql = ReplaceParams(lValue, r("OBJ_Field_ID").ToString, lsql)
            Next r
            lsql = lsql.Replace("  ", " ")
            lsql = lsql.Replace(" AND ORDER", " ORDER")
            If lsql.EndsWith("WHERE ") Then
                lsql = lsql.Replace("WHERE ", "")
            End If
            lsql = lsql.Replace(" WHERE ORDER", " ORDER")
            Dim lStr As String = "Select *,DATABASE_OBJ.OBJ_ID from Database_def left Join DATABASE_OBJ on DATABASE_OBJ.Database_ID = DATABASE_DEF.Database_ID" & _
             " where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "'"
            Dim ldt As DataTable
            ldt = mUser.Databasemanager.MakeDataTable(lStr)
            Dim lCurrentDatabaseDriver As String = ldt.Rows(0)("Driver").ToString
            Select Case lCurrentDatabaseDriver
                'Case "OracleConnection"
                '    If InStr(lsql.ToUpper, " WHERE ") <> 0 Then
                '        lsql = lsql.Replace("ORDER BY", " AND ROWNUM <= 800 ORDER BY")
                '    Else
                '        lsql = lsql.Replace("ORDER BY", " WHERE ROWNUM <= 800 ORDER BY")
                '    End If
                ' Reference : YHHR 2034863 - GBOX:Switch database connection to BARDO
                ' Comment   : Consider SQL connection for BARDO 
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2018-11-28
                Case "SqlConnection"
                    lsql = lsql.Replace("SELECT ", "SELECT TOP (800) ")
            End Select
            If Me.OrderByExpression <> "" Then
                lsql = lsql & Me.OrderByExpression
            End If
            Dim mySource As Object = Nothing
            pVarSQL = lsql
            RaiseEventShowSql(lsql)
            Select Case lCurrentDatabaseDriver
                'Case "OracleConnection"
                '    mySource = mUser.Databasemanager.MakeDataReader_OracleDataReader(lsql)
                '    If mySource Is Nothing Then
                '        RaiseeventErrorMessage(mUser.Databasemanager.ErrText)
                '        Return False
                '    End If
                '    RaiseEventDatabaseChange(mUser.Databasemanager.MyOraceDatabase)
                ' Reference : YHHR 2034863 - GBOX:Switch database connection to BARDO
                ' Comment   : Consider SQL connection for BARDO 
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2018-11-28
                Case "SqlConnection"
                    mySource = mUser.Databasemanager.MakeDataTable(lsql)
                    RaiseEventDatabaseChange(mUser.Databasemanager.cnSQL.Database)
            End Select

            If mySource Is Nothing Then
                ErrorInfo("MyDynamicForm_Reporter_Showquery:" & vbCrLf & mUser.Databasemanager.ErrText)
            Else
                RaiseEventShowTheQuery()
                RaiseEventShowSql(lsql)
                lgrdQuery.DataSource = mySource
                lgrdQuery.DataBind()
                mUser.Query = True
                If lgrdQuery.Rows.Count < 800 Then
                    RaiseEventCancelMany(lgrdQuery.Rows.Count)
                Else
                    RaiseEventTooManyInfo(" More than 800 rows for your request found. First 800 rows are listed." & vbCrLf & "Download 10.000 via Excel... ")
                End If
            End If

            Return True

        Catch ex As Exception
            ErrorInfo("MyDynamicForm_Reporter_Showquery:" & ex.Message)
            Return False
        End Try


    End Function
    Private Function ReplaceFixedParams(ByVal lSql As String) As String
        lSql = lSql.Replace("|GBOXSHORTCWID|", "'" & mUser.CW_ID & "'")
        lSql = lSql.Replace("|USERSSUBGROUPID|", "'" & mUser.SUBGROUP_ID & "'")
        Return lSql
    End Function
    Private Function ReplaceParams(ByVal lValue As String, ByVal lobjFieldId As String, ByVal lSql As String) As String
        ' If lobjFieldId = "NAME1" Then Stop
        lSql = lSql.ToUpper
        lobjFieldId = lobjFieldId.ToUpper
        lSql = lSql.Replace("  ", " ")
        lSql = lSql.Replace("   ", " ")
        If lValue = "" Then
            ' =
            lSql = lSql.Replace(lobjFieldId & "AND = " & "|" & lobjFieldId & "|", "")
            lSql = lSql.Replace(lobjFieldId & " = " & "|" & lobjFieldId & "| AND ", "")
            lSql = lSql.Replace(lobjFieldId & " = " & "|" & lobjFieldId & "|", "")
            ' gk 22.09.2011
            'lSql = lSql.Replace("WHERE", "")
            'LIKE
            lSql = lSql.Replace(lobjFieldId & "AND LIKE " & "|" & lobjFieldId & "|", "")
            lSql = lSql.Replace(lobjFieldId & " LIKE " & "|" & lobjFieldId & "| AND ", "")
            lSql = lSql.Replace(lobjFieldId & " LIKE " & "|" & lobjFieldId & "|", "")

            'UPPERLIKE

            lSql = lSql.Replace("AND UPPER(" & lobjFieldId & ") LIKE |" & lobjFieldId & "|", "")
            lSql = lSql.Replace("UPPER(" & lobjFieldId & ") LIKE |" & lobjFieldId & "| AND ", "")
            lSql = lSql.Replace("UPPER(" & lobjFieldId & ") LIKE |" & lobjFieldId & "|", "")

            '------------------------------------------------------------------------------
            ' Reference : CR ZHHR ZHHR 1048962 - GBOX: KPI Viewer does not work
            ' Comment   : Obj field id should be replaced with empty if the value is empty
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-10-12
            lSql = lSql.Replace("|" & lobjFieldId & "|", "'%%'")
            ' Reference END : CR ZHHR 1048962
            '------------------------------------------------------------------------------
        Else
            lSql = lSql.Replace(" AND  ORDER", " ORDER ").ToString
            lValue = lValue.Replace("*", "%")
            lSql = lSql.Replace("|" & lobjFieldId & "|", " '" & lValue & "'")
        End If

        Return lSql
    End Function
    Private mErrText As String
    Public Property ErrText() As String
        Get
            Return mErrText
        End Get
        Set(ByVal value As String)
            mErrText &= value
        End Set
    End Property
End Class
