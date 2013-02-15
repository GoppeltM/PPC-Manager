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
        Add(New Spieler() With {.Vorname = "Che", .Nachname = "Guevara", .TTRating = 11})
        Add(New Spieler() With {.Vorname = "Friedrich", .Nachname = "Nietzsche", .TTRating = 12})
        Add(New Spieler() With {.Vorname = "Johann Wolfgang von", .Nachname = "Goethe", .TTRating = 13})
        Add(New Spieler() With {.Vorname = "Margaret", .Nachname = "Thatcher", .TTRating = 14})
        Add(New Spieler() With {.Vorname = "Marie", .Nachname = "Curie", .TTRating = 15})
        Add(New Spieler() With {.Vorname = "Albert", .Nachname = "Einstein", .TTRating = 16})
        Add(New Spieler() With {.Vorname = "Julius", .Nachname = "Cäsar", .TTRating = 17})
        Add(New Spieler() With {.Vorname = "Lucius", .Nachname = "Verenus", .TTRating = 18})
        Add(New Spieler() With {.Vorname = "Titus", .Nachname = "Pullo", .TTRating = 19})
        Add(New Spieler() With {.Vorname = "Mick", .Nachname = "Jagger", .TTRating = 20})
        Add(New Spieler() With {.Vorname = "Paul", .Nachname = "Ryan", .TTRating = 21})
        Add(New Spieler() With {.Vorname = "Barack", .Nachname = "Obama", .TTRating = 22})
        Add(New Spieler() With {.Vorname = "Francois", .Nachname = "Mitterrand", .TTRating = 23})
        Add(New Spieler() With {.Vorname = "Napoleon", .Nachname = "Bonaparte", .TTRating = 24})
        Add(New Spieler() With {.Vorname = "Marty", .Nachname = "McFly", .TTRating = 25})
        Add(New Spieler() With {.Vorname = "Marilyn", .Nachname = "Monroe", .TTRating = 26})
        Add(New Spieler() With {.Vorname = "Freddy", .Nachname = "Mercury", .TTRating = 27})
        Add(New Spieler() With {.Vorname = "Stephen", .Nachname = "Hawking", .TTRating = 28})
    End Sub
End Class
