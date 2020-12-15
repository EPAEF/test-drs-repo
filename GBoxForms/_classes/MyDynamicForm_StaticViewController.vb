Option Strict Off
Public Class MyDynamicForm_StaticViewController
    Private mRequestform As Object
    Private mUser As myUser
    Private mtrvTree As TreeView
    Private mGrd As GridView
    Private mlblCountData As Label
    Public Event UpdateTree()
    Public Event ErrorInfo(ByVal lErrText As String)
    Public Event AddNew(ByVal lEnabled As Boolean)
    Public Event Delete(ByVal lEnabled As Boolean)
    Public Event XML(ByVal lEnabled As Boolean)
    Public Event Save(ByVal lEnabled As Boolean)
    Public Event Cancel()
    Private mApplicationPartnode As TreeNode
    Private mIsPostback As Boolean
    Private mRequest As HttpRequest
    '--------------------------------------------------------------------------------------
    ' Reference : CR ZHHR 1063723 - GBOX AUTH: Comment is missing in authorisation requests
    ' Comment   : Property to hold the value of AUTH request comment
    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ' Date      : 2016-10-26
    Private mStrComment As String
    Public Property Comment() As String
        Get
            Return mStrComment
        End Get
        Set(ByVal value As String)
            mStrComment = value
        End Set
    End Property

    '-------------------------------------------------------------------
    ' Reference : CR ZHHR 1038879 - GBOX AUTH MGMT: GBOX auth management
    ' Comment   : Property to hold the value of checkbox
    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ' Date      : 2015-04-06
    Private mIsAdminRole As Boolean
    Public Property IsAdminRole() As Boolean
        Get
            Return mIsAdminRole
        End Get
        Set(ByVal value As Boolean)
            mIsAdminRole = value
        End Set
    End Property
    ' Reference END : CR ZHHR 1038879
    '--------------------------------------------------------------------
    Sub Resetgrid()
        Try
            mGrd.DataSource = Nothing
            mGrd.DataBind()
            mlblCountData.Text = mGrd.Rows.Count & " user found."
        Catch ex As Exception
            RaiseEvent ErrorInfo("MyDynamicForm_StaticViewController:Resetgrid:" & ex.Message)
        End Try
    End Sub
    Public Sub RaiseErrorInfo(ByVal lText As String)
        RaiseEvent ErrorInfo(lText)
    End Sub
    Public Function fillobjects(ByRef llstUsersToCheck As ListBox, ByVal lLabelText As String, ByVal lSubgroup As String, ByVal lOrglevel_Id As String, ByVal lOrglevel_Value As String) As String
        pObjRequestedUsers = Nothing
        If pObjRequestedUsers Is Nothing Then pObjRequestedUsers = New List(Of myRequestedUser)
        For Each lItem As ListItem In llstUsersToCheck.Items
            Dim lRequestedUser As New myRequestedUser
            lRequestedUser.CW_ID = lItem.Text
            pObjRequestedUsers.Add(lRequestedUser)
            makeAuthSetByText(lLabelText, lItem.Text, lSubgroup, lOrglevel_Id, lOrglevel_Value)
        Next lItem

        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELD FROM DB AND CODE
        ' Comment           : Remove title from database and code
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2014-11-26

        Dim lReportAll As String = "Dear " & mUser.first_name & " " & mUser.last_name & "," & vbCrLf & " this is your request:" & vbCrLf
        ' Reference  END    : CR ZHHR 1035817
        lReportAll = lReportAll & "User " & mUser.CW_ID & " has requested access for " & vbCrLf & " Orggroup: " & lSubgroup & vbCrLf & "Orglevel_ID:" & lOrglevel_Id & " Orglevel:" & lOrglevel_Value & " user(s): " & vbCrLf
        lReportAll = lReportAll & mUser.GBOXmanager.MakeUserList(pObjRequestedUsers) & vbCrLf
        lReportAll = lReportAll & "for " & vbCrLf
        mUser.GBOXmanager.HeadText = lReportAll
        lReportAll = lReportAll & mUser.GBOXmanager.MakeAccessText(pObjRequestedUsers(0))
        Return lReportAll
    End Function
    Private Function RequestedOrIncomplete(ByVal lApplication_ID As String, ByVal lCW_ID As String) As String
        Dim lDt As DataTable = mUser.Databasemanager.MakeDataTable("Select Needs_Qualified_User from APPLICATION where Application_ID='" & lApplication_ID & "'")
        For Each r As DataRow In lDt.Rows
            If r("Needs_Qualified_User") Then
                Dim luserT As DataTable = mUser.Databasemanager.MakeDataTable("Select * from MDRS_USER where CW_ID ='" & lCW_ID & "'")
                If luserT.Rows.Count = 0 Then
                    Return "incomplete"
                Else
                    Return "active"
                End If

            Else
                Return "active"
            End If
        Next
        Return ""
    End Function
    Private Function GetUserById(ByVal lUser As String) As myRequestedUser
        For Each lItem As myRequestedUser In pObjRequestedUsers
            If lUser = lItem.CW_ID Then
                Return lItem
            End If
        Next
        Return Nothing
    End Function
    Private Function GetImplementationText(ByVal lApp As String, ByVal lApplicationpart As String, ByVal lApplicationRole As String) As String
        Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable("Select Implementationtext from APPLICATION_ROLE where APPLICATION_ID='" & lApp & "' and APPLICATION_PART_ID='" & lApplicationpart & "' And APPLICATION_ROLE_ID ='" & lApplicationRole & "'")
        Return mdt.Rows(0)("Implementationtext").ToString
    End Function
    Sub makeAuthSetByText(ByVal lText As String, ByVal lUser As String, ByVal lSubgroup As String, ByVal lOrglevel_Id As String, ByVal lOrglevel As String)
        Dim lRequesteduser As myRequestedUser = GetUserById(lUser)
        Dim lAuth As New AuthorizationSet
        With lAuth
            .Application = lText.Split("/")(0).Split("(")(0).ToString.Trim
            .Applicationtext = lText.Split("/")(0).Split("(")(1).ToString.Trim
            .Applicationpart = lText.Split("/")(1).Split("(")(0).ToString.Trim
            .ApplicationPartText = lText.Split("/")(1).Split("(")(1).ToString.Trim
            .Applicationrole = lText.Split("|")(1).Split("(")(0).ToString.Trim
            .CW_ID = lUser
            .State = RequestedOrIncomplete(.Application, lUser)
            .Subgroup = lSubgroup
            .Orglevel_ID = lOrglevel_Id
            .Orglevel_Value = lOrglevel
            
            If Not mUser.GBOXmanager.AuthSetExists(lAuth) Then
                .Implementationtext = GetImplementationText(.Application, .Applicationpart, .Applicationrole).Replace("USERNAME", lUser) '.Replace("|", "'").Replace("~", vbCrLf)
            Else
                .Implementationtext = .CW_ID & " is already " & .Applicationrole & " in " & .Applicationpart & ". No action required."
            End If
            .Subgroup = lSubgroup
        End With
        If Not mUser.GBOXmanager.AuthSetExists(lAuth) Then
            lRequesteduser.Authorizationsets.Add(lAuth)
        End If
    End Sub
    Public Sub SelectedNodeChange(ByVal lUser As myUser)
        Try
            Dim lFirstcolumns As String = ""
            Dim lsql As String = ""
            Dim mdt As DataTable
            mUser = lUser
            If mtrvTree.SelectedNode Is Nothing Then
                Resetgrid()
                Exit Sub
            End If
            With mtrvTree.SelectedNode
                If .Parent Is Nothing Then
                    Resetgrid()
                    RaiseEvent AddNew(False)
                    RaiseEvent XML(False)
                    Exit Sub
                End If
                If .Parent.Parent Is Nothing Then
                    lsql = "Select Application_Type from Application where Application_ID = '" & mtrvTree.SelectedNode.Parent.Text.Split("(")(0).ToString.Trim & "'"
                    mdt = mUser.Databasemanager.MakeDataTable(lsql)
                    If mdt.Rows(0)("Application_Type").ToString = "DOCUMENTATION" Then
                        lFirstcolumns = " AUTHORISATION_SET.ORG_LEVEL_ID as ORG_LEVEL_ID,AUTHORISATION_SET.ORG_LEVEL_VALUE as ORGLEVELVALUE "
                    Else
                        lFirstcolumns = " AUTHORISATION_SET.Subgroup_ID as subgroup, AUTHORISATION_SET.ORG_LEVEL_ID as [section type] , AUTHORISATION_SET.ORG_LEVEL_VALUE as [section value] "
                    End If
                    Dim lValueArray As Object = .ValuePath.Split("/")
                    Dim lAPPLICATION_ID As String = lValueArray(0).ToString.Split("(")(0).Trim
                    Dim lAPPLICATION_PART_ID As String = lValueArray(1).ToString.Split("(")(0).Trim

                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELD FROM DB AND CODE
                    ' Comment           : Remove title from database and code
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 2014-11-26

                    lsql = "Select " & lFirstcolumns & " ,AUTHORISATION_SET.CW_ID as [cw id],MDRS_USER.first_name,MDRS_USER.last_name ,MDRS_USER.SMTP_EMAIL from AUTHORISATION_SET " & _
                                        " left join  MDRS_USER on AUTHORISATION_SET.CW_ID = MDRS_USER.CW_ID" & _
                                        " Where APPLICATION_PART_ID='" & lAPPLICATION_PART_ID & "'" & _
                                        " And APPLICATION_ID = '" & lAPPLICATION_ID & "' And AUTHORISATION_SET.CW_ID is not null"
                    ' Reference  END    : CR ZHHR 1035817

                    mGrd.DataSource = mUser.Databasemanager.MakeDataTable(lsql)
                    mGrd.DataBind()
                    RaiseEvent XML(True)
                    Exit Sub
                End If
                If .Parent.Parent.Parent Is Nothing Then
                    Dim lValueArray As Object = .ValuePath.Split("/")
                    Dim lAPPLICATION_ID As String = lValueArray(0).ToString.Split("(")(0).Trim
                    Dim lAPPLICATION_PART_ID As String = lValueArray(1).ToString.Split("(")(0).Trim
                    Dim lRole As String = .Text.Split("(")(0).Trim
                    lsql = "Select Application_Type from Application where Application_ID = '" & lAPPLICATION_ID & "'"
                    mdt = mUser.Databasemanager.MakeDataTable(lsql)
                    If mdt.Rows(0)("Application_Type").ToString = "DOCUMENTATION" Then
                        lFirstcolumns = " AUTHORISATION_SET.ORG_LEVEL_ID as ORG_LEVEL_ID,AUTHORISATION_SET.ORG_LEVEL_VALUE as ORGLEVELVALUE "
                    Else
                        lFirstcolumns = "  AUTHORISATION_SET.Subgroup_ID as subgroup, AUTHORISATION_SET.ORG_LEVEL_ID as [section type] , AUTHORISATION_SET.ORG_LEVEL_VALUE as [section value] "
                    End If

                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELD FROM DB AND CODE
                    ' Comment           : Remove title from database and code
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 2014-11-26
                    lsql = "Select " & lFirstcolumns & " ,AUTHORISATION_SET.CW_ID as [cw id],MDRS_USER.first_name,MDRS_USER.last_name ,MDRS_USER.SMTP_EMAIL from AUTHORISATION_SET " & _
                                                            " left join  MDRS_USER on AUTHORISATION_SET.CW_ID = MDRS_USER.CW_ID " & _
                                                            " where APPLICATION_ROLE_ID='" & lRole & "'" & _
                                                            " And APPLICATION_PART_ID='" & lAPPLICATION_PART_ID & "'" & _
                                                            " And APPLICATION_ID = '" & lAPPLICATION_ID & "' And AUTHORISATION_SET.CW_ID is not null"
                    ' Reference  END    : CR ZHHR 1035817
                    mGrd.DataSource = mUser.Databasemanager.MakeDataTable(lsql)
                    mGrd.DataBind()
                    RaiseEvent XML(True)
                    RaiseEvent AddNew(True)
                Else
                    Dim lValueArray As Object = .ValuePath.Split("/")
                    Dim lAPPLICATION_ID As String = lValueArray(0).ToString.Split("(")(0).Trim
                    Dim lAPPLICATION_PART_ID As String = lValueArray(1).ToString.Split("(")(0).Trim
                    Dim lRole As String = .Text.Split("(")(0).Trim
                    lsql = "Select Application_Type from Application where Application_ID = '" & lAPPLICATION_ID & "'"
                    mdt = mUser.Databasemanager.MakeDataTable(lsql)
                    If mdt.Rows(0)("Application_Type").ToString = "DOCUMENTATION" Then
                        lFirstcolumns = " AUTHORISATION_SET.ORG_LEVEL_ID as ORG_LEVEL_ID,AUTHORISATION_SET.ORG_LEVEL_VALUE as ORGLEVELVALUE "
                    Else
                        lFirstcolumns = "  AUTHORISATION_SET.Subgroup_ID as subgroup, AUTHORISATION_SET.ORG_LEVEL_ID as [section type] , AUTHORISATION_SET.ORG_LEVEL_VALUE as [section value] "
                    End If
                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELD FROM DB AND CODE
                    ' Comment           : Remove title from database and code
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 2014-11-26
                    lsql = "Select " & lFirstcolumns & " ,AUTHORISATION_SET.CW_ID as [cw id],MDRS_USER.first_name,MDRS_USER.last_name ,MDRS_USER.SMTP_EMAIL from AUTHORISATION_SET " & _
                                                            " left outer join  MDRS_USER on AUTHORISATION_SET.CW_ID = MDRS_USER.CW_ID" & _
                                                            " where APPLICATION_ROLE_ID='" & lRole & "'" & _
                                                            " And APPLICATION_PART_ID='" & lAPPLICATION_PART_ID & "'" & _
                                                            " And APPLICATION_ID = '" & lAPPLICATION_ID & "' order by AUTHORISATION_SET.CW_ID"
                    ' Reference  END    : CR ZHHR 1035817
                    mGrd.DataSource = mUser.Databasemanager.MakeDataTable(lsql)
                    mGrd.DataBind()
                    RaiseEvent XML(True)
                    RaiseEvent AddNew(True)
                End If
                If mGrd.Rows.Count = 0 Then
                    RaiseEvent Delete(False)
                Else
                    RaiseEvent Delete(True)
                End If
            End With
            mlblCountData.Text = mGrd.Rows.Count & " user found"
        Catch ex As Exception
            RaiseEvent ErrorInfo("MyDynamicForm_StaticViewController:SelectednodeChange:" & ex.Message)
        End Try
    End Sub
    Public Property Requestform() As Object
        Get
            Return mRequestform
        End Get
        Set(ByVal value As Object)
            mRequestform = value
        End Set
    End Property
    Public Sub New(ByVal lUser As myUser, ByVal lRequestform As Object, ByRef ltrv As TreeView, ByRef lgrd As GridView, ByRef llblCountData As Label)
        mRequestform = lRequestform
        mtrvTree = ltrv
        mGrd = lgrd
        mlblCountData = llblCountData
        mUser = lUser
    End Sub
    '---------------------------------------------------------------------------------------------------
    ' Reference         : ZHHR 1049322 - GBOX Webforms OTT 1048: New Workflow for MDG
    ' Comment           : New Constructor for creating the object of this class
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 27-Oct-2015
    Public Sub New()

    End Sub
    ' Reference         : ZHHR 1049322
    Public Sub updategrid(ByVal lgrd As GridView, Optional ByVal lSQl As String = "vw_GBox_Cockpit_role_by_authgroup", Optional ByVal orderBy As String = "MARTY_ID", Optional ByVal lSortdirection As String = "ASC")
        Try

            With lgrd
                .DataSource = mUser.databasemanager.MakeDataTable(lSQl & " Order by " & orderBy & " " & lSortdirection)
                .DataBind()
            End With
        Catch ex As Exception
            RaiseEvent ErrorInfo("MyDynamicForm_StaticViewController:updategrid: " & ex.Message)
        End Try
    End Sub

    Public ReadOnly Property CONTEXT()
        Get
            Return pObjCurrentUsers.CONTEXT
        End Get

    End Property

    Sub loadtrvvwGBoxAuthorizationDetailsOverview(Optional ByVal lSQL As String = "SELECT [APPLICATION_ID],[Description] FROM [APPLICATION] Where Application_Type='APPLICATION' Or APPLICATION_TYPE='DOCUMENTATION'")
        Try
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
            End If
            mtrvTree.Nodes.Clear()
            Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSQL)
            For Each r As DataRow In dt.Rows
                Dim myNode As New TreeNode(r("Application_ID").ToString & " (" & r("Description").ToString & ")")
                myNode.ToolTip = r("Description").ToString
                myNode.SelectAction = TreeNodeSelectAction.SelectExpand
                If Not Me.IsPostback Then
                    If Not Request Is Nothing Then
                        If Not Request.Params("APPLICATION") Is Nothing Then
                            If myNode.Text.Split("(")(0).Trim.ToUpper = Request.Params("APPLICATION").ToString.ToUpper Then
                                myNode.Selected = True
                                myNode.Expanded = True
                            End If
                        End If
                    End If
                End If
                mtrvTree.Nodes.Add(myNode)
                myNode.ImageUrl = "~/Images/database-16x16.gif"
                LoadApplicationParts(myNode)
            Next
            mtrvTree.CollapseAll()
            If Not mApplicationPartnode Is Nothing Then
                mApplicationPartnode.Selected = True
                mApplicationPartnode.Expanded = True
                mApplicationPartnode.Parent.Expanded = True
            End If
        Catch ex As Exception
            RaiseEvent ErrorInfo("MyDynamicForm_StaticViewController:loadtrvvwGBoxAuthorizationDetailsOverview:" & ex.Message)
        End Try
    End Sub
    Public Property IsPostback() As Boolean
        Get
            Return mIsPostback
        End Get
        Set(ByVal value As Boolean)
            mIsPostback = value
        End Set
    End Property
    Public Property Request() As HttpRequest
        Get
            Return mRequest
        End Get
        Set(ByVal value As HttpRequest)
            mRequest = value
        End Set
    End Property
    Sub LoadApplicationParts(ByVal lNode As TreeNode)
        Try
            Dim lSql As String = "Select APPLICATION_PART_ID,Description From dbo.APPLICATION_PARTS Where APPLICATION_ID='" & lNode.Text.Split("(")(0).Trim & "'"
            Dim dt As DataTable = mUser.databasemanager.MakeDataTable(lSql)
            For Each r As DataRow In dt.Rows
                Dim myNode As New TreeNode(r("APPLICATION_PART_ID").ToString & " (" & r("Description").ToString & ")")
                myNode.ToolTip = r("Description").ToString
                myNode.ImageUrl = "~/Images/part-16x16.gif"
                myNode.SelectAction = TreeNodeSelectAction.SelectExpand
                If Not Me.IsPostback Then
                    If Not Request Is Nothing Then
                        If Not Request.Params("APPLICATIONPART") Is Nothing Then
                            If myNode.Text.Split("(")(0).Trim.ToUpper = Request.Params("APPLICATIONPART").ToString.ToUpper Then
                                myNode.Selected = True
                                myNode.Expanded = True
                                mApplicationPartnode = myNode
                            End If
                        End If
                    End If
                End If
                lNode.ChildNodes.Add(myNode)
                '-------------------------------------------------------------------
                ' Reference : CR ZHHR 1038879 - GBOX AUTH MGMT: GBOX auth management
                ' Comment   : Load administrative roles based on checkbox value
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-04-06
                If mIsAdminRole Then
                    LoadAdministrationRoles(myNode)
                Else
                    LoadApplicationRoles(myNode, myNode.Parent.Text.Split("(")(0).Trim, myNode.Text.Split("(")(0).Trim)
                End If
                ' Reference END : CR ZHHR 1038879
                '--------------------------------------------------------------------
            Next
        Catch ex As Exception
            RaiseEvent ErrorInfo("MyDynamicForm_StaticViewController:LoadApplicationParts:" & ex.Message)
        End Try
    End Sub
    Sub LoadAdministrationRoles(ByVal lNode As TreeNode)
        Try
            Dim lsql As String = "SELECT  [Application_ROLE_ID],[Description],APPLICATION_ROLE_CLASSIFICATION_ID " & _
                   "FROM APPLICATION_ROLE " & _
                   "where APPLICATION_ID='" & lNode.Parent.Text.Split("(")(0).Trim & "'" & _
                   " And APPLICATION_PART_ID='" & lNode.Text.Split("(")(0).Trim & "'" & _
                   " And ADMINISTRATION_ROLE_ID='LUC'"
            Dim dt As DataTable = mUser.databasemanager.MakeDataTable(lsql)
            For Each r As DataRow In dt.Rows
                Dim myNode As New TreeNode(r("Application_ROLE_ID").ToString & " (" & r("Description").ToString & ")")
                myNode.ToolTip = r("Description").ToString
                If r("APPLICATION_ROLE_CLASSIFICATION_ID").ToString.ToUpper <> "Application".ToUpper Then
                    myNode.ImageUrl = "~/Images/marty-16x16-3.gif"
                Else
                    myNode.ImageUrl = "~/Images/gbox-16x16-3.gif"
                End If
                myNode.SelectAction = TreeNodeSelectAction.SelectExpand
                lNode.ChildNodes.Add(myNode)
                LoadRoles(myNode, lNode.Parent.Text.Split("(")(0).Trim, lNode.Text.Split("(")(0).Trim)
            Next
        Catch ex As Exception
            RaiseEvent ErrorInfo("MyDynamicForm_StaticViewController:LoadAdministrationRoles:" & ex.Message)
        End Try
    End Sub
    ''' <summary>
    ''' Reference : CR ZHHR 1038879 - GBOX AUTH MGMT: GBOX auth management
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2015-04-06
    ''' </summary>
    ''' <param name="lNode"></param>
    ''' <param name="lApplication_ID"></param>
    ''' <param name="lApplicationpart_ID"></param>
    ''' <remarks>Genarate tree node other than [APPLICATION_ROLE_CLASSIFICATION_ID] = "Administration"</remarks>
    Sub LoadApplicationRoles(ByVal lNode As TreeNode, ByVal lApplication_ID As String, ByVal lApplicationpart_ID As String)
        Try
            Dim strSQLRole As String = "SELECT [APPLICATION_ROLE_ID],[Description],APPLICATION_ROLE_CLASSIFICATION_ID FROM APPLICATION_ROLE " & _
                                       " where [ADMINISTRATION_ROLE_ID] ='ADMINISTRATOR'" & _
                                       " and    [APPLICATION_ID]='" & lApplication_ID & "'" & _
                                       " AND    [APPLICATION_PART_ID]='" & lNode.Text.Split("(")(0).Trim & "'	"
            Dim dtRoles As DataTable = mUser.Databasemanager.MakeDataTable(strSQLRole)
            For Each drRole As DataRow In dtRoles.Rows
                If drRole("APPLICATION_ROLE_CLASSIFICATION_ID").ToString.ToUpper = "ADMINISTRATION" Then
                    Dim lSQl As String = "SELECT [APPLICATION_ROLE_ID],[Description],APPLICATION_ROLE_CLASSIFICATION_ID FROM APPLICATION_ROLE " & _
                                       " where [ADMINISTRATION_ROLE_ID] ='" & drRole("APPLICATION_ROLE_ID").ToString & "'" & _
                                       " and    [APPLICATION_ID]='" & lApplication_ID & "'" & _
                                       " AND    [APPLICATION_PART_ID]='" & lApplicationpart_ID & "'	"

                    Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSQl)
                    For Each r As DataRow In dt.Rows
                        Dim myNode As New TreeNode(r("Application_ROLE_ID").ToString & " (" & r("Description").ToString & ")")
                        myNode.ToolTip = r("Description").ToString
                        Select Case r("APPLICATION_ROLE_CLASSIFICATION_ID").ToString.ToUpper
                            Case "Application".ToUpper
                                myNode.ImageUrl = "~/Images/gbox-16x16-3.gif"
                            Case "Contacts".ToUpper
                                myNode.ImageUrl = "~/Images/marty-16x16-1.gif"
                        End Select
                        lNode.ChildNodes.Add(myNode)
                    Next
                Else
                    Dim myNode As New TreeNode(drRole("Application_ROLE_ID").ToString & " (" & drRole("Description").ToString & ")")
                    myNode.ToolTip = drRole("Description").ToString
                    Select Case drRole("APPLICATION_ROLE_CLASSIFICATION_ID").ToString.ToUpper
                        Case "Application".ToUpper
                            myNode.ImageUrl = "~/Images/gbox-16x16-3.gif"
                        Case "Contacts".ToUpper
                            myNode.ImageUrl = "~/Images/marty-16x16-1.gif"
                    End Select
                    lNode.ChildNodes.Add(myNode)
                End If

            Next
        Catch ex As Exception
            RaiseEvent ErrorInfo("MyDynamicForm_StaticViewController:LoadRoles:" & ex.Message)
        End Try

    End Sub
    Sub LoadRoles(ByVal lNode As TreeNode, ByVal lApplication_ID As String, ByVal lApplicationpart_ID As String)
        Try
            Dim lSQl As String = "SELECT [APPLICATION_ROLE_ID],[Description],APPLICATION_ROLE_CLASSIFICATION_ID FROM APPLICATION_ROLE " & _
                                       " where [ADMINISTRATION_ROLE_ID] ='" & lNode.Text.Split("(")(0).Trim & "'" & _
                                       " and    [APPLICATION_ID]='" & lApplication_ID & "'" & _
                                       " AND    [APPLICATION_PART_ID]='" & lApplicationpart_ID & "'	"
            Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSQl)
            For Each r As DataRow In dt.Rows
                Dim myNode As New TreeNode(r("Application_ROLE_ID").ToString & " (" & r("Description").ToString & ")")
                myNode.ToolTip = r("Description").ToString
                Select Case r("APPLICATION_ROLE_CLASSIFICATION_ID").ToString.ToUpper
                    Case "Application".ToUpper
                        myNode.ImageUrl = "~/Images/gbox-16x16-3.gif"
                    Case "Administration".ToUpper
                        myNode.ImageUrl = "~/Images/marty-16x16-3.gif"
                    Case "Contacts".ToUpper
                        myNode.ImageUrl = "~/Images/marty-16x16-1.gif"
                End Select
                lNode.ChildNodes.Add(myNode)
            Next
            For Each llNode As TreeNode In lNode.ChildNodes
                'If InStr(llNode.Text, "GOV") <> 0 Then Stop
                LoadRoles(llNode, lApplication_ID, lApplicationpart_ID)
            Next
        Catch ex As Exception
            RaiseEvent ErrorInfo("MyDynamicForm_StaticViewController:LoadRoles:" & ex.Message)
        End Try
    End Sub
    Sub CreateMailingList(ByRef lResponse As HttpResponse)
        Try
            Dim myPath As Array = mtrvTree.SelectedNode.ValuePath.Split("/")
            If myPath.GetUpperBound(0) < 2 Then Exit Sub
            Dim lApplication As String = myPath(0).ToString.Split("(")(0).Trim.ToString
            Dim lApplicationPart As String = myPath(1).ToString.Split("(")(0).Trim.ToString
            Dim lApplicationRole As String = myPath(myPath.GetUpperBound(0)).ToString.Split("(")(0).Trim.ToString

            Dim lSelectedNode As TreeNode = mtrvTree.SelectedNode
            If lSelectedNode Is Nothing Then
                mlblCountData.Text = "please select tree node !"
                Exit Sub
            End If
            Dim lsql As String = ""
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELD FROM DB AND CODE
            ' Comment           : Remove title from database and code
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-11-26
            lsql = "Select AUTHORISATION_SET.CW_ID as [cw id],MDRS_USER.first_name,MDRS_USER.last_name ,MDRS_USER.SMTP_EMAIL from AUTHORISATION_SET " & _
                                                            " left outer join  MDRS_USER on AUTHORISATION_SET.CW_ID = MDRS_USER.CW_ID" & _
                                                            " where APPLICATION_ROLE_ID='" & lApplicationRole & "'" & _
                                                            " And APPLICATION_PART_ID='" & lApplicationPart & "'" & _
                                                            " And APPLICATION_ID = '" & lApplication & "' order by AUTHORISATION_SET.CW_ID"
            ' Reference  END    : CR ZHHR 1035817
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
            End If


            Dim lDt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
            If lDt Is Nothing Then
                mlblCountData.Text = "No valid user selected."
                Exit Sub
            End If
            If lDt.Rows.Count = 0 Then
                mlblCountData.Text = "No valid user selected."
                Exit Sub
            End If
            Dim lStr As String = ""
            lResponse.ContentType = "text/txt"
            lResponse.AppendHeader("Content-Disposition", "attachment; filename=" & "Mailinglist.txt")
            For Each r As DataRow In lDt.Rows
                If r("SMTP_EMAIL").ToString <> "" Then
                    lStr = lStr & (r("SMTP_EMAIL").ToString & ", " & vbCrLf)
                End If
            Next
            lResponse.Write(lStr)
            lResponse.Flush()
            lResponse.End()
        Catch ex As Exception
            RaiseEvent ErrorInfo("MyDynamicForm_StaticViewController:CreateMailingList:" & ex.Message)
        End Try
    End Sub
    Function SubmitCancelUser(lApplicationpart As String) As String
        'Deletedata()
        Dim nstrSql As String
        ' mUser.Package
        Dim lPackage As New List(Of String)
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1048850 - GBOX Filter settings OTT 1048: New Workflow for Filter settings
        ' Comment           : Considered the Current Request id and added the status string 
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 10-Nov-2015
        Dim mStatus As String = ""
        Dim lGboxId As String = mUser.Current_Request_ID
        ' Reference         : ZHHR 1048850 

        'lPackage.AddRange(mUser.Package)
        Dim lapp As String = "GBOX"
        If mUser.RequestText.Contains("BARDO") Then
            lapp = "BARDO"
        End If

        ' Reference         : ZHHR 1054647 - GBOX Webforms: GBOX disable LOG_TABLE_USER (old workflow)
        ' Comment           : comments below code for LOG_TABLE_USER
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 14-Mar-2016
        'nstrSql = " INSERT INTO [LOG_TABLE_USER] " & _
        '             "([MARTY_ID],[CW_ID],[SMETEXT],[Customer],[Customer_Date],[Implementation_State], [Request_Type_Id]" & _
        '             " )VALUES ('" & lGboxId & "'," & _
        '             "N'" & mUser.CW_ID & "','" & mUser.RequestText.Replace(",", ";").Replace("'", "''") & "'," & "'" & mUser.CW_ID & "','" & Now.Year & "_" & Now.Month & "_" & Now.Day & "_" & Now.Hour & "_" & Now.Minute & "_" & Now.Second & "_" & Now.Millisecond & "'," & _
        '             "'submitted','GBOX_REQUEST')"
        'lPackage.Add(nstrSql)
        ' Reference End        : ZHHR 1054647

        If InStr(mUser.Databasemanager.cnnString, "MDRS_Q") = 0 And InStr(mUser.Databasemanager.cnnString, "MDRS_Q") = 0 Then


            nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                        "([M_MAILKEY]" & _
                        ",[M_RECIPIENTS]" & _
                        ",[M_SUBJECT]" & _
                        ",[M_BODY]" & _
                        ",[M_CURRENT_SENDER])" & _
                        " VALUES('" & lGboxId & "','" & mUser.SMTP_EMAIL & "','GBOX_REQUEST:" & lGboxId & "'," & _
                        "N'" & mUser.RequestText.Replace(",", ";").Replace("'", "''") & "','G_BOX')"
        Else
            nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                                        "([M_MAILKEY]" & _
                                        ",[M_RECIPIENTS]" & _
                                        ",[M_SUBJECT]" & _
                                        ",[M_BODY]" & _
                                        ",[M_CURRENT_SENDER])" & _
                                        " VALUES('" & lGboxId & "','" & mUser.SMTP_EMAIL & "','TESTREQUEST IGNORE IT:GBOX_REQUEST:" & lGboxId & "'," & _
                                        "N'" & mUser.RequestText.Replace(",", ";").Replace("'", "''") & "','G_BOX')"

        End If

        lPackage.Add(nstrSql)

        '---------------------------------------------------------------------------------------
        'Save Hotlinemail
        mUser.RequestText = mUser.RequestText & vbCrLf

        If InStr(mUser.Databasemanager.cnnString, "MDRS_Q") = 0 And InStr(mUser.Databasemanager.cnnString, "MDRS_Q") = 0 Then
            nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                               "([M_MAILKEY]" & _
                               ",[M_RECIPIENTS]" & _
                               ",[M_SUBJECT]" & _
                               ",[M_BODY]" & _
                               ",[M_CURRENT_SENDER])" & _
                               " VALUES('" & lGboxId & "','" & mUser.GBOXmanager.GetRecipientByApplicationPart(lApplicationpart) & "','GBOX_REQUEST:" & lGboxId & "'," & _
                                "N'" & mUser.RequestText & "','" & mUser.CW_ID & "')"
        Else
            nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                               "([M_MAILKEY]" & _
                               ",[M_RECIPIENTS]" & _
                               ",[M_SUBJECT]" & _
                               ",[M_BODY]" & _
                               ",[M_CURRENT_SENDER])" & _
                               " VALUES('" & lGboxId & "','" & mUser.GBOXmanager.GetRecipientByApplicationPart(lApplicationpart) & "','TESTREQUEST, PLEASE IGNORE:    GBOX_REQUEST:" & lGboxId & "'," & _
                                "N'" & mUser.RequestText & "','" & mUser.CW_ID & "')"

        End If
        lPackage.Add(nstrSql)

        'save RollBackPacks
        For Each lSql As String In mUser.RollbackPack
            nstrSql = "INSERT INTO LOG_TABLE_ROLLBACK_SQL " & _
                      "([Request_ID]" & _
                        ",[Statement_SQL])" & _
                        " VALUES ( '" & lGboxId & "', '" & lSql.Replace("'", "''") & "')"
            lPackage.Add(nstrSql)
        Next lSql
        lPackage.AddRange(mUser.Package)

        If mUser.Databasemanager.ExecutePackage(lPackage) Then
            RaiseEvent ErrorInfo("Your request " & lGboxId & " has been submitted.")
            mStatus = "Your request " & lGboxId & " has been submitted."
        Else
            RaiseEvent ErrorInfo("Following error occured:" & lGboxId & vbCrLf & mUser.Databasemanager.ErrText)
            mStatus = "Your request is not submitted."
        End If
        Try
            pObjCurrentUsers.RemoveUser(mUser.CW_ID)
        Catch

        End Try
        Return mStatus
    End Function
    Function SubmitInsertUser(lApplicationpart As String) As String
        Dim mStatus As String = ""

        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
        End If
        'mUser.GBOXmanager.RequestedApplications = Nothing
        ''Nach Application
        Dim lPackage As New List(Of String)
        Dim lGboxId As String = mUser.GBOXmanager.GetGBOXId


        mUser.GBOXmanager.MakeAuthSetPackageForApps(pObjRequestedUsers)
        mUser.GBOXmanager.MakeRoleBackPackageForApps(pObjRequestedUsers)
        mUser.GBOXmanager.MakeImplementationBlock(pObjRequestedUsers)
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1048850 - GBOX Filter settings OTT 1048: New Workflow for Filter settings
        ' Comment           : New Workflow for Filter settings
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 21-Oct-2015
        If (mUser.GBOXmanager.RequestedApplications Is Nothing) Then

            mStatus = "User(s) is already created. No action required."

        Else

        lGboxId = Replace(lGboxId, "_DRS", "") & "_AUTH"
            'Added iCount below for IM0002691013 by EOJCH, as it is urgent.
            Dim iCount As Integer = 0

        For Each lApplication As myApplication In mUser.GBOXmanager.RequestedApplications

            For Each lRequestedUser As myRequestedUser In pObjRequestedUsers
                For Each lAuthset As AuthorizationSet In lRequestedUser.Authorizationsets
                        With lAuthset
                            If iCount = 0 Then
                                '---------------------------------------------------------------------------------------------
                                ' Reference     : CR ZHHR 1063723 - GBOX AUTH: Comment is missing in authorisation requests 
                                ' Comment       : Add AUTH request comment to display in confirmation mail and update DB table
                                ' Added by      : Pratyusa Lenka (CWID : EOJCG) 
                                ' Date          : 2016-10-26
                                If Not lApplication.SmeText.Contains(mStrComment) Then
                                    lApplication.SmeText = lApplication.SmeText & vbCrLf & mStrComment
                                End If
                                ' Reference END : CR ZHHR 1063723 
                                '---------------------------------------------------------------------------------------------
                                lPackage.AddRange(wf_MakePackage("AUTH_" & lApplication.Applicationname, lAuthset.Orglevel_ID, lAuthset.Orglevel_Value, lGboxId, lApplication.SmeText.ToString))
                            End If
                            iCount += 1
                        End With
                Next
            Next
            ' Reference End     : ZHHR 1048850

                'SAVE Workflowdata
                Dim nstrSql As String = ""
                ' Reference         : ZHHR 1054647 - GBOX Webforms: GBOX disable LOG_TABLE_USER (old workflow)
                ' Comment           : comments below code for LOG_TABLE_USER
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 14-Mar-2016
                'Dim nstrSql As String = " INSERT INTO [LOG_TABLE_USER] " & _
                '         "([MARTY_ID],[CW_ID],[SMETEXT],[Customer],[Customer_Date],[Implementation_State], [Request_Type_Id]" & _
                '         " )VALUES ('" & lGboxId & "'," & _
                '         "'" & mUser.CW_ID & "','" & lApplication.SmeText.Replace(",", ";").Replace("'", "''") & "'," & "'" & mUser.CW_ID & "','" & Now.Year & "_" & Now.Month & "_" & Now.Day & "_" & Now.Hour & "_" & Now.Minute & "_" & Now.Second & "_" & Now.Millisecond & "'," & _
                '         "'submitted','GBOX_REQUEST')"
                'lPackage.Add(nstrSql)
                ' Reference End     : ZHHR 1054647
            '---------------------------------------------------------------------------------------
            'Save AuthSets
            lPackage.AddRange(lApplication.ImplementationPack)
            '---------------------------------------------------------------------------------------
            'Save Usermail
            If InStr(mUser.Databasemanager.cnnString, "MDRS_Q") = 0 And InStr(mUser.Databasemanager.cnnString, "MDRS_Q") = 0 Then

                nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                            "([M_MAILKEY]" & _
                            ",[M_RECIPIENTS]" & _
                            ",[M_SUBJECT]" & _
                            ",[M_BODY]" & _
                            ",[M_CURRENT_SENDER])" & _
                            " VALUES('" & lGboxId & "','" & mUser.SMTP_EMAIL & "','GBOX_REQUEST:" & lGboxId & "'," & _
                            "N'" & lApplication.SmeText.Replace(",", ";").Replace("'", "''") & "','G_BOX')"
            Else
                nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                                            "([M_MAILKEY]" & _
                                            ",[M_RECIPIENTS]" & _
                                            ",[M_SUBJECT]" & _
                                            ",[M_BODY]" & _
                                            ",[M_CURRENT_SENDER])" & _
                                            " VALUES('" & lGboxId & "','" & mUser.SMTP_EMAIL & "','TESTREQUEST IGNORE IT:GBOX_REQUEST:" & lGboxId & "'," & _
                                            "N'" & lApplication.SmeText.Replace(",", ";").Replace("'", "''") & "','G_BOX')"

            End If

            lPackage.Add(nstrSql)

            '---------------------------------------------------------------------------------------
            'Save Hotlinemail
            If InStr(mUser.Databasemanager.cnnString, "MDRS_Q") = 0 And InStr(mUser.Databasemanager.cnnString, "MDRS_Q") = 0 Then
                nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                                   "([M_MAILKEY]" & _
                                   ",[M_RECIPIENTS]" & _
                                   ",[M_SUBJECT]" & _
                                   ",[M_BODY]" & _
                                   ",[M_CURRENT_SENDER])" & _
                                   " VALUES('" & lGboxId & "','" & mUser.GBOXmanager.GetRecipientByApplicationPart(lApplicationpart) & "','GBOX_REQUEST:" & lGboxId & "'," & _
                                    "N'" & lApplication.Implementationtext & vbCrLf & lApplication.Implementationblock & "','" & mUser.CW_ID & "')"
            Else
                nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                                   "([M_MAILKEY]" & _
                                   ",[M_RECIPIENTS]" & _
                                   ",[M_SUBJECT]" & _
                                   ",[M_BODY]" & _
                                   ",[M_CURRENT_SENDER])" & _
                                   " VALUES('" & lGboxId & "','" & mUser.GBOXmanager.GetRecipientByApplicationPart(lApplicationpart) & "','TESTREQUEST, PLEASE IGNORE:    GBOX_REQUEST:" & lGboxId & "'," & _
                                    "N'" & lApplication.Implementationtext & vbCrLf & lApplication.Implementationblock & "','" & mUser.CW_ID & "')"

            End If
            lPackage.Add(nstrSql)

            'save RollBackPacks
            For Each lSql As String In lApplication.RollbackPack
                nstrSql = "INSERT INTO LOG_TABLE_ROLLBACK_SQL " & _
                          "([Request_ID]" & _
                            ",[Statement_SQL])" & _
                            " VALUES ( '" & lGboxId & "', '" & lSql.Replace("'", "''") & "')"
                lPackage.Add(nstrSql)
            Next lSql
        Next lApplication
        End If
        If (lPackage.Count > 0) Then
            If mUser.Databasemanager.ExecutePackage(lPackage) Then
                RaiseEvent ErrorInfo("Your request " & lGboxId & " has been submitted.")
                mStatus = "Your request " & lGboxId & " has been submitted."
            Else
                RaiseEvent ErrorInfo("Your request is not submitted.")
                mStatus = "Your request is not submitted."
            End If
        End If
        pObjRequestedUsers = Nothing
        Try
            pObjCurrentUsers.RemoveUser(mUser.CW_ID)
        Catch

        End Try

        Return mStatus
    End Function
    Public Function CreateDeleteUserRequest(ByRef ltrvRole As TreeView, ByRef grdData As GridView) As Boolean
        mUser.RequestText = ""
        Dim lApplication As String = ltrvRole.SelectedNode.ValuePath.Split("/")(0).Trim
        Dim lApplicationpart As String = ltrvRole.SelectedNode.ValuePath.Split("/")(1).Trim
        Dim myPath As Array = ltrvRole.SelectedNode.ValuePath.Split("/")

        Dim lApplicationRole As String = myPath(myPath.GetUpperBound(0)).ToString.Split("(")(0).Trim.ToString
        lApplication = lApplication.Split("(")(0).Trim
        lApplicationpart = lApplicationpart.Split("(")(0).Trim
        lApplicationRole = lApplicationRole.Split("(")(0).Trim

        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable("Select DELETION_LEVEL From Application where Application_ID ='" & lApplication & "'")
        Dim lDELETION_LEVEL As String = ldt.Rows(0)("DELETION_LEVEL").ToString
        Dim ldelSQL As String = "Select Deletiontext From APPLICATION_ROLE Where APPLICATION_ID='" & lApplication & "' and APPLICATION_Part_ID='" & lApplicationpart & "' AND APPLICATION_ROLE_ID='" & lApplicationRole & "'"
        Dim ldelDT As DataTable = mUser.Databasemanager.MakeDataTable(ldelSQL)
        Dim lDeletionText As String = ldelDT.Rows(0)("Deletiontext").ToString

        Dim lUserHaeder As String

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1048850 - GBOX Filter settings OTT 1048: New Workflow for Filter settings
        ' Comment           : Added below string
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 20-Nov-2015
        Dim mOrgLevelId As String = ""
        Dim mOrgLevelValue As String = ""
        Dim mText As String = ""
        ' Reference End     : ZHHR 1048850

        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELD FROM DB AND CODE
        ' Comment           : Remove title from database and code
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2014-11-26
        lUserHaeder = "Dear " & mUser.first_name & " " & mUser.last_name & "," & vbCrLf _
            & vbCrLf & "you have requested deletion of the following authorization set(s):" _
            & vbCrLf & "Application; Application part; Application role for Section"
        ' Reference  END    : CR ZHHR 1035817
        If lDELETION_LEVEL <> "APPLICATION_ROLE" Then
            mUser.RequestText = mUser.RequestText & vbCrLf & "NOTE: additional authorization sets for this application will be deleted (deletion level:" & lDELETION_LEVEL & ")." & vbCrLf _
            & "This is the actual set to be deleted:" _
            & vbCrLf & vbCrLf & "Application; Application part; Application role for Section"
        End If

        For Each r As GridViewRow In grdData.Rows
            If mUser.Databasemanager.GetCheckedValuebyId(mRequestform, r.Cells(0).Controls(0).UniqueID) Then
                'DataBase SQL String
                Dim lAuthset As New AuthorizationSet
                With lAuthset
                    .Application = lApplication
                    .Applicationpart = lApplicationpart
                    .Applicationrole = lApplicationRole
                    .Subgroup = r.Cells(1).Text.Trim

                    '---------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1036809 - GBOX AUTH MGMT: Delete an & entry in AUTHORISATION_SET
                    ' Comment           : Replace "&amp;" with "&" in Orglevel_Value string value
                    ' Added by          : Pratyusa Lenka (CWID : EOJCG)
                    ' Date              : 2014-12-18
                    .Orglevel_Value = r.Cells(3).Text.Trim.Replace("&amp;", "&")
                    ' Reference  END    : CR ZHHR 1036809
                    .CW_ID = r.Cells(4).Text.Trim
                    lUserHaeder = lUserHaeder & vbCrLf & vbCrLf & .Application & ";" & .Applicationpart & ";" & .Applicationrole & " for " & .Orglevel_Value & ";" & vbCrLf & " for user " & .CW_ID

                    mOrgLevelId = r.Cells(2).Text.Trim
                    mOrgLevelValue = .Orglevel_Value
                    mText = lUserHaeder
                End With
                mUser.Package.Add(GetDeleteStatement(lAuthset))
                mUser.RollbackPack.AddRange(GetRollbackStatement(lAuthset))

                If lDELETION_LEVEL <> "APPLICATION_ROLE" Then
                    Dim lmyAdditionaldt As DataTable = mUser.Databasemanager.MakeDataTable("Select * from AUTHORISATION_SET where CW_ID='" & lAuthset.CW_ID & "' " & GetWhereByDeletionLevel(lDELETION_LEVEL, lApplicationpart, lApplicationRole))
                    For Each mr In lmyAdditionaldt.Rows
                        mUser.RequestText = mUser.RequestText & vbCrLf & mr("APPLICATION_ID").ToString & ";" & mr("APPLICATION_Part_ID").ToString & ";" & mr("APPLICATION_ROLE_ID").ToString & ";" & mr("Subgroup_Id").ToString & ";;" & ";" & mr("CW_ID").ToString.Trim & ";"
                    Next
                End If
            End If
        Next

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1048850 - GBOX Filter settings OTT 1048: New Workflow for Filter settings
        ' Comment           : New Workflow for Filter settings
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 10-Nov-2015
        If mOrgLevelId.Trim <> "" Then
            Dim lGboxId As String = mUser.GBOXmanager.GetGBOXId
            lGboxId = Replace(lGboxId, "_DRS", "") & "_AUTH"
            mUser.Current_Request_ID = lGboxId
            mUser.Package.AddRange(wf_MakePackage("AUTH_" & lApplication, mOrgLevelId, mOrgLevelValue, lGboxId, mText))
        End If
        ' Reference         : ZHHR 1048850


        If lDELETION_LEVEL <> "APPLICATION_ROLE" Then
            mUser.RequestText = lUserHaeder & vbCrLf & mUser.RequestText
        Else
            mUser.RequestText = lUserHaeder
        End If
        mUser.RequestText = mUser.RequestText & vbCrLf & "====" & lDeletionText
        Return True
    End Function
    Public Function GetDeleteStatement(ByVal lAuthset As AuthorizationSet) As String
        Dim lSQL As String = ""
        With lAuthset
            Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable("Select DELETION_LEVEL From Application where Application_ID ='" & .Application & "'")
            Dim lDELETION_LEVEL As String = ldt.Rows(0)("DELETION_LEVEL").ToString
            lSQL = "DELETE From AUTHORISATION_SET where APPLICATION_ID='" & .Application & "'"
            lSQL = lSQL & GetWhereByDeletionLevel(lDELETION_LEVEL, .Applicationpart, .Applicationrole)
            lSQL = lSQL & " AND ORG_LEVEL_VALUE ='" & .Orglevel_Value & "'"
            lSQL = lSQL & " AND CW_ID ='" & .CW_ID & "'"
        End With
        lSQL.Replace("'", "''")
        Return lSQL
    End Function
    Public Function GetRollbackStatement(ByVal lAuthset As AuthorizationSet) As List(Of String)
        Dim lRollbackPack As New List(Of String)
        With lAuthset
            Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable("Select DELETION_LEVEL From Application where Application_ID ='" & .Application & "'")
            Dim lDELETION_LEVEL As String = ldt.Rows(0)("DELETION_LEVEL").ToString
            Dim lCurrentAuthsetSQL As String
            lCurrentAuthsetSQL = "select * from AUTHORISATION_SET where APPLICATION_ID='" & .Application & "'"
            lCurrentAuthsetSQL = lCurrentAuthsetSQL & GetWhereByDeletionLevel(lDELETION_LEVEL, .Applicationpart, .Applicationrole)
            lCurrentAuthsetSQL = lCurrentAuthsetSQL & " AND Subgroup_ID ='" & .Subgroup & "'"
            lCurrentAuthsetSQL = lCurrentAuthsetSQL & " AND CW_ID ='" & .CW_ID & "'"
            Dim lUsersAuthSets As DataTable = mUser.Databasemanager.MakeDataTable(lCurrentAuthsetSQL)
            For Each myAuthSet As DataRow In lUsersAuthSets.Rows
                Dim lSqlRollback As String = "INSERT INTO AUTHORISATION_SET "
                lSqlRollback = lSqlRollback & " (APPLICATION_ID "
                lSqlRollback = lSqlRollback & " ,APPLICATION_PART_ID "
                lSqlRollback = lSqlRollback & " ,APPLICATION_ROLE_ID "
                lSqlRollback = lSqlRollback & " ,SUBGROUP_ID "
                lSqlRollback = lSqlRollback & " ,CW_ID "
                lSqlRollback = lSqlRollback & " ,AUTH_STATE_ID) "
                lSqlRollback = lSqlRollback & " VALUES "
                lSqlRollback = lSqlRollback & " ('" & myAuthSet("APPLICATION_ID").ToString.Trim & "' "
                lSqlRollback = lSqlRollback & " ,'" & myAuthSet("APPLICATION_PART_ID").ToString.Trim & "' "
                lSqlRollback = lSqlRollback & " ,'" & myAuthSet("APPLICATION_ROLE_ID").ToString.Trim & "' "
                lSqlRollback = lSqlRollback & " ,'" & myAuthSet("SUBGROUP_ID").ToString.Trim & "' "
                lSqlRollback = lSqlRollback & " ,'" & myAuthSet("CW_ID").ToString.Trim & "' "
                lSqlRollback = lSqlRollback & " ,'ACTIVE')"
                lRollbackPack.Add(lSqlRollback)
            Next myAuthSet
        End With
        Return lRollbackPack
    End Function
    Public Sub RemoveUser(ByRef lstUsersToCheck As ListBox, ByVal imgShowRequest As ImageButton)
        lstUsersToCheck.Items.Remove(lstUsersToCheck.SelectedItem)
        If lstUsersToCheck.Items.Count = 0 Then
            imgShowRequest.Enabled = False
        Else
            imgShowRequest.Enabled = True
        End If

    End Sub
    Public Function Adduser(ByRef ltxtUserToAdd As TextBox, ByRef lstUsersToCheck As ListBox, ByRef lblError As Label) As Boolean
        '--------------------------------------------------------------------------------------------------------- 
        ' Reference     : CR ZHHR 1035650 - GBOX AUTH MGMT: No error message when requests a role for his own CWID 
        ' Comment       : It was not showing error message when user requests a role for his own CWID
        '                 Empty string is not allowed, show the error message
        '                 Trim the CWID to check for own CWID
        '                 Validate the characters with trim string
        '                 Check the duplicate entry or blank value to the listbox, if not available then add the user request
        ' Added by      : Pratyusa Lenka (CWID : EOJCG) 
        ' Date          : 2014-11-24 
        lblError.Text = ""
        If ltxtUserToAdd.Text.Trim = "" Then
            lblError.Text = "Blank is not allowed, please enter only CWID."
            Return False
        End If

        Dim lStringValidator = New StringValidator
        ltxtUserToAdd.Text = ltxtUserToAdd.Text.Replace(",", ";").ToUpper
        Dim lArrUser As Array = ltxtUserToAdd.Text.Split(";")
        Dim lError As Boolean = False

        If ltxtUserToAdd.Text.ToUpper.Contains(mUser.CW_ID.ToUpper) Then
            lblError.Text = "It is not possible to request for yourself. Please contact another requester or GBOX support via MDAS hotline."
        Else
            lblError.Text = ""
        End If

        For i As Integer = 0 To lArrUser.GetUpperBound(0)
            If lArrUser(i).ToString.Trim <> mUser.CW_ID.ToUpper Then
                If lStringValidator.VaidateChars(lArrUser(i).ToString.Trim) Then
                    Dim lstItem As ListItem = lstUsersToCheck.Items.FindByValue(lArrUser(i).ToString.Trim)
                    If lstItem Is Nothing And Not lArrUser(i).ToString = "" Then
                        lstUsersToCheck.Items.Add(lArrUser(i).ToString.Trim)
                        lError = False
                    End If
                Else
                    lblError.Text = lStringValidator.NotAllowedKeys & " is not allowed !"
                    lError = True
                End If
            End If

        Next
        ' Reference END : CR ZHHR 1035650 
        ' Added by      : Pratyusa Lenka (CWID : EOJCG)
        '---------------------------------------------------------------------------------------------------------
        If Not lError Then ltxtUserToAdd.Text = ""
        Return Not lError

    End Function



    Public Function GetWhereByDeletionLevel(ByVal lDELETION_LEVEL As String, ByVal lApplicationpart As String, ByVal lApplicationRole As String) As String
        Dim lSQL As String = ""
        Select Case lDELETION_LEVEL
            Case "APPLICATION"
            Case "APPLICATION_PART"
                lSQL = lSQL & " AND APPLICATION_PART_ID='" & lApplicationpart & "'"
            Case "APPLICATION_ROLE"
                lSQL = lSQL & " AND APPLICATION_PART_ID='" & lApplicationpart & "'"
                lSQL = lSQL & " AND  APPLICATION_ROLE_ID='" & lApplicationRole & "'"
        End Select
        Return lSQL
    End Function

    '---------------------------------------------------------------------------------------------------
    ' Reference         : ZHHR 1048850 - GBOX Filter settings OTT 1048: New Workflow for Filter settings
    ' Comment           : New function added for Filter settings workflow
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 12-Oct-2015
    Private Function wf_MakePackage(ByVal Obj_ID As String, ByVal orgLevelID As String, ByVal orgLevelValue As String, ByVal mId As String, ByVal mText As String) As List(Of String)
        Dim lPack As New List(Of String)
        Dim lSql As String = ""
        Dim MartyId As String = mId
        Dim strOrgLevelID As String = orgLevelID
        Dim strOrgLevelValue As String = orgLevelValue
        Dim lWORKFLOW_ID As String = ""
        Dim lSTATION_ID As String = ""
        Dim lOBJ_Description As String = ""

        Dim lwf_DEFINE_WORKFLOW_DETAILS As String = "Select * from wf_DEFINE_WORKFLOW_DETAILS Where Rank = '1' And OBJ_ID='" & Obj_ID & "'"
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lwf_DEFINE_WORKFLOW_DETAILS)

        If (dt.Rows.Count > 0) Then
            lWORKFLOW_ID = dt.Rows(0)("WORKFLOW_ID").ToString
            lSTATION_ID = dt.Rows(0)("STATION_ID").ToString
        End If
        Dim lITEM_STATUS As String = "QUEUED"
        Dim lREQUEST_STATUS_ID As String = "REQUESTED"
        Dim lCW_ID_CURRENT_RESPONSIBLE As String = mUser.CW_ID
        Dim lCustomersDATASQL As String = "Select * from MDRS_USER where cw_ID ='" & mUser.CW_ID & "'"
        Dim ldtRequesterData As DataTable = mUser.Databasemanager.MakeDataTable(lCustomersDATASQL)

        ' ---------------------------------------------------------------------------------------
        Dim lOBJ_SQL As String = "SELECT OBJ_DESCRIPTION FROM OBJ where obj_ID ='" & Obj_ID & "'"
        Dim lDesDt As DataTable = mUser.Databasemanager.MakeDataTable(lOBJ_SQL)
        If lDesDt.Rows.Count > 0 Then
            lOBJ_Description = lDesDt.Rows(0)("OBJ_DESCRIPTION").ToString
        End If
        Dim lWorkflowinfo As String = lWORKFLOW_ID


        Dim lsystemId As String = ""

        Select Case Obj_ID
            Case "AUTH_BARDO"
                lsystemId = "BARDO"
            Case "AUTH_BCC"
                lsystemId = "BCC"
            Case "AUTH_GBOX"
                lsystemId = "GBOX"
            Case "AUTH_MARTY"
                lsystemId = "MARTY"
        End Select

        If Obj_ID <> "AUTH_MARTY" And Obj_ID <> "AUTH_GBOX" Then

            ' Implementation of pMDAS DATA
            lSql = "INSERT INTO [pMDAS_System_Implementation_Target_Info]"
            lSql &= "([REQUEST_ID]"
            lSql &= ",[pMDAS_System_System_ID]"
            lSql &= ",[Target_Date]"
            lSql &= ",[Inform_by_Mail]"
            lSql &= ",[THIRD_PARTY_SYSTEM_KEY]"
            lSql &= ",[THIRD_PARTY_SYSTEM_KEY_VALUE]"
            lSql &= ",[Start_Timestamp]"
            lSql &= ",[End_Timestamp])"
            lSql &= " VALUES"
            lSql &= " ('" & MartyId & "'"
            lSql &= " ,'" & lsystemId & "'"
            lSql &= " ,NULL" '<Target_Date, nvarchar(50),>"
            lSql &= " ,NULL" '<Inform_by_Mail, nvarchar(50),>"
            lSql &= " ,'UNKNOWN'" '<THIRD_PARTY_SYSTEM_KEY, nvarchar(50),>"
            lSql &= " ,'not filled'"
            lSql &= " ,Getdate()"
            lSql &= " ,NULL)"
            lPack.Add(lSql)

        End If

        For Each rw As DataRow In dt.Rows

            If (strOrgLevelValue.Trim <> "") Then
                If (rw("ORG_LEVEL_ID") = "SUBGROUP") And rw("STATION_ID") = "SME" And rw("ORG_LEVEL_VALUE") <> orgLevelValue Then

                    lSql = "INSERT INTO wf_REQUEST_ACTIVE_ITEM"
                    lSql &= "   (REQUEST_ID"
                    lSql &= "   ,ORG_LEVEL_ID"
                    lSql &= "   ,ORG_LEVEL_VALUE"
                    lSql &= "   ,RANK"
                    lSql &= "   ,WORKFLOW_ID"
                    lSql &= "   ,STATION_ID"
                    lSql &= "   ,ITEM_STATUS"
                    lSql &= "   ,ROLE_CLUSTER_ID"
                    lSql &= "   ,REQUEST_STATUS_ID"
                    lSql &= "   ,Timestamp"
                    lSql &= "   ,CW_ID_CURRENT_RESPONSIBLE"
                    lSql &= "   ,REQUESTER)"
                    lSql &= "VALUES "
                    lSql &= "   ('" & MartyId & "'"
                    lSql &= "   ,'" & rw("ORG_LEVEL_ID").ToString & "'"
                    lSql &= "   ,'" & rw("ORG_LEVEL_VALUE").ToString & "'"
                    lSql &= "   ,'" & rw("RANK").ToString & "'"
                    lSql &= "   ,'" & rw("WORKFLOW_ID").ToString & "'"
                    lSql &= "   ,'" & rw("STATION_ID").ToString & "'"
                    lSql &= "   ,'" & lITEM_STATUS & "'"
                    lSql &= "   ,'" & rw("ROLE_CLUSTER_ID").ToString & "'"
                    lSql &= "   ,'" & lREQUEST_STATUS_ID & "'"
                    lSql &= "   ,GetDate()"
                    lSql &= "   ,'" & lCW_ID_CURRENT_RESPONSIBLE & "'"
                    lSql &= "   ,'" & mUser.CW_ID & "')"

                    lPack.Add(lSql)

                    lPack.AddRange(SendEmailToApproverGroup(Obj_ID, rw("ORG_LEVEL_VALUE"), MartyId, rw("STATION_ID")))

                End If
            End If
        Next

        For Each r As DataRow In dt.Rows

            If (r("STATION_ID") = "SME" And (r("ORG_LEVEL_ID") <> orgLevelID Or r("ORG_LEVEL_VALUE") <> orgLevelValue)) Then

            Else
                lSql = "INSERT INTO wf_REQUEST_ACTIVE_ITEM"
                lSql &= "   (REQUEST_ID"
                lSql &= "   ,ORG_LEVEL_ID"
                lSql &= "   ,ORG_LEVEL_VALUE"
                lSql &= "   ,RANK"
                lSql &= "   ,WORKFLOW_ID"
                lSql &= "   ,STATION_ID"
                lSql &= "   ,ITEM_STATUS"
                lSql &= "   ,ROLE_CLUSTER_ID"
                lSql &= "   ,REQUEST_STATUS_ID"
                lSql &= "   ,Timestamp"
                lSql &= "   ,CW_ID_CURRENT_RESPONSIBLE"
                lSql &= "   ,REQUESTER)"
                lSql &= "VALUES "
                lSql &= "   ('" & MartyId & "'"
                lSql &= "   ,'" & r("ORG_LEVEL_ID").ToString & "'"
                lSql &= "   ,'" & r("ORG_LEVEL_VALUE").ToString & "'"
                lSql &= "   ,'" & r("RANK").ToString & "'"
                lSql &= "   ,'" & r("WORKFLOW_ID").ToString & "'"
                lSql &= "   ,'" & r("STATION_ID").ToString & "'"
                lSql &= "   ,'" & lITEM_STATUS & "'"
                lSql &= "   ,'" & r("ROLE_CLUSTER_ID").ToString & "'"
                lSql &= "   ,'" & lREQUEST_STATUS_ID & "'"
                lSql &= "   ,GetDate()"
                lSql &= "   ,'" & lCW_ID_CURRENT_RESPONSIBLE & "'"
                lSql &= "   ,'" & mUser.CW_ID & "')"

                lPack.Add(lSql)

                lPack.AddRange(SendEmailToApproverGroup(Obj_ID, r("ORG_LEVEL_VALUE"), MartyId, r("STATION_ID")))

            End If
        Next r

        If lSql = "" Then
            lSql = "Select * from wf_DEFINE_WORKFLOW_DETAILS Where Rank = '2' And OBJ_ID='" & Obj_ID & "'"
            dt = mUser.Databasemanager.MakeDataTable(lSql)
            For Each r As DataRow In dt.Rows
                lSql = "INSERT INTO wf_REQUEST_ACTIVE_ITEM"
                lSql &= "   (REQUEST_ID"
                lSql &= "   ,ORG_LEVEL_ID"
                lSql &= "   ,ORG_LEVEL_VALUE"
                lSql &= "   ,RANK"
                lSql &= "   ,WORKFLOW_ID"
                lSql &= "   ,STATION_ID"
                lSql &= "   ,ITEM_STATUS"
                lSql &= "   ,ROLE_CLUSTER_ID"
                lSql &= "   ,REQUEST_STATUS_ID"
                lSql &= "   ,Timestamp"
                lSql &= "   ,CW_ID_CURRENT_RESPONSIBLE"
                lSql &= "   ,REQUESTER)"
                lSql &= "VALUES "
                lSql &= "   ('" & MartyId & "'"
                lSql &= "   ,'" & r("ORG_LEVEL_ID").ToString & "'"
                lSql &= "   ,'" & r("ORG_LEVEL_VALUE").ToString & "'"
                lSql &= "   ,'" & r("RANK").ToString & "'"
                lSql &= "   ,'" & r("WORKFLOW_ID").ToString & "'"
                lSql &= "   ,'" & r("STATION_ID").ToString & "'"
                lSql &= "   ,'" & lITEM_STATUS & "'"
                lSql &= "   ,'" & r("ROLE_CLUSTER_ID").ToString & "'"
                lSql &= "   ,'" & lREQUEST_STATUS_ID & "'"
                lSql &= "   ,GetDate()"
                lSql &= "   ,'" & lCW_ID_CURRENT_RESPONSIBLE & "'"
                lSql &= "   ,'" & mUser.CW_ID & "')"

                lPack.Add(lSql)

                lPack.AddRange(SendEmailToApproverGroup(Obj_ID, r("ORG_LEVEL_VALUE"), MartyId, r("STATION_ID")))

            Next
        End If

        lSql = "INSERT INTO wf_REQUEST_STATUS"
        lSql &= "   (REQUEST_ID"
        lSql &= "   ,CW_ID_REQUESTER"
        lSql &= "   ,WORKFLOW_ID"
        lSql &= "   ,START_TIMESTAMP"
        lSql &= "   ,STEP_TIMESTAMP"
        lSql &= "   ,END_TIMESTAMP"
        lSql &= "   ,CW_ID_CURRENT_RESPONSIBLE"
        lSql &= "   ,REQUEST_STATUS_ID"
        lSql &= "   ,REQUEST_TEXT"
        lSql &= "   ,OBJ_ID"
        lSql &= "   ,REQUEST_ORG_LEVEL"
        lSql &= "   ,REQUEST_ORG_LEVEL_VALUE"
        lSql &= "   ,REQUEST_COMMENT)"
        lSql &= "VALUES "
        lSql &= "   ('" & MartyId & "'"
        lSql &= "   ,'" & mUser.CW_ID & "'"
        lSql &= "   ,'" & lWORKFLOW_ID & "'"
        lSql &= "   ,Getdate()"
        lSql &= "   ,Getdate()"
        lSql &= "   ,Getdate()"
        lSql &= "   ,'" & lCW_ID_CURRENT_RESPONSIBLE & "'"
        lSql &= "   ,'" & lITEM_STATUS & "'"
        lSql &= "   ,N'" & mText & "'"
        lSql &= "   ,'" & Obj_ID & "'"
        lSql &= "   ,'" & orgLevelID & "'"
        lSql &= "   ,'" & orgLevelValue & "'"
        lSql &= "   ,'')"
        lPack.Add(lSql)

        Return lPack

    End Function

    Private Function SendEmailToApproverGroup(ByVal OBJ_ID As String, ByVal lOrg_Level_Value As String, ByVal MartyId As String, ByVal StationId As String) As List(Of String)
        Dim lSql As String = ""
        Dim lRequest_text As String
        Dim lPack As New List(Of String)

        'lSql = " SELECT * FROM wf_DEFINE_WORKFLOW_DETAILS WHERE OBJ_ID ='" & OBJ_ID & "' AND ORG_LEVEL_VALUE = '" & lOrg_Level_Value & "' AND EMAIL = 1 "
        lSql = " SELECT U.SMTP_EMAIL  FROM wf_DEFINE_WORKFLOW_DETAILS WDWD " & _
                  " INNER JOIN [ROLE_CLUSTER] RC ON WDWD.ROLE_CLUSTER_ID  =  RC.Role_Cluster_ID  " & _
                  " INNER JOIN AUTHORISATION_SET ASE ON RC.Application_Role_ID = ASE.Application_Role_ID  " & _
                  " AND RC.Application_ID = ASE.APPLICATION_ID  " & _
                  " INNER JOIN MDRS_USER U ON U.CW_ID = ASE.CW_ID " & _
                  " WHERE WDWD.OBJ_ID = '" & OBJ_ID & "' AND WDWD.EMAIL = 1 " & _
                  " AND WDWD.STATION_ID ='" & StationId & "' AND ASE.Application_Part_ID ='" & OBJ_ID & "'"
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
        lSql = ""
        If Not dt Is Nothing Then
            If dt.Rows.Count > 0 Then
                
                Dim lUserEmail As String = ""
                lRequest_text = "Dear Sir/Madam, " & vbCrLf & vbCrLf & "A workflow item in GBOX requires your attention. " & vbCrLf
                lRequest_text += "Please start GBOX Manager and evaluate the item(s) assigned to you." & vbCrLf
                lRequest_text += "Information/help regarding GBOX Manager can be found here: http://by-gbox.bayer-ag.com/GBOX-Manager" & vbCrLf & vbCrLf & vbCrLf
                lRequest_text += "Regards," & vbCrLf
                lRequest_text += "DRS Administration Team"

                For Each row In dt.Rows
                    If (Not IsDBNull(row("SMTP_EMAIL"))) Then
                        lUserEmail = row("SMTP_EMAIL")
                        If lUserEmail.Trim <> "" Then
                            lSql = "INSERT INTO M_MAILTRIGGER"
                            lSql &= "  (M_MAILKEY"
                            lSql &= "  ,M_RECIPIENTS"
                            lSql &= "   ,M_SUBJECT"
                            lSql &= "   ,M_BODY"
                            lSql &= "   ,M_CURRENT_SENDER)"
                            lSql &= "VALUES "
                            lSql &= "   ('" & MartyId & "',"
                            lSql &= "   '" & lUserEmail & "',"
                            lSql &= "   'New workflow item in GBOX AUTH: " & MartyId & "',"
                            lSql &= "   N'" & lRequest_text & "',"
                            lSql &= "   '" & mUser.CW_ID & "')"
                            lPack.Add(lSql)
                        End If
                    End If
                Next
            End If
        End If
                   
        Return lPack

    End Function
    ' Reference End      : ZHHR 1048850


End Class
