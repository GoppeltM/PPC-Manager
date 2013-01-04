Public Class DesignSpielRunden
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
        Add(New DesignSpieler() With {.Vorname = "Marius", .Nachname = "Goppelt", .StartNummer = 1})
        Add(New DesignSpieler() With {.Vorname = "Flo", .Nachname = "Ewald", .StartNummer = 2})
        Add(New DesignSpieler() With {.Vorname = "Hartmut", .Nachname = "Seiter", .StartNummer = 3})
        Add(New DesignSpieler() With {.Vorname = "Max", .Nachname = "Mustermann", .StartNummer = 4})
        Add(New DesignSpieler() With {.Vorname = "Leonardo", .Nachname = "Da Vinci", .StartNummer = 5})
        Add(New DesignSpieler() With {.Vorname = "Sarah", .Nachname = "Palin", .StartNummer = 6})
        Add(New DesignSpieler() With {.Vorname = "Manuel José", .Nachname = "Barroso", .StartNummer = 7})
        Add(New DesignSpieler() With {.Vorname = "Catharina", .Nachname = "Sforza", .StartNummer = 8})
        Add(New DesignSpieler() With {.Vorname = "Adam", .Nachname = "Jensen", .StartNummer = 9})
        Add(New DesignSpieler() With {.Vorname = "Miroslav", .Nachname = "Klose", .StartNummer = 10})
    End Sub
End Class

Friend Class DesignSpieler
    Inherits Spieler

    Public Shared runden As New SpielRunden

    Public Sub New()
        MyBase.New(runden)
    End Sub

End Class