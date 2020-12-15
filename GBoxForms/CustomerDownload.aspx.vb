' ------------------------------------------------------------------------
' DESCRIPTION
' ------------------------------------------------------------------------
'
' CustomerDownload.aspx.vb
'
' Page with download request functionality
' ------------------------------------------------------------------------
' Created by: Surendra Purav (EQIZU)
' Version   : 1.0
' Comment   : Added functinality to Download Request in XML format
' ------------------------------------------------------------------------
'Revision History: 
' -------------------------------------------------------------------------------------------------
' Updated by        :EQIZU 
' Date              :2013-11-14
' Version           :1.0 
' Comment           :BY-RZ04-CMT-27932 - Enhance GBOX with a database parametered download website
'---------------------------------------------------------------------------------------------------
' Updated by        :EQIZU 
' Date              :2014-03-12
' Version           :1.1 
' Reference         : CR-1024616-110IM08705465-Gbox Web Portal: Enhance DownLoad.aspx with Bardo ACCESS
' Comment           : To retrieve the details for dropdownlist and query for report
' --------------------------------------------------------------------------------------------------

Public Class CustomerDownload
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Authenticate the User
        Authenticate()
        If mUser Is Nothing Then
            Exit Sub
        Else
            mUser.GBOXmanager.User = mUser
        End If
        mUser.GBOXmanager.LinkServer = pConstServername
        'Check if any Database maintenance activity is going on
        If mUser.GBOXmanager.CHECK_MDRS_DATABASE_MAINTENANCE() Then
            Dim lText As String = "G|Box System Access  is currently locked due to maintenance "
            lText = lText & vbCrLf & mUser.GBOXmanager.Get_MDRS_DATABASE_MAINTENANCE_STRING
            lblErr.Text = lText
            mErrText &= ""
            Exit Sub
        End If

        If Not IsPostBack Then
            'Get the details for download request list
            FillCombo(cboReqFor0)
            btnDownload.Enabled = False
        End If
    End Sub


#Region "Declare Private Vars"

    Private mUser As myUser
    Private mErrText As String = "Due to a technical problem your GBOX request could not be posted. Please send the complete error message to our hotline at </HOTLINE/" & vbCrLf
#End Region

#Region "Subs And Functions - PAGE LOAD"
    Private Sub Authenticate()
        Dim mAuthenticate_User As New Authorization
        mAuthenticate_User.Context = Context
        mUser = mAuthenticate_User.Authenticate_User()
        If mUser Is Nothing Then
            lblErr.Text = mAuthenticate_User.ErrText
        Else
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035817 - GBOX WebForms: Add New Function for updating columns
            ' Comment           : Added code for updating new columns in User table
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-12-12
            mUser.UserAccessStatus(mUser.CW_ID, "GBOX CUST DOWNLOAD")
            ' Reference  END    : CR ZHHR 1035817
        End If

        mAuthenticate_User = Nothing
    End Sub

    Private Function FillCombo(ByRef lcmb As DropDownList) As Integer
        Dim dt As DataTable = Nothing
        Dim lSql As String = ""

        lSql = GetQuery("") 'No filter criteria

        dt = mUser.Databasemanager.MakeDataTable(lSql)
        With lcmb
            .DataSource = dt
            .DataTextField = dt.Columns("TITLE").ToString 'CR-1024616
            .DataValueField = dt.Columns("DETAIL").ToString
            .DataBind()
        End With
        If dt.Rows.Count > 0 Then
            lcmb.Items.Insert(0, "Please Select ...")
        End If
        Return dt.Rows.Count
    End Function

    Private Function GetQuery(ByVal lFilterCriteria As String) As String

        ' ----------------------------------------------------------------------------------------
        ' Reference  : CR-1024616-110IM08705465-Gbox Web Portal: Enhance DownLoad.aspx with Bardo ACCESS
        ' Comment    : To retrieve the details for dropdownlist and query for report
        ' Created by : EQIZU
        ' Date       : 12-MAR-2014
        ' ---------------------------------------------------------------------------------------

        Dim lSql As String = ""

        lSql = "SELECT TITLE,SOURCE,[DESCRIPTION] + '|' + ISNULL([LONG_DESCRIPTION],'') AS DETAIL "
        lSql = lSql & "FROM dp_Additional_Information "
        lSql = lSql & "WHERE ACTIVE = 1 and [dp_Additional_Information_Classification_ID]='Download' "

        If Not String.IsNullOrEmpty(lFilterCriteria) Then
            lSql = "SELECT SOURCE "
            lSql = lSql & "FROM dp_Additional_Information "
            lSql = lSql & "WHERE ACTIVE = 1 and [dp_Additional_Information_Classification_ID]='Download' "
            lSql = lSql & "and TITLE='" & lFilterCriteria & "' "
        End If

        lSql = lSql & "Order by RANK "

        Return lSql
    End Function




#End Region

#Region "HANDLES FORM CONTROLS EVENTS"
    
    Protected Sub cboReqFor0_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboReqFor0.SelectedIndexChanged
        lblErr.Text = ""
        If cboReqFor0.SelectedIndex = 0 Then
            btnDownload.Enabled = False
            lblDesc.Text = ""
            lblLongDesc.Text = ""
        Else
            ' ----------------------------------------------------------------------------------------
            ' Reference  : CR-1024616-110IM08705465-Gbox Web Portal: Enhance DownLoad.aspx with Bardo ACCESS
            ' Comment    : Description and Long description is dispalyed based on option selected by the user
            ' Created by : EQIZU
            ' Date       : 12-MAR-2014
            ' ---------------------------------------------------------------------------------------
            Dim lDescArray As String = cboReqFor0.SelectedValue.ToString
            If Not String.IsNullOrEmpty(lDescArray) Then
                lblDesc.Text = cboReqFor0.SelectedValue.ToString.Split(CChar("|"))(0)
                lblLongDesc.Text = cboReqFor0.SelectedValue.ToString.Split(CChar("|"))(1)
            End If
            btnDownload.Enabled = True
        End If

    End Sub

    Protected Sub btnDownload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDownload.Click
        'Check for valid User 
        If mUser Is Nothing Then
            Exit Sub
        End If
        'Check if first row from dropdownbox is not selected
        If cboReqFor0.SelectedIndex = 0 Then
            Exit Sub
        End If
        Dim lResponse As Boolean = False
        'File name for report to download
        Dim strFilename As String = "GBOX_" & DateTime.Now.ToLongTimeString.Replace(":", "_") & "_" & cboReqFor0.SelectedItem.Text
        Dim dt As DataTable = Nothing
        Dim lSql As String = ""        
        lSql = GetQuery(cboReqFor0.SelectedItem.Text) 'with filter criteria
        'Get source query for report to download
        dt = mUser.Databasemanager.MakeDataTable(lSql)
        If dt.Rows.Count > 0 Then
            lSql = dt.Rows(0).ItemArray(0).ToString
            lblErr.Text = ""
        Else
            lblErr.Text = "No data found"            
        End If

        If String.IsNullOrEmpty(lblErr.Text) Then            
            'Get the report data
            ' ----------------------------------------------------------------------------------------
            ' Reference  : CR-1024616-110IM08705465-Gbox Web Portal: Enhance DownLoad.aspx with Bardo ACCESS
            ' Comment    : To download the report based on the required report title selected from BARDO Oracle database
            ' Created by : EQIZU
            ' Date       : 19-MAR-2014
            ' ---------------------------------------------------------------------------------------
            Dim lParamStr As String = ""
            Try
                mUser.GBOXmanager.Makereport(Response, strFilename, lSql, Server, lResponse, lParamStr)
                If lParamStr = "OK" Then
                    lblErr.Text = "The GBOXMANAGER Download was succesful."
                Else
                    lblErr.Text = lParamStr                    
                End If
            Catch ex As Exception
                lblErr.Text = "Error: download report" & ex.Message
            End Try
        End If
    End Sub
#End Region



End Class