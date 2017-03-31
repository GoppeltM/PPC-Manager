
Imports PPC_Manager

Public Class Paket(Of T)
    Implements IComparable(Of Paket(Of T))

    Sub New(ByVal paket As Paket(Of T))
        _SuchePaarungen = paket._SuchePaarungen
        [Set](paket)
    End Sub


    Sub [Set](ByVal backup As Paket(Of T))
        SpielerListe.Clear()
        SpielerListe.AddRange(backup.SpielerListe)
        Absteigend = backup.Absteigend
        aktuellerSchwimmer = backup.aktuellerSchwimmer
        Partien.Clear()
        Partien.AddRange(backup.Partien)
        InitialNummer = backup.InitialNummer
    End Sub

    Sub New(suchePaarungen As SuchePaarungen(Of T),
            ByVal initialNummer As Integer,
            istAltschwimmer As Predicate(Of T),
            setzeAltschwimmer As Action(Of T)
            )

        _SuchePaarungen = suchePaarungen
        _IstAltschwimmer = istAltschwimmer
        _SetzeAltschwimmer = setzeAltschwimmer
        Me.InitialNummer = initialNummer
    End Sub


    Property InitialNummer As Integer

    Property Absteigend As Boolean = True

    Property Partien As New List(Of Tuple(Of T, T))

    Property SpielerListe As New List(Of T)

    Private ReadOnly _SuchePaarungen As SuchePaarungen(Of T)
    Protected ReadOnly _IstAltschwimmer As Predicate(Of T)
    Protected ReadOnly _SetzeAltschwimmer As Action(Of T)
    Public Overridable Property aktuellerSchwimmer As T

    Sub VerschiebeSchwimmer(ByVal paket As Paket(Of T))
        If aktuellerSchwimmer IsNot Nothing Then
            paket.moveAltSchwimmer(aktuellerSchwimmer)
            SpielerListe.Remove(aktuellerSchwimmer)
        End If
        aktuellerSchwimmer = Nothing
    End Sub

    Private Sub moveAltSchwimmer(ByVal aktuellerSchwimmer As T)
        _SetzeAltschwimmer(aktuellerSchwimmer)
        SpielerListe.Add(aktuellerSchwimmer)
    End Sub

    Sub übernimmPaket(ByVal aktuellesPaket As Paket(Of T))
        For Each s In aktuellesPaket.SpielerListe
            _SetzeAltschwimmer(s)
        Next
        SpielerListe.AddRange(aktuellesPaket.SpielerListe)
        aktuellesPaket.SpielerListe.Clear()
        aktuellesPaket.aktuellerSchwimmer = Nothing
    End Sub

    Function SuchePaarungen() As Boolean
        Dim container = _SuchePaarungen(Function(x) _IstAltschwimmer(x), SpielerListe, Absteigend)
        If container IsNot Nothing Then
            aktuellerSchwimmer = container.Übrig
            Partien.Clear()
            Partien.AddRange(container.Partien)
            Return True
        End If
        Return False
    End Function

    Public Function CompareTo(ByVal other As Paket(Of T)) As Integer Implements System.IComparable(Of Paket(Of T)).CompareTo
        Return InitialNummer - other.InitialNummer
    End Function

End Class
