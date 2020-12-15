Imports Microsoft.VisualBasic.DateAndTime
Public Class Arbeitstage
    Public arbtage As Integer
    Public monatstage(11) As Integer
    Public Property arbeitstage_imjahr(ByVal zahl As Integer) As Integer
        Get
            Return monatstage(zahl)
        End Get
        Set(ByVal anz As Integer)
        End Set
    End Property
    Public Property arbeitstage_immonat() As Integer
        Get
            Return arbtage
        End Get
        Set(ByVal anz As Integer)
        End Set
    End Property

    Public Function Ostern(ByVal x As Integer) As Date
        Dim K, M, S, D, R, A, OG, SZ, OE, OSI As Integer
        Dim OS1 As Double
        Dim OS2 As String
        K = Int(x / 100)
        M = 15 + Int((3 * K + 3) / 4) - Int((8 * K + 13) / 25)
        S = 2 - Int((3 * K + 3) / 4)
        A = x Mod 19
        D = (19 * A + M) Mod 30
        R = Int(D / 29) + (Int(D / 28) - Int(D / 29)) * Int(A / 11)
        OG = 21 + D - R
        SZ = 7 - (x + Int(x / 4) + S) Mod 7
        OE = 7 - (OG - SZ) Mod 7
        OSI = ((OG + OE) - 1)
        OS2 = "01.03." & x
        OS2 = CDate(OS2)
        Dim da As New Date
        da = OS2
        OS1 = CType(da.ToOADate(), Double)
        OS1 = CType(da.ToOADate(), Double)
        OS1 = OS1 + OSI
        Ostern = CType(Date.FromOADate(OS1), Date)
    End Function

    Public Function arbeitstag(ByVal x As Date) As Boolean
        Dim feiertage(12) As Date
        Dim feiertag As String
        Dim jahr, SaSo, Arbeitst, i As Integer
        jahr = CInt(Format(x, "yyyy"))
        SaSo = CInt(Weekday(x))
        arbeitstag = True
        If (SaSo = 1 Or SaSo = 7) Then
            arbeitstag = False
        Else
            feiertage(1) = DateAdd(DateInterval.Day, -2, Ostern(jahr))
            feiertage(2) = DateAdd(DateInterval.Day, +1, Ostern(jahr))
            feiertage(3) = DateAdd(DateInterval.Day, +50, Ostern(jahr))
            feiertage(4) = DateAdd(DateInterval.Day, +39, Ostern(jahr))
            feiertage(5) = DateAdd(DateInterval.Day, +60, Ostern(jahr))

            feiertage(6) = (CDate("01.01." & jahr))
            feiertage(7) = (CDate("01.05." & jahr))
            feiertage(8) = (CDate("03.10." & jahr))
            feiertage(9) = (CDate("01.11." & jahr))
            feiertage(10) = (CDate("25.12." & jahr))
            feiertage(11) = (CDate("26.12." & jahr))

            For i = 1 To 11
                If x = feiertage(i) Then
                    arbeitstag = False
                    Exit For
                End If
            Next
        End If
    End Function

    Public Function arbeitstage(ByVal x As Date, ByVal zahl As Integer)
        Dim tage, m, anz, i As Integer
        Dim wert As String
        ' unterscheiden, ob Jahres oder Monatsberechnung
        ' anhand Parameter "zahl"
        Select Case zahl
            Case 1
                ' jeden Monat durchlaufen
                For m = 1 To 12
                    Dim datum As New Date
                    datum = "#01/" & m & "/" & Year(x) & "#"
                    ' Anzahl der Tage im Monat ermitteln
                    tage = x.DaysInMonth(Year(x), Month(datum))
                    anz = 0
                    ' alle Tage überprüfen ob Arbeitstag!
                    For i = 1 To tage
                        wert = "#" & i & "/" & m & "/" & Year(x) & "#"
                        If arbeitstag(wert) Then
                            anz = anz + 1
                        End If
                    Next
                    ' Anzahl der Arbeitstage im Array speichern
                    monatstage(m - 1) = anz
                Next
            Case 2
                ' Anzahl der Tage im Monat ermitteln
                tage = x.DaysInMonth(Year(x), Month(x))
                ' alle Tage überprüfen ob Arbeitstag!
                For i = 1 To tage
                    wert = "#" & i & "/" & Month(x) & "/" & Year(x) & "#"
                    If arbeitstag(wert) Then
                        anz = anz + 1
                    End If
                Next
                ' Anzahl der Arbeitstage speichern
                arbtage = anz
        End Select
    End Function
End Class
'Beispiel für die Anwendung/Aufruf der Klasse
'Benötigen Sie nachfolgende Controls auf die Form:

'Label (Label1)
'TextBox (TextBox1)
'CommandButton (Button1)

'Die TextBox dient der Eingabe für Monat/Jahr bzw. nur Jahr. Beim Klick auf den Button werden demzufolge die Anzahl der Arbeitstage entweder für den eingegeben Monat oder der Reihe nach für alle Monate des Jahrs ermittelt.

