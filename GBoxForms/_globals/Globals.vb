Module Globals
#Region "Public Variable "
    Public pObjCurrentUsers As myUsers
    Public pObjRequestedUsers As List(Of myRequestedUser)
    Public pVarSQL As String = ""
    Public pVarIsTree As Boolean
    Public pVarErrMsg As String = ""
#End Region
#Region "Public Const"
    Public Const pVarConstWith As Long = 300

    Public Const pConstServername As String = "by-gbox.bayer-ag.com"
    'Public Const pConstServername As String = "localhost:22611"

    Public Const mConstVersion As String = "2.1.0"
    Public Const pConstSqlServerDatabase As String = "MDRS"
    'Public Const pConstSqlDatabase As String = "PBARDO" '
    Public Const pConstGUMSDatabase As String = "GUMSCWIDDB"
    'Public Const PublishingFolder As String = "\\sharedwebcontent.de.bayer.cnb\by-swp101019_by-gbox.bayer-ag.com"
#End Region
#Region "Public Functions"

    Public Function pGetErrorMsg() As String
        ' ----------------------------------------------------------------------------------------
        ' Reference  : BY-RZ04 - CMT - 28633 - 110IM08222136,110IM08175309 - "Unknown User error in accessing GBOX"
        ' Comment    : Add Error information to user if he is not able able to login to GBox site
        ' Created by : EQIZU
        ' Date       : 06-NOV-2011
        ' --------------------------------------------------------------------------------------- 
        Dim lMsg As String = ""
        lMsg = "You have lost your connection to G|Box server, " & vbCrLf
        lMsg = lMsg & "please copy the following error message and submit it to our hotline or " & vbCrLf
        lMsg = lMsg & "contact your local IT support to address local network dropouts. " & vbCrLf
        lMsg = lMsg & "------------------------------------------------" & vbCrLf
        lMsg = lMsg & "To retry close your local internet explorer browser, start a new instance and retry. " & vbCrLf
        lMsg = lMsg & "Error Message: Active directory not available -  windows user authentication not possible." & vbCrLf
        Return lMsg
    End Function
#End Region
End Module
