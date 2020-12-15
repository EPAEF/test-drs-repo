Public Class SearchResult
    Private mNavigateUrl As String = ""
    Private mResultText As String = ""
    Public Property NavigateUrl As String
        Get
            Return mNavigateUrl
        End Get
        Set(ByVal value As String)
            mNavigateUrl = value
        End Set
    End Property
    Public Property ResultText As String
        Get
            Return mResultText
        End Get
        Set(ByVal value As String)
            mResultText = value
        End Set
    End Property
End Class
Public Class mySearchEngine
    Private mContext As HttpContext
    Private mUser As myUser
    Private mLinkserver As String = "by-gbox"
    Private mRequest As HttpRequest
    Private mSearchResult As New List(Of SearchResult)
    Private mPath As String = ""
    Private mViewname As String = "vw_Search_Engine"
    Private mQyeryString As String = ""

    Public Function Authenticate_User() As myUser
        If Context.User.Identity.Name = "" Then
            Return Nothing
        End If
        Dim lContextUser As String = Context.User.Identity.Name
        If pObjCurrentUsers Is Nothing Then pObjCurrentUsers = New myUsers
        pObjCurrentUsers.CONTEXT = Context
        If InStr(lContextUser, My.Computer.Name) <> 0 Then
            Return Nothing
        End If
        mUser = pObjCurrentUsers.GetCurrentUserByCwId(lContextUser)
        If mUser Is Nothing Then
            Return Nothing
        Else
            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035817 - GBOX WebForms: Add New Function for updating columns
            ' Comment           : Added code for updating new columns in User table
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-12-12
            mUser.UserAccessStatus(mUser.CW_ID, "GBOX SEARCH")
            ' Reference  END    : CR ZHHR 1035817
        End If
        Return mUser
    End Function
    Private Sub FindPath(ByVal lSearch_Row As DataRow)
        Dim lsql As String = "Select * from " & mViewname & " where "
        lsql &= "TOPIC_GROUP_CONTEXT_ID='" & lSearch_Row("TOPIC_GROUP_CONTEXT_ID").ToString & "' AND "
        lsql &= "TOPIC_GROUP_ID='" & lSearch_Row("TOPIC_GROUP_ID").ToString & "' AND "
        lsql &= "TOPIC_ID='" & lSearch_Row("TOPIC_ID").ToString & "' AND "
        lsql &= "CHILD_OBJ_ID='" & lSearch_Row("CHILD_OBJ_ID").ToString & "'"
        Dim lDT As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        If lDT Is Nothing Then Exit Sub
        If lDT.Rows.Count = 0 Then Exit Sub
        If lDT.Rows(0)("PARENT_OBJ_ID").ToString = "ROOT" Then
            mPath = lDT.Rows(0)("CHILD_OBJ_ID").ToString.Trim & "/" & mPath
        Else
            mPath = lDT.Rows(0)("CHILD_OBJ_ID").ToString.Trim & "/" & mPath
            lsql = "Select * from " & mViewname & " where "
            lsql &= "TOPIC_GROUP_CONTEXT_ID='" & lSearch_Row("TOPIC_GROUP_CONTEXT_ID").ToString & "' AND "
            lsql &= "TOPIC_GROUP_ID='" & lSearch_Row("TOPIC_GROUP_ID").ToString & "' AND "
            lsql &= "TOPIC_ID='" & lSearch_Row("TOPIC_ID").ToString & "' AND "
            lsql &= "CHILD_OBJ_ID='" & lSearch_Row("PARENT_OBJ_ID").ToString & "'"
            lDT = mUser.Databasemanager.MakeDataTable("Search", "Search", "Search", "Search", "Search", lsql)
            If lDT Is Nothing Then Exit Sub
            If lDT.Rows.Count = 0 Then Exit Sub
            FindPath(lDT.Rows(0))
        End If
    End Sub

    Private Function makeUrlPath(ByVal lSearch_Row As DataRow) As String
        Dim lUrlAndPath As String = ""
        lUrlAndPath = "?CONTEXT=" & lSearch_Row("TOPIC_GROUP_CONTEXT_ID").ToString
        With Request
            lUrlAndPath = lUrlAndPath & "&TOPICGROUP=" & lSearch_Row("TOPIC_GROUP_MENU_TEXT").ToString
            lUrlAndPath = lUrlAndPath & "&TOPIC=" & lSearch_Row("TOPIC_MENU_TEXT").ToString
            FindPath(lSearch_Row)
            lUrlAndPath = lUrlAndPath & "&PATH=" & mPath
            mPath = ""
        End With
        '------------------------------------------------------------------------------
        'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
        'Comment    : Replace all blanks in GBOX Cockpit URL by %20
        'Added by   : Pratyusa Lenka (CWID : EOJCG)
        'Date       : 2016-02-22
        lUrlAndPath = Regex.Replace(lUrlAndPath, "\s+", "%20")
        ' Reference END : CR ZHHR 1053558
        '------------------------------------------------------------------------------
        Return lUrlAndPath
    End Function
    Public Function Search(ByVal lCriteria As String) As List(Of SearchResult)
        Dim lsql As String = "Select Top(1) * from " & mViewname & " "

        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)

        lsql = "Select distinct *, Child_Obj_ID + ' ' + OBJ_DESCRIPTION as txt from " & mViewname & " where "
        For Each lColumn As DataColumn In ldt.Columns
            lsql = lsql & lColumn.ColumnName & " like '%" & lCriteria & "%' OR "
        Next lColumn
        lsql = lsql.Substring(0, lsql.Length - 4)

        ldt = mUser.Databasemanager.MakeDataTable("Search", "Search", "Search", "Search", "Search", lsql)
        Dim i As Long = 0
        For Each r As DataRow In ldt.Rows
            Dim ctl As New HyperLink
            Dim lPath As String = makeUrlPath(r)
            If lPath.EndsWith("/") Then lPath = lPath.Substring(0, lPath.Length - 1)
            Dim lScript As String = "http://" & mLinkserver & "/Cockpit.aspx" & lPath & ""
            Dim lresult As New SearchResult
            lresult.NavigateUrl = lScript
            lresult.ResultText = r("Description").ToString & " | " & r("TOPIC_GROUP_PRESENTATION_TEXT").ToString & " | " & r("TOPIC_MENU_TEXT").ToString & " | " & r("OBJ_DISPLAY_NAME").ToString & " (" & r("OBJ_DESCRIPTION").ToString & ")"
            mSearchResult.Add(lresult)
            mPath = ""
        Next r

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1055145 - GBOX Cockpit: Enhancement of Search Engine
        ' Comment           : Added code for enhancement of search functionality in cockpit
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 05-Apr-2016

        'Dynamic query using cursor in storeprocedure
        lsql = ""
        Dim lsqla As String = ""
        Dim reader = mUser.Databasemanager.GetDataReader("sp_Search_Engine_Enhance")
        While reader.Read()
            lsql = reader.Item(0).ToString
        End While
        reader.Close()

        'Read search table and column information for preparing filter condition and display search result
        Dim ldto As DataTable = mUser.Databasemanager.MakeDataTable("select * from OBJ_SEARCH_SETTINGS WHERE OBJ_ID='" & lCriteria & "'")
        If (Not ldto Is Nothing) Then
            If ldto.Rows.Count > 0 Then
                For Each ro As DataRow In ldto.Rows
                    lsqla = lsqla & ro("COL_NAME").ToString & " like '%" & lCriteria & "%' OR "
                Next
                lsql = lsql & " AND " & lsqla
                lsql = lsql.Substring(0, lsql.Length - 4)

                Dim ldtn As DataTable = mUser.Databasemanager.MakeDataTable(lsql)

                If mUser.Databasemanager.ErrText.Contains("Invalid") And (ldtn Is Nothing) Then
                    Dim lresult As New SearchResult
                    lresult.ResultText = "Error in table {OBJ_SEARCH_SETTINGS}"
                    mSearchResult.Add(lresult)
                End If

                If (Not ldtn Is Nothing) Then
                    For Each r As DataRow In ldtn.Rows
                        Dim ctl As New HyperLink
                        Dim lPath As String = makeUrlPath(r)
                        If lPath.EndsWith("/") Then lPath = lPath.Substring(0, lPath.Length - 1)
                        Dim lScript As String = "http://" & mLinkserver & "/Cockpit.aspx" & lPath & ""
                        Dim lresult As New SearchResult
                        lresult.NavigateUrl = lScript
                        lresult.ResultText = r("Description").ToString & " | " & r("TOPIC_GROUP_PRESENTATION_TEXT").ToString & " | " & r("TOPIC_MENU_TEXT").ToString & " | " & r("OBJ_DISPLAY_NAME").ToString & " (" & r("OBJ_DESCRIPTION").ToString & ")"
                        mSearchResult.Add(lresult)
                        mPath = ""
                    Next
                End If
                ' Reference End    : ZHHR 1055145
            End If
        End If
        Return mSearchResult
    End Function

    Public Property Context As HttpContext
        Get
            Return mContext
        End Get
        Set(ByVal value As HttpContext)
            mContext = value
        End Set
    End Property

    Public Property Linkserver As String
        Get
            Return mLinkserver
        End Get
        Set(ByVal value As String)
            mLinkserver = value
        End Set
    End Property

    Public Property Request As HttpRequest
        Get
            Return mRequest
        End Get
        Set(ByVal value As HttpRequest)
            mRequest = value
        End Set
    End Property

    Public ReadOnly Property SearchResult As List(Of SearchResult)
        Get
            Return mSearchResult
        End Get
    End Property

    Public Property Viewname As String
        Get
            Return mViewname
        End Get
        Set(ByVal value As String)
            mViewname = value
        End Set
    End Property

    Public Property QyeryString As String
        Get
            Return mQyeryString
        End Get
        Set(ByVal value As String)
            mQyeryString = value
        End Set
    End Property

End Class
