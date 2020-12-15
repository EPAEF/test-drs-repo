Option Strict Off
Imports System.Text
Imports Bayer.GBOX.FrameworkClassLibrary
Public Class MDG_Request
    Inherits System.Web.UI.Page
    Private mUser As myUser
    Private mGums As myGumsManager
    Private WithEvents mStaticviewcontroller As MyDynamicForm_StaticViewController
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
            Dim lContextUser As String = Context.User.Identity.Name
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(lContextUser)
            mUser.GBOXmanager.User = mUser
            pObjCurrentUsers.CONTEXT = Context

            lblDatabase.Text = mUser.Databasemanager.cnSQL.Database

            imghelp_Logo.OnClientClick = "javascript:window.open('" & "http://by-gbox.bayer-ag.com/HOTLINE/" & "',null, 'toolbar=yes,location=yes, status= yes,menubar=yes, scrollbars=yes,resizable=yes'); void('');"

            If Me.IsPostBack Then Exit Sub

        Catch ex As Exception
            ' Reference         : ZHHR 1053139 - GBOX Webforms: MDG Connection error handling
            ' Comment           : Connection error handling
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 09-Feb-2016
            lblError.Text = "Connection Error, please try later."
            ' Reference End     : ZHHR 1053139
        End Try
    End Sub
    Private Sub LoadMDGRoles()

        Dim lSql As String = "Select * from [MDRS_Configuration] where agr_type_id='CAG_MDG'"
        Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
        If (mdt.Rows.Count > 0) Then

            Dim marea_id As String
            Dim m_approver As String
            Dim m_requester As String
            marea_id = ""
            m_approver = ""
            m_requester = ""

            If (mdt.Rows.Count > 0) Then
                Dim dmdt As DataTable = mdt.DefaultView.ToTable(True, "area_id")
                For Each row In dmdt.Rows
                    chkArea.Items.Add(row.ItemArray(0))
                Next

                dmdt = mdt.DefaultView.ToTable(True, "requester")
                For Each row In dmdt.Rows
                    RdbRequester.Items.Add(row.ItemArray(0))
                    'CreateTB()
                Next
                dmdt = mdt.DefaultView.ToTable(True, "approver")
                For Each row In dmdt.Rows
                    ChkApprover.Items.Add(row.ItemArray(0))
                Next

            End If
        End If
    End Sub
    'Sub CreateTB()
    '    Dim NewTB As New Textbox
    '    NewTB = New TextBox
    '    NewTB.ID = "txtbox"
    '    pnlTbox.Controls.Add(NewTB)
    '    pnlTbox.Controls.Add(New LiteralControl("<br />"))
    'End Sub


    Protected Sub imgSubmitRequest_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgSubmitRequest.Click
        Dim lrSME As String
        Dim lUsers As String
        lrSME = ""
        lUsers = ""
        lblError.Text = ""

        For i = 0 To lstUsersToCheck.Items.Count - 1
            lUsers += lstUsersToCheck.Items(i).Text & vbCrLf
        Next

        For i = 0 To chkShoppingCart.Items.Count - 1
            lrSME += chkShoppingCart.Items(i).Text & vbCrLf
        Next

        If lUsers = "" Or lrSME = "" Then
            '-----------------------------------------------------------------------------------------
            ' Reference : ZHHR 1051815 - GBOX MDG: Change error text when submit without shopping cart
            ' Comment   : Changed the message text when submit without shopping cart
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-12-18
            lblError.Text = "Select Users and Roles for assignment! Add roles to shopping cart!"
            ' Reference  END    : ZHHR 1051815
            '-----------------------------------------------------------------------------------------
            Exit Sub
        End If

        '-------------------------------------------------------------------------------------------------------------
        ' Reference : ZHHR 1069971 - GBOX COC: Update MDG form: add comment field
        ' Comment   : To make request comment mandatory, display error message when field is empty and click on submit
        '           : Add request comment to request text and display in details of request in GBox MGR
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2017-04-11
        If String.IsNullOrEmpty(txtRequestComment.Text.Trim) Then
            lblError.Text = "Please enter the comment for roles needed!"
            txtRequestComment.Focus()
            Exit Sub
        End If

        ' Reference : YHHR 2039818 - Change the description in the MARTY tool for Authorization requests
        ' Comment   : Changed the description to show it in GBox Manager details screen
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2019-01-16
        lrSME = mUser.CW_ID + " created the following request for MDG access. " + vbCrLf + vbCrLf + _
                            "Requested user :" + vbCrLf + lUsers + vbCrLf + "Comment:" + vbCrLf + txtRequestComment.Text.Trim + vbCrLf + vbCrLf + "Requested roles:" + vbCrLf + lrSME + vbCrLf + vbCrLf + _
                            "Please create a PMD account request for these user/roles. After approval of the account request(s) the authorization will be available." + vbCrLf + _
                            "For CR_WF requests always Add the role	 	GLO__ALL_MDGS_________MENU" + vbCrLf + _
                            "For DE_WF requests always Add the role		GLO__ALL_MDGC_________MENU" + vbCrLf + _
                            "For new pmd user always Add the role		GLO__ALL_GEN__________DISP_ALL"

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1049322 - GBOX Webforms OTT 1048: New Workflow for MDG
        ' Comment           : Below lines commented and added workflow functionality
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 27-Oct-2015
        'Dim url As String = GetItServiceUrl(lrSME)
        'Dim stxt As String = "A new tab is opened in your browser. \n" + _
        '                     "Please send the IT Service Portal ticket in this tab to our hotline by clicking the button <SEND>. \n" + _
        '                     "You can close the MDG request form after sending the IT Service Portal Ticket. \n\n" + _
        '                     "The requested roles will be assigned automatically after approval of the account requests. An automated email will be sent to the users."
        'Dim s As String = "alert('" + stxt + "','');window.open('" & url + "', 'MDG_Request');"
        'ClientScript.RegisterStartupScript(Me.GetType(), "script", s, True)
        Dim lPackage As New List(Of String)
        Dim lGboxId As String = Replace(mUser.GBOXmanager.GetGBOXId, "_DRS", "_MDG")

        lPackage.AddRange(wf_MakePackage("MDG_OBJ", lGboxId, mUser, lrSME))

        'SAVE Workflowdata
        ' Reference         : ZHHR 1054647 - GBOX Webforms: GBOX disable LOG_TABLE_USER (old workflow)
        ' Comment           : comments below code for LOG_TABLE_USER
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 14-Mar-2016
        'Dim nstrSql As String = " INSERT INTO [LOG_TABLE_USER] " & _
        '         "([MARTY_ID],[CW_ID],[SMETEXT],[Customer],[Customer_Date],[Implementation_State], [Request_Type_Id]" & _
        '         " )VALUES ('" & lGboxId & "'," & _
        '         "'" & mUser.CW_ID & "','" & lrSME.Replace(",", ";").Replace("'", "''") & "'," & "'" & mUser.CW_ID & "','" & Now.Year & "_" & Now.Month & "_" & Now.Day & "_" & Now.Hour & "_" & Now.Minute & "_" & Now.Second & "_" & Now.Millisecond & "'," & _
        '         "'submitted','GBOX_REQUEST')"
        'lPackage.Add(nstrSql)
        ' Reference End     : ZHHR 1054647

        If (lPackage.Count > 0) Then
            If mUser.Databasemanager.ExecutePackage(lPackage) Then
                '---------------------------------------------------------------------------------------------------
                ' Reference         : ADHOC change - MDG Request duplication changes
                ' Comment           : MDG duplicate records.
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 10-Dec-2015
                'lblError.Text = "Your request " & lGboxId & " has been submitted."
                ClearFields()
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "MDG Request", javaMsg("Your request " & lGboxId & " has been submitted."), True)
                ShowRoles()
                ' Reference End     : ADHOC change
            Else
                lblError.Text = "Your request is not submitted."
            End If
        End If

        ' Reference End     : ZHHR 1049322
    End Sub
    '---------------------------------------------------------------------------------------------------
    ' Reference         : ADHOC change - MDG Request duplication changes
    ' Comment           : MDG duplicate records.
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 10-Dec-2015
    Public Function javaMsg(ByVal message As String) As String

        Dim sb As New System.Text.StringBuilder()
        sb.Append("window.onload=function(){")
        sb.Append("alert('")
        sb.Append(message)
        sb.Append("')};")

        Return sb.ToString()

    End Function
    Private Sub ClearFields()
        chkArea.ClearSelection()
        ChkApprover.ClearSelection()
        chkShoppingCart.Items.Clear()
        lstUsersToCheck.Items.Clear()
        txtBegru.Text = ""
        txtboxLE.Text = ""
        txtHub.Text = ""
        txtEKORG.Text = ""
        txtVKORG.Text = ""
        RdbRequester.ClearSelection()
    End Sub
    ' Reference End    : ADHOC change

    Protected Sub imgAddRole_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgAddRole.Click
        Dim z As Integer = chkShoppingCart.Items.Count - 1
        Dim bduplicate As Boolean = False
        lblError.Text = ""

        For Each lItem As ListItem In chkBoxAgr.Items
            'Check duplicates
            For i As Integer = 0 To z
                If ((lItem.Text = chkShoppingCart.Items(i).ToString) And (lItem.Selected)) Then
                    lblError.Text = "Role(s) already added to shopping cart."
                    bduplicate = True
                    Exit For
                Else
                    bduplicate = False
                End If
            Next

            If lItem.Selected And bduplicate = False Then
                chkShoppingCart.Items.Add(lItem)
            End If
        Next lItem
        chkShoppingCart.ClearSelection()
    End Sub

    Protected Sub imgRemoveRole_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgRemoveRole.Click
        Dim lItem As New List(Of ListItem)
        Dim z As Integer = chkShoppingCart.Items.Count - 1
        For i As Integer = 0 To z
            If Not chkShoppingCart.Items(i).Selected Then
                lItem.Add(chkShoppingCart.Items(i))
            End If
        Next i
        chkShoppingCart.Items.Clear()
        For Each iI In lItem
            chkShoppingCart.Items.Add(iI)
        Next
        lblError.Text = ""
    End Sub

    Protected Sub imgUserAdd_Role_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgUserAdd_Role.Click

        Try

            lblError.Text = ""
            Dim iCnt As Integer
            If txtUserToAdd.Text.Trim = "" Then
                lblError.Text = "Blank is not allowed, please enter only CWID."
                Exit Sub
            End If

            Dim lStringValidator = New StringValidator
            txtUserToAdd.Text = txtUserToAdd.Text.Replace(",", ";").ToUpper
            Dim lArrUser As Array = txtUserToAdd.Text.Split(";")
            Dim lError As Boolean = False

            For i As Integer = 0 To lArrUser.GetUpperBound(0)

                If lStringValidator.VaidateChars(lArrUser(i).ToString.Trim) Then
                    iCnt = CheckGumsCwIDExists(lArrUser(i).ToString.Trim)
                    If iCnt = 0 Then
                        lblError.Text = "Entered CW_ID is not valid, please enter valid CW_ID !!"
                    Else
                        Dim lstItem As ListItem = lstUsersToCheck.Items.FindByValue(lArrUser(i).ToString.Trim)
                        If lstItem Is Nothing And Not lArrUser(i).ToString = "" Then
                            lstUsersToCheck.Items.Add(lArrUser(i).ToString.Trim)
                            lError = False
                            txtUserToAdd.Text = txtUserToAdd.Text.Replace(lArrUser(i).ToString.Trim, "")
                        End If
                    End If
                Else
                    lblError.Text = lStringValidator.NotAllowedKeys & " is not allowed !"
                    lError = True
                End If
            Next

        Catch ex As Exception
            ' Reference         : ZHHR 1053139 - GBOX Webforms: MDG Connection error handling
            ' Comment           : Connection error handling
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 09-Feb-2016
            'lblError.Text = ex.Message
            lblError.Text = "Connection Error, please try later."
            ' Reference End     : ZHHR 1053139
        End Try

    End Sub

    Protected Sub imgUserDelete_Role_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgUserDelete_Role.Click

        lstUsersToCheck.Items.Remove(lstUsersToCheck.SelectedItem)

    End Sub

    Private Function CheckGumsCwIDExists(ByVal lCw_Id As String) As Integer

        If mGums Is Nothing Then mGums = New myGumsManager
        '--------------------------------------------------------------------
        'Reference  : ZHHR 1047321 - GBOX: change connection to CWID database
        'Comment    : Get the new GUMS DB connection sting from database
        'Added by   : Pratyusa Lenka (CWID : EOJCG)
        'Date       : 2015-08-17
        'Dim lDatabase As String = "GUMSCWIDDB"
        'Dim lServerName As String = "BY-GDCSQL056\GDC056,1450"
        'Dim lSQLCnnString As String = "Data Source=" & lServerName & ";Initial Catalog=" & lDatabase & ";Persist Security Info=FALSE;User ID='MYALB';Password='Current$MYALB$REQUEST'"
        Dim objConn As DatabaseConnection = New DatabaseConnection
        mGums.cnnString = objConn.GetConnectionString(pConstGUMSDatabase)
        ' Reference END : CR ZHHR 1047321
        '--------------------------------------------------------------------
        Dim lGumsData As DataTable = mGums.MakeDataTable("Select top(1) * from pub.Users5 where USer_ID ='" & lCw_Id.ToUpper & "' ")

        Return lGumsData.Rows.Count

    End Function

    Protected Sub ImgShowRoles_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgShowRoles.Click
        ' Reference         : ZHHR 1053139 - GBOX Webforms: MDG Connection error handling
        ' Comment           : Connection error handling
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 09-Feb-2016

        Try

            ' Reference         : ADHOC change - MDG Request duplication changes
            ' Comment           : MDG duplicate records.
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 10-Dec-2015
            ShowRoles()
            ' Reference End     : ADHOC change

        Catch ex As Exception
            lblError.Text = "Connection Error, please try later."
        End Try
        ' Reference End    : ZHHR 1053139
    End Sub
    '---------------------------------------------------------------------------------------------------
    ' Reference         : ADHOC change - MDG Request duplication changes
    ' Comment           : MDG duplicate records.
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 10-Dec-2015
    Private Sub ShowRoles()
        Try
            lblError.Text = ""
            Dim iCnt As Integer
            iCnt = 0
            'Dim lSql As String = "Select AGR_DEFINE_ID,Description, AGR_DEFINE_ID +' (' + Description + ')'  as AGR_DEFINE_VIEW from [AGR_DEFINE] where [AGR_TYPE_ID] = 'CAG_MDG' And ([AREA_ID] in ('"

            Dim lSql As String
            lSql = ""
            'lSql = "Select distinct A.AGR_DEFINE_ID,A.Description, A.AGR_DEFINE_ID +' (' + A.Description + ')'  as AGR_DEFINE_VIEW from [AGR_DEFINE] A where A.[AGR_TYPE_ID] = 'CAG_MDG' And (A.[AREA_ID] in ('"
            '---------------------------------------------------------------------------------------------------
            ' Reference         : ZHHR 1045686 - GBOX WEBFORMS OTT 1046: Rename MARTY tables
            ' Comment           : Added prefix Marty_ to marty application tables.
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 22-Jul-2015
            For Each lItem As ListItem In chkArea.Items
                If lItem.Selected = True Then
                    'lSql = " Select distinct B.GROUP_AGR_DEFINE_ID, A.AGR_DEFINE_ID,A.Description, A.AGR_DEFINE_ID +' (' + A.Description + ')'  as AGR_DEFINE_VIEW from [AGR_DEFINE] A "
                    '---------------------------------------------------------------------------------------------------
                    ' Reference         : ZHHR 1050010 - GBOX WebForms OTT 1670: GBOX MDG form
                    ' Comment           : Commented below Sql and writted new sql for listing no assigned roles.
                    ' Added by          : Milind Randive (CWID : EOJCH)
                    ' Date              : 12-Nov-2015
                    'lSql = " Select distinct B.GROUP_AGR_DEFINE_ID,C.Description, B.GROUP_AGR_DEFINE_ID +' (' + C.Description + ')'  as AGR_DEFINE_VIEW FROM MARTY_AGR_AGRS B " & _
                    '       " INNER JOIN  MARTY_AGR_OBJECTS A " & _
                    '       " ON A.AGR_DEFINE_ID = B.SINGLE_AGR_DEFINE_ID " & _
                    '       " INNER JOIN [MARTY_AGR_DEFINE] C " & _
                    '       " ON C.AGR_DEFINE_ID = B.SINGLE_AGR_DEFINE_ID " & _
                    '       " INNER JOIN MARTY_AUTH_OBJECTS D " & _
                    '       " ON A.AGR_TYPE_ID = D.AGR_TYPE_ID "


                    lSql = " Select distinct B.GROUP_AGR_DEFINE_ID,E.Description, B.GROUP_AGR_DEFINE_ID +' (' + E.Description + ')'  as AGR_DEFINE_VIEW FROM [MARTY_AGR_DEFINE] C  " & _
                           " INNER JOIN MARTY_AGR_AGRS B  ON C.AGR_DEFINE_ID = B.SINGLE_AGR_DEFINE_ID  " & _
                           " LEFT JOIN  MARTY_AGR_OBJECTS A  ON A.AGR_DEFINE_ID = B.SINGLE_AGR_DEFINE_ID  " & _
                           " LEFT JOIN MARTY_AUTH_OBJECTS D  ON A.AGR_TYPE_ID = D.AGR_TYPE_ID " & _
                           " INNER JOIN MARTY_AGR_DEFINE E ON B.group_agr_define_id = E.agr_define_id "

                    ' Reference         : ZHHR 1050010
                    Exit For
                End If
            Next
            ' Reference End         : ZHHR 1045686

            For Each lItem As ListItem In RdbRequester.Items
                If lItem.Selected = True Then
                    'lSql &= " INNER JOIN  MARTY_AGR_OBJECTS C " & _
                    '        " ON C.AGR_DEFINE_ID  = B.SINGLE_AGR_DEFINE_ID " & _
                    '        " INNER JOIN MARTY_AGR_OBJECTS D " & _
                    '        " ON D.AGR_DEFINE_ID = C.AGR_DEFINE_ID " & _
                    '        " INNER JOIN MARTY_AUTH_OBJECTS E " & _
                    '        " ON D.AGR_TYPE_ID = E.AGR_TYPE_ID "
                    Exit For
                End If
            Next

            'lSql &= " where A.[AGR_TYPE_ID] IN ('CAG_MDG','CR_MDG','DE_MDG') And (A.[AREA_ID] in ('"
            lSql &= " where C.[AGR_TYPE_ID] IN ('CAG_MDG','CR_MDG','DE_MDG','MM_MDG') And (C.[AREA_ID] in ('"

            For Each lItem As ListItem In chkArea.Items
                If lItem.Selected = True Then
                    lSql &= lItem.Value & "','"
                End If
            Next
            If Right(lSql, 6) = "' or '" Then
                lSql = Left(lSql, Len(lSql) - 5)
            End If
            lSql &= "'))"

            iCnt = 0
            For Each lItem As ListItem In ChkApprover.Items
                If lItem.Selected = True Then
                    iCnt += 1
                    If iCnt = 1 Then
                        lSql &= " and ("
                    End If
                    'lSql &= " A.AGR_DEFINE_ID like '%" & lItem.Value & "%' or "
                    If (lItem.Value = "Approver") Then
                        lSql &= " C.AGR_DEFINE_ID like '%_APP%' or C.AGR_DEFINE_ID like '%_APP1%' or C.AGR_DEFINE_ID like '%_APPF%' or C.AGR_DEFINE_ID like '%_APPB%' "
                    End If

                    If lItem.Value = "Requester" Then
                        If (ChkApprover.Items(0).Selected) Then
                            lSql &= " or "
                        End If
                        lSql &= " C.AGR_DEFINE_ID like '%_REQ%' "
                    End If

                End If
            Next
            If Right(lSql, 4) = " or " Then
                lSql = Left(lSql, Len(lSql) - 4)
            End If
            If iCnt >= 1 Then
                lSql &= ")"
            End If

            iCnt = 0
            For Each lItem As ListItem In RdbRequester.Items
                If lItem.Selected = True Then
                    iCnt += 1
                    'If iCnt = 1 Then

                    If lItem.Value.Trim = "LE" Then
                        If txtboxLE.Text.Trim <> "" Then
                            lSql &= " AND  D.AUTH_OBJ_TEXT IN ('BUKRS') AND A.AGR_OBJECTS_VALUE = '" & txtboxLE.Text & "'"
                        Else
                            lSql &= " AND  D.AUTH_OBJ_TEXT IN ('BUKRS') "
                        End If
                        iCnt += 1
                    End If

                    If lItem.Value.Trim = "BEGRU" Then
                        If txtBegru.Text.Trim <> "" Then
                            lSql &= " AND D.AUTH_OBJ_TEXT IN ('BRGRU') AND A.AGR_OBJECTS_VALUE = '" & txtBegru.Text & "'"
                        Else
                            lSql &= " AND D.AUTH_OBJ_TEXT IN ('BRGRU') "
                        End If
                        iCnt += 1
                    End If
                    If lItem.Value.Trim = "HUB" Then
                        If txtHub.Text.Trim <> "" Then
                            lSql &= " AND ( D.AUTH_OBJ_TEXT IN ('BRGRU') AND A.AGR_OBJECTS_VALUE LIKE ('H0%') AND A.AGR_OBJECTS_VALUE = '" & txtHub.Text & "' )"
                        Else
                            lSql &= " AND  ( D.AUTH_OBJ_TEXT IN ('BRGRU') AND A.AGR_OBJECTS_VALUE LIKE ('H0%') )"
                        End If
                        iCnt += 1
                    End If
                    '-----------------------------------------------------------------------
                    ' Reference : OTT 2235 ZHHR 1058579 - GBOX MDG form: add search criteria
                    ' Comment   : Consider EKORG and VKORG as search criteria
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2016-06-24
                    If lItem.Value.Trim = "EKORG" Then
                        If txtEKORG.Text.Trim <> "" Then
                            lSql &= " AND A.AGR_TYPE_ID LIKE '%MDG%' AND A.AUTH_OBJ_TEXT IN ('EKORG') AND A.AGR_OBJECTS_VALUE = '" & txtEKORG.Text.Trim & "'"
                        Else
                            lSql &= " AND A.AGR_TYPE_ID LIKE '%MDG%' AND A.AUTH_OBJ_TEXT IN ('EKORG') "
                        End If
                        iCnt += 1
                    End If
                    If lItem.Value.Trim = "VKORG" Then
                        If txtVKORG.Text.Trim <> "" Then
                            lSql &= " AND A.AGR_TYPE_ID LIKE '%MDG%' AND A.AUTH_OBJ_TEXT IN ('VKORG') AND A.AGR_OBJECTS_VALUE = '" & txtVKORG.Text.Trim & "'"
                        Else
                            lSql &= " AND A.AGR_TYPE_ID LIKE '%MDG%' AND A.AUTH_OBJ_TEXT IN ('VKORG') "
                        End If
                        iCnt += 1
                    End If
                    If lItem.Value.Trim = "WERKS" Then
                        If txtVKORG.Text.Trim <> "" Then
                            lSql &= " AND A.AGR_TYPE_ID LIKE '%MDG%' AND A.AUTH_OBJ_TEXT IN ('WERKS') AND A.AGR_OBJECTS_VALUE = '" & txtWERKS.Text.Trim & "'"
                        Else
                            lSql &= " AND A.AGR_TYPE_ID LIKE '%MDG%' AND A.AUTH_OBJ_TEXT IN ('WERKS') "
                        End If
                        iCnt += 1
                    End If
                    ' Reference END : OTT 2235 ZHHR 1058579
                    '-----------------------------------------------------------------------
                    'lSql &= " and ("
                    'End If
                    'lSql &= " [AUTH_OBJ_TEXT] like '%" & lItem.Text & "%' or "
                End If
            Next
            'If Right(lSql, 4) = " or " Then
            '    lSql = Left(lSql, Len(lSql) - 4)
            'End If
            'If iCnt >= 1 Then
            '    lSql &= ")"
            'End If

            Dim mdt As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            If mdt Is Nothing Then
                imgSubmitRequest.Enabled = False
                pnlView.Visible = False
                lblSearchResult.Text = "Search result: "
                Exit Sub
            Else
                imgSubmitRequest.Enabled = True
                pnlView.Visible = True
                lblSearchResult.Text = "Search result: " + mdt.Rows.Count.ToString
            End If
            chkBoxAgr.DataSource = mdt
            chkBoxAgr.DataBind()

        Catch ex As Exception
            lblDatabase.Text = ex.Message
        End Try
    End Sub
    ' Reference End        : ADHOC change

    Private Function GetItServiceUrl(ByVal lrSME As String) As String
        Dim lStr As String = ""
        Dim idt As DataTable = mUser.Databasemanager.MakeDataTable("SELECT * FROM MDRS_INCIDENT_INFO WHERE APPLICATION_ID ='MDG'")
        Dim iRow, iCol As Int16

        iRow = 0
        iCol = 0
        For Each r As DataRow In idt.Rows
            lStr += Trim(idt.Rows(iRow)(iCol + 1)) + "=" + SanitizeURLString(Trim(idt.Rows(iRow)(iCol + 2))) + "&"
            iRow += 1
        Next r

        lStr = "https://itserviceportal.intranet.cnb/spo/faces/bay/call/create?" + lStr + "DESCRIPTION=" + URLEncoder(lrSME)

        Return lStr
    End Function
    Protected Function SanitizeURLString(ByVal RawURLParameter As String) As String

        Dim Results As String

        Results = RawURLParameter

        Results = Results.Replace("%", "%25")
        Results = Results.Replace("<", "%3C")
        Results = Results.Replace(">", "%3E")
        Results = Results.Replace("#", "%23")
        Results = Results.Replace("{", "%7B")
        Results = Results.Replace("}", "%7D")
        Results = Results.Replace("|", "%7C")
        Results = Results.Replace("\", "%5C")
        Results = Results.Replace("^", "%5E")
        Results = Results.Replace("~", "%7E")
        Results = Results.Replace("[", "%5B")
        Results = Results.Replace("]", "%5D")
        Results = Results.Replace("`", "%60")
        Results = Results.Replace(";", "%3B")
        Results = Results.Replace("/", "%2F")
        Results = Results.Replace("?", "%3F")
        Results = Results.Replace(":", "%3A")
        Results = Results.Replace("@", "%40")
        Results = Results.Replace("=", "%3D")
        Results = Results.Replace("&", "%26")
        Results = Results.Replace("$", "%24")

        Return Results

    End Function
    Function URLEncoder(ByVal lStr As String) As String
        Dim i As Integer
        Dim larg As String

        larg = lStr
        ' *** First replace '%' chr
        larg = Replace(larg, "%", Chr(1))
        ' *** then '+' chr
        larg = Replace(larg, "+", Chr(2))

        For i = 0 To 255
            Select Case i
                ' *** Allowed 'regular' characters
                Case 37, 43, 48 To 57, 65 To 90, 97 To 122
                Case 1  ' *** Replace original %
                    larg = Replace(larg, Chr(i), "%25")
                Case 2  ' *** Replace original +
                    larg = Replace(larg, Chr(i), "%2B")
                Case 32
                    larg = Replace(larg, Chr(i), "+")
                Case 3 To 15
                    larg = Replace(larg, Chr(i), "%0" & Hex(i))
                Case Else
                    larg = Replace(larg, Chr(i), "%" & Hex(i))

            End Select
        Next

        Return larg

    End Function

    '---------------------------------------------------------------------------------------------------
    ' Reference         : ZHHR 1049322 - GBOX Webforms OTT 1048: New Workflow for MDG
    ' Comment           : New function added for Filter settings workflow
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 28-Oct-2015
    Private Function wf_MakePackage(ByVal Obj_ID As String, ByVal mId As String, ByVal mUser As myUser, ByVal mText As String) As List(Of String)
        Dim lPack As New List(Of String)
        Dim lSql As String = ""
        Dim MartyId As String = mId
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

            If (rw("ORG_LEVEL_ID") = "SUBGROUP") And rw("STATION_ID") = "SME" Then

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

        Next

        For Each r As DataRow In dt.Rows

            If (r("STATION_ID") = "SME") Then

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
        lSql &= "   ,'" & "" & "'"
        lSql &= "   ,'" & "" & "'"
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
                            lSql &= "   'New workflow item in GBOX MDG: " & MartyId & "',"
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
