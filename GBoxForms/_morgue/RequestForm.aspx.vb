'Option Strict Off
'Imports System.Web
'Imports System
'Imports System.Data.OleDb
'Imports System.Net
'Imports System.Net.Mail
'Imports System.Text.RegularExpressions
'Imports System.IO
'Imports System.Net.Mime
'Imports System.Data.SqlClient
'Imports System.Drawing.Imaging

'Public Class RequestForm
'    Inherits System.Web.UI.Page
'    Private mUser As myUser
'    Private mErrText As String = ""
'    Private WithEvents mDynamicFormController As Dynamic_View_Controller
'    Private WithEvents mStaticViewController As MyDynamicForm_StaticViewController
'    Private mFrom As String
'    Private mTo As String
'    Private mCC As String
'    Private mMsg As String
'    Private mSubject As String
'    Private mMailServer As String
'    Private mPort As Integer
'    Private mType As String
'    Private mSubtype As String

'    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
'        Userchack()
'        lblInformations.Text = "Welcome " & mUser.first_name & mUser.last_name
'        If rblProblemType.Items.Count < 1 Then
'            LoadProblemType()
'        End If
'    End Sub
'    Private Sub Userchack()
'        If pCurrentUsers Is Nothing Then pCurrentUsers = New myUsers
'        pCurrentUsers.CONTEXT = Context
'        If mUser Is Nothing Then
'            mUser = pCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
'        End If
'    End Sub

'    Private Function LoadProblemType()
'        Try
'            ' Clear RadioButtonList
'            rblProblemType.Items.Clear()
'            ' Get SQL data 
'            Dim lSql As String
'            Dim lFormtype As String = "REQUEST_MDAS"
'            Dim dt As DataTable
'            lSql = "Select ITEM_ID, FORMTYPE, PROBLEMTYPE, DESCRIPTION"
'            lSql &= " FROM FORM_ITEMS "
'            lSql &= " WHERE FORMTYPE = '" & lFormtype & "'  Order by PROBLEMTYPE"
'            dt = mUser.Databasemanager.MakeDataTable(lSql)
'            'RadioButtonList
'            With Me.rblProblemType
'                .DataSource = dt
'                .DataTextField = "PROBLEMTYPE"
'                .DataValueField = "PROBLEMTYPE"
'                .DataBind()
'            End With
'            Return True
'        Catch ex As Exception
'            MsgBox("Clear form exception: " & ex.Message)
'            Return False
'        End Try
'    End Function

'    Protected Sub btnSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSend.Click
'        SelectedItem()
'        SelectedSubitem()
'        'send message
'        mFrom = Trim(mUser.SMTP_EMAIL)
'        mTo = Trim("BBS-MDRS-Support@bayer.com")
'        'mTo = Trim("marco.cavoto@bayer.com")
'        mCC = Trim(txtCc.Text)
'        If InStr(mUser.Databasemanager.cnnString, "MDRS_Q") = 0 And InStr(mUser.Databasemanager.cnnString, "MDRS_D") = 0 Then
'            mSubject = "" & Trim(txtReqShorttext.Text)
'        Else
'            mSubject = "TEST REQUEST" & Trim(txtReqShorttext.Text)
'        End If
'        mMsg = Trim(txtReqLongtext.Text)
'        mMailServer = "by-smtp"
'        mPort = "25"
'        Try
'            'check subject
'            If mSubject = "" Or mSubject = String.Empty Then
'                lblRed.Text = "Sorry, but message could not be sent. The subject text box is empty."
'                Exit Sub
'            End If
'            'check message
'            If mMsg = "" Or mMsg = String.Empty Then
'                lblRed.Text = "Sorry, but message could not be sent. The body text box is empty."
'                Exit Sub
'            End If
'            'check problem type
'            If mType <> "" Or mType <> String.Empty Then
'                If mSubtype <> "" Or mSubtype <> String.Empty Then
'                    mMsg = "Request is for " & Chr(9) & Chr(9) & cboReqFor.SelectedItem.Text & mSubtype & _
'                        Chr(10) & "Problem Type: " & Chr(9) & Chr(9) & mType & " > " & mSubtype & _
'                        Chr(10) & "Select priority: " & Chr(9) & Chr(9) & cboPriority.SelectedValue & Chr(10) & Chr(10) & mMsg
'                Else
'                    lblRed.Text = "Sorry, but message could not be sent. Please select problem type."
'                    Exit Sub
'                End If
'            Else
'                lblRed.Text = "Sorry, but message could not be sent. Please select problem type."
'                Exit Sub
'            End If

'            Dim att As String = 0
'            Dim message As New MailMessage(mFrom, mTo, mSubject, mMsg)

'            'check message priority
'            'message.Priority = MailPriority.Normal
'            Select Case cboPriority.SelectedValue
'                Case "Low"
'                    message.Priority = MailPriority.Low
'                Case "Normal"
'                    message.Priority = MailPriority.Normal
'                Case "High"
'                    message.Priority = MailPriority.High
'            End Select
'            'check email copy as cc
'            If mCC <> "" Or mCC <> String.Empty Then
'                Dim strCC() As String = mCC.Split(";")
'                For Each strThisCC In strCC
'                    If strThisCC <> "" Then
'                        strThisCC = strThisCC.Trim
'                        Dim pattern As String = "^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"
'                        Dim emailAddressMatch As Match = Regex.Match(strThisCC, pattern)
'                        If emailAddressMatch.Success = False Then
'                            lblRed.Text = "Sorry, but your e-mail " & mCC & " is not valid. Please insert a full e-mail adress like 'your.name@emailadress.com' or check your email again."
'                            Exit Sub
'                        Else
'                            message.CC.Add(Trim(strThisCC))
'                        End If
'                    End If
'                Next
'            End If
'            Dim lNach As String = ""
'            'CheckBox attachments
'            If fileAttachments.HasFile Then
'                With fileAttachments
'                    lblRed.Text = Trim(.PostedFile.FileName.ToString)
'                    Dim lFilePathAndName As Array = .PostedFile.FileName.ToString.Split("\")
'                    Dim lFilename As String = lFilePathAndName(lFilePathAndName.GetUpperBound(0))
'                    Dim lPre As String = Now.Hour & "_" & Now.Minute & "_" & Now.Second & "_"
'                    lFilename = lPre & lFilename
'                    Dim lWas As String = lFilename
'                    Dim lVon As String = Trim(.PostedFile.FileName.ToString)
'                    lNach = Me.Server.MapPath("~/cache/" & lWas)
'                    .SaveAs(lNach)
'                    Dim attached As New Attachment(lNach)
'                    message.Attachments.Add(attached)
'                    att += 1
'                End With
'            End If
'            If fileAttachment2.HasFile Then
'                With fileAttachment2
'                    lblRed.Text = Trim(.PostedFile.FileName.ToString)
'                    Dim lFilePathAndName As Array = .PostedFile.FileName.ToString.Split("\")
'                    Dim lFilename As String = lFilePathAndName(lFilePathAndName.GetUpperBound(0))
'                    Dim lPre As String = Now.Hour & "_" & Now.Minute & "_" & Now.Second & "_"
'                    lFilename = lPre & lFilename
'                    Dim lWas As String = lFilename
'                    Dim lVon As String = Trim(.PostedFile.FileName.ToString)
'                    lNach = Me.Server.MapPath("~/cache/" & lWas)
'                    .SaveAs(lNach)
'                    Dim attached As New Attachment(lNach)
'                    message.Attachments.Add(attached)
'                    att += 1
'                End With
'            End If
'            If fileAttachment3.HasFile Then
'                With fileAttachment3
'                    lblRed.Text = Trim(.PostedFile.FileName.ToString)
'                    Dim lFilePathAndName As Array = .PostedFile.FileName.ToString.Split("\")
'                    Dim lFilename As String = lFilePathAndName(lFilePathAndName.GetUpperBound(0))
'                    Dim lPre As String = Now.Hour & "_" & Now.Minute & "_" & Now.Second & "_"
'                    lFilename = lPre & lFilename
'                    Dim lWas As String = lFilename
'                    Dim lVon As String = Trim(.PostedFile.FileName.ToString)
'                    lNach = Me.Server.MapPath("~/cache/" & lWas)
'                    .SaveAs(lNach)
'                    Dim attached As New Attachment(lNach)
'                    message.Attachments.Add(attached)
'                    att += 1
'                End With
'            End If
'            If fileAttachment4.HasFile Then
'                With fileAttachment4
'                    lblRed.Text = Trim(.PostedFile.FileName.ToString)
'                    Dim lFilePathAndName As Array = .PostedFile.FileName.ToString.Split("\")
'                    Dim lFilename As String = lFilePathAndName(lFilePathAndName.GetUpperBound(0))
'                    Dim lPre As String = Now.Hour & "_" & Now.Minute & "_" & Now.Second & "_"
'                    lFilename = lPre & lFilename
'                    Dim lWas As String = lFilename
'                    Dim lVon As String = Trim(.PostedFile.FileName.ToString)
'                    lNach = Me.Server.MapPath("~/cache/" & lWas)
'                    .SaveAs(lNach)
'                    Dim attached As New Attachment(lNach)
'                    message.Attachments.Add(attached)
'                    att += 1
'                End With
'            End If
'            If fileAttachment5.HasFile Then
'                With fileAttachment5
'                    lblRed.Text = Trim(.PostedFile.FileName.ToString)
'                    Dim lFilePathAndName As Array = .PostedFile.FileName.ToString.Split("\")
'                    Dim lFilename As String = lFilePathAndName(lFilePathAndName.GetUpperBound(0))
'                    Dim lPre As String = Now.Hour & "_" & Now.Minute & "_" & Now.Second & "_"
'                    lFilename = lPre & lFilename
'                    Dim lWas As String = lFilename
'                    Dim lVon As String = Trim(.PostedFile.FileName.ToString)
'                    lNach = Me.Server.MapPath("~/cache/" & lWas)
'                    .SaveAs(lNach)
'                    Dim attached As New Attachment(lNach)
'                    message.Attachments.Add(attached)
'                    att += 1
'                End With
'            End If
'            'Continue on smtp
'            Dim mySmtpClient As New SmtpClient(mMailServer, mPort)
'            mySmtpClient.UseDefaultCredentials = True
'            mySmtpClient.Send(message)
'            message.Attachments.Clear()
'            message.Attachments.Dispose()
'            message.Dispose()
'            'information in message Label
'            lblGreen.Text = "The mail message has been sent to GPSC. " & att & " files has been attached."
'            lblRed.Text = lNach
'            ClearForm()
'        Catch ex As FormatException
'            lblRed.Text = ex.Message
'        Catch ex As SmtpException
'            lblRed.Text = ex.Message
'        Catch ex As Exception
'            lblRed.Text = ex.Message
'        End Try
'    End Sub

'    Protected Sub btnReset_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnReset.Click
'        ClearForm()
'    End Sub

'    Private Function ClearForm() As Boolean
'        Try
'            'txtTo.Text = Nothing
'            lblRed.Text = Nothing
'            txtCc.Text = Nothing
'            cboReqFor.SelectedItem.Selected = 0
'            txtReqShorttext.Text = Nothing
'            txtReqLongtext.Text = Nothing
'            rblProblemType.Items.Clear()
'            rblSubType.Items.Clear()
'            rblProblemType.ForeColor = Drawing.Color.Black
'            LoadProblemType()
'            Return True
'        Catch ex As Exception
'            MsgBox("Clear form exception: " & ex.Message)
'            Return False
'        End Try
'    End Function

'    ''' <summary>
'    ''' Selection of items and subitems. the data we get from database
'    ''' </summary>
'    ''' <returns>mType and mSubtype</returns>
'    ''' <remarks></remarks>
'    Private Function SelectedItem() As Boolean
'        Dim t As Boolean
'        Dim i As Long = 0
'        ' Add selected item to mType
'        For Each lst As ListItem In rblProblemType.Items
'            t = rblProblemType.Items(i).Selected
'            If t = True Then
'                mType = rblProblemType.Items(i).ToString
'            End If
'            If i < rblProblemType.Items.Count Then
'                i += 1
'            Else
'                Exit For
'            End If
'        Next
'    End Function

'    Private Function SelectedSubitem() As Boolean
'        Dim t As Boolean
'        Dim i As Long = 0
'        ' Add selected item to mType
'        For Each lst As ListItem In rblSubType.Items
'            t = rblSubType.Items(i).Selected
'            If t = True Then
'                mSubtype = rblSubType.Items(i).ToString
'            End If
'            If i < rblSubType.Items.Count Then
'                i += 1
'            Else
'                Exit For
'            End If
'        Next
'    End Function

'    Protected Sub rblProblemType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rblProblemType.SelectedIndexChanged
'        Try
'            SelectedItem()
'            If mType <> "" Or mType <> String.Empty Then
'                rblProblemType.ForeColor = Drawing.Color.LightGray
'            End If
'            Dim lSql As String
'            Dim dt As DataTable
'            ' Clear RadioButtonList
'            rblSubType.Items.Clear()
'            ' Get SQL data 
'            lSql = "SELECT FORM_SUBITEMS.SUB_ID, FORM_SUBITEMS.SUBTYPE, FORM_ITEMS.PROBLEMTYPE "
'            lSql &= " FROM FORM_SUBITEMS, FORM_ITEMS "
'            lSql &= " WHERE FORM_SUBITEMS.ITEM_ID = FORM_ITEMS.ITEM_ID "
'            lSql &= " AND FORM_ITEMS.PROBLEMTYPE ='" & mType & "' "
'            dt = mUser.Databasemanager.MakeDataTable(lSql)
'            'Add text and value to RadioButtonList
'            With Me.rblSubType
'                .DataSource = dt
'                .DataTextField = "SUBTYPE"
'                .DataValueField = "SUBTYPE"
'                .DataBind()
'            End With
'        Catch ex As Exception
'            MsgBox("Clear form exception: " & ex.Message)
'        End Try
'    End Sub

'    Private mGums As mySQLDBMANAGER


'    Protected Sub imgCheckMail_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgCheckMail.Click
'        txtCc.Text = txtCc.Text.Replace(",", ";")
'        Dim lTXT As String = ""
'        Dim lArr As Array = txtCc.Text.Split(";")

'        For i = 0 To lArr.GetUpperBound(0)
'            If InStr(lArr(i), "@") = 0 And InStr(lArr(i), "is no valid cwid") = 0 Then
'                If lArr(i).ToString.Trim <> "" Then
'                    lTXT = lTXT & mUser.GBOXmanager.GetEmailByCwId(lArr(i).ToString.Trim) & ";"
'                Else
'                    lTXT = lTXT & lArr(i).ToString.Trim & ";"
'                End If
'            Else
'                lTXT = lTXT & lArr(i).ToString.Trim & ";"
'            End If
'        Next
'        txtCc.Text = lTXT.Replace(";;", ";")
'    End Sub

'    Protected Sub Menu2_MenuItemClick(sender As Object, e As System.Web.UI.WebControls.MenuEventArgs) Handles Menu2.MenuItemClick

'    End Sub
'End Class
