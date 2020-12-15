Option Strict Off
Public Class myTemplateFactory
    Private mUser As myUser
    Public Sub New(ByVal lUser As myUser)
        mUser = lUser
    End Sub

    Public Function MakeEditItemTemplate(ByVal lFieldname As String, ByVal lFieldType As String, ByVal lDisplayName As String, ByVal lRequired As Boolean, ByVal lRequiredText As String, Optional ByVal lCopyRequest As Boolean = False) As ITemplate
        Dim lEditItemTemplate As ITemplate = Nothing
        Try
            Dim lColumn As String = lDisplayName
            Dim lIsKey As Boolean = False
            If Not mUser.GBOXmanager.KeyCollection Is Nothing Then
                For Each lKey As myKeyObj In mUser.GBOXmanager.KeyCollection
                    If lKey.Displayname = lDisplayName Then
                        lIsKey = True
                        ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
                        ' Comment   : Make the key fields non-editable in case of update or copy
                        ' Added by  : Pratyusa Lenka (CWID : EOJCG)
                        ' Date      : 2019-04-01
                        If mUser.RequestType = myUser.RequestTypeOption.update Or lCopyRequest Then
                            lFieldType = "DISPLAY"
                        End If
                    End If
                Next
            End If
            Select Case lFieldType.ToUpper
                Case "CHECKBOX"
                    lEditItemTemplate = New myGenericCheckBox(lColumn, mUser)
                Case "DISPLAY"
                    lEditItemTemplate = New myGenericLiteral(lColumn)
                Case "VERSIONNUMBER"
                    lEditItemTemplate = New myGenericLiteral(lColumn)
                Case "INPUT"
                    If lRequired = True Then
                        Dim ddl As New myGenericRequiredField(lColumn, mUser)
                        With ddl
                            .RequiredText = lRequiredText
                            lEditItemTemplate = ddl
                        End With
                    Else
                        lEditItemTemplate = New myGenericTextBox(lColumn, mUser)
                    End If
                Case "INPUTMULTILINE"
                    '--------------------------------------------------------------------
                    ' Reference : YHHR 2027769 - INC_GBox: Required fields are not yellow
                    ' Comment   : Assign the required and required text properties values
                    ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                    ' Date      : 2018-05-11
                    If lRequired = True Then
                        Dim ddl As New myGenericMultilineTextBox(lColumn, mUser)
                        With ddl
                            .Required = lRequired
                            .RequiredText = lRequiredText
                            lEditItemTemplate = ddl
                        End With
                    Else
                        lEditItemTemplate = New myGenericMultilineTextBox(lColumn, mUser)
                    End If
                    ' Reference END : YHHR 2027769
                    '--------------------------------------------------------------------
                Case "INPUT_VALIDATE_EXPRESSION"

                    Dim lDatarow As DataRow = GetdataRow(lFieldname, "OBJ_FIELD_EXPRESSION_VALIDATION")
                    If lDatarow Is Nothing Then mErrText &= "Customize " & lFieldname & " in OBJ_FIELD_EXPRESSION_VALIDATION  Fieldtype:" & lFieldType & vbCrLf
                    Dim ddl As New myGenericRegularExpressionValidator(lColumn, mUser)
                    With ddl
                        .Required = lRequired
                        .RequiredText = lRequiredText
                        .VALIDATION_EXPRESSION = lDatarow("VALIDATION_EXPRESSION").ToString
                        .VALIDATION_HELP_TEXT = lDatarow("VALIDATION_HELP_TEXT").ToString
                        .VALIDATION_TOOL_TIP = lDatarow("VALIDATION_TOOL_TIP").ToString
                    End With
                    lEditItemTemplate = ddl
                Case "LOOKUP"
                    Dim lDatarow As DataRow = GetdataRow(lFieldname, "OBJ_FIELD_LOOKUP_VALIDATON")
                    If lDatarow Is Nothing Then

                        mErrText &= mErrText & "Customize " & lFieldname & " in OBJ_FIELD_LOOKUP_VALIDATON  Fieldtype:" & lFieldType & vbCrLf
                    Else
                        Dim ddl As New myGenericDropdownList(lColumn, mUser)
                        With ddl
                            '---------------------------------------------------------------------------------
                            ' EOJCH     : 26-Jul-2017 - This development was in Q test , discussed with EOCJG and deployed the changes on production with reference to IM0004719180.
                            ' Reference : CR ZHHR 1059522 - GBOX COC: OTT 2278 - Cascaded Lookup Functionality
                            ' Comment   : Assign the required and required text properties values
                            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                            ' Date      : 2016-09-19
                            .Required = lRequired
                            .RequiredText = lRequiredText
                            ' Reference END : CR ZHHR 1059522
                            '---------------------------------------------------------------------------------

                            .Key = lDatarow("Lookup_Table_Key").ToString
                            .Table = lDatarow("Lookup_Table_Name").ToString
                            .Description = lDatarow("Lookup_Table_Description").ToString
                            .Filter = lDatarow("Lookup_Table_Filter").ToString
                        End With
                        lEditItemTemplate = ddl
                    End If
                Case "LOOKUP_LIST"
                    Dim lDatarow As DataRow = GetdataRow(lFieldname, "OBJ_FIELD_LOOKUP_LIST_VALIDATON")
                    If lDatarow Is Nothing Then
                        mErrText &= mErrText & "Customize " & lFieldname & " in OBJ_FIELD_LOOKUP_LIST_VALIDATON  Fieldtype:" & lFieldType & vbCrLf
                    Else
                        Dim ddl As New myGenericSmallDropdownList(lColumn, mUser)
                        With ddl
                            '-------------------------------------------------------------------------------------
                            ' Reference : CR ZHHR 1062011 - GBOX COC: OTT 3439 - Enhance LOOKUP_LIST functionality
                            ' Comment   : Assign the required and required text properties values
                            ' Added by  : Pratyusa Lenka (CWID : EOJCG) 
                            ' Date      : 2016-09-15
                            .Required = lRequired
                            .RequiredText = lRequiredText
                            ' Reference END : CR ZHHR 1062011
                            '-------------------------------------------------------------------------------------
                            .LookUpString = lDatarow("VALUE_LIST").ToString
                        End With
                        lEditItemTemplate = ddl
                    End If
                Case "RESTRICTED_ACCESS"
                    lEditItemTemplate = New myGenericLiteral(lColumn)
                Case "SIGNATURE"
                    lEditItemTemplate = New myGenericLiteral(lColumn)
                Case "TIMESTAMP"
                    lEditItemTemplate = New myGenericLiteral(lColumn)
            End Select

            Return lEditItemTemplate
        Catch ex As Exception
            mErrText &= "MakeEditItemTemplate" & mErrText & vbCrLf & ex.Message
            Return Nothing
        End Try
    End Function

    Public Function MakeEditItemTemplate(ByVal objFieldsRow As DataRow) As ITemplate
        Return MakeEditItemTemplate(objFieldsRow("OBJ_FIELD_ID").ToString, objFieldsRow("OBJ_Field_Type_ID").ToString, objFieldsRow("DISPLAY_NAME").ToString, CBool(objFieldsRow("Required").ToString), objFieldsRow("RequiredText").ToString)
    End Function
    Public Function DynamicField(ByVal lFieldname As String, ByVal lFiedType As String, ByVal lDisplayName As String, ByVal lRequired As Boolean, ByVal lrequiredText As String) As TemplateField

        Dim lColumn As String = lDisplayName
        Dim lGenericField As New TemplateField
        lGenericField.HeaderText = lDisplayName
        lGenericField.ItemTemplate = New myGenericLiteral(lColumn)
        lGenericField.EditItemTemplate = MakeEditItemTemplate(lFieldname, lFiedType, lDisplayName, lRequired, lrequiredText)
        Return lGenericField
    End Function
    Public Function DynamicField(ByVal objFieldsRow As DataRow, Optional ByVal lCopyRequest As Boolean = False) As TemplateField
        Dim lColumn As String = objFieldsRow("DISPLAY_NAME").ToString
        Dim lGenericField As New TemplateField
        lGenericField.HeaderText = objFieldsRow("DISPLAY_NAME").ToString
        If objFieldsRow("OBJ_Field_Type_ID").ToString <> "CHECKBOXLIST" Then
            lGenericField.ItemTemplate = New myGenericLiteral(lColumn)
        Else
            lGenericField.ItemTemplate = New myGenericLiteral(lColumn, True)
        End If
        ' Reference : YHHR 2019638 - GBOX COC: OTT OTT 1729: System dependent workflow
        ' Comment   : Make the key fields non-editable in case of update or copy
        ' Added by  : Pratyusa Lenka (CWID : EOJCG)
        ' Date      : 2019-04-01
        lGenericField.EditItemTemplate = MakeEditItemTemplate(objFieldsRow("OBJ_FIELD_ID").ToString, objFieldsRow("OBJ_Field_Type_ID").ToString, objFieldsRow("DISPLAY_NAME").ToString, CBool(objFieldsRow("Required").ToString), objFieldsRow("RequiredText").ToString, lCopyRequest)
        Return lGenericField
    End Function
    Public Function GetdataRow(ByVal lObjFieldID As String, ByVal lTableToCheck As String) As DataRow
        Dim lSQL As String = ""
        lSQL = lSQL & "Select * from " & lTableToCheck & " where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "'"
        lSQL = lSQL & " and OBJ_FIELD_ID='" & lObjFieldID & "'"
        lSQL = lSQL & " and Subgroup_ID='" & mUser.SUBGROUP_ID & "'"
        Dim ldt As DataTable = mUser.Databasemanager.MakeDataTable(lSQL)
        If ldt.Rows.Count = 0 Then
            lSQL = "Select * from " & lTableToCheck & " where OBJ_ID='" & mUser.Current_OBJ.OBJ_ID & "'"
            lSQL = lSQL & " and OBJ_FIELD_ID='" & lObjFieldID & "'"
            lSQL = lSQL & " and Subgroup_ID='ALL'"
            ldt = mUser.Databasemanager.MakeDataTable(lSQL)
        End If
        If ldt.Rows.Count = 0 Then
            Return Nothing
        End If
        Return ldt.Rows(0)
    End Function
    Public Function DynamicColumn(ByVal objFieldsRow As DataRow) As TemplateColumn
        Dim lColumn As String = objFieldsRow("DISPLAY_NAME").ToString
        Dim lGenericCoulumn As New TemplateColumn
        lGenericCoulumn.HeaderText = objFieldsRow("DISPLAY_NAME").ToString
        lGenericCoulumn.ItemTemplate = New myGenericLiteral(lColumn)
        lGenericCoulumn.EditItemTemplate = MakeEditItemTemplate(objFieldsRow)
        Return lGenericCoulumn
    End Function

    Private mErrText As String

    Public Property ErrText() As String
        Get
            Return mErrText
        End Get
        Set(ByVal value As String)
            mErrText &= value
        End Set
    End Property

    Private mRequestform As Object

    Public Property Requestform() As Object
        Get
            Return mRequestform
        End Get
        Set(ByVal value As Object)
            mRequestform = value
        End Set
    End Property

End Class
