Public Class SpielPartieTests

    <Test>
    Public Sub Partie_mit_drei_gewonnenen_Sätzen_ist_abgeschlossen()
        Dim s1 As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        Dim s2 As New Spieler(New SpielRunden, New SpielRegeln(3, True, True))
        Dim p As New SpielPartie("Runde 1", s1, s2, 3)
        p.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 13})
        p.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 13})
        p.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 13})
        Assert.That(p.Abgeschlossen, [Is].True)
    End Sub

End Class
