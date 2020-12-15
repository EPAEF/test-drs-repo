Option Strict Off
Public Class Dynamic_View_Controller_Factory
    Implements IDynamic_View_Controller_Factory
    Private mUser As myUser
    Private mIsPostback As Boolean
    Private mRequest As HttpRequest
    Private mTOPIC_ID As String
    Private mTOPIC_GROUP_ID As String
    Private mTOPIC_GROUP_CONTEXT_ID As String
    Private mErrText As String
    Public Event ErrorMessage(ByVal lErrorText As String) Implements IDynamic_View_Controller_Factory.ErrorMessage
    Public Event Impersonate(ByVal lCw_ID As String) Implements IDynamic_View_Controller_Factory.Impersonate
    Public Event LoadWizzardData(ByVal lTopic As String) Implements IDynamic_View_Controller_Factory.LoadWizzardData
    Public Event TableChange(ByVal lstrTablename As String) Implements IDynamic_View_Controller_Factory.TableChange
    Public Property IsPostback() As Boolean Implements IDynamic_View_Controller_Factory.IsPostback
        Get
            Return mIsPostback
        End Get
        Set(ByVal value As Boolean)
            mIsPostback = value
        End Set
    End Property
    Public Property Request() As HttpRequest Implements IDynamic_View_Controller_Factory.Request
        Get
            Return mRequest
        End Get
        Set(ByVal value As HttpRequest)
            mRequest = value
        End Set
    End Property
    Public Function GetUser(ByVal lstrCWID As String) As myUser Implements IDynamic_View_Controller_Factory.GetUser
        mUser = pObjCurrentUsers.GetCurrentUserByCwId(lstrCWID)
        Return mUser
    End Function
    Private mTree As TreeView
    Public Sub LoadTree(ByRef ltrvOBJ As System.Web.UI.WebControls.TreeView) Implements IDynamic_View_Controller_Factory.LoadTree
        Try
            mTree = ltrvOBJ

            ltrvOBJ.Nodes.Clear()
            RaiseEvent Impersonate(mUser.CW_ID)

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035648 - GBOX WebForms: RENAME THE COLUMN AND REMOVE ONE COLUMN FROM DB AND CODE
            ' Comment           : a) This request is for renaming column TOPIC_OBJ_OBJS_CLASSIFICATION_ID to: TREE_ICON and
            '                   : b) Also to remove [TOPIC_OBJ_OBJS_CLASSIFICATION_OBJ_ID] from table TOPIC_OBJ_OBJS and code as well. 
            '                     Both a and b changes should have no impact in any other part of the code.
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-11-24

            'lInsertIntoTopicObj_OBJs &= " ,'" & r("TOPIC_OBJ_OBJS_CLASSIFICATION_OBJ_ID").ToString & "'"
            Dim lSql As String = "Select OBJ.OBJ_DISPLAY_NAME, OBJ.OBJ_ID,OBJ_DESCRIPTION,OBJ_TABLENAME,[Rank],Topic_OBJ_OBJS.Topic_Id,Topic_OBJ_OBJS.TREE_ICON from OBJ"
            ' Reference  END    : CR ZHHR 1035648 


            lSql = lSql & " left Join Topic_OBJ_OBJS On OBJ.OBJ_ID=Topic_OBJ_OBJS.CHILD_OBJ_ID  "

            lSql = lSql & " Where (Topic_OBJ_OBJS.Topic_ID='" & mTOPIC_ID & "' "
            lSql = lSql & " And Topic_OBJ_OBJS.TOPIC_GROUP_CONTEXT_ID='" & mTOPIC_GROUP_CONTEXT_ID & "'"
            lSql = lSql & " And Topic_OBJ_OBJS.TOPIC_GROUP_ID='" & mTOPIC_GROUP_ID & "' AND PARENT_OBJ_ID='ROOT' )order by Rank"
            Dim lOBJ As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            If lOBJ Is Nothing Then
                mErrText &= mUser.Databasemanager.ErrText
                RaiseEvent ErrorMessage(mErrText)
                Exit Sub
            End If
            If lOBJ.Rows.Count = 0 Then
                mErrText &= "Customize Topic_OBJ_OBJS AND CHILD_OBJ_ID in TOPIC_OBJ_OBJS"
                RaiseEvent ErrorMessage(mErrText)
                Exit Sub
            End If
            Dim lNode As New TreeNode
            For Each r As DataRow In lOBJ.Rows
                lNode = New TreeNode(r("OBJ_DISPLAY_NAME").ToString)
                lNode.ToolTip = r("OBJ_DESCRIPTION").ToString
                lNode.Value = r("OBJ_ID").ToString


                '---------------------------------------------------------------------------------------------------
                ' Reference         : CR ZHHR 1035648 - GBOX WebForms: RENAME THE COLUMN AND REMOVE ONE COLUMN FROM DB AND CODE
                ' Comment           : This request is for renaming column TOPIC_OBJ_OBJS_CLASSIFICATION_ID to: TREE_ICON and
                '                     it should have no impact in any other part of the code.
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 2014-11-24

                Select Case r("TREE_ICON").ToString

                    ' Reference  END    : CR ZHHR 1035648 

                    Case "WRITE"
                        lNode.ImageUrl = "~/Images/application_s_gr.gif"
                    Case "READ"
                        lNode.ImageUrl = "~/Images/application_s.gif"
                End Select


                setMandatoryType(lNode, r("OBJ_ID").ToString)
                setPMD(lNode, r("OBJ_ID").ToString)
                Dim lcheckSubscriptionSql As String = "Select * from  OBJ_SUBSCRIPTION where OBJ_ID='" & lNode.Value & "' and CW_ID='" & mUser.CW_ID & "'"
                Dim lCheckdt As DataTable = mUser.Databasemanager.MakeDataTable(lcheckSubscriptionSql)
                If lCheckdt.Rows.Count <> 0 Then
                    lNode.ImageUrl = "~/Images/bell.png"
                End If
                RaiseEvent TableChange(r("OBJ_TABLENAME").ToString)
                If Not Me.IsPostback Then
                    If Not Request Is Nothing Then
                        If Not Request.Params("OBJECT") Is Nothing Then
                            If lNode.Text.ToUpper = Request.Params("OBJECT").ToString.ToUpper Then
                                lNode.Selected = True
                                lNode.Expanded = True
                            End If
                        End If
                    End If
                End If
                ltrvOBJ.Nodes.Add(lNode)
                If Not Request.Params("PATH") Is Nothing Then
                    Dim lPath As Array = Request.Params("PATH").ToString.ToUpper.Split("/")
                    Dim lGetchilds As Boolean = False
                    For i = 0 To lPath.GetUpperBound(0) - 1
                        If lPath(i) = lNode.Value.ToUpper Then lGetchilds = True
                    Next
                    '-------------------------------------------------------------------------------------------
                    ' Reference         : CR ZHHR 1060747 - GBOX COC: error in tree
                    ' Comment           : When the tree is loaded directly called from URL, expand the tree node
                    ' Added by          : Pratyusa Lenka (CWID : EOJCG)
                    ' Date              : 2016-08-10
                    If lPath.Length = 1 Then
                        If lPath(0) = lNode.Value.ToUpper Then lGetchilds = True
                    End If
                    ' Reference  END    : CR ZHHR 1060747
                    If lGetchilds Then
                        lNode.Selected = True
                        lNode.Expanded = True
                        GetChilds(lNode)
                    End If
                End If
            Next
            RaiseEvent LoadWizzardData(lNode.Text)
        Catch ex As Exception
            mErrText &= "Dynamicform_Loadtree:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            RaiseEvent ErrorMessage(mErrText)
        End Try
    End Sub
    Public Sub setPMD(ByRef lNode As TreeNode, ByVal lobj_ID As String)
        '---------------------------------------------------------------------------------------------------
        ' Reference         : ZHHR 1045115 - GBOX COC OTT 1070: GBOX 3.0 Multi-system implementation (pMDAS list)
        ' Comment           : Change name of the Table Application_Obj to Obj_Application
        ' Added by          : Milind Randive (CWID : EOJCH)
        ' Date              : 2015-07-13
        Dim lsql As String = " Select Count(*) as Result FROM OBJ_APPLICATION OBJ " & _
                             " INNER JOIN APPLICATION APP ON OBJ.APPLICATION_ID = APP.APPLICATION_ID " & _
                             " WHERE OBJ.OBJ_ID ='" & lobj_ID & "' AND  APP.APPLICATION_ID='PMDM140'"
        'Dim lsql As String = "Select Count(*) as Result from OBJ_APPLICATION where OBJ_ID='" & lobj_ID & "' AND APPLICATION_ID='PMDM140'"
        ' Reference End     : ZHHR 1045115 
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lsql)
        If ldt.Rows(0)("Result") = 1 Then
            lNode.ImageUrl = "~/Images/optional.png"
            lNode.ToolTip = "MDRS=YES"
        End If
    End Sub

    Public Sub setMandatoryType(ByRef lNode As TreeNode, ByVal lobj_ID As String)


        Dim lMandatorySql As String = ""
        Dim ldtManda As DataTable = Nothing
        lMandatorySql = "SELECT  MANDATORY_TYPE, OBJ_CPS.LOCKED "
        lMandatorySql &= " FROM TOPIC_OBJ_OBJS"
        lMandatorySql &= " Left join OBJ_CPS_ATTR on [TOPIC_OBJ_OBJS].CHILD_OBJ_ID = OBJ_CPS_ATTR.OBJ_GUID"
        lMandatorySql &= " left join OBJ_CPS on OBJ_CPS.OBJ_CPS_ID= OBJ_CPS_ATTR.OBJ_CPS_ID"
        lMandatorySql &= " where [CHILD_OBJ_ID]='" & lobj_ID & "' "
        ldtManda = mUser.Databasemanager.MakeDataTable(lMandatorySql)
        If Not ldtManda Is Nothing Then
            If ldtManda.Rows.Count <> 0 Then
                ' if locked ist voll
                '  If lobj_ID = "b5d4e93d-4ad0-4eda-9c39-a7abeb057ad6" Then Stop
                If ldtManda.Rows(0)("LOCKED").ToString <> "" Then
                    lMandatorySql = "SELECT TOP(1) MANDATORY_TYPE "
                    lMandatorySql &= " FROM TOPIC_OBJ_OBJS"
                    lMandatorySql &= " left join obj_cps_attr on obj_cps_attr.obj_guid = TOPIC_OBJ_OBJS.[CHILD_OBJ_ID] left join OBJ_CPS_HISTORY on obj_cps_attr.OBJ_CPS_ID= OBJ_CPS_HISTORY.OBJ_CPS_ID "
                    lMandatorySql &= " where [CHILD_OBJ_ID]='" & lobj_ID & "' order by OBJ_CPS_HISTORY.OBJ_VERSIONNUMBER DESC"
                Else
                    lMandatorySql = "SELECT  MANDATORY_TYPE, OBJ_CPS_ATTR.LOCKED "
                    lMandatorySql &= " FROM TOPIC_OBJ_OBJS"
                    lMandatorySql &= " Left join OBJ_CPS_ATTR on [TOPIC_OBJ_OBJS].CHILD_OBJ_ID = OBJ_CPS_ATTR.OBJ_GUID"
                    lMandatorySql &= " left join OBJ_CPS on OBJ_CPS.OBJ_CPS_ID= OBJ_CPS_ATTR.OBJ_CPS_ID"
                    lMandatorySql &= " where [CHILD_OBJ_ID]='" & lobj_ID & "' "
                End If


                ldtManda = mUser.Databasemanager.MakeDataTable(lMandatorySql)

                Dim lmanda As String = ldtManda.Rows(0)("MANDATORY_TYPE").ToString.ToUpper
                Select Case lmanda
                    Case "mandatory setting".ToUpper
                        lNode.ImageUrl = "~/Images/mandantory_setting.png"
                    Case "nc".ToUpper
                        lNode.ImageUrl = "~/Images/nc.png"
                    Case "bomb".ToUpper
                        lNode.ImageUrl = "~/Images/BOMB.png"
                    Case "fb".ToUpper
                        lNode.ImageUrl = "~/Images/fb.png"
                    Case "Draft".ToUpper
                        lNode.ImageUrl = "~/Images/DRAFT.png"
                    Case "Optional".ToUpper
                        lNode.ImageUrl = "~/Images/optional.png"
                    Case Else
                        lNode.ImageUrl = ""
                End Select

                lMandatorySql = "SELECT  MANDATORY_TYPE, OBJ_CPS_ATTR.LOCKED "
                lMandatorySql &= " FROM TOPIC_OBJ_OBJS"
                lMandatorySql &= " Left join OBJ_CPS_ATTR on [TOPIC_OBJ_OBJS].CHILD_OBJ_ID = OBJ_CPS_ATTR.OBJ_GUID"
                lMandatorySql &= " left join OBJ_CPS on OBJ_CPS.OBJ_CPS_ID= OBJ_CPS_ATTR.OBJ_CPS_ID"
                lMandatorySql &= " where [CHILD_OBJ_ID]='" & lobj_ID & "' "
                ldtManda = mUser.Databasemanager.MakeDataTable(lMandatorySql)
                If ldtManda.Rows(0)("LOCKED").ToString <> "" Then
                    lNode.ImageUrl = "~/Images/cancel__16.gif"
                    lNode.ToolTip = "locked by request: " & ldtManda.Rows(0)("LOCKED").ToString
                    lNode.Text = lNode.Text & " (locked)"
                End If
            End If
        End If
    End Sub
    Public Sub GetChilds(ByVal lNode As TreeNode)
        Try

            '---------------------------------------------------------------------------------------------------
            ' Reference         : CR ZHHR 1035648 - GBOX WebForms: RENAME THE COLUMN AND REMOVE ONE COLUMN FROM DB AND CODE
            ' Comment           : a) This request is for renaming column TOPIC_OBJ_OBJS_CLASSIFICATION_ID to: TREE_ICON and
            '                   : b) Also to remove [TOPIC_OBJ_OBJS_CLASSIFICATION_OBJ_ID] from table TOPIC_OBJ_OBJS and code as well. 
            '                     Both a and b changes should have no impact in any other part of the code.
            ' Added by          : Milind Randive (CWID : EOJCH)
            ' Date              : 2014-11-24

            Dim lSql As String = "Select OBJ.OBJ_DISPLAY_NAME, OBJ.OBJ_ID,OBJ_DESCRIPTION,OBJ_TABLENAME,[Rank],Topic_OBJ_OBJS.Topic_Id, Topic_OBJ_OBJS.CHILD_OBJ_ID, Topic_OBJ_OBJS.TREE_ICON from OBJ"

            ' Reference  END    : CR ZHHR 1035648 


            lSql = lSql & " left Join Topic_OBJ_OBJS On OBJ.OBJ_ID=Topic_OBJ_OBJS.CHILD_OBJ_ID  "
            lSql = lSql & " Where Topic_OBJ_OBJS.Topic_ID='" & mTOPIC_ID & "' and PARENT_OBJ_ID ='" & lNode.Value & "'"
            lSql = lSql & " And Topic_OBJ_OBJS.TOPIC_GROUP_CONTEXT_ID='" & mTOPIC_GROUP_CONTEXT_ID & "'"
            lSql = lSql & " Order by Rank"
            '---------------------------------------------------------------------------
            ' Reference : CR ZHHR 1060685 - GBOX REP: OTT 3229 - Reporting authorization
            ' Comment   : Check if mUser object is nothing
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-08-12
            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
            End If
            ' Reference END : CR ZHHR 1060685
            '---------------------------------------------------------------------------
            Dim lOBJ As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            For Each r As DataRow In lOBJ.Rows
                Dim lChildNode As New TreeNode(r("OBJ_DISPLAY_NAME").ToString) ' & " (" & r("OBJ_DESCRIPTION").ToString & ")")
                lChildNode.Value = r("CHILD_OBJ_ID").ToString
                lChildNode.ToolTip = r("OBJ_DESCRIPTION").ToString


                '---------------------------------------------------------------------------------------------------
                ' Reference         : CR ZHHR 1035648 - GBOX WebForms: RENAME THE COLUMN AND REMOVE ONE COLUMN FROM DB AND CODE
                ' Comment           : This request is for renaming column TOPIC_OBJ_OBJS_CLASSIFICATION_ID to: TREE_ICON and
                '                     and it should have no impact in any other part of the code.
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 2014-11-24

                Select Case r("TREE_ICON").ToString

                    ' Reference  END    : CR ZHHR 1035648 

                    Case "WRITE"
                        lChildNode.ImageUrl = "~/Images/application_s_gr.gif"
                    Case "READ"
                        lChildNode.ImageUrl = "~/Images/application_s.gif"
                End Select

                setMandatoryType(lChildNode, r("CHILD_OBJ_ID").ToString)
                setPMD(lChildNode, r("CHILD_OBJ_ID").ToString)
                Dim lcheckSubscriptionSql As String = "Select * from  OBJ_SUBSCRIPTION where OBJ_ID='" & lChildNode.Value & "' and CW_ID='" & mUser.CW_ID & "'"
                Dim lCheckdt As DataTable = mUser.Databasemanager.MakeDataTable(lcheckSubscriptionSql)
                If lCheckdt.Rows.Count <> 0 Then
                    lChildNode.ImageUrl = "~/Images/bell.png"
                End If
                lNode.ChildNodes.Add(lChildNode)
                If Not Me.IsPostback Then
                    If Not Request Is Nothing Then
                        If Not Request.Params("PATH") Is Nothing Then
                            Dim myPathObj As Array = Request.Params("PATH").ToString.ToUpper.Split("/")
                            Dim lposInPath As Integer = 0
                            For i = 0 To myPathObj.GetUpperBound(0)
                                If lChildNode.Value.ToUpper = myPathObj(i).ToString Then
                                    lposInPath = i
                                End If
                            Next
                            Dim lIs As Boolean = False
                            Dim lCurrentnode As TreeNode = lChildNode
                            For i = 1 To lposInPath
                                If InStr(Request.Params("PATH").ToString.ToUpper, lCurrentnode.Parent.Value.ToUpper) <> 0 Then
                                    lCurrentnode = lCurrentnode.Parent
                                    lCurrentnode.Expand()
                                    lIs = True
                                Else
                                    lIs = False
                                    Exit For
                                End If
                            Next
                            If lIs Then
                                lChildNode.Selected = True
                                lChildNode.Expanded = True
                                lCurrentnode.Expand()
                                lChildNode.ExpandAll()
                            End If
                        End If
                    End If
                End If
                RaiseEvent TableChange(r("OBJ_TABLENAME").ToString)

                '---------------------------------------------------------------------------------------------------
                ' Reference         : CR ZHHR 1035648 - GBOX WebForms: RENAME THE COLUMN AND REMOVE ONE COLUMN FROM DB AND CODE
                ' Comment           : This request is for renaming column TOPIC_OBJ_OBJS_CLASSIFICATION_ID to: TREE_ICON but below 
                '                   : commented this column is already commented. so i have not done any changes in below line and just tagged.
                ' Added by          : Milind Randive (CWID : EOJCH)
                ' Date              : 2014-11-24

                'If r("TOPIC_OBJ_OBJS_CLASSIFICATION_ID").ToString = "READ" Then

                ' Reference  END    : CR ZHHR 1035648 
            
                GetChilds(lChildNode)
                'End If
            Next
        Catch ex As Exception
            mErrText &= "Getchilds:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            RaiseEvent ErrorMessage(mErrText)
        End Try
    End Sub
    Public Sub GetChilds(ByVal lNode As TreeNode, ByVal mnunavigate As String)
        Try

            Dim lSql As String = "Select OBJ.OBJ_DISPLAY_NAME, OBJ.OBJ_ID,OBJ_DESCRIPTION,OBJ_TABLENAME,[Rank],Topic_OBJ_OBJS.Topic_Id, Topic_OBJ_OBJS.CHILD_OBJ_ID, Topic_OBJ_OBJS.TREE_ICON from OBJ"

            lSql = lSql & " left Join Topic_OBJ_OBJS On OBJ.OBJ_ID=Topic_OBJ_OBJS.CHILD_OBJ_ID  "
            lSql = lSql & " Where Topic_OBJ_OBJS.Topic_ID='" & mTOPIC_ID & "' and PARENT_OBJ_ID ='" & lNode.Value & "'"
            lSql = lSql & " And Topic_OBJ_OBJS.TOPIC_GROUP_CONTEXT_ID='" & mTOPIC_GROUP_CONTEXT_ID & "'"
            lSql = lSql & " Order by Rank"

            If mUser Is Nothing Then
                mUser = pObjCurrentUsers.GetCurrentUserByCwId(CONTEXT.User.Identity.Name)
            End If

            Dim lOBJ As DataTable = mUser.Databasemanager.MakeDataTable(lSql)
            For Each r As DataRow In lOBJ.Rows
                Dim lChildNode As New TreeNode(r("OBJ_DISPLAY_NAME").ToString) ' & " (" & r("OBJ_DESCRIPTION").ToString & ")")
                lChildNode.Value = r("CHILD_OBJ_ID").ToString
                lChildNode.ToolTip = r("OBJ_DESCRIPTION").ToString

                Select Case r("TREE_ICON").ToString

                    Case "WRITE"
                        lChildNode.ImageUrl = "~/Images/application_s_gr.gif"
                    Case "READ"
                        lChildNode.ImageUrl = "~/Images/application_s.gif"
                End Select

                setMandatoryType(lChildNode, r("CHILD_OBJ_ID").ToString)
                setPMD(lChildNode, r("CHILD_OBJ_ID").ToString)
                Dim lcheckSubscriptionSql As String = "Select * from  OBJ_SUBSCRIPTION where OBJ_ID='" & lChildNode.Value & "' and CW_ID='" & mUser.CW_ID & "'"
                Dim lCheckdt As DataTable = mUser.Databasemanager.MakeDataTable(lcheckSubscriptionSql)
                If lCheckdt.Rows.Count <> 0 Then
                    lChildNode.ImageUrl = "~/Images/bell.png"
                End If
                lNode.ChildNodes.Add(lChildNode)
                If Not Me.IsPostback Then
                    If Not Request Is Nothing Then
                        If Not Request.Params("PATH") Is Nothing Then
                            Dim myPathObj As Array = Request.Params("PATH").ToString.ToUpper.Split("/")
                            Dim lposInPath As Integer = 0
                            For i = 0 To myPathObj.GetUpperBound(0)
                                If lChildNode.Value.ToUpper = myPathObj(i).ToString Then
                                    lposInPath = i
                                End If
                            Next
                            Dim lIs As Boolean = False
                            Dim lCurrentnode As TreeNode = lChildNode
                            For i = 1 To lposInPath
                                If InStr(Request.Params("PATH").ToString.ToUpper, lCurrentnode.Parent.Value.ToUpper) <> 0 Then
                                    lCurrentnode = lCurrentnode.Parent
                                    lCurrentnode.Expand()
                                    lIs = True
                                Else
                                    lIs = False
                                    Exit For
                                End If
                            Next
                            If lIs Then
                                lChildNode.Selected = True
                                lChildNode.Expanded = True
                                lCurrentnode.Expand()
                                lChildNode.ExpandAll()
                            End If
                        End If
                    End If
                End If
                RaiseEvent TableChange(r("OBJ_TABLENAME").ToString)

                If (mnunavigate = "CPS") Then
                Else
                    GetChilds(lChildNode)
                End If


                ' GetChilds(lChildNode)
                'End If
            Next
        Catch ex As Exception
            mErrText &= "Getchilds:" & ex.Message & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString & vbCrLf & mUser.Databasemanager.ErrText & vbCrLf & mUser.GBOXmanager.ErrString
            RaiseEvent ErrorMessage(mErrText)
        End Try
    End Sub
    Function SelectedNodeChange(ByVal lSelectedNode As TreeNode) As IDynamic_View_Controller Implements IDynamic_View_Controller_Factory.SelectedNodeChange
        Try
            Dim lObj As myOBJ
            If lSelectedNode Is Nothing Then
                If mUser.Current_OBJ Is Nothing Then
                    Return Nothing
                Else
                    lObj = mUser.Current_OBJ
                End If
            Else
                lObj = mUser.Current_OBJ
            End If
            Dim lControler As IDynamic_View_Controller
            '---------------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
            ' Comment   : New concept for multiple text functionality
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2015-03-24
            '--------------------------------------------------------------------------------
            ' Reference : CR ZHHR 1052471 - OTT 1806 - GBOX: delete COMPOSITE_S_T object type
            ' Comment   : Delete COMPOSITE_S_T object type
            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
            ' Date      : 2016-01-25
            If lObj.OBJ_CLASSIFICATION_ID.ToUpper <> "COMPOSITE_S_T_TXT" And lObj.OBJ_CLASSIFICATION_ID.ToUpper <> "COMPOSITE_MULTI_TXT" Then
                mUser.EditMode = False
            End If
            ' Reference END : CR ZHHR 1038241
            '----------------------------------------------------------------------------------------
            mUser.Databasemanager.Request = Me.Request
            Select Case lObj.OBJ_CLASSIFICATION_ID.ToUpper
                Case "ADMINISTRATION"
                    lControler = New Dynamic_View_Requester(mUser)
                Case "COMPOSITE_S_T_TXT"
                    lControler = New Dynamic_View_Requester(mUser)
                Case "SINGLE"
                    lControler = New Dynamic_View_Requester(mUser)
                    '---------------------------------------------------------------------------------------
                    ' Reference : CR ZHHR 1038241 - GBOX COC: New concept for multiple text functionality
                    ' Comment   : New concept for multiple text functionality
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2015-03-24
                Case "COMPOSITE_MULTI_TXT"
                    lControler = New Dynamic_View_Requester(mUser)
                    ' Reference END : CR ZHHR 1038241
                    '----------------------------------------------------------------------------------------
                Case "QUERY"
                    mUser.Query = True
                    lControler = New Dynamic_View_Reporter(mUser)
                Case "DOCUMENTATION"
                    lControler = New Dynamic_View_Documentator(mUser)
                Case "TRANSACTION"
                    lControler = New Dynamic_View_Documentator(mUser)
                Case "CUSTOMIZING"
                    lControler = New Dynamic_View_Documentator(mUser)
                Case Else
                    lControler = Nothing
            End Select
            ' Reference END : CR ZHHR 1052471
            '--------------------------------------------------------------------------------
            Return lControler
        Catch ex As Exception
            mErrText &= "SelectedNodeChange:" & ex.Message
            RaiseEvent ErrorMessage(mErrText)
            Return Nothing
        End Try
    End Function
    Public Sub ErrorInfo(ByVal lErrString As String) Implements IDynamic_View_Controller_Factory.ErrorInfo
        RaiseEvent ErrorMessage(lErrString)
    End Sub
    Public ReadOnly Property CONTEXT()
        Get
            Return pObjCurrentUsers.CONTEXT
        End Get
    End Property
    Public Property TOPIC_ID As String Implements IDynamic_View_Controller_Factory.TOPIC_ID
        Get
            Return mTOPIC_ID
        End Get
        Set(ByVal value As String)
            mTOPIC_ID = value
        End Set
    End Property
    Public Property TOPIC_GROUP_ID As String Implements IDynamic_View_Controller_Factory.TOPIC_GROUP_ID
        Get
            Return mTOPIC_GROUP_ID
        End Get
        Set(ByVal value As String)
            mTOPIC_GROUP_ID = value
        End Set
    End Property
    Public Property TOPIC_GROUP_CONTEXT_ID As String Implements IDynamic_View_Controller_Factory.TOPIC_GROUP_CONTEXT_ID
        Get
            Return mTOPIC_GROUP_CONTEXT_ID
        End Get
        Set(ByVal value As String)
            mTOPIC_GROUP_CONTEXT_ID = value
        End Set
    End Property
    Public Property User As myUser
        Get
            Return mUser
        End Get
        Set(ByVal value As myUser)
            mUser = value
        End Set
    End Property
End Class
