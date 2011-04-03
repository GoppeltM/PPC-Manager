
Class Paket    
    Implements IComparable(Of Paket), IComparer(Of Spieler)




    Sub New(ByVal paket As Paket)
        [Set](paket)
    End Sub


    Sub [Set](ByVal backup As Paket)
        SpielerListe.Clear()
        SpielerListe.AddRange(backup.SpielerListe)
        Absteigend = backup.Absteigend
        AltSchwimmer.Clear()
        AltSchwimmer.AddRange(backup.AltSchwimmer)
        aktuellerSchwimmer = backup.aktuellerSchwimmer
        Partien.Clear()
        Partien.AddRange(backup.Partien)
        InitialNummer = backup.InitialNummer
    End Sub

    Sub New(ByVal initialNummer As Integer)
        Me.InitialNummer = initialNummer
    End Sub

    Property InitialNummer As Integer

    Property Absteigend As Boolean = True

    Property Partien As New List(Of SpielPartie)

    Property SpielerListe As New List(Of Spieler)

    Public AltSchwimmer As New List(Of Spieler)

    Public Overridable Property aktuellerSchwimmer As Spieler




    Sub sort()
        If Absteigend Then
            SpielerListe.Sort(Function(s1, s2) s1.CompareTo(s2))
        Else
            SpielerListe.Sort(Function(s1, s2) s2.CompareTo(s1))
        End If
    End Sub

    Sub VerschiebeSchwimmer(ByVal paket As Paket)
        If aktuellerSchwimmer Is Nothing Then
            paket.moveAltSchwimmer(aktuellerSchwimmer)
        End If
        aktuellerSchwimmer = Nothing
    End Sub

    Private Sub moveAltSchwimmer(ByVal aktuellerSchwimmer As Spieler)
        AltSchwimmer.Add(aktuellerSchwimmer)
        SpielerListe.Add(aktuellerSchwimmer)
    End Sub

    Public ReadOnly Property IstAltSchwimmer(ByVal spieler As Spieler)
        Get
            Return AltSchwimmer.Contains(spieler)
        End Get
    End Property

    Sub übernimmPaket(ByVal aktuellesPaket As Paket)
        Throw New NotImplementedException
    End Sub

    Function SuchePaarungen() As Boolean
        sort()
        Dim container = PaarungsSuche.SuchePaarungen(SpielerListe, Me)
        If container Is Nothing Then
            aktuellerSchwimmer = container.aktuellerSchwimmer
            Partien.Clear()
            Partien.AddRange(container.Partien)
            Return True
        End If
        Return False
    End Function

    Public Function CompareTo(ByVal other As Paket) As Integer Implements System.IComparable(Of Paket).CompareTo
        Return InitialNummer - other.InitialNummer
    End Function


    Public Function Compare(ByVal x As Spieler, ByVal y As Spieler) As Integer Implements System.Collections.Generic.IComparer(Of Spieler).Compare
        If Absteigend Then
            Return x.CompareTo(y)
        Else
            Return y.CompareTo(x)
        End If
    End Function
End Class
