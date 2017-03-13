Public Class SpielverlaufTests

    <Test>
    Public Sub Habengegeneinandergespielt_ist_falsch_wenn_spieler_noch_nie_gespielt_haben()
        Dim spielerA = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "A"}
        Dim spielerB = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "B"}
        Dim spielerC = New Spieler(New SpielRunden, New SpielRegeln(3, True, True)) With {.Id = "C"}
        Dim partien = {New SpielPartie("Runde 1", spielerA, spielerB, 3)}
        Dim s As New Spielverlauf(partien)
        Dim result = s.Habengegeneinandergespielt(spielerA, spielerC)
        Dim result2 = s.Habengegeneinandergespielt(spielerC, spielerA)
        Assert.That(result, [Is].False)
        Assert.That(result2, [Is].False)
    End Sub
End Class

