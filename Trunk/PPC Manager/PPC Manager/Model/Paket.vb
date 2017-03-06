﻿
Public Class Paket
    Implements IComparable(Of Paket)

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

    Sub New(ByVal initialNummer As Integer, rundenName As String, gewinnsätze As Integer)
        Me.InitialNummer = initialNummer
        _RundenName = rundenName
        _Gewinnsätze = gewinnsätze
    End Sub

    Private ReadOnly _Gewinnsätze As Integer

    Private _RundenName As String
    Public ReadOnly Property RundenName As String
        Get
            Return _RundenName
        End Get
    End Property

    Property InitialNummer As Integer

    Property Absteigend As Boolean = True

    Property Partien As New List(Of Tuple(Of Spieler, Spieler))

    Property SpielerListe As New List(Of Spieler)

    Public AltSchwimmer As New List(Of Spieler)

    Public Overridable Property aktuellerSchwimmer As Spieler

    Sub sort()
        SpielerListe.Sort()
        If Absteigend Then SpielerListe.Reverse()
    End Sub

    Sub VerschiebeSchwimmer(ByVal paket As Paket)
        If aktuellerSchwimmer IsNot Nothing Then
            paket.moveAltSchwimmer(aktuellerSchwimmer)
            SpielerListe.Remove(aktuellerSchwimmer)
        End If
        aktuellerSchwimmer = Nothing
    End Sub

    Private Sub moveAltSchwimmer(ByVal aktuellerSchwimmer As Spieler)
        AltSchwimmer.Add(aktuellerSchwimmer)
        SpielerListe.Add(aktuellerSchwimmer)
    End Sub

    Public ReadOnly Property IstAltSchwimmer(ByVal spieler As Spieler) As Boolean
        Get
            Return AltSchwimmer.Contains(spieler)
        End Get
    End Property

    Sub übernimmPaket(ByVal aktuellesPaket As Paket)
        AltSchwimmer.AddRange(aktuellesPaket.SpielerListe)
        SpielerListe.AddRange(aktuellesPaket.SpielerListe)
        aktuellesPaket.SpielerListe.Clear()
        aktuellesPaket.AltSchwimmer.Clear()
        aktuellesPaket.aktuellerSchwimmer = Nothing
    End Sub

    Function SuchePaarungen() As Boolean
        sort()
        Dim p = New PaarungsSuche(Of Spieler)(Function(a, b) a.HatBereitsGespieltGegen(b), Function(s) IstAltSchwimmer(s))
        Dim container = p.SuchePaarungen(SpielerListe, Absteigend)
        If container IsNot Nothing Then
            aktuellerSchwimmer = container.Übrig
            Partien.Clear()
            Partien.AddRange(container.Partien)
            Return True
        End If
        Return False
    End Function

    Public Function CompareTo(ByVal other As Paket) As Integer Implements System.IComparable(Of Paket).CompareTo
        Return InitialNummer - other.InitialNummer
    End Function

End Class
