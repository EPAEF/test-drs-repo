Option Strict Off
Partial Public Class UserAccess
    Inherits System.Web.UI.Page
    Private mUserInfo As myUser
    Private mUsersApplication As DataTable
    Private mUsersApplicationRoles As DataTable
    Private mdt As DataTable
    Private mUserRoles As DataTable
    Private mUserRolesString As String = "''"
    Private mRequestedUsers As List(Of myRequestedUser)
    Private mClosed As Boolean = False
    Private mFooter As String = "For further information or new request visit http://by-gbox.bayer-ag.com/Request-authorization/ ." & vbCrLf
    Private mUser As myUser

    'Checkboxes in Treeviewcontrol do not postback
    ' to raise the postback the following javascrip was established in 
    ' the aspx file
    '<script language="javascript" type="text/javascript">
    'Function postBackByObject()
    '        {
    '            var o = window.event.srcElement;
    '            if (o.tagName == "INPUT" && o.type == "checkbox")
    '            {
    '               __doPostBack("","");
    '            } 
    '        }
    '</script>
    '<asp:TreeView ID="trvApplicationSet" onclick="javascript:postBackByObject()"  runat="server" ImageSet="XPFileExplorer" 
    '                        NodeIndent="15" ShowCheckBoxes="All" ShowLines="True" 
    '                        PopulateNodesFromClient="False" ShowExpandCollapse="False" >

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        pObjCurrentUsers.CONTEXT = Context

        loadMe()
    End Sub
    Private Sub loadMe()
        Try
            mUser = Nothing
            cmbAuthSetSubgroup.Visible = True
            cmbAuthSetDivision.Visible = True
            lblChooseDivision.Visible = True
            lblChooseSubgroup.Visible = True
            If Context.User.Identity.Name = "" Then
                lblLocked.Text = "There is an authentification problem please inform imscc-mdrs@bayerbbs.com "
                mvUserAccess.ActiveViewIndex = 6
                Exit Sub
            End If
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)

                '---------------------------------------------------------------------------------------------------
                ' Reference         : CR ZHHR 1035817 - GBOX WebForms: Add New Function for updating columns
                ' Comment           : Added code for updating new columns in User table
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 2014-12-15
                mUser.UserAccessStatus(mUser.CW_ID, "GBOX USERACCESS")
                ' Reference  END    : CR ZHHR 1035817
            End If

            ' Reference         : CR YHHR 2022491 - GBOX WebForms
            ' Comment           : INC_GBox: error user data updation
            ' Added by          : Sheetal Punnapully (CWID : ETMVO)
            ' Date              : 2018-02-02

            Dim user As New myGBoxManager
            user.MakeUser(mUser.CW_ID)

            If mUser.GBOXmanager.CHECK_MDRS_DATABASE_MAINTENANCE() Then
                lblLocked.Text = "G|Box System Access  is currently locked due to maintenance "
                '  lblLocked.Text = lblLocked.Text & vbCrLf & mUser.GBOXmanager.Get_MDRS_DATABASE_MAINTENANCE_STRING
                mvUserAccess.ActiveViewIndex = 6
                Exit Sub
            End If
            cmdChooseApplication.Enabled = True
            Dim lContextUser As String = ""
            If txtImpersonate.Text = "" Then
                lContextUser = Context.User.Identity.Name
            Else
                lContextUser = txtImpersonate.Text
            End If
            If lContextUser = "" Then
                lblLocked.Text = "There is an authentification problem please inform imscc-mdrs@bayerbbs.com "
                mvUserAccess.ActiveViewIndex = 6
                Exit Sub
            End If
            lblImpersonate.Visible = mUser.GBOXmanager.IsGBoxAdmin(Context.User.Identity.Name)
            txtImpersonate.Visible = mUser.GBOXmanager.IsGBoxAdmin(Context.User.Identity.Name)
            cmdImpersonate.Visible = mUser.GBOXmanager.IsGBoxAdmin(Context.User.Identity.Name)

            If Not mUser Is Nothing Then
                With mUser
                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
                    ' Comment           : Remove title from database and code
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 2014-12-15
                    lblUsername.Text = "Welcome " & mUser.first_name & " " & mUser.last_name
                    ' Reference  END    : CR ZHHR 1035817

                    tblUserInfo.Rows.Clear()
                    AddrowToUserInfo("Subgroup:", .SUBGROUP_ID, tblUserInfo)
                    AddrowToUserInfo("Email:", .SMTP_EMAIL, tblUserInfo)
                    'AddrowToUserInfo("Domain:", .WINDOWS_DOMAIN, tblUserInfo)
                    'AddrowToUserInfo("AuthenticationType:", Context.User.Identity.AuthenticationType.ToString, tblUserInfo)
                End With
                cmdChooseApplication.Enabled = True
                mUserRoles = mUser.Databasemanager.MakeDataTable("Select Application, ApplicationPart as [Application part], Applicationrole as [Application role],ApplicationroleText As [Description],STATE as [Status] from vw_Auth_Set_With_Desriptions where [CW_ID] ='" & mUser.CW_ID & "'")
                grdRoles.DataSource = mUserRoles
                grdRoles.DataBind()

                cmdShowMySmes.Visible = True


                Dim dt As DataTable = mUser.Databasemanager.MakeDataTable("Select Application, ApplicationPart, Applicationrole, ApplicationRoleText,STATE from vw_Auth_Set_With_Desriptions where [CW_ID] ='" & mUser.CW_ID & "'")
                If dt.Rows.Count = 0 Then
                    cmdRequestResponisible.Visible = True
                    cmdChooseApplication.Visible = False
                Else
                    cmdRequestResponisible.Visible = False
                    cmdChooseApplication.Visible = True
                End If


            Else
                Dim lUsername As Object = mUser.CW_ID.Split("\")
                Dim lCW_ID As String = lUsername(1).ToString
                lblUsername.Text = "Welcome " & lCW_ID
                mUserRoles = mUser.Databasemanager.MakeDataTable("Select Application, ApplicationPart, Applicationrole, ApplicationRoleText,STATE from vw_Auth_Set_With_Desriptions where [CW_ID] ='" & lCW_ID & "'")
                'User
                If mUserRoles.Rows.Count > 0 Then
                    grdRoles.DataSource = mUserRoles
                    grdRoles.DataBind()


                Else
                    'noUser

                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
                    ' Comment           : Remove title from database and code
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 2014-12-15
                    lblUsername.Text = "Welcome: " & mUser.last_name

                    ' Reference  END    : CR ZHHR 1035817

                    lblMessage.Text = ""
                    lnkMyMDS.Visible = True
                    cmdComplete.Visible = False
                    cmdShowMySmes.Visible = False
                    grdRoles.DataSource = Nothing
                    grdRoles.DataBind()
                End If
                cmdChooseApplication.Enabled = False
            End If
            mUserInfo = mUser
            If lstUsersToCheck.Items.Count = 0 Or trvApplicationSet.CheckedNodes.Count = 0 Then
                cmdShowRequest.Enabled = False
            Else
                If trvApplicationSet.CheckedNodes.Count > 0 Then
                    cmdShowRequest.Enabled = True
                End If
            End If
            If mUser Is Nothing Then
                cmdChooseApplication.Enabled = False
            Else
                If mUser.SMTP_EMAIL = "" Or InStr(mUser.SMTP_EMAIL, "Bayernotes") <> 0 Then
                    cmdChooseApplication.Enabled = False
                Else
                    cmdChooseApplication.Enabled = True
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub AddrowToUserInfo(ByVal Firstcolumn As String, ByVal Secondcolumn As String, ByVal lTable As Object)
        Dim r As New TableRow()
        Dim c As New TableCell()
        Dim d As New TableCell()

        c.Controls.Add(New LiteralControl(Firstcolumn))
        r.Cells.Add(c)
        d.Controls.Add(New LiteralControl(Secondcolumn))
        r.Cells.Add(d)
        lTable.Rows.Add(r)

    End Sub



    Protected Sub cmdChooseApplication_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdChooseApplication.Click
        Response.Write(mUser.GBOXmanager.openInNewWindow("Autorisation.aspx"))
    End Sub

    Public Sub Loadcombos()
        Dim lPossibleSubgroups As String = "''"
        Dim lSubgroups As DataTable = mUser.Databasemanager.MakeDataTable("Select Distinct Subgroup_ID from AUTHORISATION_SET where CW_ID='" & mUserInfo.CW_ID & "'")

        For Each r As DataRow In lSubgroups.Rows
            lPossibleSubgroups = lPossibleSubgroups & ",'" & r("Subgroup_ID").ToString & "'"
        Next
        Dim dt As DataTable
        If InStr(lPossibleSubgroups, "ALL") = 0 Then
            dt = mUser.databasemanager.MakeDataTable("Select Subgroup_ID from Subgroup where  Subgroup_ID in(" & lPossibleSubgroups & ")")
        Else
            dt = mUser.databasemanager.MakeDataTable("Select Subgroup_ID from Subgroup ")
        End If
        With cmbAuthSetSubgroup
            .DataSource = dt
            .DataTextField = "Subgroup_ID"
            .DataValueField = "Subgroup_ID"
            .SelectedValue = mUserInfo.SUBGROUP_ID
            .DataBind()
        End With
        LoadDivision()
    End Sub
    Private Function LoadApplications() As Boolean
        Try
            trvApplicationSet.Nodes.Clear()
            mUsersApplication = mUser.Databasemanager.MakeDataTable("Select Distinct APPLICATION from vw_Auth_Set_With_Desriptions where applicationrole in(  " & mUserRolesString & ")   and CW_ID='" & mUserInfo.CW_ID & "'")
            For Each lApplication As DataRow In mUsersApplication.Rows
                Dim myNode As New TreeNode(lApplication("Application"))

                myNode.SelectAction = TreeNodeSelectAction.SelectExpand
                Dim lTT As DataTable = mUser.databasemanager.MakeDataTable("Select Description from Application where Application_ID ='" & lApplication("Application").ToString & "'")
                For Each r As DataRow In lTT.Rows
                    myNode.ToolTip = r("Description").ToString
                Next
                trvApplicationSet.Nodes.Add(myNode)
                Debug.Print(myNode.Text)
                LoadApplicationParts(myNode)
            Next lApplication
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Private Function LoadApplicationParts(ByRef lApplicationNode As TreeNode) As Boolean
        Try
            mUsersApplication = mUser.Databasemanager.MakeDataTable("Select Distinct APPLICATIONPart from vw_Auth_Set_With_Desriptions where Applicationrole in(  " & mUserRolesString & ") and CW_ID='" & mUserInfo.CW_ID & "' And Application ='" & lApplicationNode.Text & "'")
            Dim lsql As String = ""

            For Each lApplicationpart As DataRow In mUsersApplication.Rows
                Dim lUserrolestringInAppPart As String = mUser.gboxmanager.GetAdministratableRolesInApplicationPart(lApplicationNode.Text, lApplicationpart("APPLICATIONPart").ToString, mUserInfo.CW_ID)
                If mUserInfo.SUBGROUP_ID <> "ALL" Then
                    lsql = "Select  APPLICATION_Role_ID ,APPLICATION_ROLE_RANK from APPLICATION_ROLE where ADMINISTRATION_ROLE_ID in(  " & lUserrolestringInAppPart & ") And APPLICATION_PART_ID='" & lApplicationpart("APPLICATIONPart").ToString & "' And (SUBGROUP_ID='" & mUserInfo.SUBGROUP_ID & "' or SUBGROUP_ID ='ALL') Order By APPLICATION_ROLE_RANK"
                Else
                    lsql = "Select  APPLICATION_Role_ID ,APPLICATION_ROLE_RANK from APPLICATION_ROLE where ADMINISTRATION_ROLE_ID in(  " & lUserrolestringInAppPart & ") And APPLICATION_PART_ID='" & lApplicationpart("APPLICATIONPart").ToString & "'"
                End If
                mUsersApplicationRoles = mUser.databasemanager.MakeDataTable(lsql)
                If mUsersApplicationRoles.Rows.Count <> 0 Then
                    Dim myNode As New TreeNode(lApplicationpart("APPLICATIONPart").ToString)
                    Debug.Print(myNode.Text)
                    myNode.SelectAction = TreeNodeSelectAction.SelectExpand
                    lApplicationNode.ChildNodes.Add(myNode)
                    Dim lTT As DataTable = mUser.databasemanager.MakeDataTable("Select Description from APPLICATION_PARTS where Application_PART_ID ='" & lApplicationpart("ApplicationPART").ToString & "'")
                    For Each r As DataRow In lTT.Rows
                        myNode.ToolTip = r("Description").ToString
                    Next
                    LoadApplicationRoles(myNode)
                End If

            Next lApplicationpart
            Return True
        Catch ex As ApplicationException
            Return False
        End Try

    End Function
    Private Function LoadApplicationRoles(ByRef lApplicationPartNode As TreeNode) As Boolean
        Try
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
            End If

            Dim lUserrolestringInAppPart As String = mUser.GBOXmanager.GetAdministratableRolesInApplicationPart(lApplicationPartNode.Parent.Text, lApplicationPartNode.Text, mUserInfo.CW_ID)
            Dim lSQL As String = ""
            If mUserInfo.SUBGROUP_ID <> "ALL" Then
                lSQL = "Select  APPLICATION_Role_ID ,APPLICATION_ROLE_CLASSIFICATION_ID,APPLICATION_ROLE_RANK from APPLICATION_ROLE where ADMINISTRATION_ROLE_ID in(  " & lUserrolestringInAppPart & ") And APPLICATION_PART_ID='" & lApplicationPartNode.Text & "' And (SUBGROUP_ID='" & mUserInfo.SUBGROUP_ID & "' or SUBGROUP_ID ='ALL') Order By APPLICATION_ROLE_RANK"
            Else
                lSQL = "Select  APPLICATION_Role_ID ,APPLICATION_ROLE_CLASSIFICATION_ID,APPLICATION_ROLE_RANK from APPLICATION_ROLE where ADMINISTRATION_ROLE_ID in(  " & lUserrolestringInAppPart & ") And APPLICATION_PART_ID='" & lApplicationPartNode.Text & "'"
            End If
            mUsersApplicationRoles = mUser.databasemanager.MakeDataTable(lSQL)
            For Each lApplicationrole As DataRow In mUsersApplicationRoles.Rows
                If mUsersApplicationRoles.Rows.Count <> 0 Then
                    Dim myNode As New TreeNode(lApplicationrole("APPLICATION_Role_ID").ToString)
                    Debug.Print(myNode.Text)
                    myNode.SelectAction = TreeNodeSelectAction.SelectExpand
                    If lApplicationrole("APPLICATION_ROLE_CLASSIFICATION_ID").ToString.ToUpper <> "Application".ToUpper Then
                        myNode.ImageUrl = "~/Images/marty-16x16-3.gif"
                    Else
                        myNode.ImageUrl = "~/Images/gbox-16x16-3.gif"
                    End If
                    Dim lTT As DataTable = mUser.databasemanager.MakeDataTable("Select Description from Application_Role where Application_Role_ID ='" & lApplicationrole("Application_ROLE_ID").ToString & "' And APPLICATION_PART_ID='" & lApplicationPartNode.Text & "'")
                    For Each r As DataRow In lTT.Rows
                        myNode.ToolTip = r("Description").ToString
                    Next
                    lApplicationPartNode.ChildNodes.Add(myNode)
                End If
            Next lApplicationrole
            Return True
        Catch ex As ApplicationException
            Return False
        End Try

    End Function

    Protected Sub cmdAddUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAddUser.Click

        Adduser()
    End Sub
    Private Sub Adduser()
        If txtUserToAdd.Text = "" Then Exit Sub
        Dim lStringValidator = New StringValidator
        txtUserToAdd.Text = txtUserToAdd.Text.Replace(",", ";").ToUpper
        Dim lArrUser As Array = txtUserToAdd.Text.Split(";")
        For i As Integer = 0 To lArrUser.GetUpperBound(0)
            If lArrUser(i).ToString <> mUserInfo.CW_ID.ToUpper Then
                If lStringValidator.VaidateChars(lArrUser(i).ToString) Then
                    lstUsersToCheck.Items.Add(lArrUser(i).ToString)
                Else
                    lblError.Text = lStringValidator.NotAllowedKeys & " is not allowed !"
                End If
            End If

        Next
        txtUserToAdd.Text = ""
        txtUserToAdd.Focus()
        loadMe()
    End Sub
    Private Sub CheckallChilds(ByVal lpNode As TreeNode)
        For Each lNode As TreeNode In lpNode.ChildNodes
            lNode.Checked = lpNode.Checked

            If lNode.Checked Then
                lNode.Expand()
                lNode.Parent.Expand()
            Else
                lNode.Collapse()
                lNode.Parent.Collapse()
            End If

            If lNode.ChildNodes.Count > 0 Then
                CheckallChilds(lNode)
            End If
        Next
    End Sub



    Protected Sub trvApplicationSet_TreeNodeCheckChanged(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.TreeNodeEventArgs) Handles trvApplicationSet.TreeNodeCheckChanged
        CheckallChilds(e.Node)
    End Sub


    Private Sub txtUserToAdd_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtUserToAdd.TextChanged
        Adduser()
    End Sub


    Protected Sub cmdRomoveSelected_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRomoveSelected.Click
        lstUsersToCheck.Items.Remove(lstUsersToCheck.SelectedItem)
        If lstUsersToCheck.Items.Count = 0 Then
            cmdShowRequest.Enabled = False
        Else
            cmdShowRequest.Enabled = True
        End If

    End Sub

    Protected Sub cmdShowRequest_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdShowRequest.Click
        fillobjects()
    End Sub
    Private Sub fillobjects()
        mRequestedUsers = Nothing
        If mRequestedUsers Is Nothing Then mRequestedUsers = New List(Of myRequestedUser)
        For Each lItem As ListItem In lstUsersToCheck.Items
            Dim lRequestedUser As New myRequestedUser
            lRequestedUser.CW_ID = lItem.Text
            mRequestedUsers.Add(lRequestedUser)
            For Each lNode As TreeNode In trvApplicationSet.Nodes
                makeAuthSetByNode(lNode, lItem.Text)
            Next lNode
        Next lItem
        mvUserAccess.ActiveViewIndex = 2

        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
        ' Comment           : Remove title from database and code
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2014-11-26
        Dim lReportAll As String = "Dear " & mUserInfo.first_name & " " & mUserInfo.last_name & "," & vbCrLf & " this is your request:" & vbCrLf
        ' Reference  END    : CR ZHHR
        lReportAll = lReportAll & "User " & mUserInfo.CW_ID & " has requested access for Subgroup: " & cmbAuthSetSubgroup.Text & " Division:" & cmbAuthSetDivision.Text & " user(s): " & vbCrLf
        lReportAll = lReportAll & mUser.gboxmanager.MakeUserList(mRequestedUsers) & vbCrLf
        lReportAll = lReportAll & "for " & vbCrLf
        mUser.gboxmanager.HeadText = lReportAll
        lReportAll = lReportAll & mUser.gboxmanager.MakeAccessText(mRequestedUsers(0))

        txtSME.Text = lReportAll


    End Sub

    Sub makeAuthSetByNode(ByVal lNode As TreeNode, ByVal lUser As String)
        If lNode.ChildNodes.Count = 0 Then
            If lNode.Checked Then
                Dim lRequesteduser As myRequestedUser = GetUserById(lUser)
                Dim lAuth As New AuthorizationSet
                With lAuth
                    .Application = lNode.Parent.Parent.Text
                    .Applicationtext = lNode.Parent.Parent.ToolTip
                    .Applicationpart = lNode.Parent.Text
                    .ApplicationPartText = lNode.Parent.ToolTip
                    .Applicationrole = lNode.Text
                    .ApplicationroleText = lNode.ToolTip
                    .CW_ID = lUser
                    .State = RequestedOrIncomplete(lNode.Parent.Parent.Text, lUser)

                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELDS IN Designer, DB AND CODE
                    ' Comment           : Removed code for VALID_FROM and VALID_TO fields in table [AUTHORISATION_SET]
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 2014-12-12

                    ' Reference  END    : CR ZHHR 1035817

                    If Not mUser.gboxmanager.AuthSetExists(lAuth) Then
                        .Implementationtext = GetImplementationText(.Application, .Applicationpart, .Applicationrole).Replace("USERNAME", lUser) '.Replace("|", "'").Replace("~", vbCrLf)
                    Else
                        .Implementationtext = .CW_ID & " is already " & .Applicationrole & " in " & .Applicationpart & ". No action required."
                    End If

                    If cmbAuthSetSubgroup.Text <> "" Then .Subgroup = cmbAuthSetSubgroup.Text

                End With
                lRequesteduser.Authorizationsets.Add(lAuth)
            End If
        Else
            For Each lChild As TreeNode In lNode.ChildNodes
                makeAuthSetByNode(lChild, lUser)
            Next
        End If

    End Sub
    Private Function GetUserById(ByVal lUser As String) As myRequestedUser
        For Each lItem As myRequestedUser In mRequestedUsers
            If lUser = lItem.CW_ID Then
                Return lItem
            End If
        Next
        Return Nothing
    End Function
    Private Function GetImplementationText(ByVal lApp As String, ByVal lApplicationpart As String, ByVal lApplicationRole As String) As String
        Dim mdt As DataTable = mUser.databasemanager.MakeDataTable("Select Implementationtext from APPLICATION_ROLE where APPLICATION_ID='" & lApp & "' and APPLICATION_PART_ID='" & lApplicationpart & "' And APPLICATION_ROLE_ID ='" & lApplicationRole & "'")
        Return mdt.Rows(0)("Implementationtext").ToString
    End Function
    Private Function RequestedOrIncomplete(ByVal lApplication_ID As String, ByVal lCW_ID As String) As String
        Dim lDt As DataTable = mUser.databasemanager.MakeDataTable("Select Needs_Qualified_User from APPLICATION where Application_ID='" & lApplication_ID & "'")
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

    '---------------------------------------------------------------------------------------------------
    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE FIELDS IN Designer, DB AND CODE
    ' Comment           : Removed code for Calender
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 2014-12-12

    ' Reference  END    : CR ZHHR 1035817

    Protected Sub cmdSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSubmit.Click
        fillobjects()
        ''Nach Application
        Dim lPackage As New List(Of String)
        Dim lGboxId As String = mUser.gboxmanager.GetGBOXId
        mUser.gboxmanager.MakeAuthSetPackageForApps(mRequestedUsers)
        mUser.gboxmanager.MakeRoleBackPackageForApps(mRequestedUsers)
        mUser.gboxmanager.MakeImplementationBlock(mRequestedUsers)
        For Each lApplication As myApplication In mUser.gboxmanager.RequestedApplications
            'SAVE Workflowdata
            Dim nstrSql As String = ""
            ' Reference         : ZHHR 1054647 - GBOX Webforms: GBOX disable LOG_TABLE_USER (old workflow)
            ' Comment           : comments below code for LOG_TABLE_USER
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 14-Mar-2016
            'Dim nstrSql As String = " INSERT INTO [LOG_TABLE_USER] " & _
            '         "([MARTY_ID],[CW_ID],[SMETEXT],[Customer],[Customer_Date],[Implementation_State], [Request_Type_Id]" & _
            '         " )VALUES ('" & lGboxId & "_" & lApplication.Applicationname & "'," & _
            '         "'" & mUserInfo.CW_ID & "','" & lApplication.SmeText.Replace(",", ";").Replace("'", "''") & "'," & "'" & mUserInfo.CW_ID & "','" & Now.Year & "_" & Now.Month & "_" & Now.Day & "_" & Now.Hour & "_" & Now.Minute & "_" & Now.Second & "_" & Now.Millisecond & "'," & _
            '         "'submitted','GBOX_REQUEST')"
            nstrSql = " INSERT INTO [wf_REQUEST_STATUS] " & _
                     "([REQUEST_ID],[CW_ID_REQUESTER],[REQUEST_TEXT],[CW_ID_CURRENT_RESPONSIBLE],[START_TIMESTAMP],[REQUEST_STATUS_ID], [Request_Type_Id]" & _
                     " )VALUES ('" & lGboxId & "_" & lApplication.Applicationname & "'," & _
                     "'" & mUserInfo.CW_ID & "','" & lApplication.SmeText.Replace(",", ";").Replace("'", "''") & "'," & "'" & mUserInfo.CW_ID & "','" & Now.Year & "_" & Now.Month & "_" & Now.Day & "_" & Now.Hour & "_" & Now.Minute & "_" & Now.Second & "_" & Now.Millisecond & "'," & _
                     "'REQUESTED','GBOX_REQUEST')"
            lPackage.Add(nstrSql)

            ' Reference End     : ZHHR 1054647
            '---------------------------------------------------------------------------------------
            'Save AuthSets
            lPackage.AddRange(lApplication.ImplementationPack)
            '---------------------------------------------------------------------------------------
            'Save Usermail
            nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                        "([M_MAILKEY]" & _
                        ",[M_RECIPIENTS]" & _
                        ",[M_SUBJECT]" & _
                        ",[M_BODY]" & _
                        ",[M_CURRENT_SENDER])" & _
                        " VALUES('" & lGboxId & "_" & lApplication.Applicationname & "','" & mUserInfo.SMTP_EMAIL & "','GBOX_REQUEST:" & lGboxId & "_" & lApplication.Applicationname & "'," & _
                        "N'" & lApplication.SmeText.Replace(",", ";").Replace("'", "''") & "','G_BOX')"
            lPackage.Add(nstrSql)
            '---------------------------------------------------------------------------------------
            'Save Hotlinemail
            If txtImpersonate.Text = "" Then
                nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                                   "([M_MAILKEY]" & _
                                   ",[M_RECIPIENTS]" & _
                                   ",[M_SUBJECT]" & _
                                   ",[M_BODY]" & _
                                   ",[M_CURRENT_SENDER])" & _
                                   " VALUES('" & lGboxId & "_" & lApplication.Applicationname & "','" & mUser.GBOXmanager.GetRecipientByRequestType("GBOX_REQUEST") & "','GBOX_REQUEST:" & lGboxId & "_" & lApplication.Applicationname & "'," & _
                                    "N'" & lApplication.Implementationtext & vbCrLf & lApplication.Implementationblock & "','" & mUserInfo.CW_ID & "')"
            Else
                nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                                   "([M_MAILKEY]" & _
                                   ",[M_RECIPIENTS]" & _
                                   ",[M_SUBJECT]" & _
                                   ",[M_BODY]" & _
                                   ",[M_CURRENT_SENDER])" & _
                                   " VALUES('" & lGboxId & "_" & lApplication.Applicationname & "','" & mUser.GBOXmanager.GetRecipientByRequestType("GBOX_REQUEST") & "','TESTREQUEST IGNORE IT:    GBOX_REQUEST:" & lGboxId & "_" & lApplication.Applicationname & "'," & _
                                    "N'" & lApplication.Implementationtext & vbCrLf & lApplication.Implementationblock & "','" & mUserInfo.CW_ID & "')"

            End If
            lPackage.Add(nstrSql)

            'save RollBackPacks
            For Each lSql As String In lApplication.RollbackPack
                nstrSql = "INSERT INTO LOG_TABLE_ROLLBACK_SQL " & _
                          "([Request_ID]" & _
                            ",[Statement_SQL])" & _
                            " VALUES ( '" & lGboxId & "_" & lApplication.Applicationname & "', '" & lSql.Replace("'", "''") & "')"
                lPackage.Add(nstrSql)
            Next lSql
        Next lApplication
        If mUser.databasemanager.ExecutePackage(lPackage) Then
            mvUserAccess.ActiveViewIndex = 0
            lblMessage.Text = "Your request " & lGboxId & " has been submitted."
        Else
            mvUserAccess.ActiveViewIndex = 0
            lblMessage.Text = "Following error occured:" & lGboxId & vbCrLf & mUser.databasemanager.ErrText
        End If
        lstUsersToCheck.Items.Clear()

    End Sub


    Protected Sub cmdComplete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdComplete.Click
        mvUserAccess.ActiveViewIndex = 3

        pObjCurrentUsers = New myUsers
        mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        mUserInfo = mUser
        If mUserInfo Is Nothing Then
            Dim lUsername As Object = mUser.CW_ID("\")
            Dim lCW_ID As String = lUsername(1).ToString
            txtCwId.Text = lCW_ID
            txtWindowsDomain.Text = lUsername(0).ToString
        Else
            ' txtWindowsDomain.Text = mUserInfo.WINDOWS_DOMAIN
            txtCwId.Text = mUserInfo.CW_ID
            txtFirstname.Text = mUserInfo.first_name
            txtName.Text = mUserInfo.last_name

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
            ' Comment           : Remove title from database and code
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-11-26

            'txtTitle.Text = mUserInfo.TITLE

            ' Reference  END    : CR ZHHR

            txtEmail.Text = mUserInfo.SMTP_EMAIL
        End If
        Dim dt As DataTable = mUser.databasemanager.MakeDataTable("Select Subgroup_ID from Subgroup")
        With cmbSubgroup
            .DataSource = dt
            .DataTextField = "Subgroup_ID"
            .DataValueField = "Subgroup_ID"
            .DataBind()
        End With
        If Not mUserInfo Is Nothing Then
            cmbSubgroup.SelectedValue = mUserInfo.SUBGROUP_ID
        End If
    End Sub
    Private Function ValidateMyBoxes(ByVal txt As TextBox, ByVal lbl As Label) As Boolean
        Dim lStringval As New StringValidator
        If Not lStringval.VaidateChars(txt.Text) Then
            txt.Text = ""
            lbl.Text = lStringval.NotAllowedKeys & " is not allowed."
            lbl.ForeColor = Drawing.Color.Red
            Return False
        Else
            lbl.Text = ""
            Return True
        End If
    End Function
    Protected Sub cmdSubmitUserData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSubmitUserData.Click
        Dim lContextUser As String = ""
        If txtImpersonate.Text = "" Then
            lContextUser = Context.User.Identity.Name
        Else
            lContextUser = txtImpersonate.Text
        End If
        Dim lUser As myUser = mUser.gboxmanager.IsQualifiedUser(lContextUser)
        If mUser.gboxmanager.ErrString <> "" Then
            Exit Sub
        End If

        Dim lUsername As Object

        lUsername = mUser.CW_ID
       
        Dim lCW_ID As String = lUsername.ToString
        If Not ValidateMyBoxes(txtCwId, lblCwIdValidate) Then Exit Sub
        If Not ValidateMyBoxes(txtWindowsDomain, lblWindowsDomainValidate) Then Exit Sub
        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
        ' Comment           : Remove title from database and code
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2014-11-26
        'If Not ValidateMyBoxes(txtTitle, lblTitleValidate) Then Exit Sub
        ' Reference  END    : CR ZHHR 1035817

        If Not ValidateMyBoxes(txtEmail, lblEmailValidate) Then Exit Sub
        If Not ValidateMyBoxes(txtName, lblNameValidate) Then Exit Sub
        If Not ValidateMyBoxes(txtFirstname, lblFirstnameValidate) Then Exit Sub

        Dim lSql As String = ""
        If Not lUser Is Nothing Then

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
            ' Comment           : Remove title from database and code
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-11-26

            
            Dim users As New List(Of String)
            Dim user As String
            user = ""

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR YHHR 2022491 - GBOX WebForms
            ' Comment           : INC_GBox: error user data updation
            ' Added by          : Sheetal Punnapully (CWID : ETMVO)
            ' Date              : 2018-02-02


            If (lContextUser.Contains("\")) Then
                users.AddRange(lContextUser.Split("\"c))
                user = users(1).ToString()
            Else
                user = lContextUser
            End If


            lSql = "UPDATE MDRS_USER " & _
              "SET [SMTP_EMAIL] = '" & txtEmail.Text & "'" & _
              ",[last_name] = '" & txtName.Text & "'" & _
              ",[first_name] = '" & txtFirstname.Text & "'" & _
              ",[Subgroup_ID] = '" & cmbSubgroup.SelectedValue & "'" & _
              " WHERE CW_ID='" & user & "'"

            ' Reference  END    : CR ZHHR 1035817
        Else
            mUserRoles = mUser.Databasemanager.MakeDataTable("Select Application, ApplicationPart, Applicationrole, ApplicationRoleText,STATE from vw_Auth_Set_With_Desriptions where [CW_ID] ='" & lCW_ID & "'")
            'User
            If mUserRoles.Rows.Count > 0 Then

                '---------------------------------------------------------------------------------------------------
                ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
                ' Comment           : Remove title from database and code
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 2014-11-26

                lSql = "INSERT INTO MDRS_USER" & _
               "([CW_ID] " & _
               ",[SUBGROUP_ID]" & _
               ",[AREA_ID]" & _
                ",[CW_ID]" & _
               ",[WINDOWS_DOMAIN]" & _
               ",[SMTP_EMAIL]" & _
               ",[last_name]" & _
               ",[first_name])" & _
               " VALUES('" & lContextUser & "','" & _
               cmbSubgroup.SelectedValue & "','ALL','" & _
               lCW_ID & "','" & lUsername.ToString & "','" & _
               txtEmail.Text & "','" & _
               txtName.Text & "','" & _
               txtFirstname.Text & "')"

                ' Reference  END    : CR ZHHR
            End If
        End If
        Dim lPackage As New List(Of String)
        lPackage.Add(lSql)
        lSql = "Update AUTHORISATION_SET set AUTH_STATE_ID='active' where [CW_ID]='" & lCW_ID & "'"
        lPackage.Add(lSql)
        lSql = "Update AUTHORISATION_SET set CW_ID='" & lContextUser & "' where [CW_ID]='" & lCW_ID & "'"
        lPackage.Add(lSql)
        If mUser.Databasemanager.ExecutePackage(lPackage) Then
            cmdSubmitUserData.Text = "submitted"
        Else
            cmdSubmitUserData.Text = "submitted failed"
        End If
        loadMe()
        mvUserAccess.ActiveViewIndex = 0
    End Sub



    Protected Sub cmdBack_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdBack.Click
        mRequestedUsers = Nothing
        mUser.gboxmanager.RequestedApplications = Nothing
        mUser = Nothing
        mvUserAccess.ActiveViewIndex = 1
    End Sub


    Protected Sub cmdShowMySmes_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdShowMySmes.Click
        Dim lUser As myUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable("Select Application, ApplicationPart, Applicationrole, ApplicationRoleText,STATE from vw_Auth_Set_With_Desriptions where [CW_ID] ='" & lUser.CW_ID & "' AND Applicationrole = 'IMP'")
        updategrid()
        If dt.Rows.Count = 0 Then
            cmdInprogress.Enabled = False
            cmdRejekt.Enabled = False
            cmdClose.Enabled = False
            cmdMaintainCount.Enabled = False
            cmdRejectedCount.Enabled = False
            cmdClosedCount.Enabled = False
            cmdInprogress.Enabled = False
            grdSme.DataSource = Nothing
            grdSme.DataBind()
        End If
        mvUserAccess.ActiveViewIndex = 4
    End Sub
    Sub Search()
        ' Reference         : ZHHR 1054647 - GBOX Webforms: GBOX disable LOG_TABLE_USER (old workflow)
        ' Comment           : comments below code for LOG_TABLE_USER and added new code for wf_REQUEST_STATUS
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 14-Mar-2016
        Dim lSql As String = ""
        'lSql= "Select MARTY_ID, Implementation_State  from Log_Table_User where Customer = '" & mUserInfo.CW_ID & "' And SMETEXT like '%" & txtSearch.Text & "%' or MARTY_ID like '%" & txtSearch.Text & "%'"
        lSql = "Select REQUEST_ID , REQUEST_STATUS_ID  from wf_REQUEST_STATUS where CW_ID_REQUESTER LIKE '" & mUserInfo.CW_ID & "%' And REQUEST_TEXT like '%" & txtSearch.Text & "%' or REQUEST_ID like '%" & txtSearch.Text & "%'"
        grdSme.DataSource = mUser.Databasemanager.MakeDataTable(lSql)
        grdSme.DataBind()
        lblSearch.Text = grdSme.Rows.Count & " Items found."
        ' Reference End     : ZHHR 1054647
    End Sub
      Private Sub updategrid(Optional ByVal orderBy As String = "MARTY_ID", Optional ByVal lSortdirection As String = "ASC")
        Dim dt As DataTable
        ' Reference         : ZHHR 1054647 - GBOX Webforms: GBOX disable LOG_TABLE_USER (old workflow)
        ' Comment           : comments below code for LOG_TABLE_USER and added new code for wf_REQUEST_STATUS
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 15-Mar-2016
        Dim lSql As String = ""
        If mClosed Then
            'lSql = "Select MARTY_ID as ID ,Customer,Implementation_Manager as [Impl. Manager],Implementation_State as [Impl. State] from LOG_TABLE_USER where Implementation_State= 'closed' order by [" & orderBy & "] " & lSortdirection
            lSql = "Select REQUEST_ID as ID ,CW_ID_REQUESTER,CW_ID_CURRENT_RESPONSIBLE as [Impl. Manager],REQUEST_STATUS_ID as [Impl. State] from wf_REQUEST_STATUS where REQUEST_STATUS_ID= 'DONE' order by [" & orderBy & "] " & lSortdirection
            dt = mUser.Databasemanager.MakeDataTable(lSql)
        Else
            'lSql = "Select MARTY_ID as ID ,Customer,Implementation_Manager as [Impl. Manager],Implementation_State as [Impl. State] from LOG_TABLE_USER where  Implementation_State<> 'closed' AND Implementation_State<> 'rejected' order by [" & orderBy & "] " & lSortdirection
            lSql = "Select REQUEST_ID as ID ,CW_ID_REQUESTER,CW_ID_CURRENT_RESPONSIBLE as [Impl. Manager],REQUEST_STATUS_ID as [Impl. State] from wf_REQUEST_STATUS where  REQUEST_STATUS_ID<> 'DONE' AND REQUEST_STATUS_ID<> 'rejected' order by [" & orderBy & "] " & lSortdirection
            dt = mUser.Databasemanager.MakeDataTable(lSql)
            If Not dt Is Nothing Then
                If dt.Rows.Count > 0 Then
                    cmdMaintainCount.Text = dt.Rows.Count & " items to maintain."
                End If
            End If
        End If
        grdSme.DataSource = dt
        grdSme.DataBind()
        'lSql = "Select MARTY_ID as ID ,Customer,Implementation_Manager as [Impl. Manager],Implementation_State as [Impl. State] from LOG_TABLE_USER where  Implementation_State= 'closed'"
        lSql = "Select REQUEST_ID as ID ,CW_ID_REQUESTER,CW_ID_CURRENT_RESPONSIBLE as [Impl. Manager],REQUEST_STATUS_ID as [Impl. State] from wf_REQUEST_STATUS where  REQUEST_STATUS_ID= 'closed'"
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
        If Not ldt Is Nothing Then
            If ldt.Rows.Count > 0 Then
                cmdClosedCount.Text = ldt.Rows.Count & " items closed."
                '--------------------------------------------------------------
                'lSql = "Select MARTY_ID as ID ,Customer,Implementation_Manager as [Impl. Manager],Implementation_State as [Impl. State] from LOG_TABLE_USER where  Implementation_State= 'rejected'"
                lSql = "Select REQUEST_ID as ID ,CW_ID_REQUESTER,CW_ID_CURRENT_RESPONSIBLE as [Impl. Manager],REQUEST_STATUS_ID as [Impl. State] from wf_REQUEST_STATUS where  REQUEST_STATUS_ID= 'rejected'"
                ldt = mUser.Databasemanager.MakeDataTable(lSql)
                cmdRejectedCount.Text = ldt.Rows.Count & " items rejected."
                ' Reference End     : ZHHR 1054647 
            End If
        End If
    End Sub
    Protected Sub grdSme_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles grdSme.RowEditing
        lblID.Text = grdSme.Rows(e.NewEditIndex).Cells(1).Text.ToString()
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If
        ' Reference         : ZHHR 1054647 - GBOX Webforms: GBOX disable LOG_TABLE_USER (old workflow)
        ' Comment           : comments below code for LOG_TABLE_USER and added new code for wf_REQUEST_STATUS
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 15-Mar-2016
        Dim lSql As String = ""
        'lSql = "Select * from LOG_TABLE_USER where Marty_Id ='" & lblID.Text & "'"
        lSql = "Select * from wf_REQUEST_STATUS where REQUEST_ID ='" & lblID.Text & "'"
        ' Reference         : ZHHR 1054647
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
        Dim cw_id As String = dt.Rows(0)("CW_ID_REQUESTER").ToString
        If InStr(cw_id, "\") <> 0 Then
            cw_id = cw_id.Split("\")(1)
        End If
        lblRequester.Text = mUser.Databasemanager.MakeDataTable("select SMTP_EMAIL From MDRS_USER Where CW_ID ='" & cw_id & "'").ToString
        txtSmeText.Text = dt.Rows(0)("REQUEST_TEXT").ToString
        Select Case dt.Rows(0)("REQUEST_STATUS_ID").ToString
            Case "REQUESTED"
                cmdInprogress.Enabled = True
                cmdClose.Enabled = False
                cmdRejekt.Enabled = False
            Case "DONE"
                cmdInprogress.Enabled = False
                cmdClose.Enabled = False
                cmdRejekt.Enabled = False
            Case "QUEUED"
                cmdInprogress.Enabled = False
                cmdClose.Enabled = True
                cmdRejekt.Enabled = True
            Case "REJECTED"
                cmdInprogress.Enabled = False
                cmdClose.Enabled = False
                cmdRejekt.Enabled = True
        End Select
        mvUserAccess.ActiveViewIndex = 5
    End Sub

    Protected Sub inprogress_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdInprogress.Click
        Dim lPack As New List(Of String)
        ' Reference         : ZHHR 1054647 - GBOX Webforms: GBOX disable LOG_TABLE_USER (old workflow)
        ' Comment           : comments below code for LOG_TABLE_USER and added new code for wf_REQUEST_STATUS
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 15-Mar-2016
        Dim lstr As String = "Update wf_REQUEST_STATUS set [CW_ID_CURRENT_RESPONSIBLE] = '" & mUserInfo.CW_ID & "'," & _
            "[START_TIMESTAMP]=getdate()," & _
            "[REQUEST_STATUS_ID]='QUEUED' " & _
            "Where REQUEST_ID ='" & lblID.Text & "'"
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If

        lPack.Add(lstr)

        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable("Select * from wf_REQUEST_STATUS inner join MDRS_USER ON replace(CW_ID_REQUESTER,'BYACCOUNT\','') = MDRS_USER.CW_ID where REQUEST_ID ='" & lblID.Text & "'")
        ' Reference End     : ZHHR 1054647
        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
        ' Comment           : Remove title from database and code
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2014-11-26
        Dim lBody As String = "Dear " & dt.Rows(0)("first_name").ToString & " " & dt.Rows(0)("last_name").ToString & "," & vbCrLf
        ' Reference  END    : CR ZHHR 1035817

        lBody = lBody & "your request " & lblID.Text & " was set in progress by " & mUserInfo.CW_ID & vbCrLf
        lBody = lBody & vbCrLf & dt.Rows(0)("REQUEST_TEXT").ToString & vbCrLf & mFooter
        Dim nstrSQL As String = ""
        nstrSQL = "INSERT INTO [M_MAILTRIGGER]" & _
                               "([M_MAILKEY]" & _
                               ",[M_RECIPIENTS]" & _
                               ",[M_SUBJECT]" & _
                               ",[M_BODY]" & _
                               ",[M_CURRENT_SENDER])" & _
                               " VALUES('" & lblID.Text & "','" & dt.Rows(0)("SMTP_EMAIL").ToString & "','Request " & lblID.Text & " was set in progress by " & mUserInfo.CW_ID & "'," & _
                                "N'" & lBody & "','" & mUserInfo.CW_ID & "')"
        If InStr(lblID.Text, "_ACFS") <> 0 Then lPack.Add(nstrSQL)
        mUser.databasemanager.ExecutePackage(lPack)
        updategrid()
        mvUserAccess.ActiveViewIndex = 4
    End Sub

    Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdClose.Click
        Dim lPack As New List(Of String)
        Dim lstr As String = "Update wf_REQUEST_STATUS set [CW_ID_CURRENT_RESPONSIBLE] = '" & mUserInfo.CW_ID & "'," & _
           "[END_TIMESTAMP]=Getdate()," & _
           "[REQUEST_STATUS_ID]='DONE' " & _
           "Where REQUEST_ID ='" & lblID.Text & "'"
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If

        lPack.Add(lstr)
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable("Select * from wf_REQUEST_STATUS inner join MDRS_USER ON replace(CW_ID_REQUESTER,'BYACCOUNT\','') = MDRS_USER.CW_ID where REQUEST_ID ='" & lblID.Text & "'")

        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
        ' Comment           : Remove title from database and code
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2014-11-26
        Dim lBody As String = "Dear " & dt.Rows(0)("first_name").ToString & " " & dt.Rows(0)("last_name").ToString & "," & vbCrLf
        ' Reference  END    : CR ZHHR 1035817

        lBody = lBody & "your request " & lblID.Text & " was closed by " & mUserInfo.CW_ID
        lBody = lBody & vbCrLf & dt.Rows(0)("REQUEST_TEXT").ToString & vbCrLf & mFooter
        Dim nstrSQL As String = ""
        nstrSQL = "INSERT INTO [M_MAILTRIGGER]" & _
                               "([M_MAILKEY]" & _
                               ",[M_RECIPIENTS]" & _
                               ",[M_SUBJECT]" & _
                               ",[M_BODY]" & _
                               ",[M_CURRENT_SENDER])" & _
                               " VALUES('" & lblID.Text & "','" & dt.Rows(0)("SMTP_EMAIL").ToString & "','Request " & lblID.Text & " was closed by " & mUserInfo.CW_ID & "'," & _
                                "N'" & lBody & "','" & mUserInfo.CW_ID & "')"
        lPack.Add(nstrSQL)
        mUser.Databasemanager.ExecutePackage(lPack)
        updategrid()
        mvUserAccess.ActiveViewIndex = 4

    End Sub

    Protected Sub cmdClosedCount_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdClosedCount.Click
        lblClosed.Text = "True"
        mClosed = True
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable("Select REQUEST_ID as ID ,CW_ID_REQUESTER,CW_ID_CURRENT_RESPONSIBLE as [Impl. Manager],REQUEST_STATUS_ID as [Impl. State] from wf_REQUEST_STATUS where  REQUEST_STATUS_ID= 'DONE'")
        grdSme.DataSource = ldt
        grdSme.DataBind()

    End Sub

    Protected Sub cmdNewBack_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdNewBack.Click
        updategrid()
        mvUserAccess.ActiveViewIndex = 4
    End Sub


    Protected Sub cmbAuthSetSubgroup_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbAuthSetSubgroup.SelectedIndexChanged
        LoadDivision()
    End Sub
    Sub LoadDivision()
        'Dim db As DataTable = mUser.databasemanager.MakeDataTable("Select DIVISION_ID from DIVISION Where SUBGROUP_ID='" & cmbAuthSetSubgroup.SelectedValue & "'")
        'With cmbAuthSetDivision
        '    .DataSource = db
        '    .DataTextField = "DIVISION_ID"
        '    .DataValueField = "DIVISION_ID"
        '    .DataBind()
        '    .Enabled = True
        '    .SelectedValue = "ALL"
        '    .Enabled = True
        '    '.Text = "ALL"
        'End With

    End Sub

    Protected Sub grdSme_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles grdSme.Sorting
        If lblSort.Text = "ASC" Then
            updategrid(e.SortExpression, "DESC")
            lblSort.Text = "DESC"
        Else
            lblSort.Text = "ASC"
            updategrid(e.SortExpression, "ASC")
        End If
    End Sub

    Protected Sub cmdMaintainCount_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdMaintainCount.Click
        lblClosed.Text = "False"
        updategrid()

    End Sub
    Protected Sub cmdRejectedCount_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRejectedCount.Click
        lblClosed.Text = "True"
        mClosed = True
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable("Select REQUEST_ID as ID ,CW_ID_REQUESTER,CW_ID_CURRENT_RESPONSIBLE as [Impl. Manager],REQUEST_STATUS_ID as [Impl. State] from wf_REQUEST_STATUS where  REQUEST_STATUS_ID= 'REJECTED'")
        grdSme.DataSource = ldt
        grdSme.DataBind()
    End Sub


    Protected Sub cmdImpersonate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdImpersonate.Click
        loadMe()
    End Sub


    Protected Sub cmdRejekt_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRejekt.Click
        mvUserAccess.ActiveViewIndex = 7
    End Sub

    Protected Sub grdSme_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles grdSme.SelectedIndexChanged

    End Sub

    Protected Sub cmdRequestResponisible_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRequestResponisible.Click
        If cmdRequestResponisible.Text = "Back" Then
            cmdRequestResponisible.Text = "Who can request access  for me ?"

            loadMe()
        Else
            cmdRequestResponisible.Text = "Back"
            If Not mUserInfo Is Nothing Then
                grdRoles.DataSource = mUser.databasemanager.MakeDataTable("Select * from vw_Requester where Subgroup = '" & mUserInfo.SUBGROUP_ID & "'")
            Else
                grdRoles.DataSource = mUser.databasemanager.MakeDataTable("Select * from vw_Requester where Subgroup <>'ALL'")
            End If
            grdRoles.DataBind()
            lblRequestText.Text = "Your requester:"


        End If
    End Sub


    Protected Sub trvApplicationSet_SelectedNodeChanged(ByVal sender As Object, ByVal e As EventArgs) Handles trvApplicationSet.SelectedNodeChanged

    End Sub

    Protected Sub txtReason_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtReason.TextChanged
        If txtReason.Text.Trim <> "" Then
            cmdSubmitRejekt.Enabled = True
        Else
            cmdSubmitRejekt.Enabled = False

        End If
    End Sub

    Protected Sub cmdSubmitRejekt_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSubmitRejekt.Click
        Dim lPackage As New List(Of String)
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If

        Dim lGBoxId As String = lblID.Text
        Dim dt As DataTable = mUser.databasemanager.MakeDataTable("Select * from LOG_TABLE_ROLLBACK_SQL where Request_ID = '" & lblID.Text & "'")
        For Each r As DataRow In dt.Rows
            lPackage.Add(r("Statement_SQL").ToString)
        Next
        Dim lText As String = "Dear Customer," & vbCrLf & _
                              vbCrLf & _
                              "your request has been rejected by " & mUserInfo.CW_ID & "." & vbCrLf & _
                              "With the following comment: " & vbCrLf & txtReason.Text & vbCrLf & vbCrLf & _
                              "For further information or new request visit http://by-gbox.bayer-ag.com/Request-authorization/ ." & vbCrLf & _
                              vbCrLf
        Dim nstrSql As String = "INSERT INTO [M_MAILTRIGGER]" & _
                        "([M_MAILKEY]" & _
                        ",[M_RECIPIENTS]" & _
                        ",[M_SUBJECT]" & _
                        ",[M_BODY]" & _
                        ",[M_CURRENT_SENDER])" & _
                        " VALUES('" & lGBoxId & "_reject" & "','" & lblRequester.Text & "','GBOX_REQUEST:" & lGBoxId & "'," & _
                        "N'" & lText & "','G_BOX')"
        lPackage.Add(nstrSql)
        ' Reference         : ZHHR 1054647 - GBOX Webforms: GBOX disable LOG_TABLE_USER (old workflow)
        ' Comment           : comments below code for LOG_TABLE_USER and added new code for wf_REQUEST_STATUS
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 15-Mar-2016
        lPackage.Add("Update wf_REQUEST_STATUS set [CW_ID_CURRENT_RESPONSIBLE] = '" & mUserInfo.CW_ID & "'," & _
           "[END_TIMESTAMP]=Getdate()," & _
           "[REQUEST_STATUS_ID]='REJECTED' " & _
           "Where REQUEST_ID ='" & lblID.Text & "'")
        ' Reference End     : ZHHR 1054647
        If mUser.Databasemanager.ExecutePackage(lPackage) Then
            updategrid()
            mvUserAccess.ActiveViewIndex = 4
        Else
            txtReason.Text = txtReason.Text & vbCrLf & mUser.Databasemanager.ErrText
        End If

    End Sub

    Protected Sub cmdSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSearch.Click
        Search()
    End Sub
End Class