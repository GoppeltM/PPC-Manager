Public Class SpielPartieTests

    <Test>
    Sub SpielerLinks_GleichSpielerRechts_GleichSpielerA()
        Dim SpielerA = New SpielerInfo("PLAYER293") With {.Vorname = "Florian", .Nachname = "Ewald"}
        Dim Partie = New FreiLosSpiel("Runde 1", SpielerA)
        Assert.AreEqual(Partie.SpielerLinks, SpielerA)
        Assert.AreEqual(Partie.SpielerRechts, SpielerA)
    End Sub

    <Test>
    Sub Unterschiedliche_Partien_sind_ungleich()
        Dim partieA = New SpielPartie("Runde 1", New SpielerInfo("A"), New SpielerInfo("B"))
        Dim partieB = New SpielPartie("Runde 2", New SpielerInfo("A"), New SpielerInfo("B"))
        Assert.That(partieA <> partieB)
    End Sub

    <Test>
    Sub Gleiche_Partien_sind_gleich()
        Dim partieA = New SpielPartie("Runde 1", New SpielerInfo("A"), New SpielerInfo("B"))
        Dim partieB = New SpielPartie("Runde 1", New SpielerInfo("A"), New SpielerInfo("B"))
        Assert.That(partieA, [Is].EqualTo(partieB))
    End Sub

End Class
