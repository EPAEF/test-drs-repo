Option Strict Off
Imports System
Imports System.Web
Imports System.Data
Imports System.Web.UI
Imports System.Web.UI.WebControls
Public Class myGenericLiteral : Implements ITemplate
    Public Sub New(ByVal lColumname As String, Optional ByVal lisNotBindeble As Boolean = False)
        mColumname = lColumname
        misNotBindeble = lisNotBindeble
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New Literal
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        container.Controls.Add(myCtl)
    End Sub
    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        Dim myCtl As Literal = CType(sender, Literal)
        If TypeOf (myCtl.NamingContainer) Is GridViewRow Then
            Dim container As GridViewRow = CType(myCtl.NamingContainer, GridViewRow)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then Exit Sub
            myCtl.Text = lRowitem(Columname).ToString
        End If
        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim container As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(container.DataItem, DataRowView)
            myCtl.Text = lRowItem(Columname).ToString()
        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then Exit Sub
            If misNotBindeble Then Exit Sub
            myCtl.Text = lRowitem(Columname).ToString
        End If
    End Sub 'BindData 
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property

    Private misNotBindeble As Boolean

    Public Property isNotBindeble() As Boolean
        Get
            Return misNotBindeble
        End Get
        Set(ByVal value As Boolean)
            misNotBindeble = value
        End Set
    End Property

End Class
Public Class myGenericTextBox : Implements ITemplate
    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New TextBox
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        myCtl.ID = Columname
        myCtl.Width = pVarConstWith
        ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
        ' Comment   : When focus is on any of the input fields and press the <Enter> button, should have the same effect as click button <Next> 
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2019-09-26
        myCtl.Attributes.Add("onkeypress", "return enterKeyPress(event,'btnNextEditText')")
        container.Controls.Add(myCtl)
    End Sub

    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        Dim myCtl As TextBox = CType(sender, TextBox)
        If TypeOf (myCtl.NamingContainer) Is GridViewRow Then
            Dim container As GridViewRow = CType(myCtl.NamingContainer, GridViewRow)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then Exit Sub
            myCtl.Text = lRowitem(Columname).ToString
        End If
        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim container As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(container.DataItem, DataRowView)
            myCtl.Text = lRowItem(Columname).ToString()
        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then
                If mColumname.ToUpper = "OBJ_FIELD_ID" And mUser.OBJ_FIELD_ID <> "" Then
                    myCtl.Text = mUser.OBJ_FIELD_ID
                    mUser.OBJ_FIELD_ID = ""
                End If
                Exit Sub
            End If
            If Columname <> "External ticket no." Then
                myCtl.Text = lRowitem(Columname).ToString
            Else
                myCtl.ToolTip = "Previous External ticket no.: " & lRowitem(Columname).ToString
            End If
        End If
    End Sub 'BindData 
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property
End Class
Public Class myGenericCheckBox : Implements ITemplate
    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New CheckBox
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        myCtl.ID = Columname
        myCtl.Width = pVarConstWith
        ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
        ' Comment   : When focus is on any of the input fields and press the <Enter> button, should have the same effect as click button <Next> 
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2019-09-26
        myCtl.Attributes.Add("onkeypress", "return enterKeyPress(event,'btnNextEditText')")
        container.Controls.Add(myCtl)
    End Sub

    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        Dim myCtl As CheckBox = CType(sender, CheckBox)
        If TypeOf (myCtl.NamingContainer) Is GridViewRow Then
            Dim container As GridViewRow = CType(myCtl.NamingContainer, GridViewRow)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then Exit Sub
            myCtl.Checked = CBool(lRowitem(Columname).ToString)
        End If
        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim container As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(container.DataItem, DataRowView)
            myCtl.Checked = CBool(lRowItem(Columname).ToString)
        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            '---------------------------------------------------------------------------
            'Reference  : ZHHR 1055758 - GBOX COC: OTT 2544 - handling of deletions
            'Comment    : Click on mark as deleted checkbox, display the help url pop-up
            'Added by   : Pratyusa Lenka (CWID : EOJCG)
            'Date       : 2016-04-18
            If Columname.ToUpper = "MARKED AS DELETED" Then
                myCtl.Attributes.Add("onclick", "javascript:window.open('" + "http://sp-appl-bbs.bayer-ag.com/sites/010045/drs/DRS_wiki/Deletion.aspx" + "','popup_window', 'width=1080,height=600,left=100,top=100,resizable=yes');return true;")
            End If
            ' Reference END : CR ZHHR 1055758
            '---------------------------------------------------------------------------
            If lRowitem Is Nothing Then Exit Sub
            myCtl.Checked = CBool(lRowitem(Columname).ToString)
        End If
    End Sub 'BindData 
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property
End Class
Public Class myGenericRequiredField : Implements ITemplate
    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New TextBox
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        container.Controls.Add(myCtl)
        myCtl.ID = Columname
        myCtl.Width = pVarConstWith
        myCtl.BackColor = Drawing.Color.Gold
        ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
        ' Comment   : When focus is on any of the input fields and press the <Enter> button, should have the same effect as click button <Next> 
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2019-09-26
        myCtl.Attributes.Add("onkeypress", "return enterKeyPress(event,'btnNextEditText')")
        If mRequiredText Is Nothing Then mRequiredText = "REQUIRED"
        Dim rfv As New RequiredFieldValidator
        rfv.Text = mRequiredText
        rfv.ControlToValidate = myCtl.ID
        rfv.Display = ValidatorDisplay.Dynamic
        rfv.ID = "validate" & myCtl.ID
        myCtl.ValidationGroup = "e"
        container.Controls.Add(rfv)
    End Sub

    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        Dim myCtl As TextBox = CType(sender, TextBox)
        If TypeOf (myCtl.NamingContainer) Is GridViewRow Then
            Dim container As GridViewRow = CType(myCtl.NamingContainer, GridViewRow)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then Exit Sub
            myCtl.Text = lRowitem(Columname).ToString()
        End If

        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim container As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(container.DataItem, DataRowView)
            myCtl.Text = lRowItem(Columname).ToString()

        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then
                If mColumname.ToUpper = "OBJ_FIELD_ID" And mUser.OBJ_FIELD_ID <> "" Then
                    myCtl.Text = mUser.OBJ_FIELD_ID
                    mUser.OBJ_FIELD_ID = ""
                End If
                Exit Sub
            End If
            If Columname.ToUpper = "RANK".ToUpper Or _
            Columname.ToUpper = "APPLICATION_ROLE_RANK".ToUpper Or _
            Columname.ToUpper = "Ordinal_Position".ToUpper Then
                'ToDo: OrdinalPosition Dynamisch ermitteln
                myCtl.Text = lRowitem(Columname).ToString
            Else
                myCtl.Text = lRowitem(Columname).ToString
            End If



        End If
    End Sub 'BindData 
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property



    Private mRequiredText As String

    Public Property RequiredText() As String
        Get
            Return mRequiredText
        End Get
        Set(ByVal value As String)
            mRequiredText = value
        End Set
    End Property

End Class
Public Class myGenericDropdownList : Implements ITemplate
    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    Private myCtl As DropDownList
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New DropDownList
        myCtl.ID = mColumname
        myCtl.Width = pVarConstWith + 5
        ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
        ' Comment   : When focus is on any of the input fields and press the <Enter> button, should have the same effect as click button <Next> 
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2019-09-26
        myCtl.Attributes.Add("onkeypress", "return enterKeyPress(event,'btnNextEditText')")
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        'AddHandler myCtl.SelectedIndexChanged, AddressOf Me.ParentChange
        'myCtl.AutoPostBack = True
        container.Controls.Add(myCtl)
        '---------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
        ' Comment   : Attach the required field validator for LOOKUP fields
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-09-19
        If mRequired Then
            '--------------------------------------------------------------------------------------------
            ' Reference : ZHHR 1062728 - GBOX COC: OTT 3514 - Enhance LOOKUP functionality
            ' Comment   : If a field was marked as required in OBJ_FIELD, it should be marked as yellow
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-10-20
            myCtl.BackColor = Drawing.Color.Gold
            Dim rfv As New RequiredFieldValidator
            If mRequiredText Is Nothing Then mRequiredText = "REQUIRED"
            With rfv
                .Text = mRequiredText
                .ControlToValidate = myCtl.ID
                .Display = ValidatorDisplay.Dynamic
                .ID = "validate" & myCtl.ID
                .InitialValue = "NA"
            End With
            ' Reference END : CR ZHHR 1062728
            '--------------------------------------------------------------------------------------------
            container.Controls.Add(rfv)
        End If
        ' Reference END : CR ZHHR 1059522
        '-------------------------------------------------------------------------------------
    End Sub
    Public Sub ParentChange(ByVal sender As Object, ByVal e As EventArgs)
        'sender.databind()
    End Sub
    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        'If mUser Is Nothing Then
        '    mUser = pCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        'End If

        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1037698 - GBOX COC : Data validation in DRS Handbook
        ' Comment           : Added Trim function below to remove leading and trailing spaces.
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2015-02-18

        myCtl = CType(sender, DropDownList)

        Dim lTable As String = mTable
        Dim lFilter As String = mFilter
        Dim lKey As String = mKey
        '--------------------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
        ' Comment   : To avoid out of memory exception, don't consider filter table for cascade field
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-07-26
        Dim lsql As String = ""
        If lFilter <> "" And mUser.Databasemanager.IsObjFieldInLookupFilter(lKey) Then
            lsql = "Select DISTINCT " & lKey & "   from " & lTable & " Order by " & lKey
        Else
            lsql = "Select DISTINCT " & lKey & "   from " & lTable & " " & lFilter & " Order by " & lKey
        End If
        ' Reference END : CR ZHHR 1059522
        '--------------------------------------------------------------------------------------------


        '--------------------------------------------------------------------------------------------
        ' Reference : CRT 2046721 - GBOX_COC: add empty option in non required lookup field in report
        ' Comment   : Append each item to dorpdown control 
        '             insted of direclty apending to datasource. 
        '             Which help below to append empty item/option to non required lookup dropdown

        ''myCtl.DataSource = mUser.Databasemanager.MakeDataTable(lsql)
        myCtl.DataTextField = lKey
        myCtl.DataValueField = lKey
        'myCtl.BackColor = Drawing.Color.Gold
        'myCtl.Items.Insert(0, "Choose " & mColumname)

        Dim temTable As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        For I = 0 To temTable.Rows.Count - 1
            myCtl.Items.Add(temTable.Rows(I)(lKey).ToString())
        Next

        '--------------------------------------------------------------------------------------------

        If TypeOf (myCtl.NamingContainer) Is GridViewRow Then
            Dim container As GridViewRow = CType(myCtl.NamingContainer, GridViewRow)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            Dim lSelectedvalue As String = lRowitem(Columname).ToString().Trim
            If lSelectedvalue <> "" Then
                myCtl.SelectedValue = lSelectedvalue
            End If
        End If
        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim grdContainer As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(grdContainer.DataItem, DataRowView)
            Dim lSelectedvalue As String = lRowItem(Columname).ToString().Trim
            If lSelectedvalue <> "" Then
                myCtl.SelectedValue = lSelectedvalue
            End If
        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then
                If mColumname = "OBJ_ID" And mUser.OBJ_Value <> "" And InStr(mUser.Current_OBJ.OBJ_ID.ToUpper, "Obj_FIELD".ToUpper) = 0 Then
                    myCtl.SelectedValue = mUser.OBJ_Value
                    mUser.OBJ_Value = ""
                End If
                '--------------------------------------------------------------------------------------------
                ' Reference : CRT 2046721 - GBOX_COC: add empty option in non required lookup field in report
                ' Comment   : Add First item in dropdown list as empty               
                If Not mRequired Then
                    myCtl.Items.Insert(0, "")
                End If
                '-------------------------------------------------------------------------------------
                Exit Sub
            Else
                '--------------------------------------------------------------------------------------------
                ' Reference : CRT 2046721 - GBOX_COC: add empty option in non required lookup field in report
                ' Comment   : Add First item in dropdown list as empty 
                If Not mRequired Then
                    myCtl.Items.Insert(0, "")
                End If
                '-------------------------------------------------------------------------------------
            End If
            Dim lSelectedvalue As String = lRowitem(Columname).ToString().Trim
            If lSelectedvalue <> "" Then
                Try
                    myCtl.SelectedValue = lSelectedvalue
                Catch ex As Exception
                    Dim x As String = "myGenericDropdownList:Binddata" & ex.Message
                    MsgBox(x)
                End Try
            End If

        End If

        ' Reference End     : ZHHR 1037698
    End Sub
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property

    Private mLookUpSql As String = "Select * from Subgroup"

    Public Property LookUpSql() As String
        Get
            Return mLookUpSql
        End Get
        Set(ByVal value As String)
            mLookUpSql = value
        End Set
    End Property

    Private mTable As String = "Subgroup"

    Public Property Table() As String
        Get
            Return mTable
        End Get
        Set(ByVal value As String)
            mTable = value
        End Set
    End Property

    Private mFilter As String

    Public Property Filter() As String
        Get
            Return mFilter
        End Get
        Set(ByVal value As String)
            mFilter = value
        End Set
    End Property

    Private mKey As String = "Subgroup_ID"

    Public Property Key() As String
        Get
            Return mKey
        End Get
        Set(ByVal value As String)
            mKey = value
        End Set
    End Property

    Private mDescription As String = "TEXT"

    Public Property Description() As String
        Get
            Return mDescription
        End Get
        Set(ByVal value As String)
            mDescription = value
        End Set
    End Property
    Public ReadOnly Property Current() As DropDownList
        Get
            Return myCtl
        End Get
    End Property
    '---------------------------------------------------------------------------------
    ' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
    ' Comment   : Properties to check if required and corresponding required text
    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ' Date      : 2016-09-19
    Private mRequired As Boolean
    Public Property Required As Boolean
        Get
            Return mRequired
        End Get
        Set(ByVal value As Boolean)
            mRequired = value
        End Set
    End Property
    Private mRequiredText As String
    Public Property RequiredText() As String
        Get
            Return mRequiredText
        End Get
        Set(ByVal value As String)
            mRequiredText = value
        End Set
    End Property
    ' Reference END : CR ZHHR 1059522
    '---------------------------------------------------------------------------------
End Class
Public Class myGenericSmallDropdownList : Implements ITemplate
    Private myCtl As DropDownList

    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New DropDownList
        myCtl.ID = mColumname
        myCtl.Width = pVarConstWith + 5
        ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
        ' Comment   : When focus is on any of the input fields and press the <Enter> button, should have the same effect as click button <Next> 
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2019-09-26
        myCtl.Attributes.Add("onkeypress", "return enterKeyPress(event,'btnNextEditText')")
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        container.Controls.Add(myCtl)
        '-------------------------------------------------------------------------------------
        ' Reference : CR ZHHR 1062011 - GBOX COC: OTT 3439 - Enhance LOOKUP_LIST functionality
        ' Comment   : Attach the required field validator for LOOKUP list
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2016-09-15
        If mRequired Then
            '--------------------------------------------------------------------------------------------
            ' Reference : ZHHR 1062728 - GBOX COC: OTT 3514 - Enhance LOOKUP functionality
            ' Comment   : If a field was marked as required in OBJ_FIELD, it should be marked as yellow
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-10-19
            myCtl.BackColor = Drawing.Color.Gold
            Dim rfv As New RequiredFieldValidator
            If mRequiredText Is Nothing Then mRequiredText = "REQUIRED"
            With rfv
                .Text = mRequiredText
                .ControlToValidate = myCtl.ID
                .Display = ValidatorDisplay.Dynamic
                .ID = "validate" & myCtl.ID
                .InitialValue = "NA"
            End With
            ' Reference END : CR ZHHR 1062728
            '--------------------------------------------------------------------------------------------
            container.Controls.Add(rfv)
        End If
        ' Reference END : CR ZHHR 1062011
        '-------------------------------------------------------------------------------------
    End Sub

    Public ReadOnly Property CONTEXT()
        Get
            Return pObjCurrentUsers.CONTEXT
        End Get

    End Property

    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If


        myCtl = CType(sender, DropDownList)
        Dim lStr As Array = mLookUpString.Split(";")
        For I = 0 To lStr.GetUpperBound(0)
            myCtl.Items.Add(lStr(I))
        Next
        If TypeOf (myCtl.NamingContainer) Is GridViewRow Then
            Dim container As GridViewRow = CType(myCtl.NamingContainer, GridViewRow)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            Dim lSelectedvalue As String = lRowitem(Columname).ToString()
            If lSelectedvalue <> "" Then
                myCtl.SelectedValue = lSelectedvalue
            End If
        End If
        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim grdContainer As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(grdContainer.DataItem, DataRowView)
            Dim lSelectedvalue As String = lRowItem(Columname).ToString()
            If lSelectedvalue <> "" Then
                myCtl.SelectedValue = lSelectedvalue
            End If
        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then
                If mColumname = "OBJ_ID" And mUser.OBJ_Value <> "" And mUser.Current_OBJ.OBJ_ID.ToUpper <> "Obj_FIELD".ToUpper Then
                    myCtl.SelectedValue = mUser.OBJ_Value
                    mUser.OBJ_Value = ""
                End If
                'myCtl.Items.Insert(0, "Choose " & Columname)
                '-------------------------------------------------------------------------------------
                ' Reference : CR ZHHR 1062011 - GBOX COC: OTT 3439 - Enhance LOOKUP_LIST functionality
                ' Comment   : First item in dropdown list is empty
                ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                ' Date      : 2016-09-15
                If Not mRequired Then
                    myCtl.Items.Insert(0, "")
                End If
                'myCtl.Items.Insert(0, "")
                ' Reference END : CR ZHHR 1062011
                '-------------------------------------------------------------------------------------
                Exit Sub
            Else
                ' Reference : IM0003967749 - GBOX COC: OTT 3439 - Enhance LOOKUP_LIST functionality
                ' Comment   : First item in dropdown list is empty
                ' Added by  : Milind 
                ' Date      : 2016-12-29
                If Not mRequired Then
                    myCtl.Items.Insert(0, "")
                End If
                'Exit Sub
                ' Reference End : IM0003967749
            End If

            Dim lSelectedvalue As String = lRowitem(Columname).ToString()
            If lSelectedvalue <> "" Then
                Try
                    myCtl.SelectedValue = lSelectedvalue
                Catch ex As Exception
                    Dim x As String = "SDL:" & ex.Message
                    MsgBox(x)
                End Try
            End If

        End If
    End Sub
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property

    Private mLookUpString As String = "A;B"

    Public Property LookUpString() As String
        Get
            Return mLookUpString
        End Get
        Set(ByVal value As String)
            mLookUpString = value
        End Set
    End Property


    Public ReadOnly Property Current() As DropDownList
        Get
            Return myCtl
        End Get
    End Property
    '-------------------------------------------------------------------------------------
    ' Reference : CR ZHHR 1062011 - GBOX COC: OTT 3439 - Enhance LOOKUP_LIST functionality
    ' Comment   : Properties to check if required and corresponding required text
    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
    ' Date      : 2016-09-15
    Private mRequired As Boolean
    Public Property Required As Boolean
        Get
            Return mRequired
        End Get
        Set(ByVal value As Boolean)
            mRequired = value
        End Set
    End Property
    Private mRequiredText As String
    Public Property RequiredText() As String
        Get
            Return mRequiredText
        End Get
        Set(ByVal value As String)
            mRequiredText = value
        End Set
    End Property
    ' Reference END : CR ZHHR 1062011
    '-------------------------------------------------------------------------------------
End Class
Public Class myGenericGridview : Implements ITemplate

    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New GridView
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        container.Controls.Add(myCtl)
    End Sub

    Public ReadOnly Property CONTEXT()
        Get
            Return pObjCurrentUsers.CONTEXT
        End Get

    End Property

    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If
        Dim lCmbCombo As GridView = CType(sender, GridView)
        Dim lTable As String = mTable
        Dim lFilter As String = mFilter
        Dim lKey As String = mKey
        Dim lDescription As String = mDescription
        Dim lsql As String = "Select " & lKey & ", " & lDescription & ", rtrim(" & lKey & "+ ' ('+" & lDescription & "+')') as txt   from " & lTable & " " & lFilter & " Order by " & lKey
        lCmbCombo.DataSource = mUser.databasemanager.MakeDataTable(lsql)
        'lCmbCombo.DataTextField = "txt"
        'lCmbCombo.DataValueField = lKey
        lCmbCombo.ID = mColumname
    End Sub
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property

    Private mLookUpSql As String = "Select * from Subgroup"

    Public Property LookUpSql() As String
        Get
            Return mLookUpSql
        End Get
        Set(ByVal value As String)
            mLookUpSql = value
        End Set
    End Property

    Private mTable As String = "Subgroup"

    Public Property Table() As String
        Get
            Return mTable
        End Get
        Set(ByVal value As String)
            mTable = value
        End Set
    End Property

    Private mFilter As String

    Public Property Filter() As String
        Get
            Return mFilter
        End Get
        Set(ByVal value As String)
            mFilter = value
        End Set
    End Property

    Private mKey As String = "Subgroup_ID"

    Public Property Key() As String
        Get
            Return mKey
        End Get
        Set(ByVal value As String)
            mKey = value
        End Set
    End Property

    Private mDescription As String = "TEXT"

    Public Property Description() As String
        Get
            Return mDescription
        End Get
        Set(ByVal value As String)
            mDescription = value
        End Set
    End Property

End Class
Public Class myGenericRegularExpressionValidator : Implements ITemplate
    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New TextBox
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        container.Controls.Add(myCtl)
        myCtl.ID = Columname
        myCtl.Width = pVarConstWith
        ' Reference : YHHR 2052043 - GBOX COC: Press <ENTER> equal to the button <Next>/<Submit>
        ' Comment   : When focus is on any of the input fields and press the <Enter> button, should have the same effect as click button <Next> 
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2019-09-26
        myCtl.Attributes.Add("onkeypress", "return enterKeyPress(event,'btnNextEditText')")

        If mRequired Then
            myCtl.BackColor = Drawing.Color.Gold

            Dim rfv As New RequiredFieldValidator
            If mRequiredText Is Nothing Then mRequiredText = "REQUIRED"
            With rfv
                .Text = mRequiredText
                .ControlToValidate = myCtl.ID
                .Display = ValidatorDisplay.Dynamic
                .ID = "validate" & myCtl.ID
            End With
            container.Controls.Add(rfv)
        End If

        If myCtl.ID.ToUpper = "VALID FROM" Then

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR - BY-RZ04-CMT-28531 -  GBOXFORMS- Correction of Valid from Datetime for Server with Different Datetime Propertys
            ' Comment           : To get the Datetime in Bayer format (YYYY-MM-DD)
            ' Added by          : Surendra Purav (CWID : EQIZU)
            ' Date              : 2013-10-29
            '---------------------------------------------------------------------------------------------------                            
            myCtl.Text = GetBYDate()
        End If
        '------------------------------------------------------------------------------
        'Reference  : ZHHR 1053558 - GBOX COC: OTT 1369 - GBOX Cockpit enhancements -1-
        'Comment    : Default value for OBJ_VALID_TO should be 9999-12-31
        'Added by   : Pratyusa Lenka (CWID : EOJCG)
        'Date       : 2016-02-15
        If myCtl.ID.ToUpper = "VALID TO" Then
            myCtl.Text = "9999-12-31"
        End If
        ' Reference END : CR ZHHR 1053558
        '------------------------------------------------------------------------------
        Dim rev As New RegularExpressionValidator
        With rev
            .ValidationExpression = mVALIDATION_EXPRESSION
            .ErrorMessage = mVALIDATION_HELP_TEXT
            .ToolTip = mVALIDATION_TOOL_TIP
            .ControlToValidate = myCtl.ID
        End With
        container.Controls.Add(rev)

    End Sub

    '---------------------------------------------------------------------------------------------------
    ' Reference         : CR - BY-RZ04-CMT-28531 -  GBOXFORMS- Correction of Valid from Datetime for Server with Different Datetime Propertys
    ' Comment           : To change the Datetime format to Bayer format (YYYY-MM-DD)
    ' Added by          : Surendra Purav (CWID : EQIZU)
    ' Date              : 2013-10-29
    '---------------------------------------------------------------------------------------------------                
    Public Function GetBYDate(Optional ByVal lDateToTransform As Date = Nothing) As String

        Dim lDtYYYY As String = ""
        Dim lDtMM As String = ""
        Dim lDtDD As String = ""
        Dim lDate As String = ""

        If lDateToTransform = Nothing Then
            lDtYYYY = Now.Year()
            lDtMM = Now.Month()
            lDtDD = Now.Day()
        Else
            lDtYYYY = lDateToTransform.Year()
            lDtMM = lDateToTransform.Month()
            lDtDD = lDateToTransform.Day()
        End If

        If lDtDD.Length() = 1 Then lDtDD = Right("00" & lDtDD, 2)
        If lDtMM.Length() = 1 Then lDtMM = Right("00" & lDtMM, 2)
        lDate = lDtYYYY & "-" & lDtMM & "-" & lDtDD

        Return lDate

    End Function

    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        Dim myCtl As TextBox = CType(sender, TextBox)
        If TypeOf (myCtl.NamingContainer) Is GridViewRow Then
            Dim container As GridViewRow = CType(myCtl.NamingContainer, GridViewRow)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then Exit Sub
            myCtl.Text = lRowitem(Columname).ToString()
        End If

        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim container As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(container.DataItem, DataRowView)
            myCtl.Text = lRowItem(Columname).ToString()

        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then Exit Sub
            If lRowitem Is Nothing Then
                If mColumname.ToUpper = "OBJ_FIELD_ID" And mUser.OBJ_FIELD_ID <> "" Then
                    myCtl.Text = mUser.OBJ_FIELD_ID
                    mUser.OBJ_FIELD_ID = ""
                End If
                Exit Sub
            End If
            myCtl.Text = lRowitem(Columname).ToString
        End If
    End Sub 'BindData 
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property





    Private mVALIDATION_EXPRESSION As String

    Public Property VALIDATION_EXPRESSION() As String
        Get
            Return mVALIDATION_EXPRESSION
        End Get
        Set(ByVal value As String)
            mVALIDATION_EXPRESSION = value
        End Set
    End Property
    Private mRequiredText As String

    Public Property RequiredText() As String
        Get
            Return mRequiredText
        End Get
        Set(ByVal value As String)
            mRequiredText = value
        End Set
    End Property
    Private mVALIDATION_HELP_TEXT As String

    Public Property VALIDATION_HELP_TEXT() As String
        Get
            Return mVALIDATION_HELP_TEXT
        End Get
        Set(ByVal value As String)
            mVALIDATION_HELP_TEXT = value
        End Set
    End Property

    Private mVALIDATION_TOOL_TIP As String

    Public Property VALIDATION_TOOL_TIP() As String
        Get
            Return mVALIDATION_TOOL_TIP
        End Get
        Set(ByVal value As String)
            mVALIDATION_TOOL_TIP = value
        End Set
    End Property

    Private mRequired As Boolean


    Public Property Required As Boolean
        Get
            Return mRequired
        End Get
        Set(ByVal value As Boolean)
            mRequired = value
        End Set
    End Property

End Class
Public Class myGenericRangeValidator : Implements ITemplate
    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New TextBox
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        container.Controls.Add(myCtl)
        myCtl.ID = Columname
        myCtl.Width = pVarConstWith
        Dim rfv As New RequiredFieldValidator
        If mRequiredText Is Nothing Then mRequiredText = "REQIRED"
        With rfv
            .Text = mRequiredText
            .ControlToValidate = myCtl.ID
            .Display = ValidatorDisplay.Dynamic
            .ID = "validate" & myCtl.ID
        End With
        container.Controls.Add(rfv)
        Dim rev As New RangeValidator
        With rev
            .MinimumValue = mMIN_VALUE
            .MaximumValue = mMAX_VALUE
            .ControlToValidate = myCtl.ID
            .ErrorMessage = mVALIDATION_HELP_TEXT
            .ToolTip = mVALIDATION_TOOL_TIP
            Select Case mVALUE_TYPE.ToUpper
                Case "String".ToUpper
                    .Type = ValidationDataType.String
                Case "Integer".ToUpper
                    .Type = ValidationDataType.Integer
                Case "Double".ToUpper
                    .Type = ValidationDataType.Double
                Case "Date".ToUpper
                    .Type = ValidationDataType.Date
                Case "Currency".ToUpper
                    .Type = ValidationDataType.Currency
            End Select
        End With
        container.Controls.Add(rev)

    End Sub

    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        Dim myCtl As TextBox = CType(sender, TextBox)
        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim container As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(container.DataItem, DataRowView)
            myCtl.Text = lRowItem(Columname).ToString()

        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then Exit Sub
            If lRowitem Is Nothing Then
                If mColumname.ToUpper = "OBJ_FIELD_ID" And mUser.OBJ_FIELD_ID <> "" Then
                    myCtl.Text = mUser.OBJ_FIELD_ID
                    mUser.OBJ_FIELD_ID = ""
                End If
                Exit Sub
            End If
            If VALUE_TYPE.ToUpper = "Date".ToUpper Then
                Dim LVal As String = lRowitem(Columname).ToString
                LVal = Format(LVal, "DD.MM.YYYY")
                myCtl.Text = LVal
            Else
                myCtl.Text = lRowitem(Columname).ToString
            End If
        End If
    End Sub 'BindData 
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property



    Private mRequiredText As String




    Private mMIN_VALUE As String

    Public Property MIN_VALUE() As String
        Get
            Return mMIN_VALUE
        End Get
        Set(ByVal value As String)
            mMIN_VALUE = value
        End Set
    End Property

    Private mMAX_VALUE As String

    Public Property MAX_VALUE() As String
        Get
            Return mMAX_VALUE
        End Get
        Set(ByVal value As String)
            mMAX_VALUE = value
        End Set
    End Property

    Private mVALUE_TYPE As String

    Public Property VALUE_TYPE() As String
        Get
            Return mVALUE_TYPE
        End Get
        Set(ByVal value As String)
            mVALUE_TYPE = value
        End Set
    End Property

    Public Property RequiredText() As String
        Get
            Return mRequiredText
        End Get
        Set(ByVal value As String)
            mRequiredText = value
        End Set
    End Property
    Private mVALIDATION_HELP_TEXT As String

    Public Property VALIDATION_HELP_TEXT() As String
        Get
            Return mVALIDATION_HELP_TEXT
        End Get
        Set(ByVal value As String)
            mVALIDATION_HELP_TEXT = value
        End Set
    End Property

    Private mVALIDATION_TOOL_TIP As String

    Public Property VALIDATION_TOOL_TIP() As String
        Get
            Return mVALIDATION_TOOL_TIP
        End Get
        Set(ByVal value As String)
            mVALIDATION_TOOL_TIP = value
        End Set
    End Property

End Class
Public Class myGenericCheckboxlist : Implements ITemplate
    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New CheckBoxList
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        myCtl.ID = mColumname
        container.Controls.Add(myCtl)
    End Sub
    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.

    Public ReadOnly Property CONTEXT()
        Get
            Return pObjCurrentUsers.CONTEXT
        End Get

    End Property

    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If


        Dim myctl As CheckBoxList = CType(sender, CheckBoxList)
        Dim mdtTruePart As DataTable = mUser.Databasemanager.MakeDataTable(mSQL_TRUE_VALUES)
        mUser.CheckBoxList = New List(Of ListItem)
        For Each r As DataRow In mdtTruePart.Rows
            Dim lItem As New ListItem
            lItem.Value = r(0).ToString
            lItem.Selected = True
            myctl.Items.Add(lItem)
            mUser.CheckBoxList.Add(lItem)
        Next
        Dim mdtFalsePart As DataTable = mUser.Databasemanager.MakeDataTable(mSQL_FALSE_VALUES)
        For Each r As DataRow In mdtFalsePart.Rows
            Dim lItem As New ListItem
            lItem.Value = r(0).ToString
            lItem.Selected = False
            myctl.Items.Add(lItem)
            mUser.CheckBoxList.Add(lItem)
        Next
        Dim mdtExistingValues As DataTable = mUser.Databasemanager.MakeDataTable("Select * from " & mTARGET_TABLE & " where " & mTARGET_KEY_FIELD & "='" & mUser.OBJ_Value & "'")
        For Each r As DataRow In mdtExistingValues.Rows
            For Each lItem As ListItem In myctl.Items
                If lItem.Text.ToUpper = r(mTARGET_VALUE_FIELD).ToString.ToUpper Then
                    lItem.Selected = True
                End If
            Next
        Next


    End Sub
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property


    Private mSQL_TRUE_VALUES As String

    Public Property SQL_TRUE_VALUES() As String
        Get
            Return mSQL_TRUE_VALUES
        End Get
        Set(ByVal value As String)
            mSQL_TRUE_VALUES = value
        End Set
    End Property

    Private mSQL_FALSE_VALUES As String

    Public Property SQL_FALSE_VALUES() As String
        Get
            Return mSQL_FALSE_VALUES
        End Get
        Set(ByVal value As String)
            mSQL_FALSE_VALUES = value
        End Set
    End Property

    Private mTARGET_TABLE As String

    Public Property TARGET_TABLE() As String
        Get
            Return mTARGET_TABLE
        End Get
        Set(ByVal value As String)
            mTARGET_TABLE = value
        End Set
    End Property

    Private mTARGET_KEY_FIELD As String

    Public Property TARGET_KEY_FIELD() As String
        Get
            Return mTARGET_KEY_FIELD
        End Get
        Set(ByVal value As String)
            mTARGET_KEY_FIELD = value
        End Set
    End Property

    Private mTARGET_VALUE_FIELD As String

    Public Property TARGET_VALUE_FIELD() As String
        Get
            Return mTARGET_VALUE_FIELD
        End Get
        Set(ByVal value As String)
            mTARGET_VALUE_FIELD = value
        End Set
    End Property

End Class
Public Class myGenericCompareValidator : Implements ITemplate
    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New TextBox
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        container.Controls.Add(myCtl)
        myCtl.ID = Columname
        myCtl.Width = pVarConstWith
        If mRequiredText Is Nothing Then mRequiredText = "REQIRED"
        Dim rfv As New RequiredFieldValidator
        With rfv
            .Text = mRequiredText
            .ControlToValidate = myCtl.ID
            .Display = ValidatorDisplay.Dynamic
            .ID = "validate" & myCtl.ID
        End With
        container.Controls.Add(rfv)
        Dim rev As New CompareValidator
        With rev
            .ToolTip = mVALIDATION_TOOL_TIP
            .ControlToCompare = mControlToCompare
            .ErrorMessage = mVALIDATION_HELP_TEXT
            .ControlToValidate = myCtl.ID
            Select Case mType.ToUpper
                Case "String".ToUpper
                    .Type = ValidationDataType.String
                Case "Integer".ToUpper
                    .Type = ValidationDataType.Integer
                Case "Double".ToUpper
                    .Type = ValidationDataType.Double
                Case "Date".ToUpper
                    .Type = ValidationDataType.Date
                Case "Currency".ToUpper
                    .Type = ValidationDataType.Currency
            End Select
            Select Case mOperator_ID.ToUpper
                Case "Equal".ToUpper
                    .Operator = ValidationCompareOperator.Equal
                Case "NotEqual".ToUpper
                    .Operator = ValidationCompareOperator.NotEqual
                Case "GreaterThan".ToUpper
                    .Operator = ValidationCompareOperator.GreaterThan
                Case "GreaterThanEqual".ToUpper
                    .Operator = ValidationCompareOperator.GreaterThanEqual
                Case "LessThan".ToUpper
                    .Operator = ValidationCompareOperator.LessThan
                Case "LessThanEqual".ToUpper
                    .Operator = ValidationCompareOperator.LessThanEqual
                Case "DataTypeCheck".ToUpper
                    .Operator = ValidationCompareOperator.DataTypeCheck
            End Select

        End With
        container.Controls.Add(rev)

    End Sub

    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        Dim myCtl As TextBox = CType(sender, TextBox)
        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim container As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(container.DataItem, DataRowView)
            myCtl.Text = lRowItem(Columname).ToString()

        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then Exit Sub
            If lRowitem Is Nothing Then
                If mColumname.ToUpper = "OBJ_FIELD_ID" And mUser.OBJ_FIELD_ID <> "" Then
                    myCtl.Text = mUser.OBJ_FIELD_ID
                    mUser.OBJ_FIELD_ID = ""
                End If
                Exit Sub
            End If
            If mType.ToUpper = "Date".ToUpper Then
                Dim LVal As Date = CDate(lRowitem(Columname).ToString)
                'LVal = Format(LVal, "DD.MM.YYYY")
                myCtl.Text = LVal
            Else
                myCtl.Text = lRowitem(Columname).ToString
            End If
        End If
    End Sub 'BindData 
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property



    Private mRequiredText As String

    Public Property RequiredText() As String
        Get
            Return mRequiredText
        End Get
        Set(ByVal value As String)
            mRequiredText = value
        End Set
    End Property



    Private mOperator_ID As String

    Public Property Operator_ID() As String
        Get
            Return mOperator_ID
        End Get
        Set(ByVal value As String)
            mOperator_ID = value
        End Set
    End Property

    Private mType As String

    Public Property Type() As String
        Get
            Return mType
        End Get
        Set(ByVal value As String)
            mType = value
        End Set
    End Property

    Private mControlToCompare As String

    Public Property ControlToCompare() As String
        Get
            Return mControlToCompare
        End Get
        Set(ByVal value As String)
            mControlToCompare = value
        End Set
    End Property

    Private mControlToValidate As String

    Public Property ControlToValidate() As String
        Get
            Return mControlToValidate
        End Get
        Set(ByVal value As String)
            mControlToValidate = value
        End Set
    End Property

    Private mVALIDATION_HELP_TEXT As String

    Public Property VALIDATION_HELP_TEXT() As String
        Get
            Return mVALIDATION_HELP_TEXT
        End Get
        Set(ByVal value As String)
            mVALIDATION_HELP_TEXT = value
        End Set
    End Property

    Private mVALIDATION_TOOL_TIP As String

    Public Property VALIDATION_TOOL_TIP() As String
        Get
            Return mVALIDATION_TOOL_TIP
        End Get
        Set(ByVal value As String)
            mVALIDATION_TOOL_TIP = value
        End Set
    End Property

End Class
Public Class myGenericMultilineTextBox : Implements ITemplate
    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New TextBox
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        myCtl.ID = Columname
        myCtl.TextMode = TextBoxMode.MultiLine
        myCtl.Width = pVarConstWith
        myCtl.Height = 100
        If mRequired Then myCtl.BackColor = Drawing.Color.Gold
        container.Controls.Add(myCtl)
        '--------------------------------------------------------------------
        ' Reference : YHHR 2027769 - INC_GBox: Required fields are not yellow
        ' Comment   : Required field validator for multiline textbox
        ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
        ' Date      : 2018-05-11
        If mRequired Then
            If mRequiredText Is Nothing Then mRequiredText = "REQUIRED"
            Dim rfv As New RequiredFieldValidator
            rfv.Text = mRequiredText
            rfv.ControlToValidate = myCtl.ID
            rfv.Display = ValidatorDisplay.Dynamic
            rfv.ID = "validate" & myCtl.ID
            myCtl.ValidationGroup = "e"
            container.Controls.Add(rfv)
        End If
        ' Reference END : YHHR 2027769
        '--------------------------------------------------------------------
    End Sub

    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.
    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        Dim myCtl As TextBox = CType(sender, TextBox)
        If TypeOf (myCtl.NamingContainer) Is GridViewRow Then
            Dim container As GridViewRow = CType(myCtl.NamingContainer, GridViewRow)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then Exit Sub
            myCtl.Text = lRowitem(Columname).ToString
        End If
        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim container As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(container.DataItem, DataRowView)
            myCtl.Text = lRowItem(Columname).ToString()
        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then
                If mColumname.ToUpper = "OBJ_FIELD_ID" And mUser.OBJ_FIELD_ID <> "" Then
                    myCtl.Text = mUser.OBJ_FIELD_ID
                    mUser.OBJ_FIELD_ID = ""
                End If
                Exit Sub
            End If
            myCtl.Text = lRowitem(Columname).ToString
        End If
    End Sub 'BindData 
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property
    Private mRequired As Boolean
    Public Property Required As Boolean
        Get
            Return mRequired
        End Get
        Set(ByVal value As Boolean)
            mRequired = value
        End Set
    End Property
    Private mRequiredText As String
    Public Property RequiredText() As String
        Get
            Return mRequiredText
        End Get
        Set(ByVal value As String)
            mRequiredText = value
        End Set
    End Property
End Class
Public Class myGenericDropdownListChild : Implements ITemplate
    Private myCtl As DropDownList
    Private mUser As myUser
    Public Sub New(ByVal lColumname As String, ByVal lUser As myUser)
        mUser = lUser
        mColumname = lColumname
    End Sub
    ' Override the ITemplate.InstantiateIn method to ensure 
    ' that the templates are created in a Control and
    ' that the Controls object's DataBinding event is associated
    ' with the BindData method.
    Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        Dim myCtl As New DropDownList
        myCtl.ID = mColumname
        myCtl.Width = pVarConstWith + 5
        AddHandler myCtl.DataBinding, AddressOf Me.BindData
        'AddHandler myCtl.SelectedIndexChanged, AddressOf Me.BerndseinEventhandler
        ' myCtl.AutoPostBack = True
        container.Controls.Add(myCtl)
    End Sub
    'Public Sub BerndseinEventhandler(ByVal sender As Object, ByVal e As EventArgs)
    '    'sender.databind()
    'End Sub
    ' Create a public method that will handle the
    ' DataBinding event called in the InstantiateIn method.

    Public ReadOnly Property CONTEXT()
        Get
            Return pObjCurrentUsers.CONTEXT
        End Get

    End Property

    Public Sub BindData(ByVal sender As Object, ByVal e As EventArgs)
        If mUser Is Nothing Then
            mUser = pObjCurrentUsers.GetCurrentUserByCwId(Context.User.Identity.Name)
        End If


        myCtl = CType(sender, DropDownList)

        Dim lTable As String = mTable
        Dim lFilter As String = mFilter
        Dim lKey As String = mKey
        If lFilter = "" Then
            lFilter = " where " & mParentName & "='" & mParentValue & "'"
        Else
            If mParentName <> "" Then
                lFilter = mFilter & " AND " & mParentName & "='" & mParentValue & "'"
            End If
        End If

        Dim lsql As String = "Select DISTINCT " & lKey & "   from " & lTable & " " & lFilter & " Order by " & lKey
        myCtl.DataSource = mUser.Databasemanager.MakeDataTable(lsql)
        myCtl.DataTextField = lKey
        myCtl.DataValueField = lKey
        myCtl.Items.Insert(0, "Choose " & mColumname)
        If TypeOf (myCtl.NamingContainer) Is GridViewRow Then
            Dim container As GridViewRow = CType(myCtl.NamingContainer, GridViewRow)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            Dim lSelectedvalue As String = lRowitem(Columname).ToString()
            If lSelectedvalue <> "" Then
                myCtl.SelectedValue = lSelectedvalue
            End If
        End If
        If TypeOf (myCtl.NamingContainer) Is DataGridItem Then
            Dim grdContainer As DataGridItem = CType(myCtl.NamingContainer, DataGridItem)
            Dim lRowItem As DataRowView = CType(grdContainer.DataItem, DataRowView)
            Dim lSelectedvalue As String = lRowItem(Columname).ToString()
            If lSelectedvalue <> "" Then
                myCtl.SelectedValue = lSelectedvalue
            End If
        End If
        If TypeOf (myCtl.NamingContainer) Is DetailsView Then
            Dim container As DetailsView = CType(myCtl.NamingContainer, DetailsView)
            Dim lRowitem As DataRowView = CType(container.DataItem, DataRowView)
            If lRowitem Is Nothing Then
                If mColumname = "OBJ_ID" And mUser.OBJ_Value <> "" And InStr(mUser.Current_OBJ.OBJ_ID.ToUpper, "Obj_FIELD".ToUpper) = 0 Then
                    myCtl.SelectedValue = mUser.OBJ_Value
                    mUser.OBJ_Value = ""
                Else

                End If
                Exit Sub
            End If
            Dim lSelectedvalue As String = lRowitem(Columname).ToString()
            If lSelectedvalue <> "" Then
                myCtl.SelectedValue = lSelectedvalue
            End If
        End If
    End Sub
    Private mColumname As String
    Public Property Columname() As String
        Get
            Return mColumname
        End Get
        Set(ByVal value As String)
            mColumname = value
        End Set
    End Property

    Private mLookUpSql As String = "Select * from Subgroup"

    Public Property LookUpSql() As String
        Get
            Return mLookUpSql
        End Get
        Set(ByVal value As String)
            mLookUpSql = value
        End Set
    End Property

    Private mTable As String = "Subgroup"

    Public Property Table() As String
        Get
            Return mTable
        End Get
        Set(ByVal value As String)
            mTable = value
        End Set
    End Property

    Private mFilter As String

    Public Property Filter() As String
        Get
            Return mFilter
        End Get
        Set(ByVal value As String)
            mFilter = value
        End Set
    End Property

    Private mKey As String = "Subgroup_ID"

    Public Property Key() As String
        Get
            Return mKey
        End Get
        Set(ByVal value As String)
            mKey = value
        End Set
    End Property

    Private mDescription As String = "TEXT"

    Public Property Description() As String
        Get
            Return mDescription
        End Get
        Set(ByVal value As String)
            mDescription = value
        End Set
    End Property
    Public ReadOnly Property Current() As DropDownList
        Get
            Return myCtl
        End Get
    End Property

    Private mParentName As String

    Public Property ParentName() As String
        Get
            Return mParentName
        End Get
        Set(ByVal value As String)
            mParentName = value
        End Set
    End Property

    Private mParentValue As String

    Public Property ParentValue() As String
        Get
            Return mParentValue
        End Get
        Set(ByVal value As String)
            mParentValue = value
        End Set
    End Property

    Private mDisplayname As String

    Public Property Displayname() As String
        Get
            Return mDisplayname
        End Get
        Set(ByVal value As String)
            mDisplayname = value
        End Set
    End Property

End Class
