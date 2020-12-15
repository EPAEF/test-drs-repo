Option Strict Off
Imports Microsoft.VisualBasic
Imports System.Xml.Serialization
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
'Imports Oracle.DataAccess
'Imports Oracle.DataAccess.Client
Imports System.Data.OleDb
Imports Bayer.GBOX.FrameworkClassLibrary
Public Class mySQLDBMANAGER
    Private WithEvents mSQLCn As SqlConnection
    Private mSqlCommand As SqlCommand
    Private mAdapter As SqlDataAdapter
    Private mDS As DataSet
    Private mErrText As String = ""
    Private mUser As myUser
    'Private mMyOraceDatabase As String = pConstOracleDatabase
    Private mMySqlDatabase As String = pConstSqlServerDatabase
    Private objConn As DatabaseConnection
    Private mSQLCnnString As String = GetConnectionString()
    Private mErrNumber As Int64
    Public Function GetGUID() As String
        Dim lsql As String = ""
        lSql = "SELECT newid() as value"
        Dim dtGuid As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        If dtGuid Is Nothing Then Return ""
        If dtGuid.Rows.Count > 0 Then
            Return dtGuid.Rows(0)("value").ToString
        Else
            Return ""
        End If
    End Function
    ''' <summary>
    ''' Reference : OTT 1174 ZHHR 1046721 - GBOX: table DATABASE_DEF
    ''' Comment   : Get/Set the Sql database connection string based on database ID
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date      : 2015-08-13
    ''' </summary>
    ''' <returns>Sql server connection string</returns>
    ''' <remarks></remarks>
    Public Function GetConnectionString() As String
        Try
            objConn = New DatabaseConnection
            mSQLCnnString = objConn.GetConnectionString(pConstSqlServerDatabase)
            Return mSQLCnnString
        Catch ex As Exception
            mErrText &= "GetConnectionString:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            Return Nothing
        End Try
    End Function
    Public Overridable Property cnnString() As String
        Get
            '-----------------------------------------------------------------------------
            ' Reference : OTT 1174 ZHHR 1046721 - GBOX: table DATABASE_DEF
            ' Comment   : Get/Set the Sql database connection string based on database ID
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-08-13
            'Return pVarSQLCnnString
            Return mSQLCnnString
        End Get
        Set(ByVal value As String)
            'pVarSQLCnnString = value
            mSQLCnnString = value
            ' Reference END : OTT 1174 ZHHR 1046721
            '------------------------------------------------------------------------------
            If Not mSQLCn Is Nothing Then mSQLCn.Close()
        End Set
    End Property
    Public Property ErrText() As String
        Get
            Return mErrText
        End Get
        Set(ByVal value As String)
            mErrText &= value
        End Set
    End Property
    Public ReadOnly Property cnSQL() As SqlConnection
        Get
            Try
                If mSQLCn Is Nothing Then
                    mSQLCn = New SqlConnection(cnnString)
                End If
                If mSQLCn.State <> ConnectionState.Open Then
                    mSQLCn.ConnectionString = cnnString
                    mSQLCn.Open()
                End If
                ' If InStr(cnnString, "MDRS") = 0 Then Stop
                Return mSQLCn
            Catch ex As Exception
                mSQLCn.Close()
                mSQLCn.Dispose()
                mSQLCn = Nothing
                mErrText &= "cnSql:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
                Return Nothing
            End Try
        End Get
    End Property
    Public Function MakeSQLCommand(ByVal lSql As String) As SqlCommand
        mSqlCommand = New SqlCommand(lSql, cnSQL)
        Return mSqlCommand
    End Function
    '---------------------------------------------------------------------------------------------------
    ' Reference         : ZHHR 1055145 - GBOX Cockpit: Enhancement of Search Engine
    ' Comment           : stored procedure with cursor for enchancement of search engine
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 31-03-2016
    Public Function GetDataReader(ByVal lprname As String) As SqlDataReader
        Dim cmd As New SqlCommand
        Dim reader As SqlDataReader
        cmd.CommandText = lprname
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.Add("@query", SqlDbType.VarChar, 4000, ParameterDirection.Output)
        cmd.Parameters("@query").Direction = ParameterDirection.Output
        cmd.Connection = cnSQL
        reader = cmd.ExecuteReader()

        Return reader
    End Function
    ' Reference  End    : ZHHR 1055145

    ' Reference         : CRT 2047360 - Implementation of Attachment 
    ' Comment           : Done changes in existing function
    ' Added by          : Anant Jadhav (CWID : EPAEF)
    ' Date              : 02-APR-2020

    Public Sub ExecuteQuery(ByVal requestID As String, ByVal filename As String, ByVal bytes As Byte())
        Dim query As String = "insert into WF_REQUEST_ATTACHMENT values (@REQUEST_ID, @FILE_NAME, @FILE_BINARY)"
        Using cmd As New SqlCommand(query)
            cmd.Connection = cnSQL
            cmd.Parameters.Add("@REQUEST_ID", SqlDbType.NVarChar).Value = requestID
            cmd.Parameters.Add("@FILE_NAME", SqlDbType.NVarChar).Value = filename            
            cmd.Parameters.Add("@FILE_BINARY", SqlDbType.Binary).Value = bytes
            cmd.ExecuteNonQuery()
        End Using
    End Sub
    ' Reference         : CRT 2047360 - Implementation of Attachment 

    Public Function MakeDataSet(ByVal lSql As String) As DataSet
        Try
            mAdapter = New SqlDataAdapter(lSql, cnSQL)
            mDS = New DataSet
            mAdapter.Fill(mDS)
            Return mDS
        Catch ex As Exception
            mErrText &= "Makedataset:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            Return Nothing
        End Try
    End Function

    Public Function MakeStatsStamp(ByVal lTOPIC_GROUP_CONTEXT_ID As String, ByVal lTOPIC_GROUP_ID As String, ByVal lTOPIC_ID As String, ByVal lOBJ_ID As String, ByVal lNavigation_type As String, Optional ByVal lSql As String = "") As Boolean

        Dim lPackage As New List(Of String)
        Dim lIns As String = "INSERT INTO GBOX_STATS " & _
          " ([TOPIC_GROUP_CONTEXT_ID]" & _
           ",[TOPIC_GROUP_ID]" & _
          " ,[TOPIC_ID]" & _
          " ,[OBJ_ID]" & _
         "  ,[CW_ID]" & _
         "  ,[SUBGROUP_ID]" & _
         "  ,[NAVIGATION_TYPE]" & _
         "   ,[ACCESSDATE]" & _
        "   ,[SQL_STATEMENT])" & _
        " VALUES " & _
           "('" & lTOPIC_GROUP_CONTEXT_ID & "'," & _
            "'" & lTOPIC_GROUP_ID & "'," & _
            "'" & lTOPIC_ID & "'," & _
            "'" & lOBJ_ID & "'," & _
            "'" & Me.User.CW_ID & "'," & _
            "'" & Me.User.SUBGROUP_ID & "'," & _
            "'" & lNavigation_type & "'," & _
            "getdate()," & _
            "'" & lSql.Replace("'", "''") & "')"
        lPackage.Add(lIns)
        Return ExecutePackage(lPackage)
    End Function
    Public Function GetRequester(ByVal lApplication As String, ByVal lApplicationPart As String, ByVal lApplicationrole As String, ByVal lSubgroup As String) As String
        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
        ' Comment           : Remove title from database and code
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2014-11-26
        ' Reference         : CR ZHHR 1039297 - GBOX ACFS: Access Issue
        ' Comment           : Because of the ambiguous column name CW_ID, resulting exception and list of users are not displayed
        ' Added by          : Pratyusa Lenka (CWID : EOJCG)
        ' Date              : 2015-02-23
        Dim lsql As String = "Select MDRS_USER.CW_ID, first_name,last_name, SMTP_EMAIL from AUTHORISATION_SET left Join MDRS_USER on MDRS_USER.CW_ID= AUTHORISATION_SET.CW_ID where APPLICATION_ID='" & lApplication & "' and Application_Part_ID ='" & lApplicationPart & "' and APPLICATION_ROLE_ID='" & lApplicationrole & "' And (AUTHORISATION_SET.Subgroup_ID ='" & lSubgroup & "' Or AUTHORISATION_SET.Subgroup_ID ='ALL')"
        ' Reference  END    : CR ZHHR 1039297
        ' Reference  END    : CR ZHHR 1035817

        Dim ldt As DataTable = MakeDataTable(lsql)
        Dim lStr As String = ""
        If ldt Is Nothing Then Return ""
        For Each r As DataRow In ldt.Rows

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
            ' Comment           : Remove title from database and code
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-11-26
            lStr = lStr & r("first_name").ToString & " " & r("last_name").ToString & " " & r("SMTP_EMAIL").ToString & vbCrLf
            ' Reference  END    : CR ZHHR 1035817
        Next
        Return lStr
    End Function
    Public Function MakeDataTable(ByVal lTOPIC_GROUP_CONTEXT_ID As String, ByVal lTOPIC_GROUP_ID As String, ByVal lTOPIC_ID As String, ByVal lOBJ_ID As String, ByVal lNavigation_type As String, Optional ByVal lSql As String = "") As DataTable
        '---------------------------------------------------------------------------------------------------
        ' Reference         : BY-RZ04-CMT-28967 - 110IM08315494 - Problem with XML download 
        ' Comment           : Assign query details from local variable to global variable if exists
        ' Added by          : Surendra Purav (CWID : EQIZU)
        ' Date              : 2013-11-29
        '---------------------------------------------------------------------------------------------------
        If Not String.IsNullOrEmpty(lSql) Then
            pVarSQL = lSql
        End If
        MakeStatsStamp(lTOPIC_GROUP_CONTEXT_ID, lTOPIC_GROUP_ID, lTOPIC_ID, lOBJ_ID, lNavigation_type, lSql)
        Return MakeDataTable(lSql)
    End Function

    Public Function GetRequestValueByFieldname(ByRef Request As Object, ByVal lFieldname As String, Optional ByVal lDetailsview As String = "dvInfo") As String
        '---------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
        ' Comment   : Get the field value for cascade dropdown list, removed the else part
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-07-25, 2016-08-16
        If GetLookupCascade(lFieldname).Rows.Count > 0 And mUser.RequestType = myUser.RequestTypeOption.insert Then
            If Not Request.Form(lDetailsview & "$" & "ddlDynamic-" & lFieldname) Is Nothing Then
                Return Request.Form(lDetailsview & "$" & "ddlDynamic-" & lFieldname).ToString()
            End If
        End If
        ' Reference END : CR ZHHR 1059522
        '---------------------------------------------------------------------------------
        If Not Request.Form(lDetailsview & "$" & lFieldname) Is Nothing Then
            Return Request.Form(lDetailsview & "$" & lFieldname).ToString()
        Else
            Return Nothing
        End If
    End Function
    Public Function GetOrdinalPositionByDisplayName(ByVal lDisplayname As String) As Integer
        Dim lobj_Id As String = Me.mUser.Current_OBJ.OBJ_ID
        Dim lSql As String = "Select Ordinal_Position from OBJ_FIELD where DISPLAY_NAME = '" & lDisplayname & "' and OBJ_ID ='" & lobj_Id & "'"
        Dim mDT As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
        If mDT Is Nothing Then Return 0
        If mDT.Rows.Count = 0 Then Return 0
        Dim lOrdinalpos As Integer = mDT.Rows(0)("Ordinal_Position").ToString
        Return lOrdinalpos
    End Function
    Public Function GetRequestValuebyId(ByVal Id As String, Optional ByVal lDetailsview As String = "dvInfo") As String
        Return GetRequestValuebyId(mRequest, Id, lDetailsview)
    End Function
    Public Function GetCheckedValuebyId(ByRef lRequestForm As Object, ByVal lId As String) As Boolean
        Try
            Dim lSpritarray As Object = lId.Split("$")
            lId = lSpritarray(0).ToString & "$" & lSpritarray(1).ToString & "$Checkbox1"
            If lRequestForm(lId) Is Nothing Then
                Return False
            Else
                If lRequestForm(lId).ToString = "on" Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch
            Return False
        End Try
    End Function
    Public Function GetRequestValuebyId(ByRef Request As Object, ByVal Id As String, Optional ByVal lDetailsview As String = "dvInfo") As String
        If Not Request.Form(lDetailsview & "$" & Id) Is Nothing Then
            Return Request.Form(lDetailsview & "$" & Id).ToString()
        Else
            Return ""
        End If
    End Function
    Public Function GetRequestFormValuebyId(ByRef RequestForm As Object, ByVal Id As String, Optional ByVal lDetailsview As String = "dvInfo") As String
        If Not RequestForm(lDetailsview & "$" & Id) Is Nothing Then
            Return RequestForm(lDetailsview & "$" & Id).ToString()
        Else
            Return ""
        End If
    End Function
    Public Function GetShemadataData() As DataTable
        Return cnSQL.GetSchema("Tables")
    End Function

    Public Function MakeDataTable(ByVal lSql As String) As DataTable
        Try
            If lSql = "" Then
                Return Nothing
            End If
            lSql = Parsesql(lSql)
            If Not mUser Is Nothing Then
                mUser.Current_SQL = lSql
            End If
            mErrText &= ""

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1026150 - GBOX COC - Problems with searching in DRS handbook 
            ' Comment           : Clear the error message if any before executing query
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-03-26
            '---------------------------------------------------------------------------------------------------
            If Not mUser Is Nothing Then
                mUser.Databasemanager.ErrText = ""
            End If

            mAdapter = New SqlDataAdapter(lSql, cnSQL)
            mDS = New DataSet
            mAdapter.Fill(mDS)
            Dim mdt As New DataTable
            mdt = mDS.Tables(0)
            mDS = Nothing
            Return mdt
        Catch ex As Exception
            If Not mUser Is Nothing Then
                mErrText &= "MAKEDATATABLE:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & vbCrLf & lSql
                If InStr(ex.Message, "Conversion failed when") = 0 Then
                    mErrText &= "MyDynamicForm_Requester:BINGRID:" & ex.Message & vbCrLf & Me.User.Databasemanager.ErrText & vbCrLf & Me.User.GBOXmanager.ErrString

                Else
                    mErrText = "Problem with Form Filter: look if filter column and filter value matches"

                End If


            Else
                mErrText &= "MAKEDATATABLE:" & ex.Message & vbCrLf
            End If
            Return Nothing
        End Try
    End Function
    Public Function Parsesql(ByVal lsql As String) As String
        lsql = lsql.Replace("UPDATE", "")
        lsql = lsql.Replace("DROP", "")
        lsql = lsql.Replace("DELETE", "")
        lsql = lsql.Replace("INSERT", "")
        lsql = lsql.Replace("--", "")
        ' Reference : IM0007270618 - YHHR 2034693 - GBOX Space Display
        ' Comment   : Replace &nbsp; with " " to avoid sql query error
        ' Date      : 2018-09-20
        lsql = lsql.Replace("&nbsp;", " ")
        Return lsql
    End Function

    Public Function getFieldSQL() As String
        Dim lsqlDts As String = ""

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : Add check for SYSTEM_DEPENDENT_CUSTOMIZING
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 18-Feb-2016
        If (mUser.RequestType = myUser.RequestTypeOption.insert) Then

            Dim lSysStatus As Boolean = GetSysDependentInfo(mUser.Current_OBJ.OBJ_ID)
            If (lSysStatus = True) Then
                If mUser.SUBGROUP_ID = "ALL" Then
                    lsqlDts = "Select * From OBJ_FIELD where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "' AND OBJ_FIELD_ID  <> 'APPLICATION_ID' AND DISPLAY_NAME is Not NULL Order By Ordinal_Position"
                Else
                    lsqlDts = "Select * From OBJ_FIELD where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & _
                    "' AND OBJ_FIELD_ID  <> 'APPLICATION_ID' AND DISPLAY_NAME is Not NULL" & _
                    " AND (Subgroup_ID ='" & mUser.SUBGROUP_ID & "' or Subgroup_ID ='ALL') " & _
                    "  Order By Ordinal_Position"
                End If
            Else
                If mUser.SUBGROUP_ID = "ALL" Then
                    lsqlDts = "Select * From OBJ_FIELD where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "' AND DISPLAY_NAME is Not NULL Order By Ordinal_Position"
                Else
                    lsqlDts = "Select * From OBJ_FIELD where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & _
                    "' AND DISPLAY_NAME is Not NULL" & _
                    " AND (Subgroup_ID ='" & mUser.SUBGROUP_ID & "' or Subgroup_ID ='ALL') " & _
                    "  Order By Ordinal_Position"
                End If
            End If
        Else
            If mUser.SUBGROUP_ID = "ALL" Then
                lsqlDts = "Select * From OBJ_FIELD where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "' AND DISPLAY_NAME is Not NULL Order By Ordinal_Position"
            Else
                lsqlDts = "Select * From OBJ_FIELD where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & _
                "' AND DISPLAY_NAME is Not NULL" & _
                " AND (Subgroup_ID ='" & mUser.SUBGROUP_ID & "' or Subgroup_ID ='ALL') " & _
                "  Order By Ordinal_Position"
            End If
        End If
        ' Reference  End    : ZHHR 1053017
        Return lsqlDts
    End Function
    '-------------------------------------------------------------------------------
    ' Reference : CR ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
    ' Comment   : Check System Dependent Flag
    ' Added by  : Milind Randive (CWID : EOJCH)
    ' Date      : 18-Jan-2016
    Public Function GetSysDependentInfo(ByVal strObjId As String) As Boolean
        Dim bSysDependentInfo As Boolean = False
        Try
            Dim dtObjApp As DataTable = mUser.Databasemanager.MakeDataTable("SELECT SYSTEM_DEPENDENT_CUSTOMIZING FROM obj WHERE OBJ_ID = '" & strObjId & "'")
            If Not dtObjApp Is Nothing And dtObjApp.Rows.Count > 0 Then

                ' Reference : CR YHHR 2022549 - check if the SYSTEM_DEPENDENT_CUSTOMIZING value is null then assign it as false
                ' Comment   : SYSTEM_DEPENDENT_CUSTOMIZING
                ' Added by  : Sheetal Punnapully (CWID : ETMVO)
                ' Date      : 06-Feb-2018

                If (dtObjApp.Rows(0)("SYSTEM_DEPENDENT_CUSTOMIZING").ToString = "" Or dtObjApp.Rows(0)("SYSTEM_DEPENDENT_CUSTOMIZING").ToString = "False") Then
                    bSysDependentInfo = False
                Else
                    bSysDependentInfo = True
                End If
            End If
        Catch ex As Exception
            Return ""
        End Try

        Return bSysDependentInfo

    End Function
    ' Reference End : CR ZHHR 1053017

    Function MakeObject(ByVal lSql As String, ByVal lFilename As String, ByRef Response As Object) As Boolean
        Try
            Dim lFilemanager As New myFileManager(lFilename)

            Dim tAuthObj As DataTable = MakeDataTable(lSql)
            With lFilemanager
                For Each lAuthObj As DataColumn In tAuthObj.Columns
                    .Writeline("Public Class " & lFilename.Split(".")(0).ToString)
                    .Writeline("Private  m" + lAuthObj.ColumnName + " as string")
                    .Writeline("Public Property " & lAuthObj.ColumnName & " As string")
                    .Writeline("Get")
                    .Writeline("return m" + lAuthObj.ColumnName)
                    .Writeline("End GEt")
                    .Writeline("Set(ByVal value As String)")
                    .Writeline("m" + lAuthObj.ColumnName + " = value")
                    .Writeline("End Set")
                    .Writeline("End Property")
                Next lAuthObj
                .Writeline("End Class")
                .SendFile(Response)
            End With
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Function MakeObjectSetString(ByVal lSql As String) As String
        Dim lAllPropertys As String = ""
        Try
            Dim tAuthObj As DataTable = MakeDataTable(lSql)
            For Each lAuthObj As DataColumn In tAuthObj.Columns
                Dim strPropertys As String = vbNewLine + " ." + lAuthObj.ColumnName + " =r(" & Chr(34) & lAuthObj.ColumnName & Chr(34) & ").tostring"
                lAllPropertys = lAllPropertys & strPropertys & vbCrLf
            Next lAuthObj
            Return lAllPropertys

        Catch ex As Exception
            Return "MAKEOBJSETSTRING:" & ex.Message
        End Try
    End Function
    Public Function ExecutePackage(ByVal lPackage As List(Of String)) As Boolean
        Dim lSql As String
        Dim cmd As SqlCommand = cnSQL.CreateCommand()
        Dim mTrans As SqlTransaction = cnSQL.BeginTransaction()
        Try
            For Each lSql In lPackage
                '---------------------------------------------------------------------------------
                ' Reference : YHHR 2027809 - INC_GBox: Single quote issue
                ' Comment   : Replace Accents grave(`) with double single quotes('')
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2018-05-18

                ' Reference : IM0007270618 - YHHR 2034693 - GBOX Space Display
                ' Comment   : Replace &nbsp; with " " to avoid sql query error
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2018-09-20
                ' Reference : IM0007528687 - YHHR 2036565 - GBox: Single Quote Issue
                ' Comment   : comment code  lSql.Replace("`", "''") as all other code 
                '             replace ' with '' and not with `
                ' Added by  : Rajan Dmello (CWID : EOLRG) 
                ' Date      : 2018-10-30

                'cmd.CommandText = lSql.Replace("`", "''")
                cmd.CommandText = lSql.Replace("&nbsp;", " ")

                ' Reference END : YHHR 2036565
                ' Reference END : YHHR 2034693
                ' Reference END : YHHR 2027809
                '---------------------------------------------------------------------------------
                cmd.Transaction = mTrans
                cmd.ExecuteNonQuery()
            Next
            mTrans.Commit()
            Return True
        Catch ex As Exception
            Dim lStr As String = ""
            Debug.Print(ex.Message)
            For Each lSql In lPackage
                lStr = lStr & vbCrLf & lSql
            Next lSql
            mTrans.Rollback()
            mErrText &= "mysqldbManager:ExecutePackage" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & vbCrLf & lStr
            mErrNumber = DirectCast(ex, System.Data.SqlClient.SqlException).Number
            LogError(mErrNumber, mErrText)
            Return False
        End Try
    End Function
    'Function MakeBardoCnn_OracleConnection() As OracleConnection
    '    '---------------------------------------------------------------------------------
    '    ' Reference : OTT 1174 ZHHR 1046721 - GBOX: table DATABASE_DEF
    '    ' Comment   : Get the Oracle BARDO database connection string based on database ID
    '    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    '    ' Date      : 2015-08-13
    '    'Dim lCnnString As String = pConstBardo_OracleCnStr
    '    objConn = New DatabaseConnection
    '    Dim lCnnString As String = objConn.GetConnectionString(pConstOracleDatabase)

    '    ' Reference END : OTT 1174 ZHHR 1046721
    '    '---------------------------------------------------------------------------------
    '    Dim conn As New OracleConnection(lCnnString)
    '    conn.Open()
    '    mBardo_OracleConnection = conn
    '    Return conn
    'End Function
    'Public Function MakeDataReader_OracleDataReader(ByVal lSql As String) As OracleDataReader
    '    Try
    '        lSql = Parsesql(lSql)
    '        pVarSQL = lSql
    '        mErrText &= ""
    '        Dim cmd As New OracleCommand(lSql, Bardo_OracleConnection)
    '        cmd.CommandType = CommandType.Text
    '        Dim dr As OracleDataReader = cmd.ExecuteReader()
    '        Return dr
    '    Catch ex As Exception
    '        mErrText &= "MakeDataReader_OracleDataReader:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & vbCrLf & lSql
    '        Return Nothing
    '    End Try
    'End Function
    ''' <summary>
    ''' Reference         : ZHHR 1056919 - GBOX Webforms OTT 2365: GBox Apps Logging
    ''' Comment           : GBox Apps Logging
    ''' Added by          : Milind Randive (CWID : EOJCH)
    ''' Date              : 01-Jul-2016
    ''' 
    ''' Reference : YHHR 2036565 - GBox: Single Quote Issue
    ''' Comment   : change lErrDesc.Replace("'", "`") to lErrDesc.Replace("'", "''")
    ''' Added by  : Rajan Dmello (CWID : EOLRG) 
    ''' Date      : 2018-10-30
    ''' </summary>
    ''' <param name="lErrNum"></param>
    ''' <param name="lErrDesc"></param>
    ''' <remarks></remarks>
    Public Sub LogError(ByVal lErrNum As String, ByVal lErrDesc As String)
        Dim lSql As String
        Dim cmd As SqlCommand = cnSQL.CreateCommand()
        Dim mTrans As SqlTransaction = cnSQL.BeginTransaction()

        lSql = "INSERT INTO MDRS_ERROR_LOG VALUES ('" & lErrNum & "', '" & lErrDesc.Replace("'", "''") & "'," & _
               "'" & "GBOX WEBFORMS" & "'" & ",'" & Now() & "','" & "OCCURED" & "')"
        cmd.CommandText = lSql
        cmd.Transaction = mTrans
        cmd.ExecuteNonQuery()
        mTrans.Commit()
    End Sub
    ''' <summary>
    ''' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2016-07-25
    ''' </summary>
    ''' Reference : YHHR 2041528 - GBOX COC: Cascaded functionality does not work
    ''' Comment   : Change LOOKUP_TABLE_KEY instead of OBJ_FIELD_ID to get the required data for cascaded lookup functionality
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2019-04-25
    ''' <param name="strDisplayName"></param>
    ''' <returns>Datatable for the displayname</returns>
    ''' <remarks>Get the datatable based on displayname for lookup fields</remarks>
    Public Function GetLookupCascade(ByVal strDisplayName As String) As DataTable
        Dim dtLookupCascade As DataTable = Nothing
        Dim strSqlCascade As String = ""
        Try
            strSqlCascade = "SELECT OFD.OBJ_FIELD_ID, OFD.DISPLAY_NAME, OFD.ISKEYMEMBER, OFD.[REQUIRED], OFD.REQUIREDTEXT, OFL.LOOKUP_TABLE_NAME, OFL.LOOKUP_TABLE_KEY, OFL.LOOKUP_TABLE_FILTER, OFL.[CASCADE] FROM OBJ_FIELD OFD"
            strSqlCascade &= " INNER JOIN OBJ_FIELD_LOOKUP_VALIDATON OFL ON OFD.OBJ_ID = OFL.OBJ_ID AND OFD.OBJ_FIELD_ID = OFL.OBJ_FIELD_ID"
            strSqlCascade &= " WHERE OFD.OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "' AND OFD.OBJ_FIELD_TYPE_ID = 'LOOKUP' AND OFD.DISPLAY_NAME = '" & strDisplayName & "'"
            dtLookupCascade = MakeDataTable(strSqlCascade)
            Return dtLookupCascade
        Catch ex As Exception
            mErrText &= "GetLookupCascade:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            Return Nothing
        End Try
    End Function
    ''' <summary>
    ''' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2016-07-25
    ''' </summary>
    ''' <param name="strObjFieldId"></param>
    ''' <returns>True if field is cascade or dependent in table filter</returns>
    ''' <remarks>Check whether the objfield is cascade field or dependent cascade field</remarks>
    Public Function IsObjFieldInLookupFilter(ByVal strObjFieldId As String) As Boolean
        Dim bIsValueExists As Boolean = False
        Dim dtObjFieldInLookupFilter As DataTable = MakeDataTable("SELECT OBJ_FIELD_ID, LOOKUP_TABLE_FILTER, [CASCADE] FROM OBJ_FIELD_LOOKUP_VALIDATON WHERE OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "'")
        If Not dtObjFieldInLookupFilter Is Nothing AndAlso dtObjFieldInLookupFilter.Rows.Count > 0 Then
            For Each drRow As DataRow In dtObjFieldInLookupFilter.Rows
                'Reference  : IM0003924723 - LOOKUP_TABLE_FILTER does not work in D and Q system
                'Modified by: Pratyusa Lenka (CWID : EOJCG)
                'Date       : 2016-12-20
                If drRow("LOOKUP_TABLE_FILTER").ToString.Contains(strObjFieldId) AndAlso Not String.IsNullOrEmpty(drRow("CASCADE").ToString) AndAlso CBool(drRow("CASCADE").ToString) Then
                    bIsValueExists = True
                ElseIf drRow("OBJ_FIELD_ID").ToString = strObjFieldId AndAlso Not String.IsNullOrEmpty(drRow("CASCADE").ToString) AndAlso CBool(drRow("CASCADE").ToString) Then
                    bIsValueExists = True
                End If
            Next
        End If
        Return bIsValueExists
    End Function
    ''' <summary>
    ''' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2016-08-03
    ''' </summary>
    ''' <returns>True if atleast one cascade field</returns>
    ''' <remarks>Check if cascade is true for any field in lookup validation</remarks>
    Public Function HaveAtleastOneCascadeField() As Boolean
        Dim bIsCascadeField As Boolean = False
        Dim dtCascadeLookupVal As DataTable = MakeDataTable("SELECT [CASCADE] FROM OBJ_FIELD_LOOKUP_VALIDATON WHERE OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "'")
        If Not dtCascadeLookupVal Is Nothing AndAlso dtCascadeLookupVal.Rows.Count > 0 Then
            For Each drRow As DataRow In dtCascadeLookupVal.Rows
                If Not String.IsNullOrEmpty(drRow("CASCADE").ToString) AndAlso CBool(drRow("CASCADE").ToString) Then
                    bIsCascadeField = True
                    Exit For
                End If
            Next
        End If
        Return bIsCascadeField
    End Function
    'Private WithEvents mBardo_OracleConnection As OracleConnection

    'Public ReadOnly Property Bardo_OracleConnection() As OracleConnection
    '    Get
    '        mBardo_OracleConnection = MakeBardoCnn_OracleConnection()
    '        Return mBardo_OracleConnection
    '    End Get
    'End Property
    Public Property MySqlDatabase() As String
        Get
            Return mMySqlDatabase
        End Get
        Set(ByVal value As String)
            mMySqlDatabase = value
        End Set
    End Property

    Private mRequest As Object

    Public Property Request() As Object
        Get
            Return mRequest
        End Get
        Set(ByVal value As Object)
            mRequest = value
        End Set
    End Property

    Public Sub New()

    End Sub


    Public Property User() As myUser
        Get
            Return mUser
        End Get
        Set(ByVal value As myUser)
            mUser = value
        End Set
    End Property



    'Public Property MyOraceDatabase() As String
    '    Get
    '        Return mMyOraceDatabase
    '    End Get
    '    Set(ByVal value As String)
    '        mMyOraceDatabase = value
    '    End Set
    'End Property

    'Private Sub mBardo_OracleConnection_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles mBardo_OracleConnection.Disposed

    'End Sub

    'Private Function mBardo_OracleConnection_Failover(ByVal sender As Object, ByVal eventArgs As Oracle.DataAccess.Client.OracleFailoverEventArgs) As Oracle.DataAccess.Client.FailoverReturnCode Handles mBardo_OracleConnection.Failover

    'End Function

    'Private Sub mBardo_OracleConnection_HAEvent(ByVal eventArgs As Oracle.DataAccess.Client.OracleHAEventArgs) Handles mBardo_OracleConnection.HAEvent

    'End Sub

    'Private Sub mBardo_OracleConnection_InfoMessage(ByVal sender As Object, ByVal eventArgs As Oracle.DataAccess.Client.OracleInfoMessageEventArgs) Handles mBardo_OracleConnection.InfoMessage

    'End Sub

    'Private Sub mBardo_OracleConnection_StateChange(ByVal sender As Object, ByVal e As System.Data.StateChangeEventArgs) Handles mBardo_OracleConnection.StateChange

    'End Sub
End Class


Public Class myGumsManager
    Inherits mySQLDBMANAGER
    '--------------------------------------------------------------------
    'Reference  : ZHHR 1047321 - GBOX: change connection to CWID database
    'Comment    : Get the new GUMS DB connection sting from database
    'Added by   : Pratyusa Lenka (CWID : EOJCG)
    'Date       : 2015-08-17
    'Private mDatabase As String = "GUMSCWIDDB"
    'Private mServerName As String = "BY-GDCSQL056/GDC056:1450"
    'Private mSQLCnnString As String = "Data Source=" & mServerName & ";Initial Catalog=" & mDatabase & ";Persist Security Info=FALSE;User ID='MYALB';Password='*****'"
    Dim objConn As DatabaseConnection = New DatabaseConnection
    Private mSQLCnnString As String = objConn.GetConnectionString(pConstGUMSDatabase)
    ' Reference END : CR ZHHR 1047321
    '--------------------------------------------------------------------
    'Private mSQLCnnString As String = "Driver={SQL Server};Server=BY00XI\SQLEXPRESS;Database=MDRS;Uid=MDRSMASTER;Pwd=BMD;"
    'Private mSQLCnnString As String = "Data Source=BY-GDCSQL056\GDC056;Initial Catalog=GUMSCWIDDB;Persist Security Info=FALSE ;User ID='MDRSMASTER';Password='BMD'"
    Public Overrides Property cnnString() As String
        Get
            ' MyBase.cnnString = mSQLCnnString
            Return mSQLCnnString
        End Get
        Set(ByVal value As String)
            mSQLCnnString = value
        End Set
    End Property

End Class

