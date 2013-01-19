﻿Public Class DesignSpielRunden
    Inherits SpielRunden

    Public Sub New()

        Dim SpielRunde As New SpielRunde
        Dim spielerListe = New DesignSpielerListe()

        For Each partie In New DesignSpielPartien
            SpielRunde.Add(partie)
        Next        
        SpielRunde.AusgeschiedeneSpieler.Add(spielerListe.Last)
        Me.Push(SpielRunde)
    End Sub


End Class


Public Class DesignSpielPartien
    Inherits SpielPartien

    Public Sub New()
        Dim spielerListe = New DesignSpielerListe()
        For i = 1 To spielerListe.Count - 1 Step 2
            Dim partie = New SpielPartie(spielerListe(i - 1), spielerListe(i))
            partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz With {.PunkteLinks = 0, .PunkteRechts = 11})
            partie.Add(New Satz With {.PunkteLinks = 0, .PunkteRechts = 11})
            Me.Add(partie)
        Next
    End Sub

    Public Shared ReadOnly Property DesignSpielPartie As SpielPartie
        Get
            Dim partien = New DesignSpielPartien
            Return partien.First
        End Get
    End Property


End Class

Friend Class DesignSpielerListe
    Inherits SpielerListe

    Public Sub New()
        Add(New Spieler() With {.Vorname = "Marius", .Nachname = "Goppelt", .TTRating = 1})
        Add(New Spieler() With {.Vorname = "Flo", .Nachname = "Ewald", .TTRating = 2})
        Add(New Spieler() With {.Vorname = "Hartmut", .Nachname = "Seiter", .TTRating = 3})
        Add(New Spieler() With {.Vorname = "Max", .Nachname = "Mustermann", .TTRating = 4})
        Add(New Spieler() With {.Vorname = "Leonardo", .Nachname = "Da Vinci", .TTRating = 5})
        Add(New Spieler() With {.Vorname = "Sarah", .Nachname = "Palin", .TTRating = 6})
        Add(New Spieler() With {.Vorname = "Manuel José", .Nachname = "Barroso", .TTRating = 7})
        Add(New Spieler() With {.Vorname = "Catharina", .Nachname = "Sforza", .TTRating = 8})
        Add(New Spieler() With {.Vorname = "Adam", .Nachname = "Jensen", .TTRating = 9})
        Add(New Spieler() With {.Vorname = "Miroslav", .Nachname = "Klose", .TTRating = 10})
    End Sub
End Class
