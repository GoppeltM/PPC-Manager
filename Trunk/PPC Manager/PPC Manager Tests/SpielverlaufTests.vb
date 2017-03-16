Public Class SpielverlaufTests

    <Test>
    Public Sub Habengegeneinandergespielt_ist_falsch_wenn_spieler_noch_nie_gespielt_haben()
        Dim spielerA = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "A"}
        Dim spielerB = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "B"}
        Dim spielerC = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "C"}
        Dim partien = {New SpielPartie("Runde 1", spielerA, spielerB, 3)}
        Dim s As New Spielverlauf(partien, 3)
        Dim result = s.Habengegeneinandergespielt(spielerA, spielerC)
        Dim result2 = s.Habengegeneinandergespielt(spielerC, spielerA)
        Assert.That(result, [Is].False)
        Assert.That(result2, [Is].False)
    End Sub

    <Test>
    Public Sub Habengegeneinandergespielt_ist_wahr_wenn_spieler_gegeneinander_gespielt_haben()
        Dim spielerA = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "A"}
        Dim spielerB = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "B"}
        Dim spielerC = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "C"}
        Dim partien = {New SpielPartie("Runde 1", spielerA, spielerB, 3)}
        Dim s As New Spielverlauf(partien, 3)
        Dim result = s.Habengegeneinandergespielt(spielerA, spielerB)
        Assert.That(result, [Is].True)
    End Sub

    <Test>
    Public Sub HatFreilos_ist_wahr_wenn_Freilos_von_Spieler_in_Spielpartien()
        Dim spielerA = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "A"}
        Dim partien = {New FreiLosSpiel("Runde 1", spielerA, 3)}
        Dim s As New Spielverlauf(partien, 3)
        Dim result = s.HatFreilos(spielerA)
        Assert.That(result, Iz.True)
    End Sub

    <Test>
    Public Sub HatFreilos_ist_falsch_wenn_Freilos_anderen_Spieler_hat()
        Dim spielerA = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "A"}
        Dim spielerB = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "B"}
        Dim partien = {New FreiLosSpiel("Runde 1", spielerA, 3), New SpielPartie("Runde 1", spielerA, spielerB, 3)}
        Dim s As New Spielverlauf(partien, 3)
        Dim result = s.HatFreilos(spielerB)
        Assert.That(result, Iz.False)
    End Sub

End Class

