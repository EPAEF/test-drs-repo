
Option Strict Off

Imports System.IO

Partial Public Class DynamicForm
    Inherits System.Web.UI.Page

#Region "Declare Private Vars"

    Public Enum ViewStatus
        MainMenu = 0
        Detailsmenu = 1
        TreeClick = 2
        NodeClick = 3
        ImageClick = 4
        SelectLink = 5
        WizzardMenu = 6
        DetailViewInnerControls = 7
        GridCheckBoxClick = 8
        ButtonClick = 9
    End Enum
    Private mOBJ_FIELD_ID As String
    Private mOBJ_Field_Type_ID As String
    Private mUser As myUser
    Private mErrText As String = "Due to a technical problem your GBOX request could not be posted. Please send the complete error message to our hotline at http://by-gbox.bayer-ag.com/HOTLINE/" & vbCrLf
    Private m_Topic_Group_Context_ID As String = "GENERAL"
    Private WithEvents mControllerfactory As Dynamic_View_Controller_Factory
    Private WithEvents mDynamicFormController As Dynamic_View_Controller
    Private mPath As String = ""
    Private mLocked As Boolean = False
    Private mWithPaging As Boolean = True
    Private strResult As String
    Private lSPack As New List(Of String)
    Private strDescription As String = ""
    Private lstDDL As New List(Of KeyValuePair(Of String, String))
    Private lstObjKeyFields As New List(Of KeyValuePair(Of String, String))
    Public Shared Files As ArrayList = New ArrayList()
    Private isObjChanged As Boolean = False   'CRT-2066302 : For selecting Default All in filter functionality--by Kanchan Bhor

#End Region
#Region "HANDLES ControlerEvents"



    Private Sub mDynamicFormController_CancelMany(ByVal lHits As Long) Handles mDynamicFormController.CancelMany
        lblToMany.Text = lHits & " Hits"
    End Sub
    Private Sub mDynamicFormController_ChangeView(ByVal lViewindex As Long) Handles mDynamicFormController.ChangeView
        mvContents.ActiveViewIndex = lViewindex
    End Sub
    Private Sub mDynamicFormController_ShowSql(ByVal lsql As String) Handles mDynamicFormController.ShowSql
        lblSQlReport.Visible = True
        lblSQlReport.Text = lsql
    End Sub
    Private Sub mDynamicFormController_TooManyInfo(ByVal lToManyText As String) Handles mDynamicFormController.TooManyInfo
        lblToMany.Visible = True
        lblToMany.Text = lToManyText
        lblToMany.ForeColor = Drawing.Color.DarkRed
    End Sub
    Private Sub mDynamicFormController_DatabaseChange(ByVal lDatabase As String) Handles mDynamicFormController.DatabaseChange
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1039680 - GBOX COC: Move Database Information on screen
        ' Comment           : Move database information on screen
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2015-03-05

        'lblDatabase.Text = " | Database: " & lDatabase & " | IIS: " & Me.Context.Server.MachineName
        lblDatabase.Text = " | IIS: " & Me.Context.Server.MachineName
        lblDatabasename.Text = " Database: " & lDatabase

        ' Reference End      : ZHHR 1039680
    End Sub
    Private Sub mDynamicFormController_ErrorInfo(ByVal lErrText As String) Handles mDynamicFormController.ErrorMessage
        lblStatus.Text = lErrText
        grdDat.DataSource = Nothing
    End Sub
    Private Sub mDynamicFormController_imgQuery(ByVal lEnabled As Boolean) Handles mDynamicFormController.imgQuery
        mvContents.ActiveViewIndex = GetIndexByViewName("vwQuery")
        imgQuery.Enabled = lEnabled
        ChangeIcons()
    End Sub
    Private Sub mDynamicFormController_ShowTheQuery() Handles mDynamicFormController.ShowTheQuery
        mvContents.ActiveViewIndex = GetIndexByViewName("vwQuery")
        imgQuery.Enabled = True
    End Sub
    Private Sub mDynamicFormController_Locked(ByVal lMessage As String) Handles mDynamicFormController.Locked
        lblStatus.Text = lMessage
        mLocked = True
        mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
    End Sub
    Private Sub mControllerfactory_LoadWizzardData(ByVal lTopic As String) Handles mControllerfactory.LoadWizzardData
        LoadWizardData(lTopic)
    End Sub
#End Region
#Region "HANDLES FORM CONTROLS EVENTS"
#Region "Datagrids"
    Private Sub grdDat_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdDat.DataBinding
        Try
            If grdDat.DataSource Is Nothing Then Exit Sub
            BindDocumentation()
            'CheckAlert()
            ChangeIcons()

        Catch ex As Exception

            mErrText &= "grddat:databind:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        End Try
    End Sub


    Private Sub DisplayAdditionalDRSInfo()

        Dim lmnuIndex As Integer = 2
        '------------------------------------------------------------------------------------------------------------
        ' Reference         : CR - BY-RZ04-CMT-27932 - Enhance GBOX with a database parametered download website
        ' Comment           : Modified query - added filter criteria [dp_Additional_Information_Classification_ID]='Display' for tabs in the main screen
        ' Added by          : Surendra Purav (CWID : EQIZU)
        ' Date              : 2013-11-26
        '------------------------------------------------------------------------------------------------------------
        Dim ldtAddDRSInfo As DataTable = mUser.Databasemanager.MakeDataTable("SELECT TITLE,SOURCE, DESCRIPTION FROM dp_Additional_Information where ACTIVE = 1 and dp_Additional_Information_Classification_ID = 'Display' Order by RANK")
        Dim lSQL = ""
        HideAdditionalDRSInfo()
        Dim strURL As String
        Dim hlOrgLevelValue As HyperLink

        If ldtAddDRSInfo.Rows.Count > 0 Then
            For Each r As DataRow In ldtAddDRSInfo.Rows
                Try
                    Dim lgrVw As GridView = Me.FindControl("grd" & lmnuIndex)
                    Dim lnewMenuitem As New MenuItem
                    lnewMenuitem.Text = r("TITLE").ToString
                    lnewMenuitem.ToolTip = r("DESCRIPTION").ToString
                    lnewMenuitem.Value = lmnuIndex
                    mnuDocTab.Items.AddAt(lmnuIndex, lnewMenuitem)

                    lSQL = r("SOURCE").ToString
                    lSQL = lSQL.Replace("|OBJ_ID|", mUser.Current_OBJ.OBJ_ID)

                    Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lSQL)
                    If ldt.Rows.Count = 0 Then
                        lgrVw.DataSource = Nothing
                        lgrVw.DataBind()
                    Else
                        lgrVw.DataSource = ldt
                        lgrVw.DataBind()
                        '-------------------------------------------------------------------------------------------
                        ' Reference         : ZHHR 1038188 - GBOX COC: GBOX DRS Handbook Link approver and requester
                        ' Comment           : Add hyperlink to ORG_LEVEL_VALUE
                        ' Added by          : Pratyusa Lenka (CWID : EOJCG)
                        ' Date              : 2015-02-11
                        For Each gr As GridViewRow In lgrVw.Rows
                            strURL = String.Empty
                            hlOrgLevelValue = New HyperLink
                            hlOrgLevelValue.Text = gr.Cells(1).Text
                            strURL = String.Format("RequesterApproverDetails.aspx?GroupId={0}&Team={1}", HttpUtility.UrlEncode(hlOrgLevelValue.Text.Replace("&amp;", "&"), System.Text.Encoding.Default), r("TITLE").ToString)
                            hlOrgLevelValue.NavigateUrl = "javascript:void(window.open('" + Server.UrlEncode(strURL) + "','popup_window', 'width=500,height=400,menubar=no,resizable=no,scrollbars=yes,toolbar=no'));"
                            gr.Cells(1).Controls.Add(hlOrgLevelValue)
                        Next
                        ' Reference  END    : CR ZHHR 1038188
                    End If
                Catch ex As Exception
                    mErrText &= "DisplayAdditionalDRSInfo:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
                End Try

                lmnuIndex = lmnuIndex + 1
            Next
        End If
    End Sub

    Private Sub HideAdditionalDRSInfo()
        Try
            For lmnuIndex = 2 To 6
                If mnuDocTab.Items.Count > 2 Then
                    mnuDocTab.Items.RemoveAt(2)
                End If
            Next
        Catch ex As Exception
            '// Skip the error            
        End Try
    End Sub

    Private Sub BindDocumentation()
        Try
            If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "DOCUMENTATION" Then
                mnuDocTab.Items(1).Text = ""
                HideAdditionalDRSInfo()
                mnuDocTab.Enabled = False
                mnuDocTab.Items(0).Text = "Info"
                mnuDocTab.Items(0).Selected = True
                mvDocuTab.ActiveViewIndex = 0
                Exit Sub
            Else
                mnuDocTab.Items(0).Text = "Data"
                mnuDocTab.Enabled = True
            End If
            Dim lSQL = ""
            lSQL = "SELECT  "
            lSQL = lSQL & "OBJ_ID,TOPIC_ID, OBJ_DOCUMENTATION_FIELD_NAME AS name"
            lSQL = lSQL & ",OBJ_DOCUMENTATION_FIELD_VALUE as value "
            lSQL = lSQL & "FROM OBJ_DOCUMENTATION   where Topic_ID ='" & mUser.Current_OBJ.TOPIC_ID & "' AND OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "' Order by RANK"
            Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lSQL)
            mnuDocTab.Items(1).Text = "Information (Path)"

            If ldt.Rows.Count = 0 Then

                lSQL = "SELECT  "
                lSQL = lSQL & "OBJ_ID,TOPIC_ID, OBJ_DOCUMENTATION_FIELD_NAME AS name"
                lSQL = lSQL & ",OBJ_DOCUMENTATION_FIELD_VALUE as value "
                lSQL = lSQL & "FROM OBJ_DOCUMENTATION   where  OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "' Order by RANK"
                ldt = mUser.Databasemanager.MakeDataTable(lSQL)
                mnuDocTab.Items(1).Text = "Information (Object)"
            End If
            If ldt.Rows.Count = 0 Then mnuDocTab.Items(1).Text = "No Info defined"
            grdDocu.DataSource = ldt
            grdDocu.DataBind()

            'Display Additional DRS Handbook Information
            DisplayAdditionalDRSInfo()

        Catch ex As Exception
            grdDocu.DataSource = Nothing
            grdDocu.DataBind()
        End Try

    End Sub
    'Private Function CheckAlert() As Boolean
    '    Dim lsql As String = "Select OBJ_ID from wf_DEFINE_WORKFLOW_DETAILS Where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "'"
    '    Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
    '    If dt.Rows.Count = 0 Then
    '        imgAlert.Enabled = False
    '        imgmySubscriptions.Enabled = False
    '        imgAlert.ToolTip = "No Alert defined"
    '    Else
    '        imgmySubscriptions.Enabled = True
    '        imgAlert.Enabled = True
    '    End If

    'End Function
    Private Sub grdDat_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles grdDat.PageIndexChanging
        Dim lpage As String = "" 'Dummyeinrag, damit der eventhandler nicht weggemobbt wird
    End Sub




    Protected Sub grdDat_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles grdDat.SelectedIndexChanged
        Try
            If mLocked Then
                mLocked = False
                Exit Sub
            Else
                lblStatus.Text = ""
            End If
            If Not mUser.GBOXmanager.AuthenticateRequest(trvOBJ.SelectedNode.Value, mUser.CW_ID) Then
                lblStatus.Text = "You have no permission to request values for " & trvOBJ.SelectedNode.Value & "."
                imgCancel_Click(sender, Nothing)
                Exit Sub
            End If
            If mUser.GBOXmanager.CHECK_MDRS_DATABASE_MAINTENANCE = True Then
                lblStatus.Text = mUser.GBOXmanager.Get_MDRS_DATABASE_MAINTENANCE_STRING
                imgCancel_Click(sender, Nothing)
                Exit Sub
            End If
            '---------------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
            ' Comment   : New concept for multiple text functionality
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-03-24
            '--------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
            ' Comment   : Delete COMPOSITE_S_T object type
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-01-25
            If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID.ToUpper = "ADMINISTRATION".ToUpper _
            Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID.ToUpper = "DOCUMENTATION".ToUpper _
            Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID.ToUpper = "COMPOSITE_S_T_TXT".ToUpper _
            Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID.ToUpper = "COMPOSITE_MULTI_TXT".ToUpper _
            Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID.ToUpper = "SINGLE" Then
                mUser.RequestType = myUser.RequestTypeOption.update
                btnSubmit.CommandArgument = "Update"
            Else
                mUser.RequestType = myUser.RequestTypeOption.insert
                btnSubmit.CommandArgument = "Insert"
            End If
            ' Reference END : CR ZHHR 1052471
            '--------------------------------------------------------------------------------
            mvContents.ActiveViewIndex = GetIndexByViewName("vwDetails")

            dvInfo.EnableModelValidation = True
            dvInfo.ChangeMode(DetailsViewMode.Edit)
            dvInfo.DataBind()
            '---------------------------------------------------------------------
            ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
            ' Comment   : Placed new Add value button to create new request
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-12-24
            btnAddValue.Enabled = False
            ' Reference END : CR ZHHR 1050708
            '---------------------------------------------------------------------

            '---------------------------------------------------------------------------------------------------
            ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
            ' Comment           : System dependent workflow
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 23-Feb-2016
            btnCopyValue.Enabled = False
            ' Reference End     : ZHHR 1053017

            mUser.EditMode = True
            imgCancel.Enabled = True
            'MakeSomeEnDisabling(False)
            ChangeIcons()
        Catch ex As Exception
            mErrText &= "grdDat.selectedindexchange:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
        End Try
    End Sub

#End Region
#Region "Menues"
    Protected Sub mnuDocTab_MenuItemClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MenuEventArgs) Handles mnuDocTab.MenuItemClick
        mvDocuTab.ActiveViewIndex = mnuDocTab.SelectedItem.Value
        If mnuDocTab.SelectedItem.Value = 0 Then
            ' ----------------------------------------------------------------------------------------
            ' Reference  : BY-RZ04-CMT-28500 -  110IM08181627- Select Button dissapears by using the Tabpages
            ' Comment    : Reload the page 
            ' Created by : EQIZU
            ' Date       : 30-OCT-2013
            ' ---------------------------------------------------------------------------------------

            If IsPostBack Then
                Dim lParamlist As String = ""
                With Request
                    If Not .Params("CONTEXT") Is Nothing Then
                        lParamlist = "?CONTEXT=" & .Params("CONTEXT")
                    Else
                        lParamlist = "?CONTEXT=general"
                    End If

                    If Not .Params("TOPICGROUP") Is Nothing Then
                        lParamlist = lParamlist & "&TOPICGROUP=" & .Params("TOPICGROUP")
                    End If

                    If Not .Params("TOPIC") Is Nothing Then
                        lParamlist = lParamlist & "&TOPIC=" & .Params("TOPIC")
                    End If

                    If Not .Params("PATH") Is Nothing Then
                        lParamlist = lParamlist & "&PATH=" & .Params("PATH")
                    End If

                    If Not String.IsNullOrEmpty(txtFiltertext.Text) Then
                        Dim lFilterstring As String = ""
                        Dim lFieldname As String = mUser.GBOXmanager.GetFieldNameByObjDisplay(mUser.Current_OBJ.OBJ_ID, cmbFieldFilter.Text)
                        lFilterstring = lFieldname
                        lFilterstring &= "|" & txtFiltertext.Text
                        Dim lPathKey As String = "&VALUES=" & lFilterstring
                    End If

                    If Not .Params("VALUES") Is Nothing Then
                        lParamlist = lParamlist & "&VALUES=" & .Params("VALUES")
                    End If
                    '------------------------------------------------------------------------------
                    'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
                    'Comment    : Replace all blanks in GBOX Cockpit URL by %20
                    'Added by   : Pratyusa Lenka (CWID : EOJCG)
                    'Date       : 2016-02-22
                    lParamlist = Regex.Replace(lParamlist, "\s+", "%20")
                    ' Reference END : CR ZHHR 1053558
                    '------------------------------------------------------------------------------
                    Me.Form.Action = lParamlist
                    mUser.GBOXmanager.Paramlist = lParamlist
                End With
                Me.Response.Redirect("~" & Me.Request.FilePath & lParamlist)
            End If
        Else
        End If
    End Sub
    Protected Sub mnuDetailsMenu_MenuItemClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MenuEventArgs) Handles mnuDetailsMenu.MenuItemClick
        Try
            ResetView()

            DetailsMenuSelect(mWithPaging)
            If trvOBJ.Nodes.Count = 0 Then
                grdDat.DataSource = Nothing
                grdDat.DataBind()
            End If

            LoadFactoryAndControllerAndBind(mWithPaging)
        Catch ex As Exception
            mErrText &= "mnuDetailsmenu:MenuItemClick" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        End Try
    End Sub
    Protected Sub mnuNavigate_MenuItemClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MenuEventArgs) Handles mnuNavigate.MenuItemClick
        Try
            ResetView()
            lblStatus.Text = ""
            MakeTopicsMenu(mnuNavigate.SelectedValue)
            LoadWizardData(mnuNavigate.SelectedValue)
        Catch ex As Exception
            mErrText &= "MenuItemClick:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        End Try
    End Sub
    ' Reference         : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
    ' Comment           : Hide the wizard area
    ' Added by          : Pratyusa Lenka (CWID : EOJCG)
    ' Date              : 07-11-2018
    'Protected Sub mnuWizzard_MenuItemClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MenuEventArgs) Handles mnuWizzard.MenuItemClick
    '    Try
    '        mvWizard.ActiveViewIndex = mnuWizzard.SelectedItem.Value - 1
    '    Catch ex As Exception
    '        mErrText &= "mnuWizzard" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
    '        lblStatus.Text = mErrText
    '        mErrText &= ""
    '    End Try
    'End Sub
    'Protected Sub mnSupportNavi_MenuItemClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MenuEventArgs) Handles mnSupportNavi.MenuItemClick
    '    MakeTopicGroupMenu()
    'End Sub
#End Region
#Region "Treeview"
    Protected Sub trvOBJ_SelectedNodeChanged(ByVal sender As Object, ByVal e As EventArgs) Handles trvOBJ.SelectedNodeChanged
        Try

            lblStatus.Text = ""
            '---------------------------------------------------------------------------
            ' Reference : CR ZHHR 1060685 - GBOX REP: OTT 3229 - Reporting authorization
            ' Comment   : Check for report authorization
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-08-12
            If Not mnuNavigate Is Nothing Then
                If mnuNavigate.SelectedItem.Text.ToUpper = "REPORTING" Or mnuDetailsMenu.SelectedItem.Text.ToUpper = "WORKFLOW_REPORTING" Then
                    'For public reports, access should be possible
                    Dim dtDatabaseObjRole As DataTable = mUser.Databasemanager.MakeDataTable("SELECT [ROLE] FROM DATABASE_OBJ WHERE OBJ_ID = '" & trvOBJ.SelectedNode.Value & "'")
                    If Not dtDatabaseObjRole Is Nothing AndAlso dtDatabaseObjRole.Rows.Count > 0 AndAlso Not IsDBNull(dtDatabaseObjRole.Rows(0)("ROLE")) AndAlso Not String.IsNullOrEmpty(dtDatabaseObjRole.Rows(0)("ROLE").ToString) Then
                        Dim strSqlAuthReport As String = "SELECT DOJ.OBJ_ID, DOJ.[ROLE], AST.APPLICATION_PART_ID, AST.APPLICATION_ROLE_ID FROM DATABASE_OBJ DOJ" & _
                                                    " INNER JOIN AUTHORISATION_SET AST ON DOJ.[ROLE] = AST.APPLICATION_ROLE_ID" & _
                                                    " WHERE DOJ.OBJ_ID = '" & trvOBJ.SelectedNode.Value & "' AND AST.APPLICATION_PART_ID = 'REPORTING' AND AST.CW_ID = '" & mUser.CW_ID & "'"
                        Dim dtAuthReport As DataTable = mUser.Databasemanager.MakeDataTable(strSqlAuthReport)
                        If Not dtAuthReport Is Nothing AndAlso dtAuthReport.Rows.Count = 0 Then
                            lblStatus.Text = "Access to report " & trvOBJ.SelectedNode.Value & " not possible. Contact support for access."
                            lblStatus.ForeColor = Drawing.Color.Red
                            lblStatus.Font.Bold = True
                            HideAll()
                            Exit Sub
                        Else
                            lblStatus.Text = ""
                            LoadFactoryAndControllerAndBind(mWithPaging)
                        End If
                    End If
                End If
            End If
            ' Reference END : CR ZHHR 1060685
            '---------------------------------------------------------------------------
            If mvContents.ActiveViewIndex = 1 Or mvContents.ActiveViewIndex = 3 Then
                mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
                '---------------------------------------------------------------------
                ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
                ' Comment   : Placed new Add value button to create new request
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-12-24
                btnAddValue.Enabled = False
                ' Reference END : CR ZHHR 1050708
                '---------------------------------------------------------------------

                '---------------------------------------------------------------------------------------------------
                ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
                ' Comment           : System dependent workflow
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 23-Feb-2016
                btnCopyValue.Enabled = False
                ' Reference End     : ZHHR 1053017


                grdDat.DataSource = Nothing
                grdDat.DataBind()
                ChangeIcons()
            End If

            lblrequestfor.Visible = False
            cmborglevel.Visible = False
            cmborglevelvalue.Visible = False
            If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
            pObjCurrentUsers.CONTEXT = Context
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
            End If
            mUser.Databasemanager.MakeDataTable(m_Topic_Group_Context_ID, mnuNavigate.SelectedItem.Text, mnuDetailsMenu.SelectedItem.Text, trvOBJ.SelectedNode.Text, "MenuNavigation")
            mUser.Current_OBJ = mUser.GBOXmanager.SetCurrentObj(trvOBJ.SelectedNode.Value.Split("(")(0).ToString, mUser)
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1028074 - GBOX COC - filter function not working properly 
            ' Comment           : if tree node is clicked then do not append the param list parameters
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-05-09
            '---------------------------------------------------------------------------------------------------
            Dim lParamlist As String = GetParamList(True)
            Me.Form.Action = lParamlist
            mUser.GBOXmanager.Paramlist = lParamlist

            If trvOBJ.SelectedNode.ChildNodes.Count = 0 Then
                imgDownloadToExcel.Enabled = True
            Else
                imgDownloadToExcel.Enabled = False
            End If
            '---------------------------------------------------------------------------
            ' Reference : CR ZHHR 1060685 - GBOX REP: OTT 3229 - Reporting authorization
            ' Comment   : Check mControllerfactory object
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-08-12
            If mControllerfactory Is Nothing Then
                mControllerfactory = New Dynamic_View_Controller_Factory
            End If
            ' Reference END : CR ZHHR 1060685
            '---------------------------------------------------------------------------
            With mControllerfactory
                .TOPIC_ID = mnuDetailsMenu.SelectedValue
                .TOPIC_GROUP_ID = mnuNavigate.SelectedValue
                If Not Request.Params("CONTEXT") Is Nothing Then
                    m_Topic_Group_Context_ID = Request.Params("CONTEXT").ToString
                End If
                .TOPIC_GROUP_CONTEXT_ID = m_Topic_Group_Context_ID
                .Request = Me.Request
                If trvOBJ.SelectedNode.ChildNodes.Count = 0 Then
                    .GetChilds(trvOBJ.SelectedNode, mnuNavigate.SelectedValue)
                End If
                .IsPostback = Me.IsPostBack
                trvOBJ.SelectedNode.Expanded = True

            End With

            mvContents.Visible = True
            Select Case mUser.Current_OBJ.OBJ_CLASSIFICATION_ID
                Case "CPS_NODE"
                    chkPaging.Visible = False
                    rdFilter.Visible = False
                    lblFilter.Visible = False
                    imgLegende.Visible = True
                    lblLegende.Visible = True
                    mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")
                    FillCPSVIEW(mUser.Current_OBJ.OBJ_ID)
                    imgNewCustomizingObj.Enabled = True
                    imgNewDocumentationObject.Enabled = True
                    If txtCustomizingObjName.Text = "" Then
                        imgEditCustomizingObj.Enabled = False
                        imgEditDocumentation.Enabled = False
                    End If
                    mvContents.Visible = False
                Case "CPS_OBJ_ATTR_OLD"
                    chkPaging.Visible = False
                    rdFilter.Visible = False
                    lblFilter.Visible = False
                    imgNewCustomizingObj.Enabled = False
                    imgNewDocumentationObject.Enabled = False
                    mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")
                    FillCPSVIEW(mUser.Current_OBJ.OBJ_ID, mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                Case "CPS_OBJ_ATTR"
                    chkPaging.Visible = False
                    rdFilter.Visible = False
                    lblFilter.Visible = False
                    imgLegende.Visible = True
                    lblLegende.Visible = True
                    imgNewCustomizingObj.Enabled = False
                    imgNewDocumentationObject.Enabled = False
                    mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")
                    FillCPSVIEW(mUser.Current_OBJ.OBJ_ID)
                    '---------------------------------------------------------------------------
                    ' Reference : CR ZHHR 1060685 - GBOX REP: OTT 3229 - Reporting authorization
                    ' Comment   : Enable/Disable for QUERY object classification id
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2016-08-12
                Case "QUERY"
                    Enable_Disable("trvOBJ", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                    ' Reference END : CR ZHHR 1060685
                    '---------------------------------------------------------------------------
                Case Else
                    imgLegende.Visible = False
                    lblLegende.Visible = False
            End Select
            ChangeIcons()
        Catch ex As Exception
            mErrText &= "SelectedNodeChanged" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        End Try
    End Sub
    Private Sub FillCustomizingOnly()
        Dim lsql As String = ""
        If cboCustomizingObjName.Text = "" Then
            If txtCustomizingObjName.Text <> "" Then
                cboCustomizingObjName.SelectedValue = txtCustomizingObjName.Text
            Else
                Exit Sub
            End If
        End If
        lsql = "Select *"
        lsql &= "from OBJ_CPS "
        lsql &= "where [OBJ_CPS_ID]='" & cboCustomizingObjName.Text & "'"
        Dim custObjDt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        If custObjDt.Rows.Count = 0 Then
            lblStatus.Text = lsql & " is EMPTY"
        End If

        If custObjDt.Rows(0)("Locked").ToString <> "" Then
            lsql = "SELECT Top(1) *"
            lsql &= " FROM [OBJ_CPS_HISTORY] Where OBJ_CPS_ID='" & custObjDt.Rows(0)("OBJ_CPS_ID").ToString & "' order By Obj_Versionnumber desc"
            custObjDt = mUser.Databasemanager.MakeDataTable(lsql)

        End If

        Dim lWikiUrl As String = custObjDt.Rows(0)("WIKI_URL").ToString
        imgWiki.OnClientClick = "javascript:window.open('" & lWikiUrl & "',null, 'toolbar=yes,location=yes, status= yes,menubar=yes, scrollbars=yes,resizable=yes'); void('');"
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable("Select MANDATORY_TYPE from OBJ_CPS_MANDATORY_TYPE")
        cboMandatoryType.DataTextField = "MANDATORY_TYPE"
        cboMandatoryType.DataSource = dt
        Dim Lman As String = custObjDt.Rows(0)("MANDATORY_TYPE").ToString
        If Lman <> "" Then cboMandatoryType.SelectedValue = Lman
        txtCustomizingObjName.Text = custObjDt.Rows(0)("OBJ_CPS_ID").ToString
        txtWikiUrl.Text = lWikiUrl
        txtWikiName.Text = custObjDt.Rows(0)("WIKI_NAME").ToString
        Dim lStr As String = custObjDt.Rows(0)("SOLUTION_TYPE").ToString

        lblVersionnumberCust.Text = custObjDt.Rows(0)("OBJ_VERSIONNUMBER").ToString
        chkALL.Checked = False
        chkSO.Checked = False
        chkFi.Checked = False
        'Full;SD;FI
        If InStr(lStr, "Full") <> 0 Then chkALL.Checked = True
        If InStr(lStr, "SD") <> 0 Then chkSO.Checked = True
        If InStr(lStr, "FI") <> 0 Then chkFi.Checked = True
        cboMandatoryType.DataBind()

    End Sub
    Private Sub FillDocumentationOnly(ByVal lObj_ID As String, Optional ByVal lobJ_Classification As String = "CPS_OBJ_ATTR")
        Dim lsql As String = ""
        Dim lIfNewSQL As String = lsql
        lsql = "Select dbo.OBJ_CPS_ATTR.* "
        lsql &= "from TOPIC_OBJ_OBJS "
        lsql &= "Left join OBJ_CPS_ATTR on [TOPIC_OBJ_OBJS].CHILD_OBJ_ID = OBJ_CPS_ATTR.OBJ_GUID "
        lsql &= "left Join OBJ_Field on OBJ_CPS_ATTR.OBJ_CPS_ID=OBJ_Field.OBJ_ID "
        lsql &= "where [CHILD_OBJ_ID]='" & lObj_ID & "' order By OBJ_Field.Ordinal_Position"
        lIfNewSQL = lsql
        Dim docObjDt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        If docObjDt.Rows.Count = 0 Then
            lblStatus.Text = lsql & " is EMPTY"
        End If
        If docObjDt.Rows(0)("Locked").ToString <> "" Then
            lsql = "SELECT Top(1) *"
            lsql &= " FROM OBJ_CPS_ATTR_HISTORY Where OBJ_GUID='" & lObj_ID & "' order By Obj_Versionnumber desc"
            docObjDt = mUser.Databasemanager.MakeDataTable(lsql)
            imgEditDocumentation.Enabled = False
            imgEditCustomizingObj.Enabled = False
            If docObjDt.Rows.Count = 0 Then
                lsql = lIfNewSQL
                docObjDt = mUser.Databasemanager.MakeDataTable(lsql)
            End If
        Else
            imgEditDocumentation.Enabled = True
            imgEditCustomizingObj.Enabled = True
        End If
        MakeDataTableEditMask(tblDocumentation, docObjDt, lobJ_Classification)
        ChangeIcons()
    End Sub
    Private Sub FillCPSVIEW(ByVal lObj_ID As String, Optional ByVal lobJ_Classification As String = "CPS_OBJ_ATTR")

        mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")
        imgSubmitCustomizingObj.Enabled = False
        imgSubmitDocumentation.Enabled = False
        imgNewCustomizingObj.Enabled = True
        imgNewDocumentationObject.Enabled = True
        IsLocked()
        Dim lsql As String = ""
        lsql = "Select OBJ_CPS_ATTR.OBJ_CPS_ID, OBJ_CPS.* "
        lsql &= "from TOPIC_OBJ_OBJS "
        lsql &= "Left join OBJ_CPS_ATTR on [TOPIC_OBJ_OBJS].CHILD_OBJ_ID = OBJ_CPS_ATTR.OBJ_GUID "
        lsql &= "left join OBJ_CPS on OBJ_CPS.OBJ_CPS_ID= OBJ_CPS_ATTR.OBJ_CPS_ID "
        lsql &= "where [CHILD_OBJ_ID]='" & lObj_ID & "'"
        Dim lIfNewSQL As String = lsql
        Dim custObjDt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        If custObjDt.Rows.Count = 0 Then
            lblStatus.Text = lsql & " is EMPTY"
        End If

        If custObjDt.Rows(0)("Locked").ToString <> "" Then
            lsql = "SELECT Top(1) *"
            lsql &= " FROM [OBJ_CPS_HISTORY] Where OBJ_CPS_ID='" & custObjDt.Rows(0)("OBJ_CPS_ID").ToString & "' order By Obj_Versionnumber desc"
            custObjDt = mUser.Databasemanager.MakeDataTable(lsql)
            If custObjDt.Rows.Count = 0 Then
                lsql = lIfNewSQL
                custObjDt = mUser.Databasemanager.MakeDataTable(lsql)
            End If
        End If

        Dim lWikiUrl As String = custObjDt.Rows(0)("WIKI_URL").ToString
        imgWiki.OnClientClick = "javascript:window.open('" & lWikiUrl & "',null, 'toolbar=yes,location=yes, status= yes,menubar=yes, scrollbars=yes,resizable=yes'); void('');"
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable("Select MANDATORY_TYPE from OBJ_CPS_MANDATORY_TYPE")
        cboMandatoryType.DataTextField = "MANDATORY_TYPE"
        cboMandatoryType.DataSource = dt
        Dim Lman As String = custObjDt.Rows(0)("MANDATORY_TYPE").ToString
        If Lman <> "" Then cboMandatoryType.SelectedValue = Lman
        txtCustomizingObjName.Text = custObjDt.Rows(0)("OBJ_CPS_ID").ToString
        txtWikiUrl.Text = lWikiUrl
        txtWikiName.Text = custObjDt.Rows(0)("WIKI_NAME").ToString
        Dim lStr As String = custObjDt.Rows(0)("SOLUTION_TYPE").ToString

        lblVersionnumberCust.Text = custObjDt.Rows(0)("OBJ_VERSIONNUMBER").ToString
        chkALL.Checked = False
        chkSO.Checked = False
        chkFi.Checked = False
        'Full;SD;FI
        If InStr(lStr, "Full") <> 0 Then chkALL.Checked = True
        If InStr(lStr, "SD") <> 0 Then chkSO.Checked = True
        If InStr(lStr, "FI") <> 0 Then chkFi.Checked = True
        cboMandatoryType.DataBind()

        lsql = "Select dbo.OBJ_CPS_ATTR.* "
        lsql &= "from TOPIC_OBJ_OBJS "
        lsql &= "Left join OBJ_CPS_ATTR on [TOPIC_OBJ_OBJS].CHILD_OBJ_ID = OBJ_CPS_ATTR.OBJ_GUID "
        lsql &= "left Join OBJ_Field on OBJ_CPS_ATTR.OBJ_CPS_ID=OBJ_Field.OBJ_ID "
        lsql &= "where [CHILD_OBJ_ID]='" & lObj_ID & "' order By OBJ_Field.Ordinal_Position"
        lIfNewSQL = lsql
        Dim docObjDt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        If docObjDt.Rows.Count = 0 Then
            lblStatus.Text = lsql & " is EMPTY"
        End If
        If docObjDt.Rows(0)("Locked").ToString <> "" Then
            lsql = "SELECT Top(1) *"
            lsql &= " FROM OBJ_CPS_ATTR_HISTORY Where OBJ_GUID='" & lObj_ID & "' order By Obj_Versionnumber desc"
            docObjDt = mUser.Databasemanager.MakeDataTable(lsql)
            imgEditDocumentation.Enabled = False
            imgEditCustomizingObj.Enabled = False
            If docObjDt.Rows.Count = 0 Then
                lsql = lIfNewSQL
                docObjDt = mUser.Databasemanager.MakeDataTable(lsql)
            End If
        Else
            imgEditDocumentation.Enabled = True
            imgEditCustomizingObj.Enabled = True
        End If
        MakeDataTableEditMask(tblDocumentation, docObjDt, lobJ_Classification)
        ChangeIcons()
    End Sub
    'Sub FillCPSVIEW(ByVal lObj_ID As String, Optional ByVal lobJ_Classification As String = "CPS_OBJ_ATTR")
    '    FillCustomizingOnly()
    '    FillDocumentationOnly(lObj_ID, lobJ_Classification)
    'End Sub
    Sub MakeDataTableEditMask(ByRef lTbl As Table, ByVal mDt As DataTable, ByVal lobJ_Classification As String, Optional ByVal lStartRownumber As Long = 0)

        Try
            lTbl.Rows.Clear()
            lstCurrentDocControls.Items.Clear()
            For Each cData As DataColumn In mDt.Columns
                Dim lTablerow As New TableRow
                'CODE
                Dim lTableCell As New TableCell
                Dim ctl As Label = New Label
                ctl.ID = "lbl_" & cData.ColumnName
                Dim lTemplate As String = mUser.GBOXmanager.GetClassificationTemplateObject(mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                ctl.Text = mUser.GBOXmanager.GetDisplayNameByObjFieldId(lTemplate, cData.ColumnName)
                If ctl.Text <> "" Then
                    ctl.Width = 300
                    lTableCell = New TableCell
                    lTableCell.CssClass = "mc-dfrm-EditTextsView-tblcellLeft"
                    ctl.CssClass = "mc-dfrm-EditTextsView-Label"
                    lTableCell.Controls.Add(ctl)
                    lTablerow.Cells.Add(lTableCell)
                    Dim lFieldType As String = mUser.GBOXmanager.GetFieldType(lTemplate, cData.ColumnName)

                    Dim ctlToAdd As Control = Nothing
                    Select Case lFieldType

                        Case "INPUT"
                            'TextBox 
                            Dim ctlText As TextBox = New TextBox
                            With ctlText
                                .ID = "txt_" & cData.ColumnName

                                If ctl.Text <> "" Then lstCurrentDocControls.Items.Add(ctlText.ID)
                                .Text = mDt.Rows(0)(cData.ColumnName).ToString
                                .Width = 400
                                .Enabled = False
                            End With
                            ctlToAdd = ctlText

                        Case "LOOKUP"
                            'ComboBox
                            Dim ctlDropDown As New DropDownList
                            With ctlDropDown
                                .ID = "txt_" & cData.ColumnName
                                If ctl.Text <> "" Then lstCurrentDocControls.Items.Add(ctlDropDown.ID)

                                Dim lsqlLookup As String = "Select * FROM OBJ_FIELD_LOOKUP_VALIDATON Where OBJ_ID ='" & lTemplate & "' AND OBJ_FIELD_ID='" & cData.ColumnName & "'"
                                Dim dtLookUp As DataTable = mUser.Databasemanager.MakeDataTable(lsqlLookup)

                                Dim lTable As String = dtLookUp.Rows(0)("Lookup_Table_Name").ToString
                                Dim lFilter As String = dtLookUp.Rows(0)("Lookup_Table_Filter").ToString
                                Dim lKey As String = dtLookUp.Rows(0)("Lookup_Table_Key").ToString
                                Dim lsql As String = "Select " & lKey & "   from " & lTable & " " & lFilter & " Order by " & lKey
                                .Enabled = False
                                .DataSource = mUser.Databasemanager.MakeDataTable(lsql)
                                .DataTextField = lKey
                                .DataValueField = lKey
                                .BackColor = Drawing.Color.Gold
                                .Text = mDt.Rows(0)(cData.ColumnName).ToString
                                If .Text = "" Then
                                    .SelectedValue = "None"
                                    .Text = "None"
                                End If
                                .DataBind()


                                .SelectedValue = mDt.Rows(0)(cData.ColumnName).ToString
                            End With
                            ctlToAdd = ctlDropDown
                    End Select


                    lTableCell = New TableCell
                    lTableCell.CssClass = "mc-dfrm-EditTextsView-tblcellRight"
                    ctl.CssClass = "mc-dfrm-EditTextsView-TB"
                    lTableCell.Controls.Add(ctlToAdd)
                    lTablerow.Cells.Add(lTableCell)
                    If ctl.Text <> "" Then lTbl.Rows.Add(lTablerow)
                    lStartRownumber = lStartRownumber + 1
                End If
            Next
        Catch ex As Exception
            lblStatus.Text = "MAKETABLEROW:" & ex.Message

        End Try
    End Sub

#End Region
#Region "ImageButtons"
    Protected Sub imgAppend_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgAppend.Click
        Dim lParamlist As String = GetParamList()
        'START Filter
        Me.Form.Action = lParamlist
        mUser.GBOXmanager.Paramlist = lParamlist
        Me.Response.Redirect("~" & Me.Request.FilePath & lParamlist)
    End Sub
    Protected Sub imgRelease_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgRelease.Click

        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR - BY-RZ04-CMT-27979 - GBOX forms: clear GBOX Cockpit filter
        ' Comment           : Moved the code to common function ReleaseFilter
        ' Added by          : Surendra Purav (CWID : EQIZU)
        ' Date              : 2013-10-22
        '---------------------------------------------------------------------------------------------------
        ReleaseFilter() 'Release the filter from a common function

    End Sub
    Protected Sub lnkLogOut_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkLogOut.Click

        pObjCurrentUsers.RemoveUser(My.User.Name)
        lblInformations.Text = My.User.Name & " has been logged out successfully"
        lblStatus.Text = My.User.Name & " has been logged out successfully. Click on any object to logon again."
        BackToMainAndReset(sender, e)
    End Sub

    Protected Sub imgEditDocumentation_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgEditDocumentation.Click

        Dim lLockedSql As String = "Select Locked from OBJ_CPS_ATTR where [OBJ_GUID]='" & mUser.Current_OBJ.OBJ_ID & "' AND Locked is not null"
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lLockedSql)
        If ldt.Rows.Count <> 0 Then
            lblStatus.Text = "The Request is locked by request id: " & ldt.Rows(0)("LOCKED").ToString
            lblStatus.ForeColor = Drawing.Color.Red
            lblStatus.Font.Bold = True
            Exit Sub
        End If

        imgEditDocumentation.Enabled = False
        imgSubmitDocumentation.Enabled = True
        EnableDocumentationFields(True)
        EnableCustomizingObj(False)
        imgEditCustomizingObj.Enabled = False
        tblNewNode.Visible = False
        ChangeIcons()
    End Sub

    Protected Sub imgQuery_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgQuery.Click
        Try
            imgDownloadToExcel.Visible = True
            imgDownloadToExcel.Enabled = True
            ChangeIcons()
        Catch ex As Exception
            mErrText &= ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        End Try
    End Sub
    Sub BackToMainAndReset(sender, e)
        mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
        ''Below method is called to clean uploaded files in DRS request
        '' in previous session
        clearAttachedFilesToRequest()
    End Sub
    Protected Sub imgCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgCancel.Click
        BackToMainAndReset(sender, e)
    End Sub

    Protected Sub imgDownloadToExcel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgDownloadToExcel.Click
        Try
            If mUser Is Nothing Then
                imgDownloadToExcel.Enabled = False
                ChangeIcons()
                Exit Sub
            End If
            If mUser.Current_OBJ Is Nothing Then
                imgDownloadToExcel.Enabled = False
                ChangeIcons()
                Exit Sub
            End If
            Dim lResponse As Boolean = False
            Dim strFilename As String = "GBOX_" & DateTime.Now.ToLongTimeString.Replace(":", "_") & mUser.Current_OBJ.OBJ_ID.ToString
            'If Not mUser.CURRENT_OBJ__TYPE Is Nothing And mUser.CURRENT_OBJ__TYPE <> "QUERY" Then pSQL = pSQL.Split("Where")(0)
            '---------------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
            ' Comment   : New concept for multiple text functionality
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-03-24
            '--------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
            ' Comment   : Delete COMPOSITE_S_T object type
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-01-25
            If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "SINGLE" _
                Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" _
                Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Then
                '---------------------------------------------------------------------------------------------------
                ' Reference         : CR - 1026258 - GBOX COC: Download der T880 aus dem DRS-Handbuch 
                ' Comment           : Added conditon to check if query has WHERE or not
                ' Added by          : Surendra Purav (CWID : EQIZU)
                ' Date              : 2014-03-25
                '---------------------------------------------------------------------------------------------------
                If pVarSQL.ToUpper.Contains(" WHERE ") Then
                    ' Reference : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
                    ' Comment   : String split not working, hence giving error while Excel download 
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2018-10-24
                    pVarSQL = Regex.Split(pVarSQL.ToUpper, " WHERE ")(0)
                End If
            End If
            ' Reference END : CR ZHHR 1052471
            '--------------------------------------------------------------------------------
            '---------------------------------------------------------------------------------------------------
            ' Reference         : BY-RZ04-CMT-28967 - 110IM08315494 - Problem with XML download 
            ' Comment           : Added conditon check i query if it is a Customizing object and don't contain order by clause
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2013-11-29
            '---------------------------------------------------------------------------------------------------

            If InStr(pVarSQL.ToUpper, "where".ToUpper) = 0 And pVarSQL.Contains("CUSTOMIZING_") And Not pVarSQL.Contains("ORDER BY") Then
                pVarSQL = pVarSQL & " where active = 1"
            End If

            pVarSQL = pVarSQL.ToUpper
            pVarSQL = pVarSQL.Replace("TOP (800)", "TOP (10000)")
            pVarSQL = pVarSQL.Replace("AND ROWNUM <= 800", "AND ROWNUM <= 10000")
            '------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1060460 - GBOX REP: GBOX reporting problem
            ' Comment   : When no parameter value, consider 10.000 records for xml download
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-08-03
            pVarSQL = pVarSQL.Replace("WHERE ROWNUM <= 800", "WHERE ROWNUM <= 10000")
            ' Reference END : CR ZHHR 1060460
            '------------------------------------------------------------------------------
            If InStr(pVarSQL, "ROWNUM") <> 0 Or InStr(pVarSQL, "TOP") <> 0 Then
                mUser.GBOXmanager.Makereport(Response, strFilename, pVarSQL, Server, lResponse)
                If lResponse = True Then

                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
                    ' Comment           : Remove title from database and code
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 2014-12-15


                    Dim lMailtext As String = "Dear " & mUser.first_name & " " & mUser.last_name & ","
                    lMailtext = lMailtext & "the report you started in GBOX-Cockpit delivered too many results." & vbCrLf
                    lMailtext = lMailtext & "The result is limited to 10.000 records. Please refine you search criteria"
                    lblStatus.Text = lMailtext

                    ' Reference  END    : CR ZHHR 1035817

                End If
            Else
                mUser.GBOXmanager.Makereport(Response, strFilename, pVarSQL, Server, lResponse)
            End If
            Console.WriteLine("Download button click")
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Console.WriteLine(ex.StackTrace)
            lblStatus.Text = ex.Message
        End Try
    End Sub
    Protected Sub imgAlert_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgAlert.Click
        If trvOBJ.SelectedNode Is Nothing Then Exit Sub
        Dim lPackage As New List(Of String)
        Dim lSql As String = ""
        lSql &= "INSERT INTO OBJ_SUBSCRIPTION "
        lSql &= "  ([OBJ_ID]"
        lSql &= "  ,[CW_ID])"
        lSql &= " VALUES "
        lSql &= "  ('" & trvOBJ.SelectedNode.Value & "',"
        lSql &= "  '" & mUser.CW_ID & "')"
        lPackage.Add(lSql)
        If mUser.Databasemanager.ExecutePackage(lPackage) Then
            lblStatus.Text = "subscription for " & trvOBJ.SelectedNode.Value & " has been created."
        Else
            lblStatus.Text = "subscription for " & trvOBJ.SelectedNode.Value & " is already available." '& vbCrLf & mUser.Databasemanager.ErrText
        End If
        trvOBJ.SelectedNode.ImageUrl = "~/Images/bell.png"
    End Sub
    Private Sub imgmySubscribtions_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgmySubscriptions.Click
        lblStatus.Text = ""
        mvContents.ActiveViewIndex = GetIndexByViewName("vwSubscribe")
        chkSubscriptions.Items.Clear()
        Dim lsql As String = "Select OBJ_ID from OBJ_SUBSCRIPTION where CW_ID ='" & mUser.CW_ID & "'"
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        For Each r As DataRow In ldt.Rows
            chkSubscriptions.Items.Add(r("OBJ_ID").ToString)
        Next
        If ldt.Rows.Count = 0 Then
            chkSubscriptions.Items.Add("No Subscriptions found.")
        End If
        mnuNavigate.Enabled = False
        mnuDetailsMenu.Enabled = False
        trvOBJ.Enabled = False
    End Sub
    Protected Sub imgCancel_Subscription_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgCancel_Subscription.Click
        mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
        trvOBJ_SelectedNodeChanged(sender, e)
    End Sub
    Protected Sub imgSubmit_Cancel_Subscribtion_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgSubmit_Cancel_Subscribtion.Click
        Dim lPackage As New List(Of String)
        Dim lsql As String = ""
        lblStatus.Text = ""
        For Each r As ListItem In chkSubscriptions.Items
            If r.Selected = True Then
                lsql = "Delete from  OBJ_SUBSCRIPTION where obj_ID ='" & r.Text & "' and cw_ID='" & mUser.CW_ID & "'"
                lPackage.Add(lsql)
                For Each lNode As TreeNode In trvOBJ.Nodes
                    If lNode.Text = r.Text.ToString Then
                        lNode.ImageUrl = "~/Images/application_s.gif"
                    End If
                    lookForChildnodes(lNode, r.Text)
                Next
            End If
        Next
        If mUser.Databasemanager.ExecutePackage(lPackage) Then
            lblStatus.Text = "Delition of subscribtions succeeded"
        Else
            lblStatus.Text = mUser.Databasemanager.ErrText
        End If
        chkSubscriptions.Items.Clear()
        mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
        mnuNavigate.Enabled = True
        mnuDetailsMenu.Enabled = True
        trvOBJ.Enabled = True
        imgmySubscriptions.Enabled = True
    End Sub
    Protected Sub imgEngine_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgEngine.Click
        Search()
        mvContents.ActiveViewIndex = GetIndexByViewName("vwSearchEngine")
    End Sub
    Protected Sub imgSearchEngine_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgSearchEngine.Click
        lblLegende.Visible = False
        mvContents.Visible = True
        mvContents.ActiveViewIndex = GetIndexByViewName("vwSearchEngine")
        trvOBJ.Nodes.Clear()
        'imgEngine.Focus()
        ChangeIcons()
    End Sub
    'Sub EnDisableSearch(ByVal lStat As Boolean)
    '    imgmySubscriptions.Enabled = lStat
    '    imgAlert.Enabled = lStat
    '    imgCancel.Enabled = lStat
    '    'imgEditTexts.Enabled = lStat
    '    imgEngine.Focus()
    '    imgSearchEngine.Enabled = lStat
    '    imgHelp.Enabled = lStat
    '    mnuNavigate.Enabled = lStat
    '    imgNew.Enabled = lStat
    '    imgRefresh.Enabled = lStat
    '    mnuDetailsMenu.Enabled = lStat
    'End Sub
    'Protected Sub imgSearchCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgSearchCancel.Click
    '    trvOBJ_SelectedNodeChanged(sender, e)
    'End Sub
#End Region
#Region "AdminButtons"
    Protected Sub cmdImpersonate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdImpersonate.Click
        Try

        Catch ex As Exception
            mErrText &= ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        End Try
    End Sub



#End Region
#Region "OtherControls"
    Protected Sub cmborglevel_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmborglevel.SelectedIndexChanged
        FillCombo(cmborglevelvalue, "ORG_LEVEL_VALUE", " AND ORG_LEVEL_ID = '" & cmborglevel.SelectedValue & "'")
        mvContents.ActiveViewIndex = GetIndexByViewName("vwEditSysthems")
        ChangeIcons()
        '----------------------------------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
        ' Comment   : New Add value button to create new request and enable/disable ribbon controls for cmborglevel
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2015-12-24
        btnAddValue.Enabled = False

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : System dependent workflow
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 23-Feb-2016
        btnCopyValue.Enabled = False
        ' Reference End     : ZHHR 1053017

        EnableDisableRibbonControls("cmborglevel", "")
        ' Reference END : CR ZHHR 1050708
        '----------------------------------------------------------------------------------------------------------
    End Sub
    '---------------------------------------------------------------------------------------------------
    ' Reference         : ZHHR 1045115 - GBOX COC OTT 1070: GBOX 3.0 Multi-system implementation (pMDAS list)
    ' Comment           : GBOX 3.0 Multi-system implementation (pMDAS list)
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 2015-07-13
    Protected Sub cmborglevelValue_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmborglevelvalue.SelectedIndexChanged
        Dim dt As DataTable
        ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
        ' Comment   : For add and change value, display all the systems, but when copy display only systems which are not available
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2018-04-12
        Dim lSysSql As String = ""
        If lblRequestType.Text = "" Then
            lSysSql = " SELECT APP.APPLICATION_ID,APP.Description, OBJ.SUBGROUP_DEPENDENT, OBJ.EDITABLE, OBJ.DEFAULT_TRUE, OBJ.ORG_LEVEL_ID, OBJ.ORG_LEVEL_VALUE   FROM OBJ_APPLICATION OBJ " & _
                                " INNER JOIN APPLICATION APP ON OBJ.APPLICATION_ID = APP.APPLICATION_ID  " & _
                                " WHERE OBJ.OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID.ToString & "'"
        ElseIf lblRequestType.Text = "Copy" Then
            lSysSql = " SELECT APP.APPLICATION_ID,APP.Description, OBJ.SUBGROUP_DEPENDENT, OBJ.EDITABLE, OBJ.DEFAULT_TRUE, OBJ.ORG_LEVEL_ID, OBJ.ORG_LEVEL_VALUE   FROM OBJ_APPLICATION OBJ " & _
                                " INNER JOIN APPLICATION APP ON OBJ.APPLICATION_ID = APP.APPLICATION_ID  " & _
                                " WHERE OBJ.OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID.ToString & "' AND OBJ.APPLICATION_ID NOT IN " & _
                                " (SELECT APPLICATION_ID FROM " & mUser.Current_OBJ.OBJ_TABLENAME & " WHERE " & mUser.GBOXmanager.GetTableKey(mUser.Current_OBJ.OBJ_ID.ToString) & " =" & _
                                " '" & lblSysObj_VALUE.Text & "') "
        End If
        dt = mUser.Databasemanager.MakeDataTable(lSysSql)


        If (dt.Rows.Count = 0) Then lblchkSYSTEMSCaption.Visible = False Else lblchkSYSTEMSCaption.Visible = True

        chkSYSTEMS.Items.Clear()

        For Each r As DataRow In dt.Rows
            Dim lItem As New ListItem
            lItem.Value = r("APPLICATION_ID").ToString & " (" & r("Description").ToString & ")"
            If (r("SUBGROUP_DEPENDENT").ToString) Then
                If (r("ORG_LEVEL_VALUE").ToString = cmborglevelvalue.SelectedValue.ToString) Then
                    If (r("EDITABLE").ToString) Then
                        If (r("DEFAULT_TRUE").ToString) Then
                            lItem.Selected = True
                            lItem.Enabled = True
                        Else
                            lItem.Selected = False
                            lItem.Enabled = True
                        End If
                    Else
                        If (r("DEFAULT_TRUE").ToString) Then
                            lItem.Selected = True
                            lItem.Enabled = False
                        Else
                            lItem.Selected = False
                            lItem.Enabled = False
                        End If
                    End If
                Else
                    lItem.Selected = False
                    lItem.Enabled = False
                End If
            Else
                If (r("EDITABLE").ToString) Then
                    If (r("DEFAULT_TRUE").ToString) Then
                        lItem.Selected = True
                        lItem.Enabled = True
                    Else
                        lItem.Selected = False
                        lItem.Enabled = True
                    End If
                Else
                    If (r("DEFAULT_TRUE").ToString) Then
                        lItem.Selected = True
                        lItem.Enabled = False
                    Else
                        lItem.Selected = False
                        lItem.Enabled = False
                    End If
                End If
            End If
            chkSYSTEMS.Items.Add(lItem)
        Next

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : System dependent workflow
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 23-Feb-2016

        Dim lSysStatus As Boolean = mUser.GBOXmanager.GetSysDependentInfo(mUser.Current_OBJ.OBJ_ID.ToString)

        If (lSysStatus) Then
            Dim mApp_Id As String = ""
            If (lstValues.Items.Count > 5) Then
                For iCnt = 0 To lstFields.Items.Count - 1
                    If (lstFields.Items(iCnt).ToString = "APPLICATION_ID") Then
                        mApp_Id = lstValues.Items(iCnt).ToString
                        Exit For
                    End If
                Next iCnt
            End If

            If mUser.SystemIds <> "" Then
                mApp_Id = mUser.SystemIds
                mUser.SystemIds = ""
            End If

            If mApp_Id.Trim <> "" Then
                For Each li As ListItem In chkSYSTEMS.Items
                    If li.Value.Contains(mApp_Id.Trim) Then
                        li.Selected = True
                        'The already exist system should also be editable for de-selection for copy functionality
                        If mUser.RequestType = myUser.RequestTypeOption.update Then
                            li.Enabled = False
                        End If
                    Else
                        If mUser.RequestType = myUser.RequestTypeOption.update Then
                            'default selection of system which is selected from the 1st screen, not from OBJ_APPLICATION while update system dependent customizing object and disabled
                            li.Selected = False
                            li.Enabled = False
                        End If
                    End If
                Next
            End If
        End If
        ' Reference End     : ZHHR 1053017

        chkSYSTEMS.Visible = True
        mvContents.ActiveViewIndex = GetIndexByViewName("vwEditSysthems")
        '---------------------------------------------------------------------------
        ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
        ' Comment   : Placed new Add value button and enable/disable ribbon controls
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2015-12-24
        btnAddValue.Enabled = False

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : System dependent workflow
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 23-Feb-2016
        btnCopyValue.Enabled = False
        ' Reference End     : ZHHR 1053017

        ChangeIcons()
        EnableDisableRibbonControls("cmborglevelValue", "")
        ' Reference END : CR ZHHR 1050708
        '---------------------------------------------------------------------------
    End Sub
    ' Reference End     : ZHHR 1045115

    Private Sub grdDat_SelectedIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSelectEventArgs) Handles grdDat.SelectedIndexChanging
        grdDat.SelectedIndex = e.NewSelectedIndex
    End Sub
#End Region
#Region "Form"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim lSysStatus As Boolean

            If Not InitMe() Then Exit Sub
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1027842 - GBOX COC: new context needed 
            ' Comment           : Check if User is having KPI Role while accessing KPI context
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-05-21
            '---------------------------------------------------------------------------------------------------
            If Not Request.Params("CONTEXT") Is Nothing Then
                If Request.Params("CONTEXT").ToUpper = "KPI" Then
                    '---------------------------------------------------------------------------
                    ' Reference : CR ZHHR 1060685 - GBOX REP: OTT 3229 - Reporting authorization
                    ' Comment   : Redirect when context is KPI
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2016-08-12
                    Me.Response.Redirect("~" & Me.Request.FilePath & "?CONTEXT=general&TOPICGROUP=Reporting&TOPIC=Workflows&PATH=KPI")
                    ' Reference END : CR ZHHR 1060685
                    '---------------------------------------------------------------------------
                End If
            End If

            Dim lBuildTexts As Boolean = False
            Dim lStatus As ViewStatus = ViewStatus.NodeClick
            Dim lCriteriaTarget As String = ""
            '---------------------------------------------------------------------
            ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
            ' Comment   : Display search functionality for initial load of cockpit
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-12-24
            If Not Me.Request.Form("__EVENTTARGET") Is Nothing Then
                lCriteriaTarget = Me.Request.Form("__EVENTTARGET").ToString
            ElseIf Request.Params("CONTEXT") Is Nothing Then
                lCriteriaTarget = "imgSearchEngine"
            ElseIf Not Request.Params("CONTEXT") Is Nothing And Request.Params("CONTEXT").ToString.ToUpper = "GENERAL" And Request.Params("TOPICGROUP").ToString.ToUpper = "DRS HANDBOOK" And Request.Params("TOPIC") Is Nothing Then
                lCriteriaTarget = "imgSearchEngine"
            End If
            If lCriteriaTarget = "" Then lCriteriaTarget = GetClickedControl()
            If IsPostBack Then
                If lCriteriaTarget = "" Then
                    lCriteriaTarget = hidSourceID.Value
                End If
            End If
            ' Reference END : CR ZHHR 1050708
            '---------------------------------------------------------------------

            Select Case lCriteriaTarget
                Case "chkALL", "chkFi", "chkSO", "imgNewDocumentationObject"
                    mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")
                    imgEditCustomizingObj.Enabled = False
                    FillDocumentationOnly(mUser.Current_OBJ.OBJ_ID, mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                    imgNewDocumentationObject.Enabled = False
                    Exit Sub
                Case "imgCancel_Subscription"
                    Exit Sub
                Case "imgRefresh", "imgHotline"
                    DetailsMenuSelect(mWithPaging)
                    trvOBJ_SelectedNodeChanged(sender, e)
                Case "imgSearchCancel"
                    lCriteriaTarget = "trvOBJ"
                Case "CANCEL"
                    HideAll()
                    If lCriteriaTarget = "" Then Exit Sub
                    Enable_Disable(lCriteriaTarget, "NONE")
                    Exit Sub
                Case "mnuDocTab"
                    BindDocumentation()
                    Exit Sub
                Case "cboCustomizingObjName"
                    ShowEditNewDocumentationObject()
                    Exit Sub
                Case "chkPaging"
                    lStatus = ViewStatus.NodeClick
                Case "mnuNavigate"
                    lStatus = OverrideStatusDueUrlParams(ViewStatus.MainMenu)
                Case "mnuDetailsMenu"
                    lStatus = OverrideStatusDueUrlParams(ViewStatus.Detailsmenu)

                Case "mnuWizzard"
                    lStatus = ViewStatus.WizzardMenu
                    LoadFactoryAndControllerAndBind(False)
                    Exit Sub
                Case "trvOBJ"
                    Dim lCheckString As String = Me.Request.Form("__EVENTARGUMENT").ToString
                    If Left(lCheckString, 1) = "t" Then
                        lStatus = ViewStatus.TreeClick
                    End If
                    If Left(lCheckString, 1) = "s" Then
                        lStatus = ViewStatus.NodeClick
                    End If
                    isObjChanged = True    'CRT-2066302 : For selecting Default All in filter functionality--by Kanchan Bhor
                    rdFilter.Items.FindByText("All").Selected = True  'CRT-2066302 : For selecting Default All in filter functionality--by Kanchan Bhor


                Case "grdDat"
                    lStatus = ViewStatus.SelectLink
                    'lStatus = ViewStatus.GridCheckBoxClick
                    '---------------------------------------------------------------------------------------------------
                    ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
                    ' Comment   : Changed image Save to Submit button and image edit systems to next edit systems button
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2015-12-24
                Case "cmborglevel"
                    'lStatus = ViewStatus.ImageClick
                    'lCriteriaTarget = "'imgEditSystems"
                    lStatus = ViewStatus.ButtonClick
                    lCriteriaTarget = "btnNextEditSystems"
                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : ZHHR 1045115 - GBOX COC OTT 1070: GBOX 3.0 Multi-system implementation (pMDAS list)
                    ' Comment           : Commented Below code for this implementation
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 2015-07-14
                Case "cmborglevelvalue"
                    'lStatus = ViewStatus.ImageClick
                    'lCriteriaTarget = "'imgEditSystems"
                    lStatus = ViewStatus.ButtonClick
                    lCriteriaTarget = "btnNextEditSystems"
                    ' Reference End     : ZHHR 1045115
                Case "btnSubmit"
                    ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
                    ' Comment   : Default focus should be on <Submit> button
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2019-09-26
                    btnSubmit.Focus()
                    If InStr(cmborglevel.Text.ToUpper, "Please".ToUpper) <> 0 Then
                        lblStatus.Text = "Choose Orglevel for the request (Dropdown Boxes)"
                        Exit Sub
                    End If
                    If InStr(cmborglevelvalue.Text.ToUpper, "Please".ToUpper) <> 0 Then
                        lblStatus.Text = "Choose OrglevelValue for the request (Dropdown Boxes)"
                        Exit Sub
                    End If
                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
                    ' Comment           : Check if the System dependent work flow is true for this object
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 22-Feb-2016
                    ' Modified by       : Pratyusa Lenka(EOJCG)
                    ' Comment           : System selection is not mandatory for non system dependent workflow objects
                    ' Date              : 17-Aug-2016
                    lSysStatus = mUser.GBOXmanager.GetSysDependentInfo(mUser.Current_OBJ.OBJ_ID.ToString)

                    Dim iCntx As Integer = 0
                    Dim lSysCheck As Boolean = False
                    Dim lCheckEntry As Boolean = False
                    Dim _lstObjKeyFields As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))
                    If (lSysStatus = False) Then
                        ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                        ' Comment   : Checks the values exist for non system dependent cust objects
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                        ' Date      : 2018-10-17
                        'Dim _lstObjKeyFields As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))
                        Dim lWherestring As String = ""
                        Dim strSys As String = ""
                        ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729/JIRA GBOX 196: SUP - Problem with CUSTOMIZING_APPLICATION KOKRS
                        ' Comment   : IsAtleastOneSystemSelected - Proceed without any message in case of no system is selected
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                        ' Date      : 2019-02-08
                        If mUser.RequestType = myUser.RequestTypeOption.insert AndAlso IsAtleastOneSystemSelected() Then
                            If Not Session("ObjKeyFields") Is Nothing Then
                                _lstObjKeyFields = Session("ObjKeyFields")
                                For Each pair As KeyValuePair(Of String, String) In _lstObjKeyFields
                                    If pair.Key = "APPLICATION_ID" Then
                                        For iCntx = 0 To chkSYSTEMS.Items.Count - 1
                                            If (chkSYSTEMS.Items(iCntx).Selected = True) Then
                                                If (chkSYSTEMS.Items(iCntx).Text <> "") Then
                                                    If (strSys.Trim = "") Then
                                                        strSys = "'" & chkSYSTEMS.Items(iCntx).Text.Split("(")(0).Trim & "'"
                                                    Else
                                                        strSys += ",'" & chkSYSTEMS.Items(iCntx).Text.Split("(")(0).Trim & "'"
                                                    End If
                                                End If
                                            End If
                                        Next
                                        lWherestring = lWherestring & LTrimRTrim(pair.Key) & " in ( " & LTrimRTrim(strSys) & " ) and "
                                    Else
                                        lWherestring = lWherestring & pair.Key & "='" & pair.Value & "' and "
                                    End If

                                Next
                                lWherestring = lWherestring.Substring(0, lWherestring.Length - 4)
                                If mUser.GBOXmanager.CheckEntryExist(mUser.Current_OBJ.OBJ_ID.ToString, lWherestring) Then
                                    lblStatus.Text = "Key Value already exists. To change it select the value in the table and cancel here."
                                    Exit Sub
                                End If
                            End If
                        End If
                    Else

                        If mUser.RequestType = myUser.RequestTypeOption.insert Then

                            For iCntx = 0 To chkSYSTEMS.Items.Count - 1
                                If (chkSYSTEMS.Items(iCntx).Selected = True) Then
                                    lSysCheck = True
                                    lCheckEntry = mUser.GBOXmanager.CheckEntryExist(mUser.Current_OBJ.OBJ_ID.ToString, chkSYSTEMS.Items(iCntx).Text.Split("(")(0).Trim, lblSysObj_VALUE.Text)

                                    If (lCheckEntry) Then
                                        lblStatus.Text = "The entry for " & mUser.GBOXmanager.GetTableKey(mUser.Current_OBJ.OBJ_ID.ToString) & " | " & chkSYSTEMS.Items(iCntx).Text.Split("(")(0).Trim & " | " & lblSysObj_VALUE.Text & " is already available. Please create a change request for this entry."
                                        Exit Sub
                                    End If
                                End If
                            Next
                            If lSysCheck = False Then
                                lblStatus.Text = "Please choose atleast one system."
                                Exit Sub
                            End If
                            ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729/JIRA GBOX 501: Edit functionality for non-availability of APPLICATION_ID
                            ' Comment   : Display the message in case the APPLICATION_ID selected in 1st mask not available in 3rd mask
                            ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                            ' Date      : 2019-07-05
                        ElseIf mUser.RequestType = myUser.RequestTypeOption.update Then
                            For iCntx = 0 To chkSYSTEMS.Items.Count - 1
                                If (chkSYSTEMS.Items(iCntx).Selected = True) Then
                                    lSysCheck = True
                                End If
                            Next
                            If lSysCheck = False Then
                                If Not Session("ObjKeyFields") Is Nothing Then
                                    _lstObjKeyFields = Session("ObjKeyFields")
                                    For Each pair As KeyValuePair(Of String, String) In _lstObjKeyFields
                                        If pair.Key = "APPLICATION_ID" Then
                                            lblStatus.Text = pair.Value & " system is not available in customizing settings, contact support in case DRS request needs to be created for this system."
                                            Exit Sub
                                        End If
                                    Next
                                End If
                            End If
                        End If

                    End If
                    ' Reference End     : ZHHR 1053017
                    lBuildTexts = True
                Case Else
                    If InStr(Left(lCriteriaTarget, 3), "img") = 1 Then
                        lStatus = ViewStatus.ImageClick
                    ElseIf InStr(Left(lCriteriaTarget, 3), "btn") = 1 Then
                        lStatus = ViewStatus.ButtonClick
                    End If
                    ' Reference END : CR ZHHR 1050708
                    '---------------------------------------------------------------------------------------------------
            End Select

            mvDocuTab.ActiveViewIndex = 0
            Dim lReturnedObj As String = SetCurrentObject(lStatus)

            '---------------------------------------------------------------------------------------------------
            ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
            ' Comment           : Added filter below 
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 15-Feb-2016
            If (Not mUser Is Nothing) Then
                If (isObjChanged) Then   'CRT-2066302 : For selecting Default All in filter functionality--by Kanchan Bhor
                    mUser.CocFilter = "All"
                Else
                    mUser.CocFilter = GetFilterStatus()
                End If

                If (Not mUser.Current_OBJ Is Nothing) Then
                    lSysStatus = mUser.GBOXmanager.GetSysDependentInfo(mUser.Current_OBJ.OBJ_ID)
                    If (lSysStatus) Then
                        btnCopyValue.Visible = True
                        btnCopyValue.Enabled = False
                    Else
                        btnCopyValue.Visible = False
                    End If
                End If
            End If
            ' Reference End     : ZHHR 1053017

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1027881 - GBOX COC - Error in object handling 
            ' Comment           : In case page navigation link from grid, no change in buttons behavior
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-04-30
            '---------------------------------------------------------------------------------------------------                    
            If Not Me.Request.Form("__EVENTARGUMENT") Is Nothing Then
                If InStr(Me.Request.Form("__EVENTARGUMENT").ToString, "Page") = 0 Then
                    HideAll()
                End If
            Else
                HideAll()
            End If

            If lReturnedObj = "" Then
                If lCriteriaTarget = "" Then Exit Sub
                Enable_Disable(lCriteriaTarget, "NONE")
            Else
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "" Then
                    Enable_Disable(lCriteriaTarget, "NONE")
                Else
                    If lCriteriaTarget = "" Then lCriteriaTarget = "trvOBJ"
                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR 1027881 - GBOX COC - Error in object handling 
                    ' Comment           : In case page navigation link from grid, no change in buttons behavior
                    ' Added by          : Surendra Purav (CWID : EQIZU)
                    ' Date              : 2014-04-30
                    '---------------------------------------------------------------------------------------------------                    
                    If Not Me.Request.Form("__EVENTARGUMENT") Is Nothing Then
                        If InStr(Me.Request.Form("__EVENTARGUMENT").ToString, "Page") = 0 Then
                            Enable_Disable(lCriteriaTarget, mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                        End If
                    Else
                        Enable_Disable(lCriteriaTarget, mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                    End If
                End If
            End If
            Select Case lStatus
                Case ViewStatus.MainMenu
                    mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
                    LoadWizardData("INIT")
                    Exit Sub
                Case ViewStatus.WizzardMenu
                    'LoadFactoryAndControllerAndBind(mWithPaging)
                    Exit Sub
                Case ViewStatus.Detailsmenu
                    lblToMany.Text = ""
                    LoadFactoryAndControllerAndBind(mWithPaging)
                    ' Reference : YHHR 2050174 - GBOX COC: Run report by pressing enter
                    ' Comment   : Modified the informative message as required by this change
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                    ' Date      : 2019-08-19
                    lblToMany.Text = "To show data, please select tree node and press Enter/Run report button!"
                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : CR 1027410 - GBOX COC - Button missing in G-Box query 
                    ' Comment           : Make visible the G-Box query button to run report
                    ' Added by          : Surendra Purav (CWID : EQIZU)
                    ' Date              : 2014-04-30
                    '---------------------------------------------------------------------------------------------------
                    'imgQuery.Visible = True
                    LoadWizardData(mUser.Current_OBJ.OBJ_ID)
                    ' CheckAlert()
                    mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
                    Exit Sub
                Case ViewStatus.TreeClick
                    grdDat.EmptyDataText = "To show data select object in tree."
                    grdDat.DataSource = Nothing
                    grdDat.DataBind()
                    grdQuery.EmptyDataText = "To show data select object in tree."
                    grdQuery.DataSource = Nothing
                    grdQuery.DataBind()
                    ' MakeSomeEnDisabling(False)
                    Exit Sub
                Case ViewStatus.NodeClick
                    If LetTheNodeBeClicked() Then Exit Sub
                Case ViewStatus.ImageClick
                    lBuildTexts = LetTheImageBeClicked(lCriteriaTarget)
                    '---------------------------------------------------------------------
                    ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
                    ' Comment   : During page load, display the search functionality
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2015-12-24
                    If Me.Request.Form("__EVENTTARGET") Is Nothing And lCriteriaTarget = "imgSearchEngine" Then
                        mvContents.ActiveViewIndex = GetIndexByViewName("vwSearchEngine")
                        imgSearchEngine_Click(imgSearchEngine, Nothing)
                    End If
                    ' Reference END : CR ZHHR 1050708
                    '---------------------------------------------------------------------
                Case ViewStatus.SelectLink
                    'hier requestedFor...
                    LoadFactoryAndControllerAndBind(mWithPaging)
                    If InStr(Me.Request.Form("__EVENTARGUMENT").ToString, "Page") <> 0 Then
                        grdDat.PageIndex = Me.Request.Form("__EVENTARGUMENT").ToString.Split("$")(1) - 1

                        ' Reference         : CRT 2057353 - Implementation of Filters
                        ' Comment           : Added  code to view Filter options after paging
                        ' Added by          : Anant Jadhav (CWID : EPAEF)
                        ' Date              : 31-Mar-2020

                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        chkPaging.Visible = True

                        ' Reference         : CRT 2057353 - Implementation of Filters

                        Exit Sub
                    End If
                    grdDat.SelectedIndex = Me.Request.Form("__EVENTARGUMENT").ToString.Split("$")(1)
                    dvInfo.ToolTip = ""
                    If Not BindDetails(mWithPaging, False) Then Exit Sub
                    mvContents.ActiveViewIndex = GetIndexByViewName("vwDetails")
                    cmborglevel.Items.Clear()
                    cmborglevelvalue.Items.Clear()
                    Dim ret As Integer = FillCombo(cmborglevel, "ORG_LEVEL_ID", "")
                    If ret = 1 Then
                        FillCombo(cmborglevelvalue, "ORG_LEVEL_VALUE", " AND ORG_LEVEL_ID = '" & cmborglevel.SelectedValue & "'")
                    End If

                    '---------------------------------------------------------------------
                    ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
                    ' Comment   : Handle the button click based on source id value
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2015-12-24
                Case ViewStatus.ButtonClick
                    Select Case hidSourceID.Value
                        Case "btnNextEditText"
                            lBuildTexts = NextStepAction(lCriteriaTarget)
                        Case "btnNextEditSystems"
                            lBuildTexts = NextStepAction(lCriteriaTarget)
                        Case "btnSubmit"
                            lBuildTexts = NextStepAction(lCriteriaTarget)
                        Case "btnTextFill", "btnOverwriteEnglish"
                            lBuildTexts = LetTheImageBeClicked(lCriteriaTarget)
                    End Select
                    ' Reference END : CR ZHHR 1050708
                    '---------------------------------------------------------------------
            End Select
            If lBuildTexts Then
                LoadFactoryAndControllerAndBind(mWithPaging)
                If btnSubmit.CommandArgument = "New" Then
                    BindDetails(mWithPaging, True)
                Else
                    BindDetails(mWithPaging, False)
                End If
                Dim lcurrentkeyvalue As String = ""
                For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                    If InStr(lKey.Displayname.ToUpper, "VERSION") = 0 Then
                        lcurrentkeyvalue = lKey.CurrentValue
                    End If
                Next
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID <> "SINGLE" Then
                    lcurrentkeyvalue = lblObj_VALUE.Text
                End If
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Then
                    BuildTextgrid_TXT()
                    '---------------------------------------------------------------------------------------
                    ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                    ' Comment   : New concept for multiple text functionality
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2015-03-24
                ElseIf mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                    BuildTextgrid_TXT()
                    ' Reference END : CR ZHHR 1038241
                    '----------------------------------------------------------------------------------------
                Else
                    'BuildTextgrid(lcurrentkeyvalue)
                End If
                mvContents.ActiveViewIndex = GetIndexByViewName("vwEditTexts")
            End If

            ChangeIcons()
            '---------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
            ' Comment   : Bind details view every time for dropdown selectedindex change
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-07-25
            If lCriteriaTarget.Contains("ddlDynamic-") And Page.IsPostBack Then
                mvContents.ActiveViewIndex = GetIndexByViewName("vwDetails")
                BindDetails(mWithPaging, True)
                mUser.EditMode = True
            End If
            ' Reference END : CR ZHHR 1059522
            '---------------------------------------------------------------------------------
        Catch ex As Exception
            If Not mUser Is Nothing Then
                mErrText &= "PAGELOAD" & ex.Message '& vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
                lblStatus.Text = mErrText
            Else
                lblStatus.Text = lblInformations.Text
            End If
        End Try
    End Sub


#End Region
#End Region
#Region "Subs And Functions - Save & Co"
    Private Function GetParamList(Optional ByVal lReleaseFilter As Boolean = False) As String
        Dim lParamlist As String = ""
        If Request Is Nothing Then Return lParamlist
        With Request
            If Not .Params("CONTEXT") Is Nothing Then
                lParamlist = "?CONTEXT=" & .Params("CONTEXT")
            Else
                lParamlist = "?CONTEXT=general"
            End If

            If Not .Params("TOPICGROUP") Is Nothing Then
                If Not mnuNavigate.SelectedItem Is Nothing Then
                    lParamlist = lParamlist & "&TOPICGROUP=" & mnuNavigate.SelectedItem.Text
                Else
                    lParamlist = lParamlist & "&TOPICGROUP=" & .Params("TOPICGROUP")
                End If
            Else
                lParamlist = lParamlist & "&TOPICGROUP=" & mnuNavigate.SelectedItem.Text
            End If

            If Not .Params("TOPIC") Is Nothing Then
                If Not mnuDetailsMenu.SelectedItem Is Nothing Then
                    lParamlist = lParamlist & "&TOPIC=" & mnuDetailsMenu.SelectedItem.Text
                Else
                    lParamlist = lParamlist & "&TOPIC=" & .Params("TOPIC")
                End If
            Else
                lParamlist = lParamlist & "&TOPIC=" & mnuDetailsMenu.SelectedItem.Text
            End If

            If trvOBJ.SelectedNode.ValuePath Is Nothing Then
                lParamlist = lParamlist & "&PATH=" & .Params("PATH")
            Else
                lParamlist &= "&PATH=" & trvOBJ.SelectedNode.ValuePath
            End If
            '------------------------------------------------------------------------------
            'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
            'Comment    : Replace all blanks in GBOX Cockpit URL by %20
            'Added by   : Pratyusa Lenka (CWID : EOJCG)
            'Date       : 2016-02-15
            lParamlist = Regex.Replace(lParamlist, "\s+", "%20")
            If lReleaseFilter Then Return lParamlist
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1028074 - GBOX COC - filter function not working properly 
            ' Comment           : Check if filters is applied in the request values
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-05-06
            '---------------------------------------------------------------------------------------------------
            ' Comment           : if there is no error then append the param list parameters
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-05-09
            '---------------------------------------------------------------------------------------------------
            If Not .Params("VALUES") Is Nothing And mUser.GBOXmanager.ErrString = "" Then
                lParamlist = lParamlist & "&VALUES=" & .Params("VALUES")
            End If

            If Not String.IsNullOrEmpty(txtFiltertext.Text) Then
                Dim lFilterstring As String = ""
                Dim lFieldname As String = mUser.GBOXmanager.GetFieldNameByObjDisplay(mUser.Current_OBJ.OBJ_ID, cmbFieldFilter.Text)
                lFilterstring = lFieldname
                lFilterstring &= "|" & txtFiltertext.Text
                lParamlist &= "&VALUES=" & lFilterstring
            End If
        End With
        lParamlist = Regex.Replace(lParamlist, "\s+", "%20")
        ' Reference END : CR ZHHR 1053558
        '------------------------------------------------------------------------------
        Return lParamlist
    End Function

    Private Sub SaveMaster()
        Try
            Dim lFields As New List(Of String)
            Dim lValues As New List(Of String)
            Dim lDisplay As New List(Of String)
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
            End If
            mUser = mDynamicFormController.User
            Dim lSql As String
            lSql = "SELECT *,ROLE_CLUSTER_DEF.DESCRIPTION "
            lSql = lSql & "FROM wf_DEFINE_WORKFLOW_DETAILS Left Join ROLE_CLUSTER_DEF on ROLE_CLUSTER_DEF.ROLE_CLUSTER_ID = wf_DEFINE_WORKFLOW_DETAILS.ROLE_CLUSTER_ID"
            lSql = lSql & " Where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "'"
            lSql = lSql & " AND RANK ='1'"
            Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            If dt.Rows.Count = 0 Then
                mDynamicFormController.ErrorInfo("Customize Workflow for Object:" & mUser.Current_OBJ.OBJ_ID)
                Exit Sub
            End If
            Dim row As DataRow = dt.Rows(0)
            Dim lSqlSuffix As String = "Select REQUEST_ID_SUFFIX From wf_DEFINE_WORKFLOW where WORKFLOW_ID='" & row("WORKFLOW_ID").ToString & "'"
            Dim lSuffixDT As DataTable = mUser.Databasemanager.MakeDataTable(lSqlSuffix)
            Dim lSuffix As String = "_" & lSuffixDT.Rows(0)("REQUEST_ID_SUFFIX").ToString
            Dim lRequest_id = mUser.GBOXmanager.GetGBOXId("", lSuffix)
            mUser.Current_Request_ID = lRequest_id
            lstFields.Items.Clear()
            lstValues.Items.Clear()
            lstDisplay.Items.Clear()
            lstSQL.Items.Clear()
            mUser.EditMode = False
            Dim lOBJ_Value As String = ""
            Dim lPack As New List(Of String)
            Dim lCheckBoxListSql As String = ""
            Dim lintVersion As Long = 0
            Dim lCurrentKeyDisplayName As String = LTrimRTrim(mUser.GBOXmanager.GetDisplayNameByObjFieldId(mUser.Current_OBJ.OBJ_ID, mUser.Current_OBJ.OBJ_TABLENAME_KEY))
            Dim lCurrentKey As String = LTrimRTrim(mUser.Current_OBJ.OBJ_TABLENAME_KEY)
            Dim lCurrentKeyValue As String = LTrimRTrim(mUser.OBJ_Value)
            Dim lTempatefaktory = New myTemplateFactory(mUser)
            Dim bIsCalculatedField As Boolean = False
            lTempatefaktory.Requestform = Request.Form
            If mDynamicFormController Is Nothing Then
                Dim lFacory As New Dynamic_View_Controller_Factory
                lFacory.TOPIC_ID = LTrimRTrim(mnuDetailsMenu.SelectedValue)
                lFacory.TOPIC_GROUP_ID = LTrimRTrim(mnuNavigate.SelectedValue)
                lFacory.TOPIC_GROUP_CONTEXT_ID = Request.Params("CONTEXT").ToString()
                lFacory.IsPostback = Me.IsPostBack
                lFacory.Request = Me.Request
                mDynamicFormController = lFacory.SelectedNodeChange(trvOBJ.SelectedNode)
            End If
            'PROBLEM MEHRFACHSCHLÜSSEL Notwehr...Ja wir lesen nun aus dem ToolTip
            Dim lToolTip As String = dvInfo.ToolTip
            If lToolTip <> "" Then
                Dim lKeyArray As Array = lToolTip.Split("~")
                For i = 0 To lKeyArray.GetUpperBound(0)
                    For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                        If InStr(lKeyArray(i).ToString, lKey.Key_ID) <> 0 Then
                            '---------------------------------------------------------------------------------------------------
                            ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
                            ' Comment           : System dependent workflow
                            ' Added by          : Pratyusa Lenka (CWID : EOJCG)
                            ' Date              : 03-Nov-2017
                            Dim lreqVal As Object = ""
                            If Request.Form("dvInfo" & "$" & LTrimRTrim(lKey.Displayname)) <> Nothing Then
                                lreqVal = LTrimRTrim(Request.Form("dvInfo" & "$" & LTrimRTrim(lKey.Displayname)).ToString)
                            End If
                            ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                            ' Comment   : Change to consider the correct key value for lblObj_VALUE.Text
                            ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                            ' Date      : 2019-01-30
                            If lreqVal <> "" Then
                                lKey.CurrentValue = lreqVal
                                If mUser.GBOXmanager.GetTableKey(mUser.Current_OBJ.OBJ_ID.ToString) = lKey.Key_ID Then
                                    lblObj_VALUE.Text = lreqVal
                                End If
                            Else
                                lKey.CurrentValue = LTrimRTrim(lKeyArray(i).ToString.Split("|")(1))
                                If mUser.GBOXmanager.GetTableKey(mUser.Current_OBJ.OBJ_ID.ToString) = lKey.Key_ID Then
                                    lblObj_VALUE.Text = LTrimRTrim(lKey.CurrentValue)
                                End If
                            End If
                            ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                            ' Comment   : Add key ID and value to the list
                            ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                            ' Date      : 2018-10-17
                            lstObjKeyFields.Add(New KeyValuePair(Of String, String)(lKey.Key_ID, lKey.CurrentValue))
                        End If
                    Next
                Next
                lblObj_ID.Text = LTrimRTrim(mUser.Current_OBJ.OBJ_ID)
            Else
                Dim lText As String = LTrimRTrim(mDynamicFormController.GetKeyValue(Me.Request))
                Dim lCurrentkeys As New List(Of myKeyObj)
                Dim lArr As Array = lText.Split("|")
                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                ' Comment   : Clear the key IDs and Values for caching
                ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                ' Date      : 2019-04-01
                lstKey_IDs.Items.Clear()
                lstKeyValues.Items.Clear()
                For i = 0 To lArr.Length - 1
                    Dim lKey As New myKeyObj
                    lstKey_IDs.Items.Add(LTrimRTrim(lArr(i).ToString.Split("~")(0)))

                    lstKeyValues.Items.Add(LTrimRTrim(lArr(i).ToString.Split("~")(1)))
                    ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                    ' Comment   : Change to consider the correct key value for lblObj_VALUE.Text
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                    ' Date      : 2019-01-30
                    If mUser.GBOXmanager.GetTableKey(mUser.Current_OBJ.OBJ_ID.ToString) = LTrimRTrim(lArr(i).ToString.Split("~")(0)) Then
                        lblObj_VALUE.Text = LTrimRTrim(lArr(i).ToString.Split("~")(1))
                    End If
                    lCurrentkeys.Add(lKey)
                    ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                    ' Comment   : Add key ID and value to the list
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                    ' Date      : 2018-10-17
                    lstObjKeyFields.Add(New KeyValuePair(Of String, String)(LTrimRTrim(lArr(i).ToString.Split("~")(0)), LTrimRTrim(lArr(i).ToString.Split("~")(1))))
                Next
                lblObj_ID.Text = LTrimRTrim(mUser.Current_OBJ.OBJ_ID)

            End If
            ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
            ' Comment   : Store the key IDs and values to session variable
            ' Added by  : Pratyusa Lenka (CWID : EOJCG)
            ' Date      : 2018-10-17
            Session("ObjKeyFields") = lstObjKeyFields
            imgCancel.Enabled = True
            If Not Request.Form Is Nothing Then
                mDynamicFormController.Request = Request
            End If
            mUser.Query = False
            mDynamicFormController.TOPIC_ID = LTrimRTrim(mUser.Current_OBJ.TOPIC_ID)
            Dim lWherestring As String = ""
            ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
            ' Comment   : For Copy value consider the where string based on key collection
            ' Added by  : Pratyusa Lenka (CWID : EOJCG)
            ' Date      : 2019-04-01
            If mUser.RequestType = myUser.RequestTypeOption.insert AndAlso lblRequestType.Text <> "Copy" Then
                lWherestring = mDynamicFormController.GetWhereString(Request)
            Else
                lWherestring = " where "
                For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection

                    If InStr(lKey.Displayname.ToUpper, "Versionno".ToString.ToUpper) = 0 Then
                        lWherestring = lWherestring & LTrimRTrim(lKey.Key_ID) & "='" & LTrimRTrim(lKey.CurrentValue) & "' AND "
                    End If
                Next
                lWherestring = lWherestring.Substring(0, lWherestring.Length - 4)
            End If

            With mUser.Databasemanager
                lSql = "insert into " & mUser.Current_OBJ.OBJ_TABLENAME & "("
                'MAKE FIELD LIST
                Dim mdt As DataTable = mDynamicFormController.GetFieldInfoDataTable
                For Each rF As DataRow In mdt.Rows
                    If LTrimRTrim(rF("OBJ_Field_Type_ID").ToString) <> "CHECKBOXLIST" Then
                        lSql = lSql & LTrimRTrim(rF("OBJ_FIELD_ID").ToString) & ","
                        lDisplay.Add(LTrimRTrim(mUser.GBOXmanager.GetDisplayNameByObjFieldId(LTrimRTrim(mUser.Current_OBJ.OBJ_ID), LTrimRTrim(rF("OBJ_FIELD_ID").ToString))))
                        lFields.Add(LTrimRTrim(rF("OBJ_FIELD_ID").ToString))
                    End If
                Next rF
                '--------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
                ' Comment   : Delete COMPOSITE_S_T object type
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2016-01-25
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Then
                    lSql = lSql & "Active, REQUEST_ID,"
                End If
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "SINGLE" Then
                    lSql = lSql & "Active, REQUEST_ID,"
                End If
                ' Reference END : CR ZHHR 1052471
                '--------------------------------------------------------------------------------
                '---------------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                ' Comment   : New concept for multiple text functionality
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-03-24
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                    lSql = lSql & "Active, REQUEST_ID,"
                End If
                ' Reference END : CR ZHHR 1038241
                '----------------------------------------------------------------------------------------
                'MAKE VALUE LIST
                lSql = lSql.Substring(0, lSql.Length - 1)
                lSql = lSql & ")VALUES("
                For Each r As DataRow In mdt.Rows
                    Dim lreqVal As Object = ""
                    If r("OBJ_Field_Type_ID").ToString = "CHECKBOX" Then
                        If Request.Form("dvInfo" & "$" & LTrimRTrim(r("DISPLAY_NAME"))) Is Nothing Then
                            lreqVal = False
                            lSql = lSql & "'0',"
                            lValues.Add("False")
                        Else
                            lreqVal = True
                            lSql = lSql & "'1',"
                            lValues.Add("True")
                        End If
                    Else
                        'In einer Checkboxliste wird eine 1:N beziehung definiert
                        'Daher das Package mit Insert SQL
                        If r("OBJ_Field_Type_ID").ToString <> "CHECKBOXLIST" Then
                            'Hier merken wir uns welches Objekt bearbeitet wird
                            If r("DISPLAY_NAME").ToString = "OBJ_Field_Type_ID" Then
                                mOBJ_Field_Type_ID = LTrimRTrim(Request.Form("dvInfo" & "$" & LTrimRTrim(r("DISPLAY_NAME"))).ToString)
                            End If
                            If r("DISPLAY_NAME").ToString = "OBJ_ID" Then
                                lOBJ_Value = LTrimRTrim(Request.Form("dvInfo" & "$" & LTrimRTrim(r("DISPLAY_NAME"))).ToString)
                            End If

                            If r("DISPLAY_NAME").ToString.ToUpper = "OBJ_FIELD_ID" Then
                                mOBJ_FIELD_ID = LTrimRTrim(Request.Form("dvInfo" & "$" & LTrimRTrim(r("DISPLAY_NAME"))).ToString)
                            End If
                            '----------------------------------------------------------------------------------------------
                            ' Reference : CR ZHHR 1054864 - GBOX COC: OTT 2190 - Description and EN translation correlation
                            ' Comment   : Store the description value in first form
                            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                            ' Date      : 2016-03-29

                            ' Reference : HPSM IM0004854783 - YHHR - 2012389 - GBOX COC: Request table TTXID not possib
                            ' Comment   : Add contains function to check description text in column OBJ_FIELD.DISPLAY_NAME
                            ' Added by  : EOJCH 
                            ' Date      : 18-Jul-2017

                            If r("DISPLAY_NAME").ToString.ToUpper.Contains("DESCRIPTION") Then
                                'If r("DISPLAY_NAME").ToString.ToUpper = "DESCRIPTION" Then
                                strDescription = LTrimRTrim(Request.Form("dvInfo" & "$" & LTrimRTrim(r("DISPLAY_NAME"))).ToString)
                            End If
                            'Reference End: HPSM IM0004854783 - YHHR - 2012389

                            ' Reference END : CR ZHHR 1054864
                            '----------------------------------------------------------------------------------------------
                            If r("OBJ_Field_Type_ID").ToString.ToUpper = "VERSIONNUMBER" Then
                                'Versionermitteln
                                Dim lsqlVersion As String = "select MAX(OBJ_VERSIONNUMBER) as OBJ_VERSIONNUMBER from " & LTrimRTrim(mUser.Current_OBJ.OBJ_TABLENAME) & lWherestring & " order by OBJ_VERSIONNUMBER desc"
                                Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lsqlVersion)
                                If Not ldt Is Nothing Then
                                    Dim lVersionNumber As String = LTrimRTrim(ldt.Rows(0)("OBJ_VERSIONNUMBER").ToString)
                                    If lVersionNumber <> "" Then
                                        lintVersion = CLng(lVersionNumber)
                                    Else
                                        lintVersion = 0
                                    End If
                                End If
                                lintVersion = lintVersion + 1
                                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                                ' Comment   : For system dependent, always the version should be 1 for create/copy functionality
                                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                ' Date      : 2018-10-15
                                If (Not mUser Is Nothing) Then
                                    If (Not mUser.Current_OBJ Is Nothing) Then
                                        Dim lSysStatus As Boolean = mUser.GBOXmanager.GetSysDependentInfo(mUser.Current_OBJ.OBJ_ID)
                                        If (lSysStatus AndAlso (lblRequestType.Text = "Copy" Or mUser.RequestType = myUser.RequestTypeOption.insert)) Then
                                            lintVersion = 1
                                        End If
                                    End If
                                End If

                                lreqVal = lintVersion
                            Else
                                Dim lIsKey As Boolean = False
                                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                                ' Comment   : For copy functionality, requested value should be based on key collection
                                ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                                ' Date      : 2019-04-01
                                If mUser.RequestType = myUser.RequestTypeOption.insert AndAlso lblRequestType.Text <> "Copy" Then
                                    '-----------------------------------------------------------------------------
                                    ' Reference : CR ZHHR 1057806 - GBOX COC: Problem creating a T880 request
                                    ' Comment   : Make the calculated field as true for OBJ field type = CALCULATE
                                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                    ' Date      : 2016-06-03
                                    If r("OBJ_Field_Type_ID").ToString.ToUpper = "CALCULATE" Then
                                        bIsCalculatedField = True
                                        '---------------------------------------------------------------------------------
                                        ' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
                                        ' Comment   : Consider the dynamic lookup cascade field while save master data
                                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                        ' Date      : 2016-07-28
                                    ElseIf mUser.Databasemanager.IsObjFieldInLookupFilter(r("OBJ_FIELD_ID").ToString) Or Request.Form("dvInfo" & "$" & "ddlDynamic-" & r("DISPLAY_NAME")) <> Nothing Then
                                        lreqVal = LTrimRTrim(Request.Form("dvInfo" & "$" & "ddlDynamic-" & LTrimRTrim(r("DISPLAY_NAME")).ToString))
                                        ' Reference END : CR ZHHR 1059522
                                        '---------------------------------------------------------------------------------
                                    Else
                                        lreqVal = LTrimRTrim(Request.Form("dvInfo" & "$" & LTrimRTrim(r("DISPLAY_NAME")).ToString))
                                    End If
                                    ' Reference END : CR ZHHR 1057806
                                    '-----------------------------------------------------------------------------
                                Else
                                    For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                                        If lKey.Displayname = r("DISPLAY_NAME").ToString Then
                                            lIsKey = True
                                            ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                                            ' Comment   : For copy functionality, requested value should be based on key collection
                                            ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                                            ' Date      : 2019-04-01
                                            If mUser.RequestType = myUser.RequestTypeOption.update Or lblRequestType.Text = "Copy" Then
                                                lreqVal = LTrimRTrim(lKey.CurrentValue)
                                            End If
                                        End If
                                    Next
                                    If Not lIsKey Then
                                        '---------------------------------------------------------------------------------------------------
                                        ' Reference         : ZHHR 1038582 - GBOX COC : Workflow continues without authorization
                                        ' Comment           : Condition check for object null
                                        ' Added by          : Milind Randive (CWID : EOJCH)
                                        ' Date              : 2015-02-19
                                        If Request.Form("dvInfo" & "$" & r("DISPLAY_NAME")) <> Nothing Then
                                            lreqVal = Request.Form("dvInfo" & "$" & r("DISPLAY_NAME")).ToString
                                            '---------------------------------------------------------------------------------
                                            ' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
                                            ' Comment   : Consider the dynamic lookup cascade field while save master data
                                            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                            ' Date      : 2016-07-28
                                        ElseIf mUser.Databasemanager.IsObjFieldInLookupFilter(r("OBJ_FIELD_ID").ToString) Or Request.Form("dvInfo" & "$" & "ddlDynamic-" & r("DISPLAY_NAME")) <> Nothing Then
                                            lreqVal = Request.Form("dvInfo" & "$" & "ddlDynamic-" & r("DISPLAY_NAME")).ToString
                                            ' Reference END : CR ZHHR 1059522
                                            '---------------------------------------------------------------------------------
                                        End If
                                        ' Reference End      : ZHHR 1038582
                                    End If
                                    '----------------------------------------------------------------------------------
                                    ' Reference : CR ZHHR 1057031 - GBOX COC: OTT 2741 - New field type CALCULATE
                                    ' Comment   : Make the calculated field as true for OBJ field type = CALCULATE
                                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                    ' Date      : 2016-05-20
                                    If r("OBJ_Field_Type_ID").ToString.ToUpper = "CALCULATE" Then
                                        bIsCalculatedField = True
                                    End If
                                    ' Reference END : CR ZHHR 1057031
                                    '----------------------------------------------------------------------------------
                                End If
                            End If
                            '-------------------------------------------------------------------------------------------------

                            '' Reference : YHHR 2036565 - GBox: Single Quote Issue
                            '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
                            '' Added by  : Rajan Dmello (CWID : EOLRG) 
                            '' Date      : 2018-10-30

                            ' Reference : CRT 2046396, GBOX_COC : Handle Non Latin character
                            ' Comment   : Non latin characters should be inserted to the database as it is for all fields
                            ' Code for ZHHR 1070455 (GBOX COC: Error transferring non latin characters) was removed
                            ' as it was inplemented only for description

                            lSql = lSql & "N'" & lreqVal.ToString.Replace("'", "''") & "',"

                            '---------------------------------------------------------------------------------------------------
                            ' Reference         : ZHHR 1047686- GBOX COC: Cannot create DRS request
                            ' Comment           : Added replace function below
                            ' Added by          : Milind Randive (CWID : EOJCH)
                            ' Date              : 28-Aug-2015

                            lValues.Add(LTrimRTrim(lreqVal.ToString).Replace("'", "''"))
                            ' Reference End     : ZHHR 1047686
                        End If
                    End If
                Next
                '--------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
                ' Comment   : Delete COMPOSITE_S_T object type
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2016-01-25
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Then
                    lSql = lSql & "1,"
                    lSql = lSql & "'" & LTrimRTrim(mUser.Current_Request_ID) & "',"
                    lValues.Add("1")
                End If
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "SINGLE" Then
                    lSql = lSql & "1,"
                    lValues.Add("1")
                    lSql = lSql & "'" & LTrimRTrim(mUser.Current_Request_ID) & "',"
                End If
                ' Reference END : CR ZHHR 1052471
                '--------------------------------------------------------------------------------
                '---------------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                ' Comment   : New concept for multiple text functionality
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-03-24
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                    lSql = lSql & "1,"
                    lSql = lSql & "'" & LTrimRTrim(mUser.Current_Request_ID) & "',"
                    lValues.Add("1")
                End If
                ' Reference END : CR ZHHR 1038241
                '----------------------------------------------------------------------------------------
                lSql = lSql.Substring(0, lSql.Length - 1)
                lSql = lSql & ")"
                lPack.Add(lSql)
                ' End If
            End With

            lblVersionnumber.Text = lintVersion
            '----------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1057031 - GBOX COC: OTT 2741 - New field type CALCULATE
            ' Comment   : Get the calculation for calculated field and update customizing table
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-05-20
            If bIsCalculatedField Then
                Dim lstDelete As New List(Of String)
                Dim lIsKey As Boolean = False
                mUser.Databasemanager.ExecutePackage(lPack)
                lstDelete.Add("DELETE FROM " & mUser.Current_OBJ.OBJ_TABLENAME & lWherestring & " And OBJ_VERSIONNUMBER='" & lblVersionnumber.Text & "'")
                Dim strSqlCalculateValidation As String = "SELECT FCV.OBJ_FIELD_ID, FCV.CALCULATION FROM OBJ_FIELD_CALCULATE_VALIDATION FCV"
                strSqlCalculateValidation &= " INNER JOIN OBJ_FIELD OFL ON FCV.OBJ_ID = OFL.OBJ_ID AND FCV.OBJ_FIELD_ID = OFL.OBJ_FIELD_ID"
                strSqlCalculateValidation &= " AND OFL.OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "' AND OFL.OBJ_Field_Type_ID = 'CALCULATE'"
                Dim dtCalculateValidation As DataTable = mUser.Databasemanager.MakeDataTable(strSqlCalculateValidation)
                If Not dtCalculateValidation Is Nothing AndAlso dtCalculateValidation.Rows.Count > 0 Then
                    For Each drRow As DataRow In dtCalculateValidation.Rows
                        Dim strSqlCalculation As String = "SELECT " & drRow("CALCULATION").ToString & " AS " & drRow("OBJ_FIELD_ID").ToString & " FROM " & mUser.Current_OBJ.OBJ_TABLENAME & lWherestring & " And OBJ_VERSIONNUMBER='" & lblVersionnumber.Text & "'"
                        Dim dtCalculation As DataTable = mUser.Databasemanager.MakeDataTable(strSqlCalculation)
                        If Not dtCalculation Is Nothing AndAlso dtCalculation.Rows.Count > 0 Then
                            For iCnt = 0 To lFields.Count - 1
                                If lFields(iCnt).ToString = drRow("OBJ_FIELD_ID").ToString Then
                                    lValues(iCnt) = dtCalculation.Rows(0)(drRow("OBJ_FIELD_ID").ToString)
                                    '---------------------------------------------------------------------------------------------
                                    ' Reference : CR ZHHR 1066734 - GBOX COC: No change via CALCULATE functionality for key values
                                    ' Comment   : Check whether the CALCULATE field is NOT key field and key value should be calculated for NEW entries, but not for new versions
                                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                    ' Date      : 2017-01-11
                                    For Each objKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                                        If objKey.Key_ID.ToUpper = drRow("OBJ_FIELD_ID").ToString.ToUpper And mUser.RequestType = myUser.RequestTypeOption.update Then
                                            lIsKey = True
                                        End If
                                    Next
                                    '-------------------------------------------------------------------------------------
                                    ' Reference : CR ZHHR 1065435 - GBOX COC: CALCULATE of CS_ENH_MAT.KEY_ID does not work
                                    ' Comment   : Update the OBJ_VALUE in WF_REQUEST_ITEM_DETAIL, reassign the key value
                                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                    ' Date      : 2016-12-08
                                    'lPack.Add("UPDATE " & mUser.Current_OBJ.OBJ_TABLENAME & " SET " & drRow("OBJ_FIELD_ID").ToString & "=" & drRow("CALCULATION").ToString & lWherestring & " And OBJ_VERSIONNUMBER='" & lblVersionnumber.Text & "'")
                                    'lPack.Add("UPDATE " & mUser.Current_OBJ.OBJ_TABLENAME & " SET " & drRow("OBJ_FIELD_ID").ToString & "=" & dtCalculation.Rows(0)(drRow("OBJ_FIELD_ID").ToString) & lWherestring & " And OBJ_VERSIONNUMBER='" & lblVersionnumber.Text & "'")
                                    If Not lIsKey Then
                                        'Reference  : YHHR 2017696 - GBOX COC: bug for Object GPO_SUBTASK
                                        'Comment    : Quotes were missing when set the value to update the customizing table
                                        'Added by   : Pratyusa Lenka (CWID : EOJCG)
                                        'Date       : 2017-11-02
                                        lPack.Add("UPDATE " & LTrimRTrim(mUser.Current_OBJ.OBJ_TABLENAME) & " SET " & LTrimRTrim(drRow("OBJ_FIELD_ID").ToString) & "=" & "'" & LTrimRTrim(dtCalculation.Rows(0)(drRow("OBJ_FIELD_ID").ToString)) & "'" & lWherestring & " And OBJ_VERSIONNUMBER='" & lblVersionnumber.Text & "'")
                                    End If
                                    ' Reference END : CR ZHHR 2017696
                                    '---------------------------------------------------------------------------------------------
                                    For i As Integer = 0 To lstKey_IDs.Items.Count - 1
                                        If lstKey_IDs.Items(i).Text.ToUpper = LTrimRTrim(drRow("OBJ_FIELD_ID").ToString).ToUpper And lstKeyValues.Items(i).ToString.Trim = "" Then
                                            lstKeyValues.Items.RemoveAt(i)
                                            lstKeyValues.Items.Add(LTrimRTrim(dtCalculation.Rows(0)(drRow("OBJ_FIELD_ID").ToString)))
                                        End If
                                    Next
                                    ' Reference END : CR ZHHR 1065435
                                    '-------------------------------------------------------------------------------------
                                    Exit For
                                End If
                            Next iCnt
                        End If
                    Next
                End If
                mUser.Databasemanager.ExecutePackage(lstDelete)
            End If
            ' Reference END : CR ZHHR 1057031
            '----------------------------------------------------------------------------------
            For Each lStr As String In lPack
                lstSQL.Items.Add(lStr)
            Next
            Dim lSQLDeactivate As String = "Update " & LTrimRTrim(mUser.Current_OBJ.OBJ_TABLENAME)
            lSQLDeactivate = lSQLDeactivate & " Set ACTIVE = 0 " & lWherestring
            lSQLDeactivate = lSQLDeactivate & " And OBJ_VERSIONNUMBER='" & lblVersionnumber.Text - 1 & "'"
            lstSQL.Items.Add(lSQLDeactivate)

            For Each Val As String In lFields
                lstFields.Items.Add(LTrimRTrim(Val))
            Next
            For Each Val As String In lValues
                lstValues.Items.Add(LTrimRTrim(Val))
            Next
            For Each Val As String In lDisplay
                lstDisplay.Items.Add(LTrimRTrim(Val))
            Next

        Catch ex As Exception
            lblStatus.Text = "SAVE_MASTER:" & ex.Message
        End Try
    End Sub
    Private Sub Savedetails(ByVal lintVersion As Long, ByVal lOBJ_Value As String, ByVal lFields As List(Of String), ByVal lValues As List(Of String), ByVal lDisplay As List(Of String))
        Try
            Dim lPack As New List(Of String)
            '----------------------------------------------------------------------------------
            ' Reference : OTT 1303 ZHHR 1048070 - GBOX: align workflow completion email
            ' Comment   : Make mail text string as empty and system mail text as list of string
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-10-02
            Dim lTextMailText As String = ""
            Dim lSystemMailText As New List(Of String)
            ' Reference END : CR ZHHR 1048070
            For Each lStr As ListItem In lstSQL.Items
                lPack.Add(LTrimRTrim(lStr.Text.ToString))
            Next
            Dim lSql As String = ""
            Dim lOBJ_VERSION As String = lintVersion.ToString
            Dim lROrg_Level_Value As String = ""
            'BEGIN SaveTexts and Systems BEGIN

            Dim lWherestring As String = " where "
            Dim lMultiKeys As String = ""
            Dim lMultiValues As String = ""
            Dim lCompositeKey As Integer = 0
            If mUser.GBOXmanager.KeyCollection.Count > 2 Then
                lCompositeKey = 1
            End If
            lblObj_ID.Text = LTrimRTrim(mUser.Current_OBJ.OBJ_ID)
            If btnSubmit.CommandArgument = "New" Then
                For Each lkey As myKeyObj In mUser.GBOXmanager.KeyCollection
                    For i = 0 To lstKey_IDs.Items.Count - 1
                        ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                        ' Comment   : In case of copy value it was considering the old key value, hence added the 2nd condition
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                        ' Date      : 2018-10-24
                        ' Comment   : In case of copy value it was considering the old key value, hence consider ObjKeyFields instead of lstKeyValues
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                        ' Date      : 2019-07-05
                        'If lkey.Key_ID = lstKey_IDs.Items(i).Value.ToString AndAlso String.IsNullOrEmpty(lkey.CurrentValue) Then
                        If lkey.Key_ID = lstKey_IDs.Items(i).Value.ToString Then
                            Dim _lstObjKeyFields As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))
                            If Not Session("ObjKeyFields") Is Nothing Then
                                _lstObjKeyFields = Session("ObjKeyFields")
                                For Each pair As KeyValuePair(Of String, String) In _lstObjKeyFields
                                    If lkey.Key_ID = pair.Key Then
                                        lkey.CurrentValue = LTrimRTrim(pair.Value.ToString)
                                    End If
                                Next
                            End If
                            'lkey.CurrentValue = LTrimRTrim(lstKeyValues.Items(i).Value.ToString)
                        End If
                    Next
                Next
            End If

            '---------------------------------------------------------------------------------------------------
            ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
            ' Comment           : Add entries for system(s)
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 22-Feb-2016
            If mUser.SystemIds <> "" Then
                Dim strSystemId As Array = mUser.SystemIds.Split(",")
                For iCnt As Integer = 0 To strSystemId.Length - 1
                    If (strSystemId(iCnt).ToString.Trim <> "") Then

                        lSql = "INSERT INTO [wf_REQUEST_ITEM_DETAIL] "
                        lSql = lSql & "([REQUEST_ID]"
                        lSql = lSql & " ,[OBJ_ID]"
                        lSql = lSql & " ,[OBJ_VERSIONNUMBER]"
                        lSql = lSql & " ,OBJ_KEY_NAME"
                        lSql = lSql & " ,OBJ_VALUE"
                        lSql = lSql & "  ,REQUEST_ROW_NUM"
                        lSql = lSql & " ,IS_COMPOSITE_KEY_PART)"
                        lSql = lSql & " VALUES"
                        lSql = lSql & " ('" & LTrimRTrim(mUser.Current_Request_ID) & "'"
                        lSql = lSql & " ,'" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID.ToString) & "'"
                        lSql = lSql & "," & lOBJ_VERSION & ""
                        lSql = lSql & ",'APPLICATION_ID'"
                        lSql = lSql & ",'" & LTrimRTrim(strSystemId(iCnt).ToString) & "'"
                        lSql = lSql & ",'1'"
                        lSql = lSql & "," & LTrimRTrim(lCompositeKey) & ")"

                        lPack.Add(lSql)

                    End If
                Next
            End If
            ' Reference End     : ZHHR 1053017

            Dim strSys As String = ""

            Dim strSystems As Array
            If mUser.SystemIds <> "" Then
                strSystems = mUser.SystemIds.Split(",")
            End If

            Dim lSysStatus As Boolean = mUser.GBOXmanager.GetSysDependentInfo(mUser.Current_OBJ.OBJ_ID.ToString)

            For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                If InStr(lKey.Displayname.ToUpper, "VERSIONNO") = 0 Then

                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
                    ' Comment           : Added if condition and declared strSys, strSystemId 
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 22-Feb-2016
                    If mUser.SystemIds <> "" And lKey.Key_ID.Trim = "APPLICATION_ID" Then
                        For iCnt As Integer = 0 To strSystems.Length - 1
                            If (strSystems(iCnt) <> "") Then
                                If (iCnt = 0) Then
                                    strSys = "'" & LTrimRTrim(strSystems(iCnt)) & "'"
                                Else
                                    strSys += ",'" & LTrimRTrim(strSystems(iCnt)) & "'"
                                End If
                                'lKey.CurrentValue = LTrimRTrim(strSystems(iCnt))
                            End If
                        Next
                        lWherestring = lWherestring & LTrimRTrim(lKey.Key_ID) & " in ( " & LTrimRTrim(strSys) & " ) and "
                    Else
                        lWherestring = lWherestring & LTrimRTrim(lKey.Key_ID) & "='" & LTrimRTrim(lKey.CurrentValue) & "' and "
                    End If
                    ' Reference  End       : ZHHR 1053017

                    'lWherestring = lWherestring & lKey.Key_ID & "='" & lKey.CurrentValue & "' and "
                    lMultiKeys = LTrimRTrim(lMultiKeys) & "|" & LTrimRTrim(lKey.Key_ID)
                    lMultiValues = LTrimRTrim(lMultiValues) & "|" & LTrimRTrim(lKey.CurrentValue)
                    'wf_REQUEST_ITEM_DETAIL speichern

                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
                    ' Comment           : Added if condition
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 22-Feb-2016

                    If lKey.Key_ID.Trim <> "APPLICATION_ID" Then

                        lSql = "INSERT INTO [wf_REQUEST_ITEM_DETAIL] "
                        lSql = lSql & "([REQUEST_ID]"
                        lSql = lSql & " ,[OBJ_ID]"
                        lSql = lSql & " ,[OBJ_VERSIONNUMBER]"
                        lSql = lSql & " ,OBJ_KEY_NAME"
                        lSql = lSql & " ,OBJ_VALUE"
                        lSql = lSql & "  ,REQUEST_ROW_NUM"
                        lSql = lSql & " ,IS_COMPOSITE_KEY_PART)"
                        lSql = lSql & " VALUES"
                        lSql = lSql & " ('" & LTrimRTrim(mUser.Current_Request_ID) & "'"
                        lSql = lSql & " ,'" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID.ToString) & "'"
                        lSql = lSql & "," & LTrimRTrim(lOBJ_VERSION) & ""
                        lSql = lSql & ",'" & LTrimRTrim(lKey.Key_ID) & "'"
                        lSql = lSql & ",'" & LTrimRTrim(lKey.CurrentValue) & "'"
                        lSql = lSql & ",'1'"
                        lSql = lSql & "," & LTrimRTrim(lCompositeKey) & ")"

                        lPack.Add(lSql)

                    End If
                    ' Reference End      : ZHHR 1053017
                End If
            Next

            lWherestring = lWherestring.Substring(0, lWherestring.Length - 4)
            lMultiKeys = LTrimRTrim(lMultiKeys.Trim("|"))
            lMultiValues = LTrimRTrim(lMultiValues.Trim("|"))


            With mUser.Databasemanager
                '---------------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                ' Comment   : New concept for multiple text functionality
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-03-24
                '-------------------------------------------------------------------------------------------------------
                ' Reference : OTT 1303 ZHHR 1048070 - GBOX: align workflow completion email
                ' Comment   : Info email for obj classification COMPOSITE_S_T_TXT and COMPOSITE_MULTI_TXT
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-10-02
                '--------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
                ' Comment   : Delete COMPOSITE_S_T object type
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2016-01-25
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                    Dim lstChangeFieldList = New List(Of String)
                    Dim IsValueExistInOtherColumn As Boolean
                    Dim IsValueExistInOtherRow As Boolean = False
                    Dim iRowCount As Integer = 0
                    If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                        Dim lMakeInsert As Boolean = False
                        Dim rowIndex As Integer = 0
                        Dim strResultArray As Array = strResult.Split(",")

                        If btnSubmit.CommandArgument = "New" AndAlso lSysStatus Then
                            For iCnt As Integer = 0 To strSystems.Length - 1
                                If (strSystems(iCnt) <> "") Then
                                    For iTextCount As Integer = 0 To strResultArray.Length - 1
                                        For Each lOldItem As ListItem In lstOldTexts.Items

                                            Dim lOldText As String = lOldItem.Text.ToString
                                            Dim lOldTextArray As Array = lOldText.Split("|")
                                            For Each lNewItem As ListItem In lstNewTexts.Items

                                                Dim lNewText As String = lNewItem.Text.ToString
                                                Dim lNewTextArray As Array = lNewText.Split("|")
                                                If lOldTextArray(0).ToString = lNewTextArray(0).ToString Then
                                                    IsValueExistInOtherColumn = False
                                                    If lstChangeFieldList.Count = rowIndex Then
                                                        For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                                                            Dim strKeyValue As String = ""
                                                            If InStr(lKey.Displayname.ToUpper, "VERSIONNO") <> 0 And lKey.CurrentValue <> lOBJ_VERSION Then
                                                                strKeyValue = "<b>" & LTrimRTrim(lOBJ_VERSION) & "</b>"
                                                            ElseIf mUser.RequestType = myUser.RequestTypeOption.insert Then
                                                                strKeyValue = "<b>" & LTrimRTrim(lKey.CurrentValue) & "</b>"
                                                            Else
                                                                strKeyValue = LTrimRTrim(lKey.CurrentValue)
                                                            End If
                                                            If lKey.Key_ID.ToUpper = "APPLICATION_ID" Then
                                                                strKeyValue = "<b>" & LTrimRTrim(strSystems(iCnt)) & "</b>"
                                                            End If
                                                            If lstChangeFieldList.Count = rowIndex Then
                                                                lstChangeFieldList.Insert(rowIndex, "<td> " & LTrimRTrim(lKey.Key_ID) & " </td>")
                                                                lstChangeFieldList.Insert(rowIndex + 1, "<td> " & LTrimRTrim(strKeyValue) & " </td>")
                                                            Else
                                                                lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & LTrimRTrim(lKey.Key_ID) & " </td>"
                                                                lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & LTrimRTrim(strKeyValue) & "</td>"
                                                            End If
                                                        Next
                                                    End If
                                                    If lOldTextArray(iTextCount + 1).ToString = "" And lNewTextArray(iTextCount + 1).ToString <> "" Then
                                                        lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & LTrimRTrim(strResultArray(iTextCount).ToString) & "_" & LTrimRTrim(lNewTextArray(0).ToString) & " </td>"
                                                        lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td><b>" & lNewTextArray(iTextCount + 1).ToString & "</b></td>"
                                                    Else
                                                        If lOldTextArray(iTextCount + 1).ToString <> "" And lNewTextArray(iTextCount + 1).ToString = "" Then
                                                            lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lOldTextArray(0).ToString & " </td>"
                                                            lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td><b>" & lOldTextArray(iTextCount + 1).ToString & "</b></td>"
                                                        Else
                                                            If lOldTextArray(iTextCount + 1).ToString <> lNewTextArray(iTextCount + 1).ToString Then
                                                                lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lNewTextArray(0).ToString & " </td>"
                                                                lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td><b>" & lNewTextArray(iTextCount + 1).ToString & "</b></td>"
                                                            End If
                                                            If lOldTextArray(iTextCount + 1).ToString = lNewTextArray(iTextCount + 1).ToString Then
                                                                If lOldTextArray(iTextCount + 1).ToString <> "" Then
                                                                    lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lNewTextArray(0).ToString & " </td>"
                                                                    lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & lNewTextArray(iTextCount + 1).ToString & "</td>"
                                                                Else
                                                                    IsValueExistInOtherColumn = False
                                                                    For iNewTextCnt As Integer = 1 To lNewTextArray.Length - 1
                                                                        If Not String.IsNullOrEmpty(lNewTextArray(iNewTextCnt).ToString) Then
                                                                            IsValueExistInOtherColumn = True
                                                                        End If
                                                                    Next
                                                                    If Not IsValueExistInOtherColumn Then
                                                                        IsValueExistInOtherColumn = False
                                                                        For iOldTextCnt As Integer = 1 To lOldTextArray.Length - 1
                                                                            If Not String.IsNullOrEmpty(lOldTextArray(iOldTextCnt).ToString) Then
                                                                                IsValueExistInOtherColumn = True
                                                                            End If
                                                                        Next
                                                                    End If
                                                                    If IsValueExistInOtherColumn Then
                                                                        lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lNewTextArray(0).ToString & " </td>"
                                                                        lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & lNewTextArray(iTextCount + 1).ToString & "</td>"
                                                                    End If
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            Next lNewItem
                                        Next lOldItem
                                        'lstChangeFieldList.Item(rowIndex) = "<tr>" & lstChangeFieldList.Item(rowIndex).ToString & "</tr>"
                                        'lstChangeFieldList.Item(rowIndex + 1) = "<tr>" & lstChangeFieldList.Item(rowIndex + 1).ToString & "</tr>"
                                        'rowIndex = rowIndex + 2
                                    Next
                                    lstChangeFieldList.Item(rowIndex) = "<tr>" & lstChangeFieldList.Item(rowIndex).ToString & "</tr>"
                                    lstChangeFieldList.Item(rowIndex + 1) = "<tr>" & lstChangeFieldList.Item(rowIndex + 1).ToString & "</tr>"
                                    rowIndex = rowIndex + 2
                                End If
                            Next
                        Else
                            For iTextCount As Integer = 0 To strResultArray.Length - 1
                                For Each lOldItem As ListItem In lstOldTexts.Items

                                    Dim lOldText As String = lOldItem.Text.ToString
                                    Dim lOldTextArray As Array = lOldText.Split("|")
                                    For Each lNewItem As ListItem In lstNewTexts.Items

                                        Dim lNewText As String = lNewItem.Text.ToString
                                        Dim lNewTextArray As Array = lNewText.Split("|")
                                        If lOldTextArray(0).ToString = lNewTextArray(0).ToString Then
                                            IsValueExistInOtherColumn = False
                                            If lstChangeFieldList.Count = rowIndex Then
                                                For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                                                    Dim strKeyValue As String = ""
                                                    If InStr(lKey.Displayname.ToUpper, "VERSIONNO") <> 0 And lKey.CurrentValue <> lOBJ_VERSION Then
                                                        strKeyValue = "<b>" & LTrimRTrim(lOBJ_VERSION) & "</b>"
                                                    ElseIf mUser.RequestType = myUser.RequestTypeOption.insert Then
                                                        strKeyValue = "<b>" & LTrimRTrim(lKey.CurrentValue) & "</b>"
                                                    Else
                                                        strKeyValue = LTrimRTrim(lKey.CurrentValue)
                                                    End If
                                                    If lstChangeFieldList.Count = rowIndex Then
                                                        lstChangeFieldList.Insert(rowIndex, "<td> " & LTrimRTrim(lKey.Key_ID) & " </td>")
                                                        lstChangeFieldList.Insert(rowIndex + 1, "<td> " & LTrimRTrim(strKeyValue) & " </td>")
                                                    Else
                                                        lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & LTrimRTrim(lKey.Key_ID) & " </td>"
                                                        lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & LTrimRTrim(strKeyValue) & "</td>"
                                                    End If
                                                Next
                                            End If
                                            If lOldTextArray(iTextCount + 1).ToString = "" And lNewTextArray(iTextCount + 1).ToString <> "" Then
                                                lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & LTrimRTrim(strResultArray(iTextCount).ToString) & "_" & LTrimRTrim(lNewTextArray(0).ToString) & " </td>"
                                                lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td><b>" & lNewTextArray(iTextCount + 1).ToString & "</b></td>"
                                            Else
                                                If lOldTextArray(iTextCount + 1).ToString <> "" And lNewTextArray(iTextCount + 1).ToString = "" Then
                                                    lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lOldTextArray(0).ToString & " </td>"
                                                    lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td><b>" & lOldTextArray(iTextCount + 1).ToString & "</b></td>"
                                                Else
                                                    If lOldTextArray(iTextCount + 1).ToString <> lNewTextArray(iTextCount + 1).ToString Then
                                                        lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lNewTextArray(0).ToString & " </td>"
                                                        lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td><b>" & lNewTextArray(iTextCount + 1).ToString & "</b></td>"
                                                    End If
                                                    If lOldTextArray(iTextCount + 1).ToString = lNewTextArray(iTextCount + 1).ToString Then
                                                        If lOldTextArray(iTextCount + 1).ToString <> "" Then
                                                            lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lNewTextArray(0).ToString & " </td>"
                                                            lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & lNewTextArray(iTextCount + 1).ToString & "</td>"
                                                        Else
                                                            IsValueExistInOtherColumn = False
                                                            For iNewTextCnt As Integer = 1 To lNewTextArray.Length - 1
                                                                If Not String.IsNullOrEmpty(lNewTextArray(iNewTextCnt).ToString) Then
                                                                    IsValueExistInOtherColumn = True
                                                                End If
                                                            Next
                                                            If Not IsValueExistInOtherColumn Then
                                                                IsValueExistInOtherColumn = False
                                                                For iOldTextCnt As Integer = 1 To lOldTextArray.Length - 1
                                                                    If Not String.IsNullOrEmpty(lOldTextArray(iOldTextCnt).ToString) Then
                                                                        IsValueExistInOtherColumn = True
                                                                    End If
                                                                Next
                                                            End If
                                                            If IsValueExistInOtherColumn Then
                                                                lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lNewTextArray(0).ToString & " </td>"
                                                                lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & lNewTextArray(iTextCount + 1).ToString & "</td>"
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Next lNewItem
                                Next lOldItem
                                lstChangeFieldList.Item(rowIndex) = "<tr>" & lstChangeFieldList.Item(rowIndex).ToString & "</tr>"
                                lstChangeFieldList.Item(rowIndex + 1) = "<tr>" & lstChangeFieldList.Item(rowIndex + 1).ToString & "</tr>"
                                rowIndex = rowIndex + 2
                            Next
                        End If







                        'For iTextCount As Integer = 0 To strResultArray.Length - 1
                        '    For Each lOldItem As ListItem In lstOldTexts.Items

                        '        Dim lOldText As String = lOldItem.Text.ToString
                        '        Dim lOldTextArray As Array = lOldText.Split("|")
                        '        For Each lNewItem As ListItem In lstNewTexts.Items

                        '            Dim lNewText As String = lNewItem.Text.ToString
                        '            Dim lNewTextArray As Array = lNewText.Split("|")
                        '            If lOldTextArray(0).ToString = lNewTextArray(0).ToString Then
                        '                IsValueExistInOtherColumn = False
                        '                If lstChangeFieldList.Count = rowIndex Then
                        '                    If btnSubmit.CommandArgument = "New" AndAlso lSysStatus Then
                        '                        Dim iSysCount As Integer = 0
                        '                        For iCnt As Integer = 0 To strSystems.Length - 1
                        '                            If (strSystems(iCnt) <> "") Then
                        '                                iSysCount = iCnt
                        '                                For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                        '                                    Dim strKeyValue As String = ""
                        '                                    If InStr(lKey.Displayname.ToUpper, "VERSIONNO") <> 0 And lKey.CurrentValue <> lOBJ_VERSION Then
                        '                                        strKeyValue = "<b>" & LTrimRTrim(lOBJ_VERSION) & "</b>"
                        '                                    ElseIf mUser.RequestType = myUser.RequestTypeOption.insert Then
                        '                                        strKeyValue = "<b>" & LTrimRTrim(lKey.CurrentValue) & "</b>"
                        '                                    Else
                        '                                        strKeyValue = LTrimRTrim(lKey.CurrentValue)
                        '                                    End If
                        '                                    If lKey.Key_ID.ToUpper = "APPLICATION_ID" Then
                        '                                        strKeyValue = "<b>" & LTrimRTrim(strSystems(iCnt)) & "</b>"
                        '                                    End If
                        '                                    If lstChangeFieldList.Count = rowIndex Or (iSysCount = iCnt And lKey.Key_ID.ToUpper = "APPLICATION_ID") Then
                        '                                        lstChangeFieldList.Insert(rowIndex, "<td> " & LTrimRTrim(lKey.Key_ID) & " </td>")
                        '                                        lstChangeFieldList.Insert(rowIndex + 1, "<td> " & LTrimRTrim(strKeyValue) & " </td>")
                        '                                    Else
                        '                                        lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & LTrimRTrim(lKey.Key_ID) & " </td>"
                        '                                        lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & LTrimRTrim(strKeyValue) & "</td>"
                        '                                    End If
                        '                                    iSysCount += 1
                        '                                Next
                        '                            End If
                        '                        Next
                        '                    Else
                        '                        For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                        '                            Dim strKeyValue As String = ""
                        '                            If InStr(lKey.Displayname.ToUpper, "VERSIONNO") <> 0 And lKey.CurrentValue <> lOBJ_VERSION Then
                        '                                strKeyValue = "<b>" & LTrimRTrim(lOBJ_VERSION) & "</b>"
                        '                            ElseIf mUser.RequestType = myUser.RequestTypeOption.insert Then
                        '                                strKeyValue = "<b>" & LTrimRTrim(lKey.CurrentValue) & "</b>"
                        '                            Else
                        '                                strKeyValue = LTrimRTrim(lKey.CurrentValue)
                        '                            End If
                        '                            If lstChangeFieldList.Count = rowIndex Then
                        '                                lstChangeFieldList.Insert(rowIndex, "<td> " & LTrimRTrim(lKey.Key_ID) & " </td>")
                        '                                lstChangeFieldList.Insert(rowIndex + 1, "<td> " & LTrimRTrim(strKeyValue) & " </td>")
                        '                            Else
                        '                                lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & LTrimRTrim(lKey.Key_ID) & " </td>"
                        '                                lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & LTrimRTrim(strKeyValue) & "</td>"
                        '                            End If
                        '                        Next
                        '                    End If
                        '                    'For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                        '                    '    Dim strKeyValue As String = ""
                        '                    '    If InStr(lKey.Displayname.ToUpper, "VERSIONNO") <> 0 And lKey.CurrentValue <> lOBJ_VERSION Then
                        '                    '        strKeyValue = "<b>" & LTrimRTrim(lOBJ_VERSION) & "</b>"
                        '                    '    ElseIf mUser.RequestType = myUser.RequestTypeOption.insert Then
                        '                    '        strKeyValue = "<b>" & LTrimRTrim(lKey.CurrentValue) & "</b>"
                        '                    '    Else
                        '                    '        strKeyValue = LTrimRTrim(lKey.CurrentValue)
                        '                    '    End If
                        '                    '    If lstChangeFieldList.Count = rowIndex Then
                        '                    '        lstChangeFieldList.Insert(rowIndex, "<td> " & LTrimRTrim(lKey.Key_ID) & " </td>")
                        '                    '        lstChangeFieldList.Insert(rowIndex + 1, "<td> " & LTrimRTrim(strKeyValue) & " </td>")
                        '                    '    Else
                        '                    '        lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & LTrimRTrim(lKey.Key_ID) & " </td>"
                        '                    '        lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & LTrimRTrim(strKeyValue) & "</td>"
                        '                    '    End If
                        '                    'Next
                        '                End If
                        '                If lOldTextArray(iTextCount + 1).ToString = "" And lNewTextArray(iTextCount + 1).ToString <> "" Then
                        '                    lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & LTrimRTrim(strResultArray(iTextCount).ToString) & "_" & LTrimRTrim(lNewTextArray(0).ToString) & " </td>"
                        '                    lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td><b>" & lNewTextArray(iTextCount + 1).ToString & "</b></td>"
                        '                Else
                        '                    If lOldTextArray(iTextCount + 1).ToString <> "" And lNewTextArray(iTextCount + 1).ToString = "" Then
                        '                        lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lOldTextArray(0).ToString & " </td>"
                        '                        lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td><b>" & lOldTextArray(iTextCount + 1).ToString & "</b></td>"
                        '                    Else
                        '                        If lOldTextArray(iTextCount + 1).ToString <> lNewTextArray(iTextCount + 1).ToString Then
                        '                            lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lNewTextArray(0).ToString & " </td>"
                        '                            lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td><b>" & lNewTextArray(iTextCount + 1).ToString & "</b></td>"
                        '                        End If
                        '                        If lOldTextArray(iTextCount + 1).ToString = lNewTextArray(iTextCount + 1).ToString Then
                        '                            If lOldTextArray(iTextCount + 1).ToString <> "" Then
                        '                                lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lNewTextArray(0).ToString & " </td>"
                        '                                lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & lNewTextArray(iTextCount + 1).ToString & "</td>"
                        '                            Else
                        '                                IsValueExistInOtherColumn = False
                        '                                For iNewTextCnt As Integer = 1 To lNewTextArray.Length - 1
                        '                                    If Not String.IsNullOrEmpty(lNewTextArray(iNewTextCnt).ToString) Then
                        '                                        IsValueExistInOtherColumn = True
                        '                                    End If
                        '                                Next
                        '                                If Not IsValueExistInOtherColumn Then
                        '                                    IsValueExistInOtherColumn = False
                        '                                    For iOldTextCnt As Integer = 1 To lOldTextArray.Length - 1
                        '                                        If Not String.IsNullOrEmpty(lOldTextArray(iOldTextCnt).ToString) Then
                        '                                            IsValueExistInOtherColumn = True
                        '                                        End If
                        '                                    Next
                        '                                End If
                        '                                If IsValueExistInOtherColumn Then
                        '                                    lstChangeFieldList.Item(rowIndex) = lstChangeFieldList.Item(rowIndex).ToString & "<td> " & strResultArray(iTextCount).ToString & "_" & lNewTextArray(0).ToString & " </td>"
                        '                                    lstChangeFieldList.Item(rowIndex + 1) = lstChangeFieldList.Item(rowIndex + 1).ToString & "<td>" & lNewTextArray(iTextCount + 1).ToString & "</td>"
                        '                                End If
                        '                            End If
                        '                        End If
                        '                    End If
                        '                End If
                        '            End If
                        '        Next lNewItem
                        '    Next lOldItem
                        '    lstChangeFieldList.Item(rowIndex) = "<tr>" & lstChangeFieldList.Item(rowIndex).ToString & "</tr>"
                        '    lstChangeFieldList.Item(rowIndex + 1) = "<tr>" & lstChangeFieldList.Item(rowIndex + 1).ToString & "</tr>"
                        '    rowIndex = rowIndex + 2
                        'Next
                    End If
                    For Each lOldItem As ListItem In lstOldTexts.Items
                        Dim lOldText As String = lOldItem.Text.ToString
                        Dim lOldTextArray As Array = lOldText.Split("|")
                        For Each lNewItem As ListItem In lstNewTexts.Items
                            Dim lNewText As String = lNewItem.Text.ToString
                            Dim lNewTextArray As Array = lNewText.Split("|")
                            If lOldTextArray(0).ToString = lNewTextArray(0).ToString Then
                                Dim lMakeInsert As Boolean = False
                                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                                    Dim strResultArray As Array = strResult.Split(",")
                                    For iTextCount As Integer = 0 To strResultArray.Length - 1
                                        For iCount As Integer = 1 To lOldTextArray.Length - 1
                                            If iCount = iTextCount + 1 Then
                                                If lOldTextArray(iCount).ToString = "" And lNewTextArray(iCount).ToString <> "" Then
                                                    lMakeInsert = True
                                                Else
                                                    If lOldTextArray(iCount).ToString <> "" And lNewTextArray(iCount).ToString = "" Then
                                                        lMakeInsert = False
                                                    Else
                                                        If lOldTextArray(iCount).ToString <> lNewTextArray(iCount).ToString Then
                                                            lMakeInsert = True
                                                        End If
                                                        If lOldTextArray(iCount).ToString = lNewTextArray(iCount).ToString Then
                                                            If lOldTextArray(iCount).ToString <> "" Then
                                                                lMakeInsert = True
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        Next
                                    Next
                                End If
                                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID <> "COMPOSITE_MULTI_TXT" Then
                                    IsValueExistInOtherRow = False
                                    If lstChangeFieldList.Count <> 2 Then
                                        For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                                            Dim strKeyValue As String = ""
                                            If InStr(lKey.Displayname.ToUpper, "VERSIONNO") <> 0 And lKey.CurrentValue <> lOBJ_VERSION Then
                                                strKeyValue = "<b>" & lOBJ_VERSION & "</b>"
                                            ElseIf mUser.RequestType = myUser.RequestTypeOption.insert Then
                                                strKeyValue = "<b>" & lKey.CurrentValue & "</b>"
                                            Else
                                                strKeyValue = lKey.CurrentValue
                                            End If
                                            If lstChangeFieldList.Count = 0 Then
                                                lstChangeFieldList.Insert(0, "<td> " & lKey.Key_ID & " </td>")
                                                lstChangeFieldList.Insert(1, "<td> " & strKeyValue & " </td>")
                                            Else
                                                lstChangeFieldList.Item(0) = lstChangeFieldList.Item(0).ToString & "<td> " & lKey.Key_ID & " </td>"
                                                lstChangeFieldList.Item(1) = lstChangeFieldList.Item(1).ToString & "<td>" & strKeyValue & "</td>"
                                            End If
                                        Next
                                    End If

                                    If lOldTextArray(1).ToString = "" And lNewTextArray(1).ToString <> "" Then
                                        lstChangeFieldList.Item(0) = lstChangeFieldList.Item(0).ToString & "<td> " & lNewTextArray(0).ToString & " </td>"
                                        lstChangeFieldList.Item(1) = lstChangeFieldList.Item(1).ToString & "<td><b>" & lNewTextArray(1).ToString & "</b></td>"
                                        lMakeInsert = True
                                    Else
                                        If lOldTextArray(1).ToString <> "" And lNewTextArray(1).ToString = "" Then
                                            lstChangeFieldList.Item(0) = lstChangeFieldList.Item(0).ToString & "<td> " & lNewTextArray(0).ToString & " </td>"
                                            lstChangeFieldList.Item(1) = lstChangeFieldList.Item(1).ToString & "<td><b>" & lOldTextArray(1).ToString & "</b></td>"
                                        Else
                                            If lOldTextArray(1).ToString <> lNewTextArray(1).ToString Then
                                                lstChangeFieldList.Item(0) = lstChangeFieldList.Item(0).ToString & "<td> " & lNewTextArray(0).ToString & " </td>"
                                                lstChangeFieldList.Item(1) = lstChangeFieldList.Item(1).ToString & "<td><b>" & lNewTextArray(1).ToString & "</b></td>"
                                                lMakeInsert = True
                                            End If
                                            If lOldTextArray(1).ToString = lNewTextArray(1).ToString Then
                                                If lOldTextArray(1).ToString <> "" And mUser.RequestType <> myUser.RequestTypeOption.insert Then
                                                    lstChangeFieldList.Item(0) = lstChangeFieldList.Item(0).ToString & "<td> " & lNewTextArray(0).ToString & " </td>"
                                                    lstChangeFieldList.Item(1) = lstChangeFieldList.Item(1).ToString & "<td> " & lNewTextArray(1).ToString & " </td>"
                                                    lMakeInsert = True
                                                ElseIf lOldTextArray(1).ToString <> "" And mUser.RequestType = myUser.RequestTypeOption.insert Then
                                                    lstChangeFieldList.Item(0) = lstChangeFieldList.Item(0).ToString & "<td> " & lNewTextArray(0).ToString & " </td>"
                                                    lstChangeFieldList.Item(1) = lstChangeFieldList.Item(1).ToString & "<td><b>" & lNewTextArray(1).ToString & "</b></td>"
                                                    lMakeInsert = True
                                                Else
                                                    For iNewTextCnt As Integer = iRowCount To lstNewTexts.Items.Count - 1
                                                        If Not String.IsNullOrEmpty(lstNewTexts.Items(iNewTextCnt).ToString.Split("|")(1)) Then
                                                            IsValueExistInOtherRow = True
                                                            Exit For
                                                        End If
                                                    Next
                                                    If IsValueExistInOtherRow Then
                                                        lstChangeFieldList.Item(0) = lstChangeFieldList.Item(0).ToString & "<td> " & lNewTextArray(0).ToString & " </td>"
                                                        lstChangeFieldList.Item(1) = lstChangeFieldList.Item(1).ToString & "<td> " & lNewTextArray(1).ToString & " </td>"
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                    iRowCount += 1
                                End If
                                ' Reference END : CR ZHHR 1048070
                                '-------------------------------------------------------------------------------------------------------
                                ' Die Tabelle OBJ_TEXTS hat keine Mehrfachschlüssel
                                If lMakeInsert Then
                                    '---------------------------------------------------------------------------------------
                                    ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                                    ' Comment   : New concept for multiple text functionality
                                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                    ' Date      : 2015-03-24
                                    '--------------------------------------------------------------------------------
                                    ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
                                    ' Comment   : Delete obsolete table OBJ_TEXTS
                                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                    ' Date      : 2016-01-25
                                    '-------------------------------------------------------------------
                                    ' Reference : CR ZHHR 1054424 - GBOX COC: problem special characters
                                    ' Comment   : Replace single quote with `
                                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                    ' Date      : 2016-03-09
                                    '' Reference : YHHR 2036565 - GBox: Single Quote Issue
                                    '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
                                    '' Added by  : Rajan Dmello (CWID : EOLRG) 
                                    '' Date      : 2018-10-30
                                    ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                                    ' Comment   : Add condition for create, copy for COMPOSITE_S_T_TXT
                                    ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                                    ' Date      : 2019-01-30
                                    If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Then
                                        If lNewTextArray(1).ToString <> "" Then
                                            If lblRequestType.Text <> "Copy" AndAlso btnSubmit.CommandArgument = "New" AndAlso lSysStatus Then
                                                Dim strMultiValues As String = ""
                                                For iCnt As Integer = 0 To strSystems.Length - 1
                                                    strMultiValues = ""
                                                    If (strSystems(iCnt) <> "") Then
                                                        strMultiValues = lMultiValues
                                                        strMultiValues = LTrimRTrim(strSystems(iCnt)) & "|" & LTrimRTrim(strMultiValues)
                                                        Dim lNewInsertsSql As String = "INSERT INTO Customizing_" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "_TXT"
                                                        lNewInsertsSql = lNewInsertsSql & "(" & LTrimRTrim(lMultiKeys.Replace("|", ","))
                                                        lNewInsertsSql = lNewInsertsSql & ",[OBJ_VERSIONNUMBER]"
                                                        lNewInsertsSql = lNewInsertsSql & ",[OBJ_LANGUAGE_ISO_CODE]"
                                                        lNewInsertsSql = lNewInsertsSql & ",[OBJ_LANGUAGE_VALUE])"
                                                        lNewInsertsSql = lNewInsertsSql & " VALUES("
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(strMultiValues.Replace("|", "','")) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lOBJ_VERSION) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lNewTextArray(0).ToString) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & "N'" & LTrimRTrim(lNewTextArray(1).ToString).Replace("'", "''") & "')"
                                                        lPack.Add(lNewInsertsSql)
                                                    End If
                                                Next
                                            ElseIf lblRequestType.Text = "Copy" AndAlso btnSubmit.CommandArgument = "New" AndAlso lSysStatus Then
                                                Dim strMultiValues As String = ""
                                                For iCnt As Integer = 0 To strSystems.Length - 1
                                                    strMultiValues = ""
                                                    If (strSystems(iCnt) <> "") Then
                                                        For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                                                            If lKey.Key_ID = "APPLICATION_ID" Then
                                                                strMultiValues = lMultiValues
                                                                strMultiValues = strMultiValues.Replace(lKey.CurrentValue, LTrimRTrim(strSystems(iCnt)))
                                                            End If
                                                        Next
                                                        Dim lNewInsertsSql As String = "INSERT INTO Customizing_" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "_TXT"
                                                        lNewInsertsSql = lNewInsertsSql & "(" & LTrimRTrim(lMultiKeys.Replace("|", ","))
                                                        lNewInsertsSql = lNewInsertsSql & ",[OBJ_VERSIONNUMBER]"
                                                        lNewInsertsSql = lNewInsertsSql & ",[OBJ_LANGUAGE_ISO_CODE]"
                                                        lNewInsertsSql = lNewInsertsSql & ",[OBJ_LANGUAGE_VALUE])"
                                                        lNewInsertsSql = lNewInsertsSql & " VALUES("
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(strMultiValues.Replace("|", "','")) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lOBJ_VERSION) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lNewTextArray(0).ToString) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & "N'" & LTrimRTrim(lNewTextArray(1).ToString).Replace("'", "''") & "')"
                                                        lPack.Add(lNewInsertsSql)
                                                    End If
                                                Next
                                            Else
                                                Dim lNewInsertsSql As String = "INSERT INTO Customizing_" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "_TXT"
                                                lNewInsertsSql = lNewInsertsSql & "(" & LTrimRTrim(lMultiKeys.Replace("|", ","))
                                                lNewInsertsSql = lNewInsertsSql & ",[OBJ_VERSIONNUMBER]"
                                                lNewInsertsSql = lNewInsertsSql & ",[OBJ_LANGUAGE_ISO_CODE]"
                                                lNewInsertsSql = lNewInsertsSql & ",[OBJ_LANGUAGE_VALUE])"
                                                lNewInsertsSql = lNewInsertsSql & " VALUES("
                                                lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lMultiValues.Replace("|", "','")) & "',"
                                                lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lOBJ_VERSION) & "',"
                                                lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lNewTextArray(0).ToString) & "',"
                                                lNewInsertsSql = lNewInsertsSql & "N'" & LTrimRTrim(lNewTextArray(1).ToString).Replace("'", "''") & "')"
                                                lPack.Add(lNewInsertsSql)
                                            End If
                                        End If
                                    End If
                                    '---------------------------------------------------------------------------------------
                                    ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                                    ' Comment   : New concept for multiple text functionality
                                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                                    ' Date      : 2015-03-24

                                    '' Reference : YHHR 2036565 - GBox: Single Quote Issue
                                    '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
                                    '' Added by  : Rajan Dmello (CWID : EOLRG) 
                                    '' Date      : 2018-10-30
                                    If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                                        Dim strMultiTextValue As String = ""
                                        Dim lstTexts As New List(Of String)
                                        Dim iItem As Integer
                                        For iItem = 1 To lNewTextArray.Length - 1
                                            If lNewTextArray(iItem).ToString.Contains("'") Then
                                                lstTexts.Add("N'" & LTrimRTrim(lNewTextArray(iItem).ToString).Replace("'", "''") & "'")
                                            Else
                                                lstTexts.Add("N'" & LTrimRTrim(lNewTextArray(iItem).ToString) & "'")
                                            End If
                                        Next
                                        strMultiTextValue = String.Join(",", lstTexts.ToArray)
                                        If lNewTextArray.Length - 1 = lstTexts.Count Then
                                            If lblRequestType.Text <> "Copy" AndAlso btnSubmit.CommandArgument = "New" AndAlso lSysStatus Then
                                                Dim strMultiValues As String = ""
                                                For iCnt As Integer = 0 To strSystems.Length - 1
                                                    strMultiValues = ""
                                                    If (strSystems(iCnt) <> "") Then
                                                        strMultiValues = lMultiValues
                                                        strMultiValues = LTrimRTrim(strSystems(iCnt)) & "|" & LTrimRTrim(strMultiValues)
                                                        Dim lNewInsertsSql As String = "INSERT INTO Customizing_" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "_TXT"
                                                        lNewInsertsSql = lNewInsertsSql & "(" & LTrimRTrim(lMultiKeys.Replace("|", ","))
                                                        lNewInsertsSql = lNewInsertsSql & ",[OBJ_VERSIONNUMBER]"
                                                        lNewInsertsSql = lNewInsertsSql & ",[OBJ_LANGUAGE_ISO_CODE]"
                                                        lNewInsertsSql = lNewInsertsSql & "," & LTrimRTrim(strResult) & ")"
                                                        lNewInsertsSql = lNewInsertsSql & " VALUES("
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(strMultiValues.Replace("|", "','")) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lOBJ_VERSION) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lNewTextArray(0).ToString) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & LTrimRTrim(strMultiTextValue) & ")"
                                                        lPack.Add(lNewInsertsSql)
                                                    End If
                                                Next
                                                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                                                ' Comment   : Text table entry for system dependent cust objects for copy functionality
                                                ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                                                ' Date      : 2018-10-16
                                            ElseIf lblRequestType.Text = "Copy" AndAlso btnSubmit.CommandArgument = "New" AndAlso lSysStatus Then
                                                Dim strMultiValues As String = ""
                                                For iCnt As Integer = 0 To strSystems.Length - 1
                                                    strMultiValues = ""
                                                    If (strSystems(iCnt) <> "") Then
                                                        For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                                                            If lKey.Key_ID = "APPLICATION_ID" Then
                                                                strMultiValues = lMultiValues
                                                                strMultiValues = strMultiValues.Replace(lKey.CurrentValue, LTrimRTrim(strSystems(iCnt)))
                                                            End If
                                                        Next
                                                        Dim lNewInsertsSql As String = "INSERT INTO Customizing_" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "_TXT"
                                                        lNewInsertsSql = lNewInsertsSql & "(" & LTrimRTrim(lMultiKeys.Replace("|", ","))
                                                        lNewInsertsSql = lNewInsertsSql & ",[OBJ_VERSIONNUMBER]"
                                                        lNewInsertsSql = lNewInsertsSql & ",[OBJ_LANGUAGE_ISO_CODE]"
                                                        lNewInsertsSql = lNewInsertsSql & "," & LTrimRTrim(strResult) & ")"
                                                        lNewInsertsSql = lNewInsertsSql & " VALUES("
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(strMultiValues.Replace("|", "','")) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lOBJ_VERSION) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lNewTextArray(0).ToString) & "',"
                                                        lNewInsertsSql = lNewInsertsSql & LTrimRTrim(strMultiTextValue) & ")"
                                                        lPack.Add(lNewInsertsSql)
                                                    End If
                                                Next
                                            Else
                                                Dim lNewInsertsSql As String = "INSERT INTO Customizing_" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "_TXT"
                                                lNewInsertsSql = lNewInsertsSql & "(" & LTrimRTrim(lMultiKeys.Replace("|", ","))
                                                lNewInsertsSql = lNewInsertsSql & ",[OBJ_VERSIONNUMBER]"
                                                lNewInsertsSql = lNewInsertsSql & ",[OBJ_LANGUAGE_ISO_CODE]"
                                                lNewInsertsSql = lNewInsertsSql & "," & LTrimRTrim(strResult) & ")"
                                                lNewInsertsSql = lNewInsertsSql & " VALUES("
                                                lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lMultiValues.Replace("|", "','")) & "',"
                                                lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lOBJ_VERSION) & "',"
                                                lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lNewTextArray(0).ToString) & "',"
                                                lNewInsertsSql = lNewInsertsSql & LTrimRTrim(strMultiTextValue) & ")"
                                                lPack.Add(lNewInsertsSql)
                                            End If

                                            'Dim lNewInsertsSql As String = "INSERT INTO Customizing_" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "_TXT"
                                            'lNewInsertsSql = lNewInsertsSql & "(" & LTrimRTrim(lMultiKeys.Replace("|", ","))
                                            'lNewInsertsSql = lNewInsertsSql & ",[OBJ_VERSIONNUMBER]"
                                            'lNewInsertsSql = lNewInsertsSql & ",[OBJ_LANGUAGE_ISO_CODE]"
                                            'lNewInsertsSql = lNewInsertsSql & "," & LTrimRTrim(strResult) & ")"
                                            'lNewInsertsSql = lNewInsertsSql & " VALUES("
                                            'lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lMultiValues.Replace("|", "','")) & "',"
                                            'lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lOBJ_VERSION) & "',"
                                            'lNewInsertsSql = lNewInsertsSql & "'" & LTrimRTrim(lNewTextArray(0).ToString) & "',"
                                            'lNewInsertsSql = lNewInsertsSql & LTrimRTrim(strMultiTextValue) & ")"
                                            'lPack.Add(lNewInsertsSql)
                                        End If
                                    End If
                                    ' Reference END : CR ZHHR 1054424
                                    '-------------------------------------------------------------------
                                    ' Reference END : CR ZHHR 1052471
                                    '--------------------------------------------------------------------------------
                                    ' Reference END : CR ZHHR 1038241
                                    '----------------------------------------------------------------------------------------
                                End If

                            End If
                        Next lNewItem
                    Next lOldItem
                    If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID <> "COMPOSITE_MULTI_TXT" Then
                        lstChangeFieldList.Item(0) = "<tr>" & lstChangeFieldList.Item(0).ToString & "</tr>"
                        lstChangeFieldList.Item(1) = "<tr>" & lstChangeFieldList.Item(1).ToString & "</tr>"
                    End If

                    Dim lFieldTexts As String = ""
                    For Each lField In lstChangeFieldList
                        lFieldTexts &= LTrimRTrim(lField.ToString) & vbCrLf
                    Next
                    lTextMailText = "<table border=""1"" cellpadding=""0"" cellspacing=""0""  style=""color:Black; font-size:11pt; font-family:Calibri;"">" & lFieldTexts & "</table>" & vbCrLf
                End If
                ' Reference END : CR ZHHR 1052471
                '--------------------------------------------------------------------------------
                'END SaveTexts  mUser.Current_Request_ID
                'Check If there is a third THIRD_PARTY_SYSTEM_KEY,THIRD_PARTY_SYSTEM_KEY_VALUE
                Dim lThirdParty As String = "not defined"
                Dim I As Long = 0
                For I = 0 To lFields.Count - 1
                    If lFields.Item(I) = "THIRD_PARTY_SYSTEM_KEY_VALUE" Then
                        If lValues.Item(I) <> "" Then
                            lThirdParty = LTrimRTrim(lValues.Item(I))
                        Else
                            lThirdParty = "not filled"
                        End If
                    End If
                Next

                'Begin
                For Each lst As ListItem In chkSYSTEMS.Items
                    If lst.Selected = True Then
                        ' Implementation of pMDAS DATA
                        Dim lConv As String = lst.Value.Split("(")(0).Trim
                        '----------------------------------------------------------------------------
                        ' Reference         : CR ZHHR 1037188 - GBOX COC: System information in email
                        ' Comment           : SYSTEMS information in mail content for the requestor
                        ' Added by          : Pratyusa Lenka (CWID : EOJCG)
                        ' Date              : 2015-02-05
                        '--------------------------------------------------------------------
                        ' Reference : OTT 1303 ZHHR 1048070 - GBOX: align workflow completion email
                        ' Comment   : Make system list
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-10-02
                        lSystemMailText.Add(lConv)
                        ' Reference END : CR ZHHR 1048070
                        ' Reference  END    : CR ZHHR 1037188
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
                        lSql &= " ('" & LTrimRTrim(mUser.Current_Request_ID) & "'"
                        lSql &= " ,'" & LTrimRTrim(lConv) & "'"
                        lSql &= " ,NULL" '<Target_Date, nvarchar(50),>"
                        lSql &= " ,NULL" '<Inform_by_Mail, nvarchar(50),>"
                        lSql &= " ,'UNKNOWN'" '<THIRD_PARTY_SYSTEM_KEY, nvarchar(50),>"
                        lSql &= " ,N'" & LTrimRTrim(lThirdParty) & "'"
                        lSql &= " ,Getdate()"
                        lSql &= " ,NULL)"
                        lPack.Add(lSql)
                    End If
                Next

                'and Systems END
                'Start Workflow SQLS
                'PREPARE INSERT DATA TO ASSEMBLILINE
                'SET ALL DATA STATUS TO 
                'ASSEMBLY_LINE_STATUS.QUEUED
                Dim lOrgLevel As String = ""
                Dim lOrgLevelValue As String = ""
                If cmborglevel.SelectedIndex >= 0 And cmborglevel.SelectedValue <> "Please Select ..." _
                    And cmborglevelvalue.SelectedIndex >= 0 And cmborglevelvalue.SelectedValue <> "Please Select ..." Then
                    lOrgLevel = LTrimRTrim(cmborglevel.SelectedValue)
                    lOrgLevelValue = LTrimRTrim(cmborglevelvalue.SelectedValue)
                End If

                '---------------------------------------------------------------------------------------------------
                ' Reference         : ZHHR 1040147 - GBOX COC: Add owner subgroup to SME Approval decision
                ' Comment           : For DRS request the SME approval should be taken from other groups too.
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 23-Mar-2015
                Dim mRankOne As Boolean = False
                Dim mSubGroup_Val As String = ""
                If (lValues.Count > 5) Then
                    Dim iCnt As Integer = 0
                    For iCnt = 0 To lFields.Count - 1
                        If (lFields.Item(iCnt).ToUpper = "SUBGROUP_ID") Then
                            mSubGroup_Val = LTrimRTrim(lValues.Item(iCnt))
                            lROrg_Level_Value = mSubGroup_Val.Trim
                            If (mSubGroup_Val = "ALL") Then
                                mSubGroup_Val = ""
                            End If
                            Exit For
                        End If
                    Next iCnt

                    If (mSubGroup_Val <> "") Then
                        If (lOrgLevelValue.Trim <> mSubGroup_Val.ToString.Trim) Then
                            lSql = "SELECT *,ROLE_CLUSTER_DEF.DESCRIPTION "
                            lSql = lSql & "FROM wf_DEFINE_WORKFLOW_DETAILS Left Join ROLE_CLUSTER_DEF on ROLE_CLUSTER_DEF.ROLE_CLUSTER_ID = wf_DEFINE_WORKFLOW_DETAILS.ROLE_CLUSTER_ID"
                            lSql = lSql & " Where OBJ_ID='" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "' AND Org_Level_Value='" & mSubGroup_Val.ToString.Trim & "'"
                            lSql = lSql & " AND RANK ='1'"
                            Dim dtr As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
                            Dim rowr As DataRow = Nothing

                            If dtr.Rows.Count = 0 Then mSubGroup_Val = ""

                            For Each rowr In dtr.Rows
                                lSql = MakeREQUEST_ACTIVE_ITEMInsertSQL(rowr)
                                lPack.Add(lSql)
                                mRankOne = True
                                '---------------------------------------------------------------------------------------------------
                                ' Reference         : ZHHR 1044222 - GBOX MGR OTT 1047: Send Email from Workflow
                                ' Comment           : For each workflow step, an email is sent to all relevant user, e.g. approver in an approver group.
                                ' Added by          : Milind Randive (CWID : EOJCH)
                                ' Date              : 26-Jun-2015
                                lSPack.Clear()
                                lSPack.AddRange(SendEmailToApproverGroup(LTrimRTrim(rowr("ORG_LEVEL_VALUE"))))
                                If lSPack.Count > 0 Then lPack.AddRange(lSPack)
                                ' Reference End        : ZHHR 1044222
                            Next
                        End If
                    End If
                End If

                'Reference End       : ZHHR 1040147

                ' SME: requested for...

                '---------------------------------------------------------------------------------------------------
                ' Reference         : CRT 2048726, GBOX_COC:Create multiple request for L1
                ' Comment           : Select workflow details only on rank 1 and ObjId and not on OrgLevelID or OrgLevelValue
                ' Date              : 12-Jul-2019
                lSql = "SELECT *,ROLE_CLUSTER_DEF.DESCRIPTION "
                lSql = lSql & "FROM wf_DEFINE_WORKFLOW_DETAILS Left Join ROLE_CLUSTER_DEF on ROLE_CLUSTER_DEF.ROLE_CLUSTER_ID = wf_DEFINE_WORKFLOW_DETAILS.ROLE_CLUSTER_ID"
                lSql = lSql & " Where OBJ_ID='" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "'"
                lSql = lSql & " AND RANK ='1'"
                '---------------------------------------------------------------------------------------------------
                Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
                Dim row As DataRow = Nothing
                Dim lRoleCluster As String = ""
                If dt.Rows.Count <> 0 Then
                    row = dt.Rows(0)
                    lRoleCluster = LTrimRTrim(row("DESCRIPTION")).ToString
                End If
                lSql = ""
                Dim lLastORG_LEVEL_ID As String = ""
                Dim lLastORG_LEVEL_VALUE As String = ""
                Dim lLastRANK As String = ""
                Dim lLastWORKFLOW_ID As String = ""
                Dim lLastSTATION_ID As String = ""
                'Über alle Iterieren die auf Rank 1 sind
                For Each row In dt.Rows

                    If (row("STATION_ID") = "SME" And (row("ORG_LEVEL_ID") <> lOrgLevel Or row("ORG_LEVEL_VALUE") <> lOrgLevelValue)) Then
                        'falls der erste Workflow step (rank 1) ein SME step ist, muss der ORG_LEVEL
                        ' = dem requested for ORG_ LEVEL sein und der ORG_LEVELVALUE = dem requested 
                        'for ORG_ LEVEL. Ist das nicht der Fall: nichts(machen)
                    Else
                        lSql = MakeREQUEST_ACTIVE_ITEMInsertSQL(row)

                        '---------------------------------------------------------------------------------------------------
                        ' Reference         : CRT 2048459, GBOX_COC : avoide duplicate insert query
                        ' Comment           : Check and avoid to insert same REQUEST_ACTIVE_ITEM
                        ' Date              : 04-Jul-2019
                        If (lPack.Contains(lSql) = False) Then
                            lPack.Add(lSql)
                        End If

                        mRankOne = True
                        lLastORG_LEVEL_ID = LTrimRTrim(row("ORG_LEVEL_ID"))
                        lLastORG_LEVEL_VALUE = LTrimRTrim(row("ORG_LEVEL_VALUE"))
                        lLastRANK = LTrimRTrim(row("RANK"))
                        lLastWORKFLOW_ID = LTrimRTrim(row("WORKFLOW_ID"))
                        lLastSTATION_ID = LTrimRTrim(row("STATION_ID"))

                        '---------------------------------------------------------------------------------------------------
                        ' Reference         : ZHHR 1044222 - GBOX MGR OTT 1047: Send Email from Workflow
                        ' Comment           : For each workflow step, an email is sent to all relevant user, e.g. approver in an approver group.
                        ' Added by          : Milind Randive (CWID : EOJCH)
                        ' Date              : 26-Jun-2015
                        lSPack.Clear()
                        lSPack.AddRange(SendEmailToApproverGroup(LTrimRTrim(row("ORG_LEVEL_VALUE"))))
                        If lSPack.Count > 0 Then lPack.AddRange(lSPack)
                        ' Reference End        : ZHHR 1044222

                    End If
                Next

                If dt.Rows.Count = 0 Then
                    lSql = "SELECT *,ROLE_CLUSTER_DEF.DESCRIPTION "
                    lSql = lSql & "FROM wf_DEFINE_WORKFLOW_DETAILS Left Join ROLE_CLUSTER_DEF on ROLE_CLUSTER_DEF.ROLE_CLUSTER_ID = wf_DEFINE_WORKFLOW_DETAILS.ROLE_CLUSTER_ID"
                    lSql = lSql & " Where OBJ_ID='" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "' and STATION_ID <> 'SME'"
                    lSql = lSql & " AND RANK ='1'"
                    dt = mUser.Databasemanager.MakeDataTable(lSql)
                    If dt.Rows.Count <> 0 Then
                        row = dt.Rows(0)
                        lSql = MakeREQUEST_ACTIVE_ITEMInsertSQL(row)

                        '---------------------------------------------------------------------------------------------------
                        ' Reference         : CRT 2048459, GBOX_COC : avoide duplicate insert query
                        ' Comment           : Check and avoid to insert same REQUEST_ACTIVE_ITEM
                        ' Date              : 04-Jul-2019
                        If (lPack.Contains(lSql) = False) Then
                            lPack.Add(lSql)
                        End If

                        mRankOne = True
                        lLastORG_LEVEL_ID = LTrimRTrim(row("ORG_LEVEL_ID"))
                        lLastORG_LEVEL_VALUE = LTrimRTrim(row("ORG_LEVEL_VALUE"))
                        lLastRANK = LTrimRTrim(row("RANK"))
                        lLastWORKFLOW_ID = LTrimRTrim(row("WORKFLOW_ID"))
                        lLastSTATION_ID = LTrimRTrim(row("STATION_ID"))

                        '---------------------------------------------------------------------------------------------------
                        ' Reference         : ZHHR 1044222 - GBOX MGR OTT 1047: Send Email from Workflow
                        ' Comment           : For each workflow step, an email is sent to all relevant user, e.g. approver in an approver group.
                        ' Added by          : Milind Randive (CWID : EOJCH)
                        ' Date              : 26-Jun-2015
                        lSPack.Clear()
                        lSPack.AddRange(SendEmailToApproverGroup(LTrimRTrim(row("ORG_LEVEL_VALUE"))))
                        If lSPack.Count > 0 Then lPack.AddRange(lSPack)
                        ' Reference End        : ZHHR 1044222

                    Else
                        lSql = ""
                    End If

                End If
                ' wenn lsql leer ist, war der erste step ein SME und nun muss der 2. Workflow step 
                ' gezogen werden.
                If lSql = "" And mRankOne = False Then ' mSubGroup_Val = ""
                    lSql = "SELECT *,ROLE_CLUSTER_DEF.DESCRIPTION "
                    lSql = lSql & "FROM wf_DEFINE_WORKFLOW_DETAILS Left Join ROLE_CLUSTER_DEF on ROLE_CLUSTER_DEF.ROLE_CLUSTER_ID = wf_DEFINE_WORKFLOW_DETAILS.ROLE_CLUSTER_ID"
                    lSql = lSql & " Where OBJ_ID='" & LTrimRTrim(mUser.Current_OBJ.OBJ_ID) & "'"
                    lSql = lSql & " AND RANK ='2'"
                    dt = mUser.Databasemanager.MakeDataTable(lSql)
                    If dt.Rows.Count = 0 Then
                        mDynamicFormController.ErrorInfo("Customize Workflow for Object:" & mUser.Current_OBJ.OBJ_ID)
                        Exit Sub
                    End If
                    row = dt.Rows(0)
                    lRoleCluster = LTrimRTrim(row("DESCRIPTION")).ToString
                    For Each row In dt.Rows
                        lSql = MakeREQUEST_ACTIVE_ITEMInsertSQL(row)

                        '---------------------------------------------------------------------------------------------------
                        ' Reference         : CRT 2048459, GBOX_COC : avoide duplicate insert query
                        ' Comment           : Check and avoid to insert same REQUEST_ACTIVE_ITEM
                        ' Date              : 04-Jul-2019
                        If (lPack.Contains(lSql) = False) Then
                            lPack.Add(lSql)
                        End If

                        lLastORG_LEVEL_ID = LTrimRTrim(row("ORG_LEVEL_ID"))
                        lLastORG_LEVEL_VALUE = LTrimRTrim(row("ORG_LEVEL_VALUE"))
                        lLastRANK = LTrimRTrim(row("RANK"))
                        lLastWORKFLOW_ID = LTrimRTrim(row("WORKFLOW_ID"))
                        lLastSTATION_ID = LTrimRTrim(row("STATION_ID"))

                        '---------------------------------------------------------------------------------------------------
                        ' Reference         : ZHHR 1044222 - GBOX MGR OTT 1047: Send Email from Workflow
                        ' Comment           : For each workflow step, an email is sent to all relevant user, e.g. approver in an approver group.
                        ' Added by          : Milind Randive (CWID : EOJCH)
                        ' Date              : 26-Jun-2015
                        lSPack.Clear()
                        lSPack.AddRange(SendEmailToApproverGroup(LTrimRTrim(row("ORG_LEVEL_VALUE"))))
                        If lSPack.Count > 0 Then lPack.AddRange(lSPack)
                        ' Reference End        : ZHHR 1044222
                    Next
                End If

                '--------------------------------------------------------------------------
                ' Reference : OTT 1303 ZHHR 1048070 - GBOX: align workflow completion email
                ' Comment   : Generate the mail content for the requester 
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-09-22
                Dim strWherestring As String = ""
                If mUser.RequestType = myUser.RequestTypeOption.insert Then
                    strWherestring = mDynamicFormController.GetWhereString(Request)
                Else
                    strWherestring = " where "
                    For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                        strWherestring = strWherestring & LTrimRTrim(lKey.Key_ID) & "='" & LTrimRTrim(lKey.CurrentValue) & "' AND "
                    Next
                    strWherestring = strWherestring.Substring(0, strWherestring.Length - 4)
                End If
                Dim strFields As String = String.Join(",", lFields.ToArray)
                Dim strSqlFieldValues As String = "SELECT " & strFields & " FROM " & mUser.Current_OBJ.OBJ_TABLENAME & strWherestring
                Dim dtFieldValues As DataTable = mUser.Databasemanager.MakeDataTable(strSqlFieldValues)
                Dim strFieldValue As String = ""

                Dim lRowIndex As Integer = 0
                Dim lstFieldList = New List(Of String)
                Dim j As Long = 0
                For j = 0 To lFields.Count - 1
                    If Not dtFieldValues Is Nothing And dtFieldValues.Rows.Count > 0 Then
                        For Each dcField As DataColumn In dtFieldValues.Columns
                            If lFields.Item(j).ToString = dcField.ColumnName Then
                                If lValues.Item(j).ToString <> dtFieldValues.Rows(0)(dcField.ColumnName).ToString Then
                                    strFieldValue = "<b>" & LTrimRTrim(lValues.Item(j).ToString) & "</b>"
                                    Exit For
                                Else
                                    strFieldValue = lValues.Item(j).ToString
                                    Exit For
                                End If
                            End If
                        Next
                    Else
                        strFieldValue = "<b>" & LTrimRTrim(lValues.Item(j).ToString) & "</b>"
                    End If
                    If lstFieldList.Count = 0 Then
                        lstFieldList.Insert(lRowIndex, "<td> " & LTrimRTrim(lFields.Item(j).ToString) & " </td>")
                        lstFieldList.Insert(lRowIndex + 1, "<td> " & strFieldValue & " </td>")
                    Else
                        lstFieldList.Item(lRowIndex) = LTrimRTrim(lstFieldList.Item(lRowIndex).ToString) & "<td> " & LTrimRTrim(lFields.Item(j).ToString) & " </td>"
                        lstFieldList.Item(lRowIndex + 1) = LTrimRTrim(lstFieldList.Item(lRowIndex + 1).ToString) & "<td> " & LTrimRTrim(strFieldValue) & " </td>"
                    End If
                Next
                lstFieldList.Item(lRowIndex) = "<tr>" & LTrimRTrim(lstFieldList.Item(lRowIndex).ToString) & "</tr>"
                lRowIndex = lRowIndex + 1
                lstFieldList.Item(lRowIndex) = "<tr>" & LTrimRTrim(lstFieldList.Item(lRowIndex).ToString) & "</tr>"

                Dim lFieldDetails As String = ""
                For Each lField In lstFieldList
                    lFieldDetails &= LTrimRTrim(lField.ToString) & vbCrLf
                Next

                Dim strRequestText As String = "<table border=""1"" cellpadding=""0"" cellspacing=""0""  style=""color:Black; font-size:11pt; font-family:Calibri;"">" & lFieldDetails & "</table>" & vbCrLf
                Dim objEmailMessage As Bayer.GBOX.FrameworkClassLibrary.EmailMessage = New Bayer.GBOX.FrameworkClassLibrary.EmailMessage
                objEmailMessage.ObjID = LTrimRTrim(mUser.Current_OBJ.OBJ_ID)
                objEmailMessage.ObjDescription = LTrimRTrim(mUser.Current_OBJ.OBJ_DESCRIPTION)
                objEmailMessage.MailKey = LTrimRTrim(mUser.Current_Request_ID)
                objEmailMessage.Requester = LTrimRTrim(mUser.first_name) & " " & LTrimRTrim(mUser.last_name) & " (" & LTrimRTrim(mUser.CW_ID) & ")"
                objEmailMessage.RequestStatus = "Requested"

                '' Reference : YHHR 2036565 - GBox: Single Quote Issue
                '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
                '' Added by  : Rajan Dmello (CWID : EOLRG) 
                '' Date      : 2018-10-30
                objEmailMessage.RequestComment = txtRequestComment.Text.Trim.Replace("'", "''")
                objEmailMessage.TargetSystems = lSystemMailText
                '-------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1052355 - GBOX COC: OTT 1809 - GBOX pMDAS workflow change
                ' Comment   : Get the customized PMDAS message to display in info email content
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2016-01-12
                objEmailMessage.CustomizedPMDASMessage = mUser.GBOXmanager.GetCustomizedPMDASMessage()
                ' Reference END : CR ZHHR 1052355
                '-------------------------------------------------------------------------------
                objEmailMessage.BodyBodyText = strRequestText & lTextMailText
                objEmailMessage.makeMailMessage()
                Dim lRequest_text As String = ""
                lRequest_text = LTrimRTrim(objEmailMessage.Body)
                ' Reference END : CR ZHHR 1048070
                '-------------------------------------------------------------------------------
                'Request Text

                '---------------------------------------------------------------------------------------------------
                ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
                ' Comment           : Remove title from database and code
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 2014-12-15

                'Dim lRequest_text As String = mUser.Current_Request_ID & ": this is a request for the object " & mUser.Current_OBJ.OBJ_ID & vbCrLf
                'lRequest_text = lRequest_text & mUser.first_name & " " & mUser.last_name & " (" & mUser.CW_ID & ") has requested the following change:" & vbCrLf

                ' Reference  END    : CR ZHHR 1035817
                '------------------------------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1041161 - GBOX COC: Workflow Changes
                ' Comment   : Add request comment to the mail content and make insert script to wf_REQUEST_STATUS table
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-04-17
                'lRequest_text = lRequest_text & vbCrLf & "TEXTS:" & vbCrLf & lTextMailText & vbCrLf & vbCrLf & "REQUEST COMMENT:" & vbCrLf & txtRequestComment.Text.Trim.Replace("'", "`") & vbCrLf & vbCrLf & "SYSTEMS:" & vbCrLf & lSystemMailText
                '' Reference : YHHR 2036565 - GBox: Single Quote Issue
                '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
                '' Added by  : Rajan Dmello (CWID : EOLRG) 
                '' Date      : 2018-10-30
                lSql = MakeREQUEST_STATUSInsertSQL(LTrimRTrim(mUser.Current_Request_ID), LTrimRTrim(lLastWORKFLOW_ID), LTrimRTrim(mUser.CW_ID), LTrimRTrim(lRequest_text.Replace("'", "''")), LTrimRTrim(mUser.Current_OBJ.OBJ_ID), LTrimRTrim(lOrgLevel), LTrimRTrim(lOrgLevelValue))
                lPack.Add(lSql)
                ' Reference END : CR ZHHR 1041161
                '-------------------------------------------------------------------------------------------------------
                Dim lSQLLock As String = "Update " & LTrimRTrim(mUser.Current_OBJ.OBJ_TABLENAME)
                lSQLLock = lSQLLock & " Set Locked = '" & LTrimRTrim(mUser.Current_Request_ID) & "'"
                lSQLLock = lSQLLock & " ,[CW_ID] = '" & LTrimRTrim(mUser.CW_ID) & "'"
                lSQLLock = lSQLLock & " ,[LOCKEDTIME] = Getdate()"
                'lSQLLock = lSQLLock & " where " & mUser.Current_OBJ.OBJ_TABLENAME_KEY & "='" & lblObj_VALUE.Text & "'"
                lSQLLock = lSQLLock & lWherestring
                lSQLLock = lSQLLock & " And OBJ_VERSIONNUMBER=" & CLng(lblVersionnumber.Text)
                lPack.Add(lSQLLock)
                Dim lDatabase As String = Me.mUser.Databasemanager.cnSQL.Database

                '-------------------------------------------------------------------
                ' Reference : CR ZHHR 1054424 - GBOX COC: problem special characters
                ' Comment   : Replace single quote with `
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2016-03-09
                '' Reference : YHHR 2036565 - GBox: Single Quote Issue
                '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
                '' Added by  : Rajan Dmello (CWID : EOLRG) 
                '' Date      : 2018-10-30

                Dim lSqlSendmail As String = "INSERT INTO M_MAILTRIGGER"
                lSqlSendmail &= "  (M_MAILKEY"
                lSqlSendmail &= "  ,M_RECIPIENTS"
                lSqlSendmail &= "   ,M_SUBJECT"
                lSqlSendmail &= "   ,M_BODY"
                lSqlSendmail &= "   ,M_CURRENT_SENDER)"
                lSqlSendmail &= "VALUES "
                lSqlSendmail &= "   ('" & LTrimRTrim(mUser.Current_Request_ID) & "',"
                lSqlSendmail &= "   '" & LTrimRTrim(mUser.SMTP_EMAIL) & "',"
                lSqlSendmail &= "   '" & LTrimRTrim(objEmailMessage.Subject) & "',"
                lSqlSendmail &= "   N'" & LTrimRTrim(lRequest_text.Replace("'", "''")) & "',"
                lSqlSendmail &= "   '" & LTrimRTrim(mUser.CW_ID) & "')"
                lPack.Add(lSqlSendmail)
                ' Reference END : CR ZHHR 1054424
                '-------------------------------------------------------------------
                '---------------------------------------------------------------------------------------------------
                ' Reference         : ZHHR 1043699 - GBOX COC OTT 980: Changes in DRS Workflow
                ' Comment           : If user is not authorised to change the sub group the previous sub group owner should be informed by email.
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 15-Jun-2015
                ' Revised Date      : 07-Jun-2017
                ' Comment           : Implemented Subgroup id check in customizing object table.

                Dim lOldSubGroup As String = ""
                Dim lSqlColumnExist = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '" & mUser.Current_OBJ.OBJ_TABLENAME & "' AND COLUMN_NAME = 'SUBGROUP_ID'"
                Dim dtColumnExist As DataTable = mUser.Databasemanager.MakeDataTable(lSqlColumnExist)

                If Not dtColumnExist Is Nothing Then
                    Dim lSQLOldSubGroup As String
                    lSQLOldSubGroup = "SELECT SUBGROUP_ID FROM " & mUser.Current_OBJ.OBJ_TABLENAME
                    lSQLOldSubGroup += lWherestring
                    lSQLOldSubGroup += " And OBJ_VERSIONNUMBER=" & CLng(lblVersionnumber.Text) - 1

                    Dim dtOldSubGroup As DataTable = mUser.Databasemanager.MakeDataTable(lSQLOldSubGroup)
                    If Not dtOldSubGroup Is Nothing Then
                        If dtOldSubGroup.Rows.Count > 0 Then
                            lOldSubGroup = LTrimRTrim(dtOldSubGroup.Rows.Item(0).Item("SUBGROUP_ID").ToString)
                        End If
                    End If
                End If

                If (lOldSubGroup.Trim <> "") Then
                    lSql = "SELECT USER_SOURCE FROM dp_Additional_Information WHERE TITLE = 'Approver teams'"
                    Dim dtAuth As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
                    If Not dtAuth Is Nothing Then
                        If dtAuth.Rows.Count > 0 Then
                            If (lOldSubGroup.Trim <> "" And lROrg_Level_Value.Trim <> "") Then
                                If (lOldSubGroup <> lROrg_Level_Value) Then

                                    lSql = LTrimRTrim(dtAuth.Rows.Item(0).Item("USER_SOURCE").ToString)
                                    lSql = lSql.Replace("|ORG_LEVEL_VALUE|", LTrimRTrim(lOldSubGroup))
                                    lSql += " AND  AUTHORISATION_SET.CW_ID= '" & LTrimRTrim(mUser.CW_ID) & "'"

                                    Dim dtUsrAuth As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
                                    If Not dtUsrAuth Is Nothing Then
                                        If dtUsrAuth.Rows.Count = 0 Then
                                            lSql = ""
                                            lSql = LTrimRTrim(dtAuth.Rows.Item(0).Item("USER_SOURCE").ToString)
                                            lSql = lSql.Replace("|ORG_LEVEL_VALUE|", LTrimRTrim(lOldSubGroup))
                                            Dim dtResult As DataTable = mUser.Databasemanager.MakeDataTable(lSql)

                                            If Not dtResult Is Nothing Then
                                                If dtResult.Rows.Count > 0 Then
                                                    Dim lUserEmail As String = ""
                                                    lRequest_text = "Dear DRS approver, " & vbCrLf & vbCrLf & "The Owner Subgroup assignment in a DRS value belonging to your subgroup/service company has been changed. "
                                                    lRequest_text += "Please check DRS request " & mUser.Current_Request_ID & " for object " & mUser.Current_OBJ.OBJ_ID & ". " & vbCrLf & vbCrLf & vbCrLf
                                                    lRequest_text += "DRS Administration Team"

                                                    For Each row In dtResult.Rows
                                                        lUserEmail = row("SMTP_EMAIL")
                                                        If lUserEmail.Trim <> "" Then
                                                            Dim lSqlApproverSendmail As String = "INSERT INTO M_MAILTRIGGER"
                                                            lSqlApproverSendmail &= "  (M_MAILKEY"
                                                            lSqlApproverSendmail &= "  ,M_RECIPIENTS"
                                                            lSqlApproverSendmail &= "   ,M_SUBJECT"
                                                            lSqlApproverSendmail &= "   ,M_BODY"
                                                            lSqlApproverSendmail &= "   ,M_CURRENT_SENDER)"
                                                            lSqlApproverSendmail &= "VALUES "
                                                            lSqlApproverSendmail &= "   ('" & LTrimRTrim(mUser.Current_Request_ID) & "',"
                                                            lSqlApproverSendmail &= "   '" & LTrimRTrim(lUserEmail) & "',"
                                                            lSqlApproverSendmail &= "   'Info: Your DRS value has been changed ',"
                                                            lSqlApproverSendmail &= "   N'" & LTrimRTrim(lRequest_text) & "',"
                                                            lSqlApproverSendmail &= "   '" & LTrimRTrim(mUser.CW_ID) & "')"
                                                            lPack.Add(lSqlApproverSendmail)
                                                        End If
                                                    Next
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
                ' Reference End     : ZHHR 1043699

                If Files.Count > 0 Then
                    For Each item As FileUpload In Files
                        Dim filename As String = item.FileName
                        Dim contentType As String = item.PostedFile.ContentType
                        Using fs As Stream = item.PostedFile.InputStream
                            Using br As New BinaryReader(fs)
                                Dim bytes As Byte() = br.ReadBytes(DirectCast(fs.Length, Long))
                                mUser.Databasemanager.ExecuteQuery(mUser.Current_Request_ID, filename, bytes) 'Changed the Code for CRT - 2047360 on 02-APR-2020
                            End Using
                        End Using
                    Next
                End If


                If mUser.Databasemanager.ExecutePackage(lPack) Then
                    lblStatus.Text = "The package is saved with GBOX_ID: " & mUser.Current_Request_ID
                Else
                    lblStatus.Text = "Current Error:" & mUser.Databasemanager.ErrText
                End If
                mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
                '---------------------------------------------------------------------
                ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
                ' Comment   : Placed new Add value button to create new request
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-12-24
                btnAddValue.Enabled = True
                ' Reference END : CR ZHHR 1050708
                '---------------------------------------------------------------------

                '---------------------------------------------------------------------------------------------------
                ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
                ' Comment           : System dependent workflow
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 23-Feb-2016
                btnCopyValue.Enabled = False
                ' Reference End     : ZHHR 1053017

                ChangeIcons()
            End With
        Catch ex As Exception
            mErrText &= ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        End Try
    End Sub

    Private Function StartWorkflow(ByVal lFields As List(Of String), ByVal lValues As List(Of String)) As List(Of String)
        Dim lLookForTemplate As String = "Select CLASSIFICATION_WORKFLOW_TEMPLATE_OBJ_ID from OBJ_CLASSIFICATION_DETAILS where OBJ_CLASSIFICATION_ID='" & mUser.Current_OBJ.OBJ_CLASSIFICATION_ID & "'"
        Dim lLookForTemplateDT As DataTable = mUser.Databasemanager.MakeDataTable(lLookForTemplate)
        If lLookForTemplateDT.Rows.Count = 0 Then
            lblStatus.Text = "CUSTOMIZE OBJ_CLASSIFICATION_ID for " & mUser.Current_OBJ.OBJ_CLASSIFICATION_ID
            Return Nothing
        End If
        Dim lTemplateObject As String = lLookForTemplateDT.Rows(0)("CLASSIFICATION_WORKFLOW_TEMPLATE_OBJ_ID").ToString

        'Start Workflow SQLS
        'PREPARE INSERT DATA TO ASSEMBLILINE
        'SET ALL DATA STATUS TO 
        'ASSEMBLY_LINE_STATUS.QUEUED
        Dim lPack As New List(Of String)
        Dim lSql As String = "SELECT *,ROLE_CLUSTER_DEF.DESCRIPTION "
        lSql = lSql & "FROM wf_DEFINE_WORKFLOW_DETAILS Left Join ROLE_CLUSTER_DEF on ROLE_CLUSTER_DEF.ROLE_CLUSTER_ID = wf_DEFINE_WORKFLOW_DETAILS.ROLE_CLUSTER_ID"
        lSql = lSql & " Where OBJ_ID='" & lTemplateObject & "'"
        lSql = lSql & " AND RANK ='1'"
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
        If dt.Rows.Count = 0 Then
            lblStatus.Text = ("Customize Workflow for Object:" & mUser.Current_OBJ.OBJ_ID)
            Return Nothing
        End If
        Dim row As DataRow = dt.Rows(0)
        Dim lRoleCluster As String = ""
        lRoleCluster = row("DESCRIPTION").ToString
        ' SME: requested for...
        Dim lOrgLevel As String = "SUBGROUP"
        Dim lOrgLevelValue As String = "ALL"
        If cmborglevel.SelectedIndex >= 0 And cmborglevel.SelectedValue <> "Please Select ..." _
            And cmborglevelvalue.SelectedIndex >= 0 And cmborglevelvalue.SelectedValue <> "Please Select ..." Then
            lOrgLevel = cmborglevel.SelectedValue
            lOrgLevelValue = cmborglevelvalue.SelectedValue
        End If

        Dim lLastORG_LEVEL_ID As String = ""
        Dim lLastORG_LEVEL_VALUE As String = ""
        Dim lLastRANK As String = ""
        Dim lLastWORKFLOW_ID As String = ""
        Dim lLastSTATION_ID As String = ""
        For Each row In dt.Rows
            lSql = ""
            If (row("STATION_ID") = "SME" And (row("ORG_LEVEL_ID") <> lOrgLevel Or row("ORG_LEVEL_VALUE") <> lOrgLevelValue)) Then
                'falls der erste Workflow step (rank 1) ein SME step ist, muss der ORG_LEVEL
                ' = dem requested for ORG_ LEVEL sein und der ORG_LEVELVALUE = dem requested 
                'for ORG_ LEVEL. Ist das nicht der Fall: nichts(machen)
            Else
                lSql = MakeREQUEST_ACTIVE_ITEMInsertSQL(row)

                '---------------------------------------------------------------------------------------------------
                ' Reference         : CRT 2048459, GBOX_COC : avoide duplicate insert query
                ' Comment           : Check and avoid to insert same REQUEST_ACTIVE_ITEM
                ' Date              : 04-Jul-2019
                If (lPack.Contains(lSql) = False) Then
                    lPack.Add(lSql)
                End If
                lLastORG_LEVEL_ID = row("ORG_LEVEL_ID")
                lLastORG_LEVEL_VALUE = row("ORG_LEVEL_VALUE")
                lLastRANK = row("RANK")
                lLastWORKFLOW_ID = row("WORKFLOW_ID")
                lLastSTATION_ID = row("STATION_ID")
            End If
        Next
        ' wenn lsql leer ist, war der erste step ein SME und nun muss der 2. Workflow step 
        ' gezogen werden.
        If lSql = "" Then
            lSql = "SELECT *,ROLE_CLUSTER_DEF.DESCRIPTION "
            lSql = lSql & "FROM wf_DEFINE_WORKFLOW_DETAILS Left Join ROLE_CLUSTER_DEF on ROLE_CLUSTER_DEF.ROLE_CLUSTER_ID = wf_DEFINE_WORKFLOW_DETAILS.ROLE_CLUSTER_ID"
            lSql = lSql & " Where OBJ_ID='" & lTemplateObject & "'"
            lSql = lSql & " AND RANK ='2'"
            dt = mUser.Databasemanager.MakeDataTable(lSql)
            If dt.Rows.Count = 0 Then
                mDynamicFormController.ErrorInfo("Customize Workflow for Object:" & mUser.Current_OBJ.OBJ_ID)
                Return Nothing
            End If
            row = dt.Rows(0)
            lRoleCluster = row("DESCRIPTION").ToString
            For Each row In dt.Rows
                lSql = MakeREQUEST_ACTIVE_ITEMInsertSQL(row)
                '---------------------------------------------------------------------------------------------------
                ' Reference         : CRT 2048459, GBOX_COC : avoide duplicate insert query
                ' Comment           : Check and avoid to insert same REQUEST_ACTIVE_ITEM
                ' Date              : 04-Jul-2019
                If (lPack.Contains(lSql) = False) Then
                    lPack.Add(lSql)
                End If

                lLastORG_LEVEL_ID = row("ORG_LEVEL_ID")
                lLastORG_LEVEL_VALUE = row("ORG_LEVEL_VALUE")
                lLastRANK = row("RANK")
                lLastWORKFLOW_ID = row("WORKFLOW_ID")
                lLastSTATION_ID = row("STATION_ID")
            Next
        End If
        '--------------------------------------------------------------------------
        ' Reference : OTT 1303 ZHHR 1048070 - GBOX: align workflow completion email
        ' Comment   : Info email content for CPS object
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2015-10-02
        Dim strFields As String = String.Join(",", lFields.ToArray)
        Dim strSqlFieldValues As String = "SELECT " & strFields & " FROM OBJ_CPS WHERE [OBJ_CPS_ID] = '" & txtCustomizingObjName.Text.Trim & "'"
        Dim dtFieldValues As DataTable = mUser.Databasemanager.MakeDataTable(strSqlFieldValues)
        Dim strFieldValue As String = ""
        Dim lRowIndex As Integer = 0
        Dim lstFieldList = New List(Of String)
        Dim j As Long = 0
        For j = 0 To lFields.Count - 1
            If Not dtFieldValues Is Nothing And dtFieldValues.Rows.Count > 0 Then
                For Each dcField As DataColumn In dtFieldValues.Columns
                    If lFields.Item(j).ToString = dcField.ColumnName Then
                        If lValues.Item(j).ToString <> dtFieldValues.Rows(0)(dcField.ColumnName).ToString Then
                            strFieldValue = "<b>" & lValues.Item(j).ToString & "</b>"
                            Exit For
                        Else
                            strFieldValue = lValues.Item(j).ToString
                            Exit For
                        End If
                    End If
                Next
            Else
                strFieldValue = "<b>" & lValues.Item(j).ToString & "</b>"
            End If
            If lstFieldList.Count = 0 Then
                lstFieldList.Insert(lRowIndex, "<td> " & lFields.Item(j).ToString & " </td>")
                lstFieldList.Insert(lRowIndex + 1, "<td> " & strFieldValue & " </td>")
            Else
                lstFieldList.Item(lRowIndex) = lstFieldList.Item(lRowIndex).ToString & "<td> " & lFields.Item(j).ToString & " </td>"
                lstFieldList.Item(lRowIndex + 1) = lstFieldList.Item(lRowIndex + 1).ToString & "<td> " & strFieldValue & " </td>"
            End If
        Next
        lstFieldList.Item(lRowIndex) = "<tr>" & lstFieldList.Item(lRowIndex).ToString & "</tr>"
        lRowIndex = lRowIndex + 1
        lstFieldList.Item(lRowIndex) = "<tr>" & lstFieldList.Item(lRowIndex).ToString & "</tr>"

        Dim lFieldDetails As String = ""
        For Each lField In lstFieldList
            lFieldDetails &= lField.ToString & vbCrLf
        Next
        Dim strRequestText As String = "<table border=""1"" cellpadding=""0"" cellspacing=""0""  style=""color:Black; font-size:11pt; font-family:Calibri;"">" & lFieldDetails & "</table>" & vbCrLf

        Dim objEMailMessage As Bayer.GBOX.FrameworkClassLibrary.EmailMessage = New Bayer.GBOX.FrameworkClassLibrary.EmailMessage
        objEMailMessage.ObjID = mUser.Current_OBJ.OBJ_ID
        objEMailMessage.ObjDescription = mUser.Current_OBJ.OBJ_DESCRIPTION
        objEMailMessage.MailKey = mUser.Current_Request_ID
        objEMailMessage.Requester = mUser.first_name & " " & mUser.last_name & " (" & mUser.CW_ID & ")"
        objEMailMessage.RequestStatus = "Requested"

        '' Reference : YHHR 2036565 - GBox: Single Quote Issue
        '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
        '' Added by  : Rajan Dmello (CWID : EOLRG) 
        '' Date      : 2018-10-30
        objEMailMessage.RequestComment = txtRequestComment.Text.Trim.Replace("'", "''")
        objEMailMessage.BodyBodyText = strRequestText
        objEMailMessage.makeMailMessage()
        Dim lRequest_text As String = ""
        lRequest_text = objEMailMessage.Body
        Dim strSubject As String = objEMailMessage.Subject

        'Request Text

        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR ZHHR 1035817 - GBOX WebForms: REMOVE, ADD FIELDS, ACTIVATE FIELD FROM DB AND CODE
        ' Comment           : Remove title from database and code
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2014-12-15

        'Dim lRequest_text As String = mUser.Current_Request_ID & ": this is a request for the object " & mUser.Current_OBJ.OBJ_ID & vbCrLf
        'lRequest_text = lRequest_text & mUser.first_name & " " & mUser.last_name & " (" & mUser.CW_ID & ") has requested the following change:" & vbCrLf

        ' Reference  END    : CR ZHHR 1035817
        '-----------------------------------------------------------
        ' Reference : CR ZHHR 1041161 - GBOX COC: Workflow Changes
        ' Comment   : Make insert script to wf_REQUEST_STATUS table
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2015-04-17
        lSql = MakeREQUEST_STATUSInsertSQL(mUser.Current_Request_ID, lLastWORKFLOW_ID, mUser.CW_ID, lRequest_text, lTemplateObject, lOrgLevel, lOrgLevelValue)
        lPack.Add(lSql)
        ' Reference END : CR ZHHR 1041161
        '----------------------------------------------------------
        Dim lSqlSendmail As String = "INSERT INTO M_MAILTRIGGER"
        lSqlSendmail &= "  (M_MAILKEY"
        lSqlSendmail &= "  ,M_RECIPIENTS"
        lSqlSendmail &= "   ,M_SUBJECT"
        lSqlSendmail &= "   ,M_BODY"
        lSqlSendmail &= "   ,M_CURRENT_SENDER)"
        lSqlSendmail &= "VALUES "
        lSqlSendmail &= "   ('" & mUser.Current_Request_ID & "',"
        lSqlSendmail &= "   '" & mUser.SMTP_EMAIL & "',"
        lSqlSendmail &= "   '" & strSubject & "',"
        lSqlSendmail &= "   N'" & lRequest_text & "',"
        lSqlSendmail &= "   '" & mUser.CW_ID & "')"
        lPack.Add(lSqlSendmail)
        Return lPack
        ' Reference END : CR ZHHR 1048070
        '------------------------------------------------------------------------------------
    End Function
    ''' <summary>
    ''' Reference  : CR ZHHR 1041161 - GBOX COC: Workflow Changes
    ''' Updated by : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date       : 2015-04-16
    ''' </summary>
    ''' <param name="lREQUEST_ID"></param>
    ''' <param name="lWORKFLOW_ID"></param>
    ''' <param name="lCW_ID"></param>
    ''' <param name="strRequestText"></param>
    ''' <param name="strObjID"></param>
    ''' <param name="strOrgLevel"></param>
    ''' <param name="strOrgLevelValue"></param>
    ''' <returns>Insert SQL script for wf_REQUEST_STATUS table</returns>
    ''' <remarks>Updated the insert query to take REQUEST_TEXT, OBJ_ID, REQUEST_ORG_LEVEL, REQUEST_ORG_LEVEL_VALUE and REQUEST_COMMENT values</remarks>
    Private Function MakeREQUEST_STATUSInsertSQL(ByVal lREQUEST_ID As String, ByVal lWORKFLOW_ID As String, ByVal lCW_ID As String, ByVal strRequestText As String, ByVal strObjID As String, ByVal strOrgLevel As String, ByVal strOrgLevelValue As String) As String
        Dim lSql As String = "INSERT INTO wf_REQUEST_STATUS "
        lSql = lSql & " (REQUEST_ID "
        lSql = lSql & " ,CW_ID_REQUESTER "
        lSql = lSql & " ,WORKFLOW_ID "
        lSql = lSql & " ,START_TIMESTAMP "
        lSql = lSql & " ,STEP_TIMESTAMP "
        lSql = lSql & " ,CW_ID_CURRENT_RESPONSIBLE "
        lSql = lSql & " ,REQUEST_STATUS_ID "
        lSql = lSql & " ,REQUEST_TEXT "
        lSql = lSql & " ,OBJ_ID "
        lSql = lSql & " ,REQUEST_ORG_LEVEL "
        lSql = lSql & " ,REQUEST_ORG_LEVEL_VALUE "
        lSql = lSql & " ,REQUEST_COMMENT)"
        lSql = lSql & " VALUES "
        lSql = lSql & " ('" & lREQUEST_ID & "' "
        lSql = lSql & " ,'" & lCW_ID & "'"
        lSql = lSql & " ,'" & lWORKFLOW_ID & "'"
        lSql = lSql & " ,GETDATE()"
        lSql = lSql & " ,GETDATE()"
        lSql = lSql & " ,'" & lCW_ID & "'"
        lSql = lSql & " ,'REQUESTED'"
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1047686- GBOX COC: Cannot create DRS request
        ' Comment           : Added replace function below
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 28-Aug-2015
        '' Reference : YHHR 2036565 - GBox: Single Quote Issue
        '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
        '' Added by  : Rajan Dmello (CWID : EOLRG) 
        '' Date      : 2018-10-30

        lSql = lSql & " , N'" & strRequestText.Replace("'", "''") & "' "
        ' Reference End     : ZHHR 1047686

        lSql = lSql & " ,'" & strObjID & "' "
        lSql = lSql & " ,'" & strOrgLevel & "' "
        lSql = lSql & " ,'" & strOrgLevelValue & "' "
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1047686- GBOX COC: Cannot create DRS request
        ' Comment           : Added replace function below
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 28-Aug-2015

        '' Reference : YHHR 2036565 - GBox: Single Quote Issue
        '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
        '' Added by  : Rajan Dmello (CWID : EOLRG) 
        '' Date      : 2018-10-30

        lSql = lSql & " , N'" & txtRequestComment.Text.Trim.Replace("'", "''") & "')"
        ' Reference End     : ZHHR 1047686
        Return lSql
    End Function


    Private Function MakeREQUEST_ACTIVE_ITEMInsertSQL(ByVal row As DataRow) As String
        Dim lUser As String = ""
        If InStr(My.User.Name, "\") <> 0 Then
            lUser = My.User.Name.Split("\")(1)
        Else
            lUser = My.User.Name
        End If
        Dim lSql As String = "INSERT INTO wf_REQUEST_ACTIVE_ITEM "
        lSql = lSql & " (REQUEST_ID "
        lSql = lSql & " ,ORG_LEVEL_ID "
        lSql = lSql & " ,ORG_LEVEL_VALUE "
        lSql = lSql & " ,RANK "
        lSql = lSql & " ,WORKFLOW_ID "
        lSql = lSql & " ,STATION_ID "
        lSql = lSql & " ,ITEM_STATUS "
        lSql = lSql & " ,ROLE_CLUSTER_ID "
        lSql = lSql & " ,REQUEST_STATUS_ID "
        lSql = lSql & " ,Timestamp "
        lSql = lSql & " ,CW_ID_CURRENT_RESPONSIBLE "
        lSql = lSql & " ,REQUESTER) "
        lSql = lSql & " VALUES "
        lSql = lSql & " ('" & mUser.Current_Request_ID & "' "
        lSql = lSql & " ,'" & row("ORG_LEVEL_ID") & "' "
        lSql = lSql & " ,'" & row("ORG_LEVEL_VALUE") & "' "
        lSql = lSql & " ,'" & row("RANK") & "' "
        lSql = lSql & " ,'" & row("WORKFLOW_ID") & "' "
        lSql = lSql & " ,'" & row("STATION_ID") & "' "
        lSql = lSql & " ,'" & "QUEUED" & "' "
        lSql = lSql & " ,'" & row("ROLE_CLUSTER_ID") & "' "
        lSql = lSql & " ,'" & "REQUESTED" & "' "
        lSql = lSql & " ,getdate() "
        lSql = lSql & " ,'" & lUser & "'"
        lSql = lSql & " ,'" & lUser & "')"
        Return lSql
    End Function



    Private Function CheckTextLenght()
        lstNewTexts.Items.Clear()
        Dim lCheck As Boolean = False
        For X = 1 To tblTexts.Rows.Count - 1
            Dim lCont As String = Right("0000" & X, 2)
            Dim myLang_XX_VALUE As TextBox = Me.FindControl("txt_Edit_Lang_" & lCont)
            If Len(myLang_XX_VALUE.Text) > CLng(lblTextlenght.Text) Then
                myLang_XX_VALUE.BackColor = Drawing.Color.Red
                lCheck = False
                Exit For
            Else
                lCheck = True
                myLang_XX_VALUE.BackColor = Drawing.Color.LightGreen

            End If
            Dim myLang_XX_Lable As Label = Me.FindControl("lbl_Lang_" & lCont & "_Code")
            lstNewTexts.Items.Add(myLang_XX_Lable.Text & "|" & myLang_XX_VALUE.Text)
            '------------------------------------------------------------------------------
            'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
            'Comment    : German translation should be mandatory when handling texts
            'Added by   : Pratyusa Lenka (CWID : EOJCG)
            'Date       : 2016-02-16
            If (myLang_XX_Lable.Text = "EN" Or myLang_XX_Lable.Text = "DE") And myLang_XX_VALUE.Text.Trim = "" Then
                myLang_XX_Lable.Text = myLang_XX_Lable.Text & "(required)"
                myLang_XX_VALUE.BackColor = Drawing.Color.Red
                lCheck = False
                Exit For
            Else
                If lCheck Then
                    myLang_XX_VALUE.BackColor = Drawing.Color.LightGreen
                End If
            End If
            ' Reference END : CR ZHHR 1053558
            '------------------------------------------------------------------------------
        Next X

        Return lCheck
    End Function
    ''' <summary>
    ''' Reference : Issue reported edit text functionality not working with different column names
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date      : 2015-12-28
    ''' </summary>
    ''' <param name="dtTextLength"></param>
    ''' <param name="strFieldName"></param>
    ''' <returns>True, if the field name exists</returns>
    ''' <remarks>Check the field name exists in text length data table</remarks>
    Private Function IsFieldExists(dtTextLength As DataTable, strFieldName As String) As Boolean
        Dim bIsFieldExists As Boolean = False
        For Each drRow As DataRow In dtTextLength.Rows
            If drRow("OBJ_COLUMN_ID").ToString.ToUpper() = strFieldName.ToUpper() Then
                bIsFieldExists = True
                Exit For
            End If
        Next
        Return bIsFieldExists
    End Function
    ''' <summary>
    ''' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
    ''' </summary>
    ''' <returns>True if texts valid</returns>
    ''' <remarks>New concept for multiple text functionality</remarks>
    Private Function ValidateText()
        lstNewTexts.Items.Clear()
        Dim lCheck As Boolean = False
        Dim IsValid As Boolean = True
        Dim strSQLTextLength As String = "Select OBJ_COLUMN_ID,TEXT_LENGTH from OBJ_TEXTS_LENGTHS where OBJ_ID ='" & mUser.Current_OBJ.OBJ_ID & "'"
        Dim dtTextLength As DataTable = mUser.Databasemanager.MakeDataTable(strSQLTextLength)

        For i = 1 To tblTexts.Rows.Count - 1
            Dim lRowCount As String = Right("0000" & i, 2)
            Dim lstNewMultiTexts As New List(Of String)
            Dim strNewMultiTexts As String = String.Empty
            Dim lblLanguageValue As Label = Nothing
            For j = 0 To dtTextLength.Rows.Count - 1
                Dim drTextLength As DataRow = dtTextLength.Rows(j)
                Dim txtLanguageValue As TextBox = Me.FindControl("txt_Edit_Multi_Lang_" & lRowCount & "Col_" & j + 3)
                '---------------------------------------------------------------------
                ' Reference : Issue reported edit text functionality not working with different column names
                ' Comment   : Check the field name exists in text length data table
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-12-28
                'If Not txtLanguageValue Is Nothing And tblTexts.Rows(0).Cells(j + 2).Text = drTextLength("OBJ_COLUMN_ID") And Not String.IsNullOrEmpty(drTextLength("TEXT_LENGTH").ToString) Then
                If Not txtLanguageValue Is Nothing And IsFieldExists(dtTextLength, tblTexts.Rows(0).Cells(j + 2).Text) And Not String.IsNullOrEmpty(drTextLength("TEXT_LENGTH").ToString) Then
                    If Len(txtLanguageValue.Text.Trim) > CLng(drTextLength("TEXT_LENGTH")) Then
                        txtLanguageValue.BackColor = Drawing.Color.Red
                        lCheck = False
                        IsValid = False
                        Exit For
                    Else
                        lCheck = True
                        txtLanguageValue.BackColor = Drawing.Color.LightGreen
                    End If
                    lblLanguageValue = Me.FindControl("lbl_Lang_" & lRowCount & "_Code")
                    lstNewMultiTexts.Add(txtLanguageValue.Text)
                    strNewMultiTexts = String.Join("|", lstNewMultiTexts.ToArray())
                    '------------------------------------------------------------------------------
                    'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
                    'Comment    : German translation should be mandatory when handling texts
                    'Added by   : Pratyusa Lenka (CWID : EOJCG)
                    'Date       : 2016-02-16
                    If (lblLanguageValue.Text = "EN" Or lblLanguageValue.Text = "DE") And String.IsNullOrEmpty(txtLanguageValue.Text.Trim) Then
                        lblLanguageValue.Text = lblLanguageValue.Text & "(required)"
                        txtLanguageValue.BackColor = Drawing.Color.Red
                        lCheck = False
                        IsValid = False
                        Exit For
                    Else
                        If lCheck Then
                            txtLanguageValue.BackColor = Drawing.Color.LightGreen

                        End If
                    End If
                    ' Reference END : CR ZHHR 1053558
                    '------------------------------------------------------------------------------
                End If
            Next
            If Not lblLanguageValue Is Nothing Then
                lstNewTexts.Items.Add(lblLanguageValue.Text & "|" & strNewMultiTexts)
            End If
            If Not IsValid Then
                Exit For
            End If
        Next
        Return lCheck
    End Function
#End Region
#Region "Subs And Functions - Navigate & Co"
    Private Function InitMe() As Boolean
        Dim lSuccess As Boolean = True
        If InStr(lblStatus.Text, "The package is saved with GBOX_ID:") = 0 And InStr(lblStatus.Text, "Current Error:") = 0 Then
            lblStatus.Text = ""
        End If

        mWithPaging = chkPaging.Checked
        chkPaging.Visible = False
        rdFilter.Visible = False
        lblFilter.Visible = False
        lblFilter.Visible = False
        Authenticate_User()
        mUser.GBOXmanager.User = mUser
        If mUser Is Nothing Then
            lSuccess = False
        End If
        WelcomeWagon()
        mUser.GBOXmanager.LinkServer = pConstServername
        If mUser.GBOXmanager.CHECK_MDRS_DATABASE_MAINTENANCE() Then
            Dim lText As String = "G|Box System Access  is currently locked due to maintenance "
            lText = lText & vbCrLf & mUser.GBOXmanager.Get_MDRS_DATABASE_MAINTENANCE_STRING
            lblStatus.Text = lText
            lSuccess = False
        End If
        lblSQlReport.Text = ""
        If Not Me.IsPostBack Then
            If Not MakeTopicGroupMenu() Then
                lSuccess = False
            End If
            LoadWizardData("INIT")
        End If

        Return lSuccess
    End Function
    Private Function OverrideStatusDueUrlParams(ByVal lStatusSet As ViewStatus) As ViewStatus
        If Not Request.Params("PATH") Is Nothing Then
            lStatusSet = ViewStatus.NodeClick
            mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
        End If
        Return lStatusSet
    End Function
    Private Function SetCurrentObject(ByVal lViewStatus As ViewStatus) As String
        Dim lCurrentObj As String = ""
        Select Case lViewStatus
            Case ViewStatus.Detailsmenu
                Dim lCurrentObjArr As Array = Me.Request.Form("__EVENTARGUMENT").ToString.Split("\")
                lCurrentObj = lCurrentObjArr(lCurrentObjArr.GetUpperBound(0))
            Case ViewStatus.NodeClick, ViewStatus.TreeClick
                Dim lCheckString As String = ""
                If Me.Request.Form("__EVENTARGUMENT") Is Nothing Then
                    If Not trvOBJ.SelectedNode Is Nothing Then
                        lCheckString = trvOBJ.SelectedNode.ValuePath.Replace("/", "\")
                    End If
                Else
                    lCheckString = Me.Request.Form("__EVENTARGUMENT").ToString
                    If lCheckString = "0" Or lCheckString = "" Then
                        If Not trvOBJ.SelectedNode Is Nothing Then
                            lCheckString = trvOBJ.SelectedNode.ValuePath.Replace("/", "\")
                        End If
                    End If
                    If InStr(Me.Request.Form("__EVENTARGUMENT").ToString, "Page") <> 0 Then
                        lCheckString = trvOBJ.SelectedNode.ValuePath.Replace("/", "\")
                    End If

                End If
                If Left(lCheckString, 1) = "s" Then
                    lCheckString = lCheckString.Substring(1, lCheckString.Length - 1)
                End If
                Dim lCurrentObjArr As Array = lCheckString.Split("\")
                lCurrentObj = lCurrentObjArr(lCurrentObjArr.GetUpperBound(0))
            Case ViewStatus.ImageClick
                If Not trvOBJ.SelectedNode Is Nothing Then
                    lCurrentObj = trvOBJ.SelectedNode.Value
                End If
            Case ViewStatus.MainMenu
                If Not mnuNavigate.SelectedItem Is Nothing Then lCurrentObj = mnuNavigate.SelectedItem.Text
            Case ViewStatus.Detailsmenu
                If Not mnuDetailsMenu.SelectedItem Is Nothing Then lCurrentObj = mnuDetailsMenu.SelectedItem.Text
            Case ViewStatus.SelectLink
                lCurrentObj = trvOBJ.SelectedNode.Value
                '---------------------------------------------------------------------
                ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
                ' Comment   : Set current object for checkbox/button click
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-12-24
            Case ViewStatus.GridCheckBoxClick
                lCurrentObj = trvOBJ.SelectedNode.Value
            Case ViewStatus.ButtonClick
                lCurrentObj = trvOBJ.SelectedNode.Value
                ' Reference END : CR ZHHR 1050708
                '---------------------------------------------------------------------
        End Select
        mUser.GBOXmanager.SetCurrentObj(lCurrentObj, mUser)
        Return lCurrentObj
    End Function
    Private Sub ReleaseFilter()
        txtFiltertext.Text = ""
        Dim lparamlist As String = GetParamList(True)
        Me.Response.Redirect("~" & Me.Request.FilePath & lparamlist)
    End Sub
    Private Sub Search()
        Dim lSearch As New mySearchEngine
        With lSearch
            .Request = Me.Request
            .Context = Me.Context
            .Linkserver = mUser.GBOXmanager.LinkServer
            If chkDrsSettings.Checked = False Then
                .Viewname = "vw_Search_Engine"
            Else
                .Viewname = "vw_Search_Engine_SETTINGS_ONLY"
            End If
            .Authenticate_User()
        End With
        lSearch.Search(txtKeywords.Text)
        If lSearch.SearchResult.Count = 0 Then
            Dim ctl As New HyperLink
            Dim lTablerow As New TableRow
            Dim lTableCell As New TableCell
            ctl.ID = "Search"
            ctl.Target = "_search"
            ctl.Text = "No data found"
            ctl.Visible = True
            lTableCell = New TableCell
            lTableCell.Controls.Add(ctl)
            lTablerow.Cells.Add(lTableCell)
            myDynamicTable.Rows.Add(lTablerow)
        End If
        myDynamicTable.BorderStyle = BorderStyle.Dotted
        Dim i As Long = 0
        For Each r As SearchResult In lSearch.SearchResult
            Dim ctl As New HyperLink
            Dim lTablerow As New TableRow
            i = i + 1
            If i Mod 2 = 0 Then
                lTablerow.BackColor = Drawing.Color.LightBlue
            Else
                lTablerow.BackColor = Drawing.Color.LightGray
            End If
            Dim lTableCell As New TableCell
            ctl.NavigateUrl = r.NavigateUrl
            ctl.ID = "Search" & i
            ctl.Target = "_search"
            ctl.Text = r.ResultText
            ctl.Visible = True
            lTableCell = New TableCell
            lTableCell.Controls.Add(ctl)
            lTablerow.Cells.Add(lTableCell)
            myDynamicTable.Rows.Add(lTablerow)
            mPath = ""
        Next r
    End Sub
    Private Function IsLocked() As Boolean
        Dim lLockedSql As String = "Select Locked from OBJ_CPS_ATTR where [OBJ_GUID]='" & mUser.Current_OBJ.OBJ_ID & "' AND Locked is not null"
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lLockedSql)
        If ldt.Rows.Count <> 0 Then
            lblStatus.Text = "The Request is locked by request id: " & ldt.Rows(0)("LOCKED").ToString
            lblStatus.ForeColor = Drawing.Color.Red
            lblStatus.Font.Bold = True
            Return True
        Else
            lblStatus.Text = ""
            Return False
        End If
    End Function
    Private Function LetTheNodeBeClicked() As Boolean
        Try
            Dim lReturnAbort As Boolean = False
            lblToMany.Text = ""
            LoadFactoryAndControllerAndBind(mWithPaging)
            LoadWizardData(mUser.Current_OBJ.OBJ_ID)
            ' Reference : YHHR 2050174 - GBOX COC: Run report by pressing enter
            ' Comment   : Modified the informative message as required by this change
            ' Added by  : Pratyusa Lenka (CWID : EOJCG)
            ' Date      : 2019-08-19
            lblToMany.Text = "To show data, please select tree node and press Enter/Run report button!"
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1027410 - GBOX COC - Button missing in G-Box query 
            ' Comment           : Make visible the G-Box query button to run report
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-04-30
            '---------------------------------------------------------------------------------------------------
            'imgQuery.Visible = True

            If mUser.Current_OBJ.DATABASE_OBJ_Classification_ID = "View" Or mUser.Current_OBJ.DATABASE_OBJ_Classification_ID = "ParameterFreeView" Then

                '-----------------------------------------------------------------------
                ' Reference: CR - BY-RZ04-CMT-28707 - 110IM08174433 -  Load_Data_At_Startup
                ' Comment:   Show the details in the grid if Load_Data_At_Startup flag is true
                ' Added by:  Surendra Purav (CWID : EQIZU)
                '-----------------------------------------------------------------------
                If (mUser.Current_OBJ.Load_Data_At_Startup) Then
                    mDynamicFormController.ShowQuery(grdQuery)
                End If
                lReturnAbort = True
                mvContents.ActiveViewIndex = GetIndexByViewName("vwQuery")
                Return lReturnAbort
            End If
            If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID.Contains("CPS") Then
                Dim lLookForTemplate As String = "Select CLASSIFICATION_VIEW_TEMPLATE_OBJ_ID from OBJ_CLASSIFICATION_DETAILS where OBJ_CLASSIFICATION_ID='" & mUser.Current_OBJ.OBJ_CLASSIFICATION_ID & "'"
                Dim lLookForTemplateDT As DataTable = mUser.Databasemanager.MakeDataTable(lLookForTemplate)
                If lLookForTemplateDT.Rows.Count = 0 Then
                    lblStatus.Text = "CUSTOMIZE OBJ_CLASSIFICATION_ID for " & mUser.Current_OBJ.OBJ_CLASSIFICATION_ID
                    lReturnAbort = True
                    Return lReturnAbort
                End If
                Dim lTemplateObject As String = lLookForTemplateDT.Rows(0)("CLASSIFICATION_VIEW_TEMPLATE_OBJ_ID").ToString
                Select Case lTemplateObject
                    Case "CPS_NODE"
                        FillCPSVIEW(mUser.Current_OBJ.OBJ_ID)
                    Case "CPS_OBJ_ATTR_OLD"
                        FillCPSVIEW(mUser.Current_OBJ.OBJ_ID, mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                    Case "CPS_OBJ_ATTR"
                        FillCPSVIEW(mUser.Current_OBJ.OBJ_ID)
                        imgNewCustomizingObj.Enabled = False
                        imgNewDocumentationObject.Enabled = False
                    Case Else
                        mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
                End Select
            Else
                mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
            End If
            If trvOBJ.SelectedNode.ChildNodes.Count = 0 Then
                imgDownloadToExcel.Enabled = True
            Else
                imgDownloadToExcel.Enabled = False
            End If
            cmbFieldFilter.Items.Clear()
            txtFiltertext.Text = ""
            If Not mDynamicFormController Is Nothing Then
                If Not mDynamicFormController.columns Is Nothing Then
                    For Each c As String In mDynamicFormController.columns
                        cmbFieldFilter.Items.Add(c)
                    Next
                    '------------------------------------------------------------------------------
                    'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
                    'Comment    : Do not show OBJ_VERSIONNUMBER on  any mask
                    'Added by   : Pratyusa Lenka (CWID : EOJCG)
                    'Date       : 2016-02-22
                    If Not cmbFieldFilter.Items.FindByValue("Versionno") Is Nothing Then
                        cmbFieldFilter.Items.Remove("Versionno")
                    End If
                    ' Reference END : CR ZHHR 1053558
                    '------------------------------------------------------------------------------
                End If
            End If
            If Not Me.Request.Params("Values") Is Nothing Then
                '---------------------------------------------------------------------------------------------------
                ' Reference : CR - BY-RZ04-CMT-27979 - GBOX forms: clear GBOX Cockpit filter
                ' Comment: Check if different tree node is selected if so disable the filter criteria (where clause)
                ' Added by:  Surendra Purav (CWID : EQIZU)
                '---------------------------------------------------------------------------------------------------
                If Not Request.Params("PATH") Is Nothing Then
                    Dim lpath As String
                    lpath = Request.Params("PATH")
                    'Check if object id from url is different than in currentobject
                    If lpath.Contains(mUser.Current_OBJ.OBJ_ID) Then ' 
                        Dim lValues As String = Me.Request.Params("Values").ToString
                        Dim lPair As Array = lValues.ToString.Split("|")
                        cmbFieldFilter.SelectedValue = mUser.GBOXmanager.GetDisplayNameByObjFieldId(mUser.Current_OBJ.OBJ_ID, lPair(0))
                        If Not String.IsNullOrEmpty(cmbFieldFilter.SelectedValue) Then
                            txtFiltertext.Text = lPair(1)
                        End If
                    Else
                        'clears the filter criteria
                        txtFiltertext.Text = ""
                        cmbFieldFilter.SelectedIndex = 0
                    End If
                End If
            End If
            ''Below method clear if any files attached to previous session DRS request
            clearAttachedFilesToRequest()
        Catch ex As Exception
            lblStatus.Text = ex.Message
        End Try

    End Function
    Private Sub lookForChildnodes(ByVal lNode As TreeNode, ByVal lText As String)
        For Each ltNode As TreeNode In lNode.ChildNodes
            If ltNode.Text = lText.ToString Then
                ltNode.ImageUrl = "~/Images/application_s.gif"
            End If
            lookForChildnodes(ltNode, lText)
        Next
    End Sub
    Private Sub LoadFactoryAndControllerAndBind(ByVal lWithPages As Boolean, Optional ByVal lNewrequest As Boolean = False)
        Try
            Dim lFacory As New Dynamic_View_Controller_Factory
            mControllerfactory = lFacory
            lFacory.TOPIC_ID = mnuDetailsMenu.SelectedValue
            lFacory.TOPIC_GROUP_ID = mnuNavigate.SelectedValue
            lFacory.TOPIC_GROUP_CONTEXT_ID = m_Topic_Group_Context_ID
            lFacory.IsPostback = Me.IsPostBack
            lFacory.Request = Me.Request
            lFacory.User = mUser
            If trvOBJ.SelectedNode Is Nothing Then Exit Sub

            mDynamicFormController = lFacory.SelectedNodeChange(trvOBJ.SelectedNode)
            '---------------------------------------------------------------------------
            ' Reference : CR ZHHR 1060685 - GBOX REP: OTT 3229 - Reporting authorization
            ' Comment   : Check for report authorization
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-08-12
            If Not mnuNavigate.SelectedItem Is Nothing And Not trvOBJ.SelectedNode Is Nothing Then
                If mnuNavigate.SelectedItem.Text.ToUpper = "REPORTING" Or mnuDetailsMenu.SelectedItem.Text.ToUpper = "WORKFLOW_REPORTING" Then
                    'For public reports, access should be possible
                    Dim dtDatabaseObjRole As DataTable = mUser.Databasemanager.MakeDataTable("SELECT [ROLE] FROM DATABASE_OBJ WHERE OBJ_ID = '" & trvOBJ.SelectedNode.Value & "'")
                    If Not dtDatabaseObjRole Is Nothing AndAlso dtDatabaseObjRole.Rows.Count > 0 AndAlso Not IsDBNull(dtDatabaseObjRole.Rows(0)("ROLE")) AndAlso Not String.IsNullOrEmpty(dtDatabaseObjRole.Rows(0)("ROLE").ToString) Then
                        Dim strSqlAuthReport As String = "SELECT DOJ.OBJ_ID, DOJ.[ROLE], AST.APPLICATION_PART_ID, AST.APPLICATION_ROLE_ID FROM DATABASE_OBJ DOJ" & _
                                                   " INNER JOIN AUTHORISATION_SET AST ON DOJ.[ROLE] = AST.APPLICATION_ROLE_ID" & _
                                                   " WHERE DOJ.OBJ_ID = '" & trvOBJ.SelectedNode.Value & "' AND AST.APPLICATION_PART_ID = 'REPORTING' AND AST.CW_ID = '" & mUser.CW_ID & "'"
                        Dim dtAuthReport As DataTable = mUser.Databasemanager.MakeDataTable(strSqlAuthReport)
                        If Not dtAuthReport Is Nothing AndAlso dtAuthReport.Rows.Count = 0 Then
                            lblStatus.Text = "Access to report " & trvOBJ.SelectedNode.Value & " not possible. Contact support for access."
                            lblStatus.ForeColor = Drawing.Color.Red
                            lblStatus.Font.Bold = True
                            HideAll()
                        Else
                            lblStatus.Text = ""
                            If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID.Trim = "" Then
                                mUser.Current_OBJ = mUser.GBOXmanager.SetCurrentObj(trvOBJ.SelectedNode.Value.Split("(")(0).ToString, mUser)
                                mDynamicFormController = lFacory.SelectedNodeChange(trvOBJ.SelectedNode)
                                Enable_Disable("trvOBJ", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                            End If
                        End If
                    End If
                End If
            End If
            ' Reference END : CR ZHHR 1060685
            '---------------------------------------------------------------------------
            If mDynamicFormController Is Nothing Then Exit Sub
            mDynamicFormController.TOPIC_ID = mnuDetailsMenu.SelectedValue
            mDynamicFormController.Request = Me.Request
            mDynamicFormController.IsPostback = Me.IsPostBack
            mDynamicFormController.User = mUser
            mDynamicFormController.User.Current_OBJ = mUser.Current_OBJ

            Dim lController As String = ""
            If TypeOf mDynamicFormController Is Dynamic_View_Reporter Then
                mDynamicFormController.BindView(vwQuery, lWithPages)
                lController = "Dynamic_View_Reporter"
            End If
            If TypeOf mDynamicFormController Is Dynamic_View_Requester Then
                mDynamicFormController.BindView(vwGridView, lWithPages)
                lController = "Dynamic_View_Requester"
            End If
            If TypeOf mDynamicFormController Is Dynamic_View_Documentator Then
                mDynamicFormController.BindView(vwGridView, lWithPages)
                lController = "Dynamic_View_Documentator"
            End If
            imgHelp.OnClientClick = "javascript:window.open('" & mUser.Current_OBJ.HelpUrL & "',null, 'toolbar=yes,location=yes, status= yes,menubar=yes, scrollbars=yes,resizable=yes'); void('');"



            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR 1026150 - GBOX COC - Problems with searching in DRS handbook 
            ' Comment           : Display error essage when no records are returned after entering filter crieria
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-03-26
            '---------------------------------------------------------------------------------------------------
            '---------------------------------------------------------------------------
            ' Reference : CR ZHHR 1060685 - GBOX REP: OTT 3229 - Reporting authorization
            ' Comment   : Change for ErrString as nothing and access to report message
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-08-12
            If mUser.GBOXmanager.ErrString <> "" AndAlso mUser.GBOXmanager.ErrString.Contains("Your Query returns no results") Then
                lblStatus.Text = mUser.GBOXmanager.ErrString
                lblStatus.ForeColor = Drawing.Color.Red
                lblStatus.Font.Bold = True
            ElseIf lblStatus.Text <> "" AndAlso lblStatus.Text.Contains("Access to report") Then
                lblStatus.ForeColor = Drawing.Color.Red
                lblStatus.Font.Bold = True
                HideAll()
            Else
                lblStatus.Text = ""
            End If
            ' Reference END : CR ZHHR 1060685
            '---------------------------------------------------------------------------
        Catch ex As Exception
            mErrText &= ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        End Try
    End Sub

    Private Sub LoadWizardData(ByVal lWizardobj As String)
        Dim l As Boolean = LoadWizardDataWithReturn(lWizardobj)
        If Not l Then
            LoadWizardDataWithReturn("INIT")
        End If
    End Sub


    Private Function LoadWizardDataWithReturn(ByVal lWizardobj As String) As Boolean
        Dim myReturn As Boolean = True
        Try
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
            End If
            ' Reference         : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
            ' Comment           : Hide the wizard area
            ' Added by          : Pratyusa Lenka (CWID : EOJCG)
            ' Date              : 07-11-2018
            'Dim i As Long = 1
            'mnuWizzard.Items.Clear()
            'Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable("Select * from OBJ_WIZARD Where WIZ_ID = '" & lWizardobj & " 'And (Subgroup_ID='ALL' or Subgroup_ID='" & mUser.SUBGROUP_ID & "') order by Rank")
            'If mdt Is Nothing Then ResetView()
            'If mdt.Rows.Count = 0 Then
            '    mvWizard.Visible = False
            '    imgBubble.Visible = False
            '    mnuWizzard.Visible = False
            '    tblWiz.Visible = False
            '    lblTablenameData.Visible = False
            '    lblIsTree.Visible = False
            '    myReturn = False
            'Else
            '    mvWizard.Visible = True
            '    imgBubble.Visible = True
            '    mnuWizzard.Visible = True
            '    tblWiz.Visible = True
            '    lblTablenameData.Visible = True
            '    lblIsTree.Visible = True
            'End If
            'For Each r As DataRow In mdt.Rows
            '    Dim mnuWizz As New MenuItem
            '    mnuWizz.Text = r("StepTitle").ToString
            '    mnuWizz.Value = i

            '    mnuWizzard.Items.Add(mnuWizz)
            '    Dim lbl As Label = Me.FindControl("Label" & i)
            '    lbl.Text = r("Text").ToString
            '    i = i + 1
            'Next
            'If mnuWizzard.Items.Count > 0 Then
            '    mnuWizzard.Items(0).Selected = True
            'End If
            'mvWizard.Visible = True
            'mvWizard.ActiveViewIndex = 0
            'If mnuWizzard.Items.Count < 2 Then
            '    mnuWizzard.Enabled = False
            'Else
            '    mnuWizzard.Enabled = True
            'End If

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035538 - GBOX COC DEV Cockpit: Adding a help Button to the main screen of Cockpit
            ' Comment           : This is an exeptional button and it should have no impact in
            '                     Code Generaters as discussed 20.11.2014 in Shadowing session
            ' Added by          : Bernd Dächer (CWID : EZZRY)
            ' Date              : 2014-11-19
            imgHelp_withHelpIcon.OnClientClick = "javascript:window.open('http://sp-appl-bbs.bayer-ag.com/sites/010045/Cockpit_Wiki/Home.aspx',null, 'toolbar=yes,location=yes, status= yes,menubar=yes, scrollbars=yes,resizable=yes'); void('');"
            imgHelp_withHelpIcon.Visible = True
            ' Reference  END       : CR ZHHR 1035538
            ' Added by          : Bernd Dächer (CWID : EZZRY)
            '--------------------------------------------------

            Return myReturn
        Catch ex As Exception
            mErrText &= "LoadWizData:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
            Return False
        End Try
    End Function
    Private Sub MakeTopicsMenu(ByVal lTopicGoupId As String)
        Try
            mnuDetailsMenu.Items.Clear()
            trvOBJ.Nodes.Clear()
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
            End If

            Dim lTopics As DataTable = mUser.Databasemanager.MakeDataTable("Select FORM_VIEW,Topic_ID,MENU_TEXT,TOOL_TIP_TEXT from Topic Where Topic_Group_ID='" & lTopicGoupId & "' And Active=1 order by Rank")
            For Each r As DataRow In lTopics.Rows
                Dim lNewmenuItem As New MenuItem(r("MENU_TEXT").ToString)
                lNewmenuItem.ToolTip = r("TOOL_TIP_TEXT").ToString
                lNewmenuItem.Value = r("Topic_ID").ToString
                mnuDetailsMenu.Items.Add(lNewmenuItem)
            Next r
            If Not Request.Params("TOPIC") Is Nothing Then
                For z As Integer = 0 To mnuDetailsMenu.Items.Count - 1
                    If mnuDetailsMenu.Items(z).Text.ToUpper = Request.Params("TOPIC").ToString.ToUpper Then
                        mnuDetailsMenu.Items(z).Selected = True
                        Exit For
                    End If
                Next z
            End If
            If mnuDetailsMenu.Items.Count = 1 Then
                mnuDetailsMenu.Items(0).Selected = True
                LoadWizardData(mnuDetailsMenu.SelectedItem.Text)
                DetailsMenuSelect(mWithPaging)
                Exit Sub
            End If

            If mnuDetailsMenu.SelectedItem Is Nothing Then
                ResetView()
            Else
                LoadWizardData(mnuDetailsMenu.SelectedItem.Text)
                DetailsMenuSelect(mWithPaging)
            End If
        Catch ex As Exception
            mErrText &= "MakeTopicMenu" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        End Try
    End Sub

    Private Sub opennew()
        Try
            mUser.OBJ_Value = ""
            mvContents.ActiveViewIndex = GetIndexByViewName("vwDetails")
            BindDetails(mWithPaging, True)
            mUser.EditMode = True
            dvInfo.ChangeMode(DetailsViewMode.Insert)
        Catch ex As Exception
            mErrText &= "opennew" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        End Try
    End Sub
    Private Sub ResetView()
        Try
            imgHelp.DescriptionUrl = ""
            trvOBJ.Visible = True
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
            End If
            '---------------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
            ' Comment   : New concept for multiple text functionality
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-03-24
            '--------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
            ' Comment   : Delete COMPOSITE_S_T object type
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-01-25
            If Not mUser.Current_OBJ Is Nothing Then
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID.ToUpper = "COMPOSITE_S_T_TXT".ToUpper Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID.ToUpper = "COMPOSITE_MULTI_TXT" Then
                    btnSubmit.CommandArgument = "Insert"
                Else
                    btnSubmit.CommandArgument = "Update"
                End If
            End If
            ' Reference END : CR ZHHR 1052471
            '--------------------------------------------------------------------------------
            lblStatus.Text = ""
            ' Reference         : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
            ' Comment           : Hide the wizard area
            ' Added by          : Pratyusa Lenka (CWID : EOJCG)
            ' Date              : 07-11-2018
            'lblInfo.Text = ""
            mvContents.ActiveViewIndex = GetIndexByViewName("vwGridview")
            mUser.EditMode = False
            'lblTablenameData.Enabled = False
            'lblTablenameData.Text = ""
            grdDat.EmptyDataText = ""

            grdDat.DataSource = Nothing
            grdDat.DataBind()
            grdQuery.DataSource = Nothing
            grdQuery.DataBind()
            dvInfo.DataSource = Nothing
            dvInfo.DataBind()
            For i = 1 To 12
                Dim lbl As Label = Me.FindControl("Label" & i)
                lbl.Text = ""
            Next
            'mvWizard.Visible = False
            imgLegende.Visible = False
            lblLegende.Visible = False
            ChangeIcons()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ChangeIcons()
        With imgNewCustomizingObj
            If .Enabled Then
                .ImageUrl = "~\Images\table_add.png"
            Else
                .ImageUrl = "~\Images\table_add_grey.png"
            End If
        End With
        With imgRefresh
            If .Enabled Then
                .ImageUrl = "~\Images\arrow_refresh.gif"
            Else
                .ImageUrl = "~\Images\arrow_refresh_grey.gif"
            End If
        End With

        With imgNewDocumentationObject
            If .Enabled Then
                .ImageUrl = "~\Images\table_add.png"
            Else
                .ImageUrl = "~\Images\table_add_grey.png"
            End If
        End With
        With imgSubmitCustomizingObj
            If .Enabled Then
                .ImageUrl = "~\Images\table_go.png"
            Else
                .ImageUrl = "~\Images\table_go_grey.png"
            End If
        End With
        With imgEditCustomizingObj
            If .Enabled Then
                .ImageUrl = "~\Images\table_edit.gif"
            Else
                .ImageUrl = "~\Images\table_edit_gray.gif"
            End If
        End With
        With imgSubmitDocumentation
            If .Enabled Then
                .ImageUrl = "~\Images\table_go.png"
            Else
                .ImageUrl = "~\Images\table_go_grey.png"
            End If
        End With
        With imgQuery
            If .Enabled Then
                .ImageUrl = "~\Images\report_go_2.gif"
            Else
                .ImageUrl = "~\Images\report_go_2_grey.gif"
            End If
        End With
        With imgCancel
            If .Enabled Then
                .ImageUrl = "~\Images\cancel.gif"
            Else
                .ImageUrl = "~\Images\cancel_grey.gif"
            End If
        End With
        With imgEditDocumentation
            If .Enabled Then
                .ImageUrl = "~\Images\table_edit.gif"
            Else
                .ImageUrl = "~\Images\table_edit_gray.gif"
            End If
        End With
        With imgAlert
            If .Enabled Then
                .ImageUrl = "~\Images\bell_add.png"
            Else
                .ImageUrl = "~\Images\bell_add_grey.png"
            End If
        End With
        With imgmySubscriptions
            If .Enabled Then
                .ImageUrl = "~\Images\user_edit.gif"
            Else
                .ImageUrl = "~\Images\user_edit_Grey.gif"
            End If
        End With
        With imgSearchEngine
            If .Enabled Then
                .ImageUrl = "~\Images\page_find.gif"
            Else
                .ImageUrl = "~\Images\grey_page_find.gif"
            End If
        End With
        With imgDownloadToExcel
            If .Enabled Then
                .ImageUrl = "~\Images\download.ico"
            Else
                .ImageUrl = "~\Images\download_gray.png"
            End If
        End With
    End Sub
    Private Function GetClickedControl() As String
        Dim lControls As New List(Of String)
        '------------------------------------------------------------------------------
        'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
        'Comment    : On third mask image buttons are replaced by button controls
        'Added by   : Pratyusa Lenka (CWID : EOJCG)
        'Date       : 2016-02-16
        lControls.Add("btnTextFill")
        lControls.Add("btnOverwriteEnglish")
        ' Reference END : CR ZHHR 1053558
        '------------------------------------------------------------------------------
        lControls.Add("imgCancel")
        lControls.Add("imgAlert")
        lControls.Add("imgmySubscriptions")
        lControls.Add("imgSearchEngine")
        lControls.Add("imgEngine")
        lControls.Add("imgQuery")
        lControls.Add("imgDownloadToExcel")
        lControls.Add("imgHelp")
        lControls.Add("imgEditCustomizingObj")
        lControls.Add("imgSubmitCustomizingObj")
        lControls.Add("imgEditDocumentation")
        lControls.Add("imgSubmitDocumentation")
        lControls.Add("imgWiki")
        lControls.Add("imgNewCustomizingObj")
        lControls.Add("imgNewDocumentationObject")
        lControls.Add("imgRefresh")
        lControls.Add("imgAppend")
        lControls.Add("imgSearchCancel")
        '---------------------------------------------------------------------
        ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
        ' Comment   : Get clicked control for Add and Change value button
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2015-12-24
        lControls.Add("btnAddValue")
        lControls.Add("btnChangeValue")
        ' Reference END : CR ZHHR 1050708
        '---------------------------------------------------------------------

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : Add Copy button 
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 23-Feb-2016
        lControls.Add("btnCopyValue")
        ' Reference         : ZHHR 1053017

        Dim lClickedControl As Array = Request.Form.AllKeys
        For i = 0 To lClickedControl.GetUpperBound(0) - 1
            For Each lControl As String In lControls
                If lClickedControl(i).ToString = lControl.ToString & ".x" Then
                    Return lControl
                End If
            Next
        Next i
        Return ""
    End Function
    Private Function BindDetails(ByVal lWithPages As Boolean, Optional ByVal lNewRequest As Boolean = False, Optional ByVal lCopyRequest As Boolean = False) As Boolean
        Dim lSuccess As Boolean = False
        ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
        ' Comment   : Make the key fields non-ediatble for copy values
        ' Added by  : Pratyusa Lenka (CWID : EOJCG)
        ' Date      : 2019-04-01
        If lblRequestType.Text.ToUpper = "COPY" Then
            lCopyRequest = True
        End If
        Try
            Dim lParamlist As String = GetParamList()
            Me.Form.Action = lParamlist
            imgCancel.Enabled = True
            If Not Request.Form Is Nothing Then
                mDynamicFormController.Request = Request
            End If
            mUser.Query = False
            mDynamicFormController.TOPIC_ID = mUser.Current_OBJ.TOPIC_ID
            Dim lController As String = ""
            '---------------------------------------------------------------------
            ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
            ' Comment   : Placed new Add value button to create new request
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-12-24
            If TypeOf mDynamicFormController Is Dynamic_View_Reporter Then
                lSuccess = mDynamicFormController.BindView(vwQuery, lWithPages)
                btnAddValue.Enabled = False
                imgQuery.Visible = True
                lController = "Dynamic_View_Reporter"
            End If
            If TypeOf mDynamicFormController Is Dynamic_View_Requester Then
                lController = "Dynamic_View_Requester"
                btnAddValue.Enabled = True
                imgQuery.Visible = False
                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                ' Comment   : Make the key fields non-ediatble for copy values
                ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                ' Date      : 2019-04-01
                Return mDynamicFormController.BindDetail(dvInfo, grdDat, Me.IsPostBack, lNewRequest, lCopyRequest)
            End If
            If TypeOf mDynamicFormController Is Dynamic_View_Documentator Then
                lController = "Dynamic_View_Documentator"
                btnAddValue.Enabled = False
                imgQuery.Visible = True
                imgCancel.Enabled = True
                Return mDynamicFormController.BindView(vwQuery, lWithPages)
            End If
            ' Reference END : CR ZHHR 1050708
            '---------------------------------------------------------------------
            'END
            '  SetButtonForControllermode(lController)
            Return lSuccess
        Catch ex As Exception
            mErrText &= "BindDetails:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
            Return lSuccess
        End Try
    End Function
    Private Function UpdateChilds(ByVal lParent As String, ByVal lTopic As String) As List(Of String)
        Try
            Dim lPackage As New List(Of String)
            Dim dt As DataTable = mUser.Databasemanager.MakeDataTable("Select * from OBJ_OBJS where PARENT_OBJ_ID ='" & lParent & "'")
            For Each r As DataRow In dt.Rows
                Dim lsql As String = "Update obj Set Topic_ID = '" & lTopic & "' Where OBJ_ID = '" & r("CHILD_OBJ_ID").ToString & "'"
                lPackage.Add(lsql)
                Dim lChilds As DataTable = mUser.Databasemanager.MakeDataTable("Select * from OBJ_OBJS where PARENT_OBJ_ID ='" & r("CHILD_OBJ_ID").ToString & "'")
                If lChilds.Rows.Count <> 0 Then
                    lPackage.AddRange(UpdateChilds(r("CHILD_OBJ_ID").ToString, lTopic))
                End If
            Next
            Return lPackage
        Catch ex As Exception
            mErrText &= "UpdateChilds" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
            Return Nothing
        End Try
    End Function
    Private Function BuildTextgrid_TXT() As Boolean
        Try

            '---------------------------------------------------------------------------------------------------
            ' Reference         : ZHHR 1038582 - GBOX COC : Workflow continues without authorization
            ' Comment           : Implemented authorisation
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2015-02-19
            If Not mUser.GBOXmanager.AuthenticateRequest(trvOBJ.SelectedNode.Value, mUser.CW_ID) Then
                lblStatus.Text = "You have no permission to request values for " & trvOBJ.SelectedNode.Value & "."
                Return False
            End If
            ' Reference End      : ZHHR 1038582

            Dim lRequestvalue As String = ""
            'BEGIN COMPOSITE_S_T_TXT
            Dim lWherestring As String = ""
            For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                If InStr(lKey.Displayname.ToUpper, "VERSIONNO") = 0 Then
                    lWherestring = lWherestring & lKey.Key_ID & "='" & lKey.CurrentValue & "' and "
                End If
            Next
            lWherestring = lWherestring.Substring(0, lWherestring.Length - 4)
            'END
            lstOldTexts.Items.Clear()
            mUser.GBOXmanager.SetCurrentObj(lblObj_ID.Text, mUser)
            '---------------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1042186 - GBOX: check obsolete fields in table OBJ
            ' Comment   : Delete the columns OBJ_TABLENAME_TEXTS,OBJ_TABLENAME_TEXTS_LG_COL,OBJ_TABLENAME_TEXTS_LG_DESC since not used
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-05-18
            Dim lTextdata As DataTable = mUser.Databasemanager.MakeDataTable("Select  [OBJ_TABLENAME_TEXTS_KEY],OBJ_TABLENAME_TEXTS_LENGTH from OBJ where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "'")
            ' Reference END : CR ZHHR 1042186
            '----------------------------------------------------------------------------------------
            Dim lTexttableKey As String = lTextdata.Rows(0)("OBJ_TABLENAME_TEXTS_KEY").ToString
            '---------------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1043641 - GBOX COC: Bugfix for multimultimulti functionality
            ' Comment   : Entry in OBJ.OBJ_TABLENAME_TEXTS_LENGTH shall not be mandatory for COMPOSITE_MULTI_TXT objects
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-06-10
            If lTextdata.Rows(0)("OBJ_TABLENAME_TEXTS_LENGTH").ToString = "" And mUser.Current_OBJ.OBJ_CLASSIFICATION_ID <> "COMPOSITE_MULTI_TXT" Then
                mErrText = "CUSTOMIZE: OBJ.OBJ_TABLENAME_TEXTS_LENGTH!"
            End If
            If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID <> "COMPOSITE_MULTI_TXT" Then
                Dim lTextLength As Long = CLng(lTextdata.Rows(0)("OBJ_TABLENAME_TEXTS_LENGTH").ToString)
                lblTextlenght.Text = lTextLength
            End If
            ' Reference END : CR ZHHR 1043641
            '---------------------------------------------------------------------------------------
            Dim lVersionSQL As String = "Select MAX(OBJ_VERSIONNUMBER) as LastVersionNumber from " & mUser.Current_OBJ.OBJ_TABLENAME & " where " & lWherestring
            Dim lVersiondata As DataTable = mUser.Databasemanager.MakeDataTable(lVersionSQL)
            If lVersiondata.Rows.Count = 0 Then
                lblStatus.Text = "lVersiondata.Rows.count = 0"
                Return False
            End If
            Dim lTableNameText As String = "Customizing_" & mUser.Current_OBJ.OBJ_ID & "_TXT"
            Dim lVersionnumber As String = lVersiondata.Rows(0)("LastVersionNumber").ToString

            '------------------------------------------------------------------------------------------------------------
            ' Reference         : CR 1027258 - 110IM08894225 - Creating DRS request with texts texts may disappear  
            ' Comment           : Combine the results of 2 queries(with and without LANGUAGE_ISO_CODE code) in one resultset with order by RANK column
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2014-05-05
            '------------------------------------------------------------------------------------------------------------
            Dim lsql As String = String.Empty
            lsql = "(SELECT CUSTOMIZING_LANGUAGE_TEMPLATE.RANK, CUSTOMIZING_LANGUAGE_TEMPLATE.LANGUAGE_ISO_CODE_DESCRIPTION,"
            lsql = lsql & "CUSTOMIZING_LANGUAGE_TEMPLATE.LANGUAGE_ISO_CODE, OBJ_LANGUAGE_VALUE, OBJ_VERSIONNUMBER"
            lsql = lsql & "  FROM CUSTOMIZING_LANGUAGE_TEMPLATE "
            lsql = lsql & " LEFT  JOIN " & lTableNameText & " "
            lsql = lsql & " ON CUSTOMIZING_LANGUAGE_TEMPLATE.LANGUAGE_ISO_CODE = " & lTableNameText & ".OBJ_LANGUAGE_ISO_CODE"
            lsql = lsql & " where " & lWherestring & " And " & lTableNameText & ".OBJ_VERSIONNUMBER='" & lVersionnumber & "'"
            lsql = lsql & ") "
            lsql = lsql & " Union "
            lsql = lsql & "( "
            lsql = lsql & "  SELECT CUSTOMIZING_LANGUAGE_TEMPLATE.RANK, CUSTOMIZING_LANGUAGE_TEMPLATE.LANGUAGE_ISO_CODE_DESCRIPTION , "
            lsql = lsql & "  CUSTOMIZING_LANGUAGE_TEMPLATE.LANGUAGE_ISO_CODE, NULL,NULL "
            lsql = lsql & "  FROM CUSTOMIZING_LANGUAGE_TEMPLATE  where LANGUAGE_ISO_CODE not in (SELECT OBJ_LANGUAGE_ISO_CODE From " & lTableNameText & " "
            lsql = lsql & " Where " & lWherestring & " And " & lTableNameText & ".OBJ_VERSIONNUMBER='" & lVersionnumber & "')"
            lsql = lsql & ") "
            lsql = lsql & " ORDER BY  CUSTOMIZING_LANGUAGE_TEMPLATE.RANK"
            '---------------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
            ' Comment   : New concept for multiple text functionality
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-03-24
            If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                Dim lstTexts As New List(Of String)
                Dim strSqlText As String = String.Empty
                Dim strTextColumn As String
                Dim objmyKeyObj As New myKeyObj
                strResult = String.Empty
                strSqlText = "SELECT * from " & lTableNameText & " where " & lWherestring
                Dim dtTextData As DataTable = mUser.Databasemanager.MakeDataTable(strSqlText)
                For Each dcText As DataColumn In dtTextData.Columns
                    strTextColumn = String.Empty
                    strTextColumn = dcText.ColumnName
                    objmyKeyObj = mUser.GBOXmanager.KeyCollection.Find(Function(x) x.Key_ID = strTextColumn)
                    If objmyKeyObj Is Nothing And strTextColumn <> "OBJ_LANGUAGE_ISO_CODE" Then
                        lstTexts.Add(strTextColumn)
                    End If
                    strResult = String.Join(",", lstTexts.ToArray())
                Next
                '-----------------------------------------------------------------
                ' Reference : ZHHR 1069561 - GBOX COC: Translation text validation
                ' Comment   : Order of the text columns based on OBJ_TEXTS_LENGTHS
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2017-03-22
                Dim strSQLTextLength As String = "Select OBJ_COLUMN_ID,TEXT_LENGTH from OBJ_TEXTS_LENGTHS where OBJ_ID ='" & mUser.Current_OBJ.OBJ_ID & "'"
                Dim dtTextLength As DataTable = mUser.Databasemanager.MakeDataTable(strSQLTextLength)
                If Not dtTextLength Is Nothing AndAlso dtTextLength.Rows.Count > 0 Then
                    strResult = String.Empty
                    lstTexts = New List(Of String)
                    For Each drTextLength As DataRow In dtTextLength.Rows
                        strTextColumn = String.Empty
                        strTextColumn = drTextLength("OBJ_COLUMN_ID").ToString
                        lstTexts.Add(strTextColumn)
                        strResult = String.Join(",", lstTexts.ToArray())
                    Next
                End If
                ' Reference END : CR ZHHR 1069561
                '-----------------------------------------------------------------
                '----------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1043641 - GBOX COC: Bugfix for multimultimulti functionality
                ' Comment   : The number of text columns in CUSTOMIZING_XXX_TXT tables shall be variable for COMPOSITE_MULTI_TXT objects
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-06-10
                lsql = "(SELECT CUSTOMIZING_LANGUAGE_TEMPLATE.RANK, CUSTOMIZING_LANGUAGE_TEMPLATE.LANGUAGE_ISO_CODE_DESCRIPTION,"
                lsql = lsql & "CUSTOMIZING_LANGUAGE_TEMPLATE.LANGUAGE_ISO_CODE, OBJ_VERSIONNUMBER, " & strResult
                lsql = lsql & "  FROM CUSTOMIZING_LANGUAGE_TEMPLATE "
                lsql = lsql & " LEFT  JOIN " & lTableNameText & " "
                lsql = lsql & " ON CUSTOMIZING_LANGUAGE_TEMPLATE.LANGUAGE_ISO_CODE = " & lTableNameText & ".OBJ_LANGUAGE_ISO_CODE"
                lsql = lsql & " where " & lWherestring & " And " & lTableNameText & ".OBJ_VERSIONNUMBER='" & lVersionnumber & "'"
                lsql = lsql & ") "
                lsql = lsql & " Union "
                lsql = lsql & "( "
                lsql = lsql & "  SELECT CUSTOMIZING_LANGUAGE_TEMPLATE.RANK, CUSTOMIZING_LANGUAGE_TEMPLATE.LANGUAGE_ISO_CODE_DESCRIPTION , "
                lsql = lsql & "  CUSTOMIZING_LANGUAGE_TEMPLATE.LANGUAGE_ISO_CODE, NULL"
                For txtCount As Integer = 1 To lstTexts.Count
                    lsql = lsql & ",NULL"
                Next
                lsql = lsql & "  FROM CUSTOMIZING_LANGUAGE_TEMPLATE  where LANGUAGE_ISO_CODE not in (SELECT OBJ_LANGUAGE_ISO_CODE From " & lTableNameText & " "
                lsql = lsql & " Where " & lWherestring & " And " & lTableNameText & ".OBJ_VERSIONNUMBER='" & lVersionnumber & "')"
                lsql = lsql & ") "
                lsql = lsql & " ORDER BY  CUSTOMIZING_LANGUAGE_TEMPLATE.RANK"
                ' Reference END : CR ZHHR 1043641
                '----------------------------------------------------------------------------------

                lblTextlenght.Text = ""
                For Each drTextLength As DataRow In dtTextLength.Rows
                    lblTextlenght.Text &= " " & drTextLength("OBJ_COLUMN_ID") & "-" & drTextLength("TEXT_LENGTH")
                    If drTextLength("TEXT_LENGTH").ToString = "" Then
                        mErrText = "CUSTOMIZE: OBJ_TEXTS_LENGTHS." & drTextLength("OBJ_COLUMN_ID") & ".TEXT_LENGTH!"
                    End If
                Next
            End If
            ' Reference END : CR ZHHR 1038241
            '----------------------------------------------------------------------------------------

            Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
            Dim i As Integer = 1
            '----------------------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1054864 - GBOX COC: OTT 2190 - Description and EN translation correlation
            ' Comment   : Get the TRANSLATION_FIELD_ID for the object and make table rows for EN to display
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-03-29
            Dim strSqlTransField As String = "SELECT TRANSLATION_FIELD_ID FROM OBJ_FIELD WHERE OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "' AND OBJ_FIELD_ID = 'DESCRIPTION' AND TRANSLATION_FIELD_ID IS NOT NULL"
            Dim dtTransField As DataTable = mUser.Databasemanager.MakeDataTable(strSqlTransField)
            MakeTableRows(mdt, i, dtTransField)
            For Each dr As DataRow In mdt.Rows
                Dim Countstring As String = String.Empty
                Dim lbl_Code As Label = Nothing
                Dim txt_Edit_Lang As TextBox = Nothing
                Dim lblTransField As Label = Nothing
                '---------------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                ' Comment   : New concept for multiple text functionality
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-03-24
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID <> "COMPOSITE_MULTI_TXT" Then
                    Dim lText As New OBJ_TEXT(lTexttableKey, lRequestvalue, dr("LANGUAGE_ISO_CODE").ToString, dr("LANGUAGE_ISO_CODE_DESCRIPTION").ToString, dr("OBJ_LANGUAGE_VALUE").ToString, dr("OBJ_VERSIONNUMBER").ToString)
                    Countstring = Right("0000" & i.ToString, 2)
                    lbl_Code = Me.FindControl("lbl_Lang_" & Countstring & "_Code")
                    lbl_Code.Text = lText.LANGUAGE_CODE
                    lbl_Code.Visible = True
                    Dim lbl_LanguageText As Label = Me.FindControl("lbl_Lang_" & Countstring & "_LanguageText")
                    lbl_LanguageText.Text = lText.LANGUAGE_DESCRIPTION
                    lbl_LanguageText.Visible = True
                    txt_Edit_Lang = Me.FindControl("txt_Edit_Lang_" & Countstring)
                    txt_Edit_Lang.Visible = True
                    ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
                    ' Comment   : When focus is on any of the input fields and press the <Enter> button, should have the same effect as click button <Next> 
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2019-09-26
                    txt_Edit_Lang.Attributes.Add("onkeypress", "return enterKeyPress(event,'btnNextEditSystems')")
                    'OTT 2190
                    If lbl_Code.Text.ToUpper = "EN" And Not dtTransField Is Nothing And dtTransField.Rows.Count > 0 Then
                        lblTransField = Me.FindControl("lbl_Lang_" & Countstring & "Col_" & "TransText")
                        If mUser.RequestType <> myUser.RequestTypeOption.insert Then
                            If Not lblTransField Is Nothing Then
                                txt_Edit_Lang.Text = strDescription
                                txt_Edit_Lang.Attributes.Add("readonly", "readonly")
                                txt_Edit_Lang.BackColor = Drawing.Color.LightGray
                                lblTransField.Text = "EN text was edited. Please check this value!"
                                lblTransField.ForeColor = Drawing.Color.Red
                            Else
                                txt_Edit_Lang.Text = lText.LANGUAGE_VALUE
                            End If
                        ElseIf mUser.RequestType = myUser.RequestTypeOption.insert And Not lblTransField Is Nothing Then
                            txt_Edit_Lang.Text = strDescription
                            txt_Edit_Lang.Attributes.Add("readonly", "readonly")
                            txt_Edit_Lang.BackColor = Drawing.Color.LightGray
                        End If
                        lstOldTexts.Items.Add(lbl_Code.Text & "|" & txt_Edit_Lang.Text)
                    Else
                        If mUser.RequestType <> myUser.RequestTypeOption.insert Then
                            txt_Edit_Lang.Text = lText.LANGUAGE_VALUE
                        End If
                        lstOldTexts.Items.Add(lbl_Code.Text & "|" & txt_Edit_Lang.Text)
                    End If
                    'If mUser.RequestType <> myUser.RequestTypeOption.insert Then
                    '    txt_Edit_Lang.Text = lText.LANGUAGE_VALUE
                    'End If
                    'lstOldTexts.Items.Add(lbl_Code.Text & "|" & txt_Edit_Lang.Text)
                ElseIf mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                    Dim lstOldMultiTexts As New List(Of String)
                    Dim strOldMultiTexts As String = String.Empty
                    Dim lText As New OBJ_TEXT(lTexttableKey, lRequestvalue, dr("LANGUAGE_ISO_CODE").ToString, dr("LANGUAGE_ISO_CODE_DESCRIPTION").ToString, "", dr("OBJ_VERSIONNUMBER").ToString)
                    Countstring = Right("0000" & i.ToString, 2)
                    lbl_Code = Me.FindControl("lbl_Lang_" & Countstring & "_Code")
                    lbl_Code.Text = lText.LANGUAGE_CODE
                    lbl_Code.Visible = True
                    Dim lbl_LanguageText As Label = Me.FindControl("lbl_Lang_" & Countstring & "_LanguageText")
                    lbl_LanguageText.Text = lText.LANGUAGE_DESCRIPTION
                    lbl_LanguageText.Visible = True
                    For Each dtCol As DataColumn In dr.Table.Columns
                        If Not (dtCol.ColumnName.ToUpper = "RANK" Or _
                           dtCol.ColumnName.ToUpper = "LANGUAGE_ISO_CODE" Or _
                           dtCol.ColumnName.ToUpper = "LANGUAGE_ISO_CODE_DESCRIPTION" Or _
                           dtCol.ColumnName.ToUpper = "OBJ_VERSIONNUMBER") Then
                            Dim txt_Edit_Multi_Lang As TextBox = Me.FindControl("txt_Edit_Multi_Lang_" & Countstring & "Col_" & dtCol.Ordinal - 1)
                            ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
                            ' Comment   : When focus is on any of the input fields and press the <Enter> button, should have the same effect as click button <Next> 
                            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                            ' Date      : 2019-09-26
                            txt_Edit_Multi_Lang.Attributes.Add("onkeypress", "return enterKeyPress(event,'btnNextEditSystems')")
                            txt_Edit_Multi_Lang.Visible = True
                            'OTT 2190
                            If lbl_Code.Text.ToUpper = "EN" And Not dtTransField Is Nothing And dtTransField.Rows.Count > 0 Then
                                lblTransField = Me.FindControl("lbl_Lang_" & Countstring & "Col_" & dtCol.Ordinal - 1 & "TransText")
                                If mUser.RequestType <> myUser.RequestTypeOption.insert Then
                                    If Not lblTransField Is Nothing Then
                                        txt_Edit_Multi_Lang.Text = strDescription
                                        txt_Edit_Multi_Lang.Attributes.Add("readonly", "readonly")
                                        txt_Edit_Multi_Lang.BackColor = Drawing.Color.LightGray
                                        lblTransField.Text = "EN text was edited. Please check this value!"
                                        lblTransField.ForeColor = Drawing.Color.Red
                                    Else
                                        txt_Edit_Multi_Lang.Text = dr(dtCol.ColumnName).ToString
                                    End If
                                ElseIf mUser.RequestType = myUser.RequestTypeOption.insert And Not lblTransField Is Nothing Then
                                    txt_Edit_Multi_Lang.Text = strDescription
                                    txt_Edit_Multi_Lang.Attributes.Add("readonly", "readonly")
                                    txt_Edit_Multi_Lang.BackColor = Drawing.Color.LightGray
                                End If
                                lstOldMultiTexts.Add(dr(dtCol.ColumnName).ToString)
                                strOldMultiTexts = String.Join("|", lstOldMultiTexts.ToArray())
                            Else
                                If mUser.RequestType <> myUser.RequestTypeOption.insert Then
                                    txt_Edit_Multi_Lang.Text = dr(dtCol.ColumnName).ToString
                                End If
                                lstOldMultiTexts.Add(dr(dtCol.ColumnName).ToString)
                                strOldMultiTexts = String.Join("|", lstOldMultiTexts.ToArray())
                            End If
                            'If mUser.RequestType <> myUser.RequestTypeOption.insert Then
                            '    txt_Edit_Multi_Lang.Text = dr(dtCol.ColumnName).ToString
                            'End If
                            'lstOldMultiTexts.Add(dr(dtCol.ColumnName).ToString)
                            'strOldMultiTexts = String.Join("|", lstOldMultiTexts.ToArray())
                        End If
                    Next
                    lstOldTexts.Items.Add(lbl_Code.Text & "|" & strOldMultiTexts)
                End If
                ' Reference END : CR ZHHR 1054864
                '----------------------------------------------------------------------------------------------
                ' Reference END : CR ZHHR 1038241
                '----------------------------------------------------------------------------------------
                i = i + 1
            Next

            ChangeIcons()
            ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
            ' Comment   : Default focus should be on <Next> button
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2019-09-26
            btnNextEditSystems.Focus()
        Catch Ex As Exception
            lblStatus.Text = mErrText & lblStatus.Text & "BuildTextgrid:" & Ex.Message
        End Try
    End Function

    Sub MakeTableRows(ByVal mDt As DataTable, ByVal lStartRowNumber As Integer, ByVal dtTransField As DataTable)
        Try
            '----------------------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1054864 - GBOX COC: OTT 2190 - Description and EN translation correlation
            ' Comment   : Add label to the cell of EN translation row to display the message
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-03-29
            Dim bIsLabelExists As Boolean = False
            Dim strTransField As String = ""
            Dim strCurrDescription As String = ""
            Dim lWherestring As String = ""
            For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                lWherestring = lWherestring & lKey.Key_ID & "='" & lKey.CurrentValue & "' and "
            Next
            lWherestring = lWherestring.Substring(0, lWherestring.Length - 4)
            Dim dtCustDesc As DataTable = mUser.Databasemanager.MakeDataTable("SELECT DESCRIPTION FROM " & mUser.Current_OBJ.OBJ_TABLENAME & " WHERE " & lWherestring)
            If Not dtCustDesc Is Nothing AndAlso dtCustDesc.Rows.Count > 0 Then
                strCurrDescription = dtCustDesc.Rows(0)("DESCRIPTION").ToString
            End If
            For Each r As DataRow In mDt.Rows
                Dim lTablerow As New TableRow
                Dim tblHeaderRow As New TableHeaderRow
                Dim tblHeaderCell As New TableHeaderCell
                'CODE
                Dim lTableCell As New TableCell
                Dim ctl As Label = New Label
                Dim Countstring As String = Right("0000" & lStartRowNumber.ToString, 2)
                ctl.ID = "lbl_Lang_" & Countstring & "_Code"
                lTableCell = New TableCell
                lTableCell.CssClass = "mc-dfrm-EditTextsView-tblcellLeft"
                ctl.CssClass = "mc-dfrm-EditTextsView-Label"
                lTableCell.Controls.Add(ctl)
                lTablerow.Cells.Add(lTableCell)
                'tblHeaderRow = New TableHeaderRow
                tblHeaderCell = New TableHeaderCell
                tblHeaderCell.Text = ""
                tblHeaderRow.Controls.Add(tblHeaderCell)
                'Language Text
                ctl = New Label
                Countstring = Right("0000" & lStartRowNumber.ToString, 2)
                ctl.ID = "lbl_Lang_" & Countstring & "_LanguageText"
                lTableCell = New TableCell
                lTableCell.CssClass = "mc-dfrm-EditTextsView-tblcellCenter"
                ctl.CssClass = "mc-dfrm-EditTextsView-Label"
                lTableCell.Controls.Add(ctl)
                lTablerow.Cells.Add(lTableCell)
                'tblHeaderRow = New TableHeaderRow
                tblHeaderCell = New TableHeaderCell
                tblHeaderCell.Text = ""
                tblHeaderRow.Controls.Add(tblHeaderCell)

                'TextBox
                '---------------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                ' Comment   : New concept for multiple text functionality
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-03-24
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID <> "COMPOSITE_MULTI_TXT" Then
                    Dim ctlText As TextBox = New TextBox
                    Countstring = Right("0000" & lStartRowNumber.ToString, 2)
                    ctlText.ID = "txt_Edit_Lang_" & Countstring
                    lTableCell = New TableCell
                    lTableCell.CssClass = "mc-dfrm-EditTextsView-tblcellRight"
                    ctl.CssClass = "mc-dfrm-EditTextsView-TB"
                    lTableCell.Controls.Add(ctlText)
                    'OTT 2190
                    If Not dtTransField Is Nothing And dtTransField.Rows.Count > 0 Then
                        strTransField = dtTransField.Rows(0)("TRANSLATION_FIELD_ID").ToString
                    End If
                    If mUser.RequestType <> myUser.RequestTypeOption.insert And strDescription <> strCurrDescription And bIsLabelExists = False Then
                        Dim lblTransText As New Label
                        lblTransText.ID = "lbl_Lang_" & Countstring & "Col_" & "TransText"
                        lblTransText.CssClass = "mc-dfrm-EditTextsView-Label"
                        lTableCell.Controls.Add(lblTransText)
                    ElseIf mUser.RequestType = myUser.RequestTypeOption.insert And strTransField <> "" Then
                        Dim lblTransText As New Label
                        lblTransText.ID = "lbl_Lang_" & Countstring & "Col_" & "TransText"
                        lblTransText.CssClass = "mc-dfrm-EditTextsView-Label"
                        lTableCell.Controls.Add(lblTransText)
                    End If
                    lTablerow.Cells.Add(lTableCell)
                    'tblHeaderRow = New TableHeaderRow
                    tblHeaderCell = New TableHeaderCell
                    tblHeaderCell.Text = ""
                    tblHeaderRow.Controls.Add(tblHeaderCell)
                    'For i = 1 To mDt.Rows.Count
                    'Dim row As New TableRow
                ElseIf mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                    For Each dtCol As DataColumn In r.Table.Columns
                        If Not (dtCol.ColumnName.ToUpper = "RANK" Or _
                            dtCol.ColumnName.ToUpper = "LANGUAGE_ISO_CODE" Or _
                            dtCol.ColumnName.ToUpper = "LANGUAGE_ISO_CODE_DESCRIPTION" Or _
                            dtCol.ColumnName.ToUpper = "OBJ_VERSIONNUMBER") Then
                            lTableCell = New TableCell

                            tblHeaderCell = New TableHeaderCell
                            tblHeaderCell.Text = dtCol.ColumnName
                            tblHeaderRow.Controls.Add(tblHeaderCell)

                            Dim tb As New TextBox
                            tb.ID = "txt_Edit_Multi_Lang_" & Countstring & "Col_" & dtCol.Ordinal - 1
                            lTableCell.CssClass = "mc-dfrm-EditTextsView-tblcellRight"
                            ctl.CssClass = "mc-dfrm-EditTextsView-TB"
                            lTableCell.Controls.Add(tb)
                            'OTT 2190
                            If Not dtTransField Is Nothing And dtTransField.Rows.Count > 0 Then
                                strTransField = dtTransField.Rows(0)("TRANSLATION_FIELD_ID").ToString
                            End If
                            If mUser.RequestType <> myUser.RequestTypeOption.insert And dtCol.ColumnName = strTransField And strDescription <> strCurrDescription And bIsLabelExists = False Then
                                Dim lblTransText As New Label
                                lblTransText.ID = "lbl_Lang_" & Countstring & "Col_" & dtCol.Ordinal - 1 & "TransText"
                                lblTransText.CssClass = "mc-dfrm-EditTextsView-Label"
                                lTableCell.Controls.Add(lblTransText)
                            ElseIf mUser.RequestType = myUser.RequestTypeOption.insert And dtCol.ColumnName = strTransField Then
                                Dim lblTransText As New Label
                                lblTransText.ID = "lbl_Lang_" & Countstring & "Col_" & dtCol.Ordinal - 1 & "TransText"
                                lblTransText.CssClass = "mc-dfrm-EditTextsView-Label"
                                lTableCell.Controls.Add(lblTransText)
                            End If
                            lTablerow.Cells.Add(lTableCell)
                        End If
                    Next

                End If
                ' Reference END : CR ZHHR 1038241
                '----------------------------------------------------------------------------------------

                If tblTexts.Rows.Count = 0 Then
                    tblTexts.Rows.Add(tblHeaderRow)
                End If
                tblTexts.Rows.Add(lTablerow)
                'Next
                'tblTexts.Rows.Add(lTablerow)
                lStartRowNumber = lStartRowNumber + 1
                bIsLabelExists = True
            Next r
            ' Reference END : CR ZHHR 1054864
            '----------------------------------------------------------------------------------------------
        Catch ex As Exception
            lblStatus.Text = "MAKETABLEROW:" & ex.Message
        End Try
    End Sub

    Private Function DetailsMenuSelect(ByVal lWithPages As Boolean) As Boolean
        trvOBJ.Nodes.Clear()
        Dim lSuccess As Boolean = False
        Try
            Dim lWiz_Id As String = ""
            If Not mnuDetailsMenu.SelectedValue = "" Then
                lWiz_Id = mnuDetailsMenu.SelectedItem.Text
            End If
            mDynamicFormController = Nothing
            grdDat.DataSource = Nothing

            Dim lFactory As New Dynamic_View_Controller_Factory
            mControllerfactory = lFactory
            lFactory.User = mUser
            If mDynamicFormController Is Nothing Then
                With lFactory
                    .TOPIC_ID = mnuDetailsMenu.SelectedValue
                    .TOPIC_GROUP_ID = mnuNavigate.SelectedValue
                    If Not Request.Params("CONTEXT") Is Nothing Then
                        m_Topic_Group_Context_ID = Request.Params("CONTEXT").ToString
                    End If
                    .TOPIC_GROUP_CONTEXT_ID = m_Topic_Group_Context_ID
                    .Request = Me.Request
                    .LoadTree(trvOBJ)

                    .IsPostback = Me.IsPostBack
                End With

                If trvOBJ.SelectedNode Is Nothing Then
                    If trvOBJ.Nodes.Count <> 0 Then
                        'PARAMS HIER CHECKEN
                        If Not Request.Params("PATH") Is Nothing Then
                            Dim myPathObj As Array = Request.Params("PATH").ToString.ToUpper.Split("/")
                            For Each lNode As TreeNode In trvOBJ.Nodes
                                If lNode.Value.ToString.ToUpper = myPathObj(0).ToString.ToUpper Then
                                    lNode.Selected = True
                                End If
                            Next
                        End If

                        If trvOBJ.SelectedNode Is Nothing Then
                            trvOBJ.Nodes(0).Selected = True
                            Me.trvOBJ_SelectedNodeChanged(Nothing, Nothing)
                            lSuccess = True
                        End If

                        lWiz_Id = trvOBJ.SelectedValue
                        Dim lCriteriaArgument As String = trvOBJ.SelectedValue
                        Dim lCurrentObjArr As Array = lCriteriaArgument.Split("\")
                        Dim lCurrentObj As String = lCurrentObjArr(lCurrentObjArr.GetUpperBound(0))
                        mUser.GBOXmanager.SetCurrentObj(lCurrentObj, mUser)
                    End If
                End If
                mDynamicFormController = lFactory.SelectedNodeChange(trvOBJ.SelectedNode)
            End If
            If mDynamicFormController Is Nothing Then
                Return False
            End If
            mDynamicFormController.Request = Me.Request
            mDynamicFormController.IsPostback = Me.IsPostBack
            Dim lController As String = ""
            '---------------------------------------------------------------------
            ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
            ' Comment   : Placed new Add value button to create new request
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-12-24
            If TypeOf mDynamicFormController Is Dynamic_View_Reporter Then
                lSuccess = mDynamicFormController.BindView(vwQuery, lWithPages)
                lController = "Dynamic_View_Reporter"
                btnAddValue.Enabled = False
                imgAlert.Enabled = False
                '---------------------------------------------------------------------
                ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
                ' Comment   : Run report visible for reporter else not visible
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-12-24
                imgQuery.Visible = True
            End If
            If TypeOf mDynamicFormController Is Dynamic_View_Requester Then
                lSuccess = mDynamicFormController.BindView(vwGridView, lWithPages)
                lController = "Dynamic_View_Requester"
                btnAddValue.Enabled = True
                imgQuery.Visible = False
            End If
            ' Reference END : CR ZHHR 1050708
            '---------------------------------------------------------------------
            If TypeOf mDynamicFormController Is Dynamic_View_Documentator Then
                btnAddValue.Enabled = False
                imgAlert.Enabled = False
            End If
            ' Reference END : CR ZHHR 1050708
            '---------------------------------------------------------------------
            LoadWizardData(lWiz_Id)
            Return lSuccess
        Catch ex As Exception
            mErrText &= "DetailsmenuSelect" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
            Return lSuccess
        End Try
    End Function
    Private Function MakeTopicGroupMenu() As Boolean
        Try
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
            End If
            mnuNavigate.Items.Clear()
            Dim lTopicgroups As DataTable
            If Not Request.Params("CONTEXT") Is Nothing Then
                If Request.Params("CONTEXT").ToUpper = "ADMIN".ToUpper And Not mUser.GBOXmanager.IsGBoxAdmin(mUser.CW_ID) Then
                    Return False
                End If
                m_Topic_Group_Context_ID = Request.Params("CONTEXT").ToString()
                lTopicgroups = mUser.Databasemanager.MakeDataTable("Select Topic_Group_ID,MENU_TEXT,TOOL_TIP_TEXT from Topic_Group where TOPIC_GROUP_CONTEXT_ID='" & m_Topic_Group_Context_ID & "' And Active=1 order by Rank")
            Else
                lTopicgroups = mUser.Databasemanager.MakeDataTable("Select Topic_Group_ID,MENU_TEXT,TOOL_TIP_TEXT from Topic_Group where TOPIC_GROUP_CONTEXT_ID='GENERAL' And Active=1 order by Rank")

            End If
            Dim lMenuToSelect As String = ""
            If Not Request.Params("Topicgroup") Is Nothing Then
                lMenuToSelect = Request.Params("Topicgroup").ToString
            End If
            For Each r As DataRow In lTopicgroups.Rows
                Dim lmenuItem As New MenuItem(r("MENU_TEXT").ToString)
                lmenuItem.ToolTip = r("TOOL_TIP_TEXT").ToString
                lmenuItem.Value = r("Topic_Group_ID").ToString
                mnuNavigate.Items.Add(lmenuItem)
            Next r
            Dim lTopicGroupId As String = ""
            For z As Integer = 0 To mnuNavigate.Items.Count - 1
                If mnuNavigate.Items(z).Text.ToUpper = lMenuToSelect.ToString.ToUpper Then
                    mnuNavigate.Items(z).Selected = True
                    lTopicGroupId = mnuNavigate.Items(z).Value
                    Exit For
                End If
            Next z

            MakeTopicsMenu(lTopicGroupId)

            If mnuDetailsMenu.SelectedItem Is Nothing Then
                ResetView()
            Else
                LoadWizardData(mnuNavigate.SelectedItem.Text)
            End If

            Return True
        Catch ex As Exception
            mErrText &= "MakeTopicGroupMenu" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            mErrText &= ""
            Return False
        End Try
    End Function
#End Region
#Region "Subs And Functions - PAGE LOAD"
    Function FillCombo(ByRef lcmb As DropDownList, ByVal lSelect As String, ByVal lWhere As String) As Integer
        Dim dt As DataTable = Nothing
        Dim lSql As String = ""
        Dim lW As String = ""
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1047822 - GBOX Cockpit: GBOX request with wrong ORG_LEVEL_VALUE
        ' Comment           : Comment query and add new query 
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 01-Sep-2015
        'lSql = "SELECT DISTINCT " & lSelect & " FROM AUTHORISATION_SET WHERE CW_ID = '" & mUser.CW_ID & "' AND APPLICATION_ID = 'GBOX' AND APPLICATION_PART_ID = 'DRS' AND APPLICATION_ROLE_ID = 'REQUESTER'"
        lSql = "SELECT DISTINCT ASET." & lSelect & " FROM AUTHORISATION_SET ASET " & _
                "INNER JOIN wf_DEFINE_WORKFLOW_DETAILS WD ON WD.ORG_LEVEL_ID = ASET.ORG_LEVEL_ID " & _
                "AND WD.ORG_LEVEL_VALUE = ASET.ORG_LEVEL_VALUE  " & _
                "WHERE ASET.CW_ID ='" & mUser.CW_ID & "' AND ASET.APPLICATION_ID = 'GBOX' " & _
                "AND ASET.APPLICATION_PART_ID = 'DRS' AND ASET.APPLICATION_ROLE_ID = 'REQUESTER' AND WD.ROLE_CLUSTER_ID = 'GBOX_REQUESTER' " & _
                "AND WD.OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "'"

        lW = lWhere
        If (lW <> "") Then
            lW = lW.Replace("ORG_LEVEL_ID", "WD.ORG_LEVEL_ID")
        End If

        lSql = lSql & lW
        lW = lWhere
        lSql = lSql & " UNION " & _
                  "SELECT DISTINCT " & lSelect & " FROM wf_DEFINE_WORKFLOW_DETAILS WD WHERE WD.public_flag = 1 AND WD.OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "' AND WD.ROLE_CLUSTER_ID = 'GBOX_REQUESTER'"
        If (lW <> "") Then
            lW = lW.Replace("AND ORG_LEVEL_ID", "AND WD.ORG_LEVEL_ID")
        End If
        lSql = lSql & lW

        dt = mUser.Databasemanager.MakeDataTable(lSql)
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1040447 - GBOX COC: No display of DRS request to public users
        ' Comment           : Public bayer user can create a DRS request.
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 24-Mar-2015
        If (dt.Rows.Count = 0) Then
            If lSelect = "ORG_LEVEL_ID" Then
                lSql = " SELECT distinct " & lSelect & " FROM wf_DEFINE_WORKFLOW_DETAILS WHERE ROLE_CLUSTER_ID ='GBOX_REQUESTER' AND STATION_ACTION_TEXT LIKE '%DRS%' AND PUBLIC_FLAG=1 AND OBJ_ID ='" & mUser.Current_OBJ.OBJ_ID & "'"
            Else
                lSql = " SELECT distinct " & lSelect & " FROM wf_DEFINE_WORKFLOW_DETAILS WHERE ROLE_CLUSTER_ID ='GBOX_REQUESTER' AND STATION_ACTION_TEXT LIKE '%DRS%' AND PUBLIC_FLAG=1 AND OBJ_ID ='" & mUser.Current_OBJ.OBJ_ID & "'" & lWhere
            End If

            dt = Nothing
            dt = mUser.Databasemanager.MakeDataTable(lSql)
        End If
        'Reference End      : ZHHR 1040447

        lcmb.Items.Clear()
        With lcmb
            .DataSource = dt
            .DataTextField = lSelect
            .DataValueField = lSelect
            .DataBind()
        End With
        'If dt.Rows.Count > 1 Then
        lcmb.Items.Insert(0, "Please Select ...")
        'End If
        If Not dt Is Nothing Then
            Return dt.Rows.Count
        Else
            Return 0
        End If
        ' Reference End     : ZHHR 1047822
    End Function
    Public Function Authenticate_User() As myUser
        If Context.User.Identity.Name = "" Then
            Dim lbl As Label = Me.FindControl("Label" & 1)
            lbl.Text = "Can not identify user. Problems with persisting windows authentification. Activate windows persist security on the server."
            ' Reference         : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
            ' Comment           : Hide the wizard area
            ' Added by          : Pratyusa Lenka (CWID : EOJCG)
            ' Date              : 07-11-2018
            'mvWizard.Visible = True
            'Dim mnuWizz As New MenuItem
            'mnuWizz.Text = "Problem with Domain Controller"
            'mnuWizz.Value = 1
            'mnuWizzard.Items.Add(mnuWizz)
            'If mnuWizzard.Items.Count > 0 Then
            '    mnuWizzard.Items(0).Selected = True
            'End If
            'mvWizard.ActiveViewIndex = 0
            Return Nothing
        End If
        Dim lContextUser As String = ""
        If txtImpersonate.Text = "" Then
            lContextUser = Context.User.Identity.Name
        Else
            lContextUser = txtImpersonate.Text
        End If
        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        pObjCurrentUsers.CONTEXT = Context
        If InStr(lContextUser, My.Computer.Name) <> 0 Then
            lblInformations.Text = "This user:" & lContextUser & " is not valid please log in at your network domain e.g. BYACCOUNT\USERNAME "
            Return Nothing
        End If
        mUser = pObjCurrentUsers.GetCurrentUserByCwId(lContextUser)
        If mUser Is Nothing Then
            ' ----------------------------------------------------------------------------------------
            ' Reference  : BY-RZ04 - CMT - 28633 - 110IM08222136,110IM08175309 - "Unknown User error in accessing GBOX"
            ' Comment    : Add Error information to user if he is not able able to login to GBox site
            ' Created by : EQIZU
            ' Date       : 06-NOV-2013
            ' ---------------------------------------------------------------------------------------
            pVarErrMsg = pVarErrMsg & "Cockpit: User Authentication failed " & vbCrLf
            lblInformations.Text = pGetErrorMsg() & pVarErrMsg
            pVarErrMsg = ""

            Return Nothing

        Else

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035817 - GBOX WebForms: Add New Function for updating columns
            ' Comment           : Added code for updating new columns in User table
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-12-15
            mUser.UserAccessStatus(mUser.CW_ID, "GBOX COC")
            ' Reference  END    : CR ZHHR 1035817

        End If
        Return mUser
    End Function
    '---------------------------------------------------------------------------------------------------
    ' Reference         : CR 1027842 - GBOX COC: new context needed 
    ' Comment           : Check if User is having KPI Role while accessing KPI context
    ' Added by          : Surendra Purav (CWID : EQIZU)
    ' Date              : 2014-05-21
    '---------------------------------------------------------------------------------------------------
    Private Function IsKPIContextRoleExists() As Boolean
        Dim lKPIRole As Boolean = False
        'Check for access                    
        Dim lApproles As String = mUser.GBOXmanager.Authorisation("GBOX", "ADMINISTRATION", mUser.CW_ID)
        If InStr(lApproles.ToUpper, "KPI-VIEWER") <> 0 Then
            lKPIRole = True
        End If
        Return lKPIRole
    End Function

    Private Function WelcomeWagon() As Boolean
        Try
            '---------------------------------------------------------------------------------------------------
            ' Reference         : ZHHR 1039680 - GBOX COC: Move Database Information on screen
            ' Comment           : Move database information on screen
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2015-03-05

            'lblDatabase.Text = " | Database: " & mUser.Databasemanager.cnSQL.Database & " | IIS: " & Me.Context.Server.MachineName & " | " & mConstVersion
            'lblDatabase.Text = " | IIS: " & Me.Context.Server.MachineName & " | " & mConstVersion
            lblDatabase.Text = "IIS: " & Me.Context.Server.MachineName & " | " & mConstVersion
            lblDatabasename.Text = " Database: " & mUser.Databasemanager.cnSQL.Database
            ' Reference End     : ZHHR 1039680

            imgHelp.OnClientClick = "javascript:window.open('http://sp-appl-bbs.bayer-ag.com/sites/010045/Cockpit_Wiki/Home.aspx', null , 'toolbar=yes,location=yes, status= yes,menubar=yes, scrollbars=yes,resizable=yes, width=300,height=500'); void('');"
            ' ----------------------------------------------------------------------------------------
            ' Reference  : BY-RZ04 - CMT - 28993 - 110IM08249009 - Columns are shifted using button "Cancel action"
            ' Comment    : Changed the parameter text from "CANCEL" to "imgCancel"
            ' Created by : EQIZU
            ' Date       : 03-DEC-2013
            ' ---------------------------------------------------------------------------------------
            imgCancel.OnClientClick = "javascript:postBackByObject('imgCancel')"
            imgHotline.OnClientClick = "javascript:window.open('http://" & pConstServername & "/HOTLINE/" & "', null , 'toolbar=yes,location=yes, status= yes,menubar=yes, scrollbars=yes,resizable=yes, width=300,height=500'); void('');"
            lblInformations.Text = "Welcome " & mUser.first_name & " " & mUser.last_name & " | " & mUser.SUBGROUP_ID & ""
            Dim lIsadmin As Boolean = mUser.GBOXmanager.IsGBoxAdmin(mUser.CW_ID)
            lblImpersonate.Visible = lIsadmin
            txtImpersonate.Visible = lIsadmin
            cmdImpersonate.Visible = lIsadmin
            lblTechnicalInfo.Text = ""
            If lIsadmin Then
                'lblInformations.Text = lblInformations.Text & " | G|Box-Administrator" & " | User online: " & pObjCurrentUsers.GBOX_Users.Count
                'lblInformations.ToolTip = pObjCurrentUsers.AllUsers
                ' Reference : YHHR 2034863 - GBOX:Switch database connection to BARDO
                ' Comment   : Display SQL server name 
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2018-11-28
                lblDatabase.Text &= " | Linkserver:" & pConstServername & " |  SQL Server:" & mUser.Databasemanager.MySqlDatabase
            End If
            ' Reference         : YHHR 2035581 - OTT 6615/6616: GBox legacy improvements
            ' Comment           : Move technical info to a new section in the very bottom of the cockpit screen
            ' Added by          : Pratyusa Lenka (CWID : EOJCG)
            ' Date              : 12-11-2018
            lblDatabase.Visible = False
            lblTechnicalInfo.Text = "Technical information: <br />" & lblDatabase.Text
            If lIsadmin Then
                lblTechnicalInfo.Text &= "<br />" & "Your role: G|Box-Administrator"
                lblTechnicalInfo.Text &= "<br />" & "User online: " & pObjCurrentUsers.GBOX_Users.Count
                lblTechnicalInfo.ToolTip = pObjCurrentUsers.AllUsers
            End If

        Catch ex As Exception
            mvContents.ActiveViewIndex = GetIndexByViewName("vwError")
            lblStatus.Text = "WelcomeWagon:" & ex.Message
        End Try
    End Function
    Private Function GetIndexByViewName(ByVal lViewName As String) As Integer
        Select Case lViewName
            Case "vwGridView"
                Return 0
            Case "vwDetails"
                Return 1
            Case "vwQuery"
                Return 2
            Case "vwEditTexts"
                Return 3
            Case "vwEditSysthems"
                Return 4
            Case "vwSql"
                Return 5
            Case "vwSubscribe"
                Return 6
            Case "vwSearchEngine"
                Return 7
            Case "vwCPS"
                Return 8
        End Select
    End Function
    Private Function LetTheImageBeClicked(lCriteriaTarget As String) As Boolean
        Dim lBuildtexts As Boolean
        Select Case lCriteriaTarget
            Case "imgWiki", "imgEditCustomizingObj", "imgEditDocumentation"
                ChangeToCpsView()
            Case "imgNewCustomizingObj", "imgNewDocumentationObject"
                imgNewCustomizingObj.Enabled = False
                imgNewDocumentationObject.Enabled = False
                mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")

            Case "imgSubmitCustomizingObj"
                imgNewCustomizingObj.Enabled = True
                imgNewDocumentationObject.Enabled = True
                mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")
            Case "imgSubmitDocumentation"
                imgNewCustomizingObj.Enabled = True
                imgNewDocumentationObject.Enabled = True
                ChangeToCpsView()
            Case "imgEngine", "imgSearchEngine", "imgAppend"
                Exit Function
            Case "imgQuery"
                LoadFactoryAndControllerAndBind(mWithPaging)
                mDynamicFormController.ShowQuery(grdQuery)
                Exit Function
                '---------------------------------------------------------------------
                ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
                ' Comment   : Placed new Add value button to create new request
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-12-24
            Case "btnAddValue"
                btnSubmit.CommandArgument = "New"
                mUser.EditMode = True
                mUser.RequestType = myUser.RequestTypeOption.insert
                LoadFactoryAndControllerAndBind(True)
                opennew()
                cmborglevel.Items.Clear()
                cmborglevelvalue.Items.Clear()
                Dim ret As Integer = FillCombo(cmborglevel, "ORG_LEVEL_ID", "")
                If ret = 1 Then
                    FillCombo(cmborglevelvalue, "ORG_LEVEL_VALUE", " AND ORG_LEVEL_ID = '" & cmborglevel.SelectedValue & "'")
                End If
                ' Reference END : CR ZHHR 1050708
                '---------------------------------------------------------------------
                'Macht die Validatoren an
                '------------------------------------------------------------------------------
                'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
                'Comment    : On third mask image buttons are replaced by button controls
                'Added by   : Pratyusa Lenka (CWID : EOJCG)
                'Date       : 2016-02-16
            Case "btnTextFill", "btnOverwriteEnglish"
                lBuildtexts = False
                LoadFactoryAndControllerAndBind(mWithPaging)
                If btnSubmit.CommandArgument = "New" Then
                    BindDetails(mWithPaging, True)
                Else
                    BindDetails(mWithPaging, False)
                End If
                Dim lcurrentkeyvalue As String = lblObj_VALUE.Text
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Then
                    BuildTextgrid_TXT()
                    '---------------------------------------------------------------------------------------
                    ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                    ' Comment   : New concept for multiple text functionality
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2015-03-24
                ElseIf mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                    BuildTextgrid_TXT()
                    ' Reference END : CR ZHHR 1038241
                    '----------------------------------------------------------------------------------------
                Else
                    'BuildTextgrid(lcurrentkeyvalue)
                End If
                ChangeIcons()
                mvContents.ActiveViewIndex = GetIndexByViewName("vwEditTexts")
                ' Reference END : CR ZHHR 1053558
                '------------------------------------------------------------------------------
            Case "imgCancel"
                mErrText = ""
                lblStatus.Text = ""
                mUser.GBOXmanager.ErrString = ""
                mUser.Databasemanager.ErrText = ""
                mUser.Databasemanager.cnSQL.Close()
            Case Else
                mUser.EditMode = True
                btnSubmit.CommandArgument = "Update"
                mUser.RequestType = myUser.RequestTypeOption.update
                LoadFactoryAndControllerAndBind(mWithPaging)
                BindDetails(mWithPaging, True)
        End Select
        Return lBuildtexts
    End Function
#End Region
#Region "CPS"
#Region "Subs And Functions"
    Private Sub ShowEditNewDocumentationObject()
        FillCPSVIEW(mUser.Current_OBJ.OBJ_ID)
        FillCustomizingOnly()
        mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")
        EnableDocumentationFields(True)
        imgEditDocumentation.Enabled = False
        imgEditCustomizingObj.Enabled = False
        imgSubmitDocumentation.Enabled = True
        ChangeIcons()
        Dim txt As TextBox = Me.FindControl("txt_OBJ_GUID")
        If Not txt Is Nothing Then
            txt.Text = mUser.Databasemanager.GetGUID
            txt.Enabled = False
        End If
        txt = Me.FindControl("txt_OBJ_VERSIONNUMBER")
        If Not txt Is Nothing Then
            txt.Text = "0"
            txt.Enabled = False
        End If
        tblNewNode.Visible = True
        imgCancel.Enabled = True
        ChangeIcons()
    End Sub
    Private Sub ChangeToCpsView()

        Select Case mUser.Current_OBJ.OBJ_CLASSIFICATION_ID
            Case "CPS_NODE"
                mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")
                FillCPSVIEW(mUser.Current_OBJ.OBJ_ID)
            Case "CPS_OBJ_ATTR_OLD"
                mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")
                FillCPSVIEW(mUser.Current_OBJ.OBJ_ID, mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
            Case "CPS_OBJ_ATTR"
                mvContents.ActiveViewIndex = GetIndexByViewName("vwCPS")
                FillCPSVIEW(mUser.Current_OBJ.OBJ_ID)
        End Select
        EnableCustomizingObj(False)
    End Sub
    Public Sub EnableDocumentationFields(ByVal lValue As Boolean)
        For i = 0 To lstCurrentDocControls.Items.Count - 1
            Dim lstr As String = lstCurrentDocControls.Items(i).Text.ToString
            Dim ctl As Control = Me.FindControl(lstr)
            If TypeOf ctl Is TextBox Then
                DirectCast(ctl, TextBox).Enabled = lValue
            End If
            If TypeOf ctl Is DropDownList Then
                DirectCast(ctl, DropDownList).Enabled = lValue
            End If
        Next i
    End Sub
    Public Function CopyCPSObjDataChangesToTables() As Boolean
        Dim lFields As New List(Of String)
        Dim lValues As New List(Of String)
        Dim lPackage As New List(Of String)
        Dim lGbox_ID As String = mUser.GBOXmanager.GetGBOXId("", "_CPS")
        mUser.Current_Request_ID = lGbox_ID
        Dim lInsertNewSQL As String = "Insert Into OBJ_CPS"
        Dim lInsertFieldList As String = "("
        Dim lInsertValueList As String = "("
        'Tatsächliche FElder
        Dim lsql As String = "Select * from OBJ_CPS where [OBJ_CPS_ID]='" & txtCustomizingObjName.Text & "'"
        Dim lVersion As String = lblVersionnumberCust.Text
        If lVersion = "" Then lVersion = 0
        Dim lSolutionType As String = ";"
        If chkALL.Checked Then lSolutionType = "Full;"
        If chkSO.Checked Then lSolutionType &= "SD;"
        If chkFi.Checked Then lSolutionType &= "FI;"
        lSolutionType = Left(lSolutionType, Len(lSolutionType) - 1)
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        Dim lList As String = "[OBJ_VERSIONNUMBER],[MANDATORY_TYPE],[SOLUTION_TYPE],LOCKEDTIME, Locked, REQUEST_DATE, REQUEST_ID, Published"
        For Each c As DataColumn In ldt.Columns
            If InStr(lList.ToUpper, c.ColumnName.ToUpper) = 0 Then
                lInsertFieldList &= c.ColumnName & ","
                lFields.Add(c.ColumnName)
                If ldt.Rows.Count <> 0 Then
                    lInsertValueList &= "'" & ldt.Rows(0)(c.ColumnName).ToString & "',"
                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : ZHHR 1047686- GBOX COC: Cannot create DRS request
                    ' Comment           : Added replace function below
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 28-Aug-2015
                    '' Reference : YHHR 2036565 - GBox: Single Quote Issue
                    '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
                    '' Added by  : Rajan Dmello (CWID : EOLRG) 
                    '' Date      : 2018-10-30
                    lValues.Add(ldt.Rows(0)(c.ColumnName).ToString.Replace("'", "''"))
                Else
                    Select Case c.ColumnName
                        Case "OBJ_CPS_ID"
                            lInsertValueList &= "'" & txtCustomizingObjName.Text.Replace("'", "''") & "',"
                            lValues.Add(txtCustomizingObjName.Text.Replace("'", "''"))
                        Case "WIKI_NAME"
                            lInsertValueList &= "'" & txtWikiName.Text.Replace("'", "''") & "',"
                            lValues.Add(txtWikiName.Text.Replace("'", "''"))
                        Case "WIKI_URL"
                            lInsertValueList &= "'" & txtWikiUrl.Text.Replace("'", "''") & "',"
                            lValues.Add(txtWikiUrl.Text.Replace("'", "''"))
                        Case Else
                            lInsertValueList &= "'',"
                            lValues.Add(" ")
                    End Select
                    ' Reference End     : ZHHR 1047686
                End If
            End If
            If c.ColumnName = "OBJ_VERSIONNUMBER" Then
                lVersion = lVersion + 1
            End If
        Next
        lInsertFieldList &= "OBJ_VERSIONNUMBER,MANDATORY_TYPE,SOLUTION_TYPE,LOCKEDTIME, Locked, REQUEST_DATE, REQUEST_ID, Published"
        lInsertValueList &= "'" & lVersion & "','" & cboMandatoryType.SelectedValue.ToString & "','" & lSolutionType & "',getdate(), '" & lGbox_ID & "', getdate(),'" & lGbox_ID & "','0'"
        lInsertFieldList &= ")"
        lInsertValueList &= ")"
        lInsertNewSQL = lInsertNewSQL & lInsertFieldList & " values " & lInsertValueList
        lFields.Add("OBJ_VERSIONNUMBER")
        lFields.Add("MANDATORY_TYPE")
        lFields.Add("SOLUTION_TYPE")
        lFields.Add("LOCKEDTIME")
        lFields.Add("Locked")
        lFields.Add("REQUEST_DATE")
        lFields.Add("REQUEST_ID")
        lFields.Add("Published")
        lValues.Add(lVersion)
        lValues.Add(cboMandatoryType.SelectedValue.ToString)
        lValues.Add(lSolutionType)
        lValues.Add(Now().ToString)
        lValues.Add(lGbox_ID)
        lValues.Add(Now().ToString)
        lValues.Add(lGbox_ID)
        lValues.Add("0")


        lPackage.Add(CopyDatasetFromTableToTableSql("OBJ_CPS", "OBJ_CPS_HISTORY", " where [OBJ_CPS_ID]='" & txtCustomizingObjName.Text & "'"))
        lPackage.Add("Delete from OBJ_CPS  where [OBJ_CPS_ID]='" & txtCustomizingObjName.Text & "'")
        lPackage.Add(lInsertNewSQL)
        mUser.GBOXmanager.SetCurrentObj(txtCustomizingObjName.Text, mUser)
        mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "CPS_OBJ"
        lPackage.AddRange(StartWorkflow(lFields, lValues))
        If mUser.Databasemanager.ExecutePackage(lPackage) Then
            lblStatus.Text = "The package is saved with GBOX_ID: " & lGbox_ID
            Return True
        Else
            lblStatus.Text = mUser.Databasemanager.ErrText
            Return False
        End If
    End Function
    Public Function CopyCPSAttribDataChangesToTables(ByVal lObj_ID As String) As Boolean

        Dim lPackage As New List(Of String)
        Dim lsql As String = ""
        lsql &= "Select OBJ_CPS_ATTR.*,TOPIC_OBJ_OBJS.* "
        lsql &= "from TOPIC_OBJ_OBJS "
        lsql &= "Left join OBJ_CPS_ATTR on [TOPIC_OBJ_OBJS].CHILD_OBJ_ID = OBJ_CPS_ATTR.OBJ_GUID "
        lsql &= "left Join OBJ_Field on OBJ_CPS_ATTR.OBJ_CPS_ID=OBJ_Field.OBJ_ID "
        lsql &= "where [CHILD_OBJ_ID]='" & lObj_ID & "' order By OBJ_Field.Ordinal_Position"

        Dim docObjDt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        If docObjDt.Rows.Count = 0 Then
            lblStatus.Text = lsql & " is EMPTY"
        End If
        Dim r As DataRow = (docObjDt.Rows(0))
        Dim lInsertNewSQL As String = "Insert Into OBJ_CPS_ATTR "
        Dim lInsertFieldList As String = "("
        Dim lInsertValueList As String = "("
        Dim lClassification As String = mUser.Current_OBJ.OBJ_CLASSIFICATION_ID
        If lClassification = "CPS_NODE" Then
            lClassification = "CPS_OBJ_ATTR"
        End If
        Dim lFields As New List(Of String)
        Dim lValues As New List(Of String)


        For Each c As DataColumn In r.Table.Columns

            Dim isDisplayed As String = mUser.GBOXmanager.GetDisplayNameByObjFieldId(lClassification, c.ColumnName)
            If isDisplayed <> "" Then
                Dim ctl As Control = tblCPS.FindControl("txt_" & c.ColumnName)
                Dim txt As TextBox
                If TypeOf ctl Is TextBox Then
                    txt = ctl
                Else
                    txt = New TextBox
                    txt.ID = DirectCast(ctl, DropDownList).ID
                    txt.Text = DirectCast(ctl, DropDownList).Text
                End If


                lInsertFieldList &= "[" & txt.ID.Replace("txt_", "") & "],"
                lFields.Add(txt.ID.Replace("txt_", ""))
                If InStr(txt.ID, "OBJ_VERSIONNUMBER") <> 0 Then
                    txt.Text = txt.Text + 1
                End If
                If InStr(txt.ID, "OBJ_CPS_ID") <> 0 Then
                    If txtCustomizingObjName.Text <> "" Then
                        txt.Text = txtCustomizingObjName.Text
                    Else
                        txt.Text = cboCustomizingObjName.Text
                    End If
                End If

                lInsertValueList &= "'" & txt.Text & "',"
                '---------------------------------------------------------------------------------------------------
                ' Reference         : ZHHR 1047686- GBOX COC: Cannot create DRS request
                ' Comment           : Added replace function below
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 28-Aug-2015
                '' Reference : YHHR 2036565 - GBox: Single Quote Issue
                '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
                '' Added by  : Rajan Dmello (CWID : EOLRG) 
                '' Date      : 2018-10-30
                lValues.Add(txt.Text.Replace("'", "''"))
                ' Reference End     : ZHHR 1047686
            End If
        Next
        Dim lGbox_ID As String = mUser.GBOXmanager.GetGBOXId("", "_CPS")
        mUser.Current_Request_ID = lGbox_ID
        lInsertFieldList &= "LOCKEDTIME, Locked, REQUEST_DATE, REQUEST_ID,Published"
        lInsertValueList &= "getdate(), '" & lGbox_ID & "', getdate(),'" & lGbox_ID & "','0'"

        lInsertFieldList &= ")"
        lInsertValueList &= ")"
        lInsertNewSQL = lInsertNewSQL & lInsertFieldList & " values " & lInsertValueList
        If tblNewNode.Visible Then
            Dim txt As TextBox = tblCPS.FindControl("txt_OBJ_GUID")
            lObj_ID = txt.Text
        End If
        lPackage.Add(CopyDatasetFromTableToTableSql("OBJ_CPS_ATTR", "OBJ_CPS_ATTR_HISTORY", " Where OBJ_GUID='" & lObj_ID & "'"))
        lPackage.Add("Delete from OBJ_CPS_ATTR  Where OBJ_GUID='" & lObj_ID & "'")
        lPackage.Add(lInsertNewSQL)
        'Insert Into TopicObj_objS

        '---------------------------------------------------------------------------------------------------
        ' Reference         : CR - BY-RZ04-CMT-28707 - 110IM08174433 -  Load_Data_At_Startup
        ' Comment           : Remove the field "Loaded_First_Time" from the table "OBJ" as it is not used
        ' Added by          : Surendra Purav (CWID : EQIZU)
        ' Date              : 2013-11-12
        '---------------------------------------------------------------------------------------------------

        If tblNewNode.Visible Then
            Dim lInsertIntoObj As String = ""
            lInsertIntoObj = "INSERT INTO  Obj ("
            lInsertIntoObj &= "[OBJ_ID], "
            lInsertIntoObj &= "[OBJ_DISPLAY_NAME] ,"
            lInsertIntoObj &= "[OBJ_RESPONSIBLE_CLASSIFICATION] ,"
            lInsertIntoObj &= "[OBJ_TRANSPORT_REQUIRED] ,"
            lInsertIntoObj &= "[OBJ_DESCRIPTION] ,"
            lInsertIntoObj &= "[REQUEST_TYPE_ID] ,"
            lInsertIntoObj &= "[OBJ_CLASSIFICATION_ID] ,"
            lInsertIntoObj &= "[RANK] ,"
            lInsertIntoObj &= "[SUBGROUP_ID] ,"
            lInsertIntoObj &= "[HelpURL] ,"
            lInsertIntoObj &= "[Load_Data_At_Startup] ,"
            'lInsertIntoObj &= "[Loaded_First_Time] ,"
            lInsertIntoObj &= "[TRANSPORT_CLASSIFICATION_ID] "
            lInsertIntoObj &= ", [MULTILINE_REQUEST_POSSIBLE] "
            lInsertIntoObj &= ") VALUES ("
            lInsertIntoObj &= "N'" & lObj_ID & "', "
            lInsertIntoObj &= "'" & txtNewNode.Text & "', "
            lInsertIntoObj &= "'CPS', "
            lInsertIntoObj &= "0, "
            lInsertIntoObj &= "'" & txtNewNode.Text & "', "
            lInsertIntoObj &= "'GBOX_REQUEST', "
            lInsertIntoObj &= "'CPS_OBJ_ATTR', "
            If txtRank.Text <> "" Then
                lInsertIntoObj &= "'" & CLng(txtRank.Text) & "',"
            Else
                lInsertIntoObj &= "0, "
            End If
            lInsertIntoObj &= "'ALL', "
            lInsertIntoObj &= "'http://sp-appl-bbs.bayer-ag.com/sites/010045/Cockpit_Wiki/Home.aspx', "
            lInsertIntoObj &= "0, "
            'lInsertIntoObj &= "0, "
            lInsertIntoObj &= "'STRAND', "
            lInsertIntoObj &= "0 "
            lInsertIntoObj &= ") "
            lPackage.Add(lInsertIntoObj)

            Dim lInsertIntoTopicObj_OBJs As String = ""
            lInsertIntoTopicObj_OBJs &= "INSERT INTO [TOPIC_OBJ_OBJS]"
            lInsertIntoTopicObj_OBJs &= "([TOPIC_GROUP_CONTEXT_ID]"
            lInsertIntoTopicObj_OBJs &= ",[TOPIC_GROUP_ID]"
            lInsertIntoTopicObj_OBJs &= "  ,[TOPIC_ID]"
            lInsertIntoTopicObj_OBJs &= "  ,[PARENT_OBJ_ID]"
            lInsertIntoTopicObj_OBJs &= "  ,[CHILD_OBJ_ID]"


            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035648 - GBOX WebForms: RENAME THE COLUMN AND REMOVE ONE COLUMN FROM DB AND CODE
            ' Comment           : a) This request is for renaming column TOPIC_OBJ_OBJS_CLASSIFICATION_ID to: TREE_ICON and
            '                   : b) Also to remove [TOPIC_OBJ_OBJS_CLASSIFICATION_OBJ_ID] from table TOPIC_OBJ_OBJS and code as well. 
            '                     Both a and b changes should have no impact in any other part of the code.
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-11-24

            lInsertIntoTopicObj_OBJs &= "  ,[TREE_ICON]"
            'lInsertIntoTopicObj_OBJs &= "  ,[TOPIC_OBJ_OBJS_CLASSIFICATION_OBJ_ID]"

            ' Reference  END    : CR ZHHR 1035648 


            lInsertIntoTopicObj_OBJs &= "  ,[SEARCH_ENGINE_KEYWORDS])"
            lInsertIntoTopicObj_OBJs &= "   VALUES"
            lInsertIntoTopicObj_OBJs &= "('" & r("TOPIC_GROUP_CONTEXT_ID").ToString & "'"
            lInsertIntoTopicObj_OBJs &= " ,'" & r("TOPIC_GROUP_ID").ToString & "'"
            lInsertIntoTopicObj_OBJs &= " ,'" & r("TOPIC_ID").ToString & "'"
            lInsertIntoTopicObj_OBJs &= " ,'" & r("CHILD_OBJ_ID").ToString & "'"
            lInsertIntoTopicObj_OBJs &= " ,'" & lObj_ID & "'"



            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035648 - GBOX WebForms: RENAME THE COLUMN AND REMOVE ONE COLUMN FROM DB AND CODE
            ' Comment           : a) This request is for renaming column TOPIC_OBJ_OBJS_CLASSIFICATION_ID to: TREE_ICON and
            '                   : b) Also to remove [TOPIC_OBJ_OBJS_CLASSIFICATION_OBJ_ID] from table TOPIC_OBJ_OBJS and code as well. 
            '                     Both a and b changes should have no impact in any other part of the code.
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-11-24

            lInsertIntoTopicObj_OBJs &= " ,'" & r("TREE_ICON").ToString & "'"
            'lInsertIntoTopicObj_OBJs &= " ,'" & r("TOPIC_OBJ_OBJS_CLASSIFICATION_OBJ_ID").ToString & "'"

            ' Reference  END    : CR ZHHR 1035648 


            lInsertIntoTopicObj_OBJs &= " ,'" & txtSearchEngineKeyWords.Text & "')"
            lPackage.Add(lInsertIntoTopicObj_OBJs)
        End If

        'WorkFlowInformation



        lPackage.AddRange(StartWorkflow(lFields, lValues))
        'ENDWorkFlowInformation
        If mUser.Databasemanager.ExecutePackage(lPackage) Then
            lblStatus.Text = "The package is saved with GBOX_ID: " & mUser.Current_Request_ID
            'imgCancel_Click(Nothing, Nothing)
            Return True
        Else
            lblStatus.Text = mUser.Databasemanager.ErrText
            Return False
        End If

    End Function
    Private Function CopyDatasetFromTableToTableSql(ByVal lFromtable As String, ByVal lToTable As String, ByVal lWherecondition As String) As String
        Dim lDtToTable As DataTable = mUser.Databasemanager.MakeDataTable("select COLUMN_NAME from  INFORMATION_SCHEMA.COLUMNS Where Table_NAME ='" & lToTable & "'")
        Dim lFieldInfo As String = ""
        For Each lrow As DataRow In lDtToTable.Rows
            lFieldInfo &= "[" & lrow("COLUMN_NAME").ToString & "],"
        Next
        lFieldInfo = Left(lFieldInfo, lFieldInfo.Length - 1)
        Dim lSqlHist As String = "INSERT INTO " & lToTable
        lSqlHist &= "(" & lFieldInfo & ")"

        lSqlHist &= " Select " & lFieldInfo & " from " & lFromtable & lWherecondition
        Return lSqlHist
    End Function
    Sub EnableCustomizingObj(ByVal lbol As Boolean)

        txtCustomizingObjName.Enabled = False
        cboMandatoryType.Enabled = lbol
        chkALL.Enabled = lbol
        chkFi.Enabled = lbol
        chkSO.Enabled = lbol
        txtWikiUrl.Enabled = lbol
        imgSubmitCustomizingObj.Enabled = lbol
        ChangeIcons()
    End Sub

#Region "Handles CPS Controls"
    Protected Sub imgSubmitDocumentation_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgSubmitDocumentation.Click
        Dim lObjId As String = mUser.Current_OBJ.OBJ_ID

        If CopyCPSAttribDataChangesToTables(lObjId) Then
            imgEditDocumentation.Enabled = True
            imgSubmitDocumentation.Enabled = False
            EnableDocumentationFields(False)
            imgEditCustomizingObj.Enabled = True
            EnableCustomizingObj(False)
        Else
            imgSubmitDocumentation.Enabled = True
            imgEditDocumentation.Enabled = False
            EnableDocumentationFields(True)
        End If
        ChangeIcons()

    End Sub
    Protected Sub imgSubmitCustomizingObj_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgSubmitCustomizingObj.Click
        imgSubmitDocumentation.Enabled = False
        imgEditDocumentation.Enabled = True
        imgEditCustomizingObj.Enabled = True
        EnableCustomizingObj(False)
        EnableDocumentationFields(False)
        CopyCPSObjDataChangesToTables()
        lblWikiname.Visible = False
        lblWikiUrl.Visible = False
        txtWikiName.Visible = False
        txtWikiUrl.Visible = False
    End Sub
    Protected Sub imgEditCustomizingObj_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgEditCustomizingObj.Click
        Dim lLockedSql As String = "Select Locked from OBJ_CPS where [OBJ_CPS_ID]='" & txtCustomizingObjName.Text & "' AND Locked is not null"
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lLockedSql)
        If ldt.Rows.Count <> 0 Then
            lLockedSql = "Select Requester from wf_REQUEST_ACTIVE_ITEM where REQUEST_ID='" & ldt.Rows(0)("LOCKED").ToString & "'"
            Dim lReqDt As DataTable = mUser.Databasemanager.MakeDataTable(lLockedSql)
            lblStatus.Text = "The  object is locked by " & lReqDt.Rows(0)("REQUESTER").ToString & " with request id: " & ldt.Rows(0)("LOCKED").ToString
            lblStatus.ForeColor = Drawing.Color.Red
            lblStatus.Font.Bold = True
            Exit Sub
        End If

        imgCancel.Enabled = True
        imgEditCustomizingObj.Enabled = False
        imgEditDocumentation.Enabled = False
        EnableDocumentationFields(False)
        EnableCustomizingObj(True)
        ChangeIcons()
    End Sub
    Protected Sub imgNewCustomizingObj_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgNewCustomizingObj.Click
        imgEditCustomizingObj.Enabled = False
        imgEditDocumentation.Enabled = False
        imgNewDocumentationObject.Enabled = False
        imgNewCustomizingObj.Enabled = False
        EnableDocumentationFields(False)
        EnableCustomizingObj(True)
        imgSubmitCustomizingObj.CommandArgument = "INSERT"
        lblWikiname.Visible = True
        lblWikiUrl.Visible = True
        txtWikiName.Visible = True
        txtWikiUrl.Visible = True
        imgCancel.Enabled = True
        txtCustomizingObjName.Enabled = True
        lblVersionnumberCust.Text = "0"
        imgCancel.Enabled = True
        ChangeIcons()
    End Sub
    Protected Sub imgNewDocumentationObject_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgNewDocumentationObject.Click
        cboCustomizingObjName.Visible = True
        cboCustomizingObjName.Enabled = True
        txtCustomizingObjName.Visible = False
        With cboCustomizingObjName
            Dim dt As DataTable = mUser.Databasemanager.MakeDataTable("Select OBJ_CPS_ID from OBJ_CPS")
            .DataTextField = "OBJ_CPS_ID"
            .DataSource = dt
            .DataBind()
        End With
    End Sub

#End Region
#End Region
#End Region
    Sub HideAll()
        mnuDocTab.Visible = False
        imgRefresh.Visible = False
        imgDownloadToExcel.Visible = False
        imgQuery.Visible = False
        imgCancel.Visible = False
        imgAlert.Visible = False
        imgmySubscriptions.Visible = False
        imgSearchEngine.Visible = False
        imgHelp.Visible = False
        chkPaging.Visible = False
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : Added filter controls in this page every in the code, but not commented at all the places in the code to reduce the comments.
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 15-Feb-2016
        rdFilter.Visible = False
        lblFilter.Visible = False
        ' Reference End     : ZHHR 1053017

        cmbFieldFilter.Visible = False
        txtFiltertext.Visible = False
        imgAppend.Visible = False
        imgRelease.Visible = False
        '------------------------------------------------------------------------------
        'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
        'Comment    : On third mask image buttons are replaced by button controls
        'Added by   : Pratyusa Lenka (CWID : EOJCG)
        'Date       : 2016-02-16
        btnTextFill.Visible = False
        btnOverwriteEnglish.Visible = False
        ' Reference END : CR ZHHR 1053558
        '------------------------------------------------------------------------------
        cmborglevel.Visible = False
        cmborglevelvalue.Visible = False
        txtRequestComment.Visible = False
        chkSYSTEMS.Visible = False
        '---------------------------------------------------------------------
        ' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
        ' Comment   : Hide controls
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2015-12-24
        btnAddValue.Visible = False
        btnChangeValue.Visible = False
        'btnCopyValue.Visible = False
        'lblStart.Visible = False
        lblMainData.Visible = False
        lblTextTranslation.Visible = False
        lblSystemsInfo.Visible = False
        lblSubmit.Visible = False
        tblStartMenu.Visible = False
        ' Reference END : CR ZHHR 1050708
        '---------------------------------------------------------------------
    End Sub
    Sub Enable_Disable(lClickedControl As String, lCurrentObjClassificationID As String)
        'Generated Code
        '--------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
        ' Comment   : Delete COMPOSITE_S_T object type
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-01-25
        '------------------------------------------------------------------------------
        'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
        'Comment    : On third mask image buttons are replaced by button controls
        'Added by   : Pratyusa Lenka (CWID : EOJCG)
        'Date       : 2016-02-16
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : System dependent workflow
        ' Added by          : Pratyusa Lenka (CWID : EOJCG)
        ' Date              : 03-Nov-2017
        If lClickedControl = "rdFilter$0" Or lClickedControl = "rdFilter$1" Or lClickedControl = "rdFilter$2" Then
            lClickedControl = "trvOBJ"
        End If
        Select Case lClickedControl
            Case "mnuNavigate"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        chkPaging.Enabled = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        chkPaging.Enabled = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        chkPaging.Enabled = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "mnuDetailsMenu"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        chkPaging.Enabled = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "mnuDocTab"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "trvOBJ"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "btnChangeValue"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "grdDat"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "imgRefresh"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "imgDownloadToExcel"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "imgQuery"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "btnAddValue"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "btnNextEditText"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "btnNextEditSystems"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        btnCopyValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                End Select
            Case "btnSubmit"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        cmbFieldFilter.Visible = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        txtFiltertext.Visible = True
                        imgAppend.Visible = True
                        imgRelease.Visible = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        cmbFieldFilter.Visible = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        txtFiltertext.Visible = True
                        imgAppend.Visible = True
                        imgRelease.Visible = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        cmbFieldFilter.Visible = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        txtFiltertext.Visible = True
                        imgAppend.Visible = True
                        imgRelease.Visible = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "imgCancel"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "imgAlert"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "imgmySubscriptions"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "imgSearchEngine"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgHelp.Visible = False
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "imgHelp"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "chkPaging"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "cmbFieldFilter"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "imgAppend"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "imgRelease"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = True
                        imgDownloadToExcel.Enabled = True
                        imgQuery.Visible = False
                        btnAddValue.Visible = True
                        btnChangeValue.Visible = True
                        btnAddValue.Enabled = True
                        imgCancel.Visible = False
                        imgAlert.Visible = True
                        imgAlert.Enabled = True
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = True
                        chkPaging.Enabled = True
                        rdFilter.Visible = True
                        lblFilter.Visible = True
                        cmbFieldFilter.Visible = True
                        cmbFieldFilter.Enabled = True
                        txtFiltertext.Visible = True
                        txtFiltertext.Enabled = True
                        imgAppend.Visible = True
                        imgAppend.Enabled = True
                        imgRelease.Visible = True
                        imgRelease.Enabled = True
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = False
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = True
                        imgmySubscriptions.Enabled = True
                        imgSearchEngine.Visible = True
                        imgSearchEngine.Enabled = True
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "btnTextFill"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24 
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "btnOverwriteEnglish"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        cmbFieldFilter.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = False
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = True
                        btnTextFill.Enabled = True
                        btnOverwriteEnglish.Visible = True
                        btnOverwriteEnglish.Enabled = True
                        cmborglevel.Visible = False
                        cmborglevelvalue.Visible = False
                        txtRequestComment.Visible = False
                        chkSYSTEMS.Visible = False
                End Select
            Case "cmborglevel"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                End Select
            Case "cmborglevelvalue"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "NONE"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                End Select
            Case "chkSYSTEMS"
                Select Case lCurrentObjClassificationID
                    Case "COMPOSITE_S_T_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                        '---------------------------------------------------------------------------------------
                        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                        ' Comment   : New concept for multiple text functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                        ' Date      : 2015-03-24
                    Case "COMPOSITE_MULTI_TXT"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "DOCUMENTATION"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "QUERY"
                        mnuDocTab.Visible = False
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = True
                        imgQuery.Enabled = True
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                    Case "SINGLE"
                        mnuDocTab.Visible = True
                        mnuDocTab.Enabled = True
                        imgRefresh.Visible = True
                        imgRefresh.Enabled = True
                        imgDownloadToExcel.Visible = False
                        imgQuery.Visible = False
                        btnAddValue.Visible = False
                        btnChangeValue.Visible = False
                        imgCancel.Visible = True
                        imgCancel.Enabled = True
                        imgAlert.Visible = False
                        imgmySubscriptions.Visible = False
                        imgSearchEngine.Visible = False
                        imgHelp.Visible = True
                        imgHelp.Enabled = True
                        chkPaging.Visible = False
                        rdFilter.Visible = False
                        lblFilter.Visible = False
                        cmbFieldFilter.Visible = False
                        txtFiltertext.Visible = False
                        imgAppend.Visible = False
                        imgRelease.Visible = False
                        btnTextFill.Visible = False
                        btnOverwriteEnglish.Visible = False
                        cmborglevel.Visible = True
                        cmborglevel.Enabled = True
                        cmborglevelvalue.Visible = True
                        cmborglevelvalue.Enabled = True
                        txtRequestComment.Visible = True
                        txtRequestComment.Enabled = True
                        chkSYSTEMS.Visible = True
                        chkSYSTEMS.Enabled = True
                End Select
        End Select
        ' Reference END : CR ZHHR 1053558
        '------------------------------------------------------------------------------
        ' Reference END : CR ZHHR 1052471
        '--------------------------------------------------------------------------------
    End Sub

    ''' <summary>
    ''' Reference : CR ZHHR 1038880 - GBOX COC: GBOX dev - Enhance OBJ_FIELD
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>If helpurl is avialble for the obj field help icon should display</remarks>
    Protected Sub dvInfo_DataBound(ByVal sender As Object, ByVal e As EventArgs) Handles dvInfo.DataBound
        If Not dvInfo.DataSource Is Nothing Then
            ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
            ' Comment   : Default focus should be on <Next> button
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2019-09-26
            If Not dvInfo Is Nothing AndAlso dvInfo.Rows.Count > 0 Then
                btnNextEditText.Focus()
            End If
            Dim strSqlImgURL As String = ""
            Dim strSqlPostEdit As String = ""
            For i = 0 To dvInfo.Rows.Count - 1
                '---------------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1043661 - GBOX COC OTT 827: Subsequently editable key value
                ' Comment   : Get the OBJ_FIELD_ID, OBJ_FIELD_TYPE_ID, ISKEYMEMBER, POST_EDIT field value
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-06-16
                strSqlImgURL = "SELECT OBJ_FIELD_ID, OBJ_FIELD_TYPE_ID, ISKEYMEMBER, [REQUIRED], POST_EDIT, OBJ_FIELD_HELPURL FROM OBJ_FIELD WHERE OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "' AND DISPLAY_NAME = '" & dvInfo.Rows(i).Cells(0).Text & "'"
                ' Reference END : CR ZHHR 1043661
                '----------------------------------------------------------------------------------------
                Dim dtHelpUrl As DataTable = mUser.Databasemanager.MakeDataTable(strSqlImgURL)
                Dim drHelpUrl As DataRow = dtHelpUrl.Rows(0)
                If Not IsDBNull(drHelpUrl("OBJ_FIELD_HELPURL")) Then
                    Dim headerLabel As New Label
                    Dim imgBtn As New ImageButton
                    Dim blankLabel As New Label
                    headerLabel.Text = dvInfo.Rows(i).Cells(0).Text
                    blankLabel.Text = "  "
                    Dim strURL As String = drHelpUrl("OBJ_FIELD_HELPURL")
                    imgBtn.ImageUrl = "~/Images/help.png"
                    imgBtn.ToolTip = "DRS knowledge base for " & headerLabel.Text & " (in new window)"
                    imgBtn.OnClientClick = "javascript:window.open('" + strURL + "','popup_window', 'width=1080,height=600,left=100,top=100,resizable=yes');return false;"
                    dvInfo.Rows(i).Cells(0).Controls.Add(headerLabel)
                    dvInfo.Rows(i).Cells(0).Controls.Add(blankLabel)
                    dvInfo.Rows(i).Cells(0).Controls.Add(imgBtn)
                End If
                '---------------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1043661 - GBOX COC OTT 827: Subsequently editable key value
                ' Comment   : Get the highest TEMPXXXX value from customizing table and increment for POST_EDIT is true
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2015-06-16
                If dvInfo.CurrentMode = DetailsViewMode.Insert And CBool(drHelpUrl("ISKEYMEMBER").ToString) = True And drHelpUrl("OBJ_FIELD_TYPE_ID").ToString.ToUpper = "INPUT_VALIDATE_EXPRESSION" And CBool(drHelpUrl("POST_EDIT").ToString) = True Then
                    Dim txtPostEditKeyField As New TextBox
                    txtPostEditKeyField.BackColor = Drawing.Color.Transparent
                    txtPostEditKeyField.BorderStyle = BorderStyle.None
                    txtPostEditKeyField.ForeColor = Drawing.ColorTranslator.FromHtml("#333333")
                    txtPostEditKeyField.ReadOnly = True
                    txtPostEditKeyField.ID = dvInfo.Rows(i).Cells(0).Text
                    Dim strObjFieldID As String = drHelpUrl("OBJ_FIELD_ID").ToString
                    strSqlPostEdit = "SELECT " & strObjFieldID & " FROM CUSTOMIZING_" & mUser.Current_OBJ.OBJ_ID & " WHERE " & strObjFieldID & " LIKE 'TEMP[0-9][0-9][0-9][0-9]' ORDER BY " & strObjFieldID & " DESC"
                    Dim dtPostEdit As DataTable = mUser.Databasemanager.MakeDataTable(strSqlPostEdit)
                    If Not dtPostEdit Is Nothing Then
                        If dtPostEdit.Rows.Count = 0 Then
                            txtPostEditKeyField.Text = "TEMP0000"
                        Else
                            Dim drRow As DataRow = dtPostEdit.Rows(0)
                            Dim strValue As String = String.Format("{0}{1:d4}", "TEMP", CInt(drRow(strObjFieldID).ToString.Substring(drRow(strObjFieldID).ToString.Length - 4, 4)) + 1)
                            txtPostEditKeyField.Text = strValue
                        End If
                        dvInfo.Rows(i).Cells(1).Controls.Clear()
                        dvInfo.Rows(i).Cells(1).Controls.Add(txtPostEditKeyField)
                    End If
                    ' Reference END : CR ZHHR 1043661
                    '----------------------------------------------------------------------------------------
                End If
                '---------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
                ' Comment   : Create the dropdown list for cascade lookup fields
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2016-07-28
                ' Modified Date : 2016-09-19
                ' Comment       : Creating a new value lookup fields should be empty
                ' Modified Date : 2016-10-21
                ' Comment       : First item in dropdown should be "Select" for required LOOKUP fields
                If mUser.RequestType = myUser.RequestTypeOption.insert AndAlso CBool(drHelpUrl("REQUIRED").ToString) AndAlso TypeOf DirectCast(dvInfo.Rows(i).Controls(1), System.Web.UI.WebControls.DataControlFieldCell).Controls(0) Is DropDownList Then
                    DirectCast(dvInfo.Rows(i).Cells(1).Controls(0), System.Web.UI.WebControls.DropDownList).Items.Insert(0, New ListItem("--Select--", "NA"))
                ElseIf mUser.RequestType = myUser.RequestTypeOption.insert AndAlso drHelpUrl("OBJ_FIELD_TYPE_ID").ToString.ToUpper = "LOOKUP" AndAlso Not CBool(drHelpUrl("REQUIRED").ToString) AndAlso TypeOf DirectCast(dvInfo.Rows(i).Controls(1), System.Web.UI.WebControls.DataControlFieldCell).Controls(0) Is DropDownList Then
                    DirectCast(dvInfo.Rows(i).Cells(1).Controls(0), System.Web.UI.WebControls.DropDownList).Items.Insert(0, "")
                    '------------------------------------------------------------------------------------------------
                    ' Reference : ZHHR 1070280 - GBOX COC: Not required lookup field should be empty in G|Box Cockpit
                    ' Comment   : Make first item as blank in dropdown for non-required fields
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2017-04-12
                ElseIf mUser.RequestType = myUser.RequestTypeOption.update AndAlso drHelpUrl("OBJ_FIELD_TYPE_ID").ToString.ToUpper = "LOOKUP" AndAlso Not CBool(drHelpUrl("REQUIRED").ToString) AndAlso TypeOf DirectCast(dvInfo.Rows(i).Controls(1), System.Web.UI.WebControls.DataControlFieldCell).Controls(0) Is DropDownList Then
                    DirectCast(dvInfo.Rows(i).Cells(1).Controls(0), System.Web.UI.WebControls.DropDownList).Items.Insert(0, "")
                    ' Reference END : CR ZHHR 1070280
                    '------------------------------------------------------------------------------------------------
                End If
                'Create the dropdown list for cascade lookup fields
                Dim dtCascadeFields As DataTable = mUser.Databasemanager.GetLookupCascade(dvInfo.Rows(i).Cells(0).Text)
                If Not dtCascadeFields Is Nothing AndAlso dtCascadeFields.Rows.Count > 0 Then
                    ' Reference : YHHR 2041528 - GBOX COC: Cascaded functionality does not work
                    ' Comment   : Change LOOKUP_TABLE_KEY instead of OBJ_FIELD_ID to get the required data for cascaded lookup functionality
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                    ' Date      : 2019-05-10
                    If mUser.Databasemanager.IsObjFieldInLookupFilter(dtCascadeFields.Rows(0)("LOOKUP_TABLE_KEY").ToString) Then
                        CreateDropDownList(dtCascadeFields.Rows(0), i)
                    ElseIf dtCascadeFields.Rows(0)("OBJ_FIELD_ID").ToString = "SUBGROUP_ID" And mUser.RequestType = myUser.RequestTypeOption.insert And mUser.Databasemanager.HaveAtleastOneCascadeField Then
                        CreateDropDownList(dtCascadeFields.Rows(0), i)
                    End If
                End If
                'Retain the literal value after postback
                If TypeOf DirectCast(dvInfo.Rows(i).Controls(1), System.Web.UI.WebControls.DataControlFieldCell).Controls(0) Is Literal And mUser.Databasemanager.HaveAtleastOneCascadeField Then
                    If dvInfo.Rows(i).Cells(1).Text = "" Then
                        For Each objKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                            If objKey.Displayname = dvInfo.Rows(i).Cells(0).Text Then
                                dvInfo.Rows(i).Cells(1).Text = objKey.CurrentValue
                            End If
                        Next
                    End If
                End If
                ' Reference END : CR ZHHR 1059522
                '---------------------------------------------------------------------------------
            Next
        End If
    End Sub
    ''' <summary>
    ''' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2016-07-25
    ''' </summary>
    ''' <param name="drRow"></param>
    ''' <param name="iRow"></param>
    ''' <remarks>Create the dynamic dropdown for cascade fields</remarks>
    Private Sub CreateDropDownList(ByVal drRow As DataRow, Optional ByVal iRow As Integer = 0)
        Dim strFieldType As String = ""
        If Not mUser.GBOXmanager.KeyCollection Is Nothing Then
            For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                If lKey.Displayname = drRow("DISPLAY_NAME").ToString Then
                    If mUser.RequestType = myUser.RequestTypeOption.update Then
                        strFieldType = "DISPLAY"
                    End If
                End If
            Next
        End If
        If strFieldType <> "DISPLAY" Then
            Dim ddl As New DropDownList()
            ddl.ID = "ddlDynamic-" & drRow("DISPLAY_NAME").ToString
            ddl.Width = pVarConstWith + 5
            smdvInfo.RegisterAsyncPostBackControl(ddl)
            ddl.AutoPostBack = True
            ddl.EnableViewState = True
            If IsDBNull(drRow("CASCADE")) AndAlso drRow("CASCADE").ToString = "" Then
                Dim dtDdl As DataTable = mUser.Databasemanager.MakeDataTable("SELECT DISTINCT " & drRow("LOOKUP_TABLE_KEY").ToString & " FROM " & drRow("LOOKUP_TABLE_NAME").ToString)
                If Not dtDdl Is Nothing AndAlso dtDdl.Rows.Count > 0 Then
                    ddl.DataSource = dtDdl
                    ddl.DataTextField = drRow("LOOKUP_TABLE_KEY").ToString
                    ddl.DataValueField = drRow("LOOKUP_TABLE_KEY").ToString
                    ddl.DataBind()
                    ddl.Items.Insert(0, New ListItem("--Select--", "NA"))
                    '--------------------------------------------------------------------------------------------
                    ' Reference : ZHHR 1062728 - GBOX COC: OTT 3514 - Enhance LOOKUP functionality
                    ' Comment   : If a field was marked as required in OBJ_FIELD, it should be marked as yellow
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2016-10-19
                    If CBool(drRow("REQUIRED").ToString) Then ddl.BackColor = Drawing.Color.Gold
                    ' Reference END : CR ZHHR 1062728
                    '--------------------------------------------------------------------------------------------
                    If mUser.RequestType = myUser.RequestTypeOption.update Then
                        ddl.SelectedValue = DirectCast(dvInfo.Rows(iRow).Cells(1).Controls(0), System.Web.UI.WebControls.DropDownList).SelectedValue
                        'ddl.BackColor = Drawing.Color.Gold
                    End If
                    If Not dvInfo.DataSource Is Nothing AndAlso dvInfo.Rows.Count > 0 Then
                        dvInfo.Rows(iRow).Cells(1).Controls.Clear()
                        dvInfo.Rows(iRow).Cells(1).Controls.Add(ddl)
                        If CBool(drRow("REQUIRED").ToString) Then
                            Dim rfv As New RequiredFieldValidator
                            With rfv
                                .Text = drRow("REQUIREDTEXT").ToString
                                .ControlToValidate = ddl.ID
                                .Display = ValidatorDisplay.Dynamic
                                .ID = "validate" & ddl.ID
                                .InitialValue = "NA"
                            End With
                            dvInfo.Rows(iRow).Cells(1).Controls.Add(rfv)
                        End If
                    End If
                End If
            Else
                If mUser.RequestType = myUser.RequestTypeOption.update Then
                    Dim dtDdl As DataTable = mUser.Databasemanager.MakeDataTable("SELECT DISTINCT " & drRow("LOOKUP_TABLE_KEY").ToString & " FROM " & drRow("LOOKUP_TABLE_NAME").ToString)
                    If Not dtDdl Is Nothing AndAlso dtDdl.Rows.Count > 0 Then
                        ddl.DataSource = dtDdl
                    End If
                End If
                ddl.DataTextField = drRow("LOOKUP_TABLE_KEY").ToString
                ddl.DataValueField = drRow("LOOKUP_TABLE_KEY").ToString
                ddl.Items.Insert(0, New ListItem("--Select--", "NA"))
                ddl.DataBind()
                '--------------------------------------------------------------------------------------------
                ' Reference : ZHHR 1062728 - GBOX COC: OTT 3514 - Enhance LOOKUP functionality
                ' Comment   : If a field was marked as required in OBJ_FIELD, it should be marked as yellow
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2016-10-19
                If CBool(drRow("REQUIRED").ToString) Then ddl.BackColor = Drawing.Color.Gold
                ' Reference END : CR ZHHR 1062728
                '--------------------------------------------------------------------------------------------
                If mUser.RequestType = myUser.RequestTypeOption.update Then
                    ddl.SelectedValue = DirectCast(dvInfo.Rows(iRow).Cells(1).Controls(0), System.Web.UI.WebControls.DropDownList).SelectedValue
                    'ddl.BackColor = Drawing.Color.Gold
                Else
                    ddl.Enabled = False
                End If
                If Not dvInfo.DataSource Is Nothing AndAlso dvInfo.Rows.Count > 0 Then
                    dvInfo.Rows(iRow).Cells(1).Controls.Clear()
                    dvInfo.Rows(iRow).Cells(1).Controls.Add(ddl)
                    If CBool(drRow("REQUIRED").ToString) Then
                        Dim rfv As New RequiredFieldValidator
                        With rfv
                            .Text = drRow("REQUIREDTEXT").ToString
                            .ControlToValidate = ddl.ID
                            .Display = ValidatorDisplay.Dynamic
                            .ID = "validate" & ddl.ID
                            .InitialValue = "NA"
                        End With
                        dvInfo.Rows(iRow).Cells(1).Controls.Add(rfv)
                    End If
                End If
            End If
            AddHandler ddl.SelectedIndexChanged, AddressOf OnSelectedIndexChanged
        End If
    End Sub
    ''' <summary>
    ''' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2016-07-25
    ''' Reference : YHHR 2041528 - GBOX COC: Cascaded functionality does not work
    ''' Comment   : Change LOOKUP_TABLE_KEY instead of OBJ_FIELD_ID to get the required data for cascaded lookup functionality
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2019-04-25
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Handle the selectedindex change of dynamic dropdown</remarks>
    Protected Sub OnSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddl As DropDownList = DirectCast(sender, DropDownList)
        Dim lstLookupCols As New List(Of KeyValuePair(Of String, String))
        Dim lstLookupFilterCols As New List(Of KeyValuePair(Of String, String))
        Dim strColName As String = ""
        Dim bIsProcessed As Boolean = False
        Dim strSqlCascade As String = ""
        Dim dtCascade As DataTable = Nothing
        Dim strRequestField As String = ""
        Dim iOrdinalPosition As Integer = 0
        Dim drRowObjField As DataRow = Nothing
        Try
            strSqlCascade = "SELECT OFD.OBJ_FIELD_ID, OFD.DISPLAY_NAME, OFD.ORDINAL_POSITION, OFL.LOOKUP_TABLE_NAME, OFL.LOOKUP_TABLE_KEY, OFL.LOOKUP_TABLE_FILTER, OFL.[CASCADE] FROM OBJ_FIELD OFD"
            strSqlCascade &= " INNER JOIN OBJ_FIELD_LOOKUP_VALIDATON OFL ON OFD.OBJ_ID = OFL.OBJ_ID AND OFD.OBJ_FIELD_ID = OFL.OBJ_FIELD_ID"
            strSqlCascade &= " WHERE OFD.OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "' AND OFD.OBJ_FIELD_TYPE_ID = 'LOOKUP' AND [CASCADE] = 1"
            dtCascade = mUser.Databasemanager.MakeDataTable(strSqlCascade)
            If Not dtCascade Is Nothing AndAlso dtCascade.Rows.Count > 0 Then
                For Each drRow As DataRow In dtCascade.Rows
                    If ddl.SelectedIndex <> 0 And Not bIsProcessed Then
                        If ddl.SelectedIndex <> 0 Then
                            lstDDL.Add(New KeyValuePair(Of String, String)(ddl.DataTextField, ddl.SelectedValue))
                        ElseIf Request.Form("dvInfo$ddlDynamic-" & drRow("DISPLAY_NAME")).ToString <> "NA" Then
                            lstDDL.Add(New KeyValuePair(Of String, String)(ddl.DataTextField, LTrimRTrim(Request.Form("dvInfo$ddlDynamic-" & drRow("DISPLAY_NAME")).ToString)))
                        End If
                        If drRow("LOOKUP_TABLE_FILTER").ToString <> "" And drRow("LOOKUP_TABLE_FILTER").ToString.Contains(ddl.DataTextField) Then
                            Dim dtCustomizing As DataTable = mUser.Databasemanager.MakeDataTable("SELECT TOP 1 * FROM " & drRow("LOOKUP_TABLE_NAME").ToString)
                            If Not dtCustomizing Is Nothing AndAlso dtCustomizing.Rows.Count > 0 Then
                                For Each dcColumn As DataColumn In dtCustomizing.Columns
                                    strColName = dcColumn.ColumnName
                                    lstLookupFilterCols = lstDDL.FindAll(Function(o) o.Key = strColName)
                                    For Each pair As KeyValuePair(Of String, String) In lstLookupFilterCols
                                        Dim dtFilterObjField As DataTable = mUser.Databasemanager.MakeDataTable("SELECT OBJ_FIELD_ID FROM OBJ_FIELD_LOOKUP_VALIDATON WHERE OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "' AND LOOKUP_TABLE_KEY = '" & pair.Key & "'")
                                        If Not dtFilterObjField Is Nothing And dtFilterObjField.Rows.Count > 0 Then
                                            drRowObjField = dtFilterObjField.Rows(0)
                                            lstLookupCols.Add(New KeyValuePair(Of String, String)(drRowObjField("OBJ_FIELD_ID").ToString(), pair.Value))
                                        End If
                                        'lstLookupCols.Add(New KeyValuePair(Of String, String)(pair.Key, pair.Value))
                                    Next
                                Next
                            End If
                            Dim strWhereString As String = GetLookupFilter(lstLookupCols, drRow("LOOKUP_TABLE_FILTER").ToString)
                            Dim dtDDL As DataTable = mUser.Databasemanager.MakeDataTable("SELECT DISTINCT " & drRow("LOOKUP_TABLE_KEY").ToString & " FROM " & drRow("LOOKUP_TABLE_NAME").ToString & " " & strWhereString)
                            If Not dtDDL Is Nothing AndAlso dtDDL.Rows.Count > 0 Then
                                bIsProcessed = True
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).Enabled = True
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).Items.Clear()
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).DataSource = dtDDL
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).DataTextField = drRow("LOOKUP_TABLE_KEY").ToString
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).DataValueField = drRow("LOOKUP_TABLE_KEY").ToString
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).SelectedValue = Nothing
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).DataBind()
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).Items.Insert(0, New ListItem("--Select--", "NA"))
                            End If
                        End If
                    ElseIf Not bIsProcessed Then
                        If Request.Form("dvInfo$ddlDynamic-" & drRow("DISPLAY_NAME")).ToString <> "NA" And ddl.DataTextField = drRow("LOOKUP_TABLE_KEY").ToString Then
                            lstDDL.Add(New KeyValuePair(Of String, String)(ddl.DataTextField, LTrimRTrim(Request.Form("dvInfo$ddlDynamic-" & drRow("DISPLAY_NAME")).ToString)))
                            strRequestField = LTrimRTrim(drRow("DISPLAY_NAME").ToString)
                            iOrdinalPosition = Convert.ToInt32(drRow("ORDINAL_POSITION"))
                        End If
                        If drRow("LOOKUP_TABLE_FILTER").ToString <> "" And drRow("LOOKUP_TABLE_FILTER").ToString.Contains(ddl.DataTextField) Then
                            Dim dtCustomizing As DataTable = mUser.Databasemanager.MakeDataTable("SELECT TOP 1 * FROM " & drRow("LOOKUP_TABLE_NAME").ToString)
                            If Not dtCustomizing Is Nothing AndAlso dtCustomizing.Rows.Count > 0 Then
                                For Each dcColumn As DataColumn In dtCustomizing.Columns
                                    strColName = dcColumn.ColumnName
                                    lstLookupFilterCols = lstDDL.FindAll(Function(o) o.Key = strColName)
                                    For Each pair As KeyValuePair(Of String, String) In lstLookupFilterCols
                                        Dim dtFilterObjField As DataTable = mUser.Databasemanager.MakeDataTable("SELECT OBJ_FIELD_ID FROM OBJ_FIELD_LOOKUP_VALIDATON WHERE OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "' AND LOOKUP_TABLE_KEY = '" & pair.Key & "'")
                                        If Not dtFilterObjField Is Nothing And dtFilterObjField.Rows.Count > 0 Then
                                            drRowObjField = dtFilterObjField.Rows(0)
                                            lstLookupCols.Add(New KeyValuePair(Of String, String)(drRowObjField("OBJ_FIELD_ID").ToString(), pair.Value))
                                        End If
                                        'lstLookupCols.Add(New KeyValuePair(Of String, String)(pair.Key, pair.Value))
                                    Next
                                Next
                            End If
                            Dim strWhereString As String = GetLookupFilter(lstLookupCols, drRow("LOOKUP_TABLE_FILTER").ToString)
                            Dim dtDDL As DataTable = mUser.Databasemanager.MakeDataTable("SELECT DISTINCT " & drRow("LOOKUP_TABLE_KEY").ToString & " FROM " & drRow("LOOKUP_TABLE_NAME").ToString & " " & strWhereString)
                            If Not dtDDL Is Nothing Then
                                bIsProcessed = True
                                If drRow("ORDINAL_POSITION") = iOrdinalPosition + 1 Then
                                    DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).Enabled = True
                                End If
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).Items.Clear()
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).DataSource = dtDDL
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).DataTextField = drRow("LOOKUP_TABLE_KEY").ToString
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).DataValueField = drRow("LOOKUP_TABLE_KEY").ToString
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).DataBind()
                                DirectCast(dvInfo.FindControl("ddlDynamic-" & drRow("DISPLAY_NAME").ToString), DropDownList).Items.Insert(0, New ListItem("--Select--", "NA"))
                                ddl.SelectedValue = Request.Params("dvInfo$ddlDynamic-" & strRequestField)
                            End If
                        ElseIf lstDDL.Count > dtCascade.Rows.Count Then
                            ddl.SelectedValue = Request.Params("dvInfo$ddlDynamic-" & strRequestField)
                        End If
                    End If
                Next
            End If
            updvInfo.Update()
        Catch ex As Exception
            mErrText &= "ddlDynamic-OnSelectedIndexChanged" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mErrText &= ""
        Finally
            lstLookupCols = Nothing
            lstLookupFilterCols = Nothing
            dtCascade = Nothing
        End Try
    End Sub
    Private Function GetLookupFilter(ByVal lstLookupCols As List(Of KeyValuePair(Of String, String)), ByVal strLookupFilter As String) As String
        Dim strLookupTableFilter As String = ""
        For Each pair As KeyValuePair(Of String, String) In lstLookupCols
            strLookupFilter = strLookupFilter.Replace("|" & pair.Key & "|", "'" & pair.Value & "'")
        Next
        strLookupTableFilter = strLookupFilter
        Return strLookupTableFilter
    End Function
    '---------------------------------------------------------------------------------------------------
    ' Reference         : ZHHR 1044222 - GBOX MGR OTT 1047: Send Email from Workflow
    ' Comment           : For each workflow step, an email is sent to all relevant user, e.g. approver in an approver group.
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 26-Jun-2015
    Private Function SendEmailToApproverGroup(ByVal lOrg_Level_Value As String) As List(Of String)
        Dim lSql As String = ""
        Dim lRequest_text As String
        Dim lPack As New List(Of String)

        lSql = " SELECT * FROM wf_DEFINE_WORKFLOW_DETAILS WHERE OBJ_ID ='" & mUser.Current_OBJ.OBJ_ID & "' AND ORG_LEVEL_VALUE = '" & lOrg_Level_Value & "' AND EMAIL = 1 "
        Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
        lSql = ""
        If Not dt Is Nothing Then
            If dt.Rows.Count > 0 Then
                lSql = "SELECT USER_SOURCE FROM dp_Additional_Information WHERE TITLE = 'Approver teams'"
                Dim dtAuth As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
                If Not dtAuth Is Nothing Then
                    If dtAuth.Rows.Count > 0 Then
                        lSql = dtAuth.Rows.Item(0).Item("USER_SOURCE").ToString
                        lSql = lSql.Replace("|ORG_LEVEL_VALUE|", lOrg_Level_Value)
                        Dim dtResult As DataTable = mUser.Databasemanager.MakeDataTable(lSql)

                        If Not dtResult Is Nothing Then
                            If dtResult.Rows.Count > 0 Then
                                Dim lUserEmail As String = ""
                                lRequest_text = "Dear Sir/Madam, " & vbCrLf & vbCrLf & "A workflow item in GBOX requires your attention. " & vbCrLf
                                lRequest_text += "Please start GBOX Manager and evaluate the item(s) assigned to you." & vbCrLf
                                lRequest_text += "Information/help regarding GBOX Manager can be found here: http://by-gbox.bayer-ag.com/GBOX-Manager" & vbCrLf & vbCrLf & vbCrLf
                                lRequest_text += "Regards," & vbCrLf
                                lRequest_text += "DRS Administration Team"

                                For Each row In dtResult.Rows
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
                                            lSql &= "   ('" & mUser.Current_Request_ID & "',"
                                            lSql &= "   '" & lUserEmail & "',"
                                            lSql &= "   'New workflow item in GBOX DRS: " & mUser.Current_Request_ID & "',"
                                            lSql &= "   N'" & lRequest_text & "',"
                                            lSql &= "   '" & mUser.CW_ID & "')"
                                            lPack.Add(lSql)
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If
        End If
        Return lPack

    End Function
    ' Reference  End     : ZHHR 1044222
    ''' <summary>
    ''' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date      : 2015-12-24
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Edit/Change the existing request</remarks>
    Protected Sub btnChangeValue_Click(sender As Object, e As EventArgs) Handles btnChangeValue.Click
        ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
        ' Comment   : Make the label request type to empty other than copy value
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2018-04-12
        lblRequestType.Text = ""
        '------------------------------------------------------------------------------
        'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
        'Comment    : When starting a DRS request, a new information pop-up is shown
        'Added by   : Pratyusa Lenka (CWID : EOJCG)
        'Date       : 2016-02-15
        Dim dtRequestInfo As DataTable = Nothing
        Dim objMDGRequest As New MDG_Request
        dtRequestInfo = mUser.Databasemanager.MakeDataTable("Select MDRS_INFO_VALUE from MDRS_INFO where MDRS_INFO_ID = 'GBOX_REQUEST_INFO'")
        If Not dtRequestInfo Is Nothing And dtRequestInfo.Rows(0)("MDRS_INFO_VALUE").ToString = "1" Then
            dtRequestInfo = New DataTable
            dtRequestInfo = mUser.Databasemanager.MakeDataTable("Select MDRS_INFO_VALUE from MDRS_INFO where MDRS_INFO_ID = 'GBOX_REQUEST_INFO_TEXT'")
            If Not dtRequestInfo Is Nothing And dtRequestInfo.Rows.Count > 0 And dtRequestInfo.Rows(0)("MDRS_INFO_VALUE").ToString.Trim <> "" Then
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "GBOX Cockpit", objMDGRequest.javaMsg(dtRequestInfo.Rows(0)("MDRS_INFO_VALUE").ToString), True)
            End If
        End If
        ' Reference END : CR ZHHR 1053558
        '------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        'Reference  : ZHHR 1055644 - GBOX COC: OTT 2525 - object-specific info pop-up
        'Comment    : When starting a DRS request, a new object specific information pop-up is shown
        'Added by   : Pratyusa Lenka (CWID : EOJCG)
        'Date       : 2016-04-13
        Dim dtObjSpecificDefineWorkflow As DataTable = mUser.Databasemanager.MakeDataTable("SELECT DISTINCT STATION_ACTION_COMMENT FROM wf_DEFINE_WORKFLOW_DETAILS WHERE OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "' AND RANK = 0 AND ISNULL(STATION_ACTION_COMMENT,'') <> ''")
        If Not dtObjSpecificDefineWorkflow Is Nothing And dtObjSpecificDefineWorkflow.Rows.Count > 0 Then
            If Not IsDBNull(dtObjSpecificDefineWorkflow.Rows(0)("STATION_ACTION_COMMENT")) And dtObjSpecificDefineWorkflow.Rows(0)("STATION_ACTION_COMMENT").ToString.Trim <> "" Then
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "GBOX Cockpit", objMDGRequest.javaMsg(dtObjSpecificDefineWorkflow.Rows(0)("STATION_ACTION_COMMENT").ToString), True)
            End If
        End If
        ' Reference END : CR ZHHR 1055644
        '-------------------------------------------------------------------------------------------
        Dim iRowIndex As Integer = 0
        LoadFactoryAndControllerAndBind(mWithPaging)
        If InStr(Me.Request.Form("__EVENTARGUMENT").ToString, "Page") <> 0 Then
            grdDat.PageIndex = Me.Request.Form("__EVENTARGUMENT").ToString.Split("$")(1) - 1
            Exit Sub
        End If

        grdDat.SelectedIndex = hfRowIndex.Value
        grdDat_SelectedIndexChanged(sender, e)
        dvInfo.ToolTip = ""
        If Not BindDetails(mWithPaging, False) Then
            If lblStatus.Text.Trim <> "" Then
                EnableDisableRibbonControls("btnChangeValue", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                ' Comment   : In case of informative message, display the buttons
                ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                ' Date      : 2019-04-01
                btnAddValue.Visible = True
                btnChangeValue.Visible = True
                If (Not mUser.Current_OBJ Is Nothing) Then
                    If (mUser.GBOXmanager.GetSysDependentInfo(mUser.Current_OBJ.OBJ_ID)) Then
                        btnCopyValue.Visible = True
                    End If
                End If
            End If
            Exit Sub
        End If
        mvContents.ActiveViewIndex = GetIndexByViewName("vwDetails")
        cmborglevel.Items.Clear()
        cmborglevelvalue.Items.Clear()
        Dim ret As Integer = FillCombo(cmborglevel, "ORG_LEVEL_ID", "")
        If ret = 1 Then
            FillCombo(cmborglevelvalue, "ORG_LEVEL_VALUE", " AND ORG_LEVEL_ID = '" & cmborglevel.SelectedValue & "'")
        End If
        EnableDisableRibbonControls("btnChangeValue", "")

    End Sub
    ''' <summary>
    ''' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date      : 2015-12-24
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Add value for new DRS request</remarks>
    Protected Sub btnAddValue_Click(sender As Object, e As EventArgs) Handles btnAddValue.Click
        Try
            ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
            ' Comment   : Make the label request type to empty other than copy value
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2018-04-12
            lblRequestType.Text = ""
            '------------------------------------------------------------------------------
            'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
            'Comment    : When starting a DRS request, a new information pop-up is shown
            'Added by   : Pratyusa Lenka (CWID : EOJCG)
            'Date       : 2016-02-15
            Dim dtRequestInfo As DataTable = Nothing
            Dim objMDGRequest As New MDG_Request
            dtRequestInfo = mUser.Databasemanager.MakeDataTable("Select MDRS_INFO_VALUE from MDRS_INFO where MDRS_INFO_ID = 'GBOX_REQUEST_INFO'")
            If Not dtRequestInfo Is Nothing And dtRequestInfo.Rows(0)("MDRS_INFO_VALUE").ToString = "1" Then
                dtRequestInfo = New DataTable
                dtRequestInfo = mUser.Databasemanager.MakeDataTable("Select MDRS_INFO_VALUE from MDRS_INFO where MDRS_INFO_ID = 'GBOX_REQUEST_INFO_TEXT'")
                If Not dtRequestInfo Is Nothing And dtRequestInfo.Rows.Count > 0 And dtRequestInfo.Rows(0)("MDRS_INFO_VALUE").ToString.Trim <> "" Then
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "GBOX Cockpit", objMDGRequest.javaMsg(dtRequestInfo.Rows(0)("MDRS_INFO_VALUE").ToString), True)
                End If
            End If
            ' Reference END : CR ZHHR 1053558
            '------------------------------------------------------------------------------
            '-------------------------------------------------------------------------------------------
            'Reference  : ZHHR 1055644 - GBOX COC: OTT 2525 - object-specific info pop-up
            'Comment    : When starting a DRS request, a new object specific information pop-up is shown
            'Added by   : Pratyusa Lenka (CWID : EOJCG)
            'Date       : 2016-04-13
            Dim dtObjSpecificDefineWorkflow As DataTable = mUser.Databasemanager.MakeDataTable("SELECT DISTINCT STATION_ACTION_COMMENT FROM wf_DEFINE_WORKFLOW_DETAILS WHERE OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID & "' AND RANK = 0 AND ISNULL(STATION_ACTION_COMMENT,'') <> ''")
            If Not dtObjSpecificDefineWorkflow Is Nothing And dtObjSpecificDefineWorkflow.Rows.Count > 0 Then
                If Not IsDBNull(dtObjSpecificDefineWorkflow.Rows(0)("STATION_ACTION_COMMENT")) And dtObjSpecificDefineWorkflow.Rows(0)("STATION_ACTION_COMMENT").ToString.Trim <> "" Then
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "GBOX Cockpit", objMDGRequest.javaMsg(dtObjSpecificDefineWorkflow.Rows(0)("STATION_ACTION_COMMENT").ToString), True)
                End If
            End If
            ' Reference END : CR ZHHR 1055644
            '-------------------------------------------------------------------------------------------
            lblStatus.Text = ""
            mUser.Databasemanager.Request = Me.Request
            mUser.RequestType = myUser.RequestTypeOption.insert
            If Not mUser.GBOXmanager.AuthenticateRequest(trvOBJ.SelectedNode.Value, mUser.CW_ID) Then
                lblStatus.Text = "You have no permission to request values for " & trvOBJ.SelectedNode.Value & "."
                imgCancel_Click(sender, Nothing)
                Enable_Disable("imgCancel", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                EnableDisableRibbonControls("imgCancel", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                Exit Sub
            End If
            If mUser.GBOXmanager.CHECK_MDRS_DATABASE_MAINTENANCE = True Then
                lblStatus.Text = mUser.GBOXmanager.Get_MDRS_DATABASE_MAINTENANCE_STRING
                imgCancel_Click(sender, Nothing)
                Exit Sub
            End If
            mUser.RequestType = myUser.RequestTypeOption.insert
            btnSubmit.CommandArgument = "New"
            dvInfo.ToolTip = ""
            btnSubmit.CommandArgument = "New"
            mUser.EditMode = True
            mUser.RequestType = myUser.RequestTypeOption.insert
            LoadFactoryAndControllerAndBind(True)
            opennew()
            cmborglevel.Items.Clear()
            cmborglevelvalue.Items.Clear()
            Dim ret As Integer = FillCombo(cmborglevel, "ORG_LEVEL_ID", "")
            If ret = 1 Then
                FillCombo(cmborglevelvalue, "ORG_LEVEL_VALUE", " AND ORG_LEVEL_ID = '" & cmborglevel.SelectedValue & "'")
            End If
            EnableDisableRibbonControls("btnAddValue", "")
        Catch ex As Exception
            mErrText &= ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mvContents.ActiveViewIndex = GetIndexByViewName("vwDetails")
            mErrText &= ""
        End Try
    End Sub
    ''' <summary>
    ''' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date      : 2015-12-24
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Next button to handle the text translation</remarks>
    Protected Sub btnNextEditText_Click(sender As Object, e As EventArgs) Handles btnNextEditText.Click
        '--------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
        ' Comment   : Delete COMPOSITE_S_T object type
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-01-25
        If Not mUser.GBOXmanager.AuthenticateRequest(trvOBJ.SelectedNode.Value, mUser.CW_ID) Then
            lblStatus.Text = "You have no permission to request values for " & trvOBJ.SelectedNode.Value & "."
            imgCancel_Click(sender, Nothing)
            Enable_Disable("imgCancel", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
            EnableDisableRibbonControls("imgCancel", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
            Exit Sub
        End If
        If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
            Try
                'If Not mUser.GBOXmanager.AuthenticateRequest(trvOBJ.SelectedNode.Value, mUser.CW_ID) Then
                '    lblStatus.Text = "You have no permission to request values for " & trvOBJ.SelectedNode.Value & "."
                '    imgCancel_Click(sender, Nothing)
                '    Exit Sub
                'End If
                Me.Validate()
                If Me.Page.IsValid Then
                    EnableDisableRibbonControls("btnNextEditText", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                    SaveMaster()
                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
                    ' Comment           : Added below code
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 17-Feb-2016
                    Dim lSysStatus As Boolean = mUser.GBOXmanager.GetSysDependentInfo(mUser.Current_OBJ.OBJ_ID.ToString)
                    ' Reference         : YHHR 2025550 - Duplicate Key Entry
                    ' Comment           : For non system dependent customizing objects, display the duplicate key message
                    ' Added by          : Pratyusa Lenka (CWID : EOJCG)
                    ' Date              : 23-Mar-2018
                    If (lSysStatus = False) Then
                        If lblVersionnumber.Text <> "1" And mUser.RequestType = myUser.RequestTypeOption.insert Then
                            lblStatus.Text = "Key Value already exists. To change it select the value in the table and cancel here."
                            lstFields.Items.Clear()
                            lstValues.Items.Clear()
                            lstDisplay.Items.Clear()
                            lstSQL.Items.Clear()
                            ChangeIcons()
                            Exit Sub
                        End If
                    End If
                    Dim lcurrentkeyvalue As String = ""
                    For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                        If InStr(lKey.Displayname.ToUpper, "VERSION") = 0 Then
                            lcurrentkeyvalue = lKey.CurrentValue
                        End If
                    Next

                    If lcurrentkeyvalue = "" Then
                        lcurrentkeyvalue = lblObj_VALUE.Text
                    End If
                    If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Then
                        BuildTextgrid_TXT()
                    ElseIf mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                        BuildTextgrid_TXT()
                    Else
                        'BuildTextgrid(lcurrentkeyvalue)
                    End If
                    mvContents.ActiveViewIndex = GetIndexByViewName("vwEditTexts")
                Else
                    EnableDisableRibbonControls("btnAddValue", "")
                End If


                btnAddValue.Enabled = False
                ChangeIcons()
            Catch ex As Exception
                lblStatus.Text = "btnNextEditText_Click:" & ex.ToString
            End Try
        Else
            btnNextEditSystems_Click(sender, e)
        End If
        ' Reference END : CR ZHHR 1052471
        '--------------------------------------------------------------------------------
    End Sub
    ''' <summary>
    ''' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date      : 2015-12-24
    ''' </summary>
    ''' <param name="lCriteriaTarget"></param>
    ''' <returns>lBuildtexts = True then display text grid</returns>
    ''' <remarks>Next step for corresponding target control</remarks>
    Private Function NextStepAction(lCriteriaTarget As String) As Boolean
        Dim lBuildtexts As Boolean
        Select Case lCriteriaTarget
            Case "btnNextEditText"
                lBuildtexts = False
                LoadFactoryAndControllerAndBind(mWithPaging)
                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                ' Comment   : New request as false to bind the grid in case of copy functionality
                ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                ' Date      : 2019-04-01
                If btnSubmit.CommandArgument = "New" AndAlso lblRequestType.Text <> "Copy" Then
                    BindDetails(mWithPaging, True)
                Else
                    BindDetails(mWithPaging, False)
                End If
                'Macht die Validatoren an
                mDynamicFormController.BindView(vwDetails, mWithPaging)
                'Macht die Validatoren an
            Case "btnNextEditSystems"
                lBuildtexts = False
                LoadFactoryAndControllerAndBind(mWithPaging)
                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                ' Comment   : New request as false to bind the grid in case of copy functionality
                ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                ' Date      : 2019-04-01
                If btnSubmit.CommandArgument = "New" AndAlso lblRequestType.Text <> "Copy" Then
                    BindDetails(mWithPaging, True)
                Else
                    BindDetails(mWithPaging, False)
                End If
                mDynamicFormController.BindView(vwDetails, mWithPaging)
                Dim lcurrentkeyvalue As String = ""
                For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                    If InStr(lKey.Displayname.ToUpper, "VERSION") = 0 Then
                        lcurrentkeyvalue = lKey.CurrentValue
                    End If
                Next
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID <> "SINGLE" Then
                    lcurrentkeyvalue = lblObj_VALUE.Text
                    lBuildtexts = True
                End If
                ChangeIcons()
            Case "btnSubmit"
                ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
                ' Comment   : Default focus should be on <Submit> button
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2019-09-26
                btnSubmit.Focus()
                If InStr(cmborglevel.Text.ToUpper, "Please".ToUpper) <> 0 Then
                    lblStatus.Text = "Choose Orglevel for the request (Dropdown Boxes)"
                    Exit Function
                End If
                If InStr(cmborglevelvalue.Text.ToUpper, "Please".ToUpper) <> 0 Then
                    lblStatus.Text = "Choose OrglevelValue for the request (Dropdown Boxes)"
                    Exit Function
                End If
                lBuildtexts = True
        End Select
        Return lBuildtexts
    End Function
    ''' <summary>
    ''' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date      : 2015-12-24
    ''' </summary>
    ''' <param name="strControlName"></param>
    ''' <param name="objClassificationID"></param>
    ''' <returns></returns>
    ''' <remarks>Enable/Disable of the ribbon controls</remarks>
    Private Function EnableDisableRibbonControls(strControlName As String, objClassificationID As String) As Boolean
        '----------------------------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1053164 - GBOX-COC: display error in Cockpit
        ' Comment   : New request ribbon should only be displayed when handling a DRS request in GBOX Cockpit
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-02-02
        If objClassificationID.ToUpper = "DOCUMENTATION" Or objClassificationID.ToUpper = "NONE" Or objClassificationID.ToUpper = "QUERY" Or objClassificationID.ToUpper = "ADMINISTRATION" Or objClassificationID.ToUpper = "CPS_NODE" Or objClassificationID.ToUpper = "CPS_OBJ" Or objClassificationID.ToUpper = "CPS_OBJ_ATTR" Or objClassificationID.ToUpper = "CPS_OBJ_ATTR_OLD" Then Exit Function
        ' Reference END : CR ZHHR 1053164
        '----------------------------------------------------------------------------------------------------
        'lblStart.Visible = True
        lblMainData.Visible = True
        lblTextTranslation.Visible = True
        lblSystemsInfo.Visible = True
        lblSubmit.Visible = True
        '--------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
        ' Comment   : Delete COMPOSITE_S_T object type
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-01-25
        Select Case strControlName
            'Case "trvOBJ", "btnSubmit", "imgCancel"
            '    Select Case objClassificationID
            '        Case "COMPOSITE_S_T_TXT", "COMPOSITE_MULTI_TXT", "SINGLE"
            '            'lblStart.Font.Bold = True
            '            'lblStart.ForeColor = Drawing.Color.DarkOrange
            '            lblMainData.Font.Bold = False
            '            lblTextTranslation.Font.Bold = False
            '            lblSystemsInfo.Font.Bold = False
            '            lblSubmit.Font.Bold = False
            '            lblMainData.ForeColor = Drawing.Color.Gray
            '            lblTextTranslation.ForeColor = Drawing.Color.Gray
            '            lblSystemsInfo.ForeColor = Drawing.Color.Gray
            '            lblSubmit.ForeColor = Drawing.Color.Gray
            '    End Select
            Case "btnAddValue", "btnChangeValue"
                'lblStart.Font.Bold = False
                If InStr(lblStatus.Text, "This value is locked by USER:") = 0 Then
                    tblStartMenu.Visible = True
                Else
                    tblStartMenu.Visible = False
                End If
                lblMainData.Font.Bold = True
                lblMainData.ForeColor = Drawing.Color.DarkOrange
                lblTextTranslation.Font.Bold = False
                lblSystemsInfo.Font.Bold = False
                lblSubmit.Font.Bold = False
                'lblStart.ForeColor = Drawing.Color.Orange
                lblTextTranslation.ForeColor = Drawing.Color.Gray
                lblSystemsInfo.ForeColor = Drawing.Color.Gray
                lblSubmit.ForeColor = Drawing.Color.Gray
                '------------------------------------------------------------------------------
                'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
                'Comment    : On third mask image buttons are replaced by button controls
                'Added by   : Pratyusa Lenka (CWID : EOJCG)
                'Date       : 2016-02-16
            Case "btnNextEditText", "btnTextFill", "btnOverwriteEnglish"
                'lblStart.Font.Bold = False
                tblStartMenu.Visible = True
                lblMainData.Font.Bold = False
                lblTextTranslation.Font.Bold = True
                lblTextTranslation.ForeColor = Drawing.Color.DarkOrange
                lblSystemsInfo.Font.Bold = False
                lblSubmit.Font.Bold = False
                'lblStart.ForeColor = Drawing.Color.Orange
                lblMainData.ForeColor = Drawing.Color.Orange
                lblSystemsInfo.ForeColor = Drawing.Color.Gray
                lblSubmit.ForeColor = Drawing.Color.Gray
                ' Reference END : CR ZHHR 1053558
                '------------------------------------------------------------------------------
            Case "btnNextEditSystems", "cmborglevel"
                'lblStart.Font.Bold = False
                tblStartMenu.Visible = True
                lblMainData.Font.Bold = False
                lblTextTranslation.Font.Bold = False
                lblSystemsInfo.Font.Bold = True
                lblSystemsInfo.ForeColor = Drawing.Color.DarkOrange
                lblSubmit.Font.Bold = False
                'lblStart.ForeColor = Drawing.Color.Orange
                lblMainData.ForeColor = Drawing.Color.Orange
                ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
                ' Comment   : Default focus should be on <Submit> button
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2019-09-26
                btnSubmit.Focus()
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                    lblTextTranslation.ForeColor = Drawing.Color.Orange
                Else
                    lblTextTranslation.ForeColor = Drawing.Color.Gray
                End If

                lblSubmit.ForeColor = Drawing.Color.Gray
            Case "cmborglevelValue"
                'lblStart.Font.Bold = False
                tblStartMenu.Visible = True
                lblMainData.Font.Bold = False
                lblTextTranslation.Font.Bold = False
                lblSystemsInfo.Font.Bold = False
                lblSubmit.Font.Bold = True
                lblSubmit.ForeColor = Drawing.Color.DarkOrange
                'lblStart.ForeColor = Drawing.Color.Orange
                lblMainData.ForeColor = Drawing.Color.Orange
                ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
                ' Comment   : Default focus should be on <Submit> button
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2019-09-26
                btnSubmit.Focus()
                If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_S_T_TXT" Or mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
                    lblTextTranslation.ForeColor = Drawing.Color.Orange
                Else
                    lblTextTranslation.ForeColor = Drawing.Color.Gray
                End If
                lblSystemsInfo.ForeColor = Drawing.Color.Orange
        End Select
        ' Reference END : CR ZHHR 1052471
        '--------------------------------------------------------------------------------
    End Function
    ''' <summary>
    ''' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date      : 2015-12-24
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Next button to handle system info</remarks>
    Protected Sub btnNextEditSystems_Click(sender As Object, e As EventArgs) Handles btnNextEditSystems.Click
        '--------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
        ' Comment   : Delete COMPOSITE_S_T object type
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-01-25
        Select Case mUser.Current_OBJ.OBJ_CLASSIFICATION_ID
            '------------------------------------------------------------------------------
            'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
            'Comment    : On third mask image buttons are replaced by button controls
            'Added by   : Pratyusa Lenka (CWID : EOJCG)
            'Date       : 2016-02-22
            Case Is = "COMPOSITE_MULTI_TXT"
                If Not ValidateText() Then
                    ChangeIcons()
                    EnableDisableRibbonControls("btnNextEditText", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                    btnTextFill.Visible = True
                    btnOverwriteEnglish.Visible = True
                    Exit Sub
                Else
                    EnableDisableRibbonControls("btnNextEditSystems", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                End If
            Case Is = "COMPOSITE_S_T_TXT"
                If Not CheckTextLenght() Then
                    ChangeIcons()
                    EnableDisableRibbonControls("btnNextEditText", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                    btnTextFill.Visible = True
                    btnOverwriteEnglish.Visible = True
                    Exit Sub
                Else
                    EnableDisableRibbonControls("btnNextEditSystems", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                End If
                ' Reference END : CR ZHHR 1053558
                '------------------------------------------------------------------------------
            Case Is = "SINGLE"
                Me.Validate()
                If Page.IsValid Then
                    EnableDisableRibbonControls("btnNextEditSystems", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                    SaveMaster()
                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
                    ' Comment           : Added below code
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 08-Feb-2016
                    Dim lSysStatus As Boolean = mUser.GBOXmanager.GetSysDependentInfo(mUser.Current_OBJ.OBJ_ID.ToString)
                    If (lSysStatus = False) Then
                        If lblVersionnumber.Text <> "1" And mUser.RequestType = myUser.RequestTypeOption.insert Then
                            lblStatus.Text = "Key Value already exists. To change it select the value in the table and cancel here."
                            ChangeIcons()
                            lstFields.Items.Clear()
                            lstValues.Items.Clear()
                            lstDisplay.Items.Clear()
                            lstSQL.Items.Clear()

                            Exit Sub
                        End If
                    End If
                Else
                    ChangeIcons()
                    'EnableDisableRibbonControls("btnNextEditText", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                    'If any validation message is displayed, stay in the same screen for tblStartMenu
                    EnableDisableRibbonControls("btnAddValue", "")
                    Exit Sub
                End If
            Case Else
                'grdDat.AutoGenerateSelectButton = False
                btnAddValue.Enabled = False
        End Select
        ' Reference END : CR ZHHR 1052471
        '--------------------------------------------------------------------------------
        lblSysObj_ID.Text = lblObj_ID.Text
        lblSysObj_VALUE.Text = lblObj_VALUE.Text
        lblSysVersionnumber.Text = lblVersionnumber.Text
        If (cmborglevel.SelectedValue.ToString <> "" And cmborglevelvalue.SelectedValue.ToString <> "") Then
            chkSYSTEMS.Visible = True
        Else
            chkSYSTEMS.Items.Clear()
            chkSYSTEMS.Visible = False
        End If
        mvContents.ActiveViewIndex = GetIndexByViewName("vwEditSysthems")
        btnAddValue.Enabled = False
        txtRequestComment.Text = ""
        ChangeIcons()
        ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
        ' Comment   : Default focus should be on <Submit> button
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2019-09-26
        btnSubmit.Focus()
    End Sub
    ''' <summary>
    ''' Reference : CR ZHHR 1050708 - GBOX COC: OTT 1723 - improve usability
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ''' Date      : 2015-12-24
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>SUbmit the DRS request</remarks>
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
        ' Comment   : Default focus should be on <Submit> button
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2019-09-26
        btnSubmit.Focus()
        If InStr(cmborglevel.Text.ToUpper, "Please".ToUpper) <> 0 Then
            lblStatus.Text = "Choose Orglevel for the request (Dropdown Boxes)"
            Exit Sub
        End If
        If InStr(cmborglevelvalue.Text.ToUpper, "Please".ToUpper) <> 0 Then
            lblStatus.Text = "Choose OrglevelValue for the request (Dropdown Boxes)"
            Exit Sub
        End If
        If Not mUser.GBOXmanager.AuthenticateRequest(trvOBJ.SelectedNode.Value, mUser.CW_ID) Then
            lblStatus.Text = "You have no permission to request values for " & trvOBJ.SelectedNode.Value & "."
            imgCancel_Click(sender, Nothing)
            Exit Sub
        End If
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : Check if the System dependent work flow is true for this object
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 22-Feb-2016

        Dim lSysStatus As Boolean = mUser.GBOXmanager.GetSysDependentInfo(mUser.Current_OBJ.OBJ_ID.ToString)
        Dim iCntx As Integer = 0
        Dim lSysCheck As Boolean = False
        Dim lCheckEntry As Boolean = False
        ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
        ' Comment   : Checks the values exist for non system dependent cust objects
        ' Added by  : Pratyusa Lenka (CWID : EOJCG)
        ' Date      : 2018-10-17
        Dim _lstObjKeyFields As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))
        If Not lSysStatus Then
            Dim lWherestring As String = ""
            Dim strSys As String = ""
            ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729/JIRA GBOX 196: SUP - Problem with CUSTOMIZING_APPLICATION KOKRS
            ' Comment   : IsAtleastOneSystemSelected - Proceed without any message in case of no system is selected
            ' Added by  : Pratyusa Lenka (CWID : EOJCG)
            ' Date      : 2019-02-08
            If mUser.RequestType = myUser.RequestTypeOption.insert AndAlso IsAtleastOneSystemSelected() Then
                If Not Session("ObjKeyFields") Is Nothing Then
                    _lstObjKeyFields = Session("ObjKeyFields")
                    For Each pair As KeyValuePair(Of String, String) In _lstObjKeyFields
                        If pair.Key = "APPLICATION_ID" Then
                            For iCntx = 0 To chkSYSTEMS.Items.Count - 1
                                If (chkSYSTEMS.Items(iCntx).Selected = True) Then
                                    If (chkSYSTEMS.Items(iCntx).Text <> "") Then
                                        If (strSys.Trim = "") Then
                                            strSys = "'" & chkSYSTEMS.Items(iCntx).Text.Split("(")(0).Trim & "'"
                                        Else
                                            strSys += ",'" & chkSYSTEMS.Items(iCntx).Text.Split("(")(0).Trim & "'"
                                        End If
                                    End If
                                End If
                            Next
                            lWherestring = lWherestring & LTrimRTrim(pair.Key) & " in ( " & LTrimRTrim(strSys) & " ) and "
                        Else
                            lWherestring = lWherestring & pair.Key & "='" & pair.Value & "' and "
                        End If

                    Next
                    lWherestring = lWherestring.Substring(0, lWherestring.Length - 4)
                    If mUser.GBOXmanager.CheckEntryExist(mUser.Current_OBJ.OBJ_ID.ToString, lWherestring) Then
                        lblStatus.Text = "Key Value already exists. To change it select the value in the table and cancel here."
                        Exit Sub
                    End If
                End If
            End If
        End If
        If (lSysStatus) Then
            'If lblVersionnumber.Text <> "1" And mUser.RequestType = myUser.RequestTypeOption.insert Then
            '    lblStatus.Text = "Key Value already exists. To change it select the value in the table and cancel here."
            '    Exit Sub
            'End If
            If mUser.RequestType = myUser.RequestTypeOption.insert Then

                For iCntx = 0 To chkSYSTEMS.Items.Count - 1
                    If (chkSYSTEMS.Items(iCntx).Selected = True) Then
                        lSysCheck = True
                        lCheckEntry = mUser.GBOXmanager.CheckEntryExist(mUser.Current_OBJ.OBJ_ID.ToString, chkSYSTEMS.Items(iCntx).Text.Split("(")(0).Trim, lblSysObj_VALUE.Text)

                        If (lCheckEntry) Then
                            lblStatus.Text = "The entry for " & mUser.GBOXmanager.GetTableKey(mUser.Current_OBJ.OBJ_ID.ToString) & " | " & chkSYSTEMS.Items(iCntx).Text.Split("(")(0).Trim & " | " & lblSysObj_VALUE.Text & " is already available. Please create a change request for this entry."
                            Exit Sub
                        End If
                    End If
                Next
                If lSysCheck = False Then
                    lblStatus.Text = "Please choose atleast one system."
                    Exit Sub
                End If
                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729/JIRA GBOX 501: Edit functionality for non-availability of APPLICATION_ID
                ' Comment   : Display the message in case the APPLICATION_ID selected in 1st mask not available in 3rd mask
                ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                ' Date      : 2019-07-05
            ElseIf mUser.RequestType = myUser.RequestTypeOption.update Then
                For iCntx = 0 To chkSYSTEMS.Items.Count - 1
                    If (chkSYSTEMS.Items(iCntx).Selected = True) Then
                        lSysCheck = True
                    End If
                Next
                If lSysCheck = False Then
                    If Not Session("ObjKeyFields") Is Nothing Then
                        _lstObjKeyFields = Session("ObjKeyFields")
                        For Each pair As KeyValuePair(Of String, String) In _lstObjKeyFields
                            If pair.Key = "APPLICATION_ID" Then
                                lblStatus.Text = pair.Value & " system is not available in customizing settings, contact support in case DRS request needs to be created for this system."
                                Exit Sub
                            End If
                        Next
                    End If
                End If
            End If
        End If
        ' Reference End     : ZHHR 1053017

        '-------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1052355 - GBOX COC: OTT 1809 - GBOX pMDAS workflow change
        ' Comment   : Get the customized PMDAS message to display in info email content
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-01-12
        Dim objMDGRequest As New MDG_Request
        Dim strCustomizedPMDASMessage As String = mUser.GBOXmanager.GetCustomizedPMDASMessage()
        If Not String.IsNullOrEmpty(strCustomizedPMDASMessage.Trim) Then
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "GBOX Cockpit", objMDGRequest.javaMsg(strCustomizedPMDASMessage), True)
        End If
        ' Reference END : CR ZHHR 1052355
        '-------------------------------------------------------------------------------
        '' Reference : YHHR 2036565 - GBox: Single Quote Issue
        '' Comment   : change .Replace("'", "`") to .Replace("'", "''")
        '' Added by  : Rajan Dmello (CWID : EOLRG) 
        '' Date      : 2018-10-30

        Dim lFields As New List(Of String)
        Dim lValues As New List(Of String)
        Dim lDisplay As New List(Of String)
        For Each lVal As ListItem In lstFields.Items
            lFields.Add(lVal.Text)
        Next
        For Each lVal As ListItem In lstValues.Items
            lValues.Add(lVal.Text.Replace("'", "''"))
        Next
        For Each lVal As ListItem In lstDisplay.Items
            lDisplay.Add(lVal.Text)
        Next

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
        ' Comment           : Check if the System dependent work flow is true for this object
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 08-Feb-2016


        Dim iCnt As Integer
        Dim lSysAppID As String = ""
        Dim lSqlM As String = ""
        Dim lSql As String = ""
        Dim lSqlr As String = ""
        If (lSysStatus) Then

            mUser.SystemIds = ""
            If (lstSQL.Items.Count > 0) Then
                lSqlM = lstSQL.Items(0).ToString()
                lSqlr = lstSQL.Items(1).ToString
                lstSQL.Items.Clear()

                For iCnt = 0 To chkSYSTEMS.Items.Count - 1
                    If (chkSYSTEMS.Items(iCnt).Selected = True) Then
                        lSysAppID = chkSYSTEMS.Items(iCnt).Value.Split("(")(0).ToString.Trim()
                        lSqlM = lSqlM.Replace("APPLICATION_ID,", "")
                        lSqlr = lSqlr.Replace("APPLICATION_ID=", "")
                        ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                        ' Comment   : Replace the APPLICATION_ID with empty in case of copy functionality
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                        ' Date      : 2019-04-01
                        If lblRequestType.Text = "Copy" Then
                            For Each objKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                                If objKey.Key_ID = "APPLICATION_ID" Then
                                    lSqlM = lSqlM.Replace("N'" & objKey.CurrentValue & "',", "")
                                    lSqlr = lSqlr.Replace("'" & objKey.CurrentValue & "'", "")
                                End If
                            Next
                        Else
                            lSqlM = lSqlM.Replace("N'" & lSysAppID & "',", "")
                            lSqlr = lSqlr.Replace("'" & lSysAppID & "'", "")
                        End If
                        'lSqlM = lSqlM.Replace("'" & lSysAppID & "',", "")
                        'lSqlr = lSqlr.Replace("'" & lSysAppID & "'", "")
                    End If
                Next
            End If

            For iCnt = 0 To chkSYSTEMS.Items.Count - 1
                If (chkSYSTEMS.Items(iCnt).Selected = True) Then
                    lSysAppID = chkSYSTEMS.Items(iCnt).Value.Split("(")(0).ToString.Trim()
                    mUser.SystemIds += lSysAppID & ","
                    ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                    ' Comment   : "(" in description was incorrectly replaced with APPLICATION_ID
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2018-05-25
                    lSql = lSqlM.Replace(mUser.Current_OBJ.OBJ_TABLENAME & "(", mUser.Current_OBJ.OBJ_TABLENAME & "(APPLICATION_ID,")
                    lSql = lSql.Replace("VALUES(", "VALUES('" & lSysAppID & "',")
                    lstSQL.Items.Add(lSql)
                End If
            Next
            Dim strSystems As Array = mUser.SystemIds.Split(",")
            Dim strSys As String = ""
            For i As Integer = 0 To strSystems.Length - 1
                If (strSystems(i) <> "") Then
                    If (i = 0) Then
                        strSys = "'" & strSystems(i) & "'"
                    Else
                        strSys += ",'" & strSystems(i) & "'"
                    End If
                End If
            Next
            ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
            ' Comment   : In case of copy values, request tyoe should be considered as update
            ' Added by  : Pratyusa Lenka (CWID : EOJCG)
            ' Date      : 2019-04-01
            If mUser.RequestType = myUser.RequestTypeOption.insert AndAlso lblRequestType.Text <> "Copy" Then
                lSqlr = lSqlr.Replace("where ", " where APPLICATION_ID IN (" & strSys & ") AND ")
            Else
                lSqlr = lSqlr.Replace("where ", " where APPLICATION_ID IN (" & strSys & ") ")
            End If
            lstSQL.Items.Add(lSqlr)

        End If
        ' Reference         : ZHHR 1053017

        '---------------------------------------------------------------------------------------------------
        ' Reference         : YHHR 2018367 - GBOX COC: Active Flag was not set to 0
        ' Comment           : Since APPLICATION_ID check code not in prod, the update is not proper. Below code can be deleted after 1729 implementation
        ' Added by          : Pratyusa Lenka (CWID : EOJCG)
        ' Date              : 27-Nov-2017
        Dim dtObjField As DataTable = mUser.Databasemanager.MakeDataTable("SELECT * FROM OBJ_FIELD WHERE OBJ_ID = '" & mUser.Current_OBJ.OBJ_ID.ToString & "' AND OBJ_FIELD_ID = 'APPLICATION_ID' AND ISKEYMEMBER = 1")
        If Not dtObjField Is Nothing AndAlso dtObjField.Rows.Count > 0 Then
            Dim strSql As String = ""
            Dim lstSysIDs As List(Of String) = New List(Of String)
            For iCnt = 0 To chkSYSTEMS.Items.Count - 1
                If (chkSYSTEMS.Items(iCnt).Selected = True) Then
                    lstSysIDs.Add(chkSYSTEMS.Items(iCnt).Value.Split("(")(0).ToString.Trim())
                End If
            Next
            Dim strSys As String = String.Join("','", lstSysIDs.ToArray())
            strSys = "'" & strSys.Replace(",", "','") & "'"
            If (lstSQL.Items.Count > 1 AndAlso strSys.Trim() <> "") Then
                strSql = lstSQL.Items(1).ToString
                If InStr(strSql.ToUpper, "APPLICATION_ID") = 0 Then
                    lstSQL.Items.Remove(lstSQL.Items(1).ToString)
                    If mUser.RequestType = myUser.RequestTypeOption.insert Then
                        strSql = strSql.Replace("where ", " where APPLICATION_ID IN (" & strSys & ") AND ")
                    Else
                        strSql = strSql.Replace("where ", " where APPLICATION_ID IN (" & strSys & ") ")
                    End If
                    lstSQL.Items.Add(strSql)
                End If
            End If
        End If
        ' Reference         : YHHR 2018367

        Savedetails(lblVersionnumber.Text, lblObj_VALUE.Text, lFields, lValues, lDisplay)
        mUser.SystemIds = ""
        mUser.RequestType = myUser.RequestTypeOption.update
        Session.Remove("ObjKeyFields")
        Session.Abandon()
        EnableDisableRibbonControls("btnSubmit", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
        Files.Clear()
    End Sub
    ''' <summary>
    ''' Checks if atelast one system is selected
    ''' </summary>
    ''' <returns>True if atelast one system is selected</returns>
    ''' <remarks>Reference OTT 1729 - Date 2019-02-08</remarks>
    Private Function IsAtleastOneSystemSelected() As Boolean
        Dim lSysCheck As Boolean = False
        For iCntx = 0 To chkSYSTEMS.Items.Count - 1
            If (chkSYSTEMS.Items(iCntx).Selected = True) Then
                lSysCheck = True
                Exit For
            End If
        Next
        Return lSysCheck
    End Function
    '---------------------------------------------------------------------------------------------------
    ' Reference         : ZHHR 1053017 - GBOX Cockpit OTT 1729: System dependent workflow
    ' Comment           : Implement the filter 
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 15-Feb-2016
    Private Function GetFilterStatus() As String
        Dim lStatus As String = ""
        'If (rdFilter.Visible = True) Then

        If (rdFilter.SelectedItem.Text = "All") Then
            lStatus = "All"
        ElseIf rdFilter.SelectedItem.Text = "InActive" Then
            lStatus = "InActive"
        Else
            lStatus = "Active"
        End If
        'End If
        Return lStatus
    End Function

    ' Reference End   : ZHHR 1053017

    Protected Sub btnCopyValue_Click(sender As Object, e As EventArgs) Handles btnCopyValue.Click

        Try
            ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
            ' Comment   : Set label request type to copy for copy value to determine the Copy Value is clicked
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2018-04-12
            lblRequestType.Text = "Copy"
            LoadFactoryAndControllerAndBind(mWithPaging)

            For i As Integer = 0 To 1

                lblStatus.Text = ""
                mUser.Databasemanager.Request = Me.Request
                mUser.RequestType = myUser.RequestTypeOption.insert
                If Not mUser.GBOXmanager.AuthenticateRequest(trvOBJ.SelectedNode.Value, mUser.CW_ID) Then
                    lblStatus.Text = "You have no permission to request values for " & trvOBJ.SelectedNode.Value & "."
                    imgCancel_Click(sender, Nothing)
                    Exit Sub
                End If
                If mUser.GBOXmanager.CHECK_MDRS_DATABASE_MAINTENANCE = True Then
                    lblStatus.Text = mUser.GBOXmanager.Get_MDRS_DATABASE_MAINTENANCE_STRING
                    imgCancel_Click(sender, Nothing)
                    Exit Sub
                End If
                'mUser.RequestType = myUser.RequestTypeOption.insert
                'btnSubmit.CommandArgument = "New"
                dvInfo.ToolTip = ""
                btnSubmit.CommandArgument = "New"
                mUser.EditMode = True
                mUser.RequestType = myUser.RequestTypeOption.insert

                mUser.OBJ_Value = ""
                mvContents.ActiveViewIndex = GetIndexByViewName("vwDetails")
                BindDetails(mWithPaging, False)
                'dvInfo.ChangeMode(DetailsViewMode.Insert)

                grdDat.SelectedIndex = hfRowIndex.Value
                'grdDat_SelectedIndexChanged(sender, e)

                If mUser.SystemIds = "" Then
                    If (Not grdDat Is Nothing) Then
                        If grdDat.Rows.Count > 0 Then
                            If grdDat.Rows(hfRowIndex.Value).Cells.Count >= 3 Then
                                mUser.SystemIds = grdDat.Rows(hfRowIndex.Value).Cells(3).Text
                            End If
                        End If
                    End If
                End If

                dvInfo.EnableModelValidation = True
                dvInfo.ChangeMode(DetailsViewMode.Edit)
                dvInfo.DataBind()

                cmborglevel.Items.Clear()
                cmborglevelvalue.Items.Clear()
                Dim ret As Integer = FillCombo(cmborglevel, "ORG_LEVEL_ID", "")
                If ret = 1 Then
                    FillCombo(cmborglevelvalue, "ORG_LEVEL_VALUE", " AND ORG_LEVEL_ID = '" & cmborglevel.SelectedValue & "'")
                End If

                EnableDisableRibbonControls("btnAddValue", "")
                ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                ' Comment   : Enable/disable buttons in case of informative message
                ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                ' Date      : 2019-04-01
                If lblStatus.Text.Trim <> "" Then
                    Enable_Disable("trvOBJ", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                    btnCopyValue.Visible = True
                Else
                    Enable_Disable("btnChangeValue", mUser.Current_OBJ.OBJ_CLASSIFICATION_ID)
                End If
            Next

        Catch ex As Exception
            mErrText &= ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            lblStatus.Text = mErrText
            mvContents.ActiveViewIndex = GetIndexByViewName("vwDetails")
            mErrText &= ""
        End Try


    End Sub
    ''' <summary>
    ''' Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
    ''' Added by   : Pratyusa Lenka (CWID : EOJCG)
    ''' Date       : 2016-02-16
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>On third mask image buttons are replaced by button controls</remarks>
    Protected Sub btnTextFill_Click(sender As Object, e As EventArgs) Handles btnTextFill.Click
        Dim lValue As String = ""
        '---------------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
        ' Comment   : New concept for multiple text functionality
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2015-04-09
        If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
            For i = 1 To tblTexts.Rows.Count - 1
                Dim Countstring As String = Right("0000" & i.ToString, 2)
                Dim lbl_Code As Label = Me.FindControl("lbl_Lang_" & Countstring & "_Code")
                If lbl_Code.Text = "EN" Then
                    For j = 1 To tblTexts.Rows(i).Cells.Count
                        Dim txt_Edit_Multi_Lang As TextBox = Me.FindControl("txt_Edit_Multi_Lang_" & Countstring & "Col_" & j)
                        If Not txt_Edit_Multi_Lang Is Nothing Then
                            lValue = txt_Edit_Multi_Lang.Text
                            For rowCount = 1 To tblTexts.Rows.Count - 1
                                Dim strRowNum As String = Right("0000" & rowCount.ToString, 2)
                                Dim txtEditMultiLang As TextBox = Me.FindControl("txt_Edit_Multi_Lang_" & strRowNum & "Col_" & j)
                                If Not txtEditMultiLang Is Nothing Then
                                    If txtEditMultiLang.Visible = True Then
                                        If txtEditMultiLang.Text.Trim = "" Then
                                            txtEditMultiLang.Text = lValue
                                        End If
                                    Else
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If
            Next
        Else
            For i = 1 To tblTexts.Rows.Count - 1
                Dim Countstring As String = Right("0000" & i.ToString, 2)
                Dim lbl_Code As Label = Me.FindControl("lbl_Lang_" & Countstring & "_Code")
                If lbl_Code.Text = "EN" Then
                    Dim txt_Edit_Lang As TextBox = Me.FindControl("txt_Edit_Lang_" & Countstring)
                    lValue = txt_Edit_Lang.Text
                    Exit For
                End If
            Next
            For i = 1 To tblTexts.Rows.Count - 1
                Dim Countstring As String = Right("0000" & i.ToString, 2)
                Dim txt_Edit_Lang As TextBox = Me.FindControl("txt_Edit_Lang_" & Countstring)
                If txt_Edit_Lang.Visible = True Then
                    If txt_Edit_Lang.Text = "" Then
                        txt_Edit_Lang.Text = lValue
                    End If
                Else
                    Exit For
                End If
            Next
        End If
        ' Reference END : CR ZHHR 1038241
        '--------------------------------------------------------------------
        EnableDisableRibbonControls("btnTextFill", "")
        mvContents.ActiveViewIndex = GetIndexByViewName("vwEditTexts")
    End Sub
    ''' <summary>
    ''' Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
    ''' Added by   : Pratyusa Lenka (CWID : EOJCG)
    ''' Date       : 2016-02-16
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnOverwriteEnglish_Click(sender As Object, e As EventArgs) Handles btnOverwriteEnglish.Click
        Dim lValue As String = ""
        '---------------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
        ' Comment   : New concept for multiple text functionality
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2015-04-09
        If mUser.Current_OBJ.OBJ_CLASSIFICATION_ID = "COMPOSITE_MULTI_TXT" Then
            For i = 1 To tblTexts.Rows.Count - 1
                Dim Countstring As String = Right("0000" & i.ToString, 2)
                Dim lbl_Code As Label = Me.FindControl("lbl_Lang_" & Countstring & "_Code")
                If lbl_Code.Text = "EN" Then
                    For j = 1 To tblTexts.Rows(i).Cells.Count
                        Dim txt_Edit_Multi_Lang As TextBox = Me.FindControl("txt_Edit_Multi_Lang_" & Countstring & "Col_" & j)
                        If Not txt_Edit_Multi_Lang Is Nothing Then
                            lValue = txt_Edit_Multi_Lang.Text
                            For rowCount = 1 To tblTexts.Rows.Count - 1
                                Dim strRowNum As String = Right("0000" & rowCount.ToString, 2)
                                Dim txtEditMultiLang As TextBox = Me.FindControl("txt_Edit_Multi_Lang_" & strRowNum & "Col_" & j)
                                If Not txtEditMultiLang Is Nothing Then
                                    If txtEditMultiLang.Visible = True Then
                                        txtEditMultiLang.Text = lValue
                                    Else
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If
            Next
        Else
            For i = 1 To tblTexts.Rows.Count - 1
                Dim Countstring As String = Right("0000" & i.ToString, 2)
                Dim lbl_Code As Label = Me.FindControl("lbl_Lang_" & Countstring & "_Code")
                If lbl_Code.Text = "EN" Then
                    Dim txt_Edit_Lang As TextBox = Me.FindControl("txt_Edit_Lang_" & Countstring)
                    lValue = txt_Edit_Lang.Text
                    Exit For
                End If
            Next
            For i = 1 To tblTexts.Rows.Count - 1
                Dim Countstring As String = Right("0000" & i.ToString, 2)
                Dim txt_Edit_Lang As TextBox = Me.FindControl("txt_Edit_Lang_" & Countstring)
                If txt_Edit_Lang.Visible = True Then
                    txt_Edit_Lang.Text = lValue
                Else
                    Exit For
                End If
            Next
        End If
        ' Reference END : CR ZHHR 1038241
        '--------------------------------------------------------------------
        EnableDisableRibbonControls("btnOverwriteEnglish", "")
        mvContents.ActiveViewIndex = GetIndexByViewName("vwEditTexts")
    End Sub
    ''' <summary>
    ''' Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
    ''' Added by   : Pratyusa Lenka (CWID : EOJCG)
    ''' Date       : 2016-02-16
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Do not show OBJ_VERSIONNUMBER on  any mask</remarks>
    Protected Sub grdDat_RowCreated(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdDat.RowCreated
        If e.Row.RowType = DataControlRowType.DataRow OrElse e.Row.RowType = DataControlRowType.Header Then
            For Each cell As DataControlFieldCell In e.Row.Cells
                Dim Header As String = DirectCast(cell.ContainingField, DataControlField).ToString().ToUpper
                ' Reference : YHHR 2052807 - GBOX COC: Show locked entries in DRS handbook data grid
                ' Comment   : Hide the Locked column in datagrid
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2019-10-18
                If Header = "VERSIONNO" Or Header = "LOCKED" Then
                    cell.Visible = False
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' Reference : IM0004627941 - YHHR 3011243 - Remove leading and trailing spaces from string.
    ''' Added by  : EOJCH 
    ''' Date      : 2017-07-07
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns>string after Ltrim and RTrim</returns>
    ''' <remarks>Remove Leading and Trailing spaces from the string</remarks>
    Private Function LTrimRTrim(value As String) As String

        If value Is Nothing Then Return value

        Dim returnValue As String = ""
        returnValue = LTrim(value)
        returnValue = RTrim(returnValue)

        Return returnValue
    End Function
    ''' <summary>
    ''' Reference : IM0007270618 - YHHR 2034693 - GBOX Space Display
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2018-09-20
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Restrict multiple space elimination in cell text of grid view</remarks>
    Protected Sub grdDat_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdDat.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            For Each cell As TableCell In e.Row.Cells
                If cell.Text <> "" Then
                    cell.Text = cell.Text.Replace(" ", "&nbsp;")
                End If
            Next
        End If

    End Sub

    Protected Sub trvSearch_TreeNodeExpanded(sender As Object, e As System.Web.UI.WebControls.TreeNodeEventArgs) Handles trvSearch.TreeNodeExpanded
        If e.Node.Parent Is Nothing Then Return
        Dim strNodeValue As String = e.Node.Value
        For Each node As TreeNode In e.Node.Parent.ChildNodes
            If (node.Value <> strNodeValue) Then node.Collapse()
        Next
    End Sub
    Protected Sub clearAttachedFilesToRequest()
        lbAttachment.Items.Clear()
        lbAttachment.Visible = False
    End Sub

    '' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
    '' Comment   : Method to add files to list
    Protected Sub Upload(sender As Object, e As EventArgs)
        'Dim uploadFiles As HttpPostedFile = Context.Request.Files("Filedata")
        'Dim pathToSave As String = HttpContext.Current.Server.MapPath("~/UploadFiles/") & uploadFiles.FileName
        'uploadFiles.SaveAs(pathToSave)
        lblMessage.Text = ""
        Try
            mvContents.ActiveViewIndex = GetIndexByViewName("vwEditSysthems")
            'btnSubmit.Focus()
            If fuAttachment.HasFile Then

                If fuAttachment.PostedFile.ContentLength > 0 Then

                    If fuAttachment.PostedFile.ContentLength < 15728640 Then 'Added Code for CRT 2047360 - Implementation of Attachment on 02-APR-2020 by Anant Jadhav
                        If lbAttachment.Items.Contains(New ListItem(System.IO.Path.GetFileName(fuAttachment.PostedFile.FileName))) Then
                            lblMessage.Text = "File already in the ListBox"
                        Else
                            Files.Add(fuAttachment)
                            lbAttachment.Items.Add(System.IO.Path.GetFileName(fuAttachment.PostedFile.FileName))
                            lblMessage.Text = "Add another file or click Upload to save them all"
                            lbAttachment.Visible = True
                        End If
                        'Start - Added Code for CRT 2047360 - Implementation of Attachment on 02-APR-2020 by Anant Jadhav
                    Else
                        lblMessage.Text = "File size cannot exceed 15MB"
                    End If
                    'End - Added Code for CRT 2047360 - Implementation of Attachment on 02-APR-2020 by Anant Jadhav
                Else
                    lblMessage.Text = "File size cannot be 0"
                End If
            Else
                lblMessage.Text = "Please select a file to add"
            End If

        Catch ex As Exception
            lblMessage.Text = "File Error:- " + ex.Message
        End Try

    End Sub
    ''Method to remove selected files from list
    Protected Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Try
            mvContents.ActiveViewIndex = GetIndexByViewName("vwEditSysthems")
            'btnSubmit.Focus()
            If lbAttachment.Items.Count > 0 Then

                If lbAttachment.SelectedIndex < 0 Then
                    lblMessage.Text = "Please select a file to remove"
                Else
                    Files.RemoveAt(lbAttachment.SelectedIndex)
                    lbAttachment.Items.Remove(lbAttachment.SelectedItem.Text)
                    lblMessage.Text = "File removed"
                    If lbAttachment.Items.Count = 0 Then
                        lbAttachment.Visible = False
                    End If
                End If
            Else
                lblMessage.Text = "No file to remove"
            End If
        Catch ex As Exception
            lblMessage.Text = "File Error:- " + ex.Message
        End Try
    End Sub

    ''Method to download files from list
    Protected Sub btnDownloadAttachment_Click(sender As Object, e As EventArgs) Handles btnDownloadAttachment.Click
        'Protected Sub downloadAttachment(sender As Object, e As EventArgs)
        Try
            mvContents.ActiveViewIndex = GetIndexByViewName("vwEditSysthems")
            'btnSubmit.Focus()
            If lbAttachment.Items.Count > 0 Then

                If lbAttachment.SelectedIndex < 0 Then
                    lblMessage.Text = "Please select a file to download"
                Else
                    Dim fileName = lbAttachment.SelectedItem.Text
                    Dim fileInfo As FileInfo = New FileInfo(lbAttachment.SelectedItem.Text)
                    Dim fileExtension As String = fileInfo.Extension

                    Dim myStream As System.IO.Stream
                    Dim fileLen As Integer
                    Dim displayString As New StringBuilder()

                    If Files.Count > 0 Then
                        For Each item As FileUpload In Files
                            ' Get the length of the file.
                            fileLen = item.PostedFile.ContentLength

                            If lbAttachment.SelectedItem.Text = item.FileName Then
                                ' Create a byte array to hold the contents of the file.
                                Dim Input(fileLen) As Byte

                                ' Initialize the stream to read the uploaded file.
                                myStream = item.FileContent

                                ' Read the file into the byte array.
                                myStream.Read(Input, 0, fileLen)

                                Response.Clear()
                                Response.AddHeader("Cache-Control", "no-cache, must-revalidate, post-check=0, pre-check=0")
                                Response.AddHeader("Pragma", "no-cache")
                                Response.AddHeader("Content-Description", "File Download")
                                Response.AddHeader("Content-Type", "application/force-download")
                                Response.AddHeader("Content-Transfer-Encoding", "binary\n")
                                Response.AddHeader("content-disposition", "attachment;filename=" + fileName)
                                Response.BinaryWrite(Input)
                                Response.End()

                                Exit For
                            End If
                        Next
                    End If

                End If
            Else
                lblMessage.Text = "No file to Download"
            End If
        Catch ex As Exception
            lblMessage.Text = "File Error:- " + ex.Message
        End Try

    End Sub

    ''' <summary>
    ''' Reference : YHHR 2050174 - GBOX COC: Run report by pressing enter
    ''' Comment   : First input field in report should be highlighted and pressing Enter should act like run report button
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2019-08-19
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub dvQuery_DataBound(sender As Object, e As EventArgs) Handles dvQuery.DataBound
        If Not dvQuery Is Nothing AndAlso dvQuery.Rows.Count > 0 Then
            dvQuery.Rows(0).Cells(1).Focus()
            For i = 0 To dvQuery.Rows.Count - 1
                dvQuery.Rows(i).Cells(1).Attributes.Add("onkeypress", "return reportKeyPress(event,'" + imgQuery.ClientID + "')")
            Next
        End If
    End Sub
    ''' <summary>
    ''' Reference : YHHR 2052807 - GBOX COC: Show locked entries in DRS handbook data grid
    ''' Comment   : Check the customizing setting is locked
    ''' Added by  : Pratyusa Lenka (CWID : EOJCG)
    ''' Date      : 2019-10-18
    ''' </summary>
    ''' <param name="drvLock"></param>
    ''' <returns>True if item is locked or not NULL</returns>
    ''' <remarks></remarks>
    Public Function IsItemLocked(drvLock As DataRowView) As Boolean
        For Each obj As DataColumn In drvLock.DataView.Table.Columns
            If obj.ColumnName.ToUpper = "LOCKED" Then
                If Not drvLock.Row("Locked") Is Nothing AndAlso Not IsDBNull(drvLock.Row("Locked")) Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function

    Private Sub btnDelete_Disposed(sender As Object, e As System.EventArgs) Handles btnDelete.Disposed

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
