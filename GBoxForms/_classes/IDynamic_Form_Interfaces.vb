Public Interface IDynamic_View_Controller

#Region "Dynamic_View_Interface_Definitions"
    Event ErrorMessage(ByVal lErrorText As String)
    Event imgQuery(ByVal lEnabled As Boolean)
    Event ShowTheQuery()
    Event TooManyInfo(ByVal lToManyText As String)
    Event CancelMany(ByVal lHits As Long)
    Event DatabaseChange(ByVal lDatabase As String)
    Event ShowSql(ByVal lsql As String)
    Event ChangeView(ByVal lViewIndex As Long)
    Event Locked(ByVal lMessage As String)

    Sub ErrorInfo(ByVal lErrString As String)
    Sub RaiseEventShowSql(ByVal lsql As String)
    Sub RaiseEventimgQuery(ByVal lEnabled As Boolean)
    Sub RaiseEventShowTheQuery()
    Sub RaiseEventTooManyInfo(ByVal lToManyText As String)
    Sub RaiseEventCancelMany(ByVal lHits As Long)
    Sub RaiseEventDatabaseChange(ByVal lDatabase As String)
    Sub RaiseEventChangeView(ByVal lViewIndex As Long)

    Function BindView(ByRef lControl As View, ByVal lWithPages As Boolean) As Boolean
    'Function getupdatestring(ByVal Request As Object) As String
    Function GetWhereString(ByVal lRequest As Object) As String
    Function GetFieldInfoDataTable() As DataTable
    'Function BindTextgrid(ByRef lstTexts As ListBox, ByVal lwhereString As String, ByVal lNewRequest As Boolean) As Boolean
    'Function CancelRequest() As Boolean
    Function BindDetail(ByRef lDetailsview As DetailsView, ByRef lDataGrid As GridView, ByVal ispostback As Boolean, ByVal lNewRequest As Boolean, Optional ByVal lCopyRequest As Boolean = False) As Boolean
    Function GetKeyValue(ByVal lRequest As Object) As String
    Function Showquery(ByRef lgrdQuery As GridView, Optional ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs = Nothing) As Boolean

    Property WhereString() As String
    Property Requestform() As Object
    Property IsPostback() As Boolean
    Property TOPIC_ID() As String
    Property TOPIC_GROUP_ID As String
    Property TOPIC_GROUP_CONTEXT_ID As String
    Property Request() As HttpRequest
    ReadOnly Property View(ByVal lView As View) As View
    ReadOnly Property KeyPath As String

#End Region

End Interface
Public Interface IDynamic_View_Controller_Factory
    Sub ErrorInfo(ByVal lErrString As String)
    Sub LoadTree(ByRef ltrvOBJ As TreeView)
    Function SelectedNodeChange(ByVal lSelectedNode As TreeNode) As IDynamic_View_Controller
    Function GetUser(ByVal lstrCWID As String) As myUser
    Property IsPostback() As Boolean
    Property Request() As HttpRequest
    Property TOPIC_ID() As String
    Property TOPIC_GROUP_ID As String
    Property TOPIC_GROUP_CONTEXT_ID As String
    Event ErrorMessage(ByVal lErrorText As String)
    Event Impersonate(ByVal lCw_ID As String)
    Event LoadWizzardData(ByVal lTopic As String)
    Event TableChange(ByVal lstrTablename As String)
End Interface