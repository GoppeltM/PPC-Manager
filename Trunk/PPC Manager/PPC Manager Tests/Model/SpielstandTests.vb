Public Class SpielstandTests
    Private _P As SpielPartie
    Private _SpielerA As SpielerInfo
    Private _SpielerB As SpielerInfo

    <SetUp>
    Public Sub Init()
        _SpielerA = New SpielerInfo("A")
        _SpielerB = New SpielerInfo("B")
        _P = New SpielPartie("Runde 1", _SpielerA, _SpielerB)
    End Sub

    <Test>
    Public Sub IstAbgeschlossen_ist_false_für_leere_Partie()
        Dim s = New Spielstand(3)
        Assert.That(s.IstAbgeschlossen(_P), [Is].False)
    End Sub

    <Test>
    Public Sub IstAbgeschlossen_ist_false_wenn_weniger_Sätze_gewonnen_als_Gewinnsätze()
        Dim s = New Spielstand(3)
        _P.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 0})
        _P.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 0})
        Assert.That(s.IstAbgeschlossen(_P), [Is].False)
    End Sub

    <Test>
    Public Sub IstAbgeschlossen_ist_true_wenn_Gewinnsätze_auf_einer_Seite_erreicht()
        Dim s = New Spielstand(3)
        _P.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 0})
        _P.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 0})
        _P.Add(New Satz With {.PunkteLinks = 0, .PunkteRechts = 11})
        _P.Add(New Satz With {.PunkteLinks = 11, .PunkteRechts = 0})
        Assert.That(s.IstAbgeschlossen(_P), [Is].True)
    End Sub

    <Test>
    Public Sub IstAbgeschlossen_ist_true_wenn_Freilos()
        Dim s = New Spielstand(3)
        Dim f = New FreiLosSpiel("Runde 1", New SpielerInfo("A"))
        Assert.That(s.IstAbgeschlossen(f), [Is].True)
    End Sub

    <Test>
    Public Sub MeineGewonnenenSätze_sind_immer_Maximum_wenn_Freilos()
        Dim s = New Spielstand(3)
        Dim f = New FreiLosSpiel("Runde 1", New SpielerInfo("A"))
        Assert.That(s.MeineGewonnenenSätze(f, New SpielerInfo("A")), [Is].EqualTo(3))
    End Sub
    <Test>
    Public Sub MeineGewonnenenSätze_geben_abgeschlossene_Sätze_zurück()
        Dim s = New Spielstand(3)
        Dim satz1 = New Satz With {.PunkteLinks = 11, .PunkteRechts = 0}
        Dim satz2 = New Satz With {.PunkteLinks = 11, .PunkteRechts = 0}
        Dim satz3 = New Satz With {.PunkteLinks = 0, .PunkteRechts = 11}
        Dim satz4 = New Satz With {.PunkteLinks = 8, .PunkteRechts = 0}
        _P.Add(satz1)
        _P.Add(satz2)
        _P.Add(satz3)
        _P.Add(satz4)
        Assert.That(s.MeineGewonnenenSätze(_P, _SpielerA), [Is].EqualTo(2))
    End Sub

    <Test>
    Public Sub MeineVerlorenenSätze_geben_abgeschlossene_Sätze_zurück()
        Dim s = New Spielstand(3)
        Dim satz1 = New Satz With {.PunkteLinks = 11, .PunkteRechts = 0}
        Dim satz2 = New Satz With {.PunkteLinks = 11, .PunkteRechts = 0}
        Dim satz3 = New Satz With {.PunkteLinks = 0, .PunkteRechts = 11}
        Dim satz4 = New Satz With {.PunkteLinks = 8, .PunkteRechts = 0}
        _P.Add(satz1)
        _P.Add(satz2)
        _P.Add(satz3)
        _P.Add(satz4)
        Assert.That(s.MeineVerlorenenSätze(_P, _SpielerB), [Is].EqualTo(2))
    End Sub

    <Test>
    Public Sub HatPartieGewonnen_false_wenn_Spieler_nicht_Teil_der_Partie()
        Dim s = New Spielstand(3)
        Assert.That(s.HatPartieGewonnen(_P, New SpielerInfo("C")), [Is].False)
    End Sub

    <Test>
    Public Sub HatPartieGewonnen_true_wenn_Spieler_hat_alle_Gewinnsätze()
        Dim s = New Spielstand(3)
        Dim satz1 = New Satz With {.PunkteLinks = 11, .PunkteRechts = 0}
        _P.Add(satz1)
        _P.Add(satz1)
        _P.Add(satz1)
        Assert.That(s.HatPartieGewonnen(_P, _SpielerA), [Is].True)
    End Sub
End Class

