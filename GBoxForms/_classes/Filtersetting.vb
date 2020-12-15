Public Class Filtersetting
    Private mMessagetype As String
    Public Property Messagetype() As String
        Get
            Return mMessagetype
        End Get
        Set(ByVal value As String)
            mMessagetype = value
        End Set
    End Property

    Private mAPPLICATION_ID As String
    Public Property APPLICATION_ID() As String
        Get
            Return mAPPLICATION_ID
        End Get
        Set(ByVal value As String)
            mAPPLICATION_ID = value
        End Set
    End Property
    Private mAPPLICATION_PART_ID As String
    Public Property APPLICATION_PART_ID() As String
        Get
            Return mAPPLICATION_PART_ID
        End Get
        Set(ByVal value As String)
            mAPPLICATION_PART_ID = value
        End Set
    End Property
    Private mFILTER_GROUP As String
    Public Property FILTER_GROUP() As String
        Get
            Return mFILTER_GROUP
        End Get
        Set(ByVal value As String)
            mFILTER_GROUP = value
        End Set
    End Property
    Private mAREA_ID As String
    Public Property AREA_ID() As String
        Get
            Return mAREA_ID
        End Get
        Set(ByVal value As String)
            mAREA_ID = value
        End Set
    End Property
    Private mORG_LEVEL_ID As String
    Public Property ORG_LEVEL_ID() As String
        Get
            Return mORG_LEVEL_ID
        End Get
        Set(ByVal value As String)
            mORG_LEVEL_ID = value
        End Set
    End Property
    Private mFILTER_VALUE As String
    Public Property FILTER_VALUE() As String
        Get
            Return mFILTER_VALUE
        End Get
        Set(ByVal value As String)
            mFILTER_VALUE = value
        End Set
    End Property
    Private mCLASSIFICATION_TYPE_ID As String
    Public Property CLASSIFICATION_TYPE_ID() As String
        Get
            Return mCLASSIFICATION_TYPE_ID
        End Get
        Set(ByVal value As String)
            mCLASSIFICATION_TYPE_ID = value
        End Set
    End Property
    Private mSAP_CLASS As String
    Public Property SAP_CLASS() As String
        Get
            Return mSAP_CLASS
        End Get
        Set(ByVal value As String)
            mSAP_CLASS = value
        End Set
    End Property

    Public Function getUpdate(ByVal classification As String) As String
        Dim lsql As String = "Update SAP_FILTER_SETTINGS set CLASSIFICATION_TYPE_ID = '" & classification & "' where "
        lsql = lsql & "APPLICATION_PART_ID='" & mAPPLICATION_PART_ID & "'"
        lsql = lsql & " And FILTER_GROUP='" & mFILTER_GROUP & "'"
        lsql = lsql & " And AREA_ID='" & mAREA_ID & "'"
        lsql = lsql & " And ORG_LEVEL_ID='" & mORG_LEVEL_ID & "'"
        lsql = lsql & " And FILTER_VALUE='" & mFILTER_VALUE & "'"
        Return lsql
    End Function
    '---------------------------------------------------------------------------------------------------
    ' Reference         : ZHHR 1049328 - GBOX ACFS OTT 1048: New Workflow for ACFS
    ' Comment           : Added empty string in column MESTYP
    ' Added by          : Milind Randive (CWID : EOJCH)
    ' Date              : 29-Oct-2015
    Public Function getInsert(ByVal classification As String) As String
        Dim lsql As String = "INSERT INTO SAP_FILTER_SETTINGS"
        lsql = lsql & "(APPLICATION_PART_ID"
        lsql = lsql & " ,FILTER_GROUP"
        lsql = lsql & ",AREA_ID"
        lsql = lsql & " ,ORG_LEVEL_ID"
        lsql = lsql & ",FILTER_VALUE"
        lsql = lsql & ",MESTYP"
        lsql = lsql & ",CLASSIFICATION_TYPE_ID)"
        lsql = lsql & " VALUES "
        lsql = lsql & "('" & mAPPLICATION_PART_ID & "','"
        lsql = lsql & mFILTER_GROUP & "','"
        lsql = lsql & mAREA_ID & "','"
        lsql = lsql & mORG_LEVEL_ID & "','"
        lsql = lsql & mFILTER_VALUE & "','"
        lsql = lsql & "    " & "','"
        Select Case classification
            Case "Filtersetting"
                lsql = lsql & "FS')"
            Case "Autoclassification"
                lsql = lsql & "AC')"
            Case "Both"
                lsql = lsql & "ACFS')"
        End Select
        Return lsql
    End Function
    ' Reference End     : ZHHR 1049328

    Public Function getDelete() As String
        Dim lsql As String = "Delete From SAP_FILTER_SETTINGS  where "
        lsql = lsql & "APPLICATION_PART_ID='" & mAPPLICATION_PART_ID & "'"
        lsql = lsql & " AND FILTER_GROUP='" & mFILTER_GROUP & "'"
        lsql = lsql & " And AREA_ID='" & mAREA_ID & "'"
        lsql = lsql & " And ORG_LEVEL_ID='" & mORG_LEVEL_ID & "'"
        lsql = lsql & " And FILTER_VALUE='" & mFILTER_VALUE & "'"
        Return lsql
    End Function

End Class
