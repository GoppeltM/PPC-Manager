﻿Imports System.Collections.ObjectModel
Imports PPC_Manager

Public Class RanglisteSeite

    Public Function RowWidth() As Int32
        Return 600
    End Function


    Public Sub New(altersgruppe As String,
                   elemente As IEnumerable(Of Spieler),
                   spielRunden As IEnumerable(Of SpielRunde),
                   spielstand As ISpielstand,
                   seitenEinstellungen As SeitenEinstellung,
                   RangOffset As Int32)

        ' This call is required by the designer.
        InitializeComponent()
        Dim converter = CType(FindResource("GridIndexConverter"), GridIndexConverter)
        converter.Offset = RangOffset

        KlassementName.Text = altersgruppe
        AktuellesDatum.Text = Date.Now.ToString("dd.MM.yyyy")
        RundenNummer.Text = String.Format("Runde Nr. {0}", spielRunden.Count - 1)

        Dim druckSpieler = From x In elemente Select New RangListeSpieler(x, spielRunden, spielstand)

        Dim res = CType(FindResource("Spieler"), RangListeSpielerListe)
        For Each s In druckSpieler
            res.Add(s)
        Next

    End Sub

    Public Sub New(seite As RanglisteSeite)
        InitializeComponent()
        Dim converter = CType(FindResource("GridIndexConverter"), GridIndexConverter)
        KlassementName.Text = seite.KlassementName.Text
        AktuellesDatum.Text = seite.AktuellesDatum.Text
        RundenNummer.Text = seite.RundenNummer.Text

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

    Private ReadOnly _spielRunden As IEnumerable(Of SpielRunde)
    Private ReadOnly _Spielstand As ISpielstand

    Public Sub New(spieler As Spieler,
                   spielRunden As IEnumerable(Of SpielRunde),
                   spielstand As ISpielstand)
        MyBase.New(spieler)
        _spielRunden = spielRunden
        _Spielstand = spielstand
    End Sub

    Public ReadOnly Property GegnerProfil As IEnumerable(Of String)
        Get
            Dim l As New List(Of String)
            For Each runde In _spielRunden
                If runde.Count = 0 Then Continue For

                Dim gespieltePartien = From x In runde Where x.SpielerLinks = Me Or x.SpielerRechts = Me

                'Dependent on the length of IDs
                Dim IDPlaceholder As String = String.Join("", runde(0).SpielerLinks.StartNummer.ToString.ToList().Concat("-".ToCharArray).Select(Function(c) "-"))

                If gespieltePartien.Count = 0 Then l.Add(IDPlaceholder)


                Dim FreilosPlaceholder = "FL"
                For i As Integer = 3 To IDPlaceholder.Count
                    FreilosPlaceholder = " " & FreilosPlaceholder
                Next

                For Each partie In gespieltePartien
                    If TypeOf partie Is FreiLosSpiel Then
                        l.Add(FreilosPlaceholder)
                        Continue For
                    End If

                    Dim text = partie.GegnerVon(Me).StartNummer.ToString
                    If _Spielstand.HatPartieGewonnen(partie, Me) Then
                        text &= "G"
                    Else
                        text &= "V"
                    End If
                    l.Add(text)
                Next
            Next
            Return l
        End Get
    End Property
End Class

Public Class RangListeSpielerListe
    Inherits ObservableCollection(Of RangListeSpieler)
End Class