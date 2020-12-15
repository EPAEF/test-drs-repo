Option Strict Off
Partial Public Class AutoClass
    Inherits System.Web.UI.Page
    Private mExitall As Boolean = False
    Private mUser As myUser
    Private mTxt As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        pObjCurrentUsers.CONTEXT = Context
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035817 - GBOX WebForms: Add New Function for updating columns
            ' Comment           : Added code for updating new columns in User table
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-12-12
            mUser.UserAccessStatus(mUser.CW_ID, "GBOX ACFS")
            ' Reference  END    : CR ZHHR 1035817

        End If
        If mUser.GBOXmanager.CHECK_MDRS_DATABASE_MAINTENANCE() Then
            lblACC.Text = "G|Box autoclassification filtersettings form  is currently locked due to maintenance "
            lblACC.Text = lblACC.Text & vbCrLf & mUser.GBOXmanager.Get_MDRS_DATABASE_MAINTENANCE_STRING
            mvAutoclass.ActiveViewIndex = 2
            Exit Sub
        End If

        If Me.IsPostBack Then Exit Sub
        Dim lApproles As String = mUser.GBOXmanager.Authorisation("GBOX", "FILTERSETTINGS", mUser.CW_ID)
        If InStr(lApproles.ToUpper, "WRITER") <> 0 Or _
            InStr(lApproles.ToUpper, "ADMINISTRATOR") <> 0 Or _
            InStr(lApproles.ToUpper, "IMP") <> 0 Or _
            InStr(lApproles.ToUpper, "AUTH_MGR") <> 0 Or _
            InStr(lApproles.ToUpper, "MDR") <> 0 Then
            Loadcombos()
            Loadstrands(mUser.SUBGROUP_ID)
            cmbSubgroup.Visible = True
            lblSubgroup.Visible = True
            If InStr(lApproles.ToUpper, "ADMINISTRATOR") <> 0 Or _
            InStr(lApproles.ToUpper, "IMP") <> 0 Or _
            InStr(lApproles.ToUpper, "SUPPORTTEAM") <> 0 Then
                lblNoStrands.Visible = True
            Else
                lblNoStrands.Visible = False
            End If
        Else
            mvAutoclass.ActiveViewIndex = 2
            lblACC.Visible = True
            lblACC.Text = lblACC.Text & vbCrLf & "To get access contact one of the following person(s):" & vbCrLf & _
            mUser.Databasemanager.GetRequester("GBOX", "FILTERSETTINGS", "AUTH_MGR", mUser.SUBGROUP_ID)
            hplHome.Visible = True
        End If
        lblDatabase.Text = mUser.Databasemanager.cnSQL.Database

    End Sub

    Function Loadstrands(ByVal lSubgroup As String)
        Dim lStrands As DataTable
        Dim lSQl As String = ""
        If trvRequest.Nodes.Count = 0 Then
            optList.Enabled = True
            cmbSubgroup.Enabled = True
        Else
            optList.Enabled = False
            cmbSubgroup.Enabled = False
        End If
        If lSubgroup.ToUpper <> "ALL" Then
            lSQl = "Select  APPLICATION_ID,SUBGROUP  from APPLICATION_SUBGROUP where SUBGROUP='" & lSubgroup & "'" '& " or SUBGROUP='ALL'"
        Else
            lSQl = "Select  APPLICATION_ID,SUBGROUP   from APPLICATION_SUBGROUP  "
        End If
        Try
            trvStrands.Nodes.Clear()
            lStrands = mUser.Databasemanager.MakeDataTable(lSQl)
            Dim lNostrands As String = "Not loaded: "
            For Each lStrand As DataRow In lStrands.Rows
                Dim myNode As New TreeNode(lStrand("APPLICATION_ID").ToString)
                myNode.SelectAction = TreeNodeSelectAction.SelectExpand
                myNode.ImageUrl = "~/Images/server_key_s.gif"
                'myNode.ToolTip = lStrand("Description").ToString

                '---------------------------------------------------------------------------------------------------
                '' Reference : YHHR 2038450, GBox : Bug in Overview ALL for Filterset
                '' Comment   : Display System with subgroup when ALL option is selected in subgroup DD
                '' Added by  : Rajan Dmello (CWID : EOLRG) 
                '' Date      : 2018-12-03
                If lSubgroup.ToUpper = "ALL" Then
                    myNode.Text = myNode.Text + " (" + lStrand("SUBGROUP").ToString + ")"
                End If

                trvStrands.Nodes.Add(myNode)

                Loadsystems(myNode)
                If myNode.ChildNodes.Count = 0 Then
                    trvStrands.Nodes.Remove(myNode)
                    If lNostrands = "Following strands not loaded. Customize: " Then
                        lNostrands = lNostrands & myNode.Text
                    Else
                        lNostrands = lNostrands & ", " & myNode.Text
                    End If
                End If
            Next lStrand
            lblNoStrands.Text = lNostrands
            chkCurrentValues.DataSource = Nothing
            chkCurrentValues.DataBind()
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
    Sub Loadsystems(ByVal lNode As TreeNode)

        Dim lsql = "Select  APPLICATION_PART_ID,DESCRIPTION from [vw_auto_strands] where APPLICATION_ID='" & lNode.Text & "' And Subgroup='" & cmbSubgroup.SelectedValue & "'"

        If cmbSubgroup.SelectedValue = "ALL" Then
            lsql = "Select  APPLICATION_PART_ID,DESCRIPTION from [vw_auto_strands] where APPLICATION_ID='" & lNode.Text.Split("(")(0).Trim & "' And Subgroup='" & lNode.Text.Split("(")(1).Split(")")(0).Trim & "'"
        End If

        Dim lSys As DataTable = mUser.databasemanager.MakeDataTable(lsql)
        For Each r As DataRow In lSys.Rows
            Dim myNode As New TreeNode(r("APPLICATION_PART_ID").ToString)
            With myNode
                .SelectAction = TreeNodeSelectAction.SelectExpand
                .ToolTip = r("DESCRIPTION").ToString
                .ImageUrl = "~/Images/server_s.gif"
            End With
            lNode.ChildNodes.Add(myNode)
            LoadMessagetypes(myNode)
            If myNode.ChildNodes.Count = 0 Then
                lNode.ChildNodes.Remove(myNode)
            End If
        Next r
    End Sub

    Sub LoadMessagetypes(ByVal lNode As TreeNode)
        Dim lsql = "Select APPLICATION_ID,MESSAGETYPE_ID from [APPLICATION_MESSAGETYPE] where APPLICATION_ID='" & lNode.Parent.Text.Split("(")(0).Trim & "'" '" and RIGHT(MESSAGETYPE_ID,2)='" & cmbMessagenumber.Text & "'"
        Dim lSys As DataTable = mUser.databasemanager.MakeDataTable(lsql)

        For Each r As DataRow In lSys.Rows
            Dim myNode As New TreeNode(r("MESSAGETYPE_ID").ToString)
            myNode.SelectAction = TreeNodeSelectAction.SelectExpand
            myNode.ImageUrl = "~/Images/transmit_s.gif"
            lNode.ChildNodes.Add(myNode)
            Loadfiltergroups(myNode)
            If myNode.ChildNodes.Count = 0 Then
                lNode.ChildNodes.Remove(myNode)
            End If
        Next r
    End Sub
    Sub Loadfiltergroups(ByVal lNode As TreeNode)
        Dim lsql As String = "SELECT DISTINCT  FILTER_GROUP, APPLICATION_PART_ID" & _
       " FROM SAP_FILTER_SETTINGS  where APPLICATION_PART_ID='" & lNode.Parent.Text & "' AND AREA_ID='" & lNode.Text.Substring(2, 2) & "'"
        Dim lSys As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        For Each r As DataRow In lSys.Rows
            Dim myNode As New TreeNode(r("FILTER_GROUP").ToString & " (Filtergroup)")
            myNode.SelectAction = TreeNodeSelectAction.SelectExpand
            myNode.ImageUrl = "~/Images/table_multiple_s.gif"
            lNode.ChildNodes.Add(myNode)
            Loadorglevel(myNode)
            If myNode.ChildNodes.Count = 0 Then
                lNode.ChildNodes.Remove(myNode)
            End If
        Next r

    End Sub
    Sub Loadorglevel(ByVal lNode As TreeNode)
        Dim lsql = "Select ORG_LEVEL_ID,ApplicationText from [FILTER_OBJECTS] where AREA_ID='" & lNode.Parent.Text.Substring(2, 2) & "'"
        Dim lSys As DataTable = mUser.databasemanager.MakeDataTable(lsql)
        For Each r As DataRow In lSys.Rows
            Dim myNode As New TreeNode(r("ApplicationText").ToString & " (" & r("ORG_LEVEL_ID").ToString & ")")
            With myNode
                .SelectAction = TreeNodeSelectAction.SelectExpand
                .ToolTip = r("ORG_LEVEL_ID").ToString
                .ImageUrl = "~/Images/table_s.gif"
            End With
            Dim lnsql As String = "Select FILTER_VALUE from SAP_FILTER_SETTINGS where APPLICATION_PART_ID ='" & lNode.Parent.Parent.Text & "'"
            lnsql = lnsql & " AND AREA_ID='" & lNode.Parent.Text.Substring(2, 2) & "'"
            lnsql = lnsql & " AND ORG_LEVEL_ID='" & myNode.ToolTip & "'"
            'If optList.SelectedValue = "Filtersetting" Then
            '    lnsql = lnsql & " AND (CLASSIFICATION_TYPE_ID='FS' or CLASSIFICATION_TYPE_ID='ACFS')"
            'Else
            '    lnsql = lnsql & " AND (CLASSIFICATION_TYPE_ID='ACFS' or CLASSIFICATION_TYPE_ID='AC')"
            'End If

            Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lnsql)
            If dt.Rows.Count > 0 Then
                lNode.ChildNodes.Add(myNode)
            End If
        Next r
        trvStrands.CollapseAll()

    End Sub

    Public Sub Loadcombos()
        Dim lPossibleSubgroups As String = "''"
        Dim lSubgroups As DataTable = mUser.Databasemanager.MakeDataTable("Select Distinct Subgroup_ID from AUTHORISATION_SET where CW_ID='" & mUser.CW_ID & "'  AND APPLICATION_PART_ID = 'FILTERSETTINGS'")

        For Each r As DataRow In lSubgroups.Rows
            lPossibleSubgroups = lPossibleSubgroups & ",'" & r("Subgroup_ID").ToString & "'"
        Next
        Dim dt As DataTable
        If InStr(lPossibleSubgroups, "ALL") = 0 Then
            dt = mUser.Databasemanager.MakeDataTable("Select Subgroup_ID from Subgroup where Active = 1 and Subgroup_ID in(" & lPossibleSubgroups & ")")
        Else
            dt = mUser.Databasemanager.MakeDataTable("Select Subgroup_ID from Subgroup where Active = 1")
        End If

        With cmbSubgroup
            .DataSource = dt
            .DataTextField = "Subgroup_ID"
            .DataValueField = "Subgroup_ID"
            .SelectedValue = mUser.SUBGROUP_ID
            .DataBind()
        End With

    End Sub



    Protected Sub cmbSubgroup_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbSubgroup.SelectedIndexChanged
        chkPossibleValues.Items.Clear()
        chkCurrentValues.Items.Clear()
        trvStrands.Nodes.Clear()
        Loadstrands(cmbSubgroup.SelectedValue)
    End Sub

    Protected Sub trvStrands_SelectedNodeChanged(ByVal sender As Object, ByVal e As EventArgs) Handles trvStrands.SelectedNodeChanged
        chkPossibleValues.Items.Clear()
        chkCurrentValues.Items.Clear()
        trvStrands.SelectedNode.Expand()
        If showdata() <> "" Then
            imgSaveAsExcel.Enabled = True
        Else
            imgSaveAsExcel.Enabled = False
        End If
        ChangeIcons()
    End Sub

    Sub ChangeIcons()
        With imgShowRequest
            If .Enabled Then
                .ImageUrl = "~\Images\page_go.gif"
            Else
                .ImageUrl = "~\Images\page_go_grey.gif"
            End If
        End With
        With imgSaveAsExcel
            If .Enabled Then
                .ImageUrl = "~\Images\download.ico"
            Else
                .ImageUrl = "~\Images\download_gray.png"
            End If
        End With
    End Sub

    Public Function Getnode(ByVal lstrNodeText As String, Optional ByVal lParent As TreeNode = Nothing) As TreeNode
        Dim lNode As TreeNode = Nothing
        If lParent Is Nothing Then
            lNode = trvRequest.FindNode(lstrNodeText)
        End If
        If lNode Is Nothing Then
            lNode = New TreeNode
            lNode.Text = lstrNodeText
            If lParent Is Nothing Then
                trvRequest.Nodes.Add(lNode)
            Else
                For Each lx As TreeNode In lParent.ChildNodes
                    If lx.Text = lstrNodeText Then
                        Return lx
                    End If
                Next
                lParent.ChildNodes.Add(lNode)
            End If
        End If
        Return lNode
    End Function
    Function getRequestedValues() As String
        Dim lStr As String = "''"
        Dim lSelectedStrand As String = trvStrands.SelectedNode.Parent.Parent.Parent.Parent.Text
        Dim lSelectedSystem As String = trvStrands.SelectedNode.Parent.Parent.Parent.Text
        Dim lSelectedFilter As String = trvStrands.SelectedNode.Parent.Parent.Text
        Dim lSelectedFiltergroup As String = trvStrands.SelectedNode.Parent.Text
        Dim lSelectedOrglevel As String = trvStrands.SelectedNode.Text


        For Each lStrand As TreeNode In trvRequest.Nodes
            If lStrand.Text = lSelectedStrand Then
                For Each lSystem As TreeNode In lStrand.ChildNodes
                    If lSystem.Text = lSelectedSystem Then
                        For Each lFilter As TreeNode In lSystem.ChildNodes
                            If lFilter.Text = lSelectedFilter Then
                                For Each lFilterGroup As TreeNode In lFilter.ChildNodes
                                    If lFilterGroup.Text = lSelectedFiltergroup Then
                                        For Each lOrglevel As TreeNode In lFilterGroup.ChildNodes
                                            If lOrglevel.Text = lSelectedOrglevel Then
                                                For Each lOrglevelvalue As TreeNode In lOrglevel.ChildNodes
                                                    lStr = lStr & ",'" & lOrglevelvalue.Text.Split("(")(0).ToString.Trim & "'"
                                                Next lOrglevelvalue
                                            End If
                                        Next lOrglevel
                                    End If
                                Next lFilterGroup
                            End If
                        Next lFilter
                    End If
                Next lSystem
            End If
        Next lStrand
        Return lStr
    End Function

    Sub cleanup(ByVal lNode As TreeNode)

        Try
            If lNode.Parent.Parent.Parent.Parent Is Nothing Then
                Exit Sub
            Else
                lNode.Parent.ChildNodes.Remove(lNode)
            End If
        Catch ex As Exception
            If lNode.ChildNodes.Count = 0 Then
                If lNode.Parent Is Nothing Then Exit Sub
                lNode.Parent.ChildNodes.Remove(lNode)
            Else
                For Each xNode In lNode.ChildNodes
                    cleanup(xNode)
                Next
            End If

        End Try

    End Sub
    Protected Sub txtSearch_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtSearch.TextChanged
        showdata()
    End Sub

    Function showdata() As String
        Dim lCurrentValues As String = ""
        If trvStrands.SelectedNode Is Nothing Then Return lCurrentValues
        If trvStrands.SelectedNode.Parent Is Nothing Then Return lCurrentValues
        If trvStrands.SelectedNode.Parent.Parent Is Nothing Then Return lCurrentValues
        If trvStrands.SelectedNode.Parent.Parent.Parent Is Nothing Then Return lCurrentValues
        If trvStrands.SelectedNode.Parent.Parent.Parent.Parent Is Nothing Then Return lCurrentValues
        Dim extlsql As String = ""
        Dim lsql As String = "SELECT [LookupTable],[LookupKey],[LookupDescribtionColumn] ,[Description] ,[ApplicationText] FROM FILTER_OBJECTS where [AREA_ID]='" & trvStrands.SelectedNode.Parent.Parent.Text.Substring(2, 2) & "'" & " And [ORG_LEVEL_ID]='" & trvStrands.SelectedNode.ToolTip & "'  And Active ='1'"
        Dim fo As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        lblDatabase.Text = mUser.Databasemanager.cnSQL.Database
        If fo.Rows.Count = 0 Then
            lsql = "Select FILTER_VALUE,CLASSIFICATION_TYPE_ID from SAP_FILTER_SETTINGS where APPLICATION_PART_ID ='" & trvStrands.SelectedNode.Parent.Parent.Parent.Parent.Text & "'"
            lsql = lsql & " AND AREA_ID='" & trvStrands.SelectedNode.Parent.Parent.Text.Substring(2, 2) & "'"
            lsql = lsql & " AND ORG_LEVEL_ID='" & trvStrands.SelectedNode.ToolTip & "'"
            lsql = lsql & " AND FILTER_GROUP='" & trvStrands.SelectedNode.Parent.Text & "'"
            Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
            With chkCurrentValues
                .DataSource = dt
                .DataBind()
            End With
            chkPossibleValues.DataSource = Nothing
            chkPossibleValues.DataBind()
            chkPossibleValues.Items.Clear()
        Else
            If optList.SelectedValue = "Filtersetting" Then
                extlsql = "Select FILTER_VALUE from SAP_FILTER_SETTINGS where (APPLICATION_PART_ID ='" & trvStrands.SelectedNode.Parent.Parent.Parent.Text & "'"
                extlsql = extlsql & " AND AREA_ID='" & trvStrands.SelectedNode.Parent.Parent.Text.Substring(2, 2) & "'"
                'FILTER_GROUP
                extlsql = extlsql & " AND FILTER_GROUP='" & trvStrands.SelectedNode.Parent.Text.Split("(")(0).ToString & "'"

                extlsql = extlsql & " AND ORG_LEVEL_ID='" & trvStrands.SelectedNode.ToolTip & "' AND CLASSIFICATION_TYPE_ID='FS')"
                extlsql = extlsql & " OR "
                extlsql = extlsql & "(APPLICATION_PART_ID ='" & trvStrands.SelectedNode.Parent.Parent.Parent.Text & "'"
                extlsql = extlsql & " AND AREA_ID='" & trvStrands.SelectedNode.Parent.Parent.Text.Substring(2, 2) & "'"
                extlsql = extlsql & " AND ORG_LEVEL_ID='" & trvStrands.SelectedNode.ToolTip & "' AND CLASSIFICATION_TYPE_ID='ACFS')"

            End If
            If optList.SelectedValue = "Autoclassification" Then

                extlsql = extlsql & "Select FILTER_VALUE from SAP_FILTER_SETTINGS where(APPLICATION_PART_ID ='" & trvStrands.SelectedNode.Parent.Parent.Parent.Text & "'"
                extlsql = extlsql & " AND AREA_ID='" & trvStrands.SelectedNode.Parent.Parent.Text.Substring(2, 2) & "'"
                'FILTER_GROUP
                extlsql = extlsql & " AND FILTER_GROUP='" & trvStrands.SelectedNode.Parent.Text.Split("(")(0).ToString & "'"

                extlsql = extlsql & " AND ORG_LEVEL_ID='" & trvStrands.SelectedNode.ToolTip & "' AND (CLASSIFICATION_TYPE_ID='ACFS' OR CLASSIFICATION_TYPE_ID='AC'))"

            End If

            If optList.SelectedValue = "Both" Then

                extlsql = extlsql & "Select FILTER_VALUE from SAP_FILTER_SETTINGS where(APPLICATION_PART_ID ='" & trvStrands.SelectedNode.Parent.Parent.Parent.Text & "'"
                extlsql = extlsql & " AND AREA_ID='" & trvStrands.SelectedNode.Parent.Parent.Text.Substring(2, 2) & "'"
                'FILTER_GROUP
                extlsql = extlsql & " AND FILTER_GROUP='" & trvStrands.SelectedNode.Parent.Text.Split("(")(0).ToString & "'"

                extlsql = extlsql & " AND ORG_LEVEL_ID='" & trvStrands.SelectedNode.ToolTip & "' AND CLASSIFICATION_TYPE_ID='ACFS')"

            End If
            Dim lTXT As String = ""
            lTXT = txtCurSearch.Text

            'EIN SCHLÜSSELFELD GEHT SO: 
            Dim lLookuptable As String = fo.Rows(0)("LookupTable").ToString
            Dim lLookupKey As String = fo.Rows(0)("LookupKey").ToString
            Dim lLookupDescribtionColumn As String = fo.Rows(0)("LookupDescribtionColumn").ToString
            lsql = "Select FILTER_VALUE, FILTER_VALUE + ' (' +" & lLookupDescribtionColumn & "+')' As txt from SAP_FILTER_SETTINGS " & _
                    "  Left Join " & lLookuptable & " on FILTER_Value= " & lLookupKey
            '"  Left Join CLASSIFICATION_TYPE on SAP_FILTER_SETTINGS.CLASSIFICATION_TYPE_ID= CLASSIFICATION_TYPE.CLASSIFICATION_TYPE_ID " & _
            lsql = lsql & " where APPLICATION_PART_ID ='" & trvStrands.SelectedNode.Parent.Parent.Parent.Text & "'"
            lsql = lsql & " AND AREA_ID='" & trvStrands.SelectedNode.Parent.Parent.Text.Substring(2, 2) & "'"
            lsql = lsql & " AND ORG_LEVEL_ID='" & trvStrands.SelectedNode.ToolTip & "'"
            lsql = lsql & " AND (" & lLookupKey & " LIKE '%" & lTXT & "%'"
            lsql = lsql & " or " & lLookupDescribtionColumn & " LIKE '%" & lTXT & "%')"
            lsql = lsql & " And FILTER_VALUE not in (" & getRequestedValues() & ")"
            lsql = lsql & " AND FILTER_GROUP='" & trvStrands.SelectedNode.Parent.Text.Split("(")(0).Trim & "'"

            Select Case optList.SelectedValue
                Case "Filtersetting"
                    lsql = lsql & " AND (SAP_FILTER_SETTINGS.CLASSIFICATION_TYPE_ID='FS' OR SAP_FILTER_SETTINGS.CLASSIFICATION_TYPE_ID='ACFS')"
                Case "Autoclassification"
                    lsql = lsql & " AND (SAP_FILTER_SETTINGS.CLASSIFICATION_TYPE_ID='ACFS' OR SAP_FILTER_SETTINGS.CLASSIFICATION_TYPE_ID='AC')"
                Case "Both"
                    lsql = lsql & " AND (SAP_FILTER_SETTINGS.CLASSIFICATION_TYPE_ID='ACFS')"

            End Select
            lsql = lsql & " Order By FILTER_VALUE "
            Dim dt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
            With chkCurrentValues
                .DataSource = dt
                .DataTextField = "txt"
                .DataValueField = "Filter_VALUE"
                .DataBind()
            End With
            lTXT = txtSearch.Text
            lCurrentValues = lsql

            lsql = "Select " & lLookupKey & ", " & lLookupKey & "+' (' + " & lLookupDescribtionColumn & "+')' as txt  from " & lLookuptable & " where " & lLookupKey & " not in (" & extlsql & ")AND " & _
            "(" & lLookupKey & " LIKE '%" & lTXT & "%' or " & lLookupDescribtionColumn & " LIKE '%" & lTXT & "%')" & _
            "AND " & lLookupKey & " not in (" & getRequestedValues() & ") Order by " & lLookupKey

            fo = mUser.Databasemanager.MakeDataTable(lsql)
            With chkPossibleValues
                .DataSource = fo
                .DataTextField = "txt"
                .DataValueField = lLookupKey
                .DataBind()

            End With
            'wie geht das mit Mehreren ?
        End If
        Return lCurrentValues
    End Function
    Protected Sub optList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles optList.SelectedIndexChanged
        If trvRequest.Nodes.Count > 0 Then
            If MsgBox("This will reset yor request", MsgBoxStyle.OkCancel + MsgBoxStyle.Question) = MsgBoxResult.Ok Then
                trvRequest.Nodes.Clear()
            End If
        End If
        showdata()
    End Sub
    Sub Delnode(ByVal ltreenode As TreeNode)
        If mExitall Then Exit Sub
        If ltreenode.Parent Is Nothing Then
            If ltreenode.ChildNodes.Count = 1 Then
                trvRequest.Nodes.Remove(ltreenode)
                mExitall = True
                Exit Sub
            End If
        End If
        If ltreenode.Parent.ChildNodes.Count > 1 Then
            ltreenode.Parent.ChildNodes.Remove(ltreenode)
        Else
            Delnode(ltreenode.Parent)
        End If
    End Sub
    Protected Sub cmdsubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdsubmit.Click
        Dim lInsertPackage As New List(Of String)
        Dim lRolebackPack As String = ""
        Dim lImplementationblock As String = ""
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If
        Dim lGboxId As String = lblGboxId.Text.Replace("_DRS", "")
        Dim lFilterSetting As New Filtersetting
        For Each lStrand As TreeNode In trvRequest.Nodes
            For Each lSystem As TreeNode In lStrand.ChildNodes
                For Each lFilter As TreeNode In lSystem.ChildNodes
                    For Each lFilterGroup As TreeNode In lFilter.ChildNodes
                        For Each lOrglevel As TreeNode In lFilterGroup.ChildNodes
                            For Each lOrglevelvalue As TreeNode In lOrglevel.ChildNodes
                                Dim lClassType As String = FiltersettingExists(lSystem.Text, lFilterGroup.Text.Split("(")(0).Trim, lFilter.Text.Substring(2, 2), lOrglevel.ToolTip, lOrglevelvalue.Text.Split("(")(0).Trim)
                                Dim lsql As String
                                With lFilterSetting
                                    .APPLICATION_PART_ID = lSystem.Text.ToString
                                    .AREA_ID = lFilter.Text.Substring(2, 2)
                                    .FILTER_GROUP = lFilterGroup.Text.Split("(")(0).Trim
                                    .ORG_LEVEL_ID = lOrglevel.ToolTip
                                    .FILTER_VALUE = lOrglevelvalue.Text.Split("(")(0).Trim
                                End With
                                If lOrglevelvalue.ImageUrl = "~/Images/Page_ADD_s.gif" Then
                                    'AC / FS hinzufügen
                                    If lClassType <> "" Then
                                        'Es ist bereits ein Eintrag vorhanden
                                        lsql = lFilterSetting.getUpdate("ACFS")
                                        lInsertPackage.Add(lsql)
                                        lsql = lFilterSetting.getUpdate(lClassType)
                                        lRolebackPack = lRolebackPack & lsql & vbCrLf
                                    Else
                                        'Es ist noch kein Eintrag vorhanden
                                        lsql = lFilterSetting.getInsert(optList.SelectedValue)
                                        lInsertPackage.Add(lsql)
                                        lsql = lFilterSetting.getDelete()
                                        lRolebackPack = lRolebackPack & lsql & vbCrLf
                                    End If
                                Else
                                    'AC / FS löschen
                                    Dim lClassifiaction As String = ""
                                    Select Case optList.SelectedValue
                                        Case "Filtersetting"
                                            lClassifiaction = "AC"
                                        Case "Autoclassification"
                                            lClassifiaction = "FS"
                                        Case "Both"
                                            lClassifiaction = "ACFS"
                                    End Select
                                    If optList.SelectedValue = "Both" Then
                                        'Bei "Both" sollen auch beide (also ACFS) gelöscht werden.    
                                        lsql = lFilterSetting.getDelete()
                                        lInsertPackage.Add(lsql)
                                        lsql = lFilterSetting.getInsert(lClassifiaction)
                                        lRolebackPack = lRolebackPack & lsql & vbCrLf
                                    Else
                                        'Bei "AC" bzw. "FS" wird unterschieden:
                                        If lClassType = "ACFS" Then
                                            'Wenn bereits ein Satz mit ACFS vorhanden ist, wird auf AC bzw. FS geupdatet.
                                            lsql = lFilterSetting.getUpdate(lClassifiaction)
                                            lInsertPackage.Add(lsql)
                                            lsql = lFilterSetting.getUpdate(lClassType)
                                            lRolebackPack = lRolebackPack & lsql & vbCrLf
                                        Else
                                            'hier kann nur ein Satz mit AC, bzw. FS vorhanden sein (welcher gelöscht wird)
                                            lsql = lFilterSetting.getDelete()
                                            lInsertPackage.Add(lsql)
                                            lsql = lFilterSetting.getInsert(lClassType)
                                            lRolebackPack = lRolebackPack & lsql & vbCrLf
                                        End If
                                    End If
                                End If
                            Next lOrglevelvalue
                        Next lOrglevel
                    Next lFilterGroup
                Next lFilter
            Next lSystem
        Next lStrand

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1049328 - GBOX ACFS OTT 1048: New Workflow for ACFS
        ' Comment           : New function added for ACFS workflow
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 29-Oct-2015
        With lFilterSetting
            lInsertPackage.AddRange(wf_MakePackage("ACFS", "SUBGROUP", cmbSubgroup.SelectedValue, lGboxId, txtRequest.Text))
        End With
        ' Reference End    : ZHHR 1049328

        Dim lCustomerMail As String = txtUserMail.Text & vbCrLf & "Sincerely, your MDAS Team"
        'SAVE Workflowdata
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1053136 - GBOX MGR : Bug-Active flag wasn't set to FALSE
        ' Comment           : IM0002538068 - Two completion mails for ACFS requests, Commented by below lines for blocking LOG_TABLE_USER
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 02-Feb-2016
        'Dim nstrSql As String = " INSERT INTO [LOG_TABLE_USER] " & _
        '         "([MARTY_ID],[CW_ID],[SMETEXT],[Customer],[Customer_Date],[Implementation_State], [Request_Type_Id]" & _
        '         " )VALUES ('" & lGboxId & "'," & _
        '         "N'" & mUser.CW_ID & "','" & lCustomerMail.Replace(",", ";").Replace("'", "''") & "'," & "'" & mUser.CW_ID & "',getdate()," & _
        '         "'submitted','GBOX_REQUEST')"
        'lInsertPackage.Add(nstrSql)
        ' Reference End    : ZHHR 1053136 
        '---------------------------------------------------------------------------------------
        'Save Usermail
        Dim nstrSql As String = ""
        nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                    "([M_MAILKEY]" & _
                    ",[M_RECIPIENTS]" & _
                    ",[M_SUBJECT]" & _
                    ",[M_BODY]" & _
                    ",[M_CURRENT_SENDER])" & _
                    " VALUES('" & lGboxId & "','" & mUser.SMTP_EMAIL & "','GBOX_REQUEST:" & lGboxId & " has been created:'," & _
                    "N'" & lCustomerMail.Replace(",", ";").Replace("'", "''") & "','G_BOX')"
        lInsertPackage.Add(nstrSql)
        '---------------------------------------------------------------------------------------
        'Save Hotlinemail

        Dim lHotlinetext As String = txtImplementationMail.Text
        nstrSql = "INSERT INTO [M_MAILTRIGGER]" & _
                           "([M_MAILKEY]" & _
                           ",[M_RECIPIENTS]" & _
                           ",[M_SUBJECT]" & _
                           ",[M_BODY]" & _
                           ",[M_CURRENT_SENDER])" & _
                           " VALUES('" & lGboxId & "','" & _
                           mUser.GBOXmanager.GetRecipientByApplicationPart("FILTERSETTINGS") & _
                           "','GBox-Request:" & lGboxId & "'," & _
                           "N'" & lHotlinetext & vbCrLf & vbCrLf & Gettextwithoutdesc() & "','" & mUser.CW_ID & "')"
        lInsertPackage.Add(nstrSql)

        nstrSql = "INSERT INTO LOG_TABLE_ROLLBACK_SQL " & _
                          "([Request_ID]" & _
                            ",[Statement_SQL])" & _
                            " VALUES ( '" & lGboxId & "', N'" & lRolebackPack.Replace("'", "''") & "')"
        lInsertPackage.Add(nstrSql)
        If mUser.Databasemanager.ExecutePackage(lInsertPackage) Then
            txtRequest.Text = "SME Submitted WITH ID:" & lGboxId & ""
            Loadstrands(cmbSubgroup.SelectedValue)
            trvRequest.Nodes.Clear()
            cmdsubmit.Enabled = False
            optList.Enabled = True
            cmbSubgroup.Enabled = True
        Else
            cmdBack.Enabled = False
            txtRequest.Text = "ERROR OCCURED PLEASE CONTACT BBS-MDRS-Support@bayer.com " & vbCrLf
            txtRequest.Text = mUser.Databasemanager.ErrText
        End If
    End Sub

    Public Function GetTextByArea(ByVal lArea_ID As String)
        Select Case lArea_ID
            Case "CR"
                Return "VENDOR"
            Case "DE"
                Return "CUSTOMER"
            Case "MM"
                Return "MATERIAL"
            Case Else
                Return lArea_ID
        End Select
    End Function
    Public Function FiltersettingExists(ByVal lSystemId As String, ByVal lFiltergroup As String, ByVal lArea As String, ByVal lOrglevel As String, ByVal lValue As String) As String
        Dim lsql As String = "SELECT [CLASSIFICATION_TYPE_ID] FROM [SAP_FILTER_SETTINGS] Where "
        lsql = lsql & "APPLICATION_PART_ID='" & lSystemId & "'"
        lsql = lsql & " AND FILTER_GROUP='" & lFiltergroup & "'"
        lsql = lsql & " AND AREA_ID='" & lArea & "'"
        lsql = lsql & " AND ORG_LEVEL_ID='" & lOrglevel & "'"
        lsql = lsql & " AND FILTER_VALUE='" & lValue & "'"
        Dim mDt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        If mDt.Rows.Count = 0 Then
            Return ""
        Else
            Return mDt.Rows(0)("CLASSIFICATION_TYPE_ID").ToString
        End If
    End Function
    Protected Sub imgAppendFilter_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgAppendFilter.Click
        showdata()
    End Sub

    Protected Sub imgInvertSelection_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgInvertSelection.Click
        For Each l As ListItem In chkPossibleValues.Items
            l.Selected = Not l.Selected
        Next
    End Sub

    Protected Sub imgAdd_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgAdd.Click
        cmbSubgroup.Enabled = False
        optList.Enabled = False

        AddToTree(chkPossibleValues, "~/Images/Page_ADD_s.gif")

    End Sub
    Sub AddToTree(ByVal chk As CheckBoxList, ByVal img As String)
        Dim lStrand As TreeNode = Getnode(trvStrands.SelectedNode.Parent.Parent.Parent.Parent.Text)
        lStrand.ImageUrl = "~/Images/server_key_s.gif"
        Dim lSystem As TreeNode = Getnode(trvStrands.SelectedNode.Parent.Parent.Parent.Text, lStrand)
        lSystem.ImageUrl = "~/Images/server_s.gif"
        Dim lFilter As TreeNode = Getnode(trvStrands.SelectedNode.Parent.Parent.Text, lSystem)
        lFilter.ImageUrl = "~/Images/transmit_s.gif"
        Dim lFiltergroup As TreeNode = Getnode(trvStrands.SelectedNode.Parent.Text, lFilter)
        lFiltergroup.ImageUrl = "~/Images/table_multiple_s.gif"
        Dim lOrglevel As TreeNode = Getnode(trvStrands.SelectedNode.Text, lFiltergroup)
        lOrglevel.ImageUrl = "~/Images/table_s.gif"

        lOrglevel.ToolTip = trvStrands.SelectedNode.ToolTip
        For Each lItem As ListItem In chk.Items
            If lItem.Selected = True Then
                Dim myNode As New TreeNode
                myNode.Text = lItem.Text
                myNode.ImageUrl = img
                If mUser.gboxmanager.CountAllChilds(trvRequest) < 50 Then
                    lOrglevel.ChildNodes.Add(myNode)
                    lblLimit.Visible = True
                Else
                    lblLimit.Text = "Please split your request in parts.."
                    lblLimit.Visible = True
                    trvRequest.BackColor = Drawing.Color.Red
                    trvRequest.ExpandAll()
                    Exit Sub
                End If
            End If
        Next
        If lOrglevel.ChildNodes.Count = 0 Then
            Delnode(lOrglevel)
        End If
        trvRequest.ExpandAll()
        showdata()
    End Sub
    Protected Sub imgRemove_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgDelete.Click
        optList.Enabled = False
        cmbSubgroup.Enabled = False
        AddToTree(chkCurrentValues, "~/Images/Page_delete_s.gif")

    End Sub

    Protected Sub imgReset_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgReset.Click
        trvRequest.Nodes.Clear()
        optList.Enabled = True
        cmbSubgroup.Enabled = True
        showdata()
    End Sub

    Protected Sub imgShowRequest_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgShowRequest.Click

        txtRequest.Text = mTxt & GetText()
        If trvRequest.Nodes.Count <> 0 Then
            cmdsubmit.Enabled = True
        Else
            cmdsubmit.Enabled = False
        End If
        mvAutoclass.ActiveViewIndex = 1
    End Sub
    Private Function Gettextwithoutdesc() As String
        Dim lTempMessage As String = ""
        Dim lTempStrand As String = ""
        Dim lTempSystem As String = ""
        Dim lTempMessageType As String = ""
        Dim lTempFiltergroup As String = ""
        Dim lTempOrglevel As String = ""
        'Dim lDelMessage As String = "Delete " & optList.SelectedValue & vbCrLf
        'Dim lADDMessage As String = "ADD " & optList.SelectedValue & vbCrLf
        Dim lDelMessage As String = ""
        Dim lADDMessage As String = ""

        For Each lStrand As TreeNode In trvRequest.Nodes

            lTempStrand = Space(5) & " strand:" & lStrand.Text & vbCrLf
            For Each lSystem As TreeNode In lStrand.ChildNodes
                lTempSystem = Space(10) & "system:" & lSystem.Text & vbCrLf
                For Each lFilter As TreeNode In lSystem.ChildNodes
                    lTempMessageType = Space(15) & "messagetype:" & lFilter.Text & vbCrLf
                    For Each lFilterGroup As TreeNode In lFilter.ChildNodes
                        lTempFiltergroup = Space(20) & "filtergroup:" & lFilterGroup.Text.Split("(")(0).Trim & vbCrLf
                        For Each lOrglevel As TreeNode In lFilterGroup.ChildNodes
                            lTempOrglevel = Space(25) & "orglevel:" & lOrglevel.Text & vbCrLf
                            For Each lOrglevelvalue As TreeNode In lOrglevel.ChildNodes
                                lTempMessage = lTempStrand & lTempSystem & lTempMessageType & lTempFiltergroup & lTempOrglevel
                                If lOrglevelvalue.ImageUrl = "~/Images/Page_ADD_s.gif" Then
                                    'If optList.SelectedValue = "Both" Or optList.SelectedValue = "Filtersetting" Then
                                    '    lADDMessage = lADDMessage & "ADD Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    'End If
                                    'If optList.SelectedValue = "Both" Or optList.SelectedValue = "Autoclassification" Then
                                    '    lADDMessage = lADDMessage & "ADD Autoclassification" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    'End If                                
                                    If optList.SelectedValue = "Filtersetting" Then
                                        lADDMessage = lADDMessage & "ADD Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                    If optList.SelectedValue = "Autoclassification" Then
                                        lADDMessage = lADDMessage & "ADD Autoclassification" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                    If optList.SelectedValue = "Both" Then
                                        lADDMessage = lADDMessage & "ADD Autoclassification and Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                Else
                                    'If optList.SelectedValue = "Both" Or optList.SelectedValue = "Filtersetting" Then
                                    '    lDelMessage = lDelMessage & "Delete Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    'End If
                                    'If optList.SelectedValue = "Both" Or optList.SelectedValue = "Autoclassification" Then
                                    '    lDelMessage = lDelMessage & "Delete Autoclassification" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    'End If
                                    If optList.SelectedValue = "Filtersetting" Then
                                        lDelMessage = lDelMessage & "Delete Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                    If optList.SelectedValue = "Autoclassification" Then
                                        lDelMessage = lDelMessage & "Delete Autoclassification" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                    If optList.SelectedValue = "Both" Then
                                        lDelMessage = lDelMessage & "Delete Autoclassification and Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                End If
                            Next lOrglevelvalue
                            lTempMessage = ""
                        Next lOrglevel
                    Next lFilterGroup
                Next lFilter
            Next lSystem
        Next lStrand
        'If lDelMessage = "Delete " & optList.SelectedValue & vbCrLf Then
        'lDelMessage = ""
        'End If
        'If lADDMessage = "ADD " & optList.SelectedValue & vbCrLf Then
        'lADDMessage = ""
        'End If

        Return lDelMessage & vbCrLf & lADDMessage

    End Function
    Private Function GetText() As String
        Dim lGboxId As String = mUser.GBOXmanager.GetGBOXId.Replace("_DRS", "_ACFS")
        lblGboxId.Text = lGboxId
        mTxt = vbCrLf & "Dear " & mUser.TITLE & " " & mUser.first_name & " " & mUser.last_name & "," & vbCrLf
        mTxt = mTxt & "thank you for the filter setting and/or automatic classification request."
        mTxt = mTxt & "A Gbox request with the GBox-Ticketnumber(" & lGboxId & ") has been created for you."
        mTxt = mTxt & vbCrLf
        mTxt = mTxt & "The request (technical description) is as follows:" & vbCrLf

        Dim lHotlineText As String = mUser.TITLE & " " & mUser.first_name & " " & mUser.last_name
        lHotlineText = lHotlineText & " requests the following changes: "
        lHotlineText = lHotlineText & vbCrLf
        lHotlineText = lHotlineText & "A Gbox request with the GBox-Ticketnumber(" & lGboxId & ") has been created."
        lHotlineText = lHotlineText & "The request (technical description) is as follows:" & vbCrLf

        Dim lMessage As String = "Subgroup :" & cmbSubgroup.Text & vbCrLf
        lMessage = lMessage & " " & mUser.TITLE & " " & mUser.first_name & " " & mUser.last_name
        lMessage = lMessage & " requests the following changes: "
        lMessage = lMessage & vbCrLf & txtRequest.Text

        Dim lTempMessage As String = ""
        Dim lTempStrand As String = ""
        Dim lTempSystem As String = ""
        Dim lTempMessageType As String = ""
        Dim lTempFiltergroup As String = ""
        Dim lTempOrglevel As String = ""
        'Dim lDelMessage As String = "Delete " & optList.SelectedValue & vbCrLf
        'Dim lADDMessage As String = "ADD " & optList.SelectedValue & vbCrLf
        Dim lDelMessage As String = ""
        Dim lADDMessage As String = ""
        Dim lImplemantationSummary As String = "Implementation Changes Summary:" & vbCrLf
        lImplemantationSummary = lImplemantationSummary & "----------------------------------------------" & vbCrLf
        lImplemantationSummary = lImplemantationSummary & "For changes to the production system, please create a CRT"
        lImplemantationSummary = lImplemantationSummary & " and implement the standard change process." & vbCrLf
        lImplemantationSummary = lImplemantationSummary & "All other changes can be implemented as is." & vbCrLf
        lImplemantationSummary = lImplemantationSummary & "After the changes have been completed, please close the GBOX ticket "
        lImplemantationSummary = lImplemantationSummary & "in the request management." & vbCrLf & vbCrLf
        lImplemantationSummary = lImplemantationSummary & "Object;Org Unit ;Value;Logical System;Class;Action;Procedure" & vbCrLf

        For Each lStrand As TreeNode In trvRequest.Nodes
            lTempStrand = Space(5) & " strand:" & lStrand.Text & vbCrLf
            For Each lSystem As TreeNode In lStrand.ChildNodes
                lTempSystem = Space(10) & "system:" & lSystem.Text
                Dim lsqlQsLevel As String = "select QS_LEVEL FROM APPLICATION_PARTS Where APPLICATION_PART_ID='" & lSystem.Text & "'"
                Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lsqlQsLevel)
                Dim lQSLEVEL As String = ""
                If Not ldt Is Nothing Then
                    If Not ldt.Rows.Count = 0 Then
                        lQSLEVEL = "QS-LEVEL:" & ldt.Rows(0)("QS_LEVEL").ToString
                    End If
                End If
                lTempSystem = lTempSystem & " " & lQSLEVEL & vbCrLf
                For Each lFilter As TreeNode In lSystem.ChildNodes
                    lTempMessageType = Space(15) & "messagetype:" & lFilter.Text & vbCrLf
                    For Each lFilterGroup As TreeNode In lFilter.ChildNodes
                        lTempFiltergroup = Space(20) & "filtergroup:" & lFilterGroup.Text.Split("(")(0).Trim & vbCrLf
                        For Each lOrglevel As TreeNode In lFilterGroup.ChildNodes
                            lTempOrglevel = Space(25) & "orglevel:" & lOrglevel.Text & vbCrLf
                            For Each lOrglevelvalue As TreeNode In lOrglevel.ChildNodes
                                Dim lAddOrDelete As String = ""
                                lTempMessage = lTempStrand & lTempSystem & lTempMessageType & lTempFiltergroup & lTempOrglevel
                                If lOrglevelvalue.ImageUrl = "~/Images/Page_ADD_s.gif" Then
                                    'If optList.SelectedValue = "Both" Or optList.SelectedValue = "Filtersetting" Then
                                    '    lADDMessage = lADDMessage & "ADD Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    'End If
                                    'If optList.SelectedValue = "Both" Or optList.SelectedValue = "Autoclassification" Then
                                    '    lADDMessage = lADDMessage & "ADD Autoclassification" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    'End If
                                    If optList.SelectedValue = "Filtersetting" Then
                                        lADDMessage = lADDMessage & "ADD Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                    If optList.SelectedValue = "Autoclassification" Then
                                        lADDMessage = lADDMessage & "ADD Autoclassification" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                    If optList.SelectedValue = "Both" Then
                                        lADDMessage = lADDMessage & "ADD Autoclassification and Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                    lAddOrDelete = "Add"
                                Else
                                    'If optList.SelectedValue = "Both" Or optList.SelectedValue = "Filtersetting" Then
                                    '    lDelMessage = lDelMessage & "Delete Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    'End If
                                    'If optList.SelectedValue = "Both" Or optList.SelectedValue = "Autoclassification" Then
                                    '    lDelMessage = lDelMessage & "Delete Autoclassification" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    'End If
                                    If optList.SelectedValue = "Filtersetting" Then
                                        lDelMessage = lDelMessage & "Delete Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                    If optList.SelectedValue = "Autoclassification" Then
                                        lDelMessage = lDelMessage & "Delete Autoclassification" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                    If optList.SelectedValue = "Both" Then
                                        lDelMessage = lDelMessage & "Delete Autoclassification and Filtersetting" & vbCrLf & lTempMessage & Space(30) & lOrglevelvalue.Text & vbCrLf
                                    End If
                                    lAddOrDelete = "Del"
                                End If

                                Dim lCrtRequired As String = ""
                                If lQSLEVEL = "QS-LEVEL:P" Then
                                    lCrtRequired = "CRT required"
                                Else
                                    lCrtRequired = "no CRT required"
                                End If
                                If optList.SelectedValue <> "Filtersetting" Then
                                    Dim lGetSapClass As String = "Select  APPLICATION_MESSAGETYPE.SAP_CLASS from APPLICATION_MESSAGETYPE "
                                    lGetSapClass = lGetSapClass & "left Join Application_parts on Application_parts.APPLICATION_ID =  APPLICATION_MESSAGETYPE.APPLICATION_ID"
                                    lGetSapClass = lGetSapClass & " Where [MESSAGETYPE_ID] ='" & lFilter.Text & "' "
                                    lGetSapClass = lGetSapClass & " AND Application_parts.APPLICATION_PART_ID ='" & lSystem.Text & "'"
                                    Dim lSAP_CLASSDt As DataTable = mUser.Databasemanager.MakeDataTable(lGetSapClass)
                                    Dim lSAPCLASS As String = ""
                                    If Not lSAP_CLASSDt Is Nothing Then
                                        If lSAP_CLASSDt.Rows.Count <> 0 Then
                                            lSAPCLASS = lSAP_CLASSDt.Rows(0)("SAP_CLASS").ToString
                                        End If
                                        If lSAPCLASS = "" Then
                                            lSAPCLASS = "please check system entries (TBDLT)"
                                        End If
                                    End If
                                    lImplemantationSummary = lImplemantationSummary & GetTextByArea(lFilter.Text.Substring(2, 2)) & ";" & lOrglevel.ToolTip & ";" & lOrglevelvalue.Text.Split("(")(0).Trim & ";" & lSystem.Text & ";" & lSAPCLASS & ";" & lAddOrDelete & ";" & lCrtRequired & vbCrLf
                                Else
                                    lImplemantationSummary = lImplemantationSummary & GetTextByArea(lFilter.Text.Substring(2, 2)) & ";" & lOrglevel.ToolTip & ";" & lOrglevelvalue.Text.Split("(")(0).Trim & ";" & lSystem.Text & ";" & lAddOrDelete & ";" & lCrtRequired & vbCrLf
                                End If
                            Next lOrglevelvalue
                        Next lOrglevel
                        lTempMessage = ""
                    Next lFilterGroup
                Next lFilter
            Next lSystem
        Next lStrand
        'If lDelMessage = "Delete " & optList.SelectedValue & vbCrLf Then
        'lDelMessage = ""
        'End If
        'If lADDMessage = "ADD " & optList.SelectedValue & vbCrLf Then
        'lADDMessage = ""
        'End If
        txtUserMail.Text = mTxt & lDelMessage & vbCrLf & lADDMessage
        txtImplementationMail.Text = lHotlineText & vbCrLf & lDelMessage & vbCrLf & lADDMessage & vbCrLf & lImplemantationSummary & vbCrLf & "Implementation Changes in detail:" & vbCrLf & "----------------------------------------------"
        Return lDelMessage & vbCrLf & lADDMessage
    End Function


    Protected Sub imgUndo_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgUndo.Click
        If trvRequest.SelectedNode Is Nothing Then
            If trvRequest.Nodes.Count = 0 Then
                optList.Enabled = True
                cmbSubgroup.Enabled = True
                Exit Sub
            End If
        End If
        With trvRequest
            If .SelectedNode Is Nothing Then Exit Sub
            If .SelectedNode.Parent Is Nothing Then Exit Sub
            If .SelectedNode.Parent.Parent Is Nothing Then Exit Sub
            If .SelectedNode.Parent.Parent.Parent Is Nothing Then Exit Sub
            If .SelectedNode.Parent.Parent.Parent.Parent Is Nothing Then Exit Sub
            If .SelectedNode.Parent.Parent.Parent.Parent.Parent Is Nothing Then Exit Sub
            mExitall = False
            Delnode(.SelectedNode)
            showdata()
        End With
        If trvRequest.Nodes.Count = 0 Then
            optList.Enabled = True
            cmbSubgroup.Enabled = True
        End If

    End Sub


    Protected Sub imgcurAppendFilter_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgcurAppendFilter.Click
        showdata()
    End Sub

    Protected Sub imgcurInvertSelection_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgcurInvertSelection.Click
        For Each l As ListItem In chkCurrentValues.Items
            l.Selected = Not l.Selected
        Next
    End Sub

    Protected Sub txtCurSearch_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtCurSearch.TextChanged
        showdata()
    End Sub


    Protected Sub cmdBack_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdBack.Click
        mvAutoclass.ActiveViewIndex = mvAutoclass.ActiveViewIndex - 1
    End Sub

    'Protected Sub imgFind_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgFind.Click
    '    Findall()
    'End Sub
    'Sub Findall()
    '    ' txtFind.Text = txtFind.Text.ToUpper.Replace("*", "")
    '    trvStrands.CollapseAll()
    '    For Each lNode As TreeNode In trvStrands.Nodes
    '        Findnode(lNode)
    '    Next
    'End Sub
    'Sub Findnode(ByVal lNode As TreeNode)
    '    If InStr(lNode.Text, txtFind.Text) <> 0 Then
    '        lNode.Select()
    '        Do Until lNode Is Nothing
    '            lNode.Expand()
    '            lNode = lNode.Parent
    '        Loop
    '        Exit Sub
    '    End If
    '    If lNode.ChildNodes.Count > 0 Then
    '        For Each lcNode In lNode.ChildNodes
    '            Findnode(lcNode)
    '        Next
    '    End If
    'End Sub

    'Protected Sub txtFind_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtFind.TextChanged
    '    Findall()
    'End Sub





    Protected Sub imgSaveAsExcel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgSaveAsExcel.Click
        Dim lFilename As String = Now().ToString.Replace(":", "").Replace(" ", "").Replace(".", "_") & "_" & trvStrands.SelectedNode.ValuePath.Replace("/", "_") & "_CURRENT_VALUES"
        'lFilename = lFilename.Replace("(", "_").Replace(")", "_").Replace(" ", "") & ".XML"
        lFilename = lFilename.Replace("(", "_").Replace(")", "_").Replace(" ", "")
        Dim lsql As String = showdata()
        mUser.GBOXmanager.Makereport(Me.Response, lFilename, lsql, Me.Server, False)
    End Sub


    

    Protected Sub imgEngine_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgEngine.Click
        Dim lsql As String = "Select * from [SAP_FILTER_SETTINGS] where "
        lsql &= " [APPLICATION_PART_ID] Like '" & txtKeywords.Text.Replace("*", "%") & "' or "
        lsql &= "[FILTER_GROUP] Like '" & txtKeywords.Text.Replace("*", "%") & "' or "
        lsql &= "[AREA_ID] Like '" & txtKeywords.Text.Replace("*", "%") & "' or "
        lsql &= "[ORG_LEVEL_ID] Like '" & txtKeywords.Text.Replace("*", "%") & "' or "
        lsql &= "[FILTER_VALUE] Like '" & txtKeywords.Text.Replace("*", "%") & "' or "
        lsql &= "[CLASSIFICATION_TYPE_ID] Like '" & txtKeywords.Text.Replace("*", "%") & "' or "
        lsql &= "[SAP_CLASS] Like '" & txtKeywords.Text.Replace("*", "%") & "'"
        grdSearch.DataSource = mUser.Databasemanager.MakeDataTable(lsql)
        grdSearch.DataBind()
    End Sub

    Protected Sub imgSearchEngine_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgSearchEngine.Click
        mvAutoclass.ActiveViewIndex = 5
    End Sub

    Protected Sub imgSearchCancel_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgSearchCancel.Click
        mvAutoclass.ActiveViewIndex = 0
    End Sub

    Protected Sub cmdExpandAll_Click(sender As Object, e As EventArgs) Handles cmdExpandAll.Click
        If cmdExpandAll.Text = "expand tree" Then
            trvStrands.ExpandAll()
            cmdExpandAll.Text = "collapse tree"
            cmdExpandAll.ToolTip = "back to root nodes"
        Else
            trvStrands.CollapseAll()
            cmdExpandAll.Text = "expand tree"
            cmdExpandAll.ToolTip = "expand tree and use F3 for searching"
        End If
    End Sub

    '---------------------------------------------------------------------------------------------------
    ' Reference         : ZHHR 1049328 - GBOX ACFS OTT 1048: New Workflow for ACFS
    ' Comment           : New function added for ACFS workflow
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 29-Oct-2015
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
        lSql &= " ,'PMDM140'"
        lSql &= " ,NULL"
        lSql &= " ,NULL"
        lSql &= " ,'UNKNOWN'"
        lSql &= " ,'not filled'"
        lSql &= " ,Getdate()"
        lSql &= " ,NULL)"
        lPack.Add(lSql)

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
        lSql &= "   ,'" & mText & "'"
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
                            lSql &= "   'New workflow item in GBOX ACFS: " & MartyId & "',"
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
    ' Reference End      : ZHHR 1049328
End Class

