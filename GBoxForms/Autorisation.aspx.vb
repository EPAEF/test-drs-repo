Option Strict Off
Public Class Autorisation
    Inherits System.Web.UI.Page
    Private mUser As myUser
    Private WithEvents mStaticviewcontroller As MyDynamicForm_StaticViewController
#Region "Subs And Functions - PAGE LOAD"
    Public Function Authenticate_User() As myUser
        If Context.User.Identity.Name = "" Then
            Dim lbl As Label = Me.FindControl("Label" & 1)
            lbl.Text = "Can not identify user. Problems with persisting windows authentification. Activate windows persist security on the server."
            mvWizard.Visible = True
            Dim mnuWizz As New MenuItem
            mnuWizz.Text = "Problem with Domain Controller"
            mnuWizz.Value = 1
            mnuWizzard.Items.Add(mnuWizz)
            If mnuWizzard.Items.Count > 0 Then
                mnuWizzard.Items(0).Selected = True
            End If
            mvWizard.ActiveViewIndex = 0
            Return Nothing
        End If
        Dim lContextUser As String = Context.User.Identity.Name
        
        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        pObjCurrentUsers.CONTEXT = Context
        mUser = pObjCurrentUsers.GetCurrentUserByCwId(lContextUser)
        If mUser Is Nothing Then

            ' ----------------------------------------------------------------------------------------
            ' Reference  : BY-RZ04 - CMT - 28633 - 110IM08222136,110IM08175309 - "Unknown User error in accessing GBOX"
            ' Comment    : Display Error information to user if he is not able able to login to GBox site with additional debug info
            ' Created by : EQIZU
            ' Date       : 06-NOV-2011
            ' ---------------------------------------------------------------------------------------
            pVarErrMsg = pVarErrMsg & "Authorisation: Authenticate User" & vbCrLf
            lblInformations.Text = "Authorisation: unknown user"
            txtErrorMsg.Text = pGetErrorMsg() & pVarErrMsg '"unknown user" lblInformations.Text
            txtErrorMsg.Visible = True
            Return Nothing

        Else
            txtErrorMsg.Visible = False
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035817 - GBOX WebForms: Add New Function for updating columns
            ' Comment           : Added code for updating new columns in User table
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-12-12
            mUser.UserAccessStatus(mUser.CW_ID, "GBOX AUTH")
            ' Reference  END    : CR ZHHR 1035817

        End If
        Return mUser
    End Function
    Private Function WelcomeWagon() As Boolean
        Try
            lblDatabase.Text = " | Database: " & mUser.Databasemanager.cnSQL.Database & " | IIS: " & Me.Context.Server.MachineName
            lblInformations.Text = "Welcome " & mUser.first_name & " " & mUser.last_name & " | " & mUser.SUBGROUP_ID & ""
            Dim lIsadmin As Boolean = mUser.GBOXmanager.IsGBoxAdmin(mUser.CW_ID)

            If lIsadmin Then
                lblInformations.Text = lblInformations.Text & " | G|Box-Administrator" & " | User online: " & pObjCurrentUsers.GBOX_Users.Count
                lblInformations.ToolTip = pObjCurrentUsers.AllUsers
            End If
        Catch ex As Exception
            lblInformations.Text = ex.Message
        End Try
    End Function
    Private Function GetIndexByViewName(ByVal lViewName As String) As Integer
        Select Case lViewName
            Case "vwGridView"
                Return 0
            Case "vwDetails"
                Return 1
            Case "vwError"
                Return 2
            Case "vwQuery"
                Return 3
            Case "vwEditTexts"
                Return 4
            Case "vwEditSysthems"
                Return 5
            Case "vwSql"
                Return 6
            Case "vwSubscribe"
                Return 7
            Case "vwSearchEngine"
                Return 8
        End Select
    End Function
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim lContextUser As String = My.User.Name
        imgShowRequest.Enabled = True
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1048850 - GBOX Filter settings OTT 1048: New Workflow for Filter settings
        ' Comment           : Added label to show the message to user.
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 22-Oct-2015
        lblStatus.Text = ""
        ' Reference End     : ZHHR 1048850

        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        pObjCurrentUsers.CONTEXT = Context
        mUser = pObjCurrentUsers.GetCurrentUserByCwId(lContextUser)
        ' ----------------------------------------------------------------------------------------
        ' Reference  : BY-RZ04 - CMT - 28633 - 110IM08222136,110IM08175309 - "Unknown User error in accessing GBOX"
        ' Comment    : Move the code to else condtion to avoid Object Null Reference
        ' Created by : EQIZU
        ' Date       : 06-NOV-2011
        ' ---------------------------------------------------------------------------------------
        Authenticate_User()
        If mUser Is Nothing Then
            Exit Sub
        Else
            mUser.GBOXmanager.User = mUser
        End If
        WelcomeWagon()
        If mUser.GBOXmanager.CHECK_MDRS_DATABASE_MAINTENANCE() Then
            Dim lText As String = "G|Box System Access  is currently locked due to maintenance "
            lText = lText & vbCrLf & mUser.GBOXmanager.Get_MDRS_DATABASE_MAINTENANCE_STRING
            imgRefresh.Enabled = False
            changeicons()
            lblCountData.Text = lText
            Exit Sub
        End If
        If mStaticviewcontroller Is Nothing Then mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        If Not Me.IsPostBack Then
            Dim lSQL As String = "SELECT DISTINCT [APPLICATION].[APPLICATION_ID],[APPLICATION].[Description] FROM [APPLICATION] "
            lSQL = lSQL & " LEft Join AUTHORISATION_SET ON [APPLICATION].[APPLICATION_ID]= AUTHORISATION_SET.[APPLICATION_ID]"
            lSQL = lSQL & " Where (APPLICATION.Application_Type='APPLICATION' Or APPLICATION.APPLICATION_TYPE='DOCUMENTATION') And "
            lSQL = lSQL & " AUTHORISATION_SET.cw_ID ='" & My.User.Name.Split("\")(1) & "'" ' And Application_role_ID In(Select ADMINISTRATION_ROLE_ID From dbo.APPLICATION_ROLE)"
            mStaticviewcontroller.loadtrvvwGBoxAuthorizationDetailsOverview(lSQL)
        End If

        imgHelp0.OnClientClick = "javascript:window.open('" & "http://sp-appl-bbs.bayer-ag.com/sites/010045/Cockpit_Wiki/Request%20Management.aspx" & "',null, 'toolbar=yes,location=yes, status= yes,menubar=yes, scrollbars=yes,resizable=yes'); void('');"


        mStaticviewcontroller.SelectedNodeChange(mUser)
        LoadWizardData("Access Management")
        changeicons()
    End Sub
    Protected Sub cmbAuthSetSubgroup_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbAuthSetSubgroup.SelectedIndexChanged
        setchildCombos()
    End Sub
    Private Sub setchildCombos()
        If Not cmbAuthSetSubgroup.SelectedValue Is Nothing Then
            Dim lSubgroup As String = cmbAuthSetSubgroup.SelectedValue
            Dim lsql As String = "Select ORG_LEVEL_ID from ORG_LEVEL Where Subgroup_ID = '" & lSubgroup & "'"
            Dim Dt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
            If Dt.Rows.Count = 0 Then
                lblErr.Text = "ERROR: CUSTOMIZE ORG_LEVEL for " & lSubgroup & " Please contact MDRS-HOTLINE "
                imgShowRequest.Enabled = False
                changeicons()
                Exit Sub
            Else
                lblErr.Text = ""
                imgShowRequest.Enabled = True
                changeicons()
            End If

            Dim r As DataRow = Dt.Rows(0)
            FillCombo(cmbOrglevelID, "ORG_LEVEL_ID", "ORG_LEVEL", "Subgroup_ID = '" & lSubgroup & "'")
            setOrlevelValueCombo()
        End If
    End Sub
    Private Sub setOrlevelValueCombo()
        Try
            If Not cmbOrglevelID.SelectedValue Is Nothing Then
                Dim lSubgroup As String = cmbAuthSetSubgroup.SelectedValue
                Dim lOrg As String = cmbOrglevelID.SelectedValue
                Dim lsql As String = "Select * from ORG_LEVEL Where Subgroup_ID = '" & lSubgroup & "' and ORG_LEVEL_ID='" & lOrg & "'"
                Dim Dt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
                Dim r As DataRow = Dt.Rows(0)
                FillCombo(cmbOrglevelValue, r("Lookup_Table_Key").ToString, r("Lookup_Table_Name").ToString, r("Lookup_Table_Filter").ToString)
            End If
        Catch ex As Exception
            lblUser.Text = "Error: " & ex.Message
        End Try
    End Sub
    Function FillCombo(ByRef lDroplist As DropDownList, ByVal lSelect As String, ByVal lFrom As String, ByVal lWhere As String) As Double
        Dim dt As Object = Nothing
        Dim lSql As String = ""
        If lWhere = "" Then
            lSql = "SELECT [" & lSelect & "] FROM [" & lFrom & "]"
            dt = mUser.Databasemanager.MakeDataTable(lSql)
        Else
            lSql = "SELECT [" & lSelect & "] FROM [" & lFrom & "]" & " WHERE " & lWhere & ""
            dt = mUser.Databasemanager.MakeDataTable(lSql)
        End If
        With lDroplist
            .DataSource = dt
            .DataTextField = lSelect
            .DataValueField = lSelect
            .DataBind()
            If lSelect = "SUBGROUP_ID" And lDroplist.UniqueID = "cmbAuthSetSubgroup" Then
                .SelectedValue = "ALL"
                setchildCombos()
            End If
        End With
    End Function
    Sub changeicons()
        With imgShowRequest
            If .Enabled Then
                .ImageUrl = "~\Images\page_go.gif"
            Else
                .ImageUrl = "~\Images\page_go_grey.gif"
            End If
        End With
        With imgUserAdd
            If .Enabled Then
                .ImageUrl = "~\Images\user_add.gif"
            Else
                .ImageUrl = "~\Images\user_add_grey.gif"
            End If
        End With
        With imgUserDelete
            If .Enabled Then
                .ImageUrl = "~\Images\user_delete.gif"
            Else
                .ImageUrl = "~\Images\user_delete_grey.gif"
            End If
        End With
        With imgCancel_Role
            If .Enabled Then
                .ImageUrl = "~\Images\Cancel.gif"
            Else
                .ImageUrl = "~\Images\Cancel_grey.gif"
            End If
        End With
        With imgMailingList
            If .Enabled Then
                .ImageUrl = "~\Images\book_addresses.gif"
            Else
                .ImageUrl = "~\Images\book_addresses_grey.gif"
            End If
        End With
    End Sub
#End Region
    Private Sub grdvwGBoxAuthorizationDetailsOverview_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdvwGBoxAuthorizationDetailsOverview.DataBound
        changeicons()
    End Sub
    Protected Sub grdvwGBoxAuthorizationDetailsOverview_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles grdvwGBoxAuthorizationDetailsOverview.Sorting
        Dim lSql As String = ""
        Dim lAPPLICATION_ID As String = ""
        Dim lAPPLICATION_PART_ID As String = ""
        With trvvwGBoxAuthorizationDetailsOverview.SelectedNode
            If .Parent.Parent.Parent Is Nothing Then
                lAPPLICATION_ID = .Parent.Parent.Text.Split("(")(0)
                lAPPLICATION_PART_ID = .Parent.Text.Split("(")(0)
            Else
                lAPPLICATION_ID = .Parent.Parent.Parent.Text.Split("(")(0)
                lAPPLICATION_PART_ID = .Parent.Parent.Text.Split("(")(0)
            End If
            lSql = "Select SUBGROUP_ID as subgroup ,CW_ID as [cw id] from AUTHORISATION_SET " & _
                                  "where APPLICATION_ROLE_ID='" & .Text.Split("(")(0) & "'" & _
                                  " And APPLICATION_PART_ID='" & lAPPLICATION_PART_ID & "'" & _
                                  " And APPLICATION_ID = '" & lAPPLICATION_ID & "'"
        End With
        mStaticviewcontroller.updategrid(grdvwGBoxAuthorizationDetailsOverview, lSql, "[" & e.SortExpression & "]", "DESC")
    End Sub
    Protected Sub trvvwGBoxAuthorizationDetailsOverview_SelectedNodeChanged(ByVal sender As Object, ByVal e As EventArgs) Handles trvvwGBoxAuthorizationDetailsOverview.SelectedNodeChanged
        If mStaticviewcontroller Is Nothing Then
            mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        End If
        Dim myPath As Array = trvvwGBoxAuthorizationDetailsOverview.SelectedNode.ValuePath.Split("/")
        If myPath.GetUpperBound(0) < 2 Then
            imgMailingList.Enabled = False
        Else
            imgMailingList.Enabled = True
        End If
        lblError.Text = ""
        Dim lAccess As Boolean = mUser.GBOXmanager.Authorisation(trvvwGBoxAuthorizationDetailsOverview.SelectedNode.ValuePath)
        mStaticviewcontroller.SelectedNodeChange(mUser)
        imgUserAdd.Enabled = lAccess
        imgUserDelete.Enabled = lAccess
        '---------------------------------------------------------------------------------------------
        ' Reference     : CR ZHHR 1061748 - GBOX AUTH: OTT 3427 - Display function for auth management 
        ' Comment       : Disable the buttons when "Display only" checkbox is checked
        ' Added by      : Pratyusa Lenka (CWID : EOJCG) 
        ' Date          : 2016-09-01
        If chkDisplayOnly.Checked Then
            imgUserAdd.Enabled = False
            imgUserDelete.Enabled = False
        End If
        ' Reference END : CR ZHHR 1061748 
        '---------------------------------------------------------------------------------------------
        grdvwGBoxAuthorizationDetailsOverview.Columns(0).Visible = lAccess
        If Not trvvwGBoxAuthorizationDetailsOverview.SelectedNode Is Nothing Then
            If Not trvvwGBoxAuthorizationDetailsOverview.SelectedNode.Parent Is Nothing Then
                grdvwGBoxAuthorizationDetailsOverview.Visible = False
                lblCountData.Visible = False
                If Not trvvwGBoxAuthorizationDetailsOverview.SelectedNode.Parent.Parent Is Nothing Then
                    grdvwGBoxAuthorizationDetailsOverview.Visible = True
                    lblCountData.Visible = True
                End If
            Else
                grdvwGBoxAuthorizationDetailsOverview.Visible = False
                lblCountData.Visible = False
            End If
        Else
            grdvwGBoxAuthorizationDetailsOverview.Visible = True
            lblCountData.Visible = True
        End If
        changeicons()
        mvGBoxAuthorizationOverview.ActiveViewIndex = 0
    End Sub
    Private Sub mStaticViewController_AddNew(ByVal lEnabled As Boolean) Handles mStaticviewcontroller.AddNew
        imgUserAdd.Enabled = lEnabled
        changeicons()
    End Sub
    Private Sub mStaticViewController_Cancel() Handles mStaticviewcontroller.Cancel
        imgUserAdd.Enabled = False
        imgUserDelete.Enabled = False
        changeicons()
        Dim lSQL As String = "SELECT DISTINCT [APPLICATION].[APPLICATION_ID],[APPLICATION].[Description] FROM [APPLICATION] "
        lSQL = lSQL & " LEft Join AUTHORISATION_SET ON [APPLICATION].[APPLICATION_ID]= AUTHORISATION_SET.[APPLICATION_ID]"
        lSQL = lSQL & " Where (APPLICATION.Application_Type='APPLICATION' Or APPLICATION.APPLICATION_TYPE='DOCUMENTATION') And "
        lSQL = lSQL & " AUTHORISATION_SET.cw_ID ='" & My.User.Name.Split("\")(1) & "'"
        mStaticviewcontroller.loadtrvvwGBoxAuthorizationDetailsOverview(lSQL)
    End Sub
    Private Sub mStaticViewController_Delete(ByVal lEnabled As Boolean) Handles mStaticviewcontroller.Delete
        imgUserDelete.Enabled = lEnabled
        changeicons()
    End Sub

    Protected Sub imgRefresh_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgRefresh.Click
        If mStaticviewcontroller Is Nothing Then
            mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        End If
        Dim lSQL As String = "SELECT DISTINCT [APPLICATION].[APPLICATION_ID],[APPLICATION].[Description] FROM [APPLICATION] "
        lSQL = lSQL & " LEft Join AUTHORISATION_SET ON [APPLICATION].[APPLICATION_ID]= AUTHORISATION_SET.[APPLICATION_ID]"
        lSQL = lSQL & " Where (APPLICATION.Application_Type='APPLICATION' Or APPLICATION.APPLICATION_TYPE='DOCUMENTATION') And "
        lSQL = lSQL & " AUTHORISATION_SET.cw_ID ='" & My.User.Name.Split("\")(1) & "'"
        mStaticviewcontroller.loadtrvvwGBoxAuthorizationDetailsOverview(lSQL)
    End Sub
    Protected Sub imgMailingList_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgMailingList.Click
        If mStaticviewcontroller Is Nothing Then
            mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        End If
        mStaticviewcontroller.CreateMailingList(Response)
        mStaticviewcontroller.SelectedNodeChange(mUser)
    End Sub
    Protected Sub imgUserAdd_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgUserAdd.Click
        If trvvwGBoxAuthorizationDetailsOverview.SelectedNode Is Nothing Then
            Exit Sub
        End If
        If Not cmbAuthSetSubgroup.SelectedValue Is Nothing Then
            FillCombo(cmbAuthSetSubgroup, "SUBGROUP_ID", "SUBGROUP", "")
        End If
        Dim lAccess As Boolean = mUser.GBOXmanager.Authorisation(trvvwGBoxAuthorizationDetailsOverview.SelectedNode.ValuePath)
        If Not lAccess Then Exit Sub
        lblRole.Text = trvvwGBoxAuthorizationDetailsOverview.SelectedNode.ValuePath.Replace("|", " ") & "|" & trvvwGBoxAuthorizationDetailsOverview.SelectedNode.Text
        mvGBoxAuthorizationOverview.ActiveViewIndex = 1
        imgCancel_Role.Enabled = True
        changeicons()
    End Sub
    Protected Sub imgUserDelete_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgUserDelete.Click
        If mStaticviewcontroller Is Nothing Then
            mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        End If
        Dim lAccess As Boolean = mUser.GBOXmanager.Authorisation(trvvwGBoxAuthorizationDetailsOverview.SelectedNode.ValuePath)
        If Not lAccess Then Exit Sub
        Dim lOnChecked As Boolean
        For Each r As GridViewRow In grdvwGBoxAuthorizationDetailsOverview.Rows
            If mUser.Databasemanager.GetCheckedValuebyId(Request.Form, r.Cells(0).Controls(0).UniqueID) Then
                lOnChecked = True
                Exit For
            End If
        Next r
        If Not lOnChecked Then Exit Sub
        mStaticviewcontroller.CreateDeleteUserRequest(trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview)
        lblRole.Text = trvvwGBoxAuthorizationDetailsOverview.SelectedNode.ValuePath.Replace("|", " ") & "|" & trvvwGBoxAuthorizationDetailsOverview.SelectedNode.Text

        txtRequest.Text = mUser.RequestText
        lblInsertMode.Text = "DELETE"
        mvGBoxAuthorizationOverview.ActiveViewIndex = 2
    End Sub
    Protected Sub imgCancelRequest_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgCancelRequest.Click
        ResetAuthorization()
    End Sub
    Public Sub ResetAuthorization()
        mvGBoxAuthorizationOverview.ActiveViewIndex = 0
        If mStaticviewcontroller Is Nothing Then
            mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        End If
        mStaticviewcontroller.SelectedNodeChange(mUser)
        Dim lSQL As String = "SELECT DISTINCT [APPLICATION].[APPLICATION_ID],[APPLICATION].[Description] FROM [APPLICATION] "
        lSQL = lSQL & " LEft Join AUTHORISATION_SET ON [APPLICATION].[APPLICATION_ID]= AUTHORISATION_SET.[APPLICATION_ID]"
        lSQL = lSQL & " Where (APPLICATION.Application_Type='APPLICATION' Or APPLICATION.APPLICATION_TYPE='DOCUMENTATION') And "
        lSQL = lSQL & " AUTHORISATION_SET.cw_ID ='" & My.User.Name.Split("\")(1) & "'"
        mStaticviewcontroller.loadtrvvwGBoxAuthorizationDetailsOverview(lSQL)
        lstUsersToCheck.Items.Clear()
        txtComment.Text = ""
    End Sub
    Protected Sub imgUserAdd_Role_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgUserAdd_Role.Click
        If mStaticviewcontroller Is Nothing Then
            mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        End If
        '--------------------------------------------------------------------------------------------------------- 
        ' Reference     : CR ZHHR 1035650 - GBOX AUTH MGMT: No error message when requests a role for his own CWID 
        ' Comment       : Error should display on top instead of down
        ' Added by      : Pratyusa Lenka (CWID : EOJCG) 
        ' Date          : 2014-11-24 
        'If mStaticviewcontroller.Adduser(txtUserToAdd, lstUsersToCheck, lblError) Then
        If mStaticviewcontroller.Adduser(txtUserToAdd, lstUsersToCheck, lblErr) Then
            imgShowRequest.Enabled = True
        Else
            imgShowRequest.Enabled = False
        End If
        lblInsertMode.Text = "INSERT"
        changeicons()
        ' Reference END : CR ZHHR 1035650 
        ' Added by      : Pratyusa Lenka (CWID : EOJCG)
        '---------------------------------------------------------------------------------------------------------
    End Sub
    Protected Sub imgUserDelete_Role_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgUserDelete_Role.Click
        If mStaticviewcontroller Is Nothing Then
            mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        End If
        mStaticviewcontroller.RemoveUser(lstUsersToCheck, imgShowRequest)
        '--------------------------------------------------------------------------------------------------------- 
        ' Reference     : CR ZHHR 1035650 - GBOX AUTH MGMT: No error message when requests a role for his own CWID 
        ' Comment       : Clear the error message
        ' Added by      : Pratyusa Lenka (CWID : EOJCG) 
        ' Date          : 2014-11-24 
        lblErr.Text = ""
        ' Reference END : CR ZHHR 1035650 
        ' Added by      : Pratyusa Lenka (CWID : EOJCG)
        '---------------------------------------------------------------------------------------------------------
        changeicons()
    End Sub
    Protected Sub imgCancel_Role_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgCancel_Role.Click
        ResetAuthorization()
    End Sub
    Protected Sub imgShowRequest_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgShowRequest.Click
        If txtComment.Text = "" Then
            lblError.Text = "please fill comment field!"
            lblError.Visible = True
            lblError.ForeColor = Drawing.Color.Red
            Exit Sub
        End If
        If mStaticviewcontroller Is Nothing Then
            mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        End If
        '---------------------------------------------------------------------------------------------
        ' Reference     : CR ZHHR 1063723 - GBOX AUTH: Comment is missing in authorisation requests 
        ' Comment       : Add AUTH request comment to display in confirmation mail and update DB table
        ' Added by      : Pratyusa Lenka (CWID : EOJCG) 
        ' Date          : 2016-10-26
        mStaticviewcontroller.Comment = txtComment.Text.Trim
        ' Reference END : CR ZHHR 1063723 
        '---------------------------------------------------------------------------------------------
        txtRequest.Text = mStaticviewcontroller.fillobjects(lstUsersToCheck, lblRole.Text, cmbAuthSetSubgroup.Text, cmbOrglevelID.Text, cmbOrglevelValue.Text)
        txtRequest.Text = txtRequest.Text '& vbCrLf & lblRole.Text
        mUser.RequestText = txtRequest.Text '& vbCrLf & lblRole.Text
        txtRequest.Text = txtRequest.Text & vbCrLf & txtComment.Text
        mUser.RequestText = txtRequest.Text '& vbCrLf & txtComment.Text
        mvGBoxAuthorizationOverview.ActiveViewIndex = 2
    End Sub
    Private Sub LoadWizardData(ByVal lWizardobj As String)
        Try
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
            End If
            Dim i As Long = 1
            mnuWizzard.Items.Clear()
            Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable("Select * from OBJ_WIZARD Where WIZ_ID = '" & lWizardobj & " 'And (Subgroup_ID='ALL' or Subgroup_ID='" & mUser.SUBGROUP_ID & "') order by Rank")
            For Each r As DataRow In mdt.Rows
                Dim mnuWizz As New MenuItem
                mnuWizz.Text = r("StepTitle").ToString
                mnuWizz.Value = i
                mnuWizzard.Items.Add(mnuWizz)
                Dim lbl As Label = Me.FindControl("Label" & i)
                lbl.Text = r("Text").ToString
                i = i + 1
            Next
            If mnuWizzard.Items.Count > 0 Then
                mnuWizzard.Items(0).Selected = True
            End If
            mvWizard.Visible = True
            mvWizard.ActiveViewIndex = 0
            If mnuWizzard.Items.Count < 2 Then
                mnuWizzard.Enabled = False
            Else
                mnuWizzard.Enabled = True
            End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub imgSubmitRequest_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgSubmitRequest.Click
        Dim lApplication As String = lblRole.Text.Split("/")(0).Trim.Split("(")(0).Trim
        Dim lApplicationpart As String = lblRole.Text.Split("/")(1).Trim.Split("(")(0).Trim

        If mStaticviewcontroller Is Nothing Then
            mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        End If
        '---------------------------------------------------------------------------------------------
        ' Reference     : CR ZHHR 1063723 - GBOX AUTH: Comment is missing in authorisation requests 
        ' Comment       : Add AUTH request comment to display in confirmation mail and update DB table
        ' Added by      : Pratyusa Lenka (CWID : EOJCG) 
        ' Date          : 2016-10-26
        mStaticviewcontroller.Comment = txtComment.Text.Trim
        ' Reference END : CR ZHHR 1063723 
        '---------------------------------------------------------------------------------------------
        If lblInsertMode.Text.ToUpper = "INSERT".ToUpper Then
            lblStatus.Text = mStaticviewcontroller.SubmitInsertUser(lApplicationpart)
        Else
            lblStatus.Text = mStaticviewcontroller.SubmitCancelUser(lApplicationpart)
            'lblStatus.Text = mStaticviewcontroller.SubmitCancelUser("_AUTH")
        End If
        ResetAuthorization()
    End Sub

    Private Sub mStaticViewController_XML(ByVal lEnabled As Boolean) Handles mStaticviewcontroller.XML
          changeicons()
    End Sub
    Protected Sub txtComment_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtComment.TextChanged
        If txtComment.Text <> "" Then
            imgShowRequest.Enabled = True
        Else
            imgShowRequest.Enabled = False
        End If
    End Sub

    Private Sub cmbOrglevelID_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbOrglevelID.SelectedIndexChanged
        setOrlevelValueCombo()
    End Sub

    Protected Sub mnuWizzard_MenuItemClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MenuEventArgs) Handles mnuWizzard.MenuItemClick
        mvWizard.ActiveViewIndex = mnuWizzard.SelectedItem.Value - 1
    End Sub
    ''' <summary>
    ''' Reference : CR ZHHR 1038879 - GBOX AUTH MGMT: GBOX auth management
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2015-04-06
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Display Administrative roles while click on checkbox</remarks>
    Protected Sub chkDisplayAdminRoles_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkDisplayAdminRoles.CheckedChanged
        If mStaticviewcontroller Is Nothing Then mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        Dim lSQL As String = "SELECT DISTINCT [APPLICATION].[APPLICATION_ID],[APPLICATION].[Description] FROM [APPLICATION] "
        lSQL = lSQL & " LEft Join AUTHORISATION_SET ON [APPLICATION].[APPLICATION_ID]= AUTHORISATION_SET.[APPLICATION_ID]"
        lSQL = lSQL & " Where (APPLICATION.Application_Type='APPLICATION' Or APPLICATION.APPLICATION_TYPE='DOCUMENTATION') And "
        lSQL = lSQL & " AUTHORISATION_SET.cw_ID ='" & My.User.Name.Split("\")(1) & "'"
        mStaticviewcontroller.IsAdminRole = chkDisplayAdminRoles.Checked
        mStaticviewcontroller.loadtrvvwGBoxAuthorizationDetailsOverview(lSQL)
    End Sub
    ''' <summary>
    ''' Reference : ZHHR 1061748 - GBOX AUTH: OTT 3427 - Display function for auth management
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2016-09-01 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>When checked, the buttons "Add user" and "Delete user" should be deactivated and the whole tree should be loaded</remarks>
    Protected Sub chkDisplayOnly_CheckedChanged(sender As Object, e As EventArgs) Handles chkDisplayOnly.CheckedChanged
        If mStaticviewcontroller Is Nothing Then mStaticviewcontroller = New MyDynamicForm_StaticViewController(mUser, Request.Form, trvvwGBoxAuthorizationDetailsOverview, grdvwGBoxAuthorizationDetailsOverview, lblCountData)
        If chkDisplayOnly.Checked Then
            imgUserAdd.Enabled = False
            imgUserDelete.Enabled = False
            mStaticviewcontroller.loadtrvvwGBoxAuthorizationDetailsOverview()
        Else
            Dim lSQL As String = "SELECT DISTINCT [APPLICATION].[APPLICATION_ID],[APPLICATION].[Description] FROM [APPLICATION] "
            lSQL = lSQL & " Left Join AUTHORISATION_SET ON [APPLICATION].[APPLICATION_ID]= AUTHORISATION_SET.[APPLICATION_ID]"
            lSQL = lSQL & " Where (APPLICATION.Application_Type='APPLICATION' Or APPLICATION.APPLICATION_TYPE='DOCUMENTATION') And "
            lSQL = lSQL & " AUTHORISATION_SET.CW_ID ='" & My.User.Name.Split("\")(1) & "'"
            mStaticviewcontroller.loadtrvvwGBoxAuthorizationDetailsOverview(lSQL)
        End If
        changeicons()
    End Sub
End Class
