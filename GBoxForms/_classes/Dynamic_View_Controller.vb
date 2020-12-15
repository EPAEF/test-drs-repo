Option Strict Off
Public Class Dynamic_View_Controller
    Implements IDynamic_View_Controller

#Region "Events and Subs for Child Events"
    Public Event CancelMany(ByVal lHits As Long) Implements IDynamic_View_Controller.CancelMany
    Public Event DatabaseChange(ByVal lDatabase As String) Implements IDynamic_View_Controller.DatabaseChange
    Public Event ErrorMessage(ByVal lErrorText As String) Implements IDynamic_View_Controller.ErrorMessage
    Public Event imgQuery(ByVal lEnabled As Boolean) Implements IDynamic_View_Controller.imgQuery
    Public Event ShowTheQuery() Implements IDynamic_View_Controller.ShowTheQuery
    Public Event TooManyInfo(ByVal lToManyText As String) Implements IDynamic_View_Controller.TooManyInfo
     Public Event ShowSql(ByVal lsql As String) Implements IDynamic_View_Controller.ShowSql
    Public Event ChangeView(ByVal lViewindex As Long) Implements IDynamic_View_Controller.ChangeView
    Public Event Locked(ByVal lMessage As String) Implements IDynamic_View_Controller.Locked

    Public Sub RaiseEventShowSql(ByVal lsql As String) Implements IDynamic_View_Controller.RaiseEventShowSql
        RaiseEvent ShowSql(lsql)
    End Sub
    
    Public Sub RaiseEventCancelMany(ByVal lHits As Long) Implements IDynamic_View_Controller.RaiseEventCancelMany
        RaiseEvent CancelMany(lHits)
    End Sub
    Public Sub RaiseEventDatabaseChange(ByVal lDatabase As String) Implements IDynamic_View_Controller.RaiseEventDatabaseChange
        RaiseEvent DatabaseChange(lDatabase)
    End Sub
    Public Sub RaiseEventimgQuery(ByVal lEnabled As Boolean) Implements IDynamic_View_Controller.RaiseEventimgQuery
        RaiseEvent imgQuery(lEnabled)
    End Sub
    
    Public Sub RaiseEventShowTheQuery() Implements IDynamic_View_Controller.RaiseEventShowTheQuery
        RaiseEvent ShowTheQuery()
    End Sub
    Public Sub RaiseEventTooManyInfo(ByVal lToManyText As String) Implements IDynamic_View_Controller.RaiseEventTooManyInfo
        RaiseEvent TooManyInfo(lToManyText)
    End Sub
    
    
   
    Public Sub RaiseeventErrorMessage(ByVal lMessage As String)
        RaiseEvent ErrorMessage(lMessage)
    End Sub
    Public Sub RaiseeventLocked(ByVal lMessage As String)
        RaiseEvent Locked(lMessage)
    End Sub
    Public Sub RaiseEventChangeView(ByVal lViewindex As Long) Implements IDynamic_View_Controller.RaiseEventChangeView
        RaiseEvent ChangeView(lViewindex)
    End Sub
#End Region

    Private mView As View = Nothing
    Private mRequest As HttpRequest = Nothing
    ReadOnly Property View(ByVal lView As View) As View Implements IDynamic_View_Controller.View
        Get
            Return mView
        End Get
    End Property
    Public Sub ErrorInfo(ByVal lErrString As String) Implements IDynamic_View_Controller.ErrorInfo
        RaiseEvent ErrorMessage(lErrString)
    End Sub


    Public Property Request() As System.Web.HttpRequest Implements IDynamic_View_Controller.Request
        Get
            Return mRequest
        End Get
        Set(ByVal value As System.Web.HttpRequest)
            mRequest = value
        End Set
    End Property


    Private mIsPostback As Boolean
    Public Property IsPostback() As Boolean Implements IDynamic_View_Controller.IsPostback
        Get
            Return mIsPostback
        End Get
        Set(ByVal value As Boolean)
            mIsPostback = value
        End Set
    End Property
    Private mRequestform As Object
    Public Property Requestform() As Object Implements IDynamic_View_Controller.Requestform
        Get
            If Not mRequest Is Nothing Then
                Return mRequest.Form
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As Object)
            mRequestform = value
        End Set
    End Property
    Private mWhereString As String
    Public Overridable Function GetFieldInfoDataTable() As System.Data.DataTable Implements IDynamic_View_Controller.GetFieldInfoDataTable
        Return Nothing
    End Function

    Public Overridable Function GetWhereString(ByVal lRequest As Object) As String Implements IDynamic_View_Controller.GetWhereString
        Return Nothing
    End Function
    Public Overridable Property WhereString() As String Implements IDynamic_View_Controller.WhereString
        Get
            Return mWhereString
        End Get
        Set(ByVal value As String)
            mWhereString = value
        End Set
    End Property

    Public Overridable Function BindDetail(ByRef lDetailsview As System.Web.UI.WebControls.DetailsView, ByRef lDataGrid As System.Web.UI.WebControls.GridView, ByVal ispostback As Boolean, ByVal lNewRequest As Boolean, Optional ByVal lCopyRequest As Boolean = False) As Boolean Implements IDynamic_View_Controller.BindDetail

    End Function
    Private mTOPIC_ID As String
    Private mTOPIC_GROUP_ID As String
    Private mTOPIC_GROUP_CONTEXT_ID As String

    Public Property TOPIC_ID As String Implements IDynamic_View_Controller.TOPIC_ID
        Get
            Return mTOPIC_ID
        End Get
        Set(ByVal value As String)
            mTOPIC_ID = value
        End Set
    End Property

    Public Property TOPIC_GROUP_ID As String Implements IDynamic_View_Controller.TOPIC_GROUP_ID
        Get
            Return mTOPIC_GROUP_ID
        End Get
        Set(ByVal value As String)
            mTOPIC_GROUP_ID = value
        End Set
    End Property

    Public Property TOPIC_GROUP_CONTEXT_ID As String Implements IDynamic_View_Controller.TOPIC_GROUP_CONTEXT_ID
        Get
            Return mTOPIC_GROUP_CONTEXT_ID
        End Get
        Set(ByVal value As String)
            mTOPIC_GROUP_CONTEXT_ID = value
        End Set
    End Property

    Private mOrderByExpression As String

    Public Property OrderByExpression() As String
        Get
            Return mOrderByExpression
        End Get
        Set(ByVal value As String)
            mOrderByExpression = value
        End Set
    End Property

    Private mUser As myUser

    Public Property User() As myUser
        Get
            Return mUser
        End Get
        Set(ByVal value As myUser)
            mUser = value
        End Set
    End Property
    Public Overridable Function ShowQuery(ByRef lgrdQuery As GridView, Optional ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs = Nothing) As Boolean Implements IDynamic_View_Controller.Showquery

    End Function
    Public Overridable Function BindView(ByRef lControl As System.Web.UI.WebControls.View, ByVal lWithPages As Boolean) As Boolean Implements IDynamic_View_Controller.BindView

    End Function
   

    Public Overridable Function GetKeyValue(ByVal lRequest As Object) As String Implements IDynamic_View_Controller.GetKeyValue
        Return Nothing
    End Function

    Private m_columns As List(Of String)

    Public Property columns As List(Of String)
        Get
            Return m_columns
        End Get
        Set(ByVal value As List(Of String))
            m_columns = value
        End Set
    End Property



    Public Overridable ReadOnly Property KeyPath As String Implements IDynamic_View_Controller.KeyPath
        Get
            Return ""
        End Get
    End Property
End Class
