Public Class MyKeyObjs
    Private mMyKeyObjs As List(Of myKeyObj)
    Public Function MakeWhere() As String
        Return ""
    End Function
    Public Function Add(ByVal lKeyVal As myKeyObj) As myKeyObj
        If mMyKeyObjs Is Nothing Then mMyKeyObjs = New List(Of myKeyObj)
        mMyKeyObjs.Add(lKeyVal)
        Return lKeyVal
    End Function
    Public Function Add(ByVal lKeyId As String, ByVal lCurrentValue As String) As myKeyObj
        If mMyKeyObjs Is Nothing Then mMyKeyObjs = New List(Of myKeyObj)
        Dim lKey As New myKeyObj
        lKey.Key_ID = lKeyId
        lKey.CurrentValue = lCurrentValue
        mMyKeyObjs.Add(lKey)
        Return lKey
    End Function
    Public Property MyKeyObjs As List(Of myKeyObj)
        Get
            Return mMyKeyObjs
        End Get
        Set(ByVal value As List(Of myKeyObj))
            mMyKeyObjs = value
        End Set
    End Property

End Class



Public Class myKeyObj
    Private mKey_ID As String

    Public Property Key_ID() As String
        Get
            Return mKey_ID
        End Get
        Set(ByVal value As String)
            mKey_ID = value
        End Set
    End Property

    Private mOrdinalPosition As Integer

    Public Property OrdinalPosition() As Integer
        Get
            Return mOrdinalPosition
        End Get
        Set(ByVal value As Integer)
            mOrdinalPosition = value
        End Set
    End Property


    Private mCurrentValue As String

    Public Property CurrentValue() As String
        Get
            Return mCurrentValue
        End Get
        Set(ByVal value As String)
            mCurrentValue = value
        End Set
    End Property
    Private mDisplayname As String

    Public Property Displayname As String
        Get
            Return mDisplayname
        End Get
        Set(ByVal value As String)
            mDisplayname = value
        End Set
    End Property

End Class
