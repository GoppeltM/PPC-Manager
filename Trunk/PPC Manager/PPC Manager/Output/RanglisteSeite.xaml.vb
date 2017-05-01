Imports System.Collections.ObjectModel
Imports PPC_Manager

Public Class RanglisteSeite

    Public Sub New(altersgruppe As String,
                   rundenNummer As Integer,
                   elemente As IEnumerable(Of Spieler),
                   spielpartien As IEnumerable(Of SpielPartie),
                   spielstand As ISpielstand)
        ' This call is required by the designer.
        InitializeComponent()
        KlassementName.Text = altersgruppe
        AktuellesDatum.Text = Date.Now.ToString("dd.MM.yyyy")
        Me.RundenNummer.Text = String.Format("Runde Nr. {0}", rundenNummer)

        Dim druckSpieler = From x In elemente Select New RangListeSpieler(x, spielpartien, spielstand)

        Dim res = CType(FindResource("Spieler"), RangListeSpielerListe)
        For Each s In druckSpieler
            res.Add(s)
        Next

    End Sub

    Public Sub New(seite As RanglisteSeite)
        InitializeComponent()
        Dim converter = CType(FindResource("GridIndexConverter"), GridIndexConverter)
        Dim converterAlt = CType(seite.FindResource("GridIndexConverter"), GridIndexConverter)
        converter.Offset = converterAlt.Offset
        KlassementName.Text = seite.KlassementName.Text
        AktuellesDatum.Text = seite.AktuellesDatum.Text
        Me.RundenNummer.Text = seite.RundenNummer.Text

        Dim res = CType(FindResource("Spieler"), RangListeSpielerListe)
        Dim resAlt = CType(seite.FindResource("Spieler"), RangListeSpielerListe)
        For Each s In resAlt
            res.Add(s)
        Next
    End Sub

    Public Function GetMaxItemCount() As Integer
        Dim width = SpielerRangListe.ActualWidth
        Dim height = SpielerRangListe.ActualHeight

        Dim ItemHeight = 20

        Dim Rows = CInt(height - 25) \ ItemHeight

        If Rows = 0 Then
            Throw New Exception("Seitenformat zu klein! Kann kein Element darauf vollständig drucken!")
        End If
        Return Rows
    End Function

End Class

Public Class RangListeSpieler
    Inherits Spieler

    Private ReadOnly _Spielpartien As IEnumerable(Of SpielPartie)
    Private ReadOnly _Spielstand As ISpielstand

    Public Sub New(spieler As Spieler,
                   spielPartien As IEnumerable(Of SpielPartie),
                   spielstand As ISpielstand)
        MyBase.New(spieler)
        _Spielpartien = spielPartien
        _Spielstand = spielstand

    End Sub

    Public ReadOnly Property MeineSpieleDruck As IEnumerable(Of String)
        Get
            Dim l As New List(Of String)
            Dim gespieltePartien = From x In _Spielpartien Where x.SpielerLinks = Me Or x.SpielerRechts = Me
            For Each s In gespieltePartien

                Dim text = s.MeinGegner(Me).StartNummer.ToString
                If _Spielstand.HatPartieGewonnen(s, Me) Then
                    text &= "G"
                Else
                    text &= "V"
                End If
                l.Add(text)
            Next
            Return l
        End Get
    End Property
End Class

Public Class RangListeSpielerListe
    Inherits ObservableCollection(Of RangListeSpieler)
End Class