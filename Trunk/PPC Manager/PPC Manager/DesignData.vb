Public Class DesignSpielRunden
    Inherits SpielRunden

    Public Sub New()

        Dim SpielRunde As New SpielRunde
        Dim spielerListe = New DesignSpielerListe()

        For Each partie In New DesignSpielPartien
            SpielRunde.Add(partie)
        Next

        SpielRunde.Add(New FreiLosSpiel("Runde 1", New Spieler(Nothing) With {.Id = "999", .Nachname = "MisterX", .Vorname = "Max"}, 3))

        AusgeschiedeneSpieler.Add(New Ausgeschieden With {.Spieler = spielerListe.Last, .Runde = 1})
        Me.Push(SpielRunde)
    End Sub


End Class

Public Class DesignSpielPartienOhneFreilos
    Inherits SpielPartien

    Public Sub New()
        Dim spielerListe = New DesignSpielerListe()
        For i = 1 To spielerListe.Count - 1 Step 2
            Dim partie = New SpielPartie("Runde 2", spielerListe(i - 1), spielerListe(i), 3)
            partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz With {.PunkteLinks = 0, .PunkteRechts = 11})
            partie.Add(New Satz With {.PunkteLinks = 0, .PunkteRechts = 11})
            partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 0})
            partie.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 5})            
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

Public Class DesignSpielPartien
    Inherits DesignSpielPartienOhneFreilos

    Public Sub New()        
        Me.Add(New FreiLosSpiel("Runde 2", New Spieler(Nothing) With {.Vorname = "Günther", .Nachname = "Netzer", .Id = "PLAYER255"}, 3))
    End Sub

End Class

Friend Class DesignSpielerListe
    Inherits SpielerListe

    Public Sub New()
        Add(New Spieler(Nothing) With {.Id = "PLAYER1", .Vorname = "Marius", .Nachname = "Goppelt", .TTRating = 1, .Geschlecht = 1})
        Add(New Spieler(Nothing) With {.Id = "PLAYER2", .Vorname = "Flo", .Nachname = "Ewald", .TTRating = 2})
        Add(New Spieler(Nothing) With {.Id = "PLAYER3", .Vorname = "Hartmut", .Nachname = "Seiter", .TTRating = 3})
        Add(New Spieler(Nothing) With {.Id = "PLAYER4", .Vorname = "Max", .Nachname = "Mustermann", .TTRating = 4})
        Add(New Spieler(Nothing) With {.Id = "PLAYER5", .Vorname = "Leonardo", .Nachname = "Da Vinci", .TTRating = 5, .Geschlecht = 1})
        Add(New Spieler(Nothing) With {.Id = "PLAYER6", .Vorname = "Sarah", .Nachname = "Palin", .TTRating = 6, .Geschlecht = 0})
        Add(New Spieler(Nothing) With {.Id = "PLAYER7", .Vorname = "Manuel José", .Nachname = "Barroso", .TTRating = 7})
        Add(New Spieler(Nothing) With {.Id = "PLAYER8", .Vorname = "Catharina", .Nachname = "Sforza", .TTRating = 8})
        Add(New Spieler(Nothing) With {.Id = "PLAYER9", .Vorname = "Adam", .Nachname = "Jensen", .TTRating = 9})
        Add(New Spieler(Nothing) With {.Id = "PLAYER10", .Vorname = "Miroslav", .Nachname = "Klose", .TTRating = 10})
        Add(New Spieler(Nothing) With {.Id = "PLAYER11", .Vorname = "Che", .Nachname = "Guevara", .TTRating = 11})
        Add(New Spieler(Nothing) With {.Id = "PLAYER12", .Vorname = "Friedrich", .Nachname = "Nietzsche", .TTRating = 12})
        Add(New Spieler(Nothing) With {.Id = "PLAYER13", .Vorname = "Johann Wolfgang von", .Nachname = "Goethe", .TTRating = 13})
        Add(New Spieler(Nothing) With {.Id = "PLAYER14", .Vorname = "Margaret", .Nachname = "Thatcher", .TTRating = 14})
        Add(New Spieler(Nothing) With {.Id = "PLAYER15", .Vorname = "Marie", .Nachname = "Curie", .TTRating = 15})
        Add(New Spieler(Nothing) With {.Id = "PLAYER16", .Vorname = "Albert", .Nachname = "Einstein", .TTRating = 16})
        Add(New Spieler(Nothing) With {.Id = "PLAYER17", .Vorname = "Julius", .Nachname = "Cäsar", .TTRating = 17})
        Add(New Spieler(Nothing) With {.Id = "PLAYER18", .Vorname = "Lucius", .Nachname = "Verenus", .TTRating = 18})
        Add(New Spieler(Nothing) With {.Id = "PLAYER19", .Vorname = "Titus", .Nachname = "Pullo", .TTRating = 19})
        Add(New Spieler(Nothing) With {.Id = "PLAYER20", .Vorname = "Mick", .Nachname = "Jagger", .TTRating = 20})
        Add(New Spieler(Nothing) With {.Id = "PLAYER21", .Vorname = "Paul", .Nachname = "Ryan", .TTRating = 21})
        Add(New Spieler(Nothing) With {.Id = "PLAYER22", .Vorname = "Barack", .Nachname = "Obama", .TTRating = 22})
        Add(New Spieler(Nothing) With {.Id = "PLAYER23", .Vorname = "Francois", .Nachname = "Mitterrand", .TTRating = 23})
        Add(New Spieler(Nothing) With {.Id = "PLAYER24", .Vorname = "Napoleon", .Nachname = "Bonaparte", .TTRating = 24, .Geburtsjahr = 1790})
        Add(New Spieler(Nothing) With {.Id = "PLAYER25", .Vorname = "Marty", .Nachname = "McFly", .TTRating = 25})
        Add(New Spieler(Nothing) With {.Id = "PLAYER26", .Vorname = "Marilyn", .Nachname = "Monroe", .TTRating = 26})
        Add(New Spieler(Nothing) With {.Id = "PLAYER27", .Vorname = "Freddy", .Nachname = "Mercury", .TTRating = 27})
        Add(New Spieler(Nothing) With {.Id = "PLAYER28", .Vorname = "Stephen", .Nachname = "Hawking", .TTRating = 28})
    End Sub
End Class
