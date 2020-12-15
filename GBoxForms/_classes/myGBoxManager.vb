Option Strict Off
'Imports Oracle.DataAccess
'Imports Oracle.DataAccess.Client
Imports Bayer.GBOX.FrameworkClassLibrary
Imports System.IO
Imports System.IO.Packaging
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Spreadsheet

Public Class myGBoxManager
    Private mRequestedApplication As List(Of myApplication)
    Private mUsertext As String
    Private mHeadText As String
    Private mGums As myGumsManager
    Private mNodes As Long
    Private mAppRoles As New List(Of String)
    Private m_PathValues As String = ""
    Private objConn As DatabaseConnection

    Public Function GetFieldType(ByVal lObj_ID As String, ByVal lFieldname As String)
        Try
            Dim lSql As String = "Select OBJ_Field_Type_ID from OBJ_FIELD where OBJ_FIELD_ID ='" & lFieldname & "' And Obj_ID ='" & lObj_ID & "'"
            Dim mDt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            Dim lDiplayname As String = mDt.Rows(0)("OBJ_Field_Type_ID").ToString
            Return lDiplayname
        Catch ex As Exception
            mErrString = ex.Message
            Return ""
        End Try
    End Function
    Public Function GetClassificationTemplateObject(ByVal lClassification_ID As String, Optional ByVal lOBJ_CLASSIFICATION_DETAILS_Field As String = "CLASSIFICATION_VIEW_TEMPLATE_OBJ_ID")
        Try
            Dim lSql As String = "Select " & lOBJ_CLASSIFICATION_DETAILS_Field & " from OBJ_CLASSIFICATION_DETAILS where OBJ_CLASSIFICATION_ID ='" & lClassification_ID & "'"
            Dim mDt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            Dim lReturn As String = mDt.Rows(0)(lOBJ_CLASSIFICATION_DETAILS_Field).ToString
            Return lReturn
        Catch ex As Exception
            mErrString = ex.Message
            Return ""
        End Try
    End Function
    Public Function GetFieldNameByObjDisplay(ByVal lOBJ_ID As String, ByVal lObj_DISPLAY_ID As String)
        Try
            Dim lSql As String = "Select OBJ_FIELD_ID from OBJ_FIELD where DISPLAY_NAME ='" & lObj_DISPLAY_ID & "' And Obj_ID ='" & lOBJ_ID & "'"
            Dim mDt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            If mDt.Rows.Count = 0 Then
                Return ""
            End If

            Dim lDiplayname As String = mDt.Rows(0)("OBJ_FIELD_ID").ToString
            Return lDiplayname
        Catch ex As Exception
            mErrString = ex.Message
            Return ""
        End Try
    End Function
    Public Function GetDisplayNameByObjFieldId(ByVal lOBJ_ID As String, ByVal lObj_Field_ID As String)
        Try
            Dim lSql As String = "Select DISPLAY_NAME from OBJ_FIELD where OBJ_FIELD_ID ='" & lObj_Field_ID & "' And Obj_ID ='" & lOBJ_ID & "'"
            Dim mDt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            If mDt.Rows.Count = 0 Then
                Return ""
            End If
            Dim lDiplayname As String = mDt.Rows(0)("DISPLAY_NAME").ToString
            Return lDiplayname
        Catch ex As Exception
            mErrString = ex.Message
            Return ""
        End Try
    End Function
    Public Enum DisplayType
        Grid = 0
        DetailsView = 1
    End Enum

    Public ReadOnly Property CONTEXT()
        Get
            Return pObjCurrentUsers.CONTEXT
        End Get
    End Property
    Public Function SendMailTooManyData() As Boolean

        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
        ' Comment           : Remove title from database and code
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2014-12-15

        Dim lMailtext As String = "Dear " & mUser.first_name & " " & mUser.last_name & ","
        lMailtext = lMailtext & "the report you started in GBOX-Cockpit delivered too many results." & vbCrLf
        lMailtext = lMailtext & "The result is limited to 10.000 records. Please refine you search criteria" & vbCrLf
        lMailtext = lMailtext & "or contact your Masterdata responsible for help."
        lMailtext = lMailtext & "see http://by-gbox.bayer-ag.com/MDR-Contact-Persons/"
        lMailtext = lMailtext & vbCrLf
        lMailtext = lMailtext & "Your request:"
        lMailtext = lMailtext '& lSql
        Dim lGboxId As String = "GBOX" & Date.Now.ToString
        Dim lstrMail As String = "INSERT INTO [M_MAILTRIGGER]" & _
                            "([M_MAILKEY]" & _
                            ",[M_RECIPIENTS]" & _
                            ",[M_SUBJECT]" & _
                            ",[M_BODY]" & _
                            ",[M_CURRENT_SENDER])" & _
                            " VALUES('" & lGboxId & "_REQUEST" & "','" _
                            & mUser.SMTP_EMAIL & _
                            "','GBOX_REQUEST:" & lGboxId & "_DataSet'," & _
                            "N'" & lMailtext & "','G_BOX')"

        ' Reference  END    : CR ZHHR 1035817

        Dim lPack As New List(Of String)
        lPack.Add(lstrMail)
        Return mUser.Databasemanager.ExecutePackage(lPack)
    End Function
    Private m_LinkServer As String = ""
    Public Function AddFieldUrl(ByVal lstrSql As String)
        '------------------------------------------------------------------------------------------------------------
        ' Reference         : CR - BY-RZ04-CMT-27932 - Enhance GBOX with a database parametered download website
        ' Comment           : Added try catch block in case of any error
        ' Added by          : Surendra Purav (CWID : EQIZU)
        ' Date              : 2013-11-26
        '------------------------------------------------------------------------------------------------------------
        Try
            m_Paramlist = "HTTP://www." & m_LinkServer & "/Cockpit.aspx" & m_Paramlist
            Dim lUrlGen As String = m_Paramlist & " '& WhereInfo'"
            For Each lKey As myKeyObj In Me.KeyCollection
                If lKey.Key_ID <> "OBJ_VERSIONNUMBER" Then
                    lUrlGen &= KeyUrlInfo(lKey.Key_ID) & " AND"
                End If
            Next
            lUrlGen = lUrlGen.Substring(0, lUrlGen.Length - 3)
            lUrlGen &= " as URL"
            Return lUrlGen
        Catch ex As Exception
            Return Nothing
        End Try

    End Function
    ''WhereInfo(MTART =' + MTART + ')'  as URL FROM CUSTOMIZING_T134
    Public Function KeyUrlInfo(ByVal lKeyname As String) As String
        Return "(" & lKeyname & " =  ' + " & lKeyname & "+ ')'"
    End Function
    Private m_Paramlist As String

    Public Sub Makereport(ByRef lResponse As HttpResponse, ByVal lFilename As String, ByVal lstrSQL As String, ByRef Server As Object, ByRef TooMany As Boolean, Optional ByRef lErrMsg As String = "")

        lFilename = lFilename & ".xlsx"
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
        End If

        Dim mydt As DataTable = mUser.Databasemanager.MakeDataTable(lstrSQL)
        ' Reference : YHHR 2034863 - GBOX:Switch database connection to BARDO
        ' Comment   : Consider SQL connection for BARDO 
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2018-11-28
        AddFieldUrl(lstrSQL)
        If mydt Is Nothing Then
            If String.IsNullOrEmpty(lstrSQL) Then Exit Sub
            ''------------------------------------------------------------------------------------------------------------
            '' Reference         : BY-RZ04-CMT-28967 - 110IM08315494 - Problem with XML download 
            '' Comment           : If tables / views not found on SQL then search in oracle database
            '' Added by          : Surendra Purav (CWID : EQIZU)
            '' Date              : 2013-12-04
            ''------------------------------------------------------------------------------------------------------------
            ''ORACLE Comment out
            'Dim lOraCommand As OracleCommand = New OracleCommand _
            '    (lstrSQL, mUser.Databasemanager.Bardo_OracleConnection)

            'Dim lOraData As New DataTable
            '' ----------------------------------------------------------------------------------------
            '' Reference  : CR-1024616-110IM08705465-Gbox Web Portal: Enhance DownLoad.aspx with Bardo ACCESS
            '' Comment    : Check if CurrentOBJ is not defined then retreive the table name
            '' Created by : EQIZU
            '' Date       : 21-MAR-2014
            '' ---------------------------------------------------------------------------------------            
            'If mUser.Current_OBJ Is Nothing Then
            '    'To retrieve the table name
            '    Dim lstrTblName As String = ""
            '    Dim liStartInex As Integer = lstrSQL.IndexOf("FROM") + 4
            '    Dim liEndIndex As Integer = lstrSQL.IndexOf("WHERE")
            '    lstrTblName = Trim(lstrSQL.Substring(liStartInex, liEndIndex - liStartInex).ToUpper)
            '    lOraData.TableName = lstrTblName
            'Else
            '    lOraData.TableName = mUser.Current_OBJ.OBJ_DISPLAY_NAME
            'End If
            'Dim ladaptOraData As New OracleDataAdapter(lOraCommand)
            'ladaptOraData.Fill(lOraData)
            'mydt = lOraData
            ''END ORACLE
            'If mydt.Rows.Count > 10000 Then
            '    TooMany = True
            '    SendMailTooManyData()
            '    Exit Sub
            'End If
        End If
        If mydt.Rows.Count = 0 Then
            lErrMsg = "No records found"
            Exit Sub
        End If
        Try
            ' Reference : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
            ' Comment   : Excel download instead of XML download 
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2018-11-06
            With lResponse
                Dim stream As System.IO.MemoryStream = New System.IO.MemoryStream()
                Using document As SpreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, True)
                    If mydt.Columns.Contains("VERSIONNO") Then
                        mydt.Columns.Remove("VERSIONNO")
                    End If
                    WriteDataTableToExcelWorksheet(mydt, document)
                End Using
                stream.Flush()
                stream.Position = 0
                .ClearContent()
                .Clear()
                .Buffer = True
                .Charset = ""
                .Cache.SetCacheability(System.Web.HttpCacheability.NoCache)
                .AddHeader("content-disposition", "attachment; filename=" & lFilename)
                .ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                Dim dr As Byte() = New Byte(stream.Length - 1) {}
                stream.Read(dr, 0, dr.Length)
                stream.Close()
                .BinaryWrite(dr)
                .Flush()
                .SuppressContent = True
                HttpContext.Current.ApplicationInstance.CompleteRequest()
            End With
            lErrMsg = "OK"
        Catch ex As Exception
            MsgBox(ex.Message)
            lErrMsg = ex.Message
        Finally
            GC.Collect()
        End Try
    End Sub
    Private Shared Sub WriteDataTableToExcelWorksheet(ByVal dt As DataTable, ByVal document As SpreadsheetDocument)
        Dim workbookPart As WorkbookPart = document.AddWorkbookPart()
        workbookPart.Workbook = New Workbook()
        Dim worksheetPart As WorksheetPart = workbookPart.AddNewPart(Of WorksheetPart)()
        Dim sheetData = New SheetData()
        worksheetPart.Worksheet = New Worksheet(sheetData)
        Dim sheets As Sheets = workbookPart.Workbook.AppendChild(New Sheets())
        Dim sheet As Sheet = New Sheet() With {
            .Id = workbookPart.GetIdOfPart(worksheetPart),
            .SheetId = 1,
            .Name = dt.TableName
        }
        sheets.Append(sheet)
        Dim headerRow As Row = New Row()
        Dim columns As List(Of String) = New List(Of String)()

        For Each column As System.Data.DataColumn In dt.Columns
            columns.Add(column.ColumnName)
            Dim cell As Cell = New Cell()
            cell.DataType = CellValues.String
            cell.CellValue = New CellValue(column.ColumnName)
            headerRow.AppendChild(cell)
        Next

        sheetData.AppendChild(headerRow)

        For Each dsrow As DataRow In dt.Rows
            Dim newRow As Row = New Row()

            For Each col As String In columns
                Dim cell As Cell = New Cell()
                cell.DataType = CellValues.String
                cell.CellValue = New CellValue(dsrow(col).ToString())
                newRow.AppendChild(cell)
            Next

            sheetData.AppendChild(newRow)
        Next

        workbookPart.Workbook.Save()
    End Sub
    Public Function GetRecipientByApplicationPart(ByVal lApplicationPart As String) As String
        Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable("Select REQUEST_TYPE_ID from APPLICATION_PARTS where APPLICATION_PART_ID='" & lApplicationPart & "'")
        If mdt Is Nothing Then Return ""
        If mdt.Rows.Count = 0 Then Return ""
        Return GetRecipientByRequestType(mdt.Rows(0)("REQUEST_TYPE_ID").ToString)
    End Function
    Public Function GetRecipientByRequestType(ByVal lRequestType As String) As String
        Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable("Select HotlineMailAccount from REQUEST_TYPE where REQUEST_TYPE_ID='" & lRequestType & "'")
        Return mdt.Rows(0)("HotlineMailAccount").ToString
    End Function
    Public WriteOnly Property HeadText() As String
        Set(ByVal value As String)
            mHeadText = value
        End Set
    End Property
    Public Property RequestedApplications() As List(Of myApplication)
        Get
            Return mRequestedApplication
        End Get
        Set(ByVal value As List(Of myApplication))
            mRequestedApplication = value
        End Set
    End Property

    Public Function AuthenticateRequest(ByVal lCurrentObject As String, ByVal lCw_ID As String) As Boolean
        Try
            Dim lSql As String = ""
            lSql = "Select * From wf_DEFINE_WORKFLOW_DETAILS"
            lSql = lSql & " WHERE"
            lSql = lSql & " (wf_DEFINE_WORKFLOW_DETAILS.RANK = 0) AND (wf_DEFINE_WORKFLOW_DETAILS.OBJ_ID = '" & lCurrentObject & "') "
            lSql = lSql & " "
            lSql = lSql & " And STATION_ID='AUTHENTICATE'"
            Dim lDt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            If lDt.Rows.Count = 0 Then
                Return True
            End If

            lSql = "SELECT AUTHORISATION_SET.CW_ID, AUTHORISATION_SET.ORG_LEVEL_ID, AUTHORISATION_SET.ORG_LEVEL_VALUE, "
            lSql = lSql & "  AUTHORISATION_SET.APPLICATION_ID, AUTHORISATION_SET.APPLICATION_PART_ID, AUTHORISATION_SET.APPLICATION_ROLE_ID,"
            lSql = lSql & " wf_DEFINE_WORKFLOW_DETAILS.RANK, wf_DEFINE_WORKFLOW_DETAILS.OBJ_ID, ROLE_CLUSTER.Role_Cluster_ID"
            lSql = lSql & " FROM AUTHORISATION_SET "
            lSql = lSql & " INNER JOIN ROLE_CLUSTER "
            lSql = lSql & " ON AUTHORISATION_SET.APPLICATION_ID = ROLE_CLUSTER.Application_ID "
            lSql = lSql & " AND AUTHORISATION_SET.APPLICATION_PART_ID = ROLE_CLUSTER.Application_Part_ID "
            lSql = lSql & " AND AUTHORISATION_SET.APPLICATION_ROLE_ID = ROLE_CLUSTER.Application_Role_ID "
            lSql = lSql & " INNER JOIN wf_DEFINE_WORKFLOW_DETAILS"
            lSql = lSql & " ON AUTHORISATION_SET.ORG_LEVEL_ID = wf_DEFINE_WORKFLOW_DETAILS.ORG_LEVEL_ID "
            lSql = lSql & " AND AUTHORISATION_SET.ORG_LEVEL_VALUE = wf_DEFINE_WORKFLOW_DETAILS.ORG_LEVEL_VALUE "
            lSql = lSql & " WHERE (AUTHORISATION_SET.CW_ID = '" & lCw_ID & "') "
            lSql = lSql & " AND  (wf_DEFINE_WORKFLOW_DETAILS.RANK = 0) AND (wf_DEFINE_WORKFLOW_DETAILS.OBJ_ID = '" & lCurrentObject & "') AND "
            lSql = lSql & " (ROLE_CLUSTER.Role_Cluster_ID = N'GBOX_REQUESTER')"
            lDt = mUser.Databasemanager.MakeDataTable(lSql)
            If lDt.Rows.Count = 0 Then

                '---------------------------------------------------------------------------------------------------
                ' Reference         : ZHHR 1040447 - GBOX COC: No display of DRS request to public users
                ' Comment           : Public bayer user can create a DRS request.
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 24-Mar-2015

                lSql = " SELECT obj_id FROM wf_DEFINE_WORKFLOW_DETAILS " & _
                       " WHERE OBJ_ID = '" & lCurrentObject & "' AND Role_Cluster_ID = N'GBOX_REQUESTER' AND RANK = 0  AND Public_Flag=1 "

                Dim lDtw As DataTable = mUser.Databasemanager.MakeDataTable(lSql)

                If (lDtw.Rows.Count > 0) Then
                    lSql = " SELECT CW_ID FROM MDRS_USER WHERE CW_ID = '" & lCw_ID & "' "
                    Dim lDtu As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
                    If (lDtu.Rows.Count > 0) Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
                ' Reference End    : ZHHR 1040447

                Return False
            Else

                Return True
            End If
        Catch
            Return False
        End Try
    End Function
    Public Function Authorisation(ByVal lNodePath As String) As Boolean
        Dim lAuthArray As Array = lNodePath.Split("/")
        Dim lApplication As String = lAuthArray(0).ToString.Split("(")(0).Trim
        Dim lCurrentUserRoles As String = ""
        Dim lApplicationpart As String = ""
        Dim lApplicationrole As String = ""
        If lAuthArray.GetUpperBound(0) > 1 Then
            lApplicationpart = lAuthArray(1).ToString.Split("(")(0).Trim
            lApplicationrole = lAuthArray(lAuthArray.GetUpperBound(0)).ToString.Split("(")(0).Trim
            lCurrentUserRoles = Authorisation(lApplication, lApplicationpart)
        End If
        Dim lCurrentAdministrationRole As String = GetAdministrationroleForRolename(lApplication, lApplicationpart, lApplicationrole)
        If mAppRoles.Contains(lCurrentAdministrationRole) Or IsGBoxAdmin(mUser.CW_ID) Then
            Return True
        End If
    End Function
    Public Function GetAdministrationroleForRolename(ByVal lApplication As String, ByVal lApplicationPart As String, ByVal lRolename As String) As String
        Dim lmySQL As String = "select  ADMINISTRATION_ROLE_ID from application_role where APPLICATION_ID='" & lApplication & "' and APPLICATION_PART_ID='" & lApplicationPart & "' and APPLICATION_ROLE_ID='" & lRolename & "'"
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lmySQL)
        Dim lReturn As String = ""
        For Each r As DataRow In ldt.Rows
            lReturn = lReturn & r("ADMINISTRATION_ROLE_ID").ToString
        Next r
        Return lReturn
    End Function

    Public Function Authorisation(ByVal lApplication As String, ByVal lApplicationPart As String, Optional ByVal lCW_ID As String = "") As String
        mAppRoles = Nothing
        mAppRoles = New List(Of String)
        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        pObjCurrentUsers.CONTEXT = CONTEXT
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
        End If

        If lCW_ID = "" Then
            lCW_ID = mUser.CW_ID
        End If

        Dim lSql As String = "Select APPLICATION_ROLE_ID from  AUTHORISATION_SET where APPLICATION_ID='" & lApplication & "' And APPLICATION_PART_ID='" & lApplicationPart & "' And CW_ID='" & lCW_ID & "'"
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
        Dim lAppRoles As String = "''"
        For Each r As DataRow In dt.Rows
            lAppRoles = lAppRoles & ",'" & r("APPLICATION_ROLE_ID").ToString & "'"
            mAppRoles.Add(r("APPLICATION_ROLE_ID").ToString)
        Next
        Return lAppRoles
    End Function
    Public Function GetAdministrationRoles(ByVal lRoles As String) As String
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable("select distinct ADMINISTRATION_ROLE_ID from application_role where ADMINISTRATION_ROLE_ID in (" & lRoles & ")")
        Dim lReturn As String = "''"
        For Each r As DataRow In ldt.Rows
            lReturn = lReturn & ",'" & r("ADMINISTRATION_ROLE_ID").ToString & "'"
        Next r
        Return lReturn
    End Function
    Public Function GetAdministratableRolesInApplicationPart(ByVal lApplication As String, ByVal lApplicationpart As String, ByVal lShortCwId As String) As String
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable("select  applicationROLE from vw_Auth_Set_With_Desriptions where Application ='" & lApplication & "' and Applicationpart='" & lApplicationpart & "' And CW_ID='" & lShortCwId & "'")
        Dim lReturn As String = "''"
        For Each r As DataRow In ldt.Rows
            lReturn = lReturn & ",'" & r("applicationrole").ToString & "'"
        Next r
        Return lReturn
    End Function
    Public Function IsGBoxAdmin(ByVal lCW_ID As String) As Boolean
        Dim lSql As String = "Select * from AUTHORISATION_SET where cw_ID='" & lCW_ID & "' and APPLICATION_ID='GBOX' and  APPLICATION_ROLE_ID='ADMINISTRATOR'"
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
        If dt.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function AuthSetExists(ByVal lAuthSet As AuthorizationSet) As Boolean
        Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable("Select Count(*) as Anzahl from AUTHORISATION_SET where APPLICATION_ID = '" & lAuthSet.Application & "'" & _
                                " And APPLICATION_PART_ID = '" & lAuthSet.Applicationpart & "'" & _
                                " And APPLICATION_ROLE_ID = '" & lAuthSet.Applicationrole & "'" & _
                                " And SUBGROUP_ID ='" & lAuthSet.Subgroup & "'" & _
                                " and CW_ID ='" & lAuthSet.CW_ID & "'")
        If mdt.Rows(0)("Anzahl").ToString = "0" Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Function openInNewWindow(ByVal lUrl As String) As String
        Return "<script type='text/javascript'>window.open('" & lUrl & "');</script>"
    End Function
    Public Function SetCurrentObj(ByVal lOBJ_ID As String, ByVal lUser As myUser) As myOBJ
        Try
            If lOBJ_ID = "" Then Return Nothing
            mUser = lUser
            m_PathValues = ""
            Dim lSql As String = "SELECT TOPIC_OBJ_OBJS.Topic_Group_ID,TOPIC_OBJ_OBJS.Topic_ID, Obj.OBJ_ID "
            lSql = lSql & ",OBJ_DISPLAY_NAME "
            lSql = lSql & ",OBJ_TABLENAME "
            lSql = lSql & ",OBJ_TABLENAME_KEY "
            lSql = lSql & ",OBJ_DESCRIPTION "
            lSql = lSql & ",REQUEST_TYPE_ID "
            lSql = lSql & ",OBJ_CLASSIFICATION_ID "
            lSql = lSql & ",RANK "
            lSql = lSql & ",Load_Data_At_Startup"
            lSql = lSql & ",SUBGROUP_ID "
            lSql = lSql & ",HelpURL "
            lSql = lSql & ", Database_OBJ.Database_Obj_Classification_ID "
            lSql = lSql & "FROM OBJ "
            lSql = lSql & " Left Join Database_OBJ on  Database_Obj.Obj_ID= OBJ.OBJ_ID "
            lSql = lSql & "left Join TOPIC_OBJ_OBJS on TOPIC_OBJ_OBJS.CHILD_Obj_ID=OBJ.OBJ_ID Where OBJ.Obj_ID='" & lOBJ_ID & "'"

            Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            mUser.Current_OBJ = New myOBJ
            If dt.Rows.Count = 0 Then
                mErrString = " Customize OBJ "
                Return Nothing
            End If
            Dim r As DataRow = dt.Rows(0)
            Dim lcurrent_OBJ As New myOBJ
            With lcurrent_OBJ
                .TOPIC_ID = r("TOPIC_ID").ToString
                .OBJ_ID = r("OBJ_ID").ToString
                .OBJ_TABLENAME = r("OBJ_TABLENAME").ToString
                .OBJ_DESCRIPTION = r("OBJ_DESCRIPTION").ToString
                .REQUEST_TYPE_ID = r("REQUEST_TYPE_ID").ToString
                .OBJ_CLASSIFICATION_ID = r("OBJ_CLASSIFICATION_ID").ToString
                .OBJ_TABLENAME_KEY = r("OBJ_TABLENAME_KEY").ToString
                .HelpUrL = r("HelpURL").ToString
                .OBJ_DISPLAY_NAME = r("OBJ_DISPLAY_NAME").ToString
                .Load_Data_At_Startup = CBool(r("Load_Data_At_Startup"))
                .DATABASE_OBJ_Classification_ID = r("Database_Obj_Classification_ID").ToString
            End With
            mUser.Current_OBJ = lcurrent_OBJ
            Return lcurrent_OBJ
        Catch ex As Exception
            mErrString = ex.Message
            Return Nothing
        End Try

    End Function
    Public Function CountAllChilds(ByVal ltrv As TreeView) As Long
        mNodes = 0
        ' If the TreeView control contains any root nodes, perform a
        ' preorder traversal of the tree and display the text of each node.
        If ltrv.Nodes.Count > 0 Then

            ' Iterate through the root nodes in the Nodes property.
            Dim i As Integer

            For i = 0 To ltrv.Nodes.Count - 1
                mNodes = mNodes + 1
                ' Display the nodes.
                DisplayChildNodeText(ltrv.Nodes(i))

            Next i
            Return mNodes
        Else
            Return 0
        End If

    End Function

    Sub DisplayChildNodeText(ByVal node As TreeNode)

        ' Display the node's text value.

        ' Iterate through the child nodes of the parent node passed into
        ' this method and display their values.
        Dim i As Integer

        For i = 0 To node.ChildNodes.Count - 1
            mNodes = mNodes + 1
            ' Recursively call the DisplayChildNodeText method to
            ' traverse the tree and display all the child nodes.
            DisplayChildNodeText(node.ChildNodes(i))

        Next i

    End Sub

    Public Function GetFieldSql(ByVal lCurrentobject As myOBJ) As String
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
        End If
        If lCurrentobject Is Nothing Then Return ""
        Dim lSqldts As String = ""

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : Add check for SYSTEM_DEPENDENT_CUSTOMIZING
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 17-Feb-2016
        If (mUser.RequestType = myUser.RequestTypeOption.insert) Then

            Dim lSysStatus As Boolean = GetSysDependentInfo(lCurrentobject.OBJ_ID)
            If (lSysStatus = True) Then
                If mUser.SUBGROUP_ID = "ALL" Then
                    lSqldts = "Select * From OBJ_FIELD where OBJ_ID='" & lCurrentobject.OBJ_ID & "' AND OBJ_FIELD_ID  <> 'APPLICATION_ID' AND DISPLAY_NAME is Not NULL Order By Ordinal_Position"
                Else
                    lSqldts = "Select * From OBJ_FIELD where OBJ_ID='" & lCurrentobject.OBJ_ID & _
                    "' AND OBJ_FIELD_ID  <> 'APPLICATION_ID' AND DISPLAY_NAME is Not NULL" & _
                    " AND (Subgroup_ID ='" & mUser.SUBGROUP_ID & "' or Subgroup_ID ='ALL') " & _
                    "  Order By Ordinal_Position"
                End If
            Else
                If mUser.SUBGROUP_ID = "ALL" Then
                    lSqldts = "Select * From OBJ_FIELD where OBJ_ID='" & lCurrentobject.OBJ_ID & "' AND DISPLAY_NAME is Not NULL Order By Ordinal_Position"
                Else
                    lSqldts = "Select * From OBJ_FIELD where OBJ_ID='" & lCurrentobject.OBJ_ID & _
                    "' AND DISPLAY_NAME is Not NULL" & _
                    " AND (Subgroup_ID ='" & mUser.SUBGROUP_ID & "' or Subgroup_ID ='ALL') " & _
                    "  Order By Ordinal_Position"
                End If
            End If
        Else
            If mUser.SUBGROUP_ID = "ALL" Then
                lSqldts = "Select * From OBJ_FIELD where OBJ_ID='" & lCurrentobject.OBJ_ID & "' AND DISPLAY_NAME is Not NULL Order By Ordinal_Position"
            Else
                lSqldts = "Select * From OBJ_FIELD where OBJ_ID='" & lCurrentobject.OBJ_ID & _
                "' AND DISPLAY_NAME is Not NULL" & _
                " AND (Subgroup_ID ='" & mUser.SUBGROUP_ID & "' or Subgroup_ID ='ALL') " & _
                "  Order By Ordinal_Position"
            End If
        End If
        ' Reference  End    : ZHHR 1053017

        Return lSqldts
    End Function
    Public Function GetTablestatement(ByVal lOBJ_ID As String, Optional ByVal lview As DisplayType = DisplayType.Grid, Optional ByVal lOrderbyCondition As String = "") As String

        Dim lValues As Array
        Dim lPair As Array
        Dim lValueString As String = " AND "
        If m_PathValues <> "" Then
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1028074 - GBOX COC - filter function not working properly 
            ' Comment           : Check if multiple filters is applied in the request values
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-05-06
            '---------------------------------------------------------------------------------------------------
            If InStr(m_PathValues, ",") <> 0 And InStr(m_PathValues, "|") Then
                lValues = m_PathValues.Split(",")
            Else
                lValues = m_PathValues.Split("/")
            End If

            For i = 0 To lValues.GetUpperBound(0)
                lPair = lValues(i).ToString.Split("|")
                If lPair.Length > 1 Then
                    If InStr(lPair(1).ToString, "*") = 0 Then
                        lValueString &= lPair(0).ToString & "='" & lPair(1).ToString & "' AND "
                    Else
                        lValueString &= lPair(0).ToString & " like '" & lPair(1).ToString.Replace("*", "%") & "' AND "
                    End If
                End If
            Next
        End If
        lValueString = lValueString.Substring(0, lValueString.Length - 4)

        mErrString = ""
        '---------------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1042186 - GBOX: check obsolete fields in table OBJ
        ' Comment   : Delete the columns OBJ_TABLENAME_SYSTEMS_KEY,OBJ_TABLENAME_TEXTS,OBJ_TABLENAME_TEXTS_LG_COL,OBJ_TABLENAME_TEXTS_LG_DESC since not used
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2015-05-18
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable("Select  OBJ_CLASSIFICATION_ID,OBJ_TABLENAME,OBJ_TABLENAME_KEY,OBJ_TABLENAME_TEXTS_KEY from OBJ where OBJ_ID='" & lOBJ_ID & "'")
        ' Reference END : CR ZHHR 1042186
        '----------------------------------------------------------------------------------------
        Dim lTablename As String = ldt.Rows(0)("OBJ_TABLENAME").ToString
        Dim lObjClassification As String = ldt.Rows(0)("OBJ_CLASSIFICATION_ID").ToString
        Dim lSQL As String = "Select "
        Dim lSqldts As String = ""

        If mErrString <> "" Then
            Return ""
        End If
        If mUser.SUBGROUP_ID = "ALL" Then
            lSqldts = "Select * From OBJ_FIELD where OBJ_ID='" & lOBJ_ID & "' AND DISPLAY_NAME is Not NULL Order By Ordinal_Position"
        Else
            lSqldts = "Select * From OBJ_FIELD where OBJ_ID='" & lOBJ_ID & _
            "' AND DISPLAY_NAME is Not NULL" & _
            " AND (Subgroup_ID ='" & mUser.SUBGROUP_ID & "' or Subgroup_ID ='ALL') " & _
            "  Order By Ordinal_Position "
        End If
        Dim ldts As DataTable = mUser.Databasemanager.MakeDataTable(lSqldts)
        If ldts.Rows.Count = 0 Then
            mErrString = "No data in OBJ_FIELDS: CUSTOMIZE OBJ_FIELDS "
            Return ""
        End If
        mKeyCollection = New List(Of myKeyObj)
        Dim lOrderfield As String = ""
        For Each r As DataRow In ldts.Rows
            If r("OBJ_Field_Type_ID") <> "CHECKBOXLIST" Then
                lSQL = lSQL & lTablename & "." & r("OBJ_FIELD_ID").ToString & " AS [" & r("DISPLAY_NAME").ToString & "], "
            End If
            If CBool(r("IsKeyMember").ToString) Then
                Dim lKey As New myKeyObj
                ' lKey.Key_ID = ldt.Rows(0)("OBJ_TABLENAME").ToString & "." & r("OBJ_FIELD_ID").ToString
                lKey.Key_ID = r("OBJ_FIELD_ID").ToString
                lKey.OrdinalPosition = CInt(r("Ordinal_Position").ToString)
                lKey.Displayname = r("DISPLAY_NAME").ToString
                If InStr(lKey.Key_ID, "OBJ_VERSIONNUMBER".ToUpper) = 0 Then
                    lOrderfield = lKey.Key_ID
                End If
                mKeyCollection.Add(lKey)
            End If
        Next
        ' Reference : YHHR 2052807 - GBOX COC: Show locked entries in DRS handbook data grid
        ' Comment   : Add the Locked field into query to check whether the value is locked
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2019-10-18
        lSQL = lSQL & lTablename & "." & "LOCKED" & " AS [" & "Locked" & "], "
        If lOrderbyCondition <> "" Then lOrderfield = lOrderbyCondition
        m_PathValues = ""
        m_Paramlist = ""

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : Added filter in below query lString
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 15-Feb-2016
        Dim lString As String = ""
        If (mUser.CocFilter = "All") Then
            'lString = " AND DELETION_FLAG  IN (0,1) "
            lString = " AND ACTIVE IN (0,1) "
        ElseIf (mUser.CocFilter = "Active" Or mUser.CocFilter = "") Then
            'lString = " AND DELETION_FLAG = 0 AND OBJ_VALID_FROM is not null AND OBJ_VALID_FROM <> '' AND OBJ_VALID_TO is not null AND OBJ_VALID_TO <> '' " & _
            '          " AND  CONCAT(DATEPART(YYYY ,GETDATE()),'-',DATEPART(MM ,GETDATE()),'-',DATEPART(DD ,GETDATE())) BETWEEN OBJ_VALID_FROM AND OBJ_VALID_TO "
            lString = " AND ACTIVE ='1' "
        ElseIf (mUser.CocFilter = "InActive") Then
            'lString = " AND OBJ_VALID_TO <= CONCAT(DATEPART(YYYY ,GETDATE()),'-',DATEPART(MM ,GETDATE()),'-',DATEPART(DD ,GETDATE())) AND (OBJ_VALID_TO is not null AND OBJ_VALID_TO <> '')"
            lString = " AND ACTIVE ='0' "
        End If
        ' Reference End     : ZHHR 1053017

        '--------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
        ' Comment   : Delete COMPOSITE_S_T object type
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-01-25


        '---------------------------------------------------------------------------------------------------
        ' Reference         : CRT 2057353 - Implementation of Filters
        ' Comment           : Done changes in existing function
        ' Added by          : Anant Jadhav (CWID : EPAEF)
        ' Date              : 17-Feb-2020

        Select Case lObjClassification
            Case "ADMINISTRATION"
                lSQL = lSQL.Substring(0, lSQL.Length - 2)
                lSQL = lSQL & " from " & lTablename
                Return lSQL

            Case "SINGLE"
                Return GetFilterData(lSQL, lTablename, lValueString, lOrderfield, lview, lOrderbyCondition)

            Case "COMPOSITE_S_T_TXT"
                Return GetFilterData(lSQL, lTablename, lValueString, lOrderfield, lview, lOrderbyCondition)

            Case "COMPOSITE_MULTI_TXT"
                Return GetFilterData(lSQL, lTablename, lValueString, lOrderfield, lview, lOrderbyCondition)
                ' Reference         : CRT 2057353 - Implementation of Filters

            Case "DOCUMENTATION"
                lSQL = "SELECT  "
                lSQL = lSQL & "OBJ_ID,TOPIC_ID, OBJ_DOCUMENTATION_FIELD_NAME AS name"
                lSQL = lSQL & ",OBJ_DOCUMENTATION_FIELD_VALUE as value "
                lSQL = lSQL & "FROM OBJ_DOCUMENTATION   where Topic_ID ='" & mUser.Current_OBJ.TOPIC_ID & "' AND OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "' Order by RANK"
                Return lSQL
            Case Else
                mErrString = "OBJ_CLASSIFICATION  not defined"
                Return ""
        End Select
        ' Reference END : CR ZHHR 1052471           
        '--------------------------------------------------------------------------------
        Return ""
    End Function

    Public Function GetDisplayStatement(ByVal lOBJ_ID As String, Optional ByVal lDisplayType As DisplayType = DisplayType.Grid) As String
        Dim lReturn As String = ""
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
        End If
        Dim lmSql As String = "Select DATABASE_OBJ_Classification_ID,Query_String_SQL,Where_Condition,Order_By_Condition from DATABASE_OBJ Where OBJ_ID ='" & lOBJ_ID & "'"

        Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable(lmSql)
        If mdt.Rows.Count = 0 Then
            mErrString = "Customize DATABASE_OBJ for " & mUser.Current_OBJ.OBJ_ID
            lReturn = ""
        End If
        mUser.Current_OBJ.DATABASE_OBJ_Classification_ID = mdt.Rows(0)("DATABASE_OBJ_Classification_ID").ToString
        If mdt.Rows(0)("DATABASE_OBJ_Classification_ID").ToString.ToUpper = "Table".ToUpper Then
            lReturn = GetTablestatement(lOBJ_ID, lDisplayType, mdt.Rows(0)("Order_By_Condition").ToString())
        Else
            Dim lsql As String = mdt.Rows(0)("Query_String_SQL").ToString
            lsql = lsql & " " & mdt.Rows(0)("Where_Condition").ToString()
            lsql = lsql & " " & mdt.Rows(0)("Order_By_Condition").ToString()
            lReturn = lsql
        End If

        Return lReturn
    End Function
    Public Function GetEmailByCwId(ByVal lCwId As String) As String
        Try
            Dim lReturndata As String = ""
            If mGums Is Nothing Then mGums = New mySQLDBMANAGER
            '--------------------------------------------------------------------
            'Reference  : ZHHR 1047321 - GBOX: change connection to CWID database
            'Comment    : Get the new GUMS DB connection sting from database
            'Added by   : Pratyusa Lenka (CWID : EOJCG)
            'Date       : 2015-08-17
            'Dim lDatabase As String = "GUMSCWIDDB"
            'Dim lServerName As String = "BY-GDCSQL056\GDC056,1450"
            'Dim lSQLCnnString As String = "Data Source=" & lServerName & ";Initial Catalog=" & lDatabase & ";Persist Security Info=FALSE;User ID='MYALB';Password='Current$MYALB$REQUEST'"
            objConn = New DatabaseConnection
            mGums.cnnString = objConn.GetConnectionString(pConstGUMSDatabase)
            ' Reference END : CR ZHHR 1047321
            '--------------------------------------------------------------------
            Dim lGumsData As DataTable = mGums.MakeDataTable("Select MAIL_ADDRESS from pub.Users5 where USer_ID ='" & lCwId & "' ")
            If lGumsData.Rows.Count > 0 Then
                lReturndata = lGumsData.Rows(0)("MAIL_ADDRESS").ToString
            Else
                lReturndata = lCwId & " is no valid cwid"
            End If
            Return lReturndata
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Private Function InsertMyUserdata(ByVal lCw_ID As String, Optional ByVal lUpdate As Boolean = False) As Boolean
        Dim lPackage As New List(Of String)
        Try
            'HACK WG GUMSBERECHTIGUNG
            ' Return True
            mErrString = ""
            If mGums Is Nothing Then mGums = New mySQLDBMANAGER
            '--------------------------------------------------------------------
            'Reference  : ZHHR 1047321 - GBOX: change connection to CWID database
            'Comment    : Get the new GUMS DB connection sting from database
            'Added by   : Pratyusa Lenka (CWID : EOJCG)
            'Date       : 2015-08-17
            'Dim lDatabase As String = "GUMSCWIDDB"
            'Dim lServerName As String = "BY-GDCSQL056\GDC056,1450"
            'Dim lSQLCnnString As String = "Data Source=" & lServerName & ";Initial Catalog=" & lDatabase & ";Persist Security Info=FALSE;User ID='MYALB';Password='Current$MYALB$REQUEST'"
            objConn = New DatabaseConnection
            mGums.cnnString = objConn.GetConnectionString(pConstGUMSDatabase)
            ' Reference END : CR ZHHR 1047321
            '--------------------------------------------------------------------
            Dim lGumsData As DataTable = mGums.MakeDataTable("Select top(1) * from pub.Users5 where USer_ID ='" & lCw_ID & "' ")

            If mGums.ErrText <> "" Then
                mErrString = mGums.ErrText
                pVarErrMsg = pVarErrMsg & " GUMS database is not available   " & mErrString & "  " & vbCrLf
                Return False
            End If
            Dim lSQL As String = ""
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1028382 - GBOX COC: Not able to access authorisation link 
            ' Comment           : Remove the duplicate CW_ID from the insert query
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-05-13
            '---------------------------------------------------------------------------------------------------
            If Not lUpdate Then
                lSQL = "INSERT INTO MDRS_USER "
                lSQL = lSQL & "([CW_ID]"
                lSQL = lSQL & ",[SUBGROUP_ID]"
                lSQL = lSQL & ",[UserRole]"
                lSQL = lSQL & ",[SMTP_EMAIL]"
                lSQL = lSQL & ",[last_name]"
                lSQL = lSQL & ",[first_name])"
                lSQL = lSQL & "VALUES"
                lSQL = lSQL & "('" & lGumsData.Rows(0)("USER_ID").ToString & "'"
                If lGumsData.Rows(0)("LEADING_SUBGROUP").ToString <> "" Then
                    lSQL = lSQL & ",'" & lGumsData.Rows(0)("LEADING_SUBGROUP").ToString & "'"
                Else
                    lSQL = lSQL & ",'" & lGumsData.Rows(0)("LEADING_SUBGROUP_COMPANY").ToString() & "'"
                End If
                lSQL = lSQL & ",'Reader'"
                lSQL = lSQL & ",'" & lGumsData.Rows(0)("MAIL_ADDRESS").ToString & "'"
                lSQL = lSQL & ",'" & lGumsData.Rows(0)("LAST_NAME").ToString & "'"
                lSQL = lSQL & ",'" & lGumsData.Rows(0)("FIRST_NAME").ToString & "')"
            Else
                lSQL = "UPDATE MDRS_USER "
                lSQL = lSQL & " SET SMTP_EMAIL = '" & lGumsData.Rows(0)("MAIL_ADDRESS").ToString & "' "
                lSQL = lSQL & ",last_name = '" & lGumsData.Rows(0)("LAST_NAME").ToString & "' "
                lSQL = lSQL & ",first_name = '" & lGumsData.Rows(0)("FIRST_NAME").ToString & "' "
                lSQL = lSQL & " WHERE [CW_ID] ='" & lCw_ID & "'"
            End If

            lPackage.Add(lSQL)

            Dim lUser As New myUser
            Dim lDone As Boolean = lUser.Databasemanager.ExecutePackage(lPackage)
            If lDone Then Return lDone
            If InStr(lUser.Databasemanager.ErrText.ToUpper, "Duplicate".ToUpper) <> 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            pVarErrMsg = pVarErrMsg & "GUMS database is not available" & ex.Message & "  " & vbCrLf
            Return True
        End Try
    End Function
    Private mCount As Long
    Public Function MakeUser(ByVal cw_id As String, Optional ByVal lUpdate As Boolean = False) As myUser
        Dim lDatabasemanager As New mySQLDBMANAGER
        Dim lPackage As New List(Of String)

        If InStr(cw_id, "\") <> 0 Then
            cw_id = cw_id.Split("\")(1)
        End If

        ' Reference         : CR YHHR 2022491 - GBOX WebForms
        ' Comment           : INC_GBox: error user data updation
        ' Added by          : Sheetal Punnapully (CWID : ETMVO)
        ' Date              : 2018-02-02


        If mGums Is Nothing Then mGums = New myGumsManager
        Dim lGums As DataTable = mGums.MakeDataTable("Select top(1) * from pub.Users5 where USer_ID ='" & cw_id.ToUpper & "' ")
        Dim str As String
        Dim sdone As Boolean

        objConn = New DatabaseConnection
        mGums.cnnString = objConn.GetConnectionString(pConstGUMSDatabase)

        If mUser Is Nothing Then
            mUser = New myUser
        End If

        If lGums.Rows.Count > 0 Then
            str = "UPDATE MDRS_USER "
            str = str & " SET SMTP_EMAIL = '" & lGums.Rows(0)("MAIL_ADDRESS").ToString & "' "
            str = str & ",last_name = '" & lGums.Rows(0)("LAST_NAME").ToString & "' "
            str = str & ",first_name = '" & lGums.Rows(0)("FIRST_NAME").ToString & "' "
            str = str & " WHERE [CW_ID] ='" & cw_id & "'"

            lPackage.Add(str)

            sdone = mUser.Databasemanager.ExecutePackage(lPackage)

        End If
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1037988 - GBOX WEBFORMS: Change GUMS DB-Connectivity Flow in Gbox Webforms
        ' Comment           : Check user is created in MDRS_USER table, if it is not created then connect to gums database and create user from gums database.
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2015-02-06
        Dim lsql As String = "Select * from MDRS_USER where CW_ID ='" & cw_id.ToUpper & "'"
        Dim lDT As DataTable = lDatabasemanager.MakeDataTable(lsql)
        Dim lDone As Boolean

        If lDT.Rows.Count > 0 Then
            For Each r As DataRow In lDT.Rows
                If mUser Is Nothing Then
                    mUser = New myUser
                End If
                With mUser
                    .AREA_ID = r("AREA_ID").ToString
                    .CW_ID = r("CW_ID").ToString
                    .SMTP_EMAIL = r("SMTP_EMAIL").ToString
                    .SUBGROUP_ID = r("SUBGROUP_ID").ToString
                    .first_name = r("first_name").ToString
                    .last_name = r("last_name").ToString
                End With
            Next r
            Return mUser
        Else
            'InsertMyUserdata(cw_id, lUpdate)

            If mGums Is Nothing Then mGums = New myGumsManager
            '--------------------------------------------------------------------
            'Reference  : ZHHR 1047321 - GBOX: change connection to CWID database
            'Comment    : Get the new GUMS DB connection sting from database
            'Added by   : Pratyusa Lenka (CWID : EOJCG)
            'Date       : 2015-08-17
            'Dim lDatabase As String = "GUMSCWIDDB"
            'Dim lServerName As String = "BY-GDCSQL056\GDC056,1450"
            'Dim lSQLCnnString As String = "Data Source=" & lServerName & ";Initial Catalog=" & lDatabase & ";Persist Security Info=FALSE;User ID='MYALB';Password='Current$MYALB$REQUEST'"
            objConn = New DatabaseConnection
            mGums.cnnString = objConn.GetConnectionString(pConstGUMSDatabase)
            ' Reference END : CR ZHHR 1047321
            '--------------------------------------------------------------------
            Dim lGumsData As DataTable = mGums.MakeDataTable("Select top(1) * from pub.Users5 where USer_ID ='" & cw_id.ToUpper & "' ")

            If lGumsData.Rows.Count > 0 Then

                lsql = "INSERT INTO MDRS_USER "
                lsql = lsql & "([CW_ID]"
                lsql = lsql & ",[SUBGROUP_ID]"
                lsql = lsql & ",[UserRole]"
                lsql = lsql & ",[SMTP_EMAIL]"
                lsql = lsql & ",[last_name]"
                lsql = lsql & ",[first_name])"
                lsql = lsql & "VALUES"
                lsql = lsql & "('" & lGumsData.Rows(0)("USER_ID").ToString & "'"
                If lGumsData.Rows(0)("LEADING_SUBGROUP").ToString <> "" Then
                    lsql = lsql & ",'" & lGumsData.Rows(0)("LEADING_SUBGROUP").ToString & "'"
                Else
                    lsql = lsql & ",'" & lGumsData.Rows(0)("LEADING_SUBGROUP_COMPANY").ToString() & "'"
                End If
                lsql = lsql & ",'Reader'"
                lsql = lsql & ",'" & lGumsData.Rows(0)("MAIL_ADDRESS").ToString & "'"
                lsql = lsql & ",'" & lGumsData.Rows(0)("LAST_NAME").ToString & "'"
                lsql = lsql & ",'" & lGumsData.Rows(0)("FIRST_NAME").ToString & "')"

                If mUser Is Nothing Then
                    mUser = New myUser
                End If
                With mUser
                    .AREA_ID = ""
                    .CW_ID = lGumsData.Rows(0)("USER_ID").ToString
                    .SMTP_EMAIL = lGumsData.Rows(0)("MAIL_ADDRESS").ToString
                    .SUBGROUP_ID = lGumsData.Rows(0)("LEADING_SUBGROUP").ToString
                    .first_name = lGumsData.Rows(0)("FIRST_NAME").ToString
                    .last_name = lGumsData.Rows(0)("LAST_NAME").ToString
                End With
            Else
                pVarErrMsg = "An authentification error occurred. Please create a support ticket for GBOX as described here: http://by-gbox.bayer-ag.com/HOTLINE/"
                Return Nothing
            End If

            lPackage.Add(lsql)

            lDone = mUser.Databasemanager.ExecutePackage(lPackage)
            If (lDone) Then Return mUser

        End If

        If (lDone) Then
            Return mUser
        Else
            Return Nothing
        End If
        'Reference End      : ZHHR 1037988

        'If InsertMyUserdata(cw_id, lUpdate) Then
        '    Dim lsql As String = "Select * from MDRS_USER where CW_ID ='" & cw_id.ToUpper & "'"
        '    Dim lDT As DataTable = lDatabasemanager.MakeDataTable(lsql)
        '    For Each r As DataRow In lDT.Rows
        '        If mUser Is Nothing Then
        '            mUser = New myUser
        '        End If
        '        With mUser
        '            .AREA_ID = r("AREA_ID").ToString
        '            .CW_ID = r("CW_ID").ToString
        '            .SMTP_EMAIL = r("SMTP_EMAIL").ToString
        '            .SUBGROUP_ID = r("SUBGROUP_ID").ToString
        '            '.UserRole = r("UserRole").ToString
        '            '.WINDOWS_DOMAIN = r("WINDOWS_DOMAIN").ToString
        '            .first_name = r("first_name").ToString
        '            .last_name = r("last_name").ToString
        '        End With
        '    Next r
        '    Return mUser
        'Else
        '    ' ----------------------------------------------------------------------------------------
        '    ' Reference  : BY-RZ04 - CMT - 28633 - 110IM08222136,110IM08175309 - "Unknown User error in accessing GBOX"
        '    ' Comment    : Added code for new User if User is nothing
        '    ' Created by : EQIZU
        '    ' Date       : 06-NOV-2013
        '    ' ---------------------------------------------------------------------------------------
        '    'mUser.CW_ID = x(0) & "\" & x(1)
        '    'mUser.CW_ID = x(1).ToString
        '    'mUser.SUBGROUP_ID = "unknown"
        '    Return Nothing
        'End If

    End Function

    Public Function IsQualifiedUser(ByVal cw_id As String) As myUser
        Try
            mErrString = ""
            If InStr(cw_id, "\") <> 0 Then
                cw_id = cw_id.Split("\")(1)
            End If
            Dim lDatabasemanager As New mySQLDBMANAGER

            Dim lsql As String = "Select * from MDRS_USER where CW_ID ='" & cw_id.ToUpper & "'"
            Dim lDT As DataTable = lDatabasemanager.MakeDataTable(lsql)
            If lDT Is Nothing Then
                If mErrString = "" Then
                    Return MakeUser(cw_id)
                Else
                    ' ----------------------------------------------------------------------------------------
                    ' Reference  : BY-RZ04 - CMT - 28633 - 110IM08222136,110IM08175309 - "Unknown User error in accessing GBOX"
                    ' Comment    : To retrieve details of user from User table error occured
                    ' Created by : EQIZU
                    ' Date       : 06-NOV-2013
                    ' ---------------------------------------------------------------------------------------
                    pVarErrMsg = pVarErrMsg & "User (" & cw_id & ") :  Not there in GBOX database" & mErrString & "  |  " & vbCrLf
                    Return Nothing
                End If
            End If
            If lDT.Rows.Count = 0 Then
                If mErrString = "" Then
                    Return MakeUser(cw_id)
                Else
                    ' ----------------------------------------------------------------------------------------
                    ' Reference  : BY-RZ04 - CMT - 28633 - 110IM08222136,110IM08175309 - "Unknown User error in accessing GBOX"
                    ' Comment    : Exception occured in isQualifiedUser
                    ' Created by : EQIZU
                    ' Date       : 06-NOV-2013
                    ' ---------------------------------------------------------------------------------------
                    pVarErrMsg = pVarErrMsg & "User Id (" & cw_id & ") Not there in GBOX database : " & mErrString & "  " & vbCrLf
                    Return Nothing
                End If
            Else
                Return MakeUser(cw_id, True)
            End If

        Catch ex As Exception
            mErrString = "isQualifiedUser:" & ex.Message

            ' ----------------------------------------------------------------------------------------
            ' Reference  : BY-RZ04 - CMT - 28633 - 110IM08222136,110IM08175309 - "Unknown User error in accessing GBOX"
            ' Comment    : Exception occured in isQualifiedUser
            ' Created by : EQIZU
            ' Date       : 06-NOV-2013
            ' ---------------------------------------------------------------------------------------
            pVarErrMsg = pVarErrMsg & "User Id (" & cw_id & ") is not known : " & mErrString & "   " & vbCrLf

            Return Nothing
        End Try
    End Function
    Private Function AuthsetExitsts(ByVal lAuthSet As AuthorizationSet) As Boolean
        Dim lsql As String = "Select * from AUTHORISATION_SET where [APPLICATION_ID]='" & lAuthSet.Application & "' " & _
                                                                    "And [APPLICATION_PART_ID]='" & lAuthSet.Applicationpart & "' " & _
                                                                    "And [APPLICATION_ROLE_ID]='" & lAuthSet.Applicationrole & "' " & _
                                                                    "And [CW_ID]='" & lAuthSet.CW_ID & "' " & _
                                                                    "AND SUBGROUP_ID='" & lAuthSet.Subgroup & "' " & _
                                                                    "AND ORG_LEVEL_ID='" & lAuthSet.Orglevel_ID & "' " & _
                                                                    "AND ORG_LEVEL_VALUE='" & lAuthSet.Orglevel_Value & "' "
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        If ldt.Rows.Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Function MakeUserList(ByVal lRequestedUsers As List(Of myRequestedUser)) As String
        mUsertext = ""
        For Each lRequestedUser As myRequestedUser In lRequestedUsers
            mUsertext = mUsertext & lRequestedUser.CW_ID & ","
        Next lRequestedUser
        Return mUsertext
    End Function
    Public Sub MakeImplementationBlock(ByVal lRequestedUsers As List(Of myRequestedUser))
        For Each lRequestedUser As myRequestedUser In lRequestedUsers
            For Each lAuthset As AuthorizationSet In lRequestedUser.Authorizationsets
                If lAuthset.Implementationtext <> "No action required; GBox user data created automatically" Then
                    If AuthSetExists(lAuthset) Then
                        Dim lApplication As myApplication = GetReQuestedApplication(lAuthset.Application)
                        Dim lAuthsetstring As String = lAuthset.Application & ";" & lAuthset.Applicationpart & ";" & lAuthset.Applicationrole & ";" & lAuthset.CW_ID
                        lApplication.Implementationblock = lApplication.Implementationblock & vbCrLf & "AUTHORISATION SET (" & lAuthsetstring & ") ALREADY IN CONCEPT DATABASE"
                    End If
                End If
            Next
            For Each lAuthset As AuthorizationSet In lRequestedUser.Authorizationsets
                If lAuthset.Implementationtext <> "No action required; GBox user data created automatically" Then
                    If Not AuthSetExists(lAuthset) Then
                        Dim lApplication As myApplication = GetReQuestedApplication(lAuthset.Application)
                        lApplication.Implementationblock = lApplication.Implementationblock & vbCrLf & lAuthset.Implementationtext.Replace("|", "''").Replace("~", vbCrLf)
                    End If
                End If
            Next lAuthset
        Next lRequestedUser
    End Sub

    Public Function MakeAccessText(ByVal lRequestedUser As myRequestedUser) As String
        Dim lReportText As String = ""
        For Each lAuthset As AuthorizationSet In lRequestedUser.Authorizationsets
            Dim lAccessText As String = ""
            Dim lImplementationtext As String = ""
            With lAuthset
                If Not lAccessText.Contains(.Application) Then
                    lAccessText = lAccessText & .Application & "(" & .Applicationtext & ")" & vbCrLf
                End If
                If Not lAccessText.Contains(.Applicationpart) Then
                    lAccessText = lAccessText & "   " & .Applicationpart & "(" & .ApplicationPartText & ")" & vbCrLf
                End If
                lAccessText = lAccessText & "         " & .Applicationrole & "(" & .ApplicationroleText & ")" & vbCrLf

                If .Implementationtext = "No action required; GBox user data created automatically" Then
                    lImplementationtext = lImplementationtext & "            " & .Implementationtext & vbCrLf
                Else
                    lImplementationtext = lImplementationtext & "            " & " see implementation block below." & vbCrLf
                End If

                Dim lApplication As myApplication = GetReQuestedApplication(lAuthset.Application)
                If Not lApplication.SmeText.Contains(mHeadText) Then
                    lApplication.SmeText = lApplication.SmeText & mHeadText
                End If
                If Not lApplication.Implementationtext.Contains(mHeadText) Then
                    lApplication.Implementationtext = lApplication.Implementationtext & mHeadText
                End If
                lApplication.Implementationtext = lApplication.Implementationtext & lAccessText & vbCrLf & lImplementationtext & vbCrLf
                lApplication.SmeText = lApplication.SmeText & lAccessText & vbCrLf
                lApplication.Authorizationsets.Add(lAuthset)
                lReportText = lReportText & lAccessText
            End With
        Next lAuthset
        Return lReportText
    End Function
    Public Function Get_MDRS_DATABASE_MAINTENANCE_STRING() As String
        Dim myTable As DataTable = mUser.Databasemanager.MakeDataTable("Select * from MDRS_INFO where MDRS_INFO_APPLICATION = 'GBOX_MANAGER'")
        For Each lRow As DataRow In myTable.Rows
            If lRow("MDRS_INFO_ID").ToString = "MDRS_DATABASE_MAINTENANCE_END" Then
                Return lRow("MDRS_INFO_VALUE").ToString
            End If
        Next
        Return "UNKNOWN"
    End Function
    Public Function GetWikiURL() As String
        Dim myTable As DataTable = mUser.Databasemanager.MakeDataTable("Select * from MDRS_INFO where MDRS_INFO_APPLICATION = 'GBOX_FORMS'")
        For Each lRow As DataRow In myTable.Rows
            If lRow("MDRS_INFO_ID").ToString = "GPG_WIKI_PATH" Then
                Return lRow("MDRS_INFO_VALUE").ToString
            End If
        Next
        Return "UNKNOWN"
    End Function
    Public Function GetWikiEXT() As String
        Dim myTable As DataTable = mUser.Databasemanager.MakeDataTable("Select * from MDRS_INFO where MDRS_INFO_APPLICATION = 'GBOX_FORMS'")
        For Each lRow As DataRow In myTable.Rows
            If lRow("MDRS_INFO_ID").ToString = "GPG_WIKI_EXTENTION" Then
                Return lRow("MDRS_INFO_VALUE").ToString
            End If
        Next
        Return "UNKNOWN"
    End Function
    'Public Function GET_MDRS_DATABASE_MAINTENANCE() As Boolean
    '    If mUser Is Nothing Then
    '        mUser = pCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
    '    End If

    '    Dim myTable As DataTable = mUser.Databasemanager.MakeDataTable("Select * from MDRS_INFO where MDRS_INFO_APPLICATION = 'GBOX_MANAGER'")
    '    For Each lRow As DataRow In myTable.Rows
    '        If lRow("MDRS_INFO_ID").ToString = "MDRS_DATABASE_MAINTENANCE" Then
    '            If lRow("MDRS_INFO_VALUE").ToString = "1" Then
    '                Return True
    '            Else
    '                Return False
    '            End If
    '        End If
    '    Next
    '    Return False
    'End Function


    Public Function CHECK_MDRS_DATABASE_MAINTENANCE() As Boolean
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
        End If

        Dim myTable As DataTable = mUser.Databasemanager.MakeDataTable("Select * from MDRS_INFO where MDRS_INFO_APPLICATION = 'GBOX_MANAGER'")
        For Each lRow As DataRow In myTable.Rows
            If lRow("MDRS_INFO_ID").ToString = "MDRS_DATABASE_MAINTENANCE" Then
                If lRow("MDRS_INFO_VALUE").ToString = "1" Then
                    Return True
                Else
                    Return False
                End If
            End If
        Next
        Return False
    End Function
    Private Function GetReQuestedApplication(ByVal lApplicationname As String) As myApplication
        If mRequestedApplication Is Nothing Then mRequestedApplication = New List(Of myApplication)
        Dim lapplicationToReturn As myApplication = Nothing
        For Each lvalApplicationname As myApplication In mRequestedApplication
            If lvalApplicationname.Applicationname = lApplicationname Then
                lapplicationToReturn = lvalApplicationname
            End If
        Next
        If lapplicationToReturn Is Nothing Then
            lapplicationToReturn = New myApplication
            lapplicationToReturn.Applicationname = lApplicationname
            mRequestedApplication.Add(lapplicationToReturn)
        End If
        Return lapplicationToReturn
    End Function


    Public Sub MakeAuthSetPackageForApps(ByVal lRequestedUsers As List(Of myRequestedUser))
        For Each lRequestedUser As myRequestedUser In lRequestedUsers
            For Each lAuthset As AuthorizationSet In lRequestedUser.Authorizationsets
                If Not AuthsetExitsts(lAuthset) Then
                    With lAuthset

                        '---------------------------------------------------------------------------------------------------
                        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
                        ' Comment           : Removed columns LAST_LOGON, VALID_FROM and VALID_TO fields from table [AUTHORISATION_SET]
                        ' Added by          : Milind Randive (CWID : EOJCH)
                        ' Date              : 2014-12-15

                        Dim lSql As String = "INSERT INTO AUTHORISATION_SET "
                        lSql = lSql & " (APPLICATION_ID "
                        lSql = lSql & " ,APPLICATION_PART_ID "
                        lSql = lSql & " ,APPLICATION_ROLE_ID "
                        lSql = lSql & " ,SUBGROUP_ID "
                        lSql = lSql & " ,CW_ID "
                        lSql = lSql & " ,AUTH_STATE_ID "
                        lSql = lSql & " ,ORG_LEVEL_ID "
                        lSql = lSql & " ,ORG_LEVEL_VALUE )"
                        lSql = lSql & " VALUES (" & _
                           "'" & .Application & "'," & _
                           "'" & .Applicationpart & "'," & _
                           "'" & .Applicationrole & "'," & _
                           "'" & .Subgroup & "'," & _
                           "'" & .CW_ID & "'," & _
                           "'ACTIVE'," & _
                           "'" & .Orglevel_ID & "'," & _
                           "'" & .Orglevel_Value & "'" & ")"

                        ' Reference  END    : CR ZHHR 1035817

                        Dim lApplication As myApplication = GetReQuestedApplication(lAuthset.Application)
                        lApplication.ImplementationPack.Add(lSql)
                    End With
                End If
            Next
        Next lRequestedUser
    End Sub

    Public Sub MakeRoleBackPackageForApps(ByVal lRequestedUsers As List(Of myRequestedUser))
        For Each lRequestedUser As myRequestedUser In lRequestedUsers
            For Each lAuthset As AuthorizationSet In lRequestedUser.Authorizationsets
                If Not AuthsetExitsts(lAuthset) Then
                    With lAuthset

                        '---------------------------------------------------------------------------------------------------
                        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELDS FROM DB AND CODE
                        ' Comment           : Removed columns LAST_LOGON, VALID_FROM and VALID_TO fields from table [AUTHORISATION_SET]
                        ' Added by          : Milind Randive (CWID : EOJCH)
                        ' Date              : 2014-12-15

                        Dim lsql As String = "Delete from AUTHORISATION_SET where " & _
                           "APPLICATION_ID=" & _
                           "'" & .Application & "' AND " & _
                           " APPLICATION_PART_ID=" & _
                           "'" & .Applicationpart & "' AND " & _
                            " APPLICATION_ROLE_ID=" & _
                           "'" & .Applicationrole & "' AND " & _
                           " CW_ID=" & _
                           "'" & .CW_ID & "' AND " & _
                           " CW_ID=" & _
                           "'" & .CW_ID & "' AND " & _
                           " AUTH_STATE_ID=" & _
                           "'" & .State & "' AND " & _
                           " SUBGROUP_ID=" & _
                           "'" & .Subgroup & "' AND " & _
                           " ORG_LEVEL_ID=" & _
                           "'" & .Orglevel_ID & "' AND " & _
                           " ORG_LEVEL_VALUE=" & _
                           "'" & .Orglevel_Value & "'"

                        ' Reference  END    : CR ZHHR 1035817
                        Dim lApplication As myApplication = GetReQuestedApplication(lAuthset.Application)
                        lApplication.RollbackPack.Add(lsql)
                    End With
                End If
            Next
        Next lRequestedUser

    End Sub

    Public Function GetGBOXId(Optional ByVal lPrestring As String = "", Optional ByVal lPoststring As String = "_DRS") As String
        '140106_1324_XX_DRS resp. 140106_1324_XX_DRS_M
        Dim lStr As String = ""
        lStr = Right("00" & Now.Year.ToString, 2)
        lStr = lStr & Right("00" & Now.Month.ToString, 2)
        lStr = lStr & Right("00" & Now.Day.ToString, 2)
        lStr = lStr & "_" & Right("00" & Now.Hour, 2)
        lStr = lStr & Right("00" & Now.Minute, 2)
        Dim lEnd As String = ""
        lEnd = lEnd & Right("00" & Now.Second, 2)
        lEnd = lEnd & Right("0000" & Now.Millisecond, 4)
        lEnd = Right("00" & Quersumme(lEnd), 2)
        lStr = lStr & "_" & lEnd
        mUser.Current_Request_ID = lPrestring & lStr & lPoststring
        Dim lReturn As String = lPrestring & lStr & lPoststring
        Return Left(lReturn, 20)
    End Function

    Private Function Quersumme(ByVal Zahl As String) As String
        Dim Sum As Integer = 0
        For Each C As Char In Zahl.ToCharArray
            Sum += CInt(C.ToString)
        Next
        Return Sum.ToString
    End Function
    ''' <summary>
    ''' Reference : CR ZHHR 1052355 - GBOX COC: OTT 1809 - GBOX pMDAS workflow change
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date      : 2016-01-12
    ''' </summary>
    ''' <returns>Customized PMDAS Message</returns>
    ''' <remarks>If CUSTOMIZED_BY_PMDAS is false for the object return the Customized PMDAS Message for pop-up and email</remarks>
    Public Function GetCustomizedPMDASMessage() As String
        Dim lstApplicationID As New List(Of String)
        Dim strApplicationID As String = ""
        Dim strCustomizedPMDASMessage As String = ""
        Try
            Dim dtObjApp As DataTable = mUser.Databasemanager.MakeDataTable("SELECT APPLICATION_ID FROM OBJ_APPLICATION WHERE OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "' AND CUSTOMIZED_BY_PMDAS = 0")
            If Not dtObjApp Is Nothing And dtObjApp.Rows.Count > 0 Then
                For Each drRow As DataRow In dtObjApp.Rows
                    lstApplicationID.Add(drRow("APPLICATION_ID").ToString)
                Next
                strApplicationID = String.Join(",", lstApplicationID.ToArray)
                strCustomizedPMDASMessage = "Please be aware that changes of object " & mUser.Current_OBJ.OBJ_ID & " will not be implemented in system " & strApplicationID & " by MDAS team. An HPSM incident will be created for the responsible solver team."
            End If
        Catch ex As Exception
            mErrString = ex.Message
            Return ""
        End Try
        Return strCustomizedPMDASMessage
    End Function

    '-------------------------------------------------------------------------------
    ' Reference : CR ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
    ' Comment   : Check System Dependent Flag
    ' Added by  : Milind Randive (CWID : EOJCH)
    ' Date      : 17-Jan-2016
    Public Function GetSysDependentInfo(ByVal strObjId As String) As Boolean
        Dim bSysDependentInfo As Boolean = False
        Try
            Dim dtObjApp As DataTable = mUser.Databasemanager.MakeDataTable("SELECT SYSTEM_DEPENDENT_CUSTOMIZING FROM obj WHERE OBJ_ID = '" & strObjId & "'")
            If Not dtObjApp Is Nothing And dtObjApp.Rows.Count > 0 Then
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
    Public Function CheckEntryExist(ByVal strObjId As String, ByVal lKeyAppId As String, ByVal lKey As String) As Boolean
        Dim bCheckEntryExist As Boolean = False
        Try
            Dim lSql As String = "Select OBJ_TABLENAME, OBJ_TABLENAME_KEY  from obj where obj_id = '" & strObjId & "'"
            Dim dtObj As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            Dim lTblnm As String = ""
            Dim lTbKey As String = ""

            If Not dtObj Is Nothing And dtObj.Rows.Count > 0 Then
                lTblnm = dtObj.Rows(0)("OBJ_TABLENAME").ToString
                lTbKey = dtObj.Rows(0)("OBJ_TABLENAME_KEY").ToString
                lSql = " SELECT OBJ_VERSIONNUMBER  FROM " & lTblnm & " WHERE APPLICATION_ID='" & lKeyAppId & "' AND " & lTbKey & "='" & lKey & "'"
                Dim dtObjApp As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
                If Not dtObjApp Is Nothing And dtObjApp.Rows.Count > 0 Then
                    bCheckEntryExist = True
                Else
                    bCheckEntryExist = False
                End If
            End If
        Catch ex As Exception
            Return ""
        End Try

        Return bCheckEntryExist

    End Function
    ''' <summary>
    ''' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
    ''' Comment   : Checks the key values exist in the customizing table
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2018-10-17
    ''' </summary>
    ''' <param name="strObjId"></param>
    ''' <param name="strWhereString"></param>
    ''' <returns>True if exists</returns>
    ''' <remarks>False when not exists</remarks>
    Public Function CheckEntryExist(ByVal strObjId As String, ByVal strWhereString As String) As Boolean
        Dim bCheckEntryExist As Boolean = False
        Try
            Dim lSql As String = "Select OBJ_TABLENAME, OBJ_TABLENAME_KEY  from obj where obj_id = '" & strObjId & "'"
            Dim dtObj As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            Dim lTblnm As String = ""
            Dim lTbKey As String = ""

            If Not dtObj Is Nothing And dtObj.Rows.Count > 0 Then
                lTblnm = dtObj.Rows(0)("OBJ_TABLENAME").ToString
                lTbKey = dtObj.Rows(0)("OBJ_TABLENAME_KEY").ToString
                lSql = " SELECT OBJ_VERSIONNUMBER  FROM " & lTblnm & " WHERE " & strWhereString & ""
                Dim dtObjApp As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
                If Not dtObjApp Is Nothing And dtObjApp.Rows.Count > 0 Then
                    bCheckEntryExist = True
                Else
                    bCheckEntryExist = False
                End If
            End If
        Catch ex As Exception
            Return True
        End Try

        Return bCheckEntryExist

    End Function
    Public Function GetTableKey(ByVal strObjId As String) As String
        Dim lTbKey As String = ""
        Try
            Dim lSql As String = "Select OBJ_TABLENAME, OBJ_TABLENAME_KEY  from obj where obj_id = '" & strObjId & "'"
            Dim dtObj As DataTable = mUser.Databasemanager.MakeDataTable(lSql)

            If Not dtObj Is Nothing And dtObj.Rows.Count > 0 Then
                lTbKey = dtObj.Rows(0)("OBJ_TABLENAME_KEY").ToString
            End If
        Catch ex As Exception
            Return ""
        End Try

        Return lTbKey

    End Function
    ' Reference End : CR ZHHR 1053017


    Private mErrString As String

    Public Property ErrString() As String
        Get
            Return mErrString
        End Get
        Set(ByVal value As String)
            mErrString = value
        End Set
    End Property


    Private mKeyCollection As List(Of myKeyObj)

    Public Property KeyCollection() As List(Of myKeyObj)
        Get
            Return mKeyCollection
        End Get
        Set(ByVal value As List(Of myKeyObj))
            mKeyCollection = value
        End Set
    End Property



    Private mUser As myUser

    Public Property User() As myUser
        Get
            Return mUser
        End Get
        Set(ByVal value As myUser)
            mUser = value
        End Set
    End Property



    Public Property Paramlist As String
        Get
            Return m_Paramlist
        End Get
        Set(ByVal value As String)
            m_Paramlist = value
        End Set
    End Property

    Public Property LinkServer As String
        Get
            Return m_LinkServer
        End Get
        Set(ByVal value As String)
            m_LinkServer = value
        End Set
    End Property

    Public Property PathValues As String
        Get
            Return m_PathValues
        End Get
        Set(ByVal value As String)
            m_PathValues = value
        End Set
    End Property

    ' Reference End : CR ZHHR 1053017

    ' Reference         : CRT 2057353 - Implementation of Filters
    ' Comment           : Done changes in existing function
    ' Added by          : Anant Jadhav (CWID : EPAEF)
    ' Date              : 26-Feb-2020

    ' Change the code as per Revised SQL query
    ' Date              : 31-Mar-2020
    ' Change the code as per revised SQL query and also added 'ACTIVE' column check to existing where condition
    ' Date              : 24-Apr-2020

    ' Reference         : CRT 2066302 - Error in Filter functionality
    ' Comment           : Done changes in existing function for Inactive and All flag
    ' Added by          : Kanchan Bhor (CWID : ELNFD)
    ' Date              : 24-Sep-2020

    Public Function GetFilterData(ByVal lSQL As String, ByVal lTablename As String, ByVal lValueString As String, ByVal lOrderfield As String, Optional ByVal lview As DisplayType = DisplayType.Grid, Optional ByVal lOrderbyCondition As String = "") As String
        If lview = DisplayType.Grid Then
            lSQL = lSQL.Substring(0, lSQL.Length - 2)
            lSQL = lSQL & " from " & lTablename

            Dim ltbldt As DataTable = mUser.Databasemanager.MakeDataTable("Select top(0) * from " & lTablename)
            Dim delFlag As Boolean = False
            Dim validFromDateFlag As Boolean = False
            Dim validToDateFlag As Boolean = False
            Dim lsqlNew As String = ""

            If ltbldt.Columns.Contains("DELETION_FLAG") Then
                delFlag = True
            End If

            If ltbldt.Columns.Contains("OBJ_VALID_FROM") Then
                validFromDateFlag = True
            End If

            If ltbldt.Columns.Contains("OBJ_VALID_TO") Then
                validToDateFlag = True
            End If

            Select Case mUser.CocFilter.ToUpper
                Case "INACTIVE"
                    If delFlag Then
                        lsqlNew = lsqlNew & "(DELETION_FLAG = 1) "
                    End If

                    If validFromDateFlag And validToDateFlag Then
                        If lsqlNew <> "" Then
                            lsqlNew = lsqlNew & " OR (CONVERT(VARCHAR,GETDATE(),121) NOT BETWEEN " & _
                                    "CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_FROM IS NULL OR OBJ_VALID_FROM = '') THEN CONVERT(VARCHAR, '01-01-1900', 121) ELSE OBJ_VALID_FROM END), 121) AND" & _
                                    " CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_TO IS NULL OR OBJ_VALID_TO = '') THEN CONVERT(VARCHAR, '31-12-9999', 121) ELSE OBJ_VALID_TO END),121))"
                        Else
                            lsqlNew = " OR (CONVERT(VARCHAR,GETDATE(),121) NOT BETWEEN " & _
                                    "CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_FROM IS NULL OR OBJ_VALID_FROM = '') THEN CONVERT(VARCHAR, '01-01-1900', 121) ELSE OBJ_VALID_FROM END), 121) AND" & _
                                    " CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_TO IS NULL OR OBJ_VALID_TO = '') THEN CONVERT(VARCHAR, '31-12-9999', 121) ELSE OBJ_VALID_TO END),121))"
                        End If
                    ElseIf validFromDateFlag Then
                        If lsqlNew <> "" Then
                            lsqlNew = lsqlNew & " OR (CONVERT(VARCHAR,GETDATE(),121) NOT BETWEEN " & _
                                    "CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_FROM IS NULL OR OBJ_VALID_FROM = '') THEN CONVERT(VARCHAR, '01-01-1900', 121) ELSE OBJ_VALID_FROM END), 121) AND" & _
                                    " CONVERT(VARCHAR, '31-12-9999',121))"
                        Else
                            lsqlNew = " OR (CONVERT(VARCHAR,GETDATE(),121) NOT BETWEEN " & _
                                    "CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_FROM IS NULL OR OBJ_VALID_FROM = '') THEN CONVERT(VARCHAR, '01-01-1900', 121) ELSE OBJ_VALID_FROM END), 121) AND" & _
                                    " CONVERT(VARCHAR, '31-12-9999',121))"
                        End If
                    ElseIf validToDateFlag Then
                        If lsqlNew <> "" Then
                            lsqlNew = lsqlNew & " OR (CONVERT(VARCHAR,GETDATE(),121) NOT BETWEEN " & _
                                    "CONVERT(VARCHAR,'01-01-1900', 121) AND" & _
                                    " CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_TO IS NULL OR OBJ_VALID_TO = '') THEN CONVERT(VARCHAR, '31-12-9999', 121) ELSE OBJ_VALID_TO END),121))"
                        Else
                            lsqlNew = " OR (CONVERT(VARCHAR,GETDATE(),121) NOT BETWEEN " & _
                                    "CONVERT(VARCHAR,'01-01-1900', 121) AND" & _
                                    " CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_TO IS NULL OR OBJ_VALID_TO = '') THEN CONVERT(VARCHAR, '31-12-9999', 121) ELSE OBJ_VALID_TO END),121))"
                        End If
                    End If

                    If lsqlNew <> "" Then    'CRT-2066302 : For Filter functionality bug fix--by Kanchan Bhor
                        lSQL = lSQL & " Where ((ACTIVE = 1) AND (" & lsqlNew & "))" & lValueString & " Order by " & lOrderfield 'Added code to resolve bug of filters on Search result on 21-May-2020 By Anant Jadhav(EPAEF)
                    Else
                        lSQL = lSQL & " Where (ACTIVE = 1) " & lValueString & " Order by " & lOrderfield 'Added code for applying filters on Search result on 07-May-2020 By Anant Jadhav(EPAEF)
                    End If

                Case "ACTIVE"
                    If delFlag Then
                        lsqlNew = lsqlNew & "(DELETION_FLAG = 0) "
                    End If

                    If validFromDateFlag And validToDateFlag Then
                        If lsqlNew <> "" Then
                            lsqlNew = lsqlNew & " AND (CONVERT(VARCHAR,GETDATE(),121) BETWEEN " & _
                                    "CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_FROM IS NULL OR OBJ_VALID_FROM = '') THEN CONVERT(VARCHAR, '01-01-1900', 121) ELSE OBJ_VALID_FROM END), 121) AND" & _
                                    " CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_TO IS NULL OR OBJ_VALID_TO = '') THEN CONVERT(VARCHAR, '31-12-9999', 121) ELSE OBJ_VALID_TO END),121))"
                        Else
                            lsqlNew = " AND (CONVERT(VARCHAR,GETDATE(),121) BETWEEN " & _
                                    "CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_FROM IS NULL OR OBJ_VALID_FROM = '') THEN CONVERT(VARCHAR, '01-01-1900', 121) ELSE OBJ_VALID_FROM END), 121) AND" & _
                                    " CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_TO IS NULL OR OBJ_VALID_TO = '') THEN CONVERT(VARCHAR, '31-12-9999', 121) ELSE OBJ_VALID_TO END),121))"
                        End If
                    ElseIf validFromDateFlag Then
                        If lsqlNew <> "" Then
                            lsqlNew = lsqlNew & " AND (CONVERT(VARCHAR,GETDATE(),121) BETWEEN " & _
                                    "CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_FROM IS NULL OR OBJ_VALID_FROM = '') THEN CONVERT(VARCHAR, '01-01-1900', 121) ELSE OBJ_VALID_FROM END), 121) AND" & _
                                    " CONVERT(VARCHAR, '31-12-9999',121))"
                        Else
                            lsqlNew = " AND (CONVERT(VARCHAR,GETDATE(),121) BETWEEN " & _
                                    "CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_FROM IS NULL OR OBJ_VALID_FROM = '') THEN CONVERT(VARCHAR, '01-01-1900', 121) ELSE OBJ_VALID_FROM END), 121) AND" & _
                                    " CONVERT(VARCHAR, '31-12-9999',121))"
                        End If
                    ElseIf validToDateFlag Then
                        If lsqlNew <> "" Then
                            lsqlNew = lsqlNew & " AND (CONVERT(VARCHAR,GETDATE(),121) BETWEEN " & _
                                    "CONVERT(VARCHAR,'01-01-1900', 121) AND" & _
                                    " CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_TO IS NULL OR OBJ_VALID_TO = '') THEN CONVERT(VARCHAR, '31-12-9999', 121) ELSE OBJ_VALID_TO END),121))"
                        Else
                            lsqlNew = " AND (CONVERT(VARCHAR,GETDATE(),121) BETWEEN " & _
                                    "CONVERT(VARCHAR,'01-01-1900', 121) AND" & _
                                    " CONVERT(VARCHAR, (CASE WHEN (OBJ_VALID_TO IS NULL OR OBJ_VALID_TO = '') THEN CONVERT(VARCHAR, '31-12-9999', 121) ELSE OBJ_VALID_TO END),121))"
                        End If
                    End If

                    If lsqlNew <> "" Then
                        lSQL = lSQL & " Where (ACTIVE = 1) AND (" & lsqlNew & ")" & lValueString & " Order by " & lOrderfield
                    Else
                        lSQL = lSQL & " Where (ACTIVE = 1) " & lValueString & " Order by " & lOrderfield 'Added code for applying filters on Search result on 07-May-2020 By Anant Jadhav(EPAEF)
                    End If

                Case "ALL"
                    lSQL = lSQL & " Where (ACTIVE = 1)" & lValueString & " Order by " & lOrderfield   'CRT-2066302 : For Filter functionality Bug fix--by Kanchan Bhor

            End Select
            Return lSQL
        Else
            lSQL = lSQL.Substring(0, lSQL.Length - 2)
            lSQL = lSQL & " from " & lTablename
            Return lSQL
        End If
    End Function
    ' Reference         : CRT 2057353 - Implementation of Filters


End Class
