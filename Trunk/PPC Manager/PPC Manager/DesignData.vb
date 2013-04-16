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
        Me.Add(New FreiLosSpiel(New Spieler With {.Vorname = "Günther", .Nachname = "Netzer"}))
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
        Add(New Spieler() With {.StartNummer = 1, .Vorname = "Marius", .Nachname = "Goppelt", .TTRating = 1})
        Add(New Spieler() With {.StartNummer = 2, .Vorname = "Flo", .Nachname = "Ewald", .TTRating = 2})
        Add(New Spieler() With {.StartNummer = 3, .Vorname = "Hartmut", .Nachname = "Seiter", .TTRating = 3})
        Add(New Spieler() With {.StartNummer = 4, .Vorname = "Max", .Nachname = "Mustermann", .TTRating = 4})
        Add(New Spieler() With {.StartNummer = 5, .Vorname = "Leonardo", .Nachname = "Da Vinci", .TTRating = 5})
        Add(New Spieler() With {.StartNummer = 6, .Vorname = "Sarah", .Nachname = "Palin", .TTRating = 6})
        Add(New Spieler() With {.StartNummer = 7, .Vorname = "Manuel José", .Nachname = "Barroso", .TTRating = 7})
        Add(New Spieler() With {.StartNummer = 8, .Vorname = "Catharina", .Nachname = "Sforza", .TTRating = 8})
        Add(New Spieler() With {.StartNummer = 9, .Vorname = "Adam", .Nachname = "Jensen", .TTRating = 9})
        Add(New Spieler() With {.StartNummer = 10, .Vorname = "Miroslav", .Nachname = "Klose", .TTRating = 10})
        Add(New Spieler() With {.StartNummer = 11, .Vorname = "Che", .Nachname = "Guevara", .TTRating = 11})
        Add(New Spieler() With {.StartNummer = 12, .Vorname = "Friedrich", .Nachname = "Nietzsche", .TTRating = 12})
        Add(New Spieler() With {.StartNummer = 13, .Vorname = "Johann Wolfgang von", .Nachname = "Goethe", .TTRating = 13})
        Add(New Spieler() With {.StartNummer = 14, .Vorname = "Margaret", .Nachname = "Thatcher", .TTRating = 14})
        Add(New Spieler() With {.StartNummer = 15, .Vorname = "Marie", .Nachname = "Curie", .TTRating = 15})
        Add(New Spieler() With {.StartNummer = 16, .Vorname = "Albert", .Nachname = "Einstein", .TTRating = 16})
        Add(New Spieler() With {.StartNummer = 17, .Vorname = "Julius", .Nachname = "Cäsar", .TTRating = 17})
        Add(New Spieler() With {.StartNummer = 18, .Vorname = "Lucius", .Nachname = "Verenus", .TTRating = 18})
        Add(New Spieler() With {.StartNummer = 19, .Vorname = "Titus", .Nachname = "Pullo", .TTRating = 19})
        Add(New Spieler() With {.StartNummer = 20, .Vorname = "Mick", .Nachname = "Jagger", .TTRating = 20})
        Add(New Spieler() With {.StartNummer = 21, .Vorname = "Paul", .Nachname = "Ryan", .TTRating = 21})
        Add(New Spieler() With {.StartNummer = 22, .Vorname = "Barack", .Nachname = "Obama", .TTRating = 22})
        Add(New Spieler() With {.StartNummer = 23, .Vorname = "Francois", .Nachname = "Mitterrand", .TTRating = 23})
        Add(New Spieler() With {.StartNummer = 24, .Vorname = "Napoleon", .Nachname = "Bonaparte", .TTRating = 24})
        Add(New Spieler() With {.StartNummer = 25, .Vorname = "Marty", .Nachname = "McFly", .TTRating = 25})
        Add(New Spieler() With {.StartNummer = 26, .Vorname = "Marilyn", .Nachname = "Monroe", .TTRating = 26})
        Add(New Spieler() With {.StartNummer = 27, .Vorname = "Freddy", .Nachname = "Mercury", .TTRating = 27})
        Add(New Spieler() With {.StartNummer = 28, .Vorname = "Stephen", .Nachname = "Hawking", .TTRating = 28})
    End Sub
End Class
