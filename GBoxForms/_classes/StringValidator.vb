Public Class StringValidator
    Private mCharsAllowed As List(Of String)
    Public Enum Reasom
        ToLong = 0
        CharsNotAllowed = 1
        OK = 2
        ToShort = 3
    End Enum
    Private mNotAllowedKeys As String
    Public Property NotAllowedKeys() As String
        Get
            Return mNotAllowedKeys
        End Get
        Set(ByVal value As String)
            mNotAllowedKeys = value
        End Set
    End Property
    Public Sub New()
        mCharsAllowed = New List(Of String)
        With mCharsAllowed
            .Add("0")
            .Add("1")
            .Add("2")
            .Add("3")
            .Add("4")
            .Add("5")
            .Add("6")
            .Add("7")
            .Add("8")
            .Add("9")
            .Add("A")
            .Add("B")
            .Add("C")
            .Add("D")
            .Add("E")
            .Add("F")
            .Add("G")
            .Add("H")
            .Add("I")
            .Add("J")
            .Add("K")
            .Add("L")
            .Add("M")
            .Add("N")
            .Add("O")
            .Add("P")
            .Add("Q")
            .Add("R")
            .Add("S")
            .Add("T")
            .Add("U")
            .Add("V")
            .Add("W")
            .Add("X")
            .Add("Y")
            .Add("Z")
            .Add("_")
            .Add("a")
            .Add("b")
            .Add("c")
            .Add("d")
            .Add("e")
            .Add("f")
            .Add("g")
            .Add("h")
            .Add("i")
            .Add("j")
            .Add("k")
            .Add("l")
            .Add("m")
            .Add("n")
            .Add("o")
            .Add("p")
            .Add("q")
            .Add("r")
            .Add("s")
            .Add("t")
            .Add("u")
            .Add("v")
            .Add("w")
            .Add("x")
            .Add("y")
            .Add("z")
            .Add("&")
            .Add("_")
            .Add(":")
            .Add(";")
            .Add(",")
            .Add("@")
            .Add("Ä")
            .Add("Ö")
            .Add("Ü")
            .Add("ä")
            .Add("ü")
            .Add("ö")
            .Add(".")

        End With
    End Sub
    Public Function VaidateChars(ByVal lstrToValidate As String) As Boolean
        mNotAllowedKeys = ""
        For i As Integer = 0 To lstrToValidate.Length - 1
            'Debug.Print(lstrToValidate.Substring(i, 1))
            If Not mCharsAllowed.Contains(lstrToValidate.Substring(i, 1)) Then
                mNotAllowedKeys = mNotAllowedKeys & lstrToValidate.Substring(i, 1)
            End If
        Next
        If mNotAllowedKeys = "" Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function ValidateLengthAndChars(ByVal lstrToCheck As String, Optional ByVal lLength As Long = 30) As Reasom
        If lstrToCheck.Length > lLength Then Return Reasom.ToLong
        If lstrToCheck.Length < lLength Then Return Reasom.ToShort
        If Not VaidateChars(lstrToCheck) Then
            Return Reasom.CharsNotAllowed
        End If
        If lstrToCheck.Contains(" ") Then
            mNotAllowedKeys = "blanks "
            Return Reasom.CharsNotAllowed
        End If
        Return Reasom.OK
    End Function
End Class
